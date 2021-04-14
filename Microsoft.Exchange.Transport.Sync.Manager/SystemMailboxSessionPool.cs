using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SystemMailboxSessionPool : Pool<MailboxSession>
	{
		internal SystemMailboxSessionPool(int capacity, int maxCapacity, Guid databaseGuid, Guid systemMailboxGuid) : base(capacity, maxCapacity, ContentAggregationConfig.MaxSystemMailboxSessionsUnusedPeriod)
		{
			this.databaseGuid = databaseGuid;
			this.systemMailboxGuid = systemMailboxGuid;
		}

		internal MailboxSession GetSystemMailbox(string clientConnectionString)
		{
			SyncUtilities.ThrowIfGuidEmpty("systemMailboxGuid", this.systemMailboxGuid);
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromLocalServerMailboxGuid(ADSessionSettings.FromRootOrgScopeSet(), this.databaseGuid, this.systemMailboxGuid);
			return MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, clientConnectionString, true);
		}

		protected override MailboxSession CreateItem(out bool needsBackOff)
		{
			needsBackOff = false;
			try
			{
				return this.GetSystemMailbox(SystemMailboxSessionPool.ClientInfoString);
			}
			catch (StorageTransientException ex)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)160UL, SystemMailboxSessionPool.Tracer, (long)this.GetHashCode(), "CreateItem: Encountered a transient exception when trying to get the system mailbox for database {0}: {1}", new object[]
				{
					this.databaseGuid,
					ex
				});
			}
			catch (StoragePermanentException ex2)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)161UL, SystemMailboxSessionPool.Tracer, (long)this.GetHashCode(), "CreateItem: Encountered a permanent exception when trying to get the system mailbox for database {0}: {1}", new object[]
				{
					this.databaseGuid,
					ex2
				});
			}
			needsBackOff = true;
			return null;
		}

		protected override void DestroyItem(MailboxSession item)
		{
			try
			{
				item.Dispose();
			}
			catch (StorageTransientException ex)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)162UL, SystemMailboxSessionPool.Tracer, (long)this.GetHashCode(), "DestroyItem: Encountered a transient exception when trying to destroy the system mailbox for database {0}: {1}", new object[]
				{
					this.databaseGuid,
					ex
				});
			}
			catch (StoragePermanentException ex2)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)163UL, SystemMailboxSessionPool.Tracer, (long)this.GetHashCode(), "DestroyItem: Encountered a permanent exception when trying to destroy the system mailbox for database {0}: {1}", new object[]
				{
					this.databaseGuid,
					ex2
				});
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.SystemMailboxSessionPoolTracer;

		private static readonly string ClientInfoString = "Client=TransportSync;Action=SystemMailboxSessionPool";

		private readonly Guid databaseGuid;

		private Guid systemMailboxGuid;
	}
}
