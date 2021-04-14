using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class PopAccountSettings : ItemPropertiesBase
	{
		[DataMember]
		public PopAccountInfo[] PopAccounts { get; set; }
	}
}
