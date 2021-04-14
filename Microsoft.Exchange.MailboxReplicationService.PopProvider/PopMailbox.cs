using System;
using System.Collections.Generic;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Pop;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class PopMailbox : MailboxProviderBase, IMailbox, IDisposable
	{
		public PopMailbox(Pop3Connection popConnection) : base(LocalMailboxFlags.None)
		{
			this.PopConnection = popConnection;
		}

		public PopMailbox(ConnectionParameters connectionParameters, Pop3AuthenticationParameters authenticationParameters, Pop3ServerParameters serverParameters, SmtpServerParameters smtpParameters) : base(LocalMailboxFlags.None)
		{
			this.ConnectionParameters = connectionParameters;
			this.AuthenticationParameters = authenticationParameters;
			this.ServerParameters = serverParameters;
			this.SmtpParameters = smtpParameters;
			this.PopConnection = Pop3Connection.CreateInstance(connectionParameters);
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

		internal Dictionary<string, int> UniqueIdMap
		{
			get
			{
				return this.uniqueIdMap;
			}
		}

		internal IPop3Connection PopConnection { get; private set; }

		private protected Pop3ServerParameters ServerParameters { protected get; private set; }

		private protected SmtpServerParameters SmtpParameters { protected get; private set; }

		private protected Pop3AuthenticationParameters AuthenticationParameters { protected get; private set; }

		private protected ConnectionParameters ConnectionParameters { protected get; private set; }

		public override SyncProtocol GetSyncProtocol()
		{
			return SyncProtocol.Pop;
		}

		void IMailbox.Config(IReservation reservation, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, MailboxType mbxType, Guid? mailboxContainerGuid)
		{
			base.CheckDisposed();
			MrsTracer.Provider.Function("PopMailbox.IMailbox.Config", new object[0]);
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
			MrsTracer.Provider.Function("PopMailbox.IMailbox.IsConnected", new object[0]);
			return this.PopConnection.ConnectionContext.Client != null;
		}

		bool IMailbox.IsCapabilitySupported(MRSProxyCapabilities capability)
		{
			return true;
		}

		bool IMailbox.IsMailboxCapabilitySupported(MailboxCapabilities capability)
		{
			return false;
		}

		MailboxInformation IMailbox.GetMailboxInformation()
		{
			MrsTracer.Provider.Function("PopMailbox.IMailbox.GetMailboxInformation", new object[0]);
			return new MailboxInformation
			{
				ProviderName = "PopProvider"
			};
		}

		void IMailbox.Connect(MailboxConnectFlags connectFlags)
		{
			base.CheckDisposed();
			MrsTracer.Provider.Function("PopMailbox.IMailbox.Connect", new object[0]);
			this.PopConnection.ConnectAndAuthenticate(this.ServerParameters, this.AuthenticationParameters);
			this.AfterConnect();
			MrsTracer.Provider.Debug("PopClient.Connect: Pop Connection, Authentication and Capabilities check succeeded", new object[0]);
		}

		void IMailbox.Disconnect()
		{
			base.CheckDisposed();
			MrsTracer.Provider.Function("PopMailbox.IMailbox.Disconnect", new object[0]);
			lock (this.syncRoot)
			{
				if (this.PopConnection != null)
				{
					this.PopConnection.Dispose();
					this.PopConnection = null;
				}
			}
		}

		MailboxServerInformation IMailbox.GetMailboxServerInformation()
		{
			MrsTracer.Provider.Function("PopMailbox.IMailbox.GetMailboxServerInformation", new object[0]);
			return new MailboxServerInformation
			{
				MailboxServerName = this.PopConnection.ConnectionContext.Server
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
			MrsTracer.Provider.Function("PopMailbox.EnumerateFolderHierarchy({0})", new object[]
			{
				flags
			});
			return PopMailbox.folderHierarchy;
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
			return PopMailbox.wellKnownFolders;
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
			MrsTracer.Provider.Function("PopMailbox.IMailbox.CheckServerHealth", new object[0]);
			return ServerHealthStatus.Healthy;
		}

		byte[] IMailbox.GetReceiveFolderEntryId(string msgClass)
		{
			throw new NotImplementedException();
		}

		SessionStatistics IMailbox.GetSessionStatistics(SessionStatisticsFlags statisticsTypes)
		{
			throw new NotImplementedException();
		}

		public FolderRec GetFolderRec(byte[] folderId)
		{
			foreach (FolderRec folderRec in PopMailbox.folderHierarchy)
			{
				if (folderRec.EntryId == folderId)
				{
					return folderRec;
				}
			}
			return null;
		}

		public T GetFolder<T>(byte[] folderId) where T : PopFolder, new()
		{
			MrsTracer.Provider.Function("PopMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(folderId)
			});
			T result = Activator.CreateInstance<T>();
			result.Config(folderId, this);
			return result;
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
			return DisposeTracker.Get<PopMailbox>(this);
		}

		private const string IpmSubtreeDisplayName = "Top of Information Store";

		private const string ProviderName = "PopProvider";

		internal static readonly string IpmSubtree = WellKnownFolderType.IpmSubtree.ToString();

		internal static readonly string Inbox = WellKnownFolderType.Inbox.ToString();

		internal static readonly byte[] IpmSubtreeEntryId = PopEntryId.CreateFolderEntryId(PopMailbox.IpmSubtree);

		internal static readonly byte[] InboxEntryId = PopEntryId.CreateFolderEntryId(PopMailbox.Inbox);

		private static readonly string Root = WellKnownFolderType.Root.ToString();

		private static readonly byte[] RootEntryId = PopEntryId.CreateFolderEntryId(PopMailbox.Root);

		private static readonly List<FolderRec> folderHierarchy = new List<FolderRec>
		{
			new FolderRec(PopMailbox.RootEntryId, null, FolderType.Root, string.Empty, DateTime.MinValue, null),
			new FolderRec(PopMailbox.IpmSubtreeEntryId, PopMailbox.RootEntryId, FolderType.Generic, "Top of Information Store", DateTime.MinValue, null),
			new FolderRec(PopMailbox.InboxEntryId, PopMailbox.IpmSubtreeEntryId, FolderType.Generic, PopMailbox.Inbox, DateTime.MinValue, null)
		};

		private static readonly List<WellKnownFolder> wellKnownFolders = new List<WellKnownFolder>
		{
			new WellKnownFolder(3, PopMailbox.IpmSubtreeEntryId),
			new WellKnownFolder(10, PopMailbox.InboxEntryId)
		};

		private readonly object syncRoot = new object();

		private Dictionary<string, int> uniqueIdMap = new Dictionary<string, int>();
	}
}
