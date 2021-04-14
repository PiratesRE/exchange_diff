using System;
using System.Text;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class SummaryHeader
	{
		internal SummaryHeader(string headerTitle, int length, Func<ResponseTrackerItem, string[]> valueExtractionDelegate)
		{
			this.HeaderTitle = headerTitle;
			this.Length = length;
			this.ValueExtractionDelegate = valueExtractionDelegate;
		}

		internal void Append(bool useCsvFormat, StringBuilder stringBuilder, string itemToLog)
		{
			if (itemToLog == null)
			{
				itemToLog = string.Empty;
			}
			if (!useCsvFormat)
			{
				if (itemToLog.Length > this.Length)
				{
					itemToLog = itemToLog.Substring(0, this.Length - 1) + " ";
				}
				else if (itemToLog.Length < this.Length)
				{
					itemToLog = itemToLog.PadRight(this.Length);
				}
			}
			else
			{
				if (itemToLog.Length > this.Length)
				{
					itemToLog = itemToLog.Substring(0, this.Length);
				}
				itemToLog += ",";
			}
			stringBuilder.Append(itemToLog);
		}

		internal string HeaderTitle;

		internal int Length;

		internal Func<ResponseTrackerItem, string[]> ValueExtractionDelegate;
	}
}
