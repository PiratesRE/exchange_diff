using System;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public enum DarTaskState
	{
		None,
		Ready,
		Running,
		Completed,
		Failed,
		Cancelled
	}
}
