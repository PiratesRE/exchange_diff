using System;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public enum DarTaskExecutionResult
	{
		Completed,
		Yielded,
		Failed,
		TransientError
	}
}
