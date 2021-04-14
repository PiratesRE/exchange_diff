using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal sealed class TimerEventHandler : ITimedEventHandler
	{
		internal static Guid EventSource
		{
			get
			{
				return TimerEventHandler.eventSource;
			}
		}

		internal static void CheckAndRaiseDeferredEventTimer(Context context, TopMessage message)
		{
			if (message.IsNew || 0UL != (message.ChangedGroups & 9007199254740992UL))
			{
				bool flag = message.IsNew && !message.OriginalMessageID.IsNull;
				foreach (StorePropInfo storePropInfo in TimerEventHandler.timerPropList)
				{
					StorePropTag namedPropStorePropTag;
					if (storePropInfo.IsNamedProperty)
					{
						namedPropStorePropTag = message.Mailbox.GetNamedPropStorePropTag(context, storePropInfo.PropName, storePropInfo.PropType, ObjectType.Message);
					}
					else
					{
						namedPropStorePropTag = new StorePropTag(storePropInfo.PropId, storePropInfo.PropType, storePropInfo, ObjectType.Message);
					}
					if (message.IsPropertyChanged(context, namedPropStorePropTag))
					{
						DateTime? dateTime = (DateTime?)message.GetPropertyValue(context, namedPropStorePropTag);
						if (TimerEvent.IsValidTimerDateTime(dateTime))
						{
							DateTime t = DateTime.UtcNow - TimerEventHandler.PastTimerAllowance;
							if (t <= dateTime.Value)
							{
								byte[] eventData = TimerEvent.SerializeExtraData(message.GetDocumentId(context), new Property(namedPropStorePropTag, dateTime));
								TimedEventEntry timedEvent = new TimedEventEntry(dateTime.Value, new int?(message.Mailbox.MailboxNumber), TimerEventHandler.EventSource, 1, eventData);
								context.RaiseTimedEvent(timedEvent);
							}
							else
							{
								ExTraceGlobals.EventsTracer.TraceError(64700L, "Event time in the past: " + dateTime.Value.ToString() + " vs allowed " + t.ToString());
							}
						}
					}
					else if (flag)
					{
						message.SetProperty(context, namedPropStorePropTag, null);
					}
				}
			}
		}

		public void Invoke(Context context, TimedEventEntry entry)
		{
			ExTraceGlobals.EventsTracer.TraceDebug(64540L, "Entering TimerEventHandler.Invoke");
			if (ExTraceGlobals.EventsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.EventsTracer.TraceDebug(48156L, "Raw timed event=" + entry.ToString());
			}
			TimerEvent timerEvent = this.ExtractTimerEvent(entry);
			if (timerEvent == null)
			{
				ExTraceGlobals.EventsTracer.TraceError(39964L, "Unable to extract the event data from the raw timed event entry");
				return;
			}
			this.TriggerTimer(context, timerEvent);
			ExTraceGlobals.EventsTracer.TraceDebug(50108L, "Leaving TimerEventHandler.Invoke");
		}

		private TimerEvent ExtractTimerEvent(TimedEventEntry entry)
		{
			ExTraceGlobals.EventsTracer.TraceDebug(56348L, "Entering TimerEventHandler.ExtractTimerEvent");
			if (entry.EventType != 1)
			{
				if (ExTraceGlobals.EventsTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.EventsTracer.TraceError(44060L, "Event does not include a valid event type: " + entry.ToString());
				}
				return null;
			}
			if (entry.MailboxNumber == null || entry.MailboxNumber.Value == -1)
			{
				if (ExTraceGlobals.EventsTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.EventsTracer.TraceError(60444L, "Event does not include a valid mailboxNumber: " + entry.ToString());
				}
				return null;
			}
			if (entry.EventData == null)
			{
				if (ExTraceGlobals.EventsTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.EventsTracer.TraceError(35868L, "Event does not include event data part: " + entry.ToString());
				}
				return null;
			}
			int num;
			Property prop;
			if (!TimerEvent.DeserializeExtraData(entry.EventData, out num, out prop))
			{
				ExTraceGlobals.EventsTracer.TraceError(52252L, "Invalid extra data in the timed event");
				return null;
			}
			if (num == 0 || !TimerEvent.IsValidTimerDateTime((DateTime?)prop.Value))
			{
				if (ExTraceGlobals.EventsTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.EventsTracer.TraceError(35772L, "Timer event does not include valid data: " + entry.ToString());
				}
				return null;
			}
			ExTraceGlobals.EventsTracer.TraceDebug(52156L, "Leaving TimerEventHandler.ExtractTimerEvent");
			return new TimerEvent(entry.EventTime, entry.MailboxNumber.Value, num, prop);
		}

		private void TriggerTimer(Context context, TimerEvent timerEvent)
		{
			ExTraceGlobals.EventsTracer.TraceDebug(46012L, "Entering TimerEventHandler.ResetTimer");
			context.InitializeMailboxExclusiveOperation(timerEvent.MailboxNumber, ExecutionDiagnostics.OperationSource.MailboxTask, LockManager.InfiniteTimeout);
			bool commit = false;
			try
			{
				ErrorCode first = context.StartMailboxOperation();
				if (first != ErrorCode.NoError)
				{
					ExTraceGlobals.EventsTracer.TraceError(62396L, "No mailbox state found for mailbox number " + timerEvent.MailboxNumber);
					return;
				}
				if (!context.LockedMailboxState.IsUserAccessible)
				{
					ExTraceGlobals.EventsTracer.TraceError(37820L, string.Concat(new object[]
					{
						"Mailbox with mailbox number ",
						timerEvent.MailboxNumber,
						" is not accessible. State: ",
						context.LockedMailboxState.Status.ToString()
					}));
					return;
				}
				using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
				{
					if (mailbox == null)
					{
						ExTraceGlobals.EventsTracer.TraceError(54204L, "Open mailbox failed for mailbox number " + timerEvent.MailboxNumber);
						return;
					}
					using (TopMessage topMessage = TopMessage.OpenMessage(context, mailbox, timerEvent.DocumentId))
					{
						if (topMessage == null)
						{
							ExTraceGlobals.EventsTracer.TraceError(41916L, "Could not open the message");
							return;
						}
						DateTime? dateTime = (DateTime?)topMessage.GetPropertyValue(context, timerEvent.Prop.Tag);
						if (dateTime == null || DateTime.Compare(dateTime.Value, (DateTime)timerEvent.Prop.Value) != 0)
						{
							ExTraceGlobals.EventsTracer.TraceError(58300L, "Timer has been changed on the original message, skip event");
							return;
						}
						topMessage.SetProperty(context, timerEvent.Prop.Tag, TimerEventHandler.DateTimeMax);
						topMessage.SaveChanges(context, SaveMessageChangesFlags.SkipMailboxQuotaCheck | SaveMessageChangesFlags.SkipFolderQuotaCheck | SaveMessageChangesFlags.TimerEventFired);
						mailbox.Save(context);
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
			ExTraceGlobals.EventsTracer.TraceDebug(33724L, "Leaving TimerEventHandler.ResetTimer");
		}

		private static readonly Guid eventSource = new Guid("80644F47-EAAC-4530-BC5E-E54BA9407A32");

		private static readonly List<StorePropInfo> timerPropList = new List<StorePropInfo>
		{
			PropTag.Message.EventEmailReminderTimer.PropInfo
		};

		internal static DateTime DateTimeMax = ExDateTime.OutlookDateTimeMax;

		internal static readonly TimeSpan PastTimerAllowance = TimeSpan.FromMinutes(60.0);
	}
}
