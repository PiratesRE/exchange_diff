using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[DataContract]
	internal class DiagnosticLog
	{
		[DataMember]
		public MigrationEventType Level { get; set; }

		[DataMember]
		public string LogEntry { get; set; }

		[DataMember]
		public Exception Exception { get; set; }

		public override string ToString()
		{
			if (this.Exception == null)
			{
				return string.Format("[{0:-5,5}] {1}", this.Level, this.LogEntry);
			}
			return string.Format("[{0:-5,5}] {1} - {2}", this.Level, this.LogEntry, this.Exception);
		}
	}
}
