using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class EventController : Base, IDisposable
	{
		public EventController(DatabaseInfo databaseInfo, EventBasedAssistantCollection assistants, PoisonEventControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters, ThrottleGovernor serverGovernor, MapiEventTypeFlags moreEvents)
		{
			this.databaseInfo = databaseInfo;
			this.databaseCounters = databaseCounters;
			this.assistants = assistants;
			this.shutdownState = 0;
			this.poisonControl = poisonControl;
			MapiEventTypeFlags mapiEventTypeFlags = this.assistants.EventMask | moreEvents;
			this.filter = (((MapiEventTypeFlags)(-1) == mapiEventTypeFlags) ? null : Restriction.BitMaskNonZero(PropTag.EventMask, (int)mapiEventTypeFlags));
			this.governor = new DatabaseGovernor("event processing on '" + databaseInfo.DisplayName + "'", serverGovernor, new Throttle("EventDatabase", serverGovernor.Throttle.OpenThrottleValue, serverGovernor.Throttle));
			this.eventAccess = EventAccess.Create(this.DatabaseInfo, this.assistants);
		}

		public DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.databaseInfo;
			}
		}

		public EventBasedAssistantCollection Assistants
		{
			get
			{
				return this.assistants;
			}
		}

		public PoisonEventControl PoisonControl
		{
			get
			{
				return this.poisonControl;
			}
		}

		public PerformanceCountersPerDatabaseInstance DatabaseCounters
		{
			get
			{
				return this.databaseCounters;
			}
		}

		public bool Shutdown
		{
			get
			{
				return this.shutdownState != 0;
			}
		}

		public Throttle Throttle
		{
			get
			{
				return this.governor.Throttle;
			}
		}

		public ThrottleGovernor Governor
		{
			get
			{
				return this.governor;
			}
		}

		public Restriction Filter
		{
			get
			{
				return this.filter;
			}
		}

		public EventAccess EventAccess
		{
			get
			{
				return this.eventAccess;
			}
		}

		public bool RestartRequired
		{
			get
			{
				return this.eventAccess.RestartRequired;
			}
		}

		private protected Bookmark DatabaseBookmark { protected get; private set; }

		protected long HighestEventPolled
		{
			get
			{
				return this.highestEventPolled;
			}
			set
			{
				this.highestEventPolled = value;
				this.DatabaseCounters.HighestEventPolled.RawValue = this.highestEventPolled;
				long timestamp = Stopwatch.GetTimestamp();
				if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest<long>(2764451133U, ref timestamp);
				}
				this.DatabaseCounters.ElapsedTimeSinceLastEventPolled.RawValue = timestamp;
			}
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "EventController for database '" + this.databaseInfo.DisplayName + "'";
			}
			return this.toString;
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public void Start()
		{
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Starting", this);
			bool flag = false;
			try
			{
				AIBreadcrumbs.StartupTrail.Drop("Starting database: " + this.DatabaseInfo.Guid);
				this.DatabaseBookmark = this.eventAccess.GetDatabaseBookmark();
				Btree<Guid, Bookmark> btree = this.eventAccess.LoadAllMailboxBookmarks(this.DatabaseBookmark);
				bool flag2 = false;
				int num = 0;
				using (List<AssistantCollectionEntry>.Enumerator enumerator = this.assistants.ToList<AssistantCollectionEntry>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AssistantCollectionEntry assistant = enumerator.Current;
						do
						{
							try
							{
								base.CatchMeIfYouCan(delegate
								{
									assistant.Start(EventBasedStartInfo.NoInformation);
								}, assistant.Name);
							}
							catch (AIException ex)
							{
								if (num >= 1 || this.assistants.Count <= 1)
								{
									throw;
								}
								if (!flag2)
								{
									ExTraceGlobals.EventControllerTracer.TraceError<AssistantCollectionEntry, AIException>((long)this.GetHashCode(), "Event Based Assistant {0} cannot start due to Exception: {1}, Retrying now", assistant, ex);
									SingletonEventLogger.Logger.LogEvent(AssistantsEventLogConstants.Tuple_RetryAssistantFailedToStart, null, new object[]
									{
										assistant.Identity.ToString(),
										ex.ToString(),
										EventController.sleepStartingThread.TotalSeconds.ToString()
									});
									Thread.Sleep(EventController.sleepStartingThread);
									flag2 = true;
								}
								else
								{
									ExTraceGlobals.EventControllerTracer.TraceError<AssistantCollectionEntry, AIException>((long)this.GetHashCode(), "Event Based Assistant {0} cannot start after retry, due to Exception: {1}, will not start it anymore", assistant, ex);
									SingletonEventLogger.Logger.LogEvent(AssistantsEventLogConstants.Tuple_AssistantFailedToStart, null, new object[]
									{
										assistant.Identity.ToString(),
										ex.ToString()
									});
									flag2 = false;
									this.assistants.RemoveAssistant(assistant);
									num++;
								}
							}
						}
						while (flag2);
					}
				}
				this.InitializeEventDispatchers(btree);
				this.timeToSaveWatermarks = DateTime.UtcNow + Configuration.ActiveWatermarksSaveInterval;
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, DateTime>((long)this.GetHashCode(), "{0}: Next time to save watermarks: {1}", this, this.timeToSaveWatermarks);
				long num2 = long.MaxValue;
				foreach (Bookmark bookmark in btree)
				{
					num2 = Math.Min(num2, bookmark.GetLowestWatermark());
				}
				num2 = Math.Min(this.DatabaseBookmark.GetLowestWatermark(), num2);
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, long>((long)this.GetHashCode(), "{0}: Smallest watermark after initialization is: {1}", this, num2);
				this.HighestEventPolled = num2;
				this.timer = new Timer(new TimerCallback(this.TimerRoutine), null, TimeSpan.Zero, Configuration.EventPollingInterval);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					RpcHangDetector rpcHangDetector = RpcHangDetector.Create();
					rpcHangDetector.InvokeUnderHangDetection(delegate(HangDetector hangDetector)
					{
						AIBreadcrumbs.StatusTrail.Drop("Did not succeed to start event controller, stopping.");
						this.RequestStop(rpcHangDetector);
						this.WaitUntilAssistantsStopped();
						AIBreadcrumbs.StatusTrail.Drop("Exiting stop on fail to start event controller to start.");
					});
				}
				else
				{
					AIBreadcrumbs.StartupTrail.Drop("Finished starting " + this.DatabaseInfo.Guid);
				}
			}
			base.TracePfd("PFD AIS {0} {1}: Started successfully", new object[]
			{
				21335,
				this
			});
		}

		public void RequestStop(HangDetector hangDetector)
		{
			EventController.ShutdownState shutdownState = (EventController.ShutdownState)Interlocked.CompareExchange(ref this.shutdownState, 1, 0);
			AIBreadcrumbs.ShutdownTrail.Drop(string.Concat(new object[]
			{
				"Previous shutdown state: ",
				shutdownState,
				". Current: ",
				this.shutdownState.ToString()
			}));
			if (shutdownState == EventController.ShutdownState.NotRequested)
			{
				base.TracePfd("PFD AIS {0} {1}: phase1 shutdown", new object[]
				{
					27735,
					this
				});
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
				this.assistants.ShutdownAssistants(hangDetector);
			}
		}

		public void WaitUntilStopped()
		{
			this.WaitUntilAssistantsStopped();
			if (this.shutdownState == 1)
			{
				lock (this.watermarkUpdateLock)
				{
					if (this.shutdownState == 1)
					{
						ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Saving watermarks...", this);
						this.UpdateWatermarks();
						this.shutdownState = 2;
					}
				}
			}
			base.TracePfd("PFD AIS {0} {1}: Saved watermarks.", new object[]
			{
				17495,
				this
			});
		}

		public void Stop()
		{
			RpcHangDetector rpcHangDetector = RpcHangDetector.Create();
			rpcHangDetector.InvokeUnderHangDetection(delegate(HangDetector hangDetector)
			{
				AIBreadcrumbs.StatusTrail.Drop("Event controller stop called.");
				this.RequestStop(rpcHangDetector);
				this.WaitUntilStopped();
				AIBreadcrumbs.StatusTrail.Drop("Exiting event controller stop.");
			});
		}

		public virtual void IncrementEventQueueCount()
		{
			long num = Interlocked.Increment(ref this.numberEventsInQueueCurrent);
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<long>(3838192957U, ref num);
			}
			this.DatabaseCounters.EventsInQueueCurrent.RawValue = num;
			if (num == (long)Configuration.MaximumEventQueueSize)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Number of events queued is at maximum.", this);
			}
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, long>((long)this.GetHashCode(), "{0}: Incremented numberEventsInQueueCurrent to {1}", this, num);
		}

		public void DecrementEventQueueCount()
		{
			this.DecrementEventQueueCount(1L);
		}

		public virtual void DecrementEventQueueCount(long count)
		{
			long num = Interlocked.Add(ref this.numberEventsInQueueCurrent, -count);
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<long>(3838192957U, ref num);
			}
			this.DatabaseCounters.EventsInQueueCurrent.RawValue = num;
			if (num + count >= (long)Configuration.MaximumEventQueueSize && num < (long)Configuration.MaximumEventQueueSize)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Number of events queued is below maximum.", this);
			}
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, long, long>((long)this.GetHashCode(), "{0}: Decremented numberEventsInQueueCurrent from {1} to {2}", this, num + count, num);
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableEventController queryableEventController = queryableObject as QueryableEventController;
			if (queryableEventController != null)
			{
				queryableEventController.ShutdownState = ((EventController.ShutdownState)this.shutdownState).ToString();
				queryableEventController.TimeToSaveWatermarks = this.timeToSaveWatermarks;
				queryableEventController.HighestEventPolled = this.highestEventPolled;
				queryableEventController.NumberEventsInQueueCurrent = this.numberEventsInQueueCurrent;
				queryableEventController.RestartRequired = this.RestartRequired;
				QueryableThrottleGovernor queryableObject2 = new QueryableThrottleGovernor();
				this.governor.ExportToQueryableObject(queryableObject2);
				queryableEventController.Governor = queryableObject2;
				if (this.filter != null)
				{
					queryableEventController.EventFilter = this.filter.ToString();
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.assistants != null)
				{
					this.assistants.Dispose();
				}
				this.eventAccess.Dispose();
				if (this.timer != null)
				{
					this.timer.Dispose();
				}
				this.governor.Dispose();
			}
		}

		protected virtual void InitializeEventDispatchers(Btree<Guid, Bookmark> allBookmarks)
		{
		}

		protected virtual void WaitUntilStoppedInternal()
		{
		}

		protected virtual void PeriodicMaintenance()
		{
		}

		protected virtual void DisposeOfIdleDispatchers()
		{
		}

		protected abstract void ProcessPolledEvent(MapiEvent mapiEvent);

		protected abstract void UpdateWatermarksForAssistant(Guid assistantId);

		private void WaitUntilAssistantsStopped()
		{
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Waiting for timer routine to exit...", this);
			lock (this.timerLock)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Timer routine is clear", this);
			}
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Waiting for poller to stop...", this);
			base.TracePfd("PFD AIS {0} {1}: Poller has stopped.", new object[]
			{
				23639,
				this
			});
			this.WaitUntilStoppedInternal();
		}

		private void TimerRoutine(object stateNotUsed)
		{
			using (ExPerfTrace.RelatedActivity(this.pollingActivityId))
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: TimerRoutine", this);
				if (!Monitor.TryEnter(this.timerLock))
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: timer already busy", this);
				}
				else
				{
					try
					{
						long timestamp = Stopwatch.GetTimestamp();
						if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
						{
							ExTraceGlobals.FaultInjectionTracer.TraceTest<long>(3443928381U, ref timestamp);
							long rawValue = 0L;
							long rawValue2 = 0L;
							ExTraceGlobals.FaultInjectionTracer.TraceTest<long>(2764451133U, ref rawValue);
							ExTraceGlobals.FaultInjectionTracer.TraceTest<long>(3838192957U, ref rawValue2);
							this.DatabaseCounters.EventsInQueueCurrent.RawValue = rawValue2;
							this.DatabaseCounters.ElapsedTimeSinceLastEventPolled.RawValue = rawValue;
							ExTraceGlobals.FaultInjectionTracer.TraceTest(3703975229U);
						}
						this.DatabaseCounters.ElapsedTimeSinceLastEventPollingAttempt.RawValue = timestamp;
						bool noMoreEvents = false;
						while (this.ReadyToPoll() && !noMoreEvents)
						{
							try
							{
								base.CatchMeIfYouCan(delegate
								{
									noMoreEvents = this.PollAndQueueEvents();
								}, "EventController");
							}
							catch (AIException ex)
							{
								ExTraceGlobals.EventControllerTracer.TraceError<EventController, AIException>((long)this.GetHashCode(), "{0}: Exception while polling: {1}", this, ex);
								this.governor.ReportResult(ex);
							}
							this.PeriodicMaintenance();
						}
						ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Out of polling loop", this);
						if (!this.Shutdown && this.RestartRequired)
						{
							ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Restart required; stopping...", this);
							RpcHangDetector rpcHangDetector = RpcHangDetector.Create();
							rpcHangDetector.InvokeUnderHangDetection(delegate(HangDetector hangDetector)
							{
								AIBreadcrumbs.StatusTrail.Drop("Restart required, stopping.");
								this.RequestStop(rpcHangDetector);
								AIBreadcrumbs.StatusTrail.Drop("Exiting stop due to restart.");
							});
						}
						if (!this.Shutdown && this.governor.Status != GovernorStatus.Failure && this.timeToSaveWatermarks < DateTime.UtcNow)
						{
							ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Time to update watermarks...", this);
							AIBreadcrumbs.StatusTrail.Drop("Begin Update Watermarks");
							this.UpdateWatermarks();
							AIBreadcrumbs.StatusTrail.Drop("End Update Watermarks");
							ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Updated watermarks.", this);
							this.timeToSaveWatermarks = DateTime.UtcNow + Configuration.ActiveWatermarksSaveInterval;
							ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, DateTime>((long)this.GetHashCode(), "{0}: Next watermark update: {1}", this, this.timeToSaveWatermarks);
						}
					}
					finally
					{
						Monitor.Exit(this.timerLock);
					}
				}
			}
		}

		private bool ReadyToPoll()
		{
			if (this.Shutdown || this.governor.Status == GovernorStatus.Failure || this.RestartRequired)
			{
				return false;
			}
			long num = Interlocked.Read(ref this.numberEventsInQueueCurrent);
			return num < (long)Configuration.MaximumEventQueueSize;
		}

		private bool PollAndQueueEvents()
		{
			long num = this.highestEventPolled + 1L;
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, long>((long)this.GetHashCode(), "{0}: ReadEvents({1},...)", this, num);
			long num2;
			MapiEvent[] array;
			try
			{
				array = this.eventAccess.ReadEvents(num, 100, 10000, this.filter, out num2);
			}
			catch (MapiExceptionNotFound innerException)
			{
				throw new DatabaseIneptException(innerException);
			}
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, int, long>((long)this.GetHashCode(), "{0}: Processing {1} polled events.  Endcounter: {2}", this, array.Length, num2);
			this.DatabaseCounters.EventsPolled.IncrementBy((long)array.Length);
			TimeSpan timeSpan = (array.Length <= 0) ? TimeSpan.Zero : (DateTime.UtcNow - array[array.Length - 1].CreateTime);
			this.DatabaseCounters.PollingDelay.RawValue = (long)timeSpan.TotalSeconds;
			for (int i = 0; i < array.Length; i++)
			{
				if (this.IsSearchFolderEvent(array[i]))
				{
					ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, MapiEvent>((long)this.GetHashCode(), "{0}: Ignoring search folder event: {1}", this, array[i]);
				}
				else
				{
					this.ProcessPolledEvent(array[i]);
				}
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, long, long>((long)this.GetHashCode(), "{0}: Updating highest event polled from {1} to {2}", this, this.highestEventPolled, array[i].EventCounter);
				this.HighestEventPolled = array[i].EventCounter;
			}
			if (array.Length > 0 || num2 > num)
			{
				long arg = Math.Max(this.highestEventPolled, num2);
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController, long, long>((long)this.GetHashCode(), "{0}: Endcounter updating highest event polled from {1} to {2}", this, this.highestEventPolled, arg);
				this.HighestEventPolled = arg;
			}
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Processed events.", this);
			return array.Length == 0 && num2 <= num;
		}

		private void UpdateWatermarks()
		{
			if (this.RestartRequired)
			{
				ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Not updating watermarks because restart is required", this);
				return;
			}
			try
			{
				base.CatchMeIfYouCan(delegate
				{
					foreach (AssistantCollectionEntry assistantCollectionEntry in this.Assistants)
					{
						this.UpdateWatermarksForAssistant(assistantCollectionEntry.Identity);
					}
					if (!this.Shutdown)
					{
						this.DisposeOfIdleDispatchers();
					}
					if (Test.NotifyAllWatermarksCommitted != null)
					{
						Test.NotifyAllWatermarksCommitted();
					}
				}, "EventController");
			}
			catch (AIException arg)
			{
				ExTraceGlobals.EventControllerTracer.TraceError<EventController, AIException>((long)this.GetHashCode(), "{0}: failed to save high watermark due to exception: {1}", this, arg);
				return;
			}
			ExTraceGlobals.EventControllerTracer.TraceDebug<EventController>((long)this.GetHashCode(), "{0}: Updated watermarks successfully", this);
		}

		private bool IsSearchFolderEvent(MapiEvent notification)
		{
			return (notification.EventFlags & MapiEventFlags.SearchFolder) != MapiEventFlags.None;
		}

		private const string EventControlerName = "EventController";

		private const int MaximumEventsToCheckPerPoll = 10000;

		private const int NumberOfEventsPerPoll = 100;

		private const int MaxAssistantsToRemove = 1;

		private static readonly TimeSpan sleepStartingThread = TimeSpan.FromSeconds(10.0);

		private readonly Guid pollingActivityId = Guid.NewGuid();

		private PerformanceCountersPerDatabaseInstance databaseCounters;

		private EventBasedAssistantCollection assistants;

		private DateTime timeToSaveWatermarks = DateTime.MinValue;

		private long numberEventsInQueueCurrent;

		private PoisonEventControl poisonControl;

		private string toString;

		private DatabaseInfo databaseInfo;

		private ThrottleGovernor governor;

		private Restriction filter;

		private EventAccess eventAccess;

		private Timer timer;

		private object timerLock = new object();

		private object watermarkUpdateLock = new object();

		private long highestEventPolled;

		private int shutdownState;

		private enum ShutdownState
		{
			NotRequested,
			InProgress,
			Completed
		}
	}
}
