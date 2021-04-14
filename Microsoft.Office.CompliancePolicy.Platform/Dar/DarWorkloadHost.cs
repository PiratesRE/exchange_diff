using System;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public abstract class DarWorkloadHost
	{
		public virtual DarTaskExecutionCommand ShouldContinue(DarTask task, out string additionalInformation)
		{
			additionalInformation = null;
			return DarTaskExecutionCommand.ContinueExecution;
		}
	}
}
