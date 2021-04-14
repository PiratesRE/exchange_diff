using System;
using System.Collections;
using System.Linq;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public sealed class BulkEditProgressCalculator : ProgressCalculatorBase
	{
		public override void CalculateProgressImpl(ProgressReportEventArgs e)
		{
			if (e.Errors == null || e.Errors.Count == 0)
			{
				base.ProgressRecord.SucceededCount++;
			}
			else
			{
				base.ProgressRecord.FailedCount++;
				base.ProgressRecord.Errors = ((base.ProgressRecord.Errors == null) ? e.Errors.ToArray<ErrorRecord>() : base.ProgressRecord.Errors.Concat(e.Errors.ToArray<ErrorRecord>() ?? new ErrorRecord[0]).ToArray<ErrorRecord>());
			}
			base.ProgressRecord.Percent = (base.ProgressRecord.SucceededCount + base.ProgressRecord.FailedCount) * 100 / base.ProgressRecord.MaxCount;
		}

		public override void SetPipelineInput(IEnumerable pipelineInput)
		{
			if (pipelineInput != null)
			{
				base.ProgressRecord.MaxCount += pipelineInput.OfType<object>().Count<object>();
			}
		}
	}
}
