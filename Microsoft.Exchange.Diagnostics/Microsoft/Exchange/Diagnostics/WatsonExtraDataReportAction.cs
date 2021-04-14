using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonExtraDataReportAction : WatsonReportAction
	{
		public WatsonExtraDataReportAction(string text) : base(text, true)
		{
		}

		public override string ActionName
		{
			get
			{
				return "Extra Data";
			}
		}

		public override string Evaluate(WatsonReport watsonReport)
		{
			watsonReport.LogExtraData(base.Expression);
			return base.Expression;
		}
	}
}
