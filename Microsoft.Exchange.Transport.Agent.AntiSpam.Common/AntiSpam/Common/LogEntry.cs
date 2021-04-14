using System;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	internal class LogEntry
	{
		public LogEntry(string reason, string reasonData, string diagnostics)
		{
			this.reason = reason;
			this.reasonData = reasonData;
			this.diagnostics = diagnostics;
		}

		public LogEntry(string reason, string reasonData) : this(reason, reasonData, null)
		{
		}

		public LogEntry(object reason, object reasonData) : this((reason != null) ? reason.ToString() : null, (reasonData != null) ? reasonData.ToString() : null)
		{
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		public string ReasonData
		{
			get
			{
				return this.reasonData;
			}
		}

		public string Diagnostics
		{
			get
			{
				return this.diagnostics;
			}
		}

		private readonly string reason;

		private readonly string reasonData;

		private readonly string diagnostics;
	}
}
