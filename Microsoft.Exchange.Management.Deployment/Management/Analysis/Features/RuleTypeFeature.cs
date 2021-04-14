using System;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	internal class RuleTypeFeature : Feature
	{
		public RuleTypeFeature(RuleType ruleType) : base(false, false)
		{
			this.RuleType = ruleType;
		}

		public RuleType RuleType { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.RuleType.ToString());
		}
	}
}
