using System;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.LogAnalyzer.Extensions.OABDownloadLog
{
	public class LogOABDownloadSchema : LogCsvSchema
	{
		public override bool IsHeader(LogSourceLine line)
		{
			return line.Text.StartsWith("DateTime", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
