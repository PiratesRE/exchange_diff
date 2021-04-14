using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MailboxWrapper : WrapperBase<IMailbox>, IFilterBuilderHelper, IMailbox, IDisposable
	{
		public MailboxWrapper(IMailbox mailbox, MailboxWrapperFlags flags, LocalizedString tracingId) : base(mailbox, null)
		{
			this.Flags = flags;
			this.tracingContext = new WrappedDataContext(tracingId);
			this.mailboxVersion = null;
			base.CreateContext = new CommonUtils.CreateContextDelegate(this.CreateDataContext);
			this.NamedPropMapper = new NamedPropMapper(this.Mailbox, (this.Flags & MailboxWrapperFlags.Target) != (MailboxWrapperFlags)0);
			this.PrincipalMapper = new PrincipalMapper(this.Mailbox);
		}

		public IMailbox Mailbox
		{
			get
			{
				return this;
			}
		}

		public MailboxServerInformation LastConnectedServerInfo { get; private set; }

		public MailboxWrapperFlags Flags { get; private set; }

		public NamedPropMapper NamedPropMapper { get; private set; }

		public PrincipalMapper PrincipalMapper { get; private set; }

		public FolderMap FolderMap { get; private set; }

		public bool HasMapiSession { get; private set; }

		public int? MailboxVersion
		{
			get
			{
				if (this.mailboxVersion == null && this.Mailbox.IsConnected())
				{
					this.Mailbox.GetMailboxInformation();
				}
				return this.mailboxVersion;
			}
		}

		public CultureInfo MailboxCulture
		{
			get
			{
				int num = 0;
				PropValueData[] props = this.Mailbox.GetProps(new PropTag[]
				{
					PropTag.LocaleId
				});
				if (1721827331 == props[0].PropTag)
				{
					num = (int)props[0].Value;
				}
				CultureInfo cultureInfo;
				if (num == 0 || num == 1024 || num == 127 || num == CultureInfo.InvariantCulture.LCID)
				{
					cultureInfo = new CultureInfo("en-US");
				}
				else
				{
					cultureInfo = CultureInfo.GetCultureInfo(num);
				}
				MrsTracer.Service.Debug("Destination MailboxCulture: LCID = {0}, Culture = {1}", new object[]
				{
					num,
					(cultureInfo == null) ? "null" : cultureInfo.ToString()
				});
				return cultureInfo;
			}
		}

		protected abstract OperationSideDataContext SideOperationContext { get; }

		public void LoadFolderMap(GetFolderMapFlags flags, Func<FolderMap> getFolderMap)
		{
			if (this.FolderMap != null && !flags.HasFlag(GetFolderMapFlags.ForceRefresh))
			{
				return;
			}
			this.FolderMap = getFolderMap();
		}

		public List<FolderRecWrapper> LoadFolders<TFolderRec>(EnumerateFolderHierarchyFlags enumerateFolderHierarchyFlags, PropTag[] additionalPtags) where TFolderRec : FolderRecWrapper, new()
		{
			return FolderRecWrapper.WrapList<TFolderRec>(this.Mailbox.EnumerateFolderHierarchy(enumerateFolderHierarchyFlags, additionalPtags));
		}

		public abstract IFolder GetFolder(byte[] folderId);

		public void Ping()
		{
			using (IFolder folder = this.GetFolder(null))
			{
				folder.GetProps(MailboxWrapper.pingProperties);
			}
		}

		public void UpdateLastConnectedServerInfo(out MailboxServerInformation serverInfo, out bool hasDatabaseFailedOver)
		{
			serverInfo = this.Mailbox.GetMailboxServerInformation();
			hasDatabaseFailedOver = false;
			if (serverInfo != null)
			{
				if (this.LastConnectedServerInfo != null && serverInfo.MailboxServerGuid != Guid.Empty && this.LastConnectedServerInfo.MailboxServerGuid != serverInfo.MailboxServerGuid)
				{
					this.LastConnectedServerInfo = null;
					hasDatabaseFailedOver = true;
					return;
				}
				this.LastConnectedServerInfo = serverInfo;
			}
		}

		public virtual void Clear()
		{
			this.FolderMap = null;
		}

		PropTag IFilterBuilderHelper.MapNamedProperty(NamedPropData npd, PropType propType)
		{
			return this.NamedPropMapper.MapNamedProp(npd, propType);
		}

		Guid[] IFilterBuilderHelper.MapPolicyTag(string policyTagStr)
		{
			return this.Mailbox.ResolvePolicyTag(policyTagStr);
		}

		string[] IFilterBuilderHelper.MapRecipient(string recipientId)
		{
			MappedPrincipal mappedPrincipal = new MappedPrincipal();
			mappedPrincipal.Alias = recipientId;
			MappedPrincipal[] array = this.Mailbox.ResolvePrincipals(new MappedPrincipal[]
			{
				mappedPrincipal
			});
			if (array == null || array[0] == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			for (mappedPrincipal = array[0]; mappedPrincipal != null; mappedPrincipal = mappedPrincipal.NextEntry)
			{
				if (!string.IsNullOrEmpty(mappedPrincipal.Alias))
				{
					list.Add(mappedPrincipal.Alias);
				}
				if (!string.IsNullOrEmpty(mappedPrincipal.DisplayName))
				{
					list.Add(mappedPrincipal.DisplayName);
				}
				if (!string.IsNullOrEmpty(mappedPrincipal.LegacyDN))
				{
					list.Add(mappedPrincipal.LegacyDN);
				}
				ProxyAddressCollection proxyAddressCollection = new ProxyAddressCollection(mappedPrincipal.ProxyAddresses);
				ProxyAddress proxyAddress = proxyAddressCollection.FindPrimary(ProxyAddressPrefix.Smtp);
				if (proxyAddress != null && !string.IsNullOrEmpty(proxyAddress.AddressString))
				{
					list.Add(proxyAddress.AddressString);
				}
			}
			return list.ToArray();
		}

		LatencyInfo IMailbox.GetLatencyInfo()
		{
			LatencyInfo result = new LatencyInfo();
			base.CreateContext("IMailbox.GetLatency", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetLatencyInfo();
			}, true);
			return result;
		}

		bool IMailbox.IsConnected()
		{
			bool result = false;
			base.CreateContext("IMailbox.IsConnected", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.IsConnected();
			}, true);
			return result;
		}

		void IMailbox.ConfigADConnection(string domainControllerName, string configDomainControllerName, NetworkCredential cred)
		{
			base.CreateContext("IMailbox.ConfigADConnection", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.ConfigADConnection(domainControllerName, configDomainControllerName, cred);
			}, true);
		}

		void IMailbox.ConfigPreferredADConnection(string preferredDomainControllerName)
		{
			base.CreateContext("IMailbox.ConfigPreferredADConnection", new DataContext[]
			{
				new SimpleValueDataContext("preferredDomainControllerName", preferredDomainControllerName)
			}).Execute(delegate
			{
				this.WrappedObject.ConfigPreferredADConnection(preferredDomainControllerName);
			}, true);
		}

		void IMailbox.Config(IReservation reservation, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, MailboxType mbxType, Guid? mailboxContainerGuid)
		{
			base.CreateContext("IMailbox.Config", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.Config(reservation, primaryMailboxGuid, physicalMailboxGuid, partitionHint, mdbGuid, mbxType, mailboxContainerGuid);
			}, true);
		}

		void IMailbox.ConfigPst(string filePath, int? contentCodePage)
		{
			base.CreateContext("IMailbox.ConfigPst", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.ConfigPst(filePath, contentCodePage);
			}, true);
		}

		void IMailbox.ConfigEas(NetworkCredential userCredential, SmtpAddress smtpAddress, Guid mailboxGuid, string remoteHostName)
		{
			base.CreateContext("IMailbox.ConfigEas", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.ConfigEas(userCredential, smtpAddress, mailboxGuid, remoteHostName);
			}, true);
		}

		void IMailbox.ConfigRestore(MailboxRestoreType restoreFlags)
		{
			base.CreateContext("IMailbox.ConfigRestore", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.ConfigRestore(restoreFlags);
			}, true);
		}

		void IMailbox.ConfigMDBByName(string mdbName)
		{
			base.CreateContext("IMailbox.ConfigMDBByName", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.ConfigMDBByName(mdbName);
			}, true);
		}

		void IMailbox.ConfigOlc(OlcMailboxConfiguration config)
		{
			base.CreateContext("IMailbox.ConfigOlc", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.ConfigOlc(config);
			}, true);
		}

		void IMailbox.ConfigMailboxOptions(MailboxOptions options)
		{
			base.CreateContext("IMailbox.ConfigMailboxOptions", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.ConfigMailboxOptions(options);
			}, true);
		}

		MailboxInformation IMailbox.GetMailboxInformation()
		{
			MailboxInformation result = null;
			base.CreateContext("IMailbox.GetMailboxInformation", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetMailboxInformation();
			}, true);
			if (result != null)
			{
				this.mailboxVersion = new int?(result.ServerVersion);
			}
			return result;
		}

		void IMailbox.Connect(MailboxConnectFlags connectFlags)
		{
			this.CreateDataContextWithType("IMailbox.Connect", OperationType.Connect, new DataContext[]
			{
				new SimpleValueDataContext("Flags", connectFlags)
			}).Execute(delegate
			{
				this.WrappedObject.Connect(connectFlags);
			}, true);
			this.HasMapiSession = !connectFlags.HasFlag(MailboxConnectFlags.DoNotOpenMapiSession);
		}

		bool IMailbox.IsCapabilitySupported(MRSProxyCapabilities capability)
		{
			bool result = false;
			base.CreateContext("IMailbox.IsCapabilitySupported", new DataContext[]
			{
				new SimpleValueDataContext("Capability", capability)
			}).Execute(delegate
			{
				result = this.WrappedObject.IsCapabilitySupported(capability);
			}, true);
			return result;
		}

		bool IMailbox.IsMailboxCapabilitySupported(MailboxCapabilities capability)
		{
			bool result = false;
			base.CreateContext("IMailbox.IsMailboxCapabilitySupported", new DataContext[]
			{
				new SimpleValueDataContext("Capability", capability)
			}).Execute(delegate
			{
				bool result;
				if (this.mailboxCapabilities.TryGetValue((int)capability, out result))
				{
					result = result;
					return;
				}
				MailboxCapabilities capability2 = capability;
				MailboxCapabilities capability3 = capability;
				MailboxCapabilities capability4 = capability;
				MailboxCapabilities capability5 = capability;
				result = this.WrappedObject.IsMailboxCapabilitySupported(capability);
				this.mailboxCapabilities.Add((int)capability, result);
			}, true);
			return result;
		}

		void IMailbox.Disconnect()
		{
			this.mailboxVersion = null;
			this.NamedPropMapper.Clear();
			this.PrincipalMapper.Clear();
			this.HasMapiSession = false;
			base.CreateContext("IMailbox.Disconnect", new DataContext[0]).Execute(delegate
			{
				base.WrappedObject.Disconnect();
			}, true);
		}

		void IMailbox.SetInTransitStatus(InTransitStatus status, out bool onlineMoveSupported)
		{
			bool onlineMoveSupportedInt = false;
			base.CreateContext("IMailbox.SetInTransitStatus", new DataContext[]
			{
				new SimpleValueDataContext("Status", status)
			}).Execute(delegate
			{
				this.WrappedObject.SetInTransitStatus(status, out onlineMoveSupportedInt);
			}, true);
			onlineMoveSupported = onlineMoveSupportedInt;
		}

		void IMailbox.SeedMBICache()
		{
			base.CreateContext("IMailbox.SeedMBICache", new DataContext[0]).Execute(delegate
			{
				base.WrappedObject.SeedMBICache();
			}, true);
		}

		List<WellKnownFolder> IMailbox.DiscoverWellKnownFolders(int flags)
		{
			List<WellKnownFolder> wellKnownFolders = null;
			base.CreateContext("IMailbox.DiscoverWellKnownFolders", new DataContext[0]).Execute(delegate
			{
				wellKnownFolders = this.WrappedObject.DiscoverWellKnownFolders(flags);
			}, true);
			return wellKnownFolders;
		}

		MailboxServerInformation IMailbox.GetMailboxServerInformation()
		{
			MailboxServerInformation result = null;
			base.CreateContext("IMailbox.GetMailboxServerInformation", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetMailboxServerInformation();
			}, true);
			return result;
		}

		VersionInformation IMailbox.GetVersion()
		{
			if (this.providerVersion == null)
			{
				base.CreateContext("IMailbox.GetVersion", new DataContext[0]).Execute(delegate
				{
					this.providerVersion = base.WrappedObject.GetVersion();
				}, true);
			}
			return this.providerVersion;
		}

		void IMailbox.SetOtherSideVersion(VersionInformation otherSideVersion)
		{
			base.CreateContext("IMailbox.SetOtherSideMailboxServerInformation", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.SetOtherSideVersion(otherSideVersion);
			}, true);
		}

		List<FolderRec> IMailbox.EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags flags, PropTag[] additionalPtagsToLoad)
		{
			List<FolderRec> result = null;
			base.CreateContext("IMailbox.EnumerateFolderHierarchy", new DataContext[]
			{
				new PropTagsDataContext(additionalPtagsToLoad)
			}).Execute(delegate
			{
				result = this.WrappedObject.EnumerateFolderHierarchy(flags, additionalPtagsToLoad);
			}, true);
			return result;
		}

		void IMailbox.DeleteMailbox(int flags)
		{
			base.CreateContext("IMailbox.DeleteMailbox", new DataContext[]
			{
				new SimpleValueDataContext("Flags", flags)
			}).Execute(delegate
			{
				this.WrappedObject.DeleteMailbox(flags);
			}, true);
		}

		NamedPropData[] IMailbox.GetNamesFromIDs(PropTag[] pta)
		{
			NamedPropData[] result = null;
			base.CreateContext("IMailbox.GetNamesFromIDs", new DataContext[]
			{
				new PropTagsDataContext(pta)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetNamesFromIDs(pta);
			}, true);
			return result;
		}

		PropTag[] IMailbox.GetIDsFromNames(bool createIfNotExists, NamedPropData[] npa)
		{
			PropTag[] result = null;
			base.CreateContext("IMailbox.GetIDsFromNames", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetIDsFromNames(createIfNotExists, npa);
			}, true);
			return result;
		}

		byte[] IMailbox.GetSessionSpecificEntryId(byte[] entryId)
		{
			byte[] result = null;
			base.CreateContext("IMailbox.GetSessionSpecificEntryId", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetSessionSpecificEntryId(entryId);
			}, true);
			return result;
		}

		MappedPrincipal[] IMailbox.ResolvePrincipals(MappedPrincipal[] principals)
		{
			MappedPrincipal[] result = null;
			base.CreateContext("IMailbox.ResolvePrincipals", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.ResolvePrincipals(principals);
			}, true);
			return result;
		}

		bool IMailbox.UpdateRemoteHostName(string value)
		{
			bool result = false;
			base.CreateContext("IMailbox.UpdateRemoteHostName", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.UpdateRemoteHostName(value);
			}, true);
			return result;
		}

		ADUser IMailbox.GetADUser()
		{
			ADUser result = null;
			base.CreateContext("IMailbox.GetADUser", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetADUser();
			}, true);
			return result;
		}

		void IMailbox.UpdateMovedMailbox(UpdateMovedMailboxOperation op, ADUser remoteRecipientData, string domainController, out ReportEntry[] entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, ArchiveStatusFlags archiveStatus, UpdateMovedMailboxFlags updateMovedMailboxFlags, Guid? newMailboxContainerGuid, CrossTenantObjectId newUnifiedMailboxId)
		{
			ReportEntry[] entriesInt = null;
			base.CreateContext("IMailbox.UpdateMovedMailbox", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.UpdateMovedMailbox(op, remoteRecipientData, domainController, out entriesInt, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, archiveStatus, updateMovedMailboxFlags, newMailboxContainerGuid, newUnifiedMailboxId);
			}, true);
			entries = entriesInt;
		}

		RawSecurityDescriptor IMailbox.GetMailboxSecurityDescriptor()
		{
			RawSecurityDescriptor result = null;
			base.CreateContext("IMailbox.GetMailboxSecurityDescriptor", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetMailboxSecurityDescriptor();
			}, true);
			return result;
		}

		RawSecurityDescriptor IMailbox.GetUserSecurityDescriptor()
		{
			RawSecurityDescriptor result = null;
			base.CreateContext("IMailbox.GetUserSecurityDescriptor", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetUserSecurityDescriptor();
			}, true);
			return result;
		}

		void IMailbox.AddMoveHistoryEntry(MoveHistoryEntryInternal mhei, int maxMoveHistoryLength)
		{
			base.CreateContext("IMailbox.AddMoveHistoryEntry", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.AddMoveHistoryEntry(mhei, maxMoveHistoryLength);
			}, true);
		}

		ServerHealthStatus IMailbox.CheckServerHealth()
		{
			ServerHealthStatus result = null;
			base.CreateContext("IMailbox.CheckServerHealth", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.CheckServerHealth();
			}, true);
			return result;
		}

		PropValueData[] IMailbox.GetProps(PropTag[] ptags)
		{
			PropValueData[] result = null;
			base.CreateContext("IMailbox.GetProps", new DataContext[]
			{
				new PropTagsDataContext(ptags)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetProps(ptags);
			}, true);
			return result;
		}

		byte[] IMailbox.GetReceiveFolderEntryId(string msgClass)
		{
			byte[] result = null;
			base.CreateContext("IMailbox.GetReceiveFolderEntryId", new DataContext[]
			{
				new SimpleValueDataContext("MsgClass", msgClass)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetReceiveFolderEntryId(msgClass);
			}, true);
			return result;
		}

		Guid[] IMailbox.ResolvePolicyTag(string policyTagStr)
		{
			Guid[] result = null;
			base.CreateContext("IMailbox.ResolvePolicyTag", new DataContext[]
			{
				new SimpleValueDataContext("PolicyTag", policyTagStr)
			}).Execute(delegate
			{
				result = this.WrappedObject.ResolvePolicyTag(policyTagStr);
			}, true);
			return result;
		}

		string IMailbox.LoadSyncState(byte[] key)
		{
			string result = null;
			base.CreateContext("IMailbox.LoadSyncState", new DataContext[]
			{
				new SimpleValueDataContext("Key", TraceUtils.DumpBytes(key))
			}).Execute(delegate
			{
				result = this.WrappedObject.LoadSyncState(key);
			}, true);
			return result;
		}

		MessageRec IMailbox.SaveSyncState(byte[] key, string syncState)
		{
			MessageRec result = null;
			base.CreateContext("IMailbox.SaveSyncState", new DataContext[]
			{
				new SimpleValueDataContext("Key", TraceUtils.DumpBytes(key)),
				new SimpleValueDataContext("SyncStateLength", (syncState != null) ? syncState.Length : 0)
			}).Execute(delegate
			{
				result = this.WrappedObject.SaveSyncState(key, syncState);
			}, true);
			return result;
		}

		SessionStatistics IMailbox.GetSessionStatistics(SessionStatisticsFlags statisticsTypes)
		{
			SessionStatistics result = null;
			base.CreateContext("IDestinationMailbox.GetSessionStatistics", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetSessionStatistics(statisticsTypes);
			}, true);
			return result;
		}

		Guid IMailbox.StartIsInteg(List<uint> mailboxCorruptionTypes)
		{
			Guid result = Guid.Empty;
			base.CreateContext("IMailbox.StartIsInteg", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.StartIsInteg(mailboxCorruptionTypes);
			}, true);
			return result;
		}

		List<StoreIntegrityCheckJob> IMailbox.QueryIsInteg(Guid isIntegRequestGuid)
		{
			List<StoreIntegrityCheckJob> jobs = null;
			base.CreateContext("IMailbox.QueryIsInteg", new DataContext[0]).Execute(delegate
			{
				jobs = this.WrappedObject.QueryIsInteg(isIntegRequestGuid);
			}, true);
			return jobs;
		}

		public virtual List<ItemPropertiesBase> GetMailboxSettings(GetMailboxSettingsFlags flags)
		{
			List<ItemPropertiesBase> result = null;
			if (base.WrappedObject is ISourceMailbox)
			{
				base.CreateContext("ISourceMailbox.GetMailboxSettings", new DataContext[0]).Execute(delegate
				{
					result = ((ISourceMailbox)this.WrappedObject).GetMailboxSettings(flags);
				}, true);
			}
			return result;
		}

		private ExecutionContextWrapper CreateDataContext(string callName, params DataContext[] additionalContexts)
		{
			return this.CreateDataContextWithType(callName, OperationType.None, additionalContexts);
		}

		private ExecutionContextWrapper CreateDataContextWithType(string callName, OperationType operationType, params DataContext[] additionalContexts)
		{
			List<DataContext> list = new List<DataContext>();
			list.Add(new OperationDataContext(callName, operationType));
			list.Add(this.SideOperationContext);
			list.Add(this.tracingContext);
			if (additionalContexts != null)
			{
				list.AddRange(additionalContexts);
			}
			return new ExecutionContextWrapper(new CommonUtils.UpdateDuration(base.UpdateDuration), callName, list.ToArray());
		}

		private static PropTag[] pingProperties = new PropTag[]
		{
			PropTag.LocalCommitTimeMax,
			PropTag.ContentCount,
			PropTag.DeletedCountTotal
		};

		private WrappedDataContext tracingContext;

		private int? mailboxVersion;

		private readonly Dictionary<int, bool> mailboxCapabilities = new Dictionary<int, bool>(7);

		private VersionInformation providerVersion;
	}
}
