using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.CommonHandlers
{
	public class ProcessInfo
	{
		public string ServerName { get; set; }

		public int ProcessID { get; set; }

		public int ThreadCount { get; set; }

		public string Version { get; set; }

		public double ProcessUpTime { get; set; }

		public long MemorySize { get; set; }
	}
}
