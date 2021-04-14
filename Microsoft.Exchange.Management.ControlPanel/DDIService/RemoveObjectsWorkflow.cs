using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class RemoveObjectsWorkflow : Workflow
	{
		public RemoveObjectsWorkflow()
		{
			base.Name = "RemoveObjects";
			this.Output = string.Empty;
		}

		protected RemoveObjectsWorkflow(RemoveObjectsWorkflow workflow) : base(workflow)
		{
		}

		public override Workflow Clone()
		{
			return new RemoveObjectsWorkflow(this);
		}
	}
}
