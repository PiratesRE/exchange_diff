using System;
using Microsoft.Exchange.Management.Deployment.Analysis;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal class PrereqConclusionSetBuilder : ConclusionSetBuilderImplementation<PrereqConclusionSet, PrereqConclusion, PrereqSettingConclusion, PrereqRuleConclusion>
	{
		protected override PrereqConclusionSet BuildConclusionSet(Analysis analysis, PrereqConclusion rootConclusion)
		{
			PrereqConclusionSet prereqConclusionSet = new PrereqConclusionSet((PrereqAnalysis)analysis, rootConclusion);
			prereqConclusionSet.MakeReadOnly();
			return prereqConclusionSet;
		}

		protected override PrereqSettingConclusion BuildSettingConclusion(Result result)
		{
			return new PrereqSettingConclusion(result);
		}

		protected override PrereqRuleConclusion BuildRuleConclusion(RuleResult ruleResult)
		{
			return new PrereqRuleConclusion(ruleResult);
		}
	}
}
