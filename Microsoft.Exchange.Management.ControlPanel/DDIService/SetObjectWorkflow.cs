using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class SetObjectWorkflow : Workflow, ICallGetAfterExecuteWorkflow
	{
		public SetObjectWorkflow()
		{
			base.Name = "SetObject";
		}

		protected SetObjectWorkflow(SetObjectWorkflow workflow) : base(workflow)
		{
		}

		public override Workflow Clone()
		{
			return new SetObjectWorkflow(this);
		}

		public bool IgnoreGetObject { get; set; }
	}
}
