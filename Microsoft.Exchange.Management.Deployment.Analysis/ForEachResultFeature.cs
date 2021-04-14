using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	internal sealed class ForEachResultFeature : Feature
	{
		public ForEachResultFeature(Func<AnalysisMember> forEachResult)
		{
			this.forEachResultFunc = forEachResult;
		}

		public Func<AnalysisMember> ForEachResultFunc
		{
			get
			{
				return this.forEachResultFunc;
			}
		}

		public AnalysisMember AnalysisMember
		{
			get
			{
				return this.forEachResultFunc();
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.AnalysisMember.Name);
		}

		private readonly Func<AnalysisMember> forEachResultFunc;
	}
}
