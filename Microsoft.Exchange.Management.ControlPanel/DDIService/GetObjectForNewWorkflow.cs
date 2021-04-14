using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class GetObjectForNewWorkflow : MetaDataIncludeWorkflow
	{
		public GetObjectForNewWorkflow()
		{
			base.Name = "GetObjectForNew";
		}

		protected GetObjectForNewWorkflow(GetObjectForNewWorkflow workflow) : base(workflow)
		{
		}

		public override Workflow Clone()
		{
			return new GetObjectForNewWorkflow(this);
		}
	}
}
