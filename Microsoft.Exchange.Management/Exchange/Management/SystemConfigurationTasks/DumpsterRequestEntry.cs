using System;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class DumpsterRequestEntry
	{
		internal DumpsterRequestEntry(string server, DateTime startTime, DateTime endTime)
		{
			this.m_server = server;
			this.m_startTime = startTime;
			this.m_endTime = endTime;
		}

		public string Server
		{
			get
			{
				return this.m_server;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.m_startTime;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return this.m_endTime;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1};{2})", this.m_server, this.m_startTime, this.m_endTime);
		}

		private const string ToStringFormatStr = "{0}({1};{2})";

		private readonly string m_server;

		private readonly DateTime m_startTime;

		private readonly DateTime m_endTime;
	}
}
