using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public sealed class ProgressUpdateEventArgs : EventArgs
	{
		public ProgressUpdateEventArgs(AnalysisProgress progress)
		{
			this.progress = progress;
		}

		public AnalysisProgress Progress
		{
			get
			{
				return this.progress;
			}
		}

		private readonly AnalysisProgress progress;
	}
}
