using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class GetObjectWorkflow : MetaDataIncludeWorkflow
	{
		public GetObjectWorkflow()
		{
			base.Name = "GetObject";
		}

		protected GetObjectWorkflow(GetObjectWorkflow workflow) : base(workflow)
		{
		}

		public override Workflow Clone()
		{
			return new GetObjectWorkflow(this);
		}
	}
}
