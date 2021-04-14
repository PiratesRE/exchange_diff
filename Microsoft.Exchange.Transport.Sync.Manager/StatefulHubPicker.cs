using System;
using System.Globalization;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class StatefulHubPicker
	{
		private StatefulHubPicker()
		{
			this.hubState = new StatefulHubPicker.HubState();
		}

		internal static StatefulHubPicker Instance
		{
			get
			{
				return StatefulHubPicker.instance;
			}
		}

		internal void Shutdown()
		{
			lock (this.syncObject)
			{
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)112UL, StatefulHubPicker.Tracer, "StatefulHubPicker Shutdown called.", new object[0]);
				this.hubLoadReducerTimer.Dispose();
				this.hubLoadReducerTimer = null;
				this.hubActivatorTimer.Dispose();
				this.hubActivatorTimer = null;
				this.hubSubscriptionTypeActivatorTimer.Dispose();
				this.hubSubscriptionTypeActivatorTimer = null;
				if (this.currentServer != null)
				{
					this.currentServer.Dispose();
					this.currentServer = null;
				}
				this.state = StatefulHubPicker.StatefulHubPickerStatus.Stopped;
			}
		}

		internal void Start()
		{
			lock (this.syncObject)
			{
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)113UL, StatefulHubPicker.Tracer, "StatefulHubPicker: Start called.", new object[0]);
				this.hubActivatorTimer = new StatefulHubPicker.WorkTimer(ContentAggregationConfig.HubInactivityPeriod, new TimerCallback(this.HubActivator));
				this.hubLoadReducerTimer = new StatefulHubPicker.WorkTimer(ContentAggregationConfig.HubBusyPeriod, new TimerCallback(this.HubLoadReducer));
				this.hubSubscriptionTypeActivatorTimer = new StatefulHubPicker.WorkTimer(ContentAggregationConfig.HubSubscriptionTypeNotSupportedPeriod, new TimerCallback(this.HubSubscriptionTypeActivator));
				this.state = StatefulHubPicker.StatefulHubPickerStatus.Started;
			}
		}

		internal void ResetHubLoad()
		{
			lock (this.syncObject)
			{
				this.CheckStarted();
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)90UL, StatefulHubPicker.Tracer, "Resetting hub load.", new object[0]);
				switch (this.hubState.Status)
				{
				case StatefulHubPicker.HubState.HubStatus.Busy:
					this.hubLoadReducerTimer.Deactivate();
					break;
				case StatefulHubPicker.HubState.HubStatus.Inactive:
					this.hubActivatorTimer.Deactivate();
					break;
				}
				this.hubState.Status = StatefulHubPicker.HubState.HubStatus.Active;
			}
		}

		internal XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("StatefulHubPicker");
			lock (this.syncObject)
			{
				if (this.state == StatefulHubPicker.StatefulHubPickerStatus.Started)
				{
					XElement xelement2 = new XElement("HubState");
					xelement2.Add(new XElement("status", this.hubState.Status));
					xelement2.Add(new XElement("firstUnsuccessfulDispatchTime", (this.hubState.FirstUnsuccessfulDispatch != null) ? this.hubState.FirstUnsuccessfulDispatch.Value.ToString("o") : string.Empty));
					xelement2.Add(new XElement("lastDispatchAttemptTime", (this.hubState.LastDispatchUpdate != null) ? this.hubState.LastDispatchUpdate.Value.ToString("o") : string.Empty));
					xelement.Add(xelement2);
					this.hubLoadReducerTimer.AddDiagnosticsTo(xelement, "HubLoadReducer");
					this.hubActivatorTimer.AddDiagnosticsTo(xelement, "HubActivator");
					this.hubSubscriptionTypeActivatorTimer.AddDiagnosticsTo(xelement, "HubSubscriptionTypeActivator");
				}
			}
			return xelement;
		}

		internal bool IsSubscriptionTypeEnabled(AggregationSubscriptionType subscriptionType)
		{
			bool result;
			lock (this.syncObject)
			{
				this.CheckStarted();
				result = this.hubState.IsSubscriptionTypeEnabled(subscriptionType);
			}
			return result;
		}

		internal bool TryGetServerForDispatch(out ContentAggregationHubServer hubServer, out SubscriptionSubmissionResult? lastSubmissionResult)
		{
			hubServer = null;
			lock (this.syncObject)
			{
				this.CheckStarted();
				lastSubmissionResult = this.lastSubmissionResult;
				if (this.currentServer == null)
				{
					ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)221UL, StatefulHubPicker.Tracer, "Reset current hub server", new object[0]);
					this.currentServer = new ContentAggregationHubServer();
				}
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)222UL, StatefulHubPicker.Tracer, "Current Hub Server: {0} Status: {1}", new object[]
				{
					this.currentServer,
					this.hubState
				});
				if (this.hubState.IsActive())
				{
					ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)223UL, StatefulHubPicker.Tracer, "Hub Selected for next dispatch.", new object[0]);
					hubServer = this.currentServer;
					hubServer.IncrementRef();
				}
				else
				{
					ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)296UL, StatefulHubPicker.Tracer, "No Hub Selected for next dispatch.", new object[0]);
				}
			}
			return hubServer != null;
		}

		internal void ProcessDispatchResult(ContentAggregationHubServer server, AggregationSubscriptionType subscriptionType, SubscriptionSubmissionResult subscriptionSubmissionResult, ExDateTime dispatchTime)
		{
			lock (this.syncObject)
			{
				this.CheckStarted();
				this.lastSubmissionResult = new SubscriptionSubmissionResult?(subscriptionSubmissionResult);
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)300UL, StatefulHubPicker.Tracer, "ProcessDispatchResult called: SubscriptionSubmissionResult {0}.", new object[]
				{
					subscriptionSubmissionResult
				});
				this.MarkHubDispatchTime(dispatchTime, subscriptionSubmissionResult);
				if (subscriptionSubmissionResult == SubscriptionSubmissionResult.ServerNotAvailable)
				{
					ContentAggregationConfig.LogEvent(TransportSyncManagerEventLogConstants.Tuple_NoActiveBridgeheadServerForContentAggregation, Environment.MachineName, new object[0]);
					if (this.currentServer == server)
					{
						this.currentServer = null;
					}
					server.RequestDispose();
				}
				switch (subscriptionSubmissionResult)
				{
				case SubscriptionSubmissionResult.Success:
				case SubscriptionSubmissionResult.SubscriptionAlreadyOnHub:
				case SubscriptionSubmissionResult.UnknownRetryableError:
				case SubscriptionSubmissionResult.MaxConcurrentMailboxSubmissions:
				case SubscriptionSubmissionResult.RpcServerTooBusy:
				case SubscriptionSubmissionResult.RetryableRpcError:
				case SubscriptionSubmissionResult.DatabaseRpcLatencyUnhealthy:
				case SubscriptionSubmissionResult.DatabaseHealthUnknown:
				case SubscriptionSubmissionResult.DatabaseOverloaded:
				case SubscriptionSubmissionResult.TransportSyncDisabled:
				case SubscriptionSubmissionResult.UserTransportQueueUnhealthy:
				case SubscriptionSubmissionResult.TransportQueueHealthUnknown:
					goto IL_133;
				case SubscriptionSubmissionResult.SchedulerQueueFull:
				case SubscriptionSubmissionResult.ServerTransportQueueUnhealthy:
					this.MarkHubBusy();
					goto IL_133;
				case SubscriptionSubmissionResult.ServerNotAvailable:
				case SubscriptionSubmissionResult.EdgeTransportStopped:
					this.MarkHubInactive();
					goto IL_133;
				case SubscriptionSubmissionResult.SubscriptionTypeDisabled:
					this.MarkHubSubscriptionTypeDisabled(subscriptionType);
					goto IL_133;
				}
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)179UL, StatefulHubPicker.Tracer, "ProcessDispatchResult: Unexpected value of SubscriptionSubmissionResult: {0}", new object[]
				{
					subscriptionSubmissionResult
				});
				IL_133:
				server.DecrementRef();
			}
		}

		private void MarkHubDispatchTime(ExDateTime dispatchTime, SubscriptionSubmissionResult subscriptionSubmissionResult)
		{
			if (subscriptionSubmissionResult == SubscriptionSubmissionResult.ServerNotAvailable)
			{
				if (this.hubState.FirstUnsuccessfulDispatch == null)
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)301UL, StatefulHubPicker.Tracer, "Hub server became unavailable for dispatch.", new object[0]);
					this.hubState.FirstUnsuccessfulDispatch = new ExDateTime?(dispatchTime);
				}
			}
			else
			{
				if (this.hubState.FirstUnsuccessfulDispatch != null)
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)302UL, StatefulHubPicker.Tracer, "Hub server became available again for dispatch.", new object[0]);
				}
				this.hubState.FirstUnsuccessfulDispatch = null;
			}
			this.hubState.LastDispatchUpdate = new ExDateTime?(dispatchTime);
		}

		private void MarkHubBusy()
		{
			if (this.hubState.Status != StatefulHubPicker.HubState.HubStatus.Busy)
			{
				this.hubState.Status = StatefulHubPicker.HubState.HubStatus.Busy;
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)303UL, StatefulHubPicker.Tracer, "Setting hub status to Busy.", new object[0]);
				if (this.hubLoadReducerTimer.TryActivate())
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)304UL, StatefulHubPicker.Tracer, "Activating the busy timer.", new object[0]);
				}
			}
		}

		private void MarkHubInactive()
		{
			if (this.hubState.Status != StatefulHubPicker.HubState.HubStatus.Inactive)
			{
				this.hubState.Status = StatefulHubPicker.HubState.HubStatus.Inactive;
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)305UL, StatefulHubPicker.Tracer, "Setting hub status to Inactive.", new object[0]);
				if (this.hubActivatorTimer.TryActivate())
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)308UL, StatefulHubPicker.Tracer, "Activating the inactive timer.", new object[0]);
				}
			}
		}

		private void MarkHubSubscriptionTypeDisabled(AggregationSubscriptionType subscriptionType)
		{
			if (this.hubState.IsSubscriptionTypeEnabled(subscriptionType))
			{
				this.hubState.MarkSubscriptionTypeDisabled(subscriptionType);
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)310UL, StatefulHubPicker.Tracer, "Setting subscription type: {0} disabled on hub.", new object[]
				{
					subscriptionType
				});
				if (!this.hubSubscriptionTypeActivatorTimer.TryActivate())
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)312UL, StatefulHubPicker.Tracer, "Activating the subscription type disabled timer", new object[0]);
				}
			}
		}

		private void CheckStarted()
		{
			if (this.state != StatefulHubPicker.StatefulHubPickerStatus.Started)
			{
				throw new InvalidOperationException("Expected HubPicker to be in Started state.");
			}
		}

		private void HubActivator(object state)
		{
			lock (this.syncObject)
			{
				if (this.state != StatefulHubPicker.StatefulHubPickerStatus.Stopped)
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)315UL, StatefulHubPicker.Tracer, "HubActivator Invoked", new object[0]);
					if (this.hubState.Status == StatefulHubPicker.HubState.HubStatus.Inactive)
					{
						ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)316UL, StatefulHubPicker.Tracer, "Activating hub server.", new object[0]);
						this.hubState.Status = StatefulHubPicker.HubState.HubStatus.Active;
					}
				}
			}
		}

		private void HubLoadReducer(object state)
		{
			lock (this.syncObject)
			{
				if (this.state != StatefulHubPicker.StatefulHubPickerStatus.Stopped)
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)317UL, StatefulHubPicker.Tracer, "HubLoadReducer Invoked", new object[0]);
					if (this.hubState.Status == StatefulHubPicker.HubState.HubStatus.Busy)
					{
						ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)318UL, StatefulHubPicker.Tracer, "Marking hub server to be not busy.", new object[0]);
						this.hubState.Status = StatefulHubPicker.HubState.HubStatus.Active;
					}
				}
			}
		}

		private void HubSubscriptionTypeActivator(object state)
		{
			lock (this.syncObject)
			{
				if (this.state != StatefulHubPicker.StatefulHubPickerStatus.Stopped)
				{
					ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)319UL, StatefulHubPicker.Tracer, "HubSubscriptionTypeActivator Invoked.", new object[0]);
					this.hubState.MarkAllSubscriptionTypesEnabled();
				}
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.StatefulHubPickerTracer;

		private static readonly StatefulHubPicker instance = new StatefulHubPicker();

		private readonly object syncObject = new object();

		private readonly StatefulHubPicker.HubState hubState;

		private StatefulHubPicker.StatefulHubPickerStatus state;

		private ContentAggregationHubServer currentServer;

		private StatefulHubPicker.WorkTimer hubLoadReducerTimer;

		private StatefulHubPicker.WorkTimer hubActivatorTimer;

		private StatefulHubPicker.WorkTimer hubSubscriptionTypeActivatorTimer;

		private SubscriptionSubmissionResult? lastSubmissionResult;

		private enum StatefulHubPickerStatus
		{
			Created,
			Started,
			Stopped
		}

		private sealed class HubState
		{
			internal HubState()
			{
				this.hubStatus = StatefulHubPicker.HubState.HubStatus.Active;
				this.firstUnsuccessfulDispatch = null;
				this.lastDispatchUpdate = null;
				this.enabledSubscriptionTypes = AggregationSubscriptionType.All;
			}

			internal StatefulHubPicker.HubState.HubStatus Status
			{
				get
				{
					return this.hubStatus;
				}
				set
				{
					this.hubStatus = value;
				}
			}

			internal ExDateTime? FirstUnsuccessfulDispatch
			{
				get
				{
					return this.firstUnsuccessfulDispatch;
				}
				set
				{
					this.firstUnsuccessfulDispatch = value;
				}
			}

			internal ExDateTime? LastDispatchUpdate
			{
				get
				{
					return this.lastDispatchUpdate;
				}
				set
				{
					this.lastDispatchUpdate = value;
				}
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "Status: {0}, FirstUnsuccessfulDispatch: {1}, LastDispatch: {2}, EnabledSubscriptionTypes: {3}", new object[]
				{
					this.hubStatus,
					this.firstUnsuccessfulDispatch,
					this.lastDispatchUpdate,
					this.enabledSubscriptionTypes
				});
			}

			internal bool IsActive()
			{
				return this.hubStatus == StatefulHubPicker.HubState.HubStatus.Active;
			}

			internal bool IsSubscriptionTypeEnabled(AggregationSubscriptionType subscriptionType)
			{
				return (this.enabledSubscriptionTypes & subscriptionType) != AggregationSubscriptionType.Unknown;
			}

			internal void MarkSubscriptionTypeDisabled(AggregationSubscriptionType subscriptionType)
			{
				this.enabledSubscriptionTypes &= ~subscriptionType;
			}

			internal void MarkAllSubscriptionTypesEnabled()
			{
				this.enabledSubscriptionTypes = AggregationSubscriptionType.All;
			}

			private StatefulHubPicker.HubState.HubStatus hubStatus;

			private ExDateTime? firstUnsuccessfulDispatch;

			private ExDateTime? lastDispatchUpdate;

			private AggregationSubscriptionType enabledSubscriptionTypes;

			internal enum HubStatus
			{
				Active,
				Busy,
				Inactive
			}
		}

		private class WorkTimer : DisposeTrackableBase
		{
			public WorkTimer(TimeSpan delay, TimerCallback timerCallback)
			{
				SyncUtilities.ThrowIfArgumentNull("timerCallback", timerCallback);
				this.delay = delay;
				this.timerCallback = timerCallback;
				this.timer = new GuardedTimer(new TimerCallback(this.OnTimerFired));
				this.Deactivate();
			}

			public void Deactivate()
			{
				base.CheckDisposed();
				lock (this.syncRoot)
				{
					if (this.active)
					{
						this.timer.Change(StatefulHubPicker.WorkTimer.Infinite, StatefulHubPicker.WorkTimer.Infinite);
						this.nextInvokationTime = null;
						this.active = false;
					}
				}
			}

			public bool TryActivate()
			{
				base.CheckDisposed();
				bool result;
				lock (this.syncRoot)
				{
					if (!this.active)
					{
						this.nextInvokationTime = new ExDateTime?(ExDateTime.UtcNow + this.delay);
						this.timer.Change(this.delay, StatefulHubPicker.WorkTimer.Infinite);
						this.active = true;
						result = true;
					}
					else
					{
						result = false;
					}
				}
				return result;
			}

			public void AddDiagnosticsTo(XElement componentElement, string tagName)
			{
				base.CheckDisposed();
				XElement xelement = new XElement(tagName);
				xelement.Add(new XElement("active", this.active.ToString()));
				xelement.Add(new XElement("nextInvocationTime", (this.nextInvokationTime != null) ? this.nextInvokationTime.Value.ToString("o") : string.Empty));
				componentElement.Add(xelement);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					lock (this.syncRoot)
					{
						if (this.timer != null)
						{
							this.timer.Dispose(true);
							this.timer = null;
						}
					}
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<StatefulHubPicker.WorkTimer>(this);
			}

			private void OnTimerFired(object state)
			{
				this.timerCallback(null);
				this.Deactivate();
			}

			private static readonly TimeSpan Infinite = TimeSpan.FromMilliseconds(-1.0);

			private readonly object syncRoot = new object();

			private readonly TimeSpan delay;

			private readonly TimerCallback timerCallback;

			private GuardedTimer timer;

			private bool active;

			private ExDateTime? nextInvokationTime;
		}
	}
}
