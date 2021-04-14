using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxManager
	{
		internal MailboxManager(Guid databaseGuid)
		{
			this.databaseGuid = databaseGuid;
		}

		internal MailboxSession GetUserMailboxSession(Guid mailboxGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			MailboxSession result = null;
			try
			{
				result = SyncUtilities.OpenMailboxSessionAndHaveCompleteExchangePrincipal(mailboxGuid, this.databaseGuid, (IExchangePrincipal mailboxOwner) => MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, MailboxManager.ClientInfoString, true));
			}
			catch (TransientException exception)
			{
				bool flag;
				Exception ex = this.ConvertToCacheException(mailboxGuid, exception, out flag);
				throw ex;
			}
			catch (LocalServerException exception2)
			{
				bool flag2;
				Exception ex2 = this.ConvertToCacheException(mailboxGuid, exception2, out flag2);
				throw ex2;
			}
			catch (DataValidationException exception3)
			{
				bool flag3;
				Exception ex3 = this.ConvertToCacheException(mailboxGuid, exception3, out flag3);
				throw ex3;
			}
			catch (DataSourceOperationException exception4)
			{
				bool flag4;
				Exception ex4 = this.ConvertToCacheException(mailboxGuid, exception4, out flag4);
				throw ex4;
			}
			catch (StoragePermanentException exception5)
			{
				bool flag5;
				Exception ex5 = this.ConvertToCacheException(mailboxGuid, exception5, out flag5);
				if (ex5 is MailboxNotFoundException)
				{
					return null;
				}
				throw ex5;
			}
			return result;
		}

		internal void CrawlUserMailbox(MailboxSession mailboxSession, out IList<AggregationSubscription> foundSubscriptions, out Guid? tenantGuid, out Guid? externalDirectoryOrgId)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			foundSubscriptions = null;
			tenantGuid = null;
			externalDirectoryOrgId = null;
			try
			{
				foundSubscriptions = SubscriptionManager.GetAllSubscriptions(mailboxSession, AggregationSubscriptionType.All);
			}
			catch (TransientException exception)
			{
				bool flag;
				Exception ex = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception, out flag);
				throw ex;
			}
			catch (InvalidDataException exception2)
			{
				bool flag2;
				Exception ex2 = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception2, out flag2);
				throw ex2;
			}
			catch (DataSourceOperationException exception3)
			{
				bool flag3;
				Exception ex3 = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception3, out flag3);
				throw ex3;
			}
			catch (MapiPermanentException exception4)
			{
				bool flag4;
				Exception ex4 = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception4, out flag4);
				throw ex4;
			}
			catch (StoragePermanentException exception5)
			{
				bool flag5;
				Exception ex5 = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception5, out flag5);
				throw ex5;
			}
			tenantGuid = new Guid?(mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.GetTenantGuid());
			externalDirectoryOrgId = new Guid?(this.ResolveExternalDirectoryOrgId(mailboxSession));
			ManagerPerfCounterHandler.Instance.IncrementCacheMessagesRebuilt();
		}

		private Guid ResolveExternalDirectoryOrgId(MailboxSession mailboxSession)
		{
			Guid result = TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg;
			try
			{
				byte[] persistableTenantPartitionHint = mailboxSession.PersistableTenantPartitionHint;
				if (persistableTenantPartitionHint != null)
				{
					TenantPartitionHint tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(persistableTenantPartitionHint);
					result = tenantPartitionHint.GetExternalDirectoryOrganizationId();
				}
				else
				{
					MailboxManager.Tracer.TraceDebug(0L, "PersistableTenantPartitionHint is null in the mailbox session, default to RootOrg ExternalDirectoryOrgId");
				}
			}
			catch (CannotResolveTenantNameException arg)
			{
				MailboxManager.Tracer.TraceError<CannotResolveTenantNameException>(0L, "Failed to resolveExternalDirectoryOrgId: {0}", arg);
			}
			catch (ADTransientException arg2)
			{
				MailboxManager.Tracer.TraceError<ADTransientException>(0L, "Failed to resolveExternalDirectoryOrgId: {0}", arg2);
			}
			catch (ArgumentNullException arg3)
			{
				MailboxManager.Tracer.TraceError<ArgumentNullException>(0L, "Failed to resolveExternalDirectoryOrgId: {0}", arg3);
			}
			return result;
		}

		private Exception ConvertToCacheException(Guid mailboxGuid, Exception exception, out bool reuseSession)
		{
			reuseSession = false;
			return GlobalDatabaseHandler.ConvertToCacheException(MailboxManager.Tracer, this.GetHashCode(), this.databaseGuid, mailboxGuid, exception, out reuseSession);
		}

		private static readonly Trace Tracer = ExTraceGlobals.MailboxManagerTracer;

		private static readonly string ClientInfoString = "Client=TransportSync;Action=MailboxManager";

		private Guid databaseGuid;
	}
}
