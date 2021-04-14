using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class NewCmdlet : OutputObjectCmdlet, IReadOnlyChecker
	{
		public NewCmdlet()
		{
			base.IdentityVariable = string.Empty;
		}

		protected NewCmdlet(NewCmdlet activity) : base(activity)
		{
		}

		public override Activity Clone()
		{
			return new NewCmdlet(this);
		}

		protected override string GetVerb()
		{
			return "New-";
		}
	}
}
