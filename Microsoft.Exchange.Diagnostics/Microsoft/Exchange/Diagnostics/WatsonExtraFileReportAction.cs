using System;
using System.IO;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonExtraFileReportAction : WatsonReportAction
	{
		public WatsonExtraFileReportAction(string filename) : base(filename, false)
		{
		}

		public override string ActionName
		{
			get
			{
				return "Attached File";
			}
		}

		public override string Evaluate(WatsonReport watsonReport)
		{
			string result;
			try
			{
				watsonReport.PerformWerOperation(delegate(WerSafeHandle reportHandle)
				{
					DiagnosticsNativeMethods.WerReportAddFile(reportHandle, base.Expression, DiagnosticsNativeMethods.WER_FILE_TYPE.WerFileTypeOther, (DiagnosticsNativeMethods.WER_FILE_FLAGS)0U);
				});
				watsonReport.LogExtraFile(base.Expression);
				result = "Attached file \"" + Path.GetFileName(base.Expression) + "\" to report.";
			}
			catch (Exception ex)
			{
				watsonReport.LogExtraFile(this.FormatError(base.Expression, ex));
				result = this.FormatError(base.Expression, ex);
			}
			return result;
		}

		private string FormatError(string filename, Exception ex)
		{
			return string.Concat(new string[]
			{
				"Error attaching \"",
				base.Expression,
				"\" to report: ",
				ex.GetType().Name,
				" (",
				ex.Message,
				")"
			});
		}
	}
}
