using System;
using System.Collections.Generic;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas;
using Microsoft.Exchange.Connections.Eas.Commands;
using Microsoft.Exchange.Connections.Eas.Commands.FolderSync;
using Microsoft.Exchange.Connections.Eas.Model.Extensions;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class EasMailbox : MailboxProviderBase, IMailbox, IDisposable
	{
		public EasMailbox() : base(LocalMailboxFlags.EasSync)
		{
		}

		protected EasMailbox(EasConnectionParameters connectionParameters, EasAuthenticationParameters authenticationParameters, EasDeviceParameters deviceParameters) : this()
		{
			this.EasConnectionParameters = connectionParameters;
			this.EasAuthenticationParameters = authenticationParameters;
			this.EasDeviceParameters = deviceParameters;
		}

		public override int ServerVersion { get; protected set; }

		internal EasConnectionWrapper EasConnectionWrapper { get; private set; }

		internal EasConnectionWrapper CrawlerConnectionWrapper
		{
			get
			{
				if (this.crawlerConnectionWrapper == null)
				{
					lock (this.syncRoot)
					{
						if (this.crawlerConnectionWrapper == null && this.ConnectionIsOperational)
						{
							EasConnectionWrapper easConnectionWrapper = new EasConnectionWrapper(new EasCrawlerConnection(this.EasConnectionParameters, this.EasAuthenticationParameters, this.EasDeviceParameters));
							easConnectionWrapper.Connect();
							this.crawlerConnectionWrapper = easConnectionWrapper;
						}
					}
				}
				return this.crawlerConnectionWrapper;
			}
		}

		private protected EasConnectionParameters EasConnectionParameters { protected get; private set; }

		private protected EasAuthenticationParameters EasAuthenticationParameters { protected get; private set; }

		private protected EasDeviceParameters EasDeviceParameters { protected get; private set; }

		protected bool ConnectionIsOperational
		{
			get
			{
				return this.EasConnectionWrapper != null;
			}
		}

		private protected EntryIdMap<Add> EasFolderCache { protected get; private set; }

		public override SyncProtocol GetSyncProtocol()
		{
			return SyncProtocol.Eas;
		}

		void IMailbox.ConfigEas(NetworkCredential userCredential, SmtpAddress smtpAddress, Guid mailboxGuid, string remoteHostName)
		{
			this.EasAuthenticationParameters = new EasAuthenticationParameters(userCredential, smtpAddress.Local, smtpAddress.Domain, base.TestIntegration.EasAutodiscoverUrlOverride ?? remoteHostName);
			this.EasConnectionParameters = new EasConnectionParameters(new UniquelyNamedObject(), new NullLog(), EasProtocolVersion.Version140, false, false, null);
			string deviceIdPrefix = mailboxGuid.ToString("N").Substring(0, EasMailbox.EasDeviceIdPrefixLength);
			this.EasDeviceParameters = new EasDeviceParameters("0123456789ABCDEF", "EasConnectionDeviceType", "ExchangeMrsAgent", deviceIdPrefix);
		}

		void IMailbox.Config(IReservation reservation, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, MailboxType mbxType, Guid? mailboxContainerGuid)
		{
			base.CheckDisposed();
			MrsTracer.Provider.Function("EasMailbox.IMailbox.Config", new object[0]);
		}

		void IMailbox.ConfigRestore(MailboxRestoreType restoreFlags)
		{
			throw new NotImplementedException();
		}

		void IMailbox.ConfigMDBByName(string mdbName)
		{
			throw new NotImplementedException();
		}

		void IMailbox.ConfigADConnection(string domainControllerName, string configDomainControllerName, NetworkCredential cred)
		{
			throw new NotImplementedException();
		}

		void IMailbox.ConfigMailboxOptions(MailboxOptions options)
		{
		}

		bool IMailbox.IsConnected()
		{
			MrsTracer.Provider.Function("EasMailbox.IMailbox.IsConnected", new object[0]);
			return this.ConnectionIsOperational;
		}

		bool IMailbox.IsMailboxCapabilitySupported(MailboxCapabilities capability)
		{
			return capability == MailboxCapabilities.ReplayActions;
		}

		MailboxInformation IMailbox.GetMailboxInformation()
		{
			MrsTracer.Provider.Function("EasMailbox.IMailbox.GetMailboxInformation", new object[0]);
			return EasMailbox.mailboxInformationSingleton;
		}

		void IMailbox.Connect(MailboxConnectFlags connectFlags)
		{
			base.CheckDisposed();
			MrsTracer.Provider.Function("EasMailbox.IMailbox.Connect", new object[0]);
			EasConnectionWrapper easConnectionWrapper = new EasConnectionWrapper(EasConnection.CreateInstance(this.EasConnectionParameters, this.EasAuthenticationParameters, this.EasDeviceParameters));
			easConnectionWrapper.Connect();
			this.EasConnectionWrapper = easConnectionWrapper;
			this.AfterConnect();
			MrsTracer.Provider.Debug("EasMailbox.IMailbox.Connect succeeded.", new object[0]);
		}

		void IMailbox.Disconnect()
		{
			base.CheckDisposed();
			MrsTracer.Provider.Function("EasMailbox.IMailbox.Disconnect", new object[0]);
			lock (this.syncRoot)
			{
				if (this.ConnectionIsOperational)
				{
					this.EasConnectionWrapper.Disconnect();
					this.EasConnectionWrapper = null;
					if (this.crawlerConnectionWrapper != null)
					{
						this.crawlerConnectionWrapper.Disconnect();
						this.crawlerConnectionWrapper = null;
					}
				}
			}
		}

		MailboxServerInformation IMailbox.GetMailboxServerInformation()
		{
			return new MailboxServerInformation
			{
				MailboxServerName = this.EasConnectionWrapper.ServerName
			};
		}

		void IMailbox.SetInTransitStatus(InTransitStatus status, out bool onlineMoveSupported)
		{
			throw new NotImplementedException();
		}

		void IMailbox.SeedMBICache()
		{
			throw new NotImplementedException();
		}

		List<FolderRec> IMailbox.EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags flags, PropTag[] additionalPtagsToLoad)
		{
			MrsTracer.Provider.Function("EasMailbox.EnumerateFolderHierarchy({0})", new object[]
			{
				flags
			});
			this.RefreshFolderCache();
			List<FolderRec> list = new List<FolderRec>(this.EasFolderCache.Count + 2);
			foreach (Add add in this.EasFolderCache.Values)
			{
				list.Add(this.CreateGenericFolderRec(add));
			}
			list.Add(EasSyntheticFolder.RootFolder.FolderRec);
			list.Add(EasSyntheticFolder.IpmSubtreeFolder.FolderRec);
			return list;
		}

		void IMailbox.DeleteMailbox(int flags)
		{
			throw new NotImplementedException();
		}

		NamedPropData[] IMailbox.GetNamesFromIDs(PropTag[] pta)
		{
			throw new NotImplementedException();
		}

		PropTag[] IMailbox.GetIDsFromNames(bool createIfNotExists, NamedPropData[] npda)
		{
			MrsTracer.Provider.Function("EasMailbox.IMailbox.GetIDsFromNames", new object[0]);
			if (createIfNotExists)
			{
				throw new GetIdsFromNamesCalledOnDestinationException();
			}
			return SyncEmailUtils.GetIDsFromNames(npda, (NamedPropData propTag) => StringComparer.OrdinalIgnoreCase.Equals(propTag.Name, "ObjectType"));
		}

		byte[] IMailbox.GetSessionSpecificEntryId(byte[] entryId)
		{
			throw new NotImplementedException();
		}

		bool IMailbox.UpdateRemoteHostName(string value)
		{
			throw new NotImplementedException();
		}

		ADUser IMailbox.GetADUser()
		{
			throw new NotImplementedException();
		}

		void IMailbox.UpdateMovedMailbox(UpdateMovedMailboxOperation op, ADUser remoteRecipientData, string domainController, out ReportEntry[] entries, Guid targetDatabaseGuid, Guid targetArchiveDatabaseGuid, string archiveDomain, ArchiveStatusFlags archiveStatus, UpdateMovedMailboxFlags updateMovedMailboxFlags, Guid? newMailboxContainerGuid, CrossTenantObjectId newUnifiedMailboxId)
		{
			throw new NotImplementedException();
		}

		List<WellKnownFolder> IMailbox.DiscoverWellKnownFolders(int flags)
		{
			List<WellKnownFolder> list = new List<WellKnownFolder>(this.EasFolderCache.Count + 1);
			foreach (Add add in this.EasFolderCache.Values)
			{
				WellKnownFolderType wkfType;
				if (EasMailbox.folderTypeMap.TryGetValue(add.GetEasFolderType(), out wkfType))
				{
					list.Add(new WellKnownFolder((int)wkfType, EasMailbox.GetEntryId(add.ServerId)));
				}
			}
			list.Add(new WellKnownFolder(3, EasSyntheticFolder.IpmSubtreeFolder.EntryId));
			return list;
		}

		MappedPrincipal[] IMailbox.ResolvePrincipals(MappedPrincipal[] principals)
		{
			throw new NotImplementedException();
		}

		Guid[] IMailbox.ResolvePolicyTag(string policyTagStr)
		{
			throw new NotImplementedException();
		}

		RawSecurityDescriptor IMailbox.GetMailboxSecurityDescriptor()
		{
			throw new NotImplementedException();
		}

		RawSecurityDescriptor IMailbox.GetUserSecurityDescriptor()
		{
			throw new NotImplementedException();
		}

		void IMailbox.AddMoveHistoryEntry(MoveHistoryEntryInternal mhei, int maxMoveHistoryLength)
		{
			throw new NotImplementedException();
		}

		ServerHealthStatus IMailbox.CheckServerHealth()
		{
			MrsTracer.Provider.Function("EasMailbox.IMailbox.CheckServerHealth", new object[0]);
			return ServerHealthStatus.Healthy;
		}

		PropValueData[] IMailbox.GetProps(PropTag[] ptags)
		{
			MrsTracer.Provider.Function("EasMailbox.GetProps", new object[0]);
			return Array.ConvertAll<PropTag, PropValueData>(ptags, (PropTag propTag) => new PropValueData(propTag.ChangePropType(PropType.Null), null));
		}

		byte[] IMailbox.GetReceiveFolderEntryId(string msgClass)
		{
			throw new NotImplementedException();
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

		internal static byte[] GetEntryId(string stringId)
		{
			return Encoding.UTF8.GetBytes(stringId);
		}

		internal static string GetStringId(byte[] entryId)
		{
			return Encoding.UTF8.GetString(entryId);
		}

		internal EasFolderSyncState GetPersistedSyncState(SyncContentsManifestState syncBlob)
		{
			if (syncBlob.Data != null)
			{
				return EasFolderSyncState.Deserialize(syncBlob.Data);
			}
			return new EasFolderSyncState
			{
				SyncKey = "0",
				CrawlerSyncKey = "0"
			};
		}

		internal EasFolderSyncState GetPersistedSyncState(byte[] folderId)
		{
			return this.GetPersistedSyncState(this.SyncState[folderId]);
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
			return DisposeTracker.Get<EasMailbox>(this);
		}

		protected EasHierarchySyncState RefreshFolderCache()
		{
			EasHierarchySyncState hierarchySyncState = this.GetHierarchySyncState();
			this.RefreshFolderCache(hierarchySyncState);
			return hierarchySyncState;
		}

		protected void RefreshFolderCache(EasHierarchySyncState state)
		{
			IReadOnlyCollection<Add> folders = state.Folders;
			EntryIdMap<Add> entryIdMap = new EntryIdMap<Add>(folders.Count);
			this.defaultCalendarId = null;
			foreach (Add add in folders)
			{
				EasFolderType easFolderType = add.GetEasFolderType();
				if (easFolderType == EasFolderType.Calendar)
				{
					this.defaultCalendarId = EasMailbox.GetEntryId(add.ServerId);
				}
				EasFolderType easFolderType2 = easFolderType;
				if (easFolderType2 == EasFolderType.UserGeneric)
				{
					goto IL_8B;
				}
				bool flag;
				switch (easFolderType2)
				{
				case EasFolderType.Contacts:
				case EasFolderType.UserContacts:
					flag = !ConfigBase<MRSConfigSchema>.GetConfig<bool>("DisableContactSync");
					goto IL_9D;
				case EasFolderType.UserMail:
				case EasFolderType.UserCalendar:
					goto IL_8B;
				}
				flag = EasMailbox.folderTypeMap.ContainsKey(easFolderType);
				IL_9D:
				if (flag)
				{
					entryIdMap.Add(EasMailbox.GetEntryId(add.ServerId), add);
					continue;
				}
				MrsTracer.Provider.Debug("EasMailbox.RefreshFolderCache: ignore {0} folder '{1}' since it is not supported yet", new object[]
				{
					easFolderType,
					add.DisplayName
				});
				continue;
				IL_8B:
				flag = true;
				goto IL_9D;
			}
			this.EasFolderCache = entryIdMap;
		}

		private static string GetFolderClass(Add add)
		{
			switch (add.GetEasFolderType())
			{
			case EasFolderType.UserGeneric:
				return "Generic";
			case EasFolderType.Tasks:
			case EasFolderType.UserTasks:
				return "Task";
			case EasFolderType.Calendar:
			case EasFolderType.UserCalendar:
				return "Calendar";
			case EasFolderType.Contacts:
			case EasFolderType.UserContacts:
				return "Contact";
			case EasFolderType.Notes:
			case EasFolderType.UserNotes:
				return "Note";
			case EasFolderType.Journal:
			case EasFolderType.UserJournal:
				return "Journal";
			}
			return "Mail";
		}

		private FolderRec CreateGenericFolderRec(Add add)
		{
			return new FolderRec(EasMailbox.GetEntryId(add.ServerId), this.GetParentId(add), FolderType.Generic, EasMailbox.GetFolderClass(add), add.DisplayName, DateTime.MinValue, null);
		}

		private byte[] GetParentId(Add add)
		{
			if (add.GetEasFolderType() != EasFolderType.UserCalendar)
			{
				return EasMailbox.GetEntryId(add.ParentId);
			}
			return this.defaultCalendarId;
		}

		private EasHierarchySyncState GetHierarchySyncState()
		{
			SyncHierarchyManifestState hierarchyData = this.SyncState.HierarchyData;
			EasHierarchySyncState easHierarchySyncState;
			if (hierarchyData != null && !string.IsNullOrEmpty(hierarchyData.ProviderSyncState) && hierarchyData.ManualSyncData == null)
			{
				MrsTracer.Provider.Debug("EasMailbox.GetHierarchySyncState: Deserialize folder state from hierState.ProviderSyncState", new object[0]);
				easHierarchySyncState = EasHierarchySyncState.Deserialize(hierarchyData.ProviderSyncState);
			}
			else
			{
				MrsTracer.Provider.Debug("EasMailbox.GetHierarchySyncState: Get all the folders from the EAS server", new object[0]);
				string syncKey;
				Add[] allFoldersOnServer = this.GetAllFoldersOnServer(out syncKey);
				easHierarchySyncState = new EasHierarchySyncState(allFoldersOnServer, syncKey);
				hierarchyData.ProviderSyncState = easHierarchySyncState.Serialize(false);
			}
			hierarchyData.ManualSyncData = null;
			return easHierarchySyncState;
		}

		private Add[] GetAllFoldersOnServer(out string newSyncKey)
		{
			FolderSyncResponse folderSyncResponse = this.EasConnectionWrapper.FolderSync();
			List<Add> additions = folderSyncResponse.Changes.Additions;
			newSyncKey = folderSyncResponse.SyncKey;
			return additions.ToArray();
		}

		private const string ProviderName = "EasProvider";

		private const string EasDeviceId = "0123456789ABCDEF";

		private const string EasDeviceType = "EasConnectionDeviceType";

		private const string EasUserAgent = "ExchangeMrsAgent";

		private static readonly int EasDeviceIdPrefixLength = 32 - "0123456789ABCDEF".Length;

		private static readonly MailboxInformation mailboxInformationSingleton = new MailboxInformation
		{
			ProviderName = "EasProvider"
		};

		private static readonly Dictionary<EasFolderType, WellKnownFolderType> folderTypeMap = new Dictionary<EasFolderType, WellKnownFolderType>
		{
			{
				EasFolderType.Inbox,
				WellKnownFolderType.Inbox
			},
			{
				EasFolderType.DeletedItems,
				WellKnownFolderType.DeletedItems
			},
			{
				EasFolderType.Drafts,
				WellKnownFolderType.Drafts
			},
			{
				EasFolderType.SentItems,
				WellKnownFolderType.SentItems
			},
			{
				EasFolderType.Outbox,
				WellKnownFolderType.Outbox
			},
			{
				EasFolderType.JunkEmail,
				WellKnownFolderType.JunkEmail
			},
			{
				EasFolderType.Calendar,
				WellKnownFolderType.Calendar
			},
			{
				EasFolderType.Contacts,
				WellKnownFolderType.Contacts
			}
		};

		private readonly object syncRoot = new object();

		private EasConnectionWrapper crawlerConnectionWrapper;

		private byte[] defaultCalendarId;
	}
}
