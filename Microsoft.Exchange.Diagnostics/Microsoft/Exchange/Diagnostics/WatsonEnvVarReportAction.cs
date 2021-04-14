using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonEnvVarReportAction : WatsonReportAction
	{
		public WatsonEnvVarReportAction(string varName) : base(varName, false)
		{
		}

		public override string ActionName
		{
			get
			{
				return "Environment Variable";
			}
		}

		public override string Evaluate(WatsonReport watsonReport)
		{
			string text = base.Expression + "=" + Environment.GetEnvironmentVariable(base.Expression);
			watsonReport.LogExtraData(text);
			return text;
		}
	}
}
