using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Network
{
	internal class Win32DnsQueryResult<T>
	{
		internal Win32DnsQueryResult(TimeSpan duration, long resultCode, T[] records)
		{
			if (resultCode == 0L && records == null)
			{
				throw new ArgumentException("The 'records' parameter must be non-null when 'resultCode' is zero.");
			}
			this.Duration = duration;
			this.ResultCode = resultCode;
			this.Records = records;
		}

		public TimeSpan Duration { get; private set; }

		public long ResultCode { get; private set; }

		public T[] Records { get; private set; }

		public bool Success
		{
			get
			{
				return this.ResultCode == 0L;
			}
		}
	}
}
