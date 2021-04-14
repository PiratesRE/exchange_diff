using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class GetSDOWorkflow : MetaDataIncludeWorkflow
	{
		public GetSDOWorkflow()
		{
			base.Name = "GetForSDO";
			base.IncludeReadOnlyProperty = false;
			base.IncludeValidator = false;
		}

		protected GetSDOWorkflow(GetSDOWorkflow workflow) : base(workflow)
		{
		}

		public override Workflow Clone()
		{
			return new GetSDOWorkflow(this);
		}
	}
}
