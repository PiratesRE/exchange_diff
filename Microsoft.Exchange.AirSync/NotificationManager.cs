using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.AirSync
{
	internal class NotificationManager : IEventHandler, IDisposeTrackable, IDisposable
	{
		private NotificationManager(IAsyncCommand command, string uniqueId, int policyHashCode, uint policyKey)
		{
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Created);
			this.createdUTC = DateTime.UtcNow;
			this.command = command;
			this.uniqueId = uniqueId;
			this.policyHashCode = policyHashCode;
			this.policyKey = policyKey;
			this.disposeTracker = this.GetDisposeTracker();
			this.budgetKey = this.command.Context.BudgetKey;
			this.emailAddress = this.command.Context.SmtpAddress;
			this.deviceIdentity = this.command.Context.DeviceIdentity;
			this.accountValidationContext = this.command.Context.AccountValidationContext;
			Guid mdbGuid = this.command.Context.MdbGuid;
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "NotificationManager created for {0}", this.uniqueId);
		}

		public object Information { get; set; }

		public uint RequestedWaitTime { get; private set; }

		public bool MailboxLoggingEnabled { get; set; }

		public static NotificationManagerResult GetDiagnosticInfo(CallType callType, string argument)
		{
			NotificationManagerResult notificationManagerResult = new NotificationManagerResult();
			notificationManagerResult.CacheCount = NotificationManager.notificationManagerCache.Count;
			notificationManagerResult.RemovedCount = NotificationManager.removedInstances.Count;
			notificationManagerResult.CreatesPerMinute = (int)NotificationManager.createsPerMinute.GetValue();
			notificationManagerResult.HitsPerMinute = (int)NotificationManager.hitsPerMinute.GetValue();
			notificationManagerResult.ContentionsPerMinute = (int)NotificationManager.cacheContentionsPerMinute.GetValue();
			notificationManagerResult.StealsPerMinute = (int)NotificationManager.stealsPerMinute.GetValue();
			bool flag = callType != CallType.Metadata;
			if (flag)
			{
				notificationManagerResult.RemovedInstances = new List<NotificationManagerResultItem>();
				foreach (KeyValuePair<string, NotificationManager> keyValuePair in NotificationManager.removedInstances)
				{
					if (keyValuePair.Value.Matches(callType, argument))
					{
						notificationManagerResult.RemovedInstances.Add(keyValuePair.Value.GetInstanceDiagnosticInformation());
					}
				}
			}
			lock (NotificationManager.notificationManagerCache)
			{
				List<NotificationManagerResultItem> list = flag ? new List<NotificationManagerResultItem>() : null;
				List<NotificationManagerResultItem> list2 = flag ? new List<NotificationManagerResultItem>() : null;
				int num = 0;
				int num2 = 0;
				foreach (KeyValuePair<string, NotificationManager> keyValuePair2 in NotificationManager.notificationManagerCache)
				{
					if (keyValuePair2.Value.command != null)
					{
						num++;
						if (flag && keyValuePair2.Value.Matches(callType, argument))
						{
							list.Add(keyValuePair2.Value.GetInstanceDiagnosticInformation());
						}
					}
					else
					{
						num2++;
						if (flag && keyValuePair2.Value.Matches(callType, argument))
						{
							list2.Add(keyValuePair2.Value.GetInstanceDiagnosticInformation());
						}
					}
				}
				notificationManagerResult.ActiveInstances = list;
				notificationManagerResult.ActiveCount = num;
				notificationManagerResult.InactiveInstances = list2;
				notificationManagerResult.InactiveCount = num2;
			}
			return notificationManagerResult;
		}

		public static void ClearCache()
		{
			lock (NotificationManager.notificationManagerCache)
			{
				NotificationManager.notificationManagerCache.Clear();
			}
		}

		public static NotificationManager CreateNotificationManager(INotificationManagerContext context, IAsyncCommand command)
		{
			string text = NotificationManager.GetUniqueId(context);
			uint num = context.PolicyKey;
			int mailboxPolicyHash = context.MailboxPolicyHash;
			NotificationManager notificationManager = new NotificationManager(command, text, mailboxPolicyHash, num);
			NotificationManager notificationManager2;
			lock (NotificationManager.notificationManagerCache)
			{
				if (NotificationManager.notificationManagerCache.TryGetValue(text, out notificationManager2))
				{
					NotificationManager.cacheContentionsPerMinute.Add(1U);
					notificationManager2.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Removed);
					NotificationManager.notificationManagerCache.Remove(text);
					NotificationManager.removedInstances[text] = notificationManager2;
				}
				NotificationManager.createsPerMinute.Add(1U);
				notificationManager.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Cached);
				NotificationManager.notificationManagerCache.Add(text, notificationManager);
				AirSyncCounters.NumberOfNotificationManagerInCache.RawValue = (long)NotificationManager.notificationManagerCache.Count;
			}
			if (notificationManager2 != null)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, notificationManager, "Disposing existing NotificationManager for {0}", text);
				notificationManager2.EnqueueEvent(new NotificationManager.AsyncEvent(NotificationManager.AsyncEventType.Acquire, null));
				notificationManager2.EnqueueDispose();
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, notificationManager, "Created notification manager for {0}", text);
			return notificationManager;
		}

		public static NotificationManager GetOrCreateNotificationManager(INotificationManagerContext context, IAsyncCommand command, out bool wasTakenOver)
		{
			string text = NotificationManager.GetUniqueId(context);
			uint num = context.PolicyKey;
			int mailboxPolicyHash = context.MailboxPolicyHash;
			wasTakenOver = false;
			NotificationManager notificationManager = null;
			NotificationManager notificationManager2 = null;
			lock (NotificationManager.notificationManagerCache)
			{
				bool flag2 = NotificationManager.notificationManagerCache.TryGetValue(text, out notificationManager2);
				if (flag2)
				{
					NotificationManager.hitsPerMinute.Add(1U);
				}
				if (flag2 && notificationManager2.subscriptionCanBeTaken && !notificationManager2.MailboxLoggingEnabled && mailboxPolicyHash == notificationManager2.policyHashCode && num == notificationManager2.policyKey)
				{
					wasTakenOver = true;
				}
				else
				{
					if (flag2)
					{
						NotificationManager.cacheContentionsPerMinute.Add(1U);
						notificationManager2.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Removed);
						NotificationManager.notificationManagerCache.Remove(text);
						NotificationManager.removedInstances[text] = notificationManager2;
						notificationManager = notificationManager2;
					}
					notificationManager2 = new NotificationManager(command, text, mailboxPolicyHash, num);
					NotificationManager.createsPerMinute.Add(1U);
					notificationManager2.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Cached);
					NotificationManager.notificationManagerCache.Add(text, notificationManager2);
					AirSyncCounters.NumberOfNotificationManagerInCache.RawValue = (long)NotificationManager.notificationManagerCache.Count;
				}
			}
			if (notificationManager != null)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, notificationManager2, "Disposing existing NotificationManager for {0}", text);
				notificationManager.EnqueueEvent(new NotificationManager.AsyncEvent(NotificationManager.AsyncEventType.Acquire, null));
				notificationManager.EnqueueDispose();
				notificationManager = null;
			}
			if (wasTakenOver)
			{
				NotificationManager.AsyncEvent evt = new NotificationManager.AsyncEvent(NotificationManager.AsyncEventType.Acquire, command);
				if (!notificationManager2.EnqueueEvent(evt))
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, notificationManager2, "A NotificationManager was attempted to be taken over but failed for {0}", text);
					wasTakenOver = false;
					notificationManager2 = NotificationManager.CreateNotificationManager(context, command);
				}
				else
				{
					notificationManager2.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Stolen);
					NotificationManager.stealsPerMinute.Add(1U);
				}
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, notificationManager2, "Got or created notification manager for {0}", text);
			return notificationManager2;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationManager>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			throw new InvalidOperationException("DO NOT CALL NotificationManager.Dispose()! Use EnqueueDispose() instead!");
		}

		public void EnqueueDispose()
		{
			if (this.disposed)
			{
				return;
			}
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Enqueue Dispose: Enqueue a kill event.");
			this.EnqueueEvent(new NotificationManager.AsyncEvent(NotificationManager.AsyncEventType.Kill));
		}

		internal Queue<NotificationManager.AsyncEvent> QueuedEventsForTest
		{
			get
			{
				return this.eventQueue;
			}
		}

		public void Consume(Event evt)
		{
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Consume Event called {0}.", this.uniqueId);
			AccountState accountState = AccountState.AccountEnabled;
			if (this.accountValidationContext != null)
			{
				accountState = this.accountValidationContext.CheckAccount();
			}
			if (accountState != AccountState.AccountEnabled)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Account Terminated {0}.", this.uniqueId);
				this.EnqueueEvent(new NotificationManager.AsyncEvent(accountState));
				return;
			}
			this.EnqueueEvent(new NotificationManager.AsyncEvent(evt));
		}

		public void HandleException(Exception ex)
		{
			AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RequestsTracer, this, "HandleException called. uniqueID: {0}, Exception:{1}.", this.uniqueId, ex.ToString());
			this.EnqueueEvent(new NotificationManager.AsyncEvent(ex));
		}

		public void ProcessQueuedEvents(IAsyncCommand callingCommand)
		{
			NotificationManager.AsyncEvent asyncEvent = null;
			if (this.currentlyExecuting)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "ProcessQueuedEvents.UniqueId:{0}.  Already processing event.  Exiting.", this.uniqueId);
				return;
			}
			lock (this.eventQueue)
			{
				if (this.currentlyExecuting)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "ProcessQueuedEvents.UniqueId:{0}.  Already processing event.  Exiting.", this.uniqueId);
					return;
				}
				this.currentlyExecuting = true;
			}
			IBudget budget = null;
			try
			{
				this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.RunStart);
				budget = StandardBudget.Acquire(this.budgetKey);
				lock (this.eventQueue)
				{
					if (callingCommand != null)
					{
						callingCommand.ProcessingEventsEnabled = true;
					}
					AirSyncDiagnostics.TraceDebug<string, string, int>(ExTraceGlobals.RequestsTracer, this, "ProcessQueuedEvents.UniqueId:{0}, ProcessingEnabled: {1}, eventQueue count:{2}.", this.uniqueId, (callingCommand == null) ? "<null command>" : "true", this.eventQueue.Count);
					asyncEvent = this.GetNextEvent();
				}
				IAsyncCommand cmd = null;
				bool flag3 = asyncEvent != null;
				while (flag3)
				{
					try
					{
						cmd = this.StartCommandContext();
						AirSyncDiagnostics.TraceDebug<NotificationManager.AsyncEventType, string, bool>(ExTraceGlobals.ThreadingTracer, this, "Processing event {0} for {1}. Command is null? {2}", asyncEvent.Type, this.uniqueId, this.command == null);
						switch (asyncEvent.Type)
						{
						case NotificationManager.AsyncEventType.XsoEventAvailable:
							this.ProcessXsoEventAvailable(asyncEvent, budget);
							break;
						case NotificationManager.AsyncEventType.XsoException:
							this.ProcessXsoException(asyncEvent);
							break;
						case NotificationManager.AsyncEventType.Timeout:
							this.ProcessTimeout(budget);
							break;
						case NotificationManager.AsyncEventType.Acquire:
							this.ProcessAcquire(asyncEvent);
							break;
						case NotificationManager.AsyncEventType.Release:
							this.ProcessRelease(asyncEvent);
							break;
						case NotificationManager.AsyncEventType.Kill:
							this.Kill();
							break;
						case NotificationManager.AsyncEventType.AccountTerminated:
							this.ProcessAccountTerminated(asyncEvent);
							break;
						}
					}
					catch (Exception ex)
					{
						AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.ThreadingTracer, this, "Exception in processQueuedEvents for {0}. Exception:{1}", this.uniqueId, ex.ToString());
						if (!this.InternalHandleException(ex))
						{
							throw;
						}
					}
					finally
					{
						this.EndCommandContext(cmd);
						asyncEvent = this.GetNextEvent();
						flag3 = (asyncEvent != null);
					}
				}
			}
			finally
			{
				this.currentlyExecuting = false;
				this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.RunEnd);
				if (budget != null)
				{
					try
					{
						budget.Dispose();
					}
					catch (FailFastException arg)
					{
						AirSyncDiagnostics.TraceError<FailFastException>(ExTraceGlobals.RequestsTracer, this, "Budget.Dispose failed with exception: {0}", arg);
					}
				}
			}
		}

		public void ReleaseCommand(IAsyncCommand command)
		{
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Release Command for {0}", this.uniqueId);
			NotificationManager.AsyncEvent evt = new NotificationManager.AsyncEvent(NotificationManager.AsyncEventType.Release, command);
			this.EnqueueEvent(evt);
		}

		public void StartTimer(uint timeout, ExDateTime requestTime, ExDateTime policyExpirationTime)
		{
			if (this.timer != null)
			{
				throw new InvalidOperationException();
			}
			if (timeout == 0U)
			{
				throw new ArgumentException("Timeout cannot be 0!");
			}
			this.RequestedWaitTime = timeout;
			this.policyExpirationTime = policyExpirationTime;
			this.InternalStartTimer(requestTime, timeout);
		}

		public void Add(EventSubscription subscription)
		{
			this.eventSubscriptions.Add(subscription);
		}

		public void SubscriptionsCannotBeTaken()
		{
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Locked);
			this.subscriptionCanBeTaken = false;
		}

		public NotificationManagerResultItem GetInstanceDiagnosticInformation()
		{
			NotificationManagerResultItem notificationManagerResultItem = new NotificationManagerResultItem();
			notificationManagerResultItem.UniqueId = this.uniqueId;
			IAsyncCommand asyncCommand = this.command;
			notificationManagerResultItem.Command = ((asyncCommand == null) ? string.Empty : asyncCommand.ToString());
			notificationManagerResultItem.EmailAddress = this.emailAddress;
			notificationManagerResultItem.DeviceId = this.deviceIdentity.DeviceId;
			notificationManagerResultItem.TotalAcquires = this.totalAcquires;
			notificationManagerResultItem.TotalKills = this.totalKills;
			notificationManagerResultItem.TotalReleases = this.totalReleases;
			notificationManagerResultItem.TotalTimeouts = this.totalTimeouts;
			notificationManagerResultItem.TotalXsoEvents = this.totalXsoEvents;
			notificationManagerResultItem.TotalXsoExceptions = this.totalXsoExceptions;
			notificationManagerResultItem.IsExecuting = this.currentlyExecuting;
			notificationManagerResultItem.QueueCount = this.eventQueue.Count;
			notificationManagerResultItem.PolicyKey = (long)((ulong)this.policyKey);
			notificationManagerResultItem.LiveTime = (DateTime.UtcNow - this.createdUTC).ToString();
			notificationManagerResultItem.RequestedWaitTime = TimeSpan.FromSeconds(this.RequestedWaitTime).ToString();
			notificationManagerResultItem.Actions = new List<InstanceAction>(this.instanceEvents.Count);
			foreach (InstanceAction item in this.instanceEvents)
			{
				notificationManagerResultItem.Actions.Add(item);
			}
			lock (this.eventQueue)
			{
				notificationManagerResultItem.QueuedEvents = new List<string>(this.eventQueue.Count);
				foreach (NotificationManager.AsyncEvent asyncEvent in this.eventQueue)
				{
					notificationManagerResultItem.QueuedEvents.Add(asyncEvent.Type.ToString());
				}
			}
			return notificationManagerResultItem;
		}

		private static string GetUniqueId(INotificationManagerContext context)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", new object[]
			{
				context.MailboxGuid.ToString(),
				context.DeviceIdentity,
				context.CommandType.ToString()
			});
		}

		private static void TimerExpired(object state)
		{
			NotificationManager notificationManager = (NotificationManager)state;
			AccountState accountState = AccountState.AccountEnabled;
			if (notificationManager.accountValidationContext != null)
			{
				accountState = notificationManager.accountValidationContext.CheckAccount();
			}
			if (accountState != AccountState.AccountEnabled)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, notificationManager, "Account Terminated {0}.", notificationManager.uniqueId);
				notificationManager.EnqueueEvent(new NotificationManager.AsyncEvent(accountState));
				return;
			}
			notificationManager.EnqueueEvent(new NotificationManager.AsyncEvent(NotificationManager.AsyncEventType.Timeout));
		}

		private void ProcessXsoEventAvailable(NotificationManager.AsyncEvent evt, IBudget budget)
		{
			Interlocked.Increment(ref this.totalXsoEvents);
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.XsoEvent, new EventType?(evt.Event.EventType), new EventObjectType?(evt.Event.ObjectType), null);
			if (this.command == null)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "XSO event available for {0}, but command is not available. Calling Kill", this.uniqueId);
				this.Kill();
				return;
			}
			budget.CheckOverBudget();
			this.command.Consume(evt.Event);
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "XSO event available for {0}. calling Command.Consume", this.uniqueId);
		}

		private void ProcessXsoException(NotificationManager.AsyncEvent evt)
		{
			Interlocked.Increment(ref this.totalXsoExceptions);
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.XsoException, null, null, evt.Exception);
			if (this.command == null)
			{
				if (!this.InternalHandleException(evt.Exception))
				{
					throw evt.Exception;
				}
			}
			else
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "XSO exception event available for {0}. calling Command.HandleException", this.uniqueId);
				this.command.HandleException(evt.Exception);
			}
		}

		private void ProcessTimeout(IBudget budget)
		{
			Interlocked.Increment(ref this.totalTimeouts);
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.HBTimeout);
			if (ExDateTime.UtcNow.CompareTo(this.lastTargetTime) >= 0)
			{
				if (this.command != null)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Timed out for {0}. calling command.HeartbeatCallback", this.uniqueId);
					budget.CheckOverBudget();
					this.command.HeartbeatCallback();
					return;
				}
				this.Kill();
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Timed out and killing {0}", this.uniqueId);
			}
		}

		private void ProcessAcquire(NotificationManager.AsyncEvent evt)
		{
			Interlocked.Increment(ref this.totalAcquires);
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Acquired);
			if (this.command != null)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Release notificationManager for {0}", this.uniqueId);
				this.command.ReleaseNotificationManager(true);
			}
			this.command = evt.Command;
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Acquiring notification manager for {0}", this.uniqueId);
			if (this.RequestedWaitTime > 0U && this.command != null)
			{
				this.InternalStartTimer(this.command.Context.RequestTime, this.RequestedWaitTime);
			}
		}

		private void ProcessRelease(NotificationManager.AsyncEvent evt)
		{
			Interlocked.Increment(ref this.totalReleases);
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Released);
			evt.Command.ReleaseNotificationManager(false);
			if (this.command == evt.Command)
			{
				this.command = null;
				if (this.RequestedWaitTime > 0U && !this.disposed)
				{
					this.InternalStartTimer(ExDateTime.UtcNow, 120U);
					return;
				}
				this.Kill();
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Release & killing {0}", this.uniqueId);
			}
		}

		private void ProcessAccountTerminated(NotificationManager.AsyncEvent evt)
		{
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.AccountTerminated);
			if (this.command == null)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Account terminated after XSO event callback for {0}, but command is not available. Calling Kill", this.uniqueId);
				this.Kill();
				return;
			}
			this.command.HandleAccountTerminated(evt);
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Account terminated after XSO event callback for {0}. calling Command.HandleAccountTerminated", this.uniqueId);
		}

		private IAsyncCommand StartCommandContext()
		{
			IAsyncCommand asyncCommand = this.command;
			if (asyncCommand != null)
			{
				asyncCommand.SetContextDataInTls();
				if (asyncCommand.PerUserTracingEnabled)
				{
					AirSyncDiagnostics.SetThreadTracing();
				}
			}
			return asyncCommand;
		}

		private void EndCommandContext(IAsyncCommand cmd)
		{
			if (cmd != null)
			{
				Command.ClearContextDataInTls();
				if (cmd.PerUserTracingEnabled)
				{
					AirSyncDiagnostics.ClearThreadTracing();
				}
			}
		}

		private NotificationManager.AsyncEvent GetNextEvent()
		{
			IAsyncCommand asyncCommand = this.command;
			NotificationManager.AsyncEvent result;
			lock (this.eventQueue)
			{
				if (this.eventQueue.Count > 0)
				{
					NotificationManager.AsyncEvent asyncEvent = this.eventQueue.Peek();
					if ((asyncCommand != null && !asyncCommand.ProcessingEventsEnabled) || (asyncEvent.Command != null && !asyncEvent.Command.ProcessingEventsEnabled))
					{
						result = null;
					}
					else
					{
						result = this.eventQueue.Dequeue();
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private void Kill()
		{
			Interlocked.Increment(ref this.totalKills);
			if (this.disposed)
			{
				return;
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Processing Kill event for {0}", this.uniqueId);
			lock (NotificationManager.notificationManagerCache)
			{
				NotificationManager notificationManager;
				if (NotificationManager.notificationManagerCache.TryGetValue(this.uniqueId, out notificationManager) && notificationManager == this)
				{
					notificationManager.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Removed);
					NotificationManager.notificationManagerCache.Remove(this.uniqueId);
					AirSyncCounters.NumberOfNotificationManagerInCache.RawValue = (long)NotificationManager.notificationManagerCache.Count;
				}
			}
			NotificationManager notificationManager2 = null;
			if (NotificationManager.removedInstances.TryGetValue(this.uniqueId, out notificationManager2) && notificationManager2 == this)
			{
				NotificationManager.removedInstances.TryRemove(this.uniqueId, out notificationManager2);
			}
			this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Killed);
			lock (this.eventQueue)
			{
				GC.SuppressFinalize(this);
				this.subscriptionCanBeTaken = false;
				this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Locked);
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
				if (this.eventSubscriptions.Count > 0)
				{
					foreach (EventSubscription eventSubscription in this.eventSubscriptions)
					{
						eventSubscription.Dispose();
					}
					this.eventSubscriptions.Clear();
				}
				List<NotificationManager.AsyncEvent> list = new List<NotificationManager.AsyncEvent>(this.eventSubscriptions.Count + 1);
				if (this.command != null)
				{
					NotificationManager.AsyncEvent item = new NotificationManager.AsyncEvent(NotificationManager.AsyncEventType.Release, this.command);
					list.Add(item);
				}
				foreach (NotificationManager.AsyncEvent asyncEvent in this.eventQueue)
				{
					if (asyncEvent.Type == NotificationManager.AsyncEventType.Acquire)
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ThreadingTracer, this, "Changing Acquire into a Release for {0}", this.uniqueId);
						asyncEvent.Type = NotificationManager.AsyncEventType.Release;
						list.Add(asyncEvent);
					}
					else if (asyncEvent.Type == NotificationManager.AsyncEventType.Release)
					{
						list.Add(asyncEvent);
					}
					else
					{
						AirSyncDiagnostics.TraceDebug<NotificationManager.AsyncEventType, string>(ExTraceGlobals.ThreadingTracer, this, "Ignoring event {0} after a Kill for {1}", asyncEvent.Type, this.uniqueId);
					}
				}
				this.eventQueue.Clear();
				foreach (NotificationManager.AsyncEvent item2 in list)
				{
					this.eventQueue.Enqueue(item2);
				}
				this.disposed = true;
			}
		}

		private void InternalStartTimer(ExDateTime startTime, uint timeout)
		{
			AirSyncDiagnostics.TraceDebug<uint, string>(ExTraceGlobals.ThreadingTracer, this, "Starting timer with {0} seconds for {1}.", timeout, this.uniqueId);
			uint num = (timeout > (uint)GlobalSettings.EarlyWakeupBufferTime) ? (timeout - (uint)GlobalSettings.EarlyWakeupBufferTime) : timeout;
			ExDateTime exDateTime = startTime.AddSeconds(num);
			ExDateTime utcNow = ExDateTime.UtcNow;
			int num2 = (int)exDateTime.Subtract(utcNow).TotalMilliseconds + 1;
			if (num2 < 0)
			{
				num2 = 0;
			}
			this.lastTargetTime = exDateTime.AddSeconds(-2.0);
			if (this.timer == null)
			{
				this.timer = new Timer(NotificationManager.timerCallback, this, num2, -1);
				return;
			}
			this.timer.Change(num2, -1);
		}

		private bool InternalHandleException(Exception ex)
		{
			if (this.command != null)
			{
				this.command.HandleException(ex);
				return true;
			}
			if (AirSyncUtility.HandleNonCriticalException(ex, true))
			{
				this.Kill();
				return true;
			}
			return false;
		}

		private bool EnqueueEvent(NotificationManager.AsyncEvent evt)
		{
			bool result;
			lock (this.eventQueue)
			{
				if (this.disposed && evt.Type != NotificationManager.AsyncEventType.Release)
				{
					result = false;
				}
				else if (evt.Type == NotificationManager.AsyncEventType.Acquire && (!this.subscriptionCanBeTaken || ExDateTime.UtcNow > this.policyExpirationTime))
				{
					result = false;
				}
				else
				{
					if (evt.Type != NotificationManager.AsyncEventType.Timeout && evt.Type != NotificationManager.AsyncEventType.Acquire && evt.Type != NotificationManager.AsyncEventType.Release)
					{
						this.subscriptionCanBeTaken = false;
						this.EnqueueDiagOperation(NotificationManager.DiagnosticEvent.Locked);
					}
					this.eventQueue.Enqueue(evt);
					AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.ThreadingTracer, this, "Enqueue event for {0}. processingEnabled:{1}", this.uniqueId, (this.command == null) ? "<null command>" : this.command.ProcessingEventsEnabled.ToString());
					if (this.command == null || this.command.ProcessingEventsEnabled)
					{
						this.ProcessQueuedEvents(null);
					}
					result = true;
				}
			}
			return result;
		}

		private void EnqueueDiagOperation(NotificationManager.DiagnosticEvent evt)
		{
			this.EnqueueDiagOperation(evt, null, null, null);
		}

		private void EnqueueDiagOperation(NotificationManager.DiagnosticEvent evt, EventType? xsoEventType, EventObjectType? xsoEventObjectType, Exception xsoException)
		{
			if (this.instanceEvents.Count < 100)
			{
				this.instanceEvents.Enqueue(new InstanceAction
				{
					Action = evt.ToString(),
					Time = DateTime.UtcNow,
					ThreadId = Thread.CurrentThread.ManagedThreadId,
					XsoEventType = ((xsoEventType != null) ? xsoEventType.Value.ToString() : null),
					XsoObjectType = ((xsoEventObjectType != null) ? xsoEventObjectType.Value.ToString() : null),
					XsoException = ((xsoException == null) ? null : xsoException.ToString())
				});
			}
		}

		private bool Matches(CallType callType, string argument)
		{
			switch (callType)
			{
			case CallType.EmailAddress:
				return string.Compare(this.emailAddress, argument, true) == 0;
			case CallType.DeviceId:
				return this.deviceIdentity.IsDeviceId(argument);
			default:
				return true;
			}
		}

		private void RunWithThreadContext(Action action)
		{
			bool flag = false;
			IAsyncCommand asyncCommand = this.command;
			if (asyncCommand != null)
			{
				asyncCommand.SetContextDataInTls();
				if (asyncCommand.PerUserTracingEnabled)
				{
					AirSyncDiagnostics.SetThreadTracing();
				}
				flag = true;
			}
			try
			{
				action();
			}
			finally
			{
				if (flag)
				{
					Command.ClearContextDataInTls();
					if (asyncCommand.PerUserTracingEnabled)
					{
						AirSyncDiagnostics.ClearThreadTracing();
					}
				}
			}
		}

		private const int InstanceEventMaxCount = 100;

		private static readonly TimerCallback timerCallback = new TimerCallback(NotificationManager.TimerExpired);

		private static Dictionary<string, NotificationManager> notificationManagerCache = new Dictionary<string, NotificationManager>(500);

		private static ConcurrentDictionary<string, NotificationManager> removedInstances = new ConcurrentDictionary<string, NotificationManager>();

		private static FixedTimeSum createsPerMinute = new FixedTimeSum(6000, 10);

		private static FixedTimeSum hitsPerMinute = new FixedTimeSum(6000, 10);

		private static FixedTimeSum cacheContentionsPerMinute = new FixedTimeSum(6000, 10);

		private static FixedTimeSum stealsPerMinute = new FixedTimeSum(6000, 10);

		private int totalXsoEvents;

		private int totalXsoExceptions;

		private int totalTimeouts;

		private int totalKills;

		private int totalAcquires;

		private int totalReleases;

		private ConcurrentQueue<InstanceAction> instanceEvents = new ConcurrentQueue<InstanceAction>();

		private readonly DateTime createdUTC;

		private readonly string emailAddress;

		private readonly DeviceIdentity deviceIdentity;

		private Timer timer;

		private string uniqueId;

		private IAsyncCommand command;

		private bool subscriptionCanBeTaken = true;

		private List<EventSubscription> eventSubscriptions = new List<EventSubscription>(5);

		private Queue<NotificationManager.AsyncEvent> eventQueue = new Queue<NotificationManager.AsyncEvent>(4);

		private bool disposed;

		private ExDateTime lastTargetTime = ExDateTime.MinValue;

		private int policyHashCode;

		private uint policyKey;

		private BudgetKey budgetKey;

		private bool currentlyExecuting;

		private DisposeTracker disposeTracker;

		private ExDateTime policyExpirationTime = ExDateTime.MaxValue;

		private IAccountValidationContext accountValidationContext;

		private enum DiagnosticEvent
		{
			Created,
			XsoEvent,
			XsoException,
			Cached,
			Removed,
			Stolen,
			Killed,
			HBTimeout,
			Acquired,
			Released,
			Locked,
			RunStart,
			RunEnd,
			AccountTerminated
		}

		internal enum AsyncEventType
		{
			XsoEventAvailable,
			XsoException,
			Timeout,
			Acquire,
			Release,
			Kill,
			AccountTerminated
		}

		internal abstract class HbiMonitor
		{
			public void Initialize(int heartbeatSampleSize, int heartbeatAlertThreshold)
			{
				this.hbiSamples = new uint[heartbeatSampleSize];
				this.heartbeatAlertThreshold = heartbeatAlertThreshold;
				AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.ProtocolTracer, this, "Heartbeat monitor has sample size {0} and alert threshold {1}.", this.hbiSamples.Length, this.heartbeatAlertThreshold);
			}

			public void RegisterSample(uint heartbeatInterval, INotificationManagerContext context)
			{
				if (context.DeviceIdentity.DeviceType.StartsWith("TestActiveSyncConnectivity"))
				{
					return;
				}
				lock (this.hbiSamples)
				{
					uint num = this.hbiSamples[this.insertionIndex];
					this.hbiSamples[this.insertionIndex] = heartbeatInterval;
					this.insertionIndex = (this.insertionIndex + 1) % this.hbiSamples.Length;
					this.hbiSum += heartbeatInterval;
					this.hbiSum -= num;
					if ((ulong)this.numSamples == (ulong)((long)this.hbiSamples.Length))
					{
						uint num2 = this.hbiSum / this.numSamples;
						if ((ulong)num2 <= (ulong)((long)this.heartbeatAlertThreshold))
						{
							string text = context.CommandType.ToString();
							AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_AverageHbiTooLow, text, new string[]
							{
								num2.ToString(CultureInfo.InvariantCulture),
								text,
								this.heartbeatAlertThreshold.ToString(CultureInfo.InvariantCulture)
							});
						}
					}
					else
					{
						this.numSamples += 1U;
					}
				}
			}

			private uint[] hbiSamples;

			private int insertionIndex;

			private uint hbiSum;

			private uint numSamples;

			private int heartbeatAlertThreshold;
		}

		internal class AsyncEvent
		{
			internal AsyncEvent(Event xsoEvent) : this(NotificationManager.AsyncEventType.XsoEventAvailable)
			{
				this.xsoEvent = xsoEvent;
			}

			internal AsyncEvent(NotificationManager.AsyncEventType type, IAsyncCommand command) : this(type)
			{
				this.command = command;
			}

			internal AsyncEvent(Exception exception) : this(NotificationManager.AsyncEventType.XsoException)
			{
				this.exception = exception;
			}

			internal AsyncEvent(NotificationManager.AsyncEventType type)
			{
				this.CreationDate = DateTime.UtcNow;
				this.type = type;
			}

			internal AsyncEvent(AccountState accountState) : this(NotificationManager.AsyncEventType.AccountTerminated)
			{
				this.accountState = accountState;
			}

			internal DateTime CreationDate { get; private set; }

			internal Event Event
			{
				get
				{
					return this.xsoEvent;
				}
			}

			internal Exception Exception
			{
				get
				{
					return this.exception;
				}
			}

			internal NotificationManager.AsyncEventType Type
			{
				get
				{
					return this.type;
				}
				set
				{
					this.type = value;
				}
			}

			internal IAsyncCommand Command
			{
				get
				{
					return this.command;
				}
			}

			internal AccountState AccountState
			{
				get
				{
					return this.accountState;
				}
			}

			private NotificationManager.AsyncEventType type;

			private Event xsoEvent;

			private Exception exception;

			private IAsyncCommand command;

			private AccountState accountState;
		}
	}
}
