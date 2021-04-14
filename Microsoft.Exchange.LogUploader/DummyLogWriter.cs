using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	public class DummyLogWriter : ILogWriter
	{
		public void Flush()
		{
		}

		public void Close()
		{
		}

		public void Append(IEnumerable<LogRowFormatter> rows, int timestampField)
		{
		}

		public void Append(LogRowFormatter row, int timestampField)
		{
		}

		public void Append(LogRowFormatter row, int timestampField, DateTime timeStamp)
		{
		}
	}
}
