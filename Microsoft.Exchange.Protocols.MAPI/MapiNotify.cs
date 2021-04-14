using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiNotify : MapiBase
	{
		public MapiNotify() : base(MapiObjectType.Notify)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiNotify>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.subscription != null)
			{
				this.subscription.Unregister();
			}
			base.InternalDispose(calledFromDispose);
		}

		public ErrorCode Configure(MapiContext context, MapiLogon logon, EventType eventTypeMask, bool mailboxGlobal, ExchangeId fid, ExchangeId mid, uint hsot)
		{
			base.Logon = logon;
			ErrorCode errorCode = this.RegisterMapiSubscription(context, eventTypeMask, mailboxGlobal, fid, mid);
			if (errorCode == ErrorCode.NoError)
			{
				base.ParentObject = base.Logon;
				this.hsot = hsot;
				base.IsValid = true;
			}
			return errorCode;
		}

		private ErrorCode RegisterMapiSubscription(MapiContext context, EventType eventTypeMask, bool mailboxGlobal, ExchangeId fid, ExchangeId mid)
		{
			if (mailboxGlobal)
			{
				this.subscription = new MailboxGlobalSubscription(SubscriptionKind.PostCommit, base.Logon.Session.NotificationContext, base.Logon.MapiMailbox.Database, base.Logon.MapiMailbox.MailboxNumber, eventTypeMask, new NotificationCallback(this.OnNotification));
			}
			else if (mid.IsNullOrZero)
			{
				this.subscription = new FolderSubscription(SubscriptionKind.PostCommit, base.Logon.Session.NotificationContext, base.Logon.MapiMailbox.Database, base.Logon.MapiMailbox.MailboxNumber, eventTypeMask, new NotificationCallback(this.OnNotification), fid);
			}
			else
			{
				this.subscription = new MessageSubscription(SubscriptionKind.PostCommit, base.Logon.Session.NotificationContext, base.Logon.MapiMailbox.Database, base.Logon.MapiMailbox.MailboxNumber, eventTypeMask, new NotificationCallback(this.OnNotification), fid, mid);
			}
			if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.NotificationTracer.TraceDebug<NotificationSubscription>(30568L, "Register MAPI Subscription: {0}", this.subscription);
			}
			this.subscription.Register(context);
			return ErrorCode.NoError;
		}

		private void OnNotification(NotificationPublishPhase phase, Context transactionContext, NotificationEvent nev)
		{
			MapiContext mapiContext = transactionContext as MapiContext;
			using (mapiContext.SetMapiLogonForNotificationContext(base.Logon))
			{
				if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ExTraceGlobals.NotificationTracer.TraceFunction<uint>(0L, "ENTER MapiNotify.OnNotification: hsot:[{0}]", this.hsot);
				}
				try
				{
					ObjectNotificationEvent objectNotificationEvent = nev as ObjectNotificationEvent;
					if (objectNotificationEvent != null)
					{
						if ((objectNotificationEvent.EventFlags & EventFlags.Conversation) != EventFlags.None)
						{
							base.TraceNotificationIgnored(nev, "conversation notifications are not currently supported");
							return;
						}
						if (objectNotificationEvent.ExtendedEventFlags != null && (objectNotificationEvent.ExtendedEventFlags.Value & ExtendedEventFlags.InternalAccessFolder) != ExtendedEventFlags.None)
						{
							base.TraceNotificationIgnored(nev, "InternalAccess");
							return;
						}
					}
					else
					{
						MailboxModifiedNotificationEvent mailboxModifiedNotificationEvent = nev as MailboxModifiedNotificationEvent;
						if (mailboxModifiedNotificationEvent != null)
						{
							nev = new FolderModifiedNotificationEvent(mailboxModifiedNotificationEvent.Database, mailboxModifiedNotificationEvent.MailboxNumber, mailboxModifiedNotificationEvent.UserIdentity, mailboxModifiedNotificationEvent.ClientType, mailboxModifiedNotificationEvent.EventFlags, ExtendedEventFlags.None, base.Logon.FidC.FidRoot, ExchangeId.Zero, null, string.Empty, 0, 0);
						}
					}
					base.Logon.AddPendingNotification(nev, this, this.hsot);
				}
				finally
				{
					if (ExTraceGlobals.NotificationTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						ExTraceGlobals.NotificationTracer.TraceFunction<uint>(0L, "EXIT MapiNotify.OnNotification: hsot:[{0}]", this.hsot);
					}
				}
			}
		}

		private uint hsot = uint.MaxValue;

		private NotificationSubscription subscription;
	}
}
