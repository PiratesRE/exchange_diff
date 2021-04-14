using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal sealed class DeferredSendHandler : ITimedEventHandler
	{
		internal static IDisposable SetDeferredSendEventTestHook(Action action)
		{
			return DeferredSendHandler.deferredSendEventTestHook.SetTestHook(action);
		}

		public void Invoke(Context context, TimedEventEntry timedEvent)
		{
			ExTraceGlobals.DeferredSendTracer.TraceDebug(36168L, "Entering DeferredSendHandler.Invoke");
			if (ExTraceGlobals.DeferredSendTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.DeferredSendTracer.TraceDebug(52552L, "Raw timed event=" + timedEvent);
			}
			DeferredSendEvent deferredSendEvent = this.ExtractDeferredSendEvent(timedEvent);
			if (deferredSendEvent == null)
			{
				ExTraceGlobals.DeferredSendTracer.TraceError(46408L, "Unable to extract the deferred send event data from the raw timed event entry");
				return;
			}
			this.GenerateMailSubmittedEvent(context, deferredSendEvent);
			ExTraceGlobals.DeferredSendTracer.TraceDebug(62792L, "Leaving DeferredSendHandler.Invoke");
		}

		private DeferredSendEvent ExtractDeferredSendEvent(TimedEventEntry timedEvent)
		{
			ExTraceGlobals.DeferredSendTracer.TraceDebug(38216L, "Entering DeferredSendHandler.ExtractDeferredSendEventFromTimedEvent");
			if (timedEvent.MailboxNumber == null || timedEvent.MailboxNumber.Value == -1)
			{
				if (ExTraceGlobals.DeferredSendTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.DeferredSendTracer.TraceError(54600L, "Deferred send event does not include a valid mailboxNumber: " + timedEvent.ToString());
				}
				return null;
			}
			if (timedEvent.EventData == null)
			{
				if (ExTraceGlobals.DeferredSendTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.DeferredSendTracer.TraceError(42312L, "Deferred send event does not include event data part: " + timedEvent.ToString());
				}
				return null;
			}
			long num;
			long num2;
			if (!DeferredSendEvent.DeserializeExtraData(timedEvent.EventData, out num, out num2))
			{
				ExTraceGlobals.DeferredSendTracer.TraceError(58696L, "Invalid extra data in the timed event");
				return null;
			}
			if (num == 0L || num2 == 0L)
			{
				if (ExTraceGlobals.DeferredSendTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.DeferredSendTracer.TraceError(34120L, "Deferred send event does not include valid folder id or message id: " + timedEvent.ToString());
				}
				return null;
			}
			ExTraceGlobals.DeferredSendTracer.TraceDebug(50504L, "Leaving DeferredSendHandler.ExtractDeferredSendEventFromTimedEvent");
			return new DeferredSendEvent(timedEvent.EventTime, timedEvent.MailboxNumber.Value, num, num2);
		}

		private void GenerateMailSubmittedEvent(Context context, DeferredSendEvent deferredSendEvent)
		{
			ExTraceGlobals.DeferredSendTracer.TraceDebug(47432L, "Entering DeferredSendHandler.GenerateMailSubmittedEvent");
			context.InitializeMailboxExclusiveOperation(deferredSendEvent.MailboxNumber, ExecutionDiagnostics.OperationSource.MapiTimedEvent, LockManager.InfiniteTimeout);
			bool commit = false;
			try
			{
				ErrorCode first = context.StartMailboxOperation();
				if (first != ErrorCode.NoError)
				{
					ExTraceGlobals.DeferredSendTracer.TraceError(63816L, "No mailbox state found for mailbox number " + deferredSendEvent.MailboxNumber);
					return;
				}
				if (!context.LockedMailboxState.IsUserAccessible)
				{
					ExTraceGlobals.DeferredSendTracer.TraceError((long)((ulong)-1568264899), string.Concat(new object[]
					{
						"Mailbox with mailbox number ",
						deferredSendEvent.MailboxNumber,
						" is not accessible. State: ",
						context.LockedMailboxState.Status.ToString()
					}));
					return;
				}
				using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
				{
					if (mailbox == null)
					{
						ExTraceGlobals.DeferredSendTracer.TraceError(39240L, "Open mailbox failed for mailbox number " + deferredSendEvent.MailboxNumber);
						return;
					}
					ExchangeId folderId = ExchangeId.CreateFromInt64(context, mailbox.ReplidGuidMap, deferredSendEvent.FolderId);
					ExchangeId messageId = ExchangeId.CreateFromInt64(context, mailbox.ReplidGuidMap, deferredSendEvent.MessageId);
					using (TopMessage topMessage = TopMessage.OpenMessage(context, mailbox, folderId, messageId))
					{
						if (topMessage == null)
						{
							ExTraceGlobals.DeferredSendTracer.TraceError(55624L, "Could not open the message");
							return;
						}
						DateTime? dateTime = (DateTime?)topMessage.GetPropertyValue(context, PropTag.Message.DeferredSendTime);
						if (dateTime == null)
						{
							ExTraceGlobals.DeferredSendTracer.TraceError(43336L, "Could not obtain the deferred send time from the specified message");
							return;
						}
						if (DateTime.Compare(deferredSendEvent.EventTime, dateTime.Value) != 0)
						{
							ExTraceGlobals.DeferredSendTracer.TraceError(59720L, "Deferred send time has been changed on the origianl message, skip deferred send");
							return;
						}
						MailSubmittedNotificationEvent nev = NotificationEvents.CreateMailSubmittedEvent(context, topMessage);
						context.RiseNotificationEvent(nev);
						FaultInjection.InjectFault(DeferredSendHandler.deferredSendEventTestHook);
					}
				}
				commit = true;
			}
			finally
			{
				if (context.IsMailboxOperationStarted)
				{
					context.EndMailboxOperation(commit);
				}
			}
			ExTraceGlobals.DeferredSendTracer.TraceDebug(35144L, "Leaving DeferredSendHandler.GenerateMailSubmittedEvent");
		}

		private static Hookable<Action> deferredSendEventTestHook = Hookable<Action>.Create(true, null);
	}
}
