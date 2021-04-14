using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class ContainerMailboxInformation
	{
		[DataMember(IsRequired = true)]
		public Guid MailboxGuid { get; set; }

		[DataMember(IsRequired = true)]
		public byte[] PersistableTenantPartitionHint { get; set; }
	}
}
