using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public sealed class Setting<T> : AnalysisMember<T>
	{
		public Setting(Analysis analysis, FeatureSet features) : base(analysis, features)
		{
		}
	}
}
