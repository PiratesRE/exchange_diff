using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class OlcMailboxConfiguration
	{
		public override string ToString()
		{
			return string.Format("Puid=0x{0:X}, DGroup={1}", this.Puid, this.DGroup);
		}

		[DataMember]
		public long Puid;

		[DataMember]
		public int DGroup;

		[DataMember]
		public string RemoteHostName;
	}
}
