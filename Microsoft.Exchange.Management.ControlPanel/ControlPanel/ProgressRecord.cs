using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ProgressRecord
	{
		public ProgressRecord()
		{
		}

		public ProgressRecord(ProgressRecord progressRecord)
		{
			this.Percent = ((progressRecord.PercentComplete >= 0) ? progressRecord.PercentComplete : 100);
			this.Status = progressRecord.StatusDescription;
		}

		public object SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		[DataMember]
		public int Percent { get; set; }

		[DataMember]
		public string Status { get; set; }

		[DataMember]
		public int MaxCount { get; set; }

		[DataMember]
		public int SucceededCount { get; set; }

		[DataMember]
		public int FailedCount { get; set; }

		[DataMember]
		public ErrorRecord[] Errors { get; set; }

		[DataMember]
		public bool HasCompleted { get; set; }

		[DataMember]
		public bool IsCancelled { get; set; }

		private object syncRoot = new object();
	}
}
