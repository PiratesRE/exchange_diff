using System;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal interface IReportProgress
	{
		void ReportProgress(int workProcessed, int totalWork, string statusText, string errorText);
	}
}
