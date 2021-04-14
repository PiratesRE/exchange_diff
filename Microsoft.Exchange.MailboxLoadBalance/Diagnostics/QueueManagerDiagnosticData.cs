using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class QueueManagerDiagnosticData
	{
		[DataMember]
		public IList<QueueDiagnosticData> ProcessingQueues { get; set; }

		[DataMember]
		public IList<QueueDiagnosticData> InjectionQueues { get; set; }
	}
}
