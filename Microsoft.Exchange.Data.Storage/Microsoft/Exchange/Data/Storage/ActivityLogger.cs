using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActivityLogger : IActivityLogger
	{
		public static IActivityLogger Create(IMailboxSession mailboxSession)
		{
			if (!ActivityLogger.IsLoggingEnabled)
			{
				return null;
			}
			return new ActivityLogger(mailboxSession);
		}

		public static IActivityLogger Create()
		{
			return ActivityLogger.Create(null);
		}

		private ActivityLogger(IMailboxSession mailboxSession)
		{
			MailboxSession mailboxSession2 = mailboxSession as MailboxSession;
			if (mailboxSession2 != null)
			{
				this.storeLogger = ActivityLogFactory.Current.Bind(mailboxSession2);
			}
			if (this.storeLogger != null)
			{
				this.storeEligibleActivities = (this.storeLogger.IsGroup() ? ActivityLogger.GroupLoggedToStoreActivityIds : ActivityLogger.LoggedToStoreActivityIds);
			}
		}

		public void Log(IEnumerable<Activity> activities)
		{
			if (activities == null || !activities.Any<Activity>())
			{
				throw new ArgumentException("activities enumerable cannot be null or empty");
			}
			List<Activity> activitiesCache = (from activity in activities
			where activity != null
			select activity).ToList<Activity>();
			ActivityLogger.activityPerfCounters.ActivityLogsActivityCount.IncrementBy((long)activitiesCache.Count);
			if (this.storeLogger != null)
			{
				bool flag = ActivityLogHelper.CatchNonFatalExceptions(delegate
				{
					this.storeLogger.Append(this.GetStoreEligibleActivities(activitiesCache));
				}, null);
				if (flag)
				{
					ActivityLogger.activityPerfCounters.ActivityLogsStoreWriteExceptions.Increment();
				}
			}
			ActivityFileLogger.Instance.Log(activitiesCache);
		}

		private IEnumerable<Activity> GetStoreEligibleActivities(IEnumerable<Activity> activities)
		{
			foreach (Activity activity in activities)
			{
				if (this.storeEligibleActivities.Contains(activity.Id))
				{
					ActivityLogger.activityPerfCounters.ActivityLogsSelectedForStore.Increment();
					yield return activity;
				}
			}
			yield break;
		}

		internal static readonly HashSet<ActivityId> LoggedToStoreActivityIds = new HashSet<ActivityId>
		{
			ActivityId.Categorize,
			ActivityId.ClutterGroupClosed,
			ActivityId.ClutterGroupOpened,
			ActivityId.Delete,
			ActivityId.Flag,
			ActivityId.FlagCleared,
			ActivityId.FlagComplete,
			ActivityId.Forward,
			ActivityId.Logon,
			ActivityId.MarkAsRead,
			ActivityId.MarkAsUnread,
			ActivityId.Move,
			ActivityId.NewMessage,
			ActivityId.ReadingPaneDisplayStart,
			ActivityId.ReadingPaneDisplayEnd,
			ActivityId.Reply,
			ActivityId.ReplyAll,
			ActivityId.MarkMessageAsClutter,
			ActivityId.MarkMessageAsNotClutter,
			ActivityId.MoveFromInbox,
			ActivityId.MoveToInbox,
			ActivityId.MoveFromClutter,
			ActivityId.MoveToClutter,
			ActivityId.DeleteFromInbox,
			ActivityId.DeleteFromClutter,
			ActivityId.RemoteSend,
			ActivityId.CreateCalendarEvent,
			ActivityId.UpdateCalendarEvent,
			ActivityId.ServerLogon
		};

		internal static readonly HashSet<ActivityId> GroupLoggedToStoreActivityIds = new HashSet<ActivityId>
		{
			ActivityId.Forward,
			ActivityId.Like,
			ActivityId.ReadingPaneDisplayStart,
			ActivityId.ReadingPaneDisplayEnd,
			ActivityId.Reply,
			ActivityId.ReplyAll
		};

		private static readonly bool IsLoggingEnabled = ActivityLogHelper.IsActivityLoggingEnabled(false);

		private static MiddleTierStoragePerformanceCountersInstance activityPerfCounters = NamedPropMap.GetPerfCounters();

		private readonly HashSet<ActivityId> storeEligibleActivities;

		private readonly IActivityLog storeLogger;
	}
}
