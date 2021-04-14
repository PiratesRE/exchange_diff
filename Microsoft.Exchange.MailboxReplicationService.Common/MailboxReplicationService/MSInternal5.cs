using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MSInternal5 : ItemPropertiesBase
	{
		[DataMember]
		public MSInternal6[] Field1 { get; set; }

		[DataMember]
		public string Field2 { get; set; }
	}
}
