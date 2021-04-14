using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal class MarkInstancesAsExceptionsCommand : ErrorRecoverySeriesCommand
	{
		public MarkInstancesAsExceptionsCommand(Guid origianlActionId) : base(origianlActionId)
		{
		}

		protected override void PrepareRecoveryData(Event instanceDelta)
		{
			base.PrepareRecoveryData(instanceDelta);
			((IEventInternal)instanceDelta).MarkAllPropagatedPropertiesAsException = true;
		}
	}
}
