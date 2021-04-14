using System;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol
{
	[DataContract]
	public class DarTaskParams : DarTaskParamsBase
	{
		[DataMember]
		public DarTaskState TaskState { get; set; }

		[DataMember]
		public DateTime MinQueuedTime { get; set; }

		[DataMember]
		public DateTime MaxQueuedTime { get; set; }

		[DataMember]
		public DateTime MinCompletionTime { get; set; }

		[DataMember]
		public DateTime MaxCompletionTime { get; set; }

		[DataMember]
		public bool ActiveInRuntime { get; set; }

		[DataMember]
		public int Priority { get; set; }
	}
}
