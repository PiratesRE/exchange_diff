using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal interface IAsyncLogWrapper
	{
		void Append(LogRowFormatter row, int timestampField);

		void Configure(string logDirectory, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval);
	}
}
