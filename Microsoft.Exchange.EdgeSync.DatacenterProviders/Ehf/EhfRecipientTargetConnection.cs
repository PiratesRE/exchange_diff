using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.EdgeSync.Logging;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfRecipientTargetConnection : EhfTargetConnection
	{
		public EhfRecipientTargetConnection(int localServerVersion, EhfTargetServerConfig config, EhfSynchronizationProvider provider, EdgeSyncLogSession logSession) : base(localServerVersion, config, provider.RecipientSyncInterval, logSession)
		{
			this.provider = provider;
		}

		public EhfRecipientTargetConnection(int localServerVersion, EhfTargetServerConfig config, EdgeSyncLogSession logSession, EhfPerfCounterHandler perfCounterHandler, IAdminSyncService adminSyncservice, EhfADAdapter adapter, EnhancedTimeSpan syncInterval, EhfSynchronizationProvider provider) : base(localServerVersion, config, logSession, perfCounterHandler, null, null, adminSyncservice, adapter, syncInterval)
		{
			this.provider = provider;
		}

		public EhfAdminAccountSynchronizer AdminAccountSynchronizer
		{
			get
			{
				return this.adminSync;
			}
		}

		public override bool SkipSyncCycle
		{
			get
			{
				return !base.Config.AdminSyncEnabled;
			}
		}

		public override void AbortSyncCycle(Exception cause)
		{
			if (this.adminSync != null)
			{
				this.adminSync.ClearBatches();
			}
			base.AbortSyncCycle(cause);
		}

		public override bool OnSynchronizing()
		{
			bool result = base.OnSynchronizing();
			if (this.adminSync != null)
			{
				throw new InvalidOperationException("Admin sync already exists");
			}
			this.provider.AdminSyncErrorTracker.PrepareForNewSyncCycle(this.SyncInterval);
			this.adminSync = new EhfAdminAccountSynchronizer(this, this.provider.AdminSyncErrorTracker);
			return result;
		}

		public override bool OnSynchronized()
		{
			if (this.adminSync != null && !this.adminSync.FlushBatches())
			{
				return false;
			}
			this.provider.AdminSyncErrorTracker.AbortSyncCycleIfRequired(this.adminSync, base.DiagSession);
			return true;
		}

		public override SyncResult OnAddEntry(ExSearchResultEntry entry, SortedList<string, DirectoryAttribute> sourceAttributes)
		{
			SyncResult result = SyncResult.Added;
			EhfRecipientTargetConnection.AdminObjectType syncObjectType = this.GetSyncObjectType(entry, "Add");
			switch (syncObjectType)
			{
			case EhfRecipientTargetConnection.AdminObjectType.MailboxUser:
				this.adminSync.HandleWlidAddedEvent(entry);
				break;
			case EhfRecipientTargetConnection.AdminObjectType.Organization:
				this.adminSync.HandleOrganizationAddedEvent(entry);
				break;
			case EhfRecipientTargetConnection.AdminObjectType.UniversalSecurityGroup:
				this.adminSync.HandleGroupAddedEvent(entry);
				break;
			default:
				throw new InvalidOperationException("EhfRecipientTargetConnection.GetSyncObjectType() returned unexpected value " + syncObjectType);
			}
			return result;
		}

		public override SyncResult OnModifyEntry(ExSearchResultEntry entry, SortedList<string, DirectoryAttribute> sourceAttributes)
		{
			SyncResult result = SyncResult.Modified;
			EhfRecipientTargetConnection.AdminObjectType syncObjectType = this.GetSyncObjectType(entry, "Modify");
			switch (syncObjectType)
			{
			case EhfRecipientTargetConnection.AdminObjectType.MailboxUser:
				this.adminSync.HandleWlidChangedEvent(entry);
				break;
			case EhfRecipientTargetConnection.AdminObjectType.Organization:
				this.adminSync.HandleOrganizationChangedEvent(entry);
				break;
			case EhfRecipientTargetConnection.AdminObjectType.UniversalSecurityGroup:
				this.adminSync.HandleGroupChangedEvent(entry);
				break;
			default:
				throw new InvalidOperationException("EhfRecipientTargetConnection.GetSyncObjectType() returned unexpected value " + syncObjectType);
			}
			return result;
		}

		public override SyncResult OnDeleteEntry(ExSearchResultEntry entry)
		{
			EhfRecipientTargetConnection.AdminObjectType syncObjectType = this.GetSyncObjectType(entry, "Delete");
			SyncResult result;
			switch (syncObjectType)
			{
			case EhfRecipientTargetConnection.AdminObjectType.MailboxUser:
				this.adminSync.HandleWlidDeletedEvent(entry);
				result = SyncResult.Deleted;
				break;
			case EhfRecipientTargetConnection.AdminObjectType.Organization:
				this.adminSync.HandleOrganizationDeletedEvent(entry);
				result = SyncResult.Deleted;
				break;
			case EhfRecipientTargetConnection.AdminObjectType.UniversalSecurityGroup:
				this.adminSync.HandleGroupDeletedEvent(entry);
				result = SyncResult.Deleted;
				break;
			default:
				throw new InvalidOperationException("EhfRecipientTargetConnection.GetSyncObjectType() returned unexpected value " + syncObjectType);
			}
			return result;
		}

		private EhfRecipientTargetConnection.AdminObjectType GetSyncObjectType(ExSearchResultEntry entry, string operation)
		{
			string objectClass = entry.ObjectClass;
			if (string.IsNullOrEmpty(objectClass))
			{
				base.DiagSession.LogAndTraceError("Entry <{0}> contains no objectClass attribute in operation {1}; cannot proceed", new object[]
				{
					entry.DistinguishedName,
					operation
				});
				throw new ArgumentException("Entry does not contain objectClass attribute", "entry");
			}
			string a;
			if ((a = objectClass) != null)
			{
				if (a == "group")
				{
					return EhfRecipientTargetConnection.AdminObjectType.UniversalSecurityGroup;
				}
				if (a == "user")
				{
					return EhfRecipientTargetConnection.AdminObjectType.MailboxUser;
				}
				if (a == "organizationalUnit")
				{
					return EhfRecipientTargetConnection.AdminObjectType.Organization;
				}
			}
			base.DiagSession.LogAndTraceError("Entry <{0}> contains unexpected objectClass value <{1}> in operation {2}; cannot proceed", new object[]
			{
				entry.DistinguishedName,
				objectClass,
				operation
			});
			throw new ArgumentException("Entry's objectClass is invalid: " + objectClass, "entry");
		}

		private EhfAdminAccountSynchronizer adminSync;

		private EhfSynchronizationProvider provider;

		private enum AdminObjectType
		{
			MailboxUser,
			Organization,
			UniversalSecurityGroup,
			Unknown
		}
	}
}
