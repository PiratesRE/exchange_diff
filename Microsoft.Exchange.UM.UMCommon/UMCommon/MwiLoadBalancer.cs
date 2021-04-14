using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MwiLoadBalancer
	{
		internal MwiLoadBalancer(Microsoft.Exchange.Diagnostics.Trace tracer, IServerPicker<IMwiTarget, Guid> serverPicker, MwiFailureEventLogStrategy eventLogStrategy)
		{
			this.tracer = tracer;
			this.serverPicker = serverPicker;
			this.activeMessageCount = 0;
			this.shutdownInProgress = false;
			this.shutdownEvent = new AutoResetEvent(false);
			this.eventLogStrategy = eventLogStrategy;
			this.InitializePerfCounters();
		}

		internal AutoResetEvent ShutDownEvent
		{
			get
			{
				return this.shutdownEvent;
			}
		}

		internal void SendMessage(MwiMessage message)
		{
			try
			{
				this.TraceDebug("MwiLoadBalancer.SendMessage(message={0})", new object[]
				{
					message
				});
				if (this.perfCounters != null)
				{
					MwiDiagnostics.IncrementCounter(this.perfCounters.TotalMwiMessages);
				}
				this.SendMessageToNextTarget(message);
			}
			catch (DataSourceTransientException error)
			{
				this.HandleDeliveryFailure(message, error);
			}
			catch (DataValidationException error2)
			{
				this.HandleDeliveryFailure(message, error2);
			}
			catch (DataSourceOperationException error3)
			{
				this.HandleDeliveryFailure(message, error3);
			}
		}

		internal void Shutdown()
		{
			bool flag = false;
			lock (this)
			{
				this.shutdownInProgress = true;
				flag = (this.activeMessageCount > 0);
			}
			if (flag)
			{
				this.TraceDebug("MwiLoadBalancer.Shutdown: Waiting for shutdownEvent event.", new object[0]);
				if (!this.shutdownEvent.WaitOne(MwiLoadBalancer.ShutdownTimeout, false))
				{
					this.TraceWarning("MwiLoadBalancer.Shutdown: Timed out waiting for shutdown event", new object[0]);
				}
			}
			this.shutdownEvent.Close();
			this.shutdownEvent = null;
		}

		internal void ShutdownAsync()
		{
			lock (this)
			{
				this.shutdownInProgress = true;
				if (this.activeMessageCount <= 0)
				{
					this.shutdownEvent.Set();
				}
			}
		}

		internal void CleanupAfterAsyncShutdown()
		{
			this.shutdownEvent.Close();
			this.shutdownEvent = null;
		}

		internal virtual IServerPicker<IMwiTarget, Guid> GetTargetPicker(Guid dialPlanGuid)
		{
			return this.serverPicker;
		}

		private void InitializePerfCounters()
		{
			string processName;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				processName = currentProcess.ProcessName;
			}
			this.perfCounters = MwiDiagnostics.GetInstance(processName);
			this.perfCounters.Reset();
		}

		private void SendMessageToNextTarget(MwiMessage message)
		{
			this.TraceDebug("MwiLoadBalancer.SendMessageToNextTarget: GetNextAvailableTarget()", new object[0]);
			if (MwiLoadBalancer.outstandingRequestsCount >= MwiLoadBalancer.MaxNumOfOutstandingRequests)
			{
				this.HandleDeliveryFailure(message, new TooManyOustandingMwiRequestsException(message.UserName));
				return;
			}
			if (message.Expired)
			{
				this.HandleDeliveryFailure(message, new MwiMessageExpiredException(message.UserName));
				return;
			}
			IServerPicker<IMwiTarget, Guid> targetPicker = this.GetTargetPicker(message.DialPlanGuid);
			int num = 0;
			IMwiTarget mwiTarget = null;
			for (int i = 0; i < 2; i++)
			{
				mwiTarget = targetPicker.PickNextServer(message.DialPlanGuid, message.TenantGuid, out num);
				if (mwiTarget == null || !mwiTarget.Equals(message.CurrentTarget))
				{
					break;
				}
			}
			if (mwiTarget == null || mwiTarget.Equals(message.CurrentTarget) || message.NumberOfTargetsAttempted >= num)
			{
				this.HandleDeliveryFailure(message, new MwiNoTargetsAvailableException(message.UserName));
				return;
			}
			lock (this)
			{
				if (!this.shutdownInProgress)
				{
					this.activeMessageCount++;
					int num2 = Interlocked.Increment(ref MwiLoadBalancer.outstandingRequestsCount);
					message.SendAsync(mwiTarget, new SendMessageCompletedDelegate(this.SendMessageCompletedCallback));
					this.TraceDebug("MwiLB.SendMessageToNextTarget: MsgCount={0} TotalReqs={1}", new object[]
					{
						this.activeMessageCount,
						num2
					});
				}
				else
				{
					this.TraceWarning("MwiLB.SendMessageToNextTarget: Shutting down, discarding message", new object[0]);
				}
			}
		}

		private void SendMessageCompletedCallback(MwiMessage message, MwiDeliveryException error)
		{
			lock (this)
			{
				if (this.perfCounters != null)
				{
					TimeSpan timeSpan = ExDateTime.UtcNow.Subtract(message.SentTimeUtc);
					MwiDiagnostics.SetCounterValue(this.perfCounters.AverageMwiProcessingTime, this.averageMwiProcessingTime.Update((long)timeSpan.TotalMilliseconds));
				}
				this.activeMessageCount--;
				int num = Interlocked.Decrement(ref MwiLoadBalancer.outstandingRequestsCount);
				this.TraceWarning("LB.SendMessageCompletedCallback: Msg:{0} Err={1} MsgCnt={2} TotalReqs={3} Shutdown={4}", new object[]
				{
					message,
					error,
					this.activeMessageCount,
					num,
					this.shutdownInProgress
				});
				if (this.shutdownInProgress)
				{
					if (this.activeMessageCount == 0 && this.shutdownEvent != null)
					{
						this.TraceWarning("LB.SendMessageCompletedCallback: There are no pending requests->shutdownEvent.Set()", new object[0]);
						this.shutdownEvent.Set();
					}
					return;
				}
			}
			if (error != null)
			{
				message.DeliveryErrors.Add(error);
				IServerPicker<IMwiTarget, Guid> targetPicker = this.GetTargetPicker(message.DialPlanGuid);
				targetPicker.ServerUnavailable(message.CurrentTarget);
				this.SendMessageToNextTarget(message);
				return;
			}
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MwiMessageDeliverySucceeded, null, new object[]
			{
				message.UnreadVoicemailCount,
				message.TotalVoicemailCount - message.UnreadVoicemailCount,
				message.MailboxDisplayName,
				message.UserExtension,
				message.CurrentTarget.Name
			});
		}

		private void HandleDeliveryFailure(MwiMessage message, Exception error)
		{
			this.TraceError("MwiLoadBalancer.HandleDeliveryFailure: Message={0}. Error={1}", new object[]
			{
				message,
				error
			});
			if (this.perfCounters != null)
			{
				MwiDiagnostics.IncrementCounter(this.perfCounters.TotalFailedMwiMessages);
			}
			this.eventLogStrategy.LogFailure(message, error);
		}

		private void TraceDebug(string format, params object[] args)
		{
			CallIdTracer.TraceDebug(this.tracer, this.GetHashCode(), format, args);
		}

		private void TraceWarning(string format, params object[] args)
		{
			CallIdTracer.TraceWarning(this.tracer, this.GetHashCode(), format, args);
		}

		private void TraceError(string format, params object[] args)
		{
			CallIdTracer.TraceError(this.tracer, this.GetHashCode(), format, args);
		}

		internal static readonly TimeSpan ShutdownTimeout = new TimeSpan(0, 1, 0);

		internal static readonly int MaxNumOfOutstandingRequests = 500 * Environment.ProcessorCount;

		private static int outstandingRequestsCount = 0;

		private MwiLoadBalancerPerformanceCountersInstance perfCounters;

		private MovingAverage averageMwiProcessingTime = new MovingAverage(50);

		private int activeMessageCount;

		private bool shutdownInProgress;

		private AutoResetEvent shutdownEvent;

		private Microsoft.Exchange.Diagnostics.Trace tracer;

		private IServerPicker<IMwiTarget, Guid> serverPicker;

		private MwiFailureEventLogStrategy eventLogStrategy;
	}
}
