using System;
using System.Diagnostics;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class EventDispatcher : QueueProcessor<InterestingEvent>
	{
		protected EventDispatcher(AssistantCollectionEntry assistant, ThrottleGovernor governor, EventController controller) : base(governor)
		{
			this.Assistant = assistant;
			this.controller = controller;
		}

		public abstract string MailboxDisplayName { get; }

		public EventBasedAssistantCollection Assistants
		{
			get
			{
				return this.controller.Assistants;
			}
		}

		public PoisonEventControl PoisonControl
		{
			get
			{
				return this.controller.PoisonControl;
			}
		}

		public DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.controller.DatabaseInfo;
			}
		}

		public EventController Controller
		{
			get
			{
				return this.controller;
			}
		}

		private protected AssistantCollectionEntry Assistant { protected get; private set; }

		protected virtual bool CountEvents
		{
			get
			{
				return true;
			}
		}

		protected override bool Shutdown
		{
			get
			{
				return this.controller.Shutdown;
			}
		}

		protected override PerformanceCountersPerDatabaseInstance PerformanceCounters
		{
			get
			{
				return this.controller.DatabaseCounters;
			}
		}

		protected virtual void EnqueueEvent(InterestingEvent interestingEvent)
		{
			base.EnqueueItem(interestingEvent);
		}

		protected override void OnEnqueueItem(InterestingEvent interestingEvent)
		{
			this.Assistant.PerformanceCounters.EventsInQueueCurrent.Increment();
			this.Assistant.PerformanceCounters.ElapsedTimeSinceLastEventQueued.RawValue = Stopwatch.GetTimestamp();
			if (this.CountEvents)
			{
				this.controller.IncrementEventQueueCount();
			}
		}

		protected override void OnCompletedItem(InterestingEvent interestingEvent, AIException exception)
		{
			this.Assistant.PerformanceCounters.EventsProcessed.Increment();
			this.Assistant.PerformanceCounters.EventsInQueueCurrent.Decrement();
			if (this.CountEvents)
			{
				this.controller.DecrementEventQueueCount();
			}
			if (exception != null)
			{
				this.NotifySkippedEvent(interestingEvent.MapiEvent, exception);
			}
		}

		protected override AIException ProcessItem(InterestingEvent interestingEvent)
		{
			EventDispatcher.<>c__DisplayClass4 CS$<>8__locals1 = new EventDispatcher.<>c__DisplayClass4();
			CS$<>8__locals1.interestingEvent = interestingEvent;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.exception = null;
			CS$<>8__locals1.kit = new EmergencyKit(CS$<>8__locals1.interestingEvent.MapiEvent);
			this.PoisonControl.PoisonCall(CS$<>8__locals1.kit, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ProcessItem>b__3)));
			return CS$<>8__locals1.exception;
		}

		protected abstract AIException DangerousProcessItem(EmergencyKit kit, InterestingEvent interestingEvent);

		protected bool EnqueueIfInteresting(EmergencyKit emergencyKit)
		{
			bool flag = this.IsAssistantInterested(emergencyKit);
			if (flag)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcher, long>((long)this.GetHashCode(), "{0}: Enqueuing event {1}...", this, emergencyKit.MapiEvent.EventCounter);
				this.EnqueueEvent(InterestingEvent.Create(emergencyKit.MapiEvent));
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcher, long>((long)this.GetHashCode(), "{0}: Event {1} enqueued", this, emergencyKit.MapiEvent.EventCounter);
			}
			else
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcher, MapiEvent>((long)this.GetHashCode(), "{0}: Polled event not interesting: {1}", this, emergencyKit.MapiEvent);
			}
			return flag;
		}

		protected void HandleEvent(EmergencyKit kit, InterestingEvent interestingEvent, MailboxSession mailboxSession, Item item)
		{
			if (!interestingEvent.WasProcessed)
			{
				if (!this.IsAssistantInterested(kit))
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcher, MapiEvent>((long)this.GetHashCode(), "{0}: Discarding event as no assistants are interested: {1}", this, kit.MapiEvent);
					return;
				}
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcher, MapiEvent>((long)this.GetHashCode(), "{0}: Assistants still interested in event: {1}", this, kit.MapiEvent);
			}
			this.Assistant.PerformanceCounters.AverageEventQueueTime.IncrementBy(interestingEvent.EnqueuedStopwatch.ElapsedTicks);
			this.Assistant.PerformanceCounters.AverageEventQueueTimeBase.Increment();
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcher, long>((long)this.GetHashCode(), "{0}: Calling HandleEvent for {1}", this, kit.MapiEvent.EventCounter);
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.HandleEventHelper(kit, mailboxSession, item);
			stopwatch.Stop();
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcher, long>((long)this.GetHashCode(), "{0}: HandleEvent for {1} has returned.", this, kit.MapiEvent.EventCounter);
			this.Assistant.PerformanceCounters.AverageEventProcessingTime.IncrementBy(stopwatch.ElapsedTicks);
			this.Assistant.PerformanceCounters.AverageEventProcessingTimeBase.Increment();
			this.Controller.DatabaseCounters.AverageEventProcessingTime.IncrementBy(stopwatch.ElapsedTicks);
			this.Controller.DatabaseCounters.AverageEventProcessingTimeBase.Increment();
		}

		private void HandleEventHelper(EmergencyKit kit, MailboxSession itemStore, StoreObject item)
		{
			kit.SetContext(this.Assistant.Instance, this);
			try
			{
				base.CatchMeIfYouCan(delegate
				{
					this.Assistant.Instance.HandleEvent(kit.MapiEvent, itemStore, item);
				}, (this.Assistant != null) ? this.Assistant.Name : "<null>");
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<EventDispatcher, MapiEvent>((long)this.GetHashCode(), "{0}: Event handled successfully: {1}", this, kit.MapiEvent);
			}
			catch (SkipException ex)
			{
				this.Assistant.PerformanceCounters.HandledExceptions.Increment();
				ExTraceGlobals.EventDispatcherTracer.TraceError<EventDispatcher, MapiEvent, SkipException>((long)this.GetHashCode(), "{0}: Assistant has instructed us to skip event {1} with exception {2}", this, kit.MapiEvent, ex);
				base.LogEvent(AssistantsEventLogConstants.Tuple_AssistantSkippingEvent, null, new object[]
				{
					this.Assistant.Name,
					kit.MapiEvent,
					this.MailboxDisplayName,
					ex
				});
				IEventSkipNotification eventSkipNotification = this.Assistant.Instance as IEventSkipNotification;
				if (eventSkipNotification != null)
				{
					eventSkipNotification.OnSkipEvent(kit.MapiEvent, ex);
				}
			}
			catch (AIException ex2)
			{
				this.Assistant.PerformanceCounters.HandledExceptions.Increment();
				ExTraceGlobals.EventDispatcherTracer.TraceError<EventDispatcher, MapiEvent, AIException>((long)this.GetHashCode(), "{0}: Failed to handle event {1} with exception {2}", this, kit.MapiEvent, ex2);
				string periodicKey = string.Format("{0}: Assistant {1} failed to handle event {2} with exception {3} on mailbox {4}", new object[]
				{
					this,
					this.Assistant.Name,
					kit.MapiEvent,
					ex2,
					this.MailboxDisplayName
				});
				base.LogEvent(AssistantsEventLogConstants.Tuple_AssistantFailedToProcessEvent, periodicKey, new object[]
				{
					this.Assistant.Name,
					kit.MapiEvent,
					this.MailboxDisplayName,
					ex2
				});
				kit.UnsetContext();
				throw;
			}
			kit.UnsetContext();
		}

		private bool IsAssistantInterested(EmergencyKit kit)
		{
			EventDispatcher.<>c__DisplayClassb CS$<>8__locals1 = new EventDispatcher.<>c__DisplayClassb();
			CS$<>8__locals1.kit = kit;
			CS$<>8__locals1.<>4__this = this;
			if (this.PoisonControl.IsPoisonEvent(CS$<>8__locals1.kit.MapiEvent))
			{
				base.TracePfd("PFD AIS {0} {1}: Poison event detected and skipped. EventCounter: {2}, crashCount: {3}", new object[]
				{
					26711,
					this,
					CS$<>8__locals1.kit.MapiEvent.EventCounter,
					this.PoisonControl.GetCrashCount(CS$<>8__locals1.kit.MapiEvent)
				});
				this.NotifySkippedEvent(CS$<>8__locals1.kit, null);
				if (Test.NotifyPoisonEventSkipped != null)
				{
					Test.NotifyPoisonEventSkipped(this.DatabaseInfo, CS$<>8__locals1.kit.MapiEvent);
				}
				return false;
			}
			CS$<>8__locals1.assistantIsInterested = false;
			this.PoisonControl.PoisonCall(CS$<>8__locals1.kit, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<IsAssistantInterested>b__a)));
			if (CS$<>8__locals1.assistantIsInterested)
			{
				this.Assistant.PerformanceCounters.InterestingEvents.Increment();
			}
			this.Assistant.PerformanceCounters.InterestingEventsBase.Increment();
			return CS$<>8__locals1.assistantIsInterested;
		}

		private void NotifySkippedEvent(MapiEvent mapiEvent, Exception e)
		{
			this.NotifySkippedEvent(new EmergencyKit(mapiEvent), e);
		}

		private void NotifySkippedEvent(EmergencyKit kit, Exception e)
		{
			EventDispatcher.<>c__DisplayClassf CS$<>8__locals1 = new EventDispatcher.<>c__DisplayClassf();
			CS$<>8__locals1.kit = kit;
			CS$<>8__locals1.e = e;
			CS$<>8__locals1.<>4__this = this;
			if (this.PoisonControl.IsToxicEvent(CS$<>8__locals1.kit.MapiEvent))
			{
				ExTraceGlobals.EventDispatcherTracer.TraceError<MapiEvent, Exception>((long)this.GetHashCode(), "Toxic event: {0} : {1}", CS$<>8__locals1.kit.MapiEvent, CS$<>8__locals1.e);
				return;
			}
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<MapiEvent, Exception>((long)this.GetHashCode(), "Notifing assistants of toxic event {0}, exception {1}", CS$<>8__locals1.kit.MapiEvent, CS$<>8__locals1.e);
			this.PoisonControl.PoisonCall(CS$<>8__locals1.kit, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<NotifySkippedEvent>b__d)));
		}

		private EventController controller;
	}
}
