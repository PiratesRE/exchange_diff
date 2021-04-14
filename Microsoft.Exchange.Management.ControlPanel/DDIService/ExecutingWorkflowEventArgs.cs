using System;

namespace Microsoft.Exchange.Management.DDIService
{
	internal class ExecutingWorkflowEventArgs : EventArgs
	{
		internal ExecutingWorkflowEventArgs(Workflow workflow)
		{
			this.ExecutingWorkflow = workflow;
		}

		internal Workflow ExecutingWorkflow { get; private set; }
	}
}
