using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class RequestDiagnosticData
	{
		[DataMember]
		public string BatchName { get; set; }

		[DataMember]
		public Exception Exception { get; set; }

		public TimeSpan ExecutionDuration
		{
			get
			{
				if (this.executionDuration != null)
				{
					return this.executionDuration.Value;
				}
				if (this.ExecutionStartedTimestamp == null)
				{
					return TimeSpan.Zero;
				}
				DateTime d = this.ExecutionFinishedTimestamp ?? DateTime.UtcNow;
				return d - this.ExecutionStartedTimestamp.Value;
			}
			set
			{
				this.executionDuration = new TimeSpan?(value);
			}
		}

		[DataMember]
		public DateTime? ExecutionFinishedTimestamp { get; set; }

		[DataMember]
		public DateTime? ExecutionStartedTimestamp { get; set; }

		[DataMember]
		public Guid MovedMailboxGuid { get; set; }

		[DataMember]
		public Guid Queue { get; set; }

		public TimeSpan QueueDuration
		{
			get
			{
				if (this.QueuedTimestamp == null)
				{
					return TimeSpan.Zero;
				}
				DateTime d = this.ExecutionStartedTimestamp ?? DateTime.UtcNow;
				return d - this.QueuedTimestamp.Value;
			}
		}

		[DataMember]
		public DateTime? QueuedTimestamp { get; set; }

		[DataMember]
		public string RequestKind { get; set; }

		public string Result
		{
			get
			{
				if (this.Exception != null)
				{
					return "Failed";
				}
				return "Success";
			}
		}

		[DataMember]
		public string SourceDatabaseName { get; set; }

		[DataMember]
		public string TargetDatabaseName { get; set; }

		[DataMember]
		private TimeSpan? executionDuration;
	}
}
