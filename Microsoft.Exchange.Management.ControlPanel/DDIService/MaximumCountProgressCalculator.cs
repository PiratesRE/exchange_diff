using System;
using System.Linq;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public sealed class MaximumCountProgressCalculator : ProgressCalculatorBase
	{
		public override void CalculateProgressImpl(ProgressReportEventArgs e)
		{
			this.totalReceivedEvents++;
			if (e.Errors != null && e.Errors.Count > 0)
			{
				base.ProgressRecord.FailedCount = 1;
				base.ProgressRecord.Errors = ((base.ProgressRecord.Errors == null) ? e.Errors.ToArray<ErrorRecord>() : base.ProgressRecord.Errors.Concat(e.Errors.ToArray<ErrorRecord>() ?? new ErrorRecord[0]).ToArray<ErrorRecord>());
			}
			if (this.totalReceivedEvents <= base.ProgressRecord.MaxCount)
			{
				base.ProgressRecord.Percent = this.totalReceivedEvents * 100 / base.ProgressRecord.MaxCount;
			}
		}

		private int totalReceivedEvents;
	}
}
