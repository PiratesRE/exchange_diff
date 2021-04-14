using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class QueueDiagnosticData
	{
		[DataMember]
		public Guid QueueGuid { get; set; }

		[DataMember]
		public int QueueLength { get; set; }

		[DataMember]
		public RequestDiagnosticData CurrentRequest { get; set; }

		[DataMember]
		public IList<RequestDiagnosticData> Requests { get; set; }

		[DataMember]
		public bool IsActive { get; set; }
	}
}
