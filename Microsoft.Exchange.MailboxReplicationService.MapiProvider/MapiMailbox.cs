using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MapiMailbox : MailboxProviderBase, IMailbox, IDisposable
	{
		public MapiMailbox(LocalMailboxFlags flags) : base(flags)
		{
			this.createdFromMapiStore = false;
			this.syncStateMessageIds = new Dictionary<byte[], byte[]>();
			this.HTTPProxyServerName = null;
			this.inTransitStatus = InTransitStatus.NotInTransit;
			this.StoreSupportsOnlineMove = true;
			this.store = null;
		}

		internal MapiMailbox(MapiStore mapiStore) : base(LocalMailboxFlags.None)
		{
			this.MapiStore = mapiStore;
			this.createdFromMapiStore = true;
		}

		public MapiStore MapiStore
		{
			get
			{
				return this.store;
			}
			protected set
			{
				this.store = value;
			}
		}

		public string UserLegDN { get; private set; }

		public override int ServerVersion
		{
			get
			{
				if (this.MapiStore != null)
				{
					return new ServerVersion(this.MapiStore.VersionMajor, this.MapiStore.VersionMinor, this.MapiStore.BuildMajor, this.MapiStore.BuildMinor).ToInt();
				}
				return this.serverVersion;
			}
			protected set
			{
				this.serverVersion = value;
			}
		}

		public string HTTPProxyServerName { get; private set; }

		public bool StoreSupportsOnlineMove { get; private set; }

		public InTransitStatus InTransitStatus
		{
			get
			{
				return this.inTransitStatus;
			}
			set
			{
				bool flag;
				((IMailbox)this).SetInTransitStatus(value, out flag);
			}
		}

		public void ConfigRPCHTTP(string mailboxLegDN, string userLegDN, string serverDN, string httpProxyServerName, NetworkCredential cred, bool credentialIsAdmin, bool useNTLMAuth)
		{
			base.MailboxDN = mailboxLegDN;
			this.UserLegDN = userLegDN;
			base.ServerDN = serverDN;
			this.HTTPProxyServerName = ((!string.IsNullOrEmpty(httpProxyServerName)) ? httpProxyServerName : null);
			base.Credential = cred;
			if (!credentialIsAdmin)
			{
				base.Flags |= LocalMailboxFlags.CredentialIsNotAdmin;
			}
			if (useNTLMAuth)
			{
				base.Flags |= LocalMailboxFlags.UseNTLMAuth;
			}
			base.ServerDisplayName = DNConvertor.ServerNameFromServerLegacyDN(base.ServerDN);
			if (string.IsNullOrEmpty(base.ServerDisplayName))
			{
				base.ServerDisplayName = httpProxyServerName;
			}
			base.ServerGuid = Guid.Empty;
		}

		public MapiProp OpenMapiEntry(byte[] folderId, byte[] entryId, OpenEntryFlags flags)
		{
			MapiProp result;
			using (base.RHTracker.Start())
			{
				if (!base.IsPublicFolderMigrationSource)
				{
					result = (MapiProp)this.MapiStore.OpenEntry(entryId, flags);
				}
				else
				{
					byte[] key = folderId ?? MailboxProviderBase.NullFolderKey;
					if (this.folderSessions == null)
					{
						this.folderSessions = new EntryIdMap<MapiStore>();
					}
					MapiStore mapiStore;
					if (this.folderSessions.TryGetValue(key, out mapiStore))
					{
						result = (MapiProp)mapiStore.OpenEntry(entryId, flags);
					}
					else
					{
						MapiFolder mapiFolder = null;
						try
						{
							mapiFolder = (MapiFolder)this.MapiStore.OpenEntry(folderId, flags);
							if (mapiFolder == null)
							{
								result = null;
							}
							else
							{
								if (mapiFolder.IsContentAvailable)
								{
									mapiStore = this.MapiStore;
								}
								else
								{
									MrsTracer.Provider.Debug("Folder is not available on server '{0}', will try replicas.", new object[]
									{
										base.ServerDN
									});
									string[] array = null;
									try
									{
										array = mapiFolder.GetReplicaServers();
									}
									catch (MapiExceptionNoReplicaAvailable mapiExceptionNoReplicaAvailable)
									{
										MrsTracer.Provider.Error("Exception encountered while loading replicas for folder '{0}': {1}", new object[]
										{
											TraceUtils.DumpEntryId(folderId),
											mapiExceptionNoReplicaAvailable
										});
									}
									if (array == null || array.Length == 0)
									{
										MrsTracer.Provider.Error("Folder {0} does not have any replicas.", new object[]
										{
											TraceUtils.DumpEntryId(folderId)
										});
										mapiStore = this.MapiStore;
									}
									else
									{
										mapiStore = this.FindStoreSession(array);
									}
								}
								this.folderSessions.Add(key, mapiStore);
								MapiProp mapiProp;
								if (CommonUtils.IsSameEntryId(folderId, entryId) && mapiStore == this.MapiStore)
								{
									mapiProp = mapiFolder;
									mapiFolder = null;
								}
								else
								{
									mapiProp = (MapiProp)mapiStore.OpenEntry(entryId, flags);
								}
								result = mapiProp;
							}
						}
						finally
						{
							if (mapiFolder != null)
							{
								mapiFolder.Dispose();
							}
						}
					}
				}
			}
			return result;
		}

		public override SyncProtocol GetSyncProtocol()
		{
			return SyncProtocol.None;
		}

		bool IMailbox.IsConnected()
		{
			return this.connectedWithoutMailboxSession || this.MapiStore != null;
		}

		bool IMailbox.IsMailboxCapabilitySupported(MailboxCapabilities capability)
		{
			if (capability == MailboxCapabilities.PagedEnumerateChanges)
			{
				return !base.IsTitanium;
			}
			switch (capability)
			{
			case MailboxCapabilities.FolderRules:
				return !(this is MapiDestinationMailbox) || this.ServerVersion >= Server.E14MinVersion;
			case MailboxCapabilities.FolderAcls:
				return true;
			default:
				return false;
			}
		}

		MailboxInformation IMailbox.GetMailboxInformation()
		{
			MrsTracer.Provider.Function("MapiMailbox.GetMailboxInformation", new object[0]);
			MailboxInformation mailboxInformation = new MailboxInformation();
			if (!base.IsPureMAPI)
			{
				if (base.IsPublicFolderMigrationSource)
				{
					PublicFolderDatabase publicFolderDatabase = base.FindDatabaseByGuid<PublicFolderDatabase>(base.MdbGuid);
					mailboxInformation.MdbGuid = base.MdbGuid;
					mailboxInformation.MdbName = publicFolderDatabase.Identity.ToString();
					mailboxInformation.MdbLegDN = publicFolderDatabase.ExchangeLegacyDN;
				}
				else
				{
					MailboxDatabase mailboxDatabase = base.FindDatabaseByGuid<MailboxDatabase>(base.MdbGuid);
					mailboxInformation.MailboxGuid = base.MailboxGuid;
					mailboxInformation.MdbGuid = base.MdbGuid;
					mailboxInformation.MdbName = mailboxDatabase.Identity.ToString();
					mailboxInformation.MdbLegDN = mailboxDatabase.ExchangeLegacyDN;
					mailboxInformation.MdbQuota = (mailboxDatabase.ProhibitSendReceiveQuota.IsUnlimited ? null : new ulong?(mailboxDatabase.ProhibitSendReceiveQuota.Value.ToBytes()));
					mailboxInformation.MdbDumpsterQuota = (mailboxDatabase.RecoverableItemsQuota.IsUnlimited ? null : new ulong?(mailboxDatabase.RecoverableItemsQuota.Value.ToBytes()));
				}
			}
			mailboxInformation.ServerVersion = this.ServerVersion;
			mailboxInformation.ServerMailboxRelease = base.ServerMailboxRelease.ToString();
			mailboxInformation.ProviderName = "MapiProvider";
			mailboxInformation.RecipientType = this.recipientType;
			mailboxInformation.RecipientDisplayType = this.recipientDisplayType;
			mailboxInformation.RecipientTypeDetailsLong = this.recipientTypeDetails;
			mailboxInformation.MailboxHomeMdbGuid = base.MbxHomeMdbGuid;
			mailboxInformation.ArchiveGuid = this.archiveGuid;
			mailboxInformation.AlternateMailboxes = this.alternateMailboxes;
			mailboxInformation.UseMdbQuotaDefaults = this.useMdbQuotaDefaults;
			mailboxInformation.MailboxQuota = this.mbxQuota;
			mailboxInformation.MailboxDumpsterQuota = this.mbxDumpsterQuota;
			mailboxInformation.MailboxArchiveQuota = this.mbxArchiveQuota;
			mailboxInformation.MailboxIdentity = ((base.MailboxId != null) ? base.MailboxId.ToString() : null);
			mailboxInformation.MailboxItemCount = 0UL;
			mailboxInformation.MailboxSize = 0UL;
			mailboxInformation.RegularItemCount = 0UL;
			mailboxInformation.RegularDeletedItemCount = 0UL;
			mailboxInformation.AssociatedItemCount = 0UL;
			mailboxInformation.AssociatedDeletedItemCount = 0UL;
			mailboxInformation.RegularItemsSize = 0UL;
			mailboxInformation.RegularDeletedItemsSize = 0UL;
			mailboxInformation.AssociatedItemsSize = 0UL;
			mailboxInformation.AssociatedDeletedItemsSize = 0UL;
			mailboxInformation.RulesSize = 0;
			if (this.MapiStore != null)
			{
				PropValue[] props;
				using (base.RHTracker.Start())
				{
					props = this.MapiStore.GetProps(MailboxProviderBase.MailboxInformationPropertyTags);
				}
				for (int i = 0; i < props.Length; i++)
				{
					object value = props[i].Value;
					if (value != null)
					{
						MailboxProviderBase.PopulateMailboxInformation(mailboxInformation, props[i].PropTag, value);
					}
				}
				if (!base.IsPublicFolderMigrationSource && !base.IsPublicFolderMailbox)
				{
					using (base.RHTracker.Start())
					{
						using (MapiFolder inboxFolder = this.MapiStore.GetInboxFolder())
						{
							if (inboxFolder != null)
							{
								PropValue[] props2 = inboxFolder.GetProps(MapiMailbox.InboxProperties);
								PropValue propValue = props2[0];
								if (!propValue.IsNull() && !propValue.IsError())
								{
									mailboxInformation.RulesSize = propValue.GetInt();
								}
								PropValue propValue2 = props2[1];
								if (!propValue2.IsNull() && !propValue2.IsError())
								{
									mailboxInformation.ContentAggregationFlags = propValue2.GetInt();
								}
								else
								{
									mailboxInformation.ContentAggregationFlags = 0;
								}
							}
						}
					}
				}
			}
			if (!base.IsPureMAPI && !base.IsPublicFolderMigrationSource)
			{
				using (ExRpcAdmin rpcAdmin = base.GetRpcAdmin())
				{
					using (base.RHTracker.Start())
					{
						mailboxInformation.MailboxTableFlags = (int)MapiUtils.GetMailboxTableFlags(rpcAdmin, base.MdbGuid, base.MailboxGuid);
					}
				}
			}
			return mailboxInformation;
		}

		void IMailbox.Connect(MailboxConnectFlags connectFlags)
		{
			base.CreateStoreSession(connectFlags, delegate
			{
				this.store = this.CreateStoreConnection(this.ServerDN, this.ServerFqdn, connectFlags);
			});
		}

		public override void Disconnect()
		{
			base.CheckDisposed();
			lock (this)
			{
				base.Disconnect();
				if (this.additionalSessions != null)
				{
					foreach (MapiStore mapiStore in this.additionalSessions.Values)
					{
						mapiStore.Dispose();
					}
					this.additionalSessions.Clear();
				}
				if (this.folderSessions != null)
				{
					this.folderSessions.Clear();
				}
				this.inTransitStatus = InTransitStatus.NotInTransit;
				if (this.store != null)
				{
					if (this.createdFromMapiStore)
					{
						MrsTracer.Provider.Debug("Not disconnecting as the object was created from the MapiStore.", new object[0]);
					}
					else
					{
						MrsTracer.Provider.Debug("Disconnecting from server \"{0}\", mailbox \"{1}\".", new object[]
						{
							base.ServerDN,
							base.TraceMailboxId
						});
						this.store.Dispose();
					}
					this.store = null;
				}
			}
		}

		void IMailbox.SetInTransitStatus(InTransitStatus status, out bool onlineMoveSupported)
		{
			if (this.inTransitStatus == status)
			{
				onlineMoveSupported = this.StoreSupportsOnlineMove;
				return;
			}
			this.SetInTransitStatus(this.MapiStore, status, out onlineMoveSupported);
			if (base.IsPublicFolderMigrationSource)
			{
				if (!onlineMoveSupported)
				{
					throw new OfflinePublicFolderMigrationNotSupportedException();
				}
				if (this.additionalSessions != null)
				{
					foreach (MapiStore mapiStore in this.additionalSessions.Values)
					{
						this.SetInTransitStatus(mapiStore, status, out onlineMoveSupported);
						if (!onlineMoveSupported)
						{
							throw new OfflinePublicFolderMigrationNotSupportedException();
						}
					}
				}
			}
		}

		List<FolderRec> IMailbox.EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags flags, PropTag[] additionalPtagsToLoad)
		{
			MrsTracer.Provider.Function("MapiMailbox.EnumerateFolderHierarchy({0})", new object[]
			{
				flags
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			List<FolderRec> list = new List<FolderRec>(50);
			using (base.RHTracker.Start())
			{
				if (!flags.HasFlag(EnumerateFolderHierarchyFlags.WellKnownPublicFoldersOnly))
				{
					this.LoadFolderHierarchy(null, additionalPtagsToLoad, list);
				}
				else
				{
					using (MapiFolder mapiFolder = (MapiFolder)this.MapiStore.OpenEntry(null))
					{
						using (MapiFolder mapiFolder2 = (MapiFolder)this.MapiStore.OpenEntry((byte[])this.MapiStore.GetProp(PropTag.DeferredActionFolderEntryID).Value))
						{
							using (MapiFolder mapiFolder3 = (MapiFolder)this.MapiStore.OpenEntry((byte[])this.MapiStore.GetProp(PropTag.SpoolerQueueEntryId).Value))
							{
								using (MapiFolder mapiFolder4 = (MapiFolder)this.MapiStore.OpenEntry((byte[])this.MapiStore.GetProp(PropTag.IpmSubtreeEntryId).Value))
								{
									using (MapiFolder mapiFolder5 = (MapiFolder)this.MapiStore.OpenEntry((byte[])this.MapiStore.GetProp(PropTag.IpmSentMailEntryId).Value))
									{
										using (MapiFolder mapiFolder6 = (MapiFolder)this.MapiStore.OpenEntry((byte[])this.MapiStore.GetProp(PropTag.IpmInboxEntryId).Value))
										{
											using (MapiFolder mapiFolder7 = (MapiFolder)this.MapiStore.OpenEntry((byte[])this.MapiStore.GetProp(PropTag.IpmWasteBasketEntryId).Value))
											{
												using (MapiFolder mapiFolder8 = (MapiFolder)this.MapiStore.OpenEntry((byte[])this.MapiStore.GetProp(PropTag.IpmOutboxEntryId).Value))
												{
													list.Add(FolderRec.Create(mapiFolder, additionalPtagsToLoad));
													list.Add(FolderRec.Create(mapiFolder2, additionalPtagsToLoad));
													list.Add(FolderRec.Create(mapiFolder3, additionalPtagsToLoad));
													list.Add(FolderRec.Create(mapiFolder4, additionalPtagsToLoad));
													list.Add(FolderRec.Create(mapiFolder5, additionalPtagsToLoad));
													list.Add(FolderRec.Create(mapiFolder6, additionalPtagsToLoad));
													list.Add(FolderRec.Create(mapiFolder7, additionalPtagsToLoad));
													list.Add(FolderRec.Create(mapiFolder8, additionalPtagsToLoad));
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			MrsTracer.Provider.Debug("Loaded {0} folders", new object[]
			{
				list.Count
			});
			return list;
		}

		NamedPropData[] IMailbox.GetNamesFromIDs(PropTag[] pta)
		{
			MrsTracer.Provider.Function("MapiMailbox.GetNamedFromIDs", new object[0]);
			base.CheckDisposed();
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			List<NamedProp> result = new List<NamedProp>(pta.Length);
			using (base.RHTracker.Start())
			{
				MapiUtils.ProcessMapiCallInBatches<PropTag>(pta, delegate(PropTag[] batch)
				{
					result.AddRange(this.MapiStore.GetNamesFromIDs(batch));
				});
			}
			return DataConverter<NamedPropConverter, NamedProp, NamedPropData>.GetData(result.ToArray());
		}

		PropTag[] IMailbox.GetIDsFromNames(bool createIfNotExists, NamedPropData[] npda)
		{
			MrsTracer.Provider.Function("MapiMailbox.GetIDsFromNames", new object[0]);
			base.CheckDisposed();
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			NamedProp[] native = DataConverter<NamedPropConverter, NamedProp, NamedPropData>.GetNative(npda);
			List<PropTag> result = new List<PropTag>(npda.Length);
			using (base.RHTracker.Start())
			{
				MapiUtils.ProcessMapiCallInBatches<NamedProp>(native, delegate(NamedProp[] batch)
				{
					result.AddRange(this.MapiStore.GetIDsFromNames(createIfNotExists, batch));
				});
			}
			return result.ToArray();
		}

		byte[] IMailbox.GetSessionSpecificEntryId(byte[] entryId)
		{
			MrsTracer.Provider.Function("MapiMailbox.GetSessionSpecificEntryId", new object[0]);
			base.CheckDisposed();
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			long fid;
			if (entryId.Length == 22)
			{
				fid = this.MapiStore.IdFromGlobalId(entryId);
			}
			else
			{
				fid = this.MapiStore.GetFidFromEntryId(entryId);
			}
			return this.MapiStore.CreateEntryId(fid);
		}

		void IMailbox.AddMoveHistoryEntry(MoveHistoryEntryInternal mhei, int maxMoveHistoryLength)
		{
			MrsTracer.Provider.Function("MapiMailbox.AddMoveHistoryEntry", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			using (base.RHTracker.Start())
			{
				mhei.SaveToMailbox(this.MapiStore, maxMoveHistoryLength);
			}
		}

		PropValueData[] IMailbox.GetProps(PropTag[] ptags)
		{
			MrsTracer.Provider.Function("MapiMailbox.GetProps", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			PropValue[] props;
			using (base.RHTracker.Start())
			{
				props = this.MapiStore.GetProps(ptags);
			}
			return DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(props);
		}

		byte[] IMailbox.GetReceiveFolderEntryId(string msgClass)
		{
			MrsTracer.Provider.Function("MapiMailbox.GetReceiveFolderEntryId", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			if (base.IsPublicFolderMigrationSource)
			{
				return null;
			}
			byte[] receiveFolderEntryId;
			using (base.RHTracker.Start())
			{
				receiveFolderEntryId = this.MapiStore.GetReceiveFolderEntryId(msgClass);
			}
			return receiveFolderEntryId;
		}

		string IMailbox.LoadSyncState(byte[] key)
		{
			MrsTracer.Provider.Function("MapiMailbox.LoadSyncState", new object[0]);
			MapiStore mapiStore = null;
			bool flag = false;
			string result;
			try
			{
				using (base.RHTracker.Start())
				{
					if (base.IsPureMAPI || base.UseHomeMDB || base.IsPublicFolderMove)
					{
						mapiStore = this.MapiStore;
					}
					else
					{
						mapiStore = this.OpenSystemMailbox();
						flag = true;
					}
					using (MoveObjectInfo<string> syncStateMOI = this.GetSyncStateMOI(mapiStore, key))
					{
						string text = syncStateMOI.ReadObject(ReadObjectFlags.DontThrowOnCorruptData);
						if (text == null)
						{
							MrsTracer.Provider.Debug("Sync state does not exist.", new object[0]);
						}
						else
						{
							this.syncStateMessageIds[key] = syncStateMOI.MessageId;
						}
						result = text;
					}
				}
			}
			finally
			{
				if (flag && mapiStore != null)
				{
					mapiStore.Dispose();
				}
			}
			return result;
		}

		MessageRec IMailbox.SaveSyncState(byte[] key, string syncStateStr)
		{
			MrsTracer.Provider.Function("MapiMailbox.SaveSyncState", new object[0]);
			MapiStore mapiStore = null;
			bool flag = false;
			MessageRec result = null;
			try
			{
				using (base.RHTracker.Start())
				{
					if (base.IsPureMAPI || base.UseHomeMDB || base.IsPublicFolderMove)
					{
						mapiStore = this.MapiStore;
					}
					else
					{
						mapiStore = this.OpenSystemMailbox();
						flag = true;
					}
					using (MoveObjectInfo<string> syncStateMOI = this.GetSyncStateMOI(mapiStore, key))
					{
						syncStateMOI.OpenMessage();
						if (syncStateStr != null)
						{
							syncStateMOI.SaveObject(syncStateStr);
							this.syncStateMessageIds[key] = syncStateMOI.MessageId;
							result = new MessageRec(syncStateMOI.MessageId, syncStateMOI.FolderId, DateTime.MinValue, 0, MsgRecFlags.None, null);
						}
						else
						{
							if (syncStateMOI.MessageFound)
							{
								syncStateMOI.DeleteMessage();
							}
							this.syncStateMessageIds[key] = null;
						}
					}
				}
			}
			finally
			{
				if (flag && mapiStore != null)
				{
					mapiStore.Dispose();
				}
			}
			return result;
		}

		SessionStatistics IMailbox.GetSessionStatistics(SessionStatisticsFlags statisticsTypes)
		{
			return new SessionStatistics();
		}

		Guid IMailbox.StartIsInteg(List<uint> mailboxCorruptionTypes)
		{
			throw new NotImplementedException();
		}

		List<StoreIntegrityCheckJob> IMailbox.QueryIsInteg(Guid isIntegRequestGuid)
		{
			throw new NotImplementedException();
		}

		protected override void AfterConnect()
		{
			this.syncStateMessageIds = new Dictionary<byte[], byte[]>();
			base.AfterConnect();
		}

		protected T GetFolder<T>(byte[] folderId) where T : MapiFolder, new()
		{
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			T result;
			using (base.RHTracker.Start())
			{
				MapiFolder mapiFolder = (MapiFolder)this.OpenMapiEntry(folderId, folderId, this.GetFolderOpenEntryFlags());
				if (mapiFolder == null)
				{
					MrsTracer.Provider.Debug("Folder does not exist", new object[0]);
					result = default(T);
				}
				else
				{
					if (MrsTracer.Provider.IsEnabled(TraceType.DebugTrace))
					{
						string @string = mapiFolder.GetProp(PropTag.DisplayName).GetString();
						MrsTracer.Provider.Debug("Opened folder '{0}'", new object[]
						{
							@string
						});
					}
					T t = Activator.CreateInstance<T>();
					t.Config(folderId, mapiFolder, this);
					result = t;
				}
			}
			return result;
		}

		protected MapiStore OpenSystemMailbox()
		{
			MrsTracer.Provider.Function("MapiMailbox.OpenSystemMailbox", new object[0]);
			base.CheckDisposed();
			if (base.IsPureMAPI)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			MapiStore systemMailbox;
			using (base.RHTracker.Start())
			{
				systemMailbox = MapiUtils.GetSystemMailbox(base.MdbGuid, base.ConfigDomainControllerName, base.Credential);
			}
			return systemMailbox;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			base.InternalDispose(calledFromDispose);
			if (calledFromDispose)
			{
				((IMailbox)this).Disconnect();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiMailbox>(this);
		}

		protected abstract Exception GetMailboxInTransitException(Exception innerException);

		protected abstract OpenEntryFlags GetFolderOpenEntryFlags();

		protected IConfigurationSession GetSystemConfigurationSession(bool readOnly, bool tenantScoped)
		{
			if (base.IsPureMAPI)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			ADSessionSettings sessionSettings;
			if (tenantScoped && base.PartitionHint != null)
			{
				sessionSettings = ADSessionSettings.FromTenantPartitionHint(base.PartitionHint);
			}
			else
			{
				sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.ConfigDomainControllerName, readOnly, ConsistencyMode.PartiallyConsistent, base.Credential, sessionSettings, 1140, "GetSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Provider\\MapiProvider\\MapiMailbox.cs");
			if (!tenantScoped)
			{
				tenantOrTopologyConfigurationSession.EnforceDefaultScope = false;
			}
			return tenantOrTopologyConfigurationSession;
		}

		protected virtual MapiStore CreateStoreConnection(string serverLegDN, string serverFqdn, MailboxConnectFlags mailboxConnectFlags)
		{
			MrsTracer.Provider.Function("MapiMailbox.CreateStoreConnection", new object[0]);
			base.CheckDisposed();
			if (!base.IsPureMAPI && !base.IsRestore && !base.IsPublicFolderMigrationSource)
			{
				Guid guid = base.IsArchiveMailbox ? base.MbxArchiveMdbGuid : base.MbxHomeMdbGuid;
				if (mailboxConnectFlags.HasFlag(MailboxConnectFlags.NonMrsLogon) && base.TestIntegration.GetIntValueAndDecrement("InjectNFaultsTargetConnectivityVerification", 0, 0, 2147483647) > 0)
				{
					guid = new Guid("F00DBABE-0000-0000-0000-000000000000");
				}
				if (base.MdbGuid != guid)
				{
					MrsTracer.Provider.Warning("Source mailbox does not exist or is in the wrong database.", new object[0]);
					throw new MailboxIsNotInExpectedMDBPermanentException(guid);
				}
			}
			if (base.IsPublicFolderMigrationSource)
			{
				if (base.IsPureMAPI)
				{
				}
			}
			else
			{
				bool isRestore = base.IsRestore;
			}
			OpenStoreFlag openStoreFlag;
			ConnectFlag connectFlag;
			this.GetConnectionFlags(out openStoreFlag, out connectFlag);
			base.VerifyRestoreSource(mailboxConnectFlags);
			bool flag = false;
			string userName;
			string domainName;
			string password;
			base.GetCreds(out userName, out domainName, out password);
			TimeSpan timeSpan;
			TimeSpan callTimeout;
			if (base.IsPureMAPI || base.Credential != null)
			{
				timeSpan = base.TestIntegration.RemoteMailboxConnectionTimeout;
				callTimeout = base.TestIntegration.RemoteMailboxCallTimeout;
			}
			else
			{
				timeSpan = base.TestIntegration.LocalMailboxConnectionTimeout;
				callTimeout = base.TestIntegration.LocalMailboxCallTimeout;
			}
			string text = ((mailboxConnectFlags & MailboxConnectFlags.NonMrsLogon) != MailboxConnectFlags.None) ? "Client=Management" : "Client=MSExchangeMigration";
			if ((mailboxConnectFlags & MailboxConnectFlags.PublicFolderHierarchyReplication) != MailboxConnectFlags.None)
			{
				text = "Client=PublicFolderSystem;Action=PublicFolderHierarchyReplication";
			}
			byte[] tenantPartitionHint = (base.PartitionHint != null) ? TenantPartitionHint.Serialize(base.PartitionHint) : null;
			MapiStore result;
			for (;;)
			{
				string text2 = (!string.IsNullOrEmpty(serverFqdn)) ? serverFqdn : serverLegDN;
				bool flag2 = (connectFlag & ConnectFlag.ConnectToExchangeRpcServerOnly) != ConnectFlag.None;
				CultureInfo cultureInfo;
				if (base.IsPureMAPI)
				{
					cultureInfo = CultureInfo.InvariantCulture;
				}
				else
				{
					cultureInfo = null;
					openStoreFlag |= OpenStoreFlag.NoLocalization;
				}
				MrsTracer.Provider.Debug("Opening MapiStore: serverDN=\"{0}\", mailbox=\"{1}\", connectFlags=[{2}], openStoreFlags=[{3}], timeout={4}", new object[]
				{
					text2,
					base.TraceMailboxId,
					connectFlag,
					openStoreFlag,
					timeSpan
				});
				result = null;
				try
				{
					using (base.RHTracker.Start())
					{
						if (base.IsPublicFolderMigrationSource)
						{
							if (base.IsPureMAPI)
							{
								result = MapiStore.OpenPublicStore(text2, base.MailboxDN, userName, domainName, password, this.HTTPProxyServerName, connectFlag, openStoreFlag, cultureInfo, null, text, timeSpan, callTimeout);
							}
							else
							{
								result = MapiStore.OpenPublicStore(text2, Guid.Empty, Server.GetSystemAttendantLegacyDN(serverLegDN), userName, domainName, password, connectFlag, openStoreFlag, cultureInfo, null, text, timeSpan, callTimeout);
							}
						}
						else if (base.IsRestore)
						{
							result = MapiStore.OpenMailbox(text2, Server.GetSystemAttendantLegacyDN(serverLegDN), base.MailboxGuid, base.MdbGuid, userName, domainName, password, connectFlag, openStoreFlag, cultureInfo, null, text, timeSpan, callTimeout, tenantPartitionHint);
						}
						else if (base.IsPureMAPI || base.IsTitanium || flag2)
						{
							string text3 = base.MailboxDN;
							string userDn = text3;
							if (base.IsPureMAPI && !string.IsNullOrEmpty(this.UserLegDN))
							{
								userDn = this.UserLegDN;
							}
							if (base.IsArchiveMailbox)
							{
								text3 = text3 + "/guid=" + this.archiveGuid.ToString();
							}
							result = MapiStore.OpenMailbox(text2, userDn, text3, userName, domainName, password, this.HTTPProxyServerName, connectFlag, openStoreFlag, cultureInfo, null, text, timeSpan, callTimeout);
						}
						else
						{
							openStoreFlag |= OpenStoreFlag.MailboxGuid;
							result = MapiStore.OpenMailbox(text2, base.MailboxDN, base.MailboxGuid, base.MdbGuid, userName, domainName, password, connectFlag, openStoreFlag, cultureInfo, null, text, timeSpan, callTimeout, tenantPartitionHint);
						}
						MapiUtils.StartMapiDeadSessionChecking(result, base.TraceMailboxId);
					}
				}
				catch (MapiExceptionNotFound originalException)
				{
					base.VerifyMdbIsOnline(originalException);
					throw;
				}
				catch (MapiExceptionLogonFailed originalException2)
				{
					if (!base.IsPureMAPI && !flag)
					{
						MrsTracer.Provider.Debug("OpenMailbox returned LogonFailed, forcing AM rediscovery", new object[0]);
						base.ResolveMDB(true);
						serverLegDN = base.ServerDN;
						serverFqdn = base.ServerFqdn;
						flag = true;
						continue;
					}
					base.VerifyMdbIsOnline(originalException2);
					throw;
				}
				catch (MapiExceptionWrongServer)
				{
					if (!base.IsPureMAPI && !flag)
					{
						MrsTracer.Provider.Debug("OpenMailbox returned WrongServer, forcing AM rediscovery", new object[0]);
						base.ResolveMDB(true);
						serverLegDN = base.ServerDN;
						serverFqdn = base.ServerFqdn;
						flag = true;
						continue;
					}
					throw;
				}
				catch (MapiExceptionNetworkError mapiExceptionNetworkError)
				{
					if (base.IsPureMAPI)
					{
						if (base.IsPublicFolderMigrationSource)
						{
							if (connectFlag.HasFlag(ConnectFlag.PublicFolderMigration))
							{
								MrsTracer.Provider.Debug("PureMAPI OpenPublicStore returned ecNetworkError {0}, retrying without PublicFolderMigration flag", new object[]
								{
									mapiExceptionNetworkError
								});
								connectFlag &= ~ConnectFlag.PublicFolderMigration;
								continue;
							}
						}
						else if (!flag2)
						{
							MrsTracer.Provider.Debug("PureMAPI OpenMailbox returned ecNetworkError {0}, retrying with MoMT flags", new object[]
							{
								mapiExceptionNetworkError
							});
							connectFlag |= ConnectFlag.ConnectToExchangeRpcServerOnly;
							openStoreFlag |= OpenStoreFlag.NoExtendedFlags;
							continue;
						}
					}
					throw;
				}
				catch (MapiExceptionMailboxInTransit innerException)
				{
					throw this.GetMailboxInTransitException(innerException);
				}
				break;
			}
			return result;
		}

		private void GetConnectionFlags(out OpenStoreFlag osFlags, out ConnectFlag connectFlags)
		{
			osFlags = OpenStoreFlag.TakeOwnership;
			connectFlags = ConnectFlag.None;
			if ((base.Flags & LocalMailboxFlags.CredentialIsNotAdmin) == LocalMailboxFlags.None)
			{
				osFlags |= OpenStoreFlag.UseAdminPrivilege;
				connectFlags |= ConnectFlag.UseAdminPrivilege;
			}
			if (base.IsRestore)
			{
				osFlags |= (OpenStoreFlag.OverrideHomeMdb | OpenStoreFlag.NoLocalization | OpenStoreFlag.MailboxGuid);
				if ((base.RestoreFlags & MailboxRestoreType.Recovery) != MailboxRestoreType.None)
				{
					osFlags |= OpenStoreFlag.RestoreDatabase;
				}
				if ((base.RestoreFlags & MailboxRestoreType.SoftDeleted) != MailboxRestoreType.None || (base.RestoreFlags & MailboxRestoreType.Disabled) != MailboxRestoreType.None)
				{
					osFlags |= OpenStoreFlag.DisconnectedMailbox;
				}
			}
			if (base.Credential == null && !base.IsTitanium && !base.IsExchange2007)
			{
				connectFlags |= ConnectFlag.UseRpcContextPool;
			}
			if (!string.IsNullOrEmpty(this.HTTPProxyServerName))
			{
				connectFlags |= ConnectFlag.UseHTTPS;
				if ((base.Flags & LocalMailboxFlags.UseNTLMAuth) != LocalMailboxFlags.None)
				{
					connectFlags |= ConnectFlag.UseNTLM;
				}
				if (base.IsPublicFolderMigrationSource)
				{
					connectFlags |= ConnectFlag.PublicFolderMigration;
				}
			}
			if (base.IsTitanium || base.IsExchange2007)
			{
				connectFlags |= ConnectFlag.AllowLegacyStore;
			}
			if (base.IsPublicFolderMigrationSource)
			{
				connectFlags |= ConnectFlag.AllowLegacyStore;
				connectFlags &= ~(ConnectFlag.ConnectToExchangeRpcServerOnly | ConnectFlag.UseRpcContextPool);
				osFlags |= OpenStoreFlag.IgnoreHomeMdb;
			}
		}

		private MapiStore FindStoreSession(string[] replicaLegDNs)
		{
			MapiStore mapiStore = null;
			if (this.additionalSessions == null)
			{
				this.additionalSessions = new Dictionary<string, MapiStore>(StringComparer.OrdinalIgnoreCase);
			}
			else
			{
				foreach (string text in replicaLegDNs)
				{
					if (this.additionalSessions.TryGetValue(text, out mapiStore))
					{
						MrsTracer.Provider.Debug("Located an existing store session '{0}'", new object[]
						{
							text
						});
						return mapiStore;
					}
				}
			}
			if (this.additionalSessions.Count + 1 >= base.TestIntegration.MaxOpenConnectionsPerPublicFolderMigration)
			{
				throw new UnexpectedErrorPermanentException(-2147221230);
			}
			for (int j = 0; j < replicaLegDNs.Length; j++)
			{
				string text2 = replicaLegDNs[j];
				bool flag = true;
				try
				{
					MrsTracer.Provider.Debug("Connecting to '{0}'", new object[]
					{
						text2
					});
					mapiStore = this.CreateStoreConnection(text2, null, MailboxConnectFlags.None);
					if (base.Flags.HasFlag(LocalMailboxFlags.ParallelPublicFolderMigration))
					{
						ServerVersion sourceServerVersion = new ServerVersion(mapiStore.VersionMajor, mapiStore.VersionMinor, mapiStore.BuildMajor, mapiStore.BuildMinor);
						ParallelPublicFolderMigrationVersionChecker.ThrowIfMinimumRequiredVersionNotInstalled(sourceServerVersion);
					}
					bool flag2;
					this.SetInTransitStatus(mapiStore, this.inTransitStatus, out flag2);
					if (!flag2)
					{
						throw new OfflinePublicFolderMigrationNotSupportedException();
					}
					this.additionalSessions.Add(text2, mapiStore);
					flag = false;
					return mapiStore;
				}
				catch (MapiExceptionNetworkError ex)
				{
					if (j >= replicaLegDNs.Length - 1)
					{
						throw;
					}
					MrsTracer.Provider.Warning("Failed to connect to '{0}', ignoring.\n{1}", new object[]
					{
						text2,
						CommonUtils.FullExceptionMessage(ex)
					});
				}
				finally
				{
					if (flag && mapiStore != null)
					{
						mapiStore.Dispose();
					}
				}
			}
			throw new UnexpectedErrorPermanentException(-2147221230);
		}

		private void SetInTransitStatus(MapiStore mapiStore, InTransitStatus status, out bool onlineMoveSupported)
		{
			MrsTracer.Provider.Function("MapiMailbox.SetInTransitStatus({0})", new object[]
			{
				status
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			onlineMoveSupported = this.StoreSupportsOnlineMove;
			PropTag propTag = base.IsTitanium ? ((PropTag)1712848907U) : PropTag.InTransitStatus;
			for (;;)
			{
				object value = (int)status;
				if (base.IsTitanium)
				{
					status &= ~InTransitStatus.OnlineMove;
					value = (status != InTransitStatus.NotInTransit);
					this.StoreSupportsOnlineMove = false;
				}
				bool flag = (status & InTransitStatus.OnlineMove) != InTransitStatus.NotInTransit;
				try
				{
					PropValue[] props = new PropValue[]
					{
						new PropValue(propTag, value)
					};
					PropProblem[] array;
					using (base.RHTracker.Start())
					{
						array = mapiStore.SetProps(props);
					}
					if (array != null)
					{
						MrsTracer.Provider.Error("Failed to set InTransitStatus: error {0}", new object[]
						{
							array[0].Scode
						});
						if (array[0].Scode == -2147024891 && status != InTransitStatus.NotInTransit)
						{
							throw this.GetMailboxInTransitException(null);
						}
						throw new UnexpectedErrorPermanentException(array[0].Scode);
					}
				}
				catch (MapiExceptionCorruptData)
				{
					if (!flag)
					{
						throw;
					}
					MrsTracer.Provider.Error("Got MapiExceptionCorruptData, probably a downlevel store. Trying to set offline move status instead.", new object[0]);
					status &= ~InTransitStatus.OnlineMove;
					this.StoreSupportsOnlineMove = false;
					continue;
				}
				break;
			}
			this.inTransitStatus = status;
			onlineMoveSupported = this.StoreSupportsOnlineMove;
		}

		private void LoadFolderHierarchy(byte[] rootFolderEntryId, PropTag[] additionalPtagsToLoad, List<FolderRec> result)
		{
			MrsTracer.Provider.Function("MapiMailbox.LoadFolderHierarchy", new object[0]);
			using (MapiFolder mapiFolder = (MapiFolder)this.MapiStore.OpenEntry(rootFolderEntryId))
			{
				FolderRec folderRec = FolderRec.Create(mapiFolder, additionalPtagsToLoad);
				if (base.IsPublicFolderMigrationSource)
				{
					folderRec.FolderName = "Public Root";
				}
				bool flag = true;
				byte[] nonIpmSubtreeId = null;
				using (MapiTable hierarchyTable = mapiFolder.GetHierarchyTable(HierarchyTableFlags.ConvenientDepth))
				{
					PropTag[] propTags;
					if (additionalPtagsToLoad == null || additionalPtagsToLoad.Length == 0)
					{
						propTags = FolderRec.PtagsToLoad;
					}
					else
					{
						List<PropTag> list = new List<PropTag>();
						list.AddRange(FolderRec.PtagsToLoad);
						list.AddRange(additionalPtagsToLoad);
						propTags = list.ToArray();
					}
					foreach (PropValue[] pva in MapiUtils.QueryAllRows(hierarchyTable, null, propTags, 1000))
					{
						FolderRec folderRec2 = FolderRec.Create(pva);
						if (folderRec2 != null)
						{
							if (CommonUtils.IsSameEntryId(folderRec2.EntryId, folderRec.EntryId))
							{
								flag = false;
								if (base.IsPublicFolderMigrationSource)
								{
									folderRec2.FolderName = "Public Root";
								}
							}
							if (base.IsPublicFolderMigrationSource)
							{
								if (StringComparer.OrdinalIgnoreCase.Equals(folderRec2.FolderName, "NON_IPM_SUBTREE") && CommonUtils.IsSameEntryId(folderRec2.ParentId, folderRec.EntryId))
								{
									nonIpmSubtreeId = folderRec2.EntryId;
								}
								else if (this.ShouldSkipFolder(folderRec2, nonIpmSubtreeId))
								{
									this.publicFoldersToSkip[folderRec2.EntryId] = true;
									goto IL_144;
								}
							}
							result.Add(folderRec2);
						}
						IL_144:;
					}
				}
				if (flag)
				{
					result.Insert(0, folderRec);
				}
			}
		}

		private bool ShouldSkipFolder(FolderRec folderRec, byte[] nonIpmSubtreeId)
		{
			if (this.publicFoldersToSkip.ContainsKey(folderRec.ParentId))
			{
				return true;
			}
			bool flag = MapiMailbox.PublicFolderBranchToSkip.Contains(folderRec.FolderName) || folderRec.FolderName.StartsWith(MapiMailbox.OWAScratchPad, StringComparison.OrdinalIgnoreCase) || folderRec.FolderName.StartsWith(MapiMailbox.StoreEvents, StringComparison.OrdinalIgnoreCase);
			return flag && CommonUtils.IsSameEntryId(nonIpmSubtreeId, folderRec.ParentId);
		}

		private MoveObjectInfo<string> GetSyncStateMOI(MapiStore mbx, byte[] key)
		{
			DateTime utcNow = DateTime.UtcNow;
			byte[] messageId;
			if (!this.syncStateMessageIds.TryGetValue(key, out messageId))
			{
				messageId = null;
			}
			string subject = string.Format(CultureInfo.InvariantCulture, "SyncState: {0}-{1} :: {2} :: {3}", new object[]
			{
				base.MailboxGuid.ToString(),
				TraceUtils.DumpBytes(key),
				utcNow.ToLongDateString(),
				utcNow.ToLongTimeString()
			});
			byte[] array = new byte[16 + key.Length];
			base.MailboxGuid.ToByteArray().CopyTo(array, 0);
			key.CopyTo(array, 16);
			return new MoveObjectInfo<string>(Guid.Empty, mbx, messageId, "MailboxReplicationService SyncStates", "IPM.MS-Exchange.MailboxSyncState", subject, array);
		}

		private const string ProviderName = "MapiProvider";

		private static readonly PropTag[] InboxProperties = new PropTag[]
		{
			PropTag.RulesSize,
			PropTag.ContentAggregationFlags
		};

		private static readonly string OWAScratchPad = "OWAScratchPad";

		private static readonly string StoreEvents = "StoreEvents";

		private static readonly HashSet<string> PublicFolderBranchToSkip = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"OFFLINE ADDRESS BOOK",
			"SCHEDULE+ FREE BUSY",
			"schema-root",
			MapiMailbox.OWAScratchPad,
			MapiMailbox.StoreEvents,
			"Events Root"
		};

		private readonly bool createdFromMapiStore;

		private MapiStore store;

		private Dictionary<string, MapiStore> additionalSessions;

		private EntryIdMap<MapiStore> folderSessions;

		private int serverVersion;

		private InTransitStatus inTransitStatus;

		private Dictionary<byte[], byte[]> syncStateMessageIds;
	}
}
