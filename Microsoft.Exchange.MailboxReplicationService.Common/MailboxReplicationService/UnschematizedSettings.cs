using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class UnschematizedSettings : ItemPropertiesBase
	{
		[DataMember]
		public NameValuePair[] KvpSettings { get; set; }
	}
}
