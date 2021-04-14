using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class GetObjectForListWorkflow : MetaDataIncludeWorkflow
	{
		public GetObjectForListWorkflow()
		{
			base.Name = "GetObjectForList";
			base.IncludeReadOnlyProperty = false;
			base.IncludeNotAccessProperty = false;
			base.IncludeValidator = false;
		}

		protected GetObjectForListWorkflow(GetObjectForListWorkflow workflow) : base(workflow)
		{
		}

		public override Workflow Clone()
		{
			return new GetObjectForListWorkflow(this);
		}
	}
}
