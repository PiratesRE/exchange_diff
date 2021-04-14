using System;
using Microsoft.Exchange.CommonHelpProvider;

namespace Microsoft.Exchange.Management.Analysis
{
	internal sealed class RuleResult : Result<bool>
	{
		static RuleResult()
		{
			HelpProvider.Initialize(HelpProvider.HelpAppName.Setup);
		}

		public RuleResult(bool value) : base(value)
		{
		}

		public RuleResult(Exception exception) : base(exception)
		{
		}

		public string GetHelpUrl()
		{
			return HelpProvider.ConstructHelpRenderingUrlWithQualifierHelpId("ms.exch.setupreadiness.", ((Rule)base.Source).GetHelpId()).ToString();
		}
	}
}
