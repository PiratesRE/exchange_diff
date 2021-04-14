using System;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class ProgressUpdateEventArgs : EventArgs
	{
		public ProgressUpdateEventArgs(int completedRules, int totalRules)
		{
			this.CompletedRules = completedRules;
			this.TotalRules = totalRules;
			this.CompletedPercentage = 100;
			if (totalRules != 0)
			{
				this.CompletedPercentage = (int)((float)completedRules / (float)totalRules * 100f);
			}
		}

		public int CompletedRules { get; private set; }

		public int TotalRules { get; private set; }

		public int CompletedPercentage { get; private set; }
	}
}
