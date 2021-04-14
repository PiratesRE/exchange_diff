using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[DataContract]
	internal struct TraceDecoratedResult
	{
		[DataMember]
		public object Result { get; set; }

		[DataMember]
		public IList<DiagnosticLog> Logs { get; set; }
	}
}
