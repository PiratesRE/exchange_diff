using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PrimaryConsistencyCheckChain : ConsistencyCheckChain<PrimaryConsistencyCheckResult>
	{
		internal PrimaryConsistencyCheckChain(int capacity, MeetingComparisonResult totalResult) : base(capacity, totalResult)
		{
			this.ShouldTerminate = false;
		}

		internal bool ShouldTerminate { get; private set; }

		internal void PerformChecks(CalendarValidationContext context)
		{
			base.PerformChecks();
			if (!this.ShouldTerminate)
			{
				MeetingExistenceCheck check = new MeetingExistenceCheck(context);
				base.PerformCheck<MeetingExistenceConsistencyCheckResult>(check);
			}
		}

		protected override bool ShouldContinue(PrimaryConsistencyCheckResult lastCheckResult)
		{
			this.ShouldTerminate = lastCheckResult.ShouldTerminate;
			return !this.ShouldTerminate;
		}
	}
}
