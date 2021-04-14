using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class AddCmdlet : CmdletActivity, IReadOnlyChecker
	{
		public AddCmdlet()
		{
		}

		protected AddCmdlet(AddCmdlet activity) : base(activity)
		{
		}

		public override Activity Clone()
		{
			return new AddCmdlet(this);
		}

		protected override string GetVerb()
		{
			return "Add-";
		}
	}
}
