using System;
using System.Threading;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class Governor : Base, IDisposable
	{
		public Governor(Governor parentGovernor)
		{
			this.lastRunTime = (DateTime)ExDateTime.Now;
			this.status = GovernorStatus.Running;
			this.parentGovernor = parentGovernor;
		}

		public GovernorStatus Status
		{
			get
			{
				return this.status;
			}
		}

		protected DateTime LastRunTime
		{
			get
			{
				return this.lastRunTime;
			}
		}

		public GovernorStatus GetHierarchyStatus()
		{
			Governor governor = this;
			GovernorStatus governorStatus;
			do
			{
				governorStatus = governor.Status;
				if (governorStatus != GovernorStatus.Running)
				{
					break;
				}
				governor = governor.parentGovernor;
			}
			while (governor != null);
			return governorStatus;
		}

		public void Dispose()
		{
			if (this.retryTimer != null)
			{
				this.retryTimer.Dispose();
			}
		}

		public bool ReportResult(AIException exception)
		{
			bool flag = true;
			ExTraceGlobals.GovernorTracer.TraceDebug<Governor, AIException>((long)this.GetHashCode(), "{0}: ReportResult: {1}", this, exception);
			AITransientException ex = null;
			if (exception is AITransientException && this.IsFailureRelevant((AITransientException)exception))
			{
				ex = (AITransientException)exception;
				ExTraceGlobals.GovernorTracer.TraceDebug<Governor>((long)this.GetHashCode(), "{0}: Exception is relevant", this);
			}
			lock (this)
			{
				switch (this.status)
				{
				case GovernorStatus.Running:
					if (ex != null)
					{
						this.numberConsecutiveFailures = 0U;
						this.lastRunTime = DateTime.UtcNow;
						this.ReportFailure(ex);
						flag = false;
					}
					break;
				case GovernorStatus.Retry:
					if (ex == null)
					{
						this.LogRecovery(exception);
						this.EnterRun();
					}
					else if (ex.RetrySchedule.FinalAction != FinalAction.RetryForever && this.lastRunTime + ex.RetrySchedule.TimeToGiveUp <= DateTime.UtcNow)
					{
						this.numberConsecutiveFailures += 1U;
						this.LogGiveUp(exception);
						this.EnterRun();
					}
					else
					{
						this.ReportFailure(ex);
						flag = false;
					}
					break;
				case GovernorStatus.Failure:
					if (ex != null)
					{
						flag = false;
						this.LogFailure(GovernorStatus.Failure, ex);
					}
					break;
				}
			}
			if (this.parentGovernor != null)
			{
				flag &= this.parentGovernor.ReportResult(exception);
			}
			return flag;
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableGovernor queryableGovernor = queryableObject as QueryableGovernor;
			if (queryableGovernor != null)
			{
				queryableGovernor.Status = this.status.ToString();
				queryableGovernor.LastRunTime = this.lastRunTime;
				queryableGovernor.NumberConsecutiveFailures = (long)((ulong)this.numberConsecutiveFailures);
			}
		}

		protected abstract bool IsFailureRelevant(AITransientException exception);

		protected virtual void Run()
		{
		}

		protected abstract void Retry();

		protected virtual void OnFailure()
		{
		}

		protected virtual void Log30MinuteWarning(AITransientException exception, TimeSpan nextRetryInterval)
		{
		}

		private static void InternalTimerCallback(object state)
		{
			((Governor)state).RetryTimerCallback();
		}

		private TimeSpan GetNextRetryInterval(RetrySchedule retrySchedule)
		{
			return retrySchedule.GetRetryInterval(this.numberConsecutiveFailures - 1U);
		}

		private void ReportFailure(AITransientException transientException)
		{
			this.numberConsecutiveFailures += 1U;
			this.LogFailure(this.status, transientException);
			this.status = GovernorStatus.Failure;
			ExTraceGlobals.GovernorTracer.TraceDebug<Governor>((long)this.GetHashCode(), "{0}: Starting timer", this);
			TimeSpan nextRetryInterval = this.GetNextRetryInterval(transientException.RetrySchedule);
			this.retryTimer = new Timer(Governor.timerCallback, this, nextRetryInterval, TimeSpan.Zero);
			this.OnFailure();
		}

		private void EnterRun()
		{
			this.status = GovernorStatus.Running;
			this.logged30MinuteWarning = false;
			this.Run();
		}

		private void RetryTimerCallback()
		{
			ExTraceGlobals.GovernorTracer.TraceDebug<Governor>((long)this.GetHashCode(), "{0}: Retry timer firing -- time to retry", this);
			lock (this)
			{
				this.retryTimer.Dispose();
				this.retryTimer = null;
				this.status = GovernorStatus.Retry;
				this.LogRetry();
				this.Retry();
			}
		}

		private void LogFailure(GovernorStatus oldStatus, AITransientException exception)
		{
			TimeSpan nextRetryInterval = this.GetNextRetryInterval(exception.RetrySchedule);
			ExTraceGlobals.GovernorTracer.TraceDebug((long)this.GetHashCode(), "{0}: {1}->Failure. {2} attempts in timespan {3}. Next retry time: now+{4}. Exception: {5}.", new object[]
			{
				this,
				oldStatus,
				this.numberConsecutiveFailures,
				DateTime.UtcNow - this.lastRunTime,
				nextRetryInterval,
				exception
			});
			base.LogEvent(AssistantsEventLogConstants.Tuple_GovernorFailure, null, new object[]
			{
				this,
				oldStatus,
				this.numberConsecutiveFailures,
				DateTime.UtcNow - this.lastRunTime,
				nextRetryInterval,
				exception
			});
			if (!this.logged30MinuteWarning && DateTime.UtcNow - this.lastRunTime > TimeSpan.FromMinutes(30.0))
			{
				this.Log30MinuteWarning(exception, nextRetryInterval);
				this.logged30MinuteWarning = true;
			}
		}

		private void LogRecovery(AIException exception)
		{
			ExTraceGlobals.GovernorTracer.TraceDebug((long)this.GetHashCode(), "{0}: Retry->Running. {1} attempts in timespan {2}. Exception: {3}.", new object[]
			{
				this,
				this.numberConsecutiveFailures,
				DateTime.UtcNow - this.lastRunTime,
				exception
			});
			base.LogEvent(AssistantsEventLogConstants.Tuple_GovernorRecovery, null, new object[]
			{
				this,
				this.numberConsecutiveFailures,
				DateTime.UtcNow - this.lastRunTime,
				exception
			});
		}

		private void LogGiveUp(AIException exception)
		{
			ExTraceGlobals.GovernorTracer.TraceDebug((long)this.GetHashCode(), "{0}: Retry->Running. Giving up after {1} attempts in timespan {2}. Exception: {3}", new object[]
			{
				this,
				this.numberConsecutiveFailures,
				DateTime.UtcNow - this.lastRunTime,
				exception
			});
			base.LogEvent(AssistantsEventLogConstants.Tuple_GovernorGiveUp, null, new object[]
			{
				this,
				this.numberConsecutiveFailures,
				DateTime.UtcNow - this.lastRunTime,
				exception
			});
		}

		private void LogRetry()
		{
			ExTraceGlobals.GovernorTracer.TraceDebug<Governor, uint, TimeSpan>((long)this.GetHashCode(), "{0}: Failure->Retry. {1} attempts in timespan {2}.", this, this.numberConsecutiveFailures, DateTime.UtcNow - this.lastRunTime);
			base.LogEvent(AssistantsEventLogConstants.Tuple_GovernorRetry, null, new object[]
			{
				this,
				this.numberConsecutiveFailures,
				DateTime.UtcNow - this.lastRunTime
			});
		}

		private static TimerCallback timerCallback = new TimerCallback(Governor.InternalTimerCallback);

		private GovernorStatus status;

		private DateTime lastRunTime;

		private uint numberConsecutiveFailures;

		private Timer retryTimer;

		private Governor parentGovernor;

		private bool logged30MinuteWarning;
	}
}
