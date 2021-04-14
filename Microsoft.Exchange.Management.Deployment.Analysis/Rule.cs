using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public sealed class Rule : AnalysisMember<bool>
	{
		public Rule(Analysis analysis, FeatureSet features) : base(analysis, features)
		{
		}
	}
}
