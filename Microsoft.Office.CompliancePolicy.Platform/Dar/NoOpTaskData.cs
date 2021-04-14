using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	[DataContract]
	public class NoOpTaskData
	{
		[DataMember]
		public List<DarTaskExecutionResult> States { get; set; }

		[DataMember]
		public List<DarTaskState> StateHistory { get; set; }
	}
}
