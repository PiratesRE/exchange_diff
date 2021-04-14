using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedMailboxRemovalResult
	{
		[DataMember]
		public Guid MailboxGuid { get; set; }

		[DataMember]
		public Guid DatabaseGuid { get; set; }

		[DataMember]
		public bool Success { get; set; }
	}
}
