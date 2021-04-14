using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class OlcLegacyMessageProperties
	{
		[DataMember]
		public byte AccountId { get; set; }

		[DataMember]
		public string Location { get; set; }

		[DataMember]
		public byte ContentTypeInt { get; set; }

		[DataMember]
		public bool IsFromSomeoneInAddressBook { get; set; }

		[DataMember]
		public bool IsToAddressInWhiteList { get; set; }

		[DataMember]
		public int CreateSequenceNumber { get; set; }

		[DataMember]
		public int OriginationIP { get; set; }

		[DataMember]
		public bool JunkedByBlockListMessageFilter { get; set; }

		[DataMember]
		public byte EeScanVersion { get; set; }

		[DataMember]
		public bool LinksAndImages { get; set; }

		[DataMember]
		public bool PutAddressBookBlockList { get; set; }

		[DataMember]
		public bool? SendAsAddressInRcptsList { get; set; }

		[DataMember]
		public byte? CategorizationChangedBy { get; set; }
	}
}
