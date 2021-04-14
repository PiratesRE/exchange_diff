using System;
using System.Collections;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class ProgressCalculatorBase
	{
		public ProgressCalculatorBase()
		{
			this.ProgressRecord = new ProgressRecord();
		}

		public void CalculateProgress(ProgressReportEventArgs e)
		{
			lock (this.ProgressRecord.SyncRoot)
			{
				this.CalculateProgressImpl(e);
			}
		}

		public abstract void CalculateProgressImpl(ProgressReportEventArgs e);

		public ProgressRecord ProgressRecord { get; private set; }

		public virtual void SetPipelineInput(IEnumerable pipelineInput)
		{
		}

		internal static int CalculatePercentageHelper(int defaultPercent, int currentIndex, int totalCount, Activity currentActivity)
		{
			double num = (double)defaultPercent / 100.0;
			if (totalCount != 0)
			{
				double num2 = 1.0 / (double)totalCount;
				num = num2 * (double)currentIndex;
				if (currentActivity != null)
				{
					num += num2 * (double)currentActivity.ProgressPercent / 100.0;
				}
			}
			return (int)Math.Floor(num * 100.0);
		}
	}
}
