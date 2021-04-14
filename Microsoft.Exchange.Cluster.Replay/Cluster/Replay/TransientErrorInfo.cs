using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Replay.Monitoring;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TransientErrorInfo
	{
		public static TransientErrorInfo ConstructFromPersisted(TransientErrorInfoPersisted errorInfo)
		{
			TransientErrorInfo transientErrorInfo = new TransientErrorInfo();
			transientErrorInfo.m_currentErrorState = StateTransitionInfo.ConvertErrorTypeFromSerializable(errorInfo.CurrentErrorState);
			transientErrorInfo.m_lastErrorState = transientErrorInfo.m_currentErrorState;
			DateTimeHelper.ParseIntoDateTimeIfPossible(errorInfo.LastSuccessTransitionUtc, ref transientErrorInfo.m_lastSuccessTransitionUtc);
			DateTimeHelper.ParseIntoDateTimeIfPossible(errorInfo.LastFailureTransitionUtc, ref transientErrorInfo.m_lastFailureTransitionUtc);
			return transientErrorInfo;
		}

		public TransientErrorInfo.ErrorType CurrentErrorState
		{
			get
			{
				return this.m_currentErrorState;
			}
		}

		public TimeSpan SuccessDuration
		{
			get
			{
				if (this.m_currentErrorState != TransientErrorInfo.ErrorType.Success)
				{
					return TimeSpan.Zero;
				}
				return DateTime.UtcNow.Subtract(this.m_lastSuccessTransitionUtc);
			}
		}

		public TimeSpan FailedDuration
		{
			get
			{
				if (this.m_currentErrorState != TransientErrorInfo.ErrorType.Failure)
				{
					return TimeSpan.Zero;
				}
				return DateTime.UtcNow.Subtract(this.m_lastFailureTransitionUtc);
			}
		}

		public DateTime LastSuccessTransitionUtc
		{
			get
			{
				return this.m_lastSuccessTransitionUtc;
			}
		}

		public DateTime LastFailureTransitionUtc
		{
			get
			{
				return this.m_lastFailureTransitionUtc;
			}
		}

		public void ReportSuccess()
		{
			this.m_numSuccessiveFailures = 0U;
			this.m_numSuccessivePasses += 1U;
			this.UpdateErrorState(TransientErrorInfo.ErrorType.Success);
			this.UpdateTransitionTimesIfNecessary();
		}

		public bool ReportSuccess(TimeSpan suppressDuration)
		{
			this.ReportSuccess();
			return suppressDuration.Equals(TimeSpan.Zero) || this.SuccessDuration >= suppressDuration;
		}

		public bool ReportSuccess(int numSuccessivePasses)
		{
			this.ReportSuccess();
			return (ulong)this.m_numSuccessivePasses >= (ulong)((long)numSuccessivePasses);
		}

		public void ReportFailure()
		{
			this.m_numSuccessivePasses = 0U;
			this.m_numSuccessiveFailures += 1U;
			this.UpdateErrorState(TransientErrorInfo.ErrorType.Failure);
			this.UpdateTransitionTimesIfNecessary();
		}

		public bool ReportFailure(TimeSpan suppressDuration)
		{
			this.ReportFailure();
			return suppressDuration.Equals(TimeSpan.Zero) || this.FailedDuration >= suppressDuration;
		}

		public bool ReportFailure(int numSuccessiveFailures)
		{
			this.ReportFailure();
			return (ulong)this.m_numSuccessiveFailures >= (ulong)((long)numSuccessiveFailures);
		}

		private bool IsTransitioningState
		{
			get
			{
				return this.m_lastErrorState != this.m_currentErrorState;
			}
		}

		private void UpdateErrorState(TransientErrorInfo.ErrorType errorState)
		{
			this.m_lastErrorState = this.m_currentErrorState;
			this.m_currentErrorState = errorState;
		}

		private void UpdateTransitionTimesIfNecessary()
		{
			if (this.IsTransitioningState)
			{
				DateTime utcNow = DateTime.UtcNow;
				if (this.m_currentErrorState == TransientErrorInfo.ErrorType.Success)
				{
					this.m_lastSuccessTransitionUtc = utcNow;
					return;
				}
				if (this.m_currentErrorState == TransientErrorInfo.ErrorType.Failure)
				{
					this.m_lastFailureTransitionUtc = utcNow;
				}
			}
		}

		private uint m_numSuccessiveFailures;

		private uint m_numSuccessivePasses;

		private DateTime m_lastFailureTransitionUtc;

		private DateTime m_lastSuccessTransitionUtc;

		private TransientErrorInfo.ErrorType m_lastErrorState;

		private TransientErrorInfo.ErrorType m_currentErrorState;

		internal enum ErrorType : short
		{
			Unknown,
			Success,
			Failure
		}
	}
}
