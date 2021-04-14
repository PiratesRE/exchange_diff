using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EventPump : IDisposeTrackable, IDisposable
	{
		internal EventPump(EventPumpManager eventPumpManager, string server, Guid mdbGuid)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.eventPumpManager = eventPumpManager;
			this.server = server;
			this.mdbGuid = mdbGuid;
			this.threadLimiter = new EventPumpThreadLimiter(this);
			StoreSession storeSession = null;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.exRpcAdmin = ExRpcAdmin.Create("Client=EventPump", server, null, null, null);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExFailedToCreateEventManager, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("EventPump::Constructor. Failed to create EventPump.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExFailedToCreateEventManager, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("EventPump::Constructor. Failed to create EventPump.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			StoreSession storeSession2 = null;
			bool flag2 = false;
			try
			{
				if (storeSession2 != null)
				{
					storeSession2.BeginMapiCall();
					storeSession2.BeginServerHealthCall();
					flag2 = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.mapiEventManager = MapiEventManager.Create(this.exRpcAdmin, Guid.Empty, this.mdbGuid);
			}
			catch (MapiPermanentException ex3)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExFailedToCreateEventManager, ex3, storeSession2, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("EventPump::Constructor. Failed to create EventPump.", new object[0]),
					ex3
				});
			}
			catch (MapiRetryableException ex4)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExFailedToCreateEventManager, ex4, storeSession2, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("EventPump::Constructor. Failed to create EventPump.", new object[0]),
					ex4
				});
			}
			finally
			{
				try
				{
					if (storeSession2 != null)
					{
						storeSession2.EndMapiCall();
						if (flag2)
						{
							storeSession2.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			this.lastEventCounter = this.ReadLastEventWatermark();
			this.RegisterMainPump(EventPump.PollingTimeSpan);
			ExTraceGlobals.EventTracer.TraceDebug<EventPump>((long)this.GetHashCode(), "EventPump::Constructor. {0}", this);
		}

		public static TimeSpan PollingTimeSpan
		{
			get
			{
				return EventPump.pollingTimeSpan;
			}
			set
			{
				EventPump.pollingTimeSpan = value;
			}
		}

		internal Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		internal int ReferenceCount
		{
			get
			{
				return this.referenceCount;
			}
		}

		protected bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EventPump>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override string ToString()
		{
			return string.Format("Server = {0}. MdbGuid = {1}.", this.server, this.mdbGuid);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void AddEventSink(EventSink eventSink)
		{
			bool flag = false;
			try
			{
				if (eventSink.FirstMissedEventWaterMark != null)
				{
					long firstEventToConsumeOnSink = eventSink.FirstMissedEventWaterMark.WasEventProcessed ? (eventSink.FirstMissedEventWaterMark.MapiWatermark + 1L) : eventSink.FirstMissedEventWaterMark.MapiWatermark;
					eventSink.SetFirstEventToConsumeOnSink(firstEventToConsumeOnSink);
				}
				else
				{
					eventSink.SetFirstEventToConsumeOnSink(this.ReadLastEventWatermark() + 1L);
				}
				eventSink.SetLastKnownWatermark(this.lastEventCounter, false);
				this.ModifyEventSinkList(EventPump.ModifyEventSinkListType.AddEventSink, eventSink);
				eventSink.SetEventPump(this);
				flag = true;
			}
			finally
			{
				if (!flag && eventSink != null)
				{
					eventSink.Dispose();
					eventSink = null;
				}
			}
		}

		internal void RemoveEventSink(EventSink eventSink)
		{
			this.ModifyEventSinkList(EventPump.ModifyEventSinkListType.RemoveEventSink, eventSink);
			this.Release();
		}

		internal void RequestRecovery(EventSink eventSink)
		{
			this.threadLimiter.RequestRecovery(eventSink);
		}

		internal void ExecuteRecovery(EventSink eventSink)
		{
			ExTraceGlobals.EventTracer.TraceDebug<EventPump, EventSink>((long)this.GetHashCode(), "EventPump::ExecuteRecovery. {0}. Starting Recovery. EventSink = {1}.", this, eventSink);
			try
			{
				IRecoveryEventSink recoveryEventSink = null;
				EventWatermark eventWatermark = null;
				long num = 0L;
				Guid mailboxGuid;
				try
				{
					mailboxGuid = eventSink.MailboxGuid;
					recoveryEventSink = eventSink.StartRecovery();
					eventWatermark = recoveryEventSink.FirstMissedEventWatermark;
					num = recoveryEventSink.LastMissedEventWatermark;
				}
				catch (ObjectDisposedException)
				{
					ExTraceGlobals.EventTracer.TraceDebug<EventPump, EventSink>((long)this.GetHashCode(), "EventPump::ExecuteRecovery. {0}. Executing recovery in a disposed sink. Aborting recovery. EventSink = {1}.", this, eventSink);
					return;
				}
				Restriction restriction = null;
				if (!eventSink.IsPublicFolderDatabase)
				{
					restriction = Restriction.EQ(PropTag.EventMailboxGuid, mailboxGuid.ToByteArray());
				}
				long num2 = eventWatermark.WasEventProcessed ? (eventWatermark.MapiWatermark + 1L) : eventWatermark.MapiWatermark;
				long num3 = num2;
				bool flag = true;
				while (flag)
				{
					int eventCountToCheck = EventPump.GetEventCountToCheck(num3, eventWatermark, num2, num);
					if (eventCountToCheck > 0)
					{
						MapiEvent[] array = null;
						long num4 = 0L;
						try
						{
							this.disposeLock.EnterReadLock();
							if (this.IsDisposed)
							{
								return;
							}
							array = EventPump.ReadEvents(this.mapiEventManager, this.mapiEventManagerLock, num3, eventCountToCheck, eventCountToCheck, restriction, out num4);
						}
						finally
						{
							try
							{
								this.disposeLock.ExitReadLock();
							}
							catch (SynchronizationLockException)
							{
							}
						}
						int num5 = 0;
						while (num5 < array.Length && flag)
						{
							MapiEvent mapiEvent = array[num5];
							if (EventPump.IsEventBetweenCounters(num3, num, mapiEvent.Watermark.EventCounter))
							{
								try
								{
									flag = recoveryEventSink.RecoveryConsume(mapiEvent);
									goto IL_179;
								}
								catch (ObjectDisposedException)
								{
									ExTraceGlobals.EventTracer.TraceDebug<EventPump, EventSink>((long)this.GetHashCode(), "EventPump::ExecuteRecovery. {0}. Executing recovery in a disposed sink. Aborting recovery. EventSink = {1}.", this, eventSink);
									return;
								}
								goto Block_9;
							}
							goto IL_150;
							IL_179:
							num5++;
							continue;
							Block_9:
							try
							{
								IL_150:
								recoveryEventSink.EndRecovery();
							}
							catch (ObjectDisposedException)
							{
								ExTraceGlobals.EventTracer.TraceDebug<EventPump, EventSink>((long)this.GetHashCode(), "EventPump::ExecuteRecovery. {0}. Executing recovery in a disposed sink. Aborting recovery. EventSink = {1}.", this, eventSink);
								return;
							}
							flag = false;
							goto IL_179;
						}
						num3 = num4 + 1L;
					}
					else
					{
						try
						{
							recoveryEventSink.EndRecovery();
						}
						catch (ObjectDisposedException)
						{
							ExTraceGlobals.EventTracer.TraceDebug<EventPump, EventSink>((long)this.GetHashCode(), "EventPump::ExecuteRecovery. {0}. Executing recovery in a disposed sink. Aborting recovery. EventSink = {1}.", this, eventSink);
							return;
						}
						flag = false;
					}
				}
			}
			catch (StoragePermanentException ex)
			{
				eventSink.HandleException(ex);
			}
			catch (StorageTransientException ex2)
			{
				eventSink.HandleException(ex2);
			}
			ExTraceGlobals.EventTracer.TraceDebug<EventPump, EventSink>((long)this.GetHashCode(), "EventPump::ExecuteRecovery. {0}. Exiting Recovery. EventSink = {1}.", this, eventSink);
		}

		internal void VerifyWatermarkIsInEventTable(EventWatermark watermark)
		{
			if (watermark.MdbGuid != this.MdbGuid)
			{
				throw new InvalidEventWatermarkException(ServerStrings.ExInvalidEventWatermarkBadOrigin(watermark.MdbGuid, this.MdbGuid));
			}
			long num = 0L;
			EventPump.ReadEvents(this.mapiEventManager, this.mapiEventManagerLock, watermark.MapiWatermark, 1, 1, null, out num);
		}

		internal void AddRef()
		{
			Interlocked.Increment(ref this.referenceCount);
		}

		internal void Release()
		{
			if (Interlocked.Decrement(ref this.referenceCount) == 0)
			{
				this.eventPumpManager.RemoveEventPump(this);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.InternalStopPumpThread();
				this.exRpcAdmin.Dispose();
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private static MapiEvent[] ReadEvents(MapiEventManager mapiEventManager, object mapiEventManagerLock, long startCounter, int eventCountWanted, int eventCountToCheck, Restriction restriction, out long endCounter)
		{
			long num = 0L;
			MapiEvent[] result = null;
			StoreSession storeSession = null;
			object thisObject = null;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				lock (mapiEventManagerLock)
				{
					result = mapiEventManager.ReadEvents(startCounter, eventCountWanted, eventCountToCheck, restriction, ReadEventsFlags.FailIfEventsDeleted, false, out num);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCaughtMapiExceptionWhileReadingEvents, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("EventPump::ReadEvents. Caught MapiException while reading events.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCaughtMapiExceptionWhileReadingEvents, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("EventPump::ReadEvents. Caught MapiException while reading events.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			endCounter = num;
			return result;
		}

		private static void ReadAndDistributeEvents(object state, bool timedOut)
		{
			WeakReference weakReference = (WeakReference)state;
			EventPump eventPump = (EventPump)weakReference.Target;
			if (weakReference.IsAlive && timedOut)
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				Dictionary<Guid, List<EventSink>> eventSinkDictionary = eventPump.eventSinks;
				bool flag = eventPump.ReadAndDistributeEvents(eventSinkDictionary);
				if (flag)
				{
					TimeSpan timeSpan = ExDateTime.UtcNow - utcNow;
					TimeSpan timeSpan2 = TimeSpan.Zero;
					if (timeSpan < EventPump.PollingTimeSpan)
					{
						timeSpan2 = EventPump.PollingTimeSpan - timeSpan;
					}
					eventPump.RegisterMainPump(timeSpan2);
				}
			}
		}

		private static int GetEventCountToCheck(long startCounter, EventWatermark firstMissedEventWatermark, long firstMissedEventCounter, long lastMissedEventCounter)
		{
			if (firstMissedEventWatermark.WasEventProcessed && firstMissedEventWatermark.MapiWatermark == lastMissedEventCounter)
			{
				return 0;
			}
			if (!EventPump.IsEventBetweenCounters(firstMissedEventCounter, lastMissedEventCounter, startCounter))
			{
				return 0;
			}
			ulong num = (ulong)(lastMissedEventCounter - startCounter);
			int result;
			if (num >= 1000UL)
			{
				result = 1000;
			}
			else
			{
				result = (int)(num + 1UL);
			}
			return result;
		}

		private static bool IsEventBetweenCounters(long firstCounter, long lastCounter, long eventCounter)
		{
			return eventCounter >= firstCounter && eventCounter <= lastCounter;
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				if (disposing && this.disposeLock != null)
				{
					try
					{
						this.disposeLock.EnterWriteLock();
						this.isDisposed = true;
					}
					finally
					{
						try
						{
							this.disposeLock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
				this.InternalDispose(disposing);
				ExTraceGlobals.EventTracer.TraceDebug<EventPump>((long)this.GetHashCode(), "EventPump::Dispose. {0}", this);
			}
		}

		private void ModifyEventSinkList(EventPump.ModifyEventSinkListType modificationType, EventSink eventSink)
		{
			lock (this.sinkListLock)
			{
				this.isModifyingEventSinkList = true;
				try
				{
					Dictionary<Guid, List<EventSink>> dictionary = this.eventSinks;
					if (modificationType == EventPump.ModifyEventSinkListType.AddEventSink)
					{
						List<EventSink> collection = null;
						List<EventSink> list;
						if (dictionary.TryGetValue(eventSink.MailboxGuid, out collection))
						{
							list = new List<EventSink>(collection);
						}
						else
						{
							list = new List<EventSink>();
						}
						list.Add(eventSink);
						Dictionary<Guid, List<EventSink>> dictionary2 = dictionary.ShallowCopy<Guid, List<EventSink>>();
						dictionary2[eventSink.MailboxGuid] = list;
						this.eventSinks = dictionary2;
						if (this.exception != null)
						{
							eventSink.HandleException(this.exception);
						}
					}
					else
					{
						List<EventSink> collection2 = dictionary[eventSink.MailboxGuid];
						List<EventSink> list2 = new List<EventSink>(collection2);
						list2.Remove(eventSink);
						Dictionary<Guid, List<EventSink>> dictionary3 = dictionary.ShallowCopy<Guid, List<EventSink>>();
						if (list2.Count == 0)
						{
							dictionary3.Remove(eventSink.MailboxGuid);
						}
						else
						{
							dictionary3[eventSink.MailboxGuid] = list2;
						}
						this.eventSinks = dictionary3;
					}
				}
				finally
				{
					this.isModifyingEventSinkList = false;
				}
			}
		}

		private void CrashPump(LocalizedException exception)
		{
			Dictionary<Guid, List<EventSink>> dictionary = null;
			lock (this.sinkListLock)
			{
				this.exception = exception;
				dictionary = this.eventSinks;
			}
			ExTraceGlobals.EventTracer.TraceDebug<EventPump, Exception>((long)this.GetHashCode(), "EventPump::CrashPump. {0}. We got an error while reading events on the current EventPump. The EventPump has been disabled. Error = {1}.", this, this.exception);
			foreach (List<EventSink> list in dictionary.Values)
			{
				foreach (EventSink eventSink in list)
				{
					eventSink.HandleException(exception);
				}
			}
			this.eventPumpManager.RemoveBrokenEventPump(this);
		}

		private bool ReadAndDistributeEvents(Dictionary<Guid, List<EventSink>> eventSinkDictionary)
		{
			long num = this.lastEventCounter;
			bool flag = false;
			int num2 = 0;
			while (!flag)
			{
				MapiEvent[] array = null;
				long num3 = this.lastEventCounter + 1L;
				long num4 = 0L;
				try
				{
					try
					{
						this.disposeLock.EnterReadLock();
						if (this.IsDisposed)
						{
							return false;
						}
						array = EventPump.ReadEvents(this.mapiEventManager, this.mapiEventManagerLock, num3, 1000, 2000, null, out num4);
					}
					finally
					{
						try
						{
							this.disposeLock.ExitReadLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
				catch (StoragePermanentException arg)
				{
					ExTraceGlobals.EventTracer.TraceDebug<EventPump, StoragePermanentException>((long)this.GetHashCode(), "EventPump::ReadAndDistributeEvents. {0}. Exception caught while reading events. Exception = {1}.", this, arg);
					this.CrashPump(arg);
					return false;
				}
				catch (StorageTransientException arg2)
				{
					ExTraceGlobals.EventTracer.TraceDebug<EventPump, StorageTransientException>((long)this.GetHashCode(), "EventPump::ReadAndDistributeEvents. {0}. Exception caught while reading events. Exception = {1}.", this, arg2);
					this.CrashPump(arg2);
					return false;
				}
				foreach (MapiEvent mapiEvent in array)
				{
					List<EventSink> list = null;
					this.lastEventCounter = mapiEvent.EventCounter;
					if (eventSinkDictionary.TryGetValue(mapiEvent.MailboxGuid, out list))
					{
						foreach (EventSink eventSink in list)
						{
							try
							{
								eventSink.Consume(mapiEvent);
							}
							catch (StoragePermanentException arg3)
							{
								ExTraceGlobals.EventTracer.TraceDebug<EventPump, StoragePermanentException>((long)this.GetHashCode(), "EventPump::ReadAndDistributeEvents. {0}. Exception caught while distributing events. Exception = {1}.", this, arg3);
								eventSink.HandleException(arg3);
							}
							catch (StorageTransientException arg4)
							{
								ExTraceGlobals.EventTracer.TraceDebug<EventPump, StorageTransientException>((long)this.GetHashCode(), "EventPump::ReadAndDistributeEvents. {0}. Exception caught while distributing events. Exception = {1}.", this, arg4);
								eventSink.HandleException(arg4);
							}
						}
					}
				}
				num2 += array.Length;
				flag = (array.Length == 0 && num3 >= num4);
				if (flag && num3 == num4)
				{
					this.lastEventCounter = num4 - 1L;
					continue;
				}
				this.lastEventCounter = num4;
			}
			foreach (List<EventSink> list2 in eventSinkDictionary.Values)
			{
				foreach (EventSink eventSink2 in list2)
				{
					eventSink2.SetLastKnownWatermark(this.lastEventCounter, true);
				}
			}
			ExTraceGlobals.EventTracer.TraceDebug((long)this.GetHashCode(), "EventPump::ReadAndDistributeEvents. {0}. Events processed = {1}. PreviousLastEventCounter = {2}. LastEventCounter = {3}.", new object[]
			{
				this,
				num2,
				num,
				this.lastEventCounter
			});
			return true;
		}

		private long ReadLastEventWatermark()
		{
			StoreSession storeSession = null;
			bool flag = false;
			long eventCounter;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				ExDateTime utcNow = ExDateTime.UtcNow;
				TimeSpan? arg = null;
				try
				{
					if (!Monitor.TryEnter(this.mapiEventManagerLock, EventPump.eventSinkCreationTimeout))
					{
						ExTraceGlobals.EventTracer.TraceDebug<EventPump, TimeSpan>((long)this.GetHashCode(), "EventPump::ReadLastEventWatermark {0}. Could not get MapiEventManager lock after {1} timeout", this, EventPump.eventSinkCreationTimeout);
						throw new CannotCompleteOperationException(ServerStrings.ExReadEventsFailed);
					}
					arg = new TimeSpan?(ExDateTime.UtcNow - utcNow);
					eventCounter = this.mapiEventManager.ReadLastEvent(false).EventCounter;
				}
				finally
				{
					if (Monitor.IsEntered(this.mapiEventManagerLock))
					{
						Monitor.Exit(this.mapiEventManagerLock);
						ExTraceGlobals.EventTracer.TraceDebug<EventPump, TimeSpan?>((long)this.GetHashCode(), "EventPump::ReadLastEventWatermark {0}. Got MapiEventManager lock in {1}", this, arg);
					}
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCaughtMapiExceptionWhileReadingEvents, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("EventPump::ReadLastEventWatermark. Failed to read current watermark.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCaughtMapiExceptionWhileReadingEvents, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("EventPump::ReadLastEventWatermark. Failed to read current watermark.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return eventCounter;
		}

		private void RegisterMainPump(TimeSpan timeSpan)
		{
			if (!this.isDisposed)
			{
				this.registeredPumpDisposedWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.pumpDisposedEvent, new WaitOrTimerCallback(EventPump.ReadAndDistributeEvents), new WeakReference(this), timeSpan, true);
			}
		}

		private void InternalStopPumpThread()
		{
			this.registeredPumpDisposedWaitHandle.Unregister(this.pumpDisposedEvent);
		}

		private const int EventCount = 1000;

		private const int EventCountToCheck = 2000;

		private readonly EventPumpManager eventPumpManager;

		private readonly AutoResetEvent pumpDisposedEvent = new AutoResetEvent(false);

		private readonly EventPumpThreadLimiter threadLimiter;

		private readonly string server;

		private readonly Guid mdbGuid;

		private readonly ExRpcAdmin exRpcAdmin;

		private readonly MapiEventManager mapiEventManager;

		private readonly object mapiEventManagerLock = new object();

		private readonly ReaderWriterLockSlim disposeLock = new ReaderWriterLockSlim();

		private readonly object sinkListLock = new object();

		private readonly DisposeTracker disposeTracker;

		private static TimeSpan pollingTimeSpan = new TimeSpan(0, 0, 2);

		private static TimeSpan eventSinkCreationTimeout = new TimeSpan(0, 0, 10);

		private bool isDisposed;

		private int referenceCount;

		private long lastEventCounter;

		private Exception exception;

		private Dictionary<Guid, List<EventSink>> eventSinks = new Dictionary<Guid, List<EventSink>>();

		private bool isModifyingEventSinkList;

		private RegisteredWaitHandle registeredPumpDisposedWaitHandle;

		private enum ModifyEventSinkListType
		{
			AddEventSink,
			RemoveEventSink
		}
	}
}
