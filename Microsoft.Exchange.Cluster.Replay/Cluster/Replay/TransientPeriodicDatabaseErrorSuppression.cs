using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TransientPeriodicDatabaseErrorSuppression : TransientPeriodicErrorSuppression<Guid>
	{
		public TransientPeriodicDatabaseErrorSuppression(TimeSpan successTransitionSuppression, TimeSpan successPeriodicInterval, TimeSpan failureTransitionSuppression, TimeSpan failurePeriodicInterval, TransientErrorInfo.ErrorType initialState) : base(successTransitionSuppression, successPeriodicInterval, failureTransitionSuppression, failurePeriodicInterval, initialState)
		{
		}

		protected override void InitializeTable()
		{
			this.m_errorTable = new Dictionary<Guid, TransientErrorInfoPeriodic>(48);
		}
	}
}
