using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public interface ILogWriter
	{
		void Flush();

		void Close();

		void Append(IEnumerable<LogRowFormatter> rows, int timestampField);

		void Append(LogRowFormatter row, int timestampField);

		void Append(LogRowFormatter row, int timestampField, DateTime timeStamp);
	}
}
