using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class StreamingSubscription : SubscriptionBase
	{
		private void InternalEventAvailableHandler(object obj)
		{
			ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
			{
				Action<StreamingSubscription> action = obj as Action<StreamingSubscription>;
				if (action != null)
				{
					action(this);
				}
			});
		}

		private void InternalDisconnectSubscriptionHandler(object obj)
		{
			ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
			{
				Action<StreamingSubscription, LocalizedException> action = obj as Action<StreamingSubscription, LocalizedException>;
				if (action != null)
				{
					action(this, new SubscriptionNewConnectionOpenedException());
				}
			});
		}

		private void InternalDisposeSubscriptionHandler(object obj)
		{
			ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
			{
				Action<StreamingSubscription, LocalizedException> action = obj as Action<StreamingSubscription, LocalizedException>;
				if (action != null)
				{
					action(this, new SubscriptionUnsubscribedException());
				}
			});
		}

		public StreamingSubscription(StreamingSubscriptionRequest subscriptionRequest, IdAndSession[] folderIds, string owner) : base(subscriptionRequest, folderIds)
		{
			this.owner = owner;
			this.SetExpirationDateTime();
			base.EventQueue.RegisterEventAvailableHandler(new EventQueue.EventAvailableHandler(this.EventAvailableHandler));
		}

		internal static int TimeToLiveDefault
		{
			get
			{
				return StreamingSubscription.timeToLiveInSeconds;
			}
			set
			{
				StreamingSubscription.timeToLiveInSeconds = value;
			}
		}

		internal static int NewEventQueueSize
		{
			get
			{
				return StreamingSubscription.newEventQueueSize;
			}
			set
			{
				StreamingSubscription.newEventQueueSize = value;
			}
		}

		internal ExDateTime ExpirationDateTime
		{
			get
			{
				return this.expirationDateTime;
			}
		}

		private void SetExpirationDateTime()
		{
			this.expirationDateTime = ExDateTime.UtcNow.AddSeconds((double)StreamingSubscription.timeToLiveInSeconds);
		}

		public EwsNotificationType GetEvents(int maxEventCount, out int eventCount)
		{
			EwsNotificationType result;
			try
			{
				lock (this.lockObject)
				{
					ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "StreamingSubscription.GetEvents. Subscription: {0}. Current queue length is {1}{2}.", base.TraceIdentifier, (base.EventQueue == null) ? "<null>" : base.EventQueue.CurrentEventsCount.ToString(), (base.EventQueue == null) ? "<null>" : (base.EventQueue.HasMissedEvents ? " (missed events)" : string.Empty));
					if (this.isDisposed)
					{
						throw new InvalidSubscriptionException();
					}
					if (this.isExpired)
					{
						ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "StreamingSubscription.GetEvents. ExpiredSubscriptionException. Subscription: {0}.", base.TraceIdentifier);
						throw new ExpiredSubscriptionException();
					}
					if (base.EventQueue.HasMissedEvents)
					{
						ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "StreamingSubscription.GetEvents. EventQueueOverflowException. Subscription: {0}.", base.TraceIdentifier);
						this.ResetEventQueue();
						throw new EventQueueOverflowException();
					}
					BudgetKey budgetKey = null;
					IEwsBudget budget = CallContext.Current.Budget;
					if (budget == null)
					{
						ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "StreamingSubscription.GetEvents. Subscription: {0}. currentCallContextBudget == null", base.TraceIdentifier);
					}
					else
					{
						budgetKey = budget.Owner;
					}
					if (budgetKey != null && budgetKey.Equals(base.BudgetKey))
					{
						EwsNotificationType events = base.GetEvents(base.LastWatermarkSent, maxEventCount, out eventCount);
						result = events;
					}
					else
					{
						using (IEwsBudget ewsBudget = EwsBudget.Acquire(base.BudgetKey))
						{
							try
							{
								ewsBudget.StartLocal("StreamingSubscription.GetEvents", default(TimeSpan));
								EwsNotificationType events2 = base.GetEvents(base.LastWatermarkSent, maxEventCount, out eventCount);
								result = events2;
							}
							finally
							{
								ewsBudget.LogEndStateToIIS();
							}
						}
					}
				}
			}
			catch (LocalizedException ex)
			{
				if (!ex.Data.Contains(StreamingConnection.IsNonFatalSubscriptionExceptionKey) || (string)ex.Data[StreamingConnection.IsNonFatalSubscriptionExceptionKey] != bool.TrueString)
				{
					this.encounteredFatalError = true;
				}
				throw;
			}
			return result;
		}

		private void ResetEventQueue()
		{
			base.EventQueue.ResetQueue();
		}

		public override bool IsExpired
		{
			get
			{
				if (!this.isExpired)
				{
					lock (this.lockObject)
					{
						this.isExpired = (this.expirationDateTime < ExDateTime.UtcNow && this.openConnection == null);
					}
				}
				return this.isExpired;
			}
		}

		public override bool UseWatermarks
		{
			get
			{
				return false;
			}
		}

		protected override int EventQueueSize
		{
			get
			{
				return StreamingSubscription.NewEventQueueSize;
			}
		}

		private void EventAvailableHandler()
		{
			lock (this.connectionLock)
			{
				ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, string>((long)this.GetHashCode(), "StreamingSubscription.EventAvailableHandler. Subscription: {0}. New events available {1} active connection.", base.TraceIdentifier, (this.openConnection != null) ? "with" : "without");
				if (this.openConnection != null && !this.openConnection.IsDisposed)
				{
					Action<StreamingSubscription> state = new Action<StreamingSubscription>(this.openConnection.EventsAvailable);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.InternalEventAvailableHandler), state);
				}
			}
		}

		public override bool CheckCallerHasRights(CallContext callContext)
		{
			string identifierString = callContext.OriginalCallerContext.IdentifierString;
			if (callContext.IsPartnerUser)
			{
				int num = identifierString.IndexOf('\\');
				if (num > 0)
				{
					string value = identifierString.Substring(num);
					return this.owner.EndsWith(value);
				}
			}
			return this.owner.Equals(identifierString, StringComparison.OrdinalIgnoreCase);
		}

		internal void RegisterConnection(ISubscriptionEventHandler connection)
		{
			lock (this.connectionLock)
			{
				if (this.openConnection != connection)
				{
					if (this.openConnection != null && !this.openConnection.IsDisposed)
					{
						Action<StreamingSubscription, LocalizedException> state = new Action<StreamingSubscription, LocalizedException>(this.openConnection.DisconnectSubscription);
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.InternalDisconnectSubscriptionHandler), state);
					}
					this.openConnection = connection;
					if (!base.EventQueue.IsQueueEmptyAndUpToDate())
					{
						Action<StreamingSubscription> state2 = new Action<StreamingSubscription>(this.openConnection.EventsAvailable);
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.InternalEventAvailableHandler), state2);
					}
				}
			}
		}

		internal void UnregisterConnection(ISubscriptionEventHandler connection)
		{
			lock (this.connectionLock)
			{
				if (this.openConnection != connection)
				{
					return;
				}
				this.openConnection = null;
			}
			lock (this.lockObject)
			{
				if (this.encounteredFatalError)
				{
					Subscriptions.Singleton.Delete(base.SubscriptionId);
				}
				else
				{
					this.SetExpirationDateTime();
				}
			}
		}

		internal bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);
			lock (this.connectionLock)
			{
				if (isDisposing && this.openConnection != null)
				{
					Action<StreamingSubscription, LocalizedException> state = new Action<StreamingSubscription, LocalizedException>(this.openConnection.DisconnectSubscription);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.InternalDisposeSubscriptionHandler), state);
				}
			}
		}

		internal const int DefaultEventQueueSize = 500;

		internal const int DefaultTimeToLive = 1800;

		private readonly string owner;

		private static int timeToLiveInSeconds = 1800;

		private static int newEventQueueSize = 500;

		private bool isExpired;

		private bool encounteredFatalError;

		private ExDateTime expirationDateTime;

		private ISubscriptionEventHandler openConnection;

		private object connectionLock = new object();
	}
}
