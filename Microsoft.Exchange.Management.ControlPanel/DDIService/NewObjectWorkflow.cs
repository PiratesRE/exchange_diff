using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class NewObjectWorkflow : Workflow
	{
		public NewObjectWorkflow()
		{
			base.Name = "NewObject";
		}

		protected NewObjectWorkflow(NewObjectWorkflow workflow) : base(workflow)
		{
		}

		public override Workflow Clone()
		{
			return new NewObjectWorkflow(this);
		}
	}
}
