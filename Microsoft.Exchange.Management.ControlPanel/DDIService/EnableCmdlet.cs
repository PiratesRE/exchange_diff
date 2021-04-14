using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class EnableCmdlet : PipelineCmdlet, IReadOnlyChecker
	{
		public EnableCmdlet()
		{
		}

		protected EnableCmdlet(EnableCmdlet activity) : base(activity)
		{
		}

		public override Activity Clone()
		{
			return new EnableCmdlet(this);
		}

		protected override string GetVerb()
		{
			return "Enable-";
		}
	}
}
