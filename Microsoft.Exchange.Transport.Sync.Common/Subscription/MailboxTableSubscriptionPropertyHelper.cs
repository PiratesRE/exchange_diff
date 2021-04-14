using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxTableSubscriptionPropertyHelper
	{
		public MailboxTableSubscriptionPropertyHelper() : this(CommonLoggingHelper.SyncLogSession, ExTraceGlobals.SubscriptionManagerTracer)
		{
		}

		internal MailboxTableSubscriptionPropertyHelper(SyncLogSession syncLogSession, Trace tracer)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNull("tracer", tracer);
			this.syncLogSession = syncLogSession;
			this.tracer = tracer;
		}

		public virtual bool TryUpdateSubscriptionListTimestamp(MailboxSession mailboxSession)
		{
			Exception ex2;
			try
			{
				this.UpdateSubscriptionListTimestamp(mailboxSession);
				return true;
			}
			catch (InvalidOperationException ex)
			{
				ex2 = ex;
			}
			catch (MapiRetryableException ex3)
			{
				ex2 = ex3;
			}
			catch (MapiPermanentException ex4)
			{
				ex2 = ex4;
			}
			this.syncLogSession.LogError((TSLID)75UL, this.tracer, "UpdateSubscriptionListTimestamp for mailbox {0} failed with {1}", new object[]
			{
				mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid,
				ex2
			});
			return false;
		}

		public virtual void UpdateSubscriptionListTimestamp(MailboxSession mailboxSession)
		{
			MapiStore mapiStore = mailboxSession.Mailbox.MapiStore;
			DateTime utcNow = DateTime.UtcNow;
			PropValue propValue = new PropValue(PropTag.TransportSyncSubscriptionListTimestamp, utcNow);
			PropProblem[] array = mapiStore.SetProps(new PropValue[]
			{
				propValue
			});
			if (array != null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to set proptag={0}: {1}", new object[]
				{
					PropTag.TransportSyncSubscriptionListTimestamp,
					array[0].Scode
				}));
			}
			this.syncLogSession.LogVerbose((TSLID)76UL, this.tracer, "UpdateSubscriptionListTimestamp for mailbox {0}: {1}", new object[]
			{
				mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid,
				utcNow.ToString("MM/dd/yyyy hh:mm:ss.fff")
			});
		}

		public virtual void UpdateContentAggregationFlags(MailboxSession mailboxSession, ContentAggregationFlags contentAggregationFlags)
		{
			MapiStore mapiStore = mailboxSession.Mailbox.MapiStore;
			using (MapiFolder inboxFolder = mapiStore.GetInboxFolder())
			{
				ContentAggregationFlags contentAggregationFlags2 = ContentAggregationFlags.None;
				PropValue prop = inboxFolder.GetProp(PropTag.ContentAggregationFlags);
				if (!prop.IsNull() && !prop.IsError())
				{
					contentAggregationFlags2 = (ContentAggregationFlags)prop.GetInt();
				}
				if (contentAggregationFlags2 != contentAggregationFlags)
				{
					PropProblem[] array = inboxFolder.SetProps(new PropValue[]
					{
						new PropValue(PropTag.ContentAggregationFlags, (int)contentAggregationFlags)
					});
					if (array != null)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to set proptag={0}: {1}", new object[]
						{
							PropTag.ContentAggregationFlags,
							array[0].Scode
						}));
					}
				}
			}
			this.syncLogSession.LogVerbose((TSLID)1392UL, this.tracer, "UpdateContentAggregationFlags for mailbox {0}: {1}", new object[]
			{
				mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid,
				contentAggregationFlags
			});
		}

		private readonly SyncLogSession syncLogSession;

		private readonly Trace tracer;
	}
}
