using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface IReportBytes
	{
		void ReportBytesRead(int count);

		void ReportBytesWritten(int count);
	}
}
