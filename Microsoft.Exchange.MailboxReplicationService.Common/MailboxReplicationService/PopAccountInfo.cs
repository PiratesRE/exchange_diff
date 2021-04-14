using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class PopAccountInfo
	{
		[DataMember]
		public uint AccountId { get; set; }

		[DataMember]
		public byte TypeByte { get; set; }

		[DataMember]
		public ushort AccountTypeUShort { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public bool DownloadNewMessagesOnly { get; set; }

		[DataMember]
		public bool LeaveMessagesOnServer { get; set; }

		[DataMember]
		public int NewMailIndicator { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public string ServerName { get; set; }

		[DataMember]
		public uint ServerPort { get; set; }

		[DataMember]
		public ushort ServerTimeout { get; set; }

		[DataMember]
		public DateTime LastWrite { get; set; }
	}
}
