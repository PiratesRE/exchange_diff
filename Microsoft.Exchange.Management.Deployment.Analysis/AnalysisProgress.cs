using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public sealed class AnalysisProgress
	{
		public AnalysisProgress(int totalConclusions, int completedConclusions)
		{
			this.totalConclusions = totalConclusions;
			this.completedConclusions = completedConclusions;
		}

		public int TotalConclusions
		{
			get
			{
				return this.totalConclusions;
			}
		}

		public int CompletedConclusions
		{
			get
			{
				return this.completedConclusions;
			}
		}

		public int PercentageComplete
		{
			get
			{
				if (this.totalConclusions == 0)
				{
					return 0;
				}
				return this.completedConclusions * 100 / this.totalConclusions;
			}
		}

		internal static AnalysisProgress Resolve(AnalysisProgress originalValue, AnalysisProgress currentValue, AnalysisProgress updatedValue)
		{
			return new AnalysisProgress(Math.Max(updatedValue.TotalConclusions, currentValue.TotalConclusions), Math.Max(updatedValue.CompletedConclusions, currentValue.CompletedConclusions));
		}

		private readonly int totalConclusions;

		private readonly int completedConclusions;
	}
}
