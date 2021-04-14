using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class TransientPeriodicErrorSuppression<TKey>
	{
		protected TransientPeriodicErrorSuppression(TimeSpan successTransitionSuppression, TimeSpan successPeriodicInterval, TimeSpan failureTransitionSuppression, TimeSpan failurePeriodicInterval, TransientErrorInfo.ErrorType initialState)
		{
			this.m_successTransitionSuppression = successTransitionSuppression;
			this.m_successPeriodicInterval = successPeriodicInterval;
			this.m_failureTransitionSuppression = failureTransitionSuppression;
			this.m_failurePeriodicInterval = failurePeriodicInterval;
			this.m_initialState = initialState;
			this.InitializeTable();
		}

		public bool ReportSuccessPeriodic(TKey key, out TransientErrorInfo.ErrorType currentState)
		{
			TransientErrorInfoPeriodic existingOrNewErrorInfo = this.GetExistingOrNewErrorInfo(key);
			return existingOrNewErrorInfo.ReportSuccessPeriodic(out currentState);
		}

		public bool ReportFailurePeriodic(TKey key, out TransientErrorInfo.ErrorType currentState)
		{
			TransientErrorInfoPeriodic existingOrNewErrorInfo = this.GetExistingOrNewErrorInfo(key);
			return existingOrNewErrorInfo.ReportFailurePeriodic(out currentState);
		}

		protected abstract void InitializeTable();

		private TransientErrorInfoPeriodic GetExistingOrNewErrorInfo(TKey key)
		{
			TransientErrorInfoPeriodic transientErrorInfoPeriodic = null;
			if (!this.m_errorTable.TryGetValue(key, out transientErrorInfoPeriodic))
			{
				transientErrorInfoPeriodic = new TransientErrorInfoPeriodic(this.m_successTransitionSuppression, this.m_successPeriodicInterval, this.m_failureTransitionSuppression, this.m_failurePeriodicInterval, this.m_initialState);
				this.m_errorTable[key] = transientErrorInfoPeriodic;
			}
			return transientErrorInfoPeriodic;
		}

		private readonly TimeSpan m_successTransitionSuppression;

		private readonly TimeSpan m_successPeriodicInterval;

		private readonly TimeSpan m_failureTransitionSuppression;

		private readonly TimeSpan m_failurePeriodicInterval;

		private readonly TransientErrorInfo.ErrorType m_initialState;

		protected Dictionary<TKey, TransientErrorInfoPeriodic> m_errorTable;
	}
}
