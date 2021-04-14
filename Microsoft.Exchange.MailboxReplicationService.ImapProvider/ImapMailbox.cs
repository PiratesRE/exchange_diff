using System;
using System.Collections.Generic;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Imap;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class ImapMailbox : MailboxProviderBase, IMailbox, IDisposable
	{
		public ImapMailbox(ImapConnection imapConnection) : base(LocalMailboxFlags.None)
		{
			this.ImapConnection = imapConnection;
		}

		public ImapMailbox(ConnectionParameters connectionParameters, ImapAuthenticationParameters authenticationParameters, ImapServerParameters serverParameters, SmtpServerParameters smtpParameters) : base(LocalMailboxFlags.None)
		{
			this.ConnectionParameters = connectionParameters;
			this.AuthenticationParameters = authenticationParameters;
			this.ServerParameters = serverParameters;
			this.SmtpParameters = smtpParameters;
			this.ImapConnection = ImapConnection.CreateInstance(connectionParameters);
		}

		public override int ServerVersion
		{
			get
			{
				throw new NotImplementedException();
			}
			protected set
			{
				throw new NotImplementedException();
			}
		}

		internal ImapConnection ImapConnection { get; private set; }

		private protected ImapServerParameters ServerParameters { protected get; private set; }

		private protected SmtpServerParameters SmtpParameters { protected get; private set; }

		private protected ImapAuthenticationParameters AuthenticationParameters { protected get; private set; }

		private protected ConnectionParameters ConnectionParameters { protected get; private set; }

		void IMailbox.Config(IReservation reservation, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, MailboxType mbxType, Guid? mailboxContainerGuid)
		{
			throw new NotImplementedException();
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
			MrsTracer.Provider.Function("ImapMailbox.IMailbox.IsConnected", new object[0]);
			return this.ImapConnection.IsConnected();
		}

		bool IMailbox.IsMailboxCapabilitySupported(MailboxCapabilities capability)
		{
			return capability == MailboxCapabilities.ReplayActions;
		}

		MailboxInformation IMailbox.GetMailboxInformation()
		{
			MrsTracer.Provider.Function("ImapMailbox.IMailbox.GetMailboxInformation", new object[0]);
			return new MailboxInformation
			{
				ProviderName = "ImapProvider"
			};
		}

		void IMailbox.Connect(MailboxConnectFlags connectFlags)
		{
			base.CheckDisposed();
			MrsTracer.Provider.Function("ImapMailbox.IMailbox.Connect", new object[0]);
			IServerCapabilities capabilities = new ImapServerCapabilities().Add("IMAP4REV1");
			this.ImapConnection.ConnectAndAuthenticate(this.ServerParameters, this.AuthenticationParameters, capabilities);
			this.AfterConnect();
			MrsTracer.Provider.Debug("ImapClient.Connect: Imap Connection, Authentication and Capabilities check succeeded", new object[0]);
		}

		void IMailbox.Disconnect()
		{
			base.CheckDisposed();
			MrsTracer.Provider.Function("ImapMailbox.IMailbox.Disconnect", new object[0]);
			lock (this.syncRoot)
			{
				if (this.ImapConnection != null)
				{
					if (this.ImapConnection.IsConnected())
					{
						this.ImapConnection.LogOff();
					}
					this.ImapConnection.Dispose();
					this.ImapConnection = null;
				}
			}
		}

		MailboxServerInformation IMailbox.GetMailboxServerInformation()
		{
			MrsTracer.Provider.Function("ImapMailbox.IMailbox.GetMailboxServerInformation", new object[0]);
			return new MailboxServerInformation
			{
				MailboxServerName = this.ImapConnection.ConnectionContext.Server
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
			MrsTracer.Provider.Function("ImapMailbox.EnumerateFolderHierarchy({0})", new object[]
			{
				flags
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			List<FolderRec> list = null;
			List<ImapClientFolder> list2 = this.EnumerateFolderHierarchy();
			list2.Add(new ImapClientFolder(ImapMailbox.Root)
			{
				IsSelectable = false
			});
			list2.Add(new ImapClientFolder(ImapMailbox.IpmSubtree)
			{
				IsSelectable = false
			});
			ImapClientFolder.FindWellKnownFolders(list2);
			this.folderCache = new EntryIdMap<ImapClientFolder>();
			list = new List<FolderRec>(list2.Count);
			foreach (ImapClientFolder imapClientFolder in list2)
			{
				FolderRec folderRec = this.CreateFolderRec(imapClientFolder);
				list.Add(folderRec);
				this.folderCache.Add(folderRec.EntryId, imapClientFolder);
			}
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
			MrsTracer.Provider.Function("ImapMailbox.IMailbox.GetIDsFromNames", new object[0]);
			if (createIfNotExists)
			{
				throw new GetIdsFromNamesCalledOnDestinationException();
			}
			return SyncEmailUtils.GetIDsFromNames(npda, null);
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
			List<WellKnownFolder> list = new List<WellKnownFolder>();
			list.Add(new WellKnownFolder(3, ImapMailbox.IpmSubtreeEntryId));
			foreach (ImapClientFolder imapClientFolder in this.folderCache.Values)
			{
				if (imapClientFolder.WellKnownFolderType != WellKnownFolderType.None)
				{
					WellKnownFolderType wellKnownFolderType = imapClientFolder.WellKnownFolderType;
					byte[] entryId = ImapEntryId.CreateFolderEntryId(imapClientFolder.Name);
					list.Add(new WellKnownFolder((int)wellKnownFolderType, entryId));
				}
			}
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
			MrsTracer.Provider.Function("ImapMailbox.IMailbox.CheckServerHealth", new object[0]);
			return new ServerHealthStatus(ServerHealthState.Healthy);
		}

		PropValueData[] IMailbox.GetProps(PropTag[] ptags)
		{
			MrsTracer.Provider.Function("ImapMailbox.GetProps", new object[0]);
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			PropValueData[] array = new PropValueData[ptags.Length];
			for (int i = 0; i < ptags.Length; i++)
			{
				PropTag propTag = ptags[i];
				propTag = propTag.ChangePropType(PropType.Null);
				array[i] = new PropValueData(propTag, null);
			}
			return array;
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

		public T GetFolder<T>(byte[] folderId) where T : ImapFolder, new()
		{
			MrsTracer.Provider.Function("ImapMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(folderId)
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			ImapClientFolder folder;
			if (!this.folderCache.TryGetValue(folderId, out folder))
			{
				MrsTracer.Provider.Debug("Folder with entryId {0} does not exist", new object[]
				{
					folderId
				});
				return default(T);
			}
			T result = Activator.CreateInstance<T>();
			result.Config(folderId, folder, this);
			return result;
		}

		public FolderRec CreateFolderRec(ImapClientFolder folder)
		{
			if (folder.Name.Equals(ImapMailbox.Root))
			{
				return new FolderRec(ImapMailbox.RootEntryId, null, FolderType.Root, string.Empty, DateTime.MinValue, null);
			}
			if (folder.Name.Equals(ImapMailbox.IpmSubtree))
			{
				return new FolderRec(ImapMailbox.IpmSubtreeEntryId, ImapMailbox.RootEntryId, FolderType.Generic, "Top of Information Store", DateTime.MinValue, null);
			}
			byte[] entryId = ImapEntryId.CreateFolderEntryId(folder.Name);
			string parentFolderPath = folder.ParentFolderPath;
			if (string.IsNullOrEmpty(parentFolderPath))
			{
				return new FolderRec(entryId, ImapMailbox.IpmSubtreeEntryId, FolderType.Generic, folder.ShortFolderName, DateTime.MinValue, null);
			}
			return new FolderRec(entryId, ImapEntryId.CreateFolderEntryId(parentFolderPath), FolderType.Generic, folder.ShortFolderName, DateTime.MinValue, null);
		}

		internal void SetImapConnectionFromTestOnly(ImapConnection imapConnection)
		{
			this.ImapConnection = imapConnection;
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
			return DisposeTracker.Get<ImapMailbox>(this);
		}

		private List<ImapClientFolder> EnumerateFolderHierarchy()
		{
			ImapMailbox.EnsureDefaultFolderMappingTable(this.ConnectionParameters.Log);
			List<ImapClientFolder> list = new List<ImapClientFolder>();
			int num = 0;
			int num2 = 0;
			while (num2 < 2 && num < 20)
			{
				num++;
				char latestSeparatorCharacter = this.GetLatestSeparatorCharacter(list);
				IList<ImapMailbox> list2 = this.ImapConnection.ListImapMailboxesByLevel(num, latestSeparatorCharacter);
				if (list2.Count == 0)
				{
					num2++;
				}
				else
				{
					num2 = 0;
					MrsTracer.Provider.Debug("Number of folders: {0}", new object[]
					{
						list2.Count
					});
					List<ImapMailbox> list3 = new List<ImapMailbox>(list2.Count);
					list3.AddRange(list2);
					foreach (ImapMailbox imapMailbox in list3)
					{
						if (imapMailbox.IsSelectable)
						{
							ImapMailbox folder = this.ImapConnection.SelectImapMailbox(imapMailbox);
							list.Add(new ImapClientFolder(folder));
						}
						else
						{
							list.Add(new ImapClientFolder(imapMailbox));
						}
						if (!string.Equals(imapMailbox.Name, "INBOX", StringComparison.OrdinalIgnoreCase) && imapMailbox.Separator != null)
						{
							this.inboxFolderHierarchySeparator = new char?(imapMailbox.Separator.Value);
						}
					}
				}
			}
			return list;
		}

		private char GetLatestSeparatorCharacter(IList<ImapClientFolder> folders)
		{
			if (folders.Count > 0 && folders[folders.Count - 1].Separator != null)
			{
				return folders[folders.Count - 1].Separator.Value;
			}
			if (this.inboxFolderHierarchySeparator == null)
			{
				return '/';
			}
			return this.inboxFolderHierarchySeparator.Value;
		}

		private const string IpmSubtreeDisplayName = "Top of Information Store";

		private const string ProviderName = "ImapProvider";

		private const int NumLevelsStopHierarchyEnumeration = 2;

		internal static readonly int ImapTimeout = 10800000;

		private static readonly string Root = WellKnownFolderType.Root.ToString();

		private static readonly byte[] RootEntryId = ImapEntryId.CreateFolderEntryId(ImapMailbox.Root);

		private static readonly string IpmSubtree = WellKnownFolderType.IpmSubtree.ToString();

		private static readonly byte[] IpmSubtreeEntryId = ImapEntryId.CreateFolderEntryId(ImapMailbox.IpmSubtree);

		private readonly object syncRoot = new object();

		private char? inboxFolderHierarchySeparator;

		private EntryIdMap<ImapClientFolder> folderCache;
	}
}
