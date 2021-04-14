using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;

namespace Microsoft.Exchange.Entities.Calendaring.ConsumerSharing
{
	internal class ConsumerCalendarSynchronizer
	{
		public ConsumerCalendarSynchronizer(IMailboxSession mailboxSession, IXSOFactory xsoFactory, ITracer tracer)
		{
			this.MailboxSession = mailboxSession;
			this.XsoFactory = xsoFactory;
			this.Tracer = tracer;
		}

		public event EventHandler<string> LogError;

		public IMailboxSession MailboxSession { get; private set; }

		public IXSOFactory XsoFactory { get; private set; }

		public ITracer Tracer { get; private set; }

		public SyncResult Synchronize(StoreObjectId folderId, Deadline deadline)
		{
			this.Tracer.TraceDebug<object, StoreObjectId, string>((long)this.GetHashCode(), "{0}: ConsumerCalendarSharingEngine.Synchronize will try to sync folder {1} for mailbox {2}.", TraceContext.Get(), folderId, this.MailboxSession.DisplayAddress);
			ConsumerCalendarSubscription consumerCalendarSubscription = this.TryGetSubscription(folderId);
			if (consumerCalendarSubscription == null)
			{
				return SyncResult.SubscriptionLost;
			}
			using (ICalendarItem calendarItem = this.XsoFactory.CreateCalendarItem(this.MailboxSession, folderId))
			{
				calendarItem.StartTime = ExDateTime.Now;
				calendarItem.EndTime = calendarItem.StartTime.AddSeconds(1.0);
				calendarItem.Subject = string.Format("Consumer Calendar Sync: OwnerId={0}, CalendarGuid={1}", consumerCalendarSubscription.ConsumerCalendarOwnerId, consumerCalendarSubscription.ConsumerCalendarGuid);
				calendarItem.Save(SaveMode.NoConflictResolutionForceSave);
			}
			return SyncResult.Completed;
		}

		public virtual ConsumerCalendarSubscription TryGetSubscription(StoreObjectId folderId)
		{
			ConsumerCalendarSubscription result;
			using (ICalendarFolder calendarFolder = this.BindToConsumerCalendarFolder(folderId))
			{
				long consumerCalendarOwnerId = calendarFolder.ConsumerCalendarOwnerId;
				if (consumerCalendarOwnerId == 0L)
				{
					this.OnLogError("ConsumerCalendarOwnerId is set to zero.");
					result = null;
				}
				else
				{
					Guid consumerCalendarGuid = calendarFolder.ConsumerCalendarGuid;
					if (consumerCalendarGuid == Guid.Empty)
					{
						this.OnLogError("ConsumerCalendarGuid is empty.");
						result = null;
					}
					else
					{
						result = new ConsumerCalendarSubscription(consumerCalendarOwnerId, consumerCalendarGuid);
					}
				}
			}
			return result;
		}

		protected ICalendarFolder BindToConsumerCalendarFolder(StoreObjectId folderId)
		{
			ICalendarFolder calendarFolder = this.XsoFactory.BindToCalendarFolder(this.MailboxSession, folderId);
			calendarFolder.Load(CalendarFolderSchema.ConsumerCalendarProperties);
			return calendarFolder;
		}

		protected void OnLogError(string error)
		{
			EventHandler<string> logError = this.LogError;
			if (logError != null)
			{
				logError(this, error);
			}
		}
	}
}
