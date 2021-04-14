using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class NotificationSubscription
	{
		protected NotificationSubscription(SubscriptionKind kind, NotificationContext notificationContext, StoreDatabase database, int mailboxNumber, int eventTypeValueMask, NotificationCallback callback)
		{
			this.kind = kind;
			this.notificationContext = notificationContext;
			this.mailboxNumber = mailboxNumber;
			this.database = database;
			this.eventTypeValueMask = eventTypeValueMask;
			this.callback = callback;
			this.userIdentityContext = ((notificationContext != null && notificationContext.Session != null) ? new Guid?(notificationContext.Session.UserGuid) : null);
		}

		public SubscriptionKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		public NotificationContext NotificationContext
		{
			get
			{
				return this.notificationContext;
			}
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		public int EventTypeValueMask
		{
			get
			{
				return this.eventTypeValueMask;
			}
		}

		public static void PumpOneNotificationInCurrentContext(Context transactionContext, NotificationEvent nev)
		{
			NotificationSubscription.EnumerateSubscriptionsForEvent(NotificationPublishPhase.Pumping, transactionContext, nev, NotificationSubscription.PumpingPublishCallback);
		}

		internal static void EnumerateSubscriptionsForEvent(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev, SubscriptionEnumerationCallback callback)
		{
			NotificationSubscription.GetGlobalSubscriptions().EnumerateSubscriptionsForEvent(phase, transactionContext, nev, callback);
			if (nev.MailboxNumber != 0)
			{
				INotificationSubscriptionList mailboxSubscriptions = Mailbox.GetMailboxSubscriptions(transactionContext, nev.MailboxNumber);
				if (mailboxSubscriptions != null)
				{
					mailboxSubscriptions.EnumerateSubscriptionsForEvent(phase, transactionContext, nev, callback);
				}
			}
		}

		internal static void PublishNotificationWhilePumping(NotificationPublishPhase phase, Context transactionContext, NotificationSubscription subscription, NotificationEvent nev)
		{
			if (subscription.NotificationContext == NotificationContext.Current && subscription.IsUserInterested(transactionContext, nev) && subscription.IsInterested(nev))
			{
				subscription.PublishEvent(phase, transactionContext, nev);
			}
		}

		internal static INotificationSubscriptionList GetGlobalSubscriptions()
		{
			return NotificationSubscription.globalSubscriptions;
		}

		private static void PublishNotificationPreCommit(NotificationPublishPhase phase, Context transactionContext, NotificationSubscription subscription, NotificationEvent nev)
		{
			if ((subscription.NotificationContext == null || subscription.NotificationContext == NotificationContext.Current) && subscription.IsUserInterested(transactionContext, nev) && subscription.IsInterested(nev))
			{
				subscription.PublishEvent(phase, transactionContext, nev);
			}
		}

		private static void PublishNotificationPostCommit(NotificationPublishPhase phase, Context transactionContext, NotificationSubscription subscription, NotificationEvent nev)
		{
			if (subscription.IsUserInterested(transactionContext, nev) && subscription.IsInterested(nev))
			{
				if (subscription.NotificationContext == null || subscription.NotificationContext == NotificationContext.Current)
				{
					subscription.PublishEvent(phase, transactionContext, nev);
					return;
				}
				subscription.NotificationContext.EnqueueEvent(nev);
			}
		}

		private bool IsUserInterested(Context transactionContext, NotificationEvent nev)
		{
			return nev.UserIdentityContext == null || this.userIdentityContext == null || this.userIdentityContext == nev.UserIdentityContext;
		}

		public abstract bool IsInterested(NotificationEvent nev);

		public void Register(Context context)
		{
			INotificationSubscriptionList mailboxSubscriptions;
			if (this.MailboxNumber == 0)
			{
				mailboxSubscriptions = NotificationSubscription.GetGlobalSubscriptions();
			}
			else
			{
				mailboxSubscriptions = Mailbox.GetMailboxSubscriptions(context, this.MailboxNumber);
			}
			mailboxSubscriptions.RegisterSubscription(this);
			this.registeredInList = mailboxSubscriptions;
		}

		public void Unregister()
		{
			if (this.registeredInList != null)
			{
				this.registeredInList.UnregisterSubscription(this);
				this.registeredInList = null;
			}
		}

		internal void PublishEvent(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev)
		{
			this.callback(phase, transactionContext, nev);
		}

		protected abstract void AppendClassName(StringBuilder sb);

		protected virtual void AppendFields(StringBuilder sb)
		{
			sb.Append("Kind:[");
			sb.Append(this.Kind);
			sb.Append("] MailboxGuid:[");
			sb.Append(this.MailboxNumber);
			sb.Append("] EventTypeValueMask:[");
			sb.Append(this.EventTypeValueMask);
			sb.Append("] NotificationContext:[");
			sb.Append(this.NotificationContext);
			sb.Append("] Callback:[");
			sb.Append(this.callback);
			sb.Append("]");
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(250);
			this.AppendClassName(stringBuilder);
			stringBuilder.Append(":[");
			this.AppendFields(stringBuilder);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private const int AvgInterestedSubscriptionsForEvent = 10;

		private static NotificationSubscription.GlobalNotificationSubscriptionList globalSubscriptions = new NotificationSubscription.GlobalNotificationSubscriptionList();

		internal static readonly SubscriptionEnumerationCallback PreCommitPublishCallback = new SubscriptionEnumerationCallback(NotificationSubscription.PublishNotificationPreCommit);

		internal static readonly SubscriptionEnumerationCallback PostCommitPublishCallback = new SubscriptionEnumerationCallback(NotificationSubscription.PublishNotificationPostCommit);

		internal static readonly SubscriptionEnumerationCallback PumpingPublishCallback = new SubscriptionEnumerationCallback(NotificationSubscription.PublishNotificationWhilePumping);

		private SubscriptionKind kind;

		private NotificationContext notificationContext;

		private int mailboxNumber;

		private StoreDatabase database;

		private int eventTypeValueMask;

		private NotificationCallback callback;

		private INotificationSubscriptionList registeredInList;

		private Guid? userIdentityContext;

		internal class GlobalNotificationSubscriptionList : INotificationSubscriptionList
		{
			internal NotificationSubscription SingleGlobalSubscription
			{
				get
				{
					return this.singleGlobalSubscription;
				}
				set
				{
					this.singleGlobalSubscription = value;
				}
			}

			public void RegisterSubscription(NotificationSubscription subscription)
			{
				this.singleGlobalSubscription = subscription;
			}

			public void UnregisterSubscription(NotificationSubscription subscription)
			{
				this.singleGlobalSubscription = null;
			}

			public void EnumerateSubscriptionsForEvent(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev, SubscriptionEnumerationCallback callback)
			{
				NotificationSubscription notificationSubscription = this.singleGlobalSubscription;
				if (notificationSubscription != null && (notificationSubscription.EventTypeValueMask & nev.EventTypeValue) != 0 && (notificationSubscription.Kind & (SubscriptionKind)phase) != (SubscriptionKind)0U)
				{
					if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.NotificationTracer.TraceDebug<NotificationEvent>(36857L, "GlobalNotifEnumeration: {0}", nev);
					}
					callback(phase, transactionContext, notificationSubscription, nev);
					return;
				}
				if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.NotificationTracer.TraceDebug<NotificationEvent, NotificationSubscription>(30628L, "GlobalNotifEnumeration: Skipping callback for {0}, {1}", nev, notificationSubscription);
				}
			}

			private NotificationSubscription singleGlobalSubscription;
		}
	}
}
