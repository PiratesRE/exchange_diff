using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class WorkflowNotExistException : Exception
	{
		public WorkflowNotExistException(string workflow)
		{
			this.workflow = workflow;
		}

		public string Workflow
		{
			get
			{
				return this.workflow;
			}
		}

		private readonly string workflow;
	}
}
