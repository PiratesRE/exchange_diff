using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.Analysis.Builders;
using Microsoft.Exchange.Management.Analysis.Features;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal sealed class Rule : AnalysisMember<bool>
	{
		private Rule(Func<AnalysisMember> parent, ConcurrencyType runAs, Analysis analysis, IEnumerable<Feature> features, Func<Result, IEnumerable<Result<bool>>> setFunction) : base(parent, runAs, analysis, features, setFunction)
		{
		}

		public static RuleParentBuilder Build()
		{
			RuleBuildContext context = new RuleBuildContext((RuleBuildContext x) => new Rule(x.Parent, x.RunAs, x.Analysis, x.Features, x.SetFunction));
			return new RuleParentBuilder(context);
		}

		public string GetHelpId()
		{
			HelpId helpId;
			if (!Enum.TryParse<HelpId>(base.Name, out helpId))
			{
				throw new ArgumentException(Strings.HelpIdNotDefined(base.Name));
			}
			return helpId.ToString();
		}
	}
}
