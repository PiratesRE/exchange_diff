using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TransientErrorInfoPeriodic
	{
		public TransientErrorInfoPeriodic(TimeSpan successTransitionSuppression, TimeSpan successPeriodicInterval, TimeSpan failureTransitionSuppression, TimeSpan failurePeriodicInterval, TransientErrorInfo.ErrorType initialState)
		{
			this.m_successTransitionSuppression = successTransitionSuppression;
			this.m_successPeriodicInterval = successPeriodicInterval;
			this.m_failureTransitionSuppression = failureTransitionSuppression;
			this.m_failurePeriodicInterval = failurePeriodicInterval;
			this.m_internalSuppressionInfo = new TransientErrorInfo();
			this.SetCurrentSuppressedState(initialState);
		}

		public TimeSpan CurrentActualStateDuration
		{
			get
			{
				if (this.m_lastActualStateTransitionAfterSuppressionUtc == null)
				{
					return TimeSpan.Zero;
				}
				return DateTime.UtcNow.Subtract(this.m_lastActualStateTransitionAfterSuppressionUtc.Value);
			}
		}

		public bool ReportSuccessPeriodic(out TransientErrorInfo.ErrorType currentState)
		{
			currentState = this.m_suppressedCurrentState;
			if (!this.m_internalSuppressionInfo.ReportSuccess(this.m_successTransitionSuppression))
			{
				return !this.ShouldSuppress(TransientErrorInfo.ErrorType.Success);
			}
			if (this.SetCurrentSuppressedState(TransientErrorInfo.ErrorType.Success))
			{
				currentState = this.m_suppressedCurrentState;
				this.m_lastPeriodicStateReturnedUtc = new DateTime?(DateTime.UtcNow);
				return true;
			}
			return !this.ShouldSuppress(TransientErrorInfo.ErrorType.Success);
		}

		public bool ReportFailurePeriodic(out TransientErrorInfo.ErrorType currentState)
		{
			currentState = this.m_suppressedCurrentState;
			if (!this.m_internalSuppressionInfo.ReportFailure(this.m_failureTransitionSuppression))
			{
				return !this.ShouldSuppress(TransientErrorInfo.ErrorType.Failure);
			}
			if (this.SetCurrentSuppressedState(TransientErrorInfo.ErrorType.Failure))
			{
				currentState = this.m_suppressedCurrentState;
				this.m_lastPeriodicStateReturnedUtc = new DateTime?(DateTime.UtcNow);
				return true;
			}
			return !this.ShouldSuppress(TransientErrorInfo.ErrorType.Failure);
		}

		private bool ShouldSuppress(TransientErrorInfo.ErrorType intendedState)
		{
			if (this.m_suppressedCurrentState != TransientErrorInfo.ErrorType.Unknown)
			{
				intendedState = this.m_suppressedCurrentState;
			}
			TimeSpan t = this.m_successTransitionSuppression;
			TimeSpan timeSpan = this.m_successPeriodicInterval;
			if (intendedState == TransientErrorInfo.ErrorType.Failure)
			{
				t = this.m_failureTransitionSuppression;
				timeSpan = this.m_failurePeriodicInterval;
			}
			if (this.CurrentActualStateDuration >= t)
			{
				if (this.m_lastPeriodicStateReturnedUtc == null)
				{
					this.m_lastPeriodicStateReturnedUtc = new DateTime?(DateTime.UtcNow);
					return false;
				}
				if (timeSpan == TransientErrorInfoPeriodic.InfiniteTimeSpan)
				{
					return true;
				}
				TimeSpan t2 = DateTime.UtcNow.Subtract(this.m_lastPeriodicStateReturnedUtc.Value);
				if (t2 >= timeSpan)
				{
					this.m_lastPeriodicStateReturnedUtc = new DateTime?(DateTime.UtcNow);
					return false;
				}
			}
			return true;
		}

		private bool SetCurrentSuppressedState(TransientErrorInfo.ErrorType newState)
		{
			if (newState != this.m_suppressedCurrentState)
			{
				this.m_suppressedCurrentState = newState;
				this.m_lastActualStateTransitionAfterSuppressionUtc = new DateTime?(DateTime.UtcNow);
				this.m_lastPeriodicStateReturnedUtc = null;
				return true;
			}
			return false;
		}

		public static readonly TimeSpan InfiniteTimeSpan = TimeSpan.FromMilliseconds(-1.0);

		private readonly TimeSpan m_successTransitionSuppression;

		private readonly TimeSpan m_successPeriodicInterval;

		private readonly TimeSpan m_failureTransitionSuppression;

		private readonly TimeSpan m_failurePeriodicInterval;

		private DateTime? m_lastPeriodicStateReturnedUtc;

		private DateTime? m_lastActualStateTransitionAfterSuppressionUtc;

		private TransientErrorInfo.ErrorType m_suppressedCurrentState;

		private TransientErrorInfo m_internalSuppressionInfo;
	}
}
