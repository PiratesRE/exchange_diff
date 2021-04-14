using System;
using System.Linq;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public sealed class SingleOperationProgressCalculator : ProgressCalculatorBase
	{
		public override void CalculateProgressImpl(ProgressReportEventArgs e)
		{
			DDIHelper.Trace("In CalculateProgress: " + e.Percent);
			base.ProgressRecord.Percent = e.Percent;
			base.ProgressRecord.Status = e.Status;
			if (e.Errors != null)
			{
				base.ProgressRecord.Errors = ((base.ProgressRecord.Errors == null) ? e.Errors.ToArray<ErrorRecord>() : base.ProgressRecord.Errors.Concat(e.Errors.ToArray<ErrorRecord>() ?? new ErrorRecord[0]).ToArray<ErrorRecord>());
			}
		}
	}
}
