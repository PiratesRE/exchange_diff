using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class RemoveCmdlet : PipelineCmdlet, IReadOnlyChecker
	{
		public RemoveCmdlet()
		{
		}

		protected RemoveCmdlet(RemoveCmdlet activity) : base(activity)
		{
		}

		public override Activity Clone()
		{
			return new RemoveCmdlet(this);
		}

		protected override string GetVerb()
		{
			return "Remove-";
		}
	}
}
