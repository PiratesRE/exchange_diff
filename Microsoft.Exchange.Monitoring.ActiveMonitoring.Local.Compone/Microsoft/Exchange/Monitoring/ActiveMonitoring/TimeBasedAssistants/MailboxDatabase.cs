using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	[DataContract(Namespace = "")]
	internal class MailboxDatabase
	{
		[DataMember(Order = 0)]
		public Guid Guid { get; set; }

		[DataMember(Order = 1)]
		public bool IsAssistantEnabled { get; set; }

		[DataMember(Order = 2)]
		public DateTime StartTime { get; set; }
	}
}
