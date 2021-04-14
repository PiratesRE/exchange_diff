using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DisableCmdlet : PipelineCmdlet, IReadOnlyChecker
	{
		public DisableCmdlet()
		{
		}

		protected DisableCmdlet(DisableCmdlet activity) : base(activity)
		{
		}

		public override Activity Clone()
		{
			return new DisableCmdlet(this);
		}

		protected override string GetVerb()
		{
			return "Disable-";
		}
	}
}
