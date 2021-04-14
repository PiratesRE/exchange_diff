using System;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal class NoOpSeriesRecoveryCommand : ErrorRecoverySeriesCommand
	{
		public NoOpSeriesRecoveryCommand(Guid originalActionId) : base(originalActionId)
		{
		}
	}
}
