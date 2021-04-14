using System;
using Microsoft.Exchange.Management.Deployment.Analysis;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal abstract class PrereqConclusion : ConclusionImplementation<PrereqConclusion, PrereqSettingConclusion, PrereqRuleConclusion>
	{
		public PrereqConclusion()
		{
		}

		public PrereqConclusion(Result result) : base(result)
		{
		}
	}
}
