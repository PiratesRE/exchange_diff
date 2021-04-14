using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class NameValuePair
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Value { get; set; }

		[DataMember]
		public DateTime LastWrite { get; set; }
	}
}
