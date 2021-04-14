using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CDRPipelineStage : CreateUnProtectedMessageStage
	{
		internal override TimeSpan ExpectedRunTime
		{
			get
			{
				return TimeSpan.FromMinutes(10.0);
			}
		}

		protected override StageRetryDetails InternalGetRetrySchedule()
		{
			return new StageRetryDetails(StageRetryDetails.FinalAction.DropMessage, TimeSpan.FromMinutes(2.0), TimeSpan.FromDays(1.0));
		}
	}
}
