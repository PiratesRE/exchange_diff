using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MSInternal1 : ItemPropertiesBase
	{
		[DataMember]
		public MSInternal2[] Field1 { get; set; }

		[DataMember]
		public MSInternal4[] Field2 { get; set; }
	}
}
