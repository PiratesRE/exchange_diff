using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class UXSettings : ItemPropertiesBase
	{
		[DataMember]
		public ushort MessagesPerPage { get; set; }

		[DataMember]
		public ushort Wrap { get; set; }

		[DataMember]
		public byte HeadersByte { get; set; }

		[DataMember]
		public byte ReplySeparatorByte { get; set; }

		[DataMember]
		public bool HasReplyText { get; set; }

		[DataMember]
		public bool HasUserDefinedReplyTo { get; set; }

		[DataMember]
		public string ReplyAddress { get; set; }

		[DataMember]
		public bool ConfirmPage { get; set; }

		[DataMember]
		public int InboxSortFlagsInt { get; set; }

		[DataMember]
		public bool MobileUser { get; set; }

		[DataMember]
		public byte MobilePageAllByte { get; set; }

		[DataMember]
		public byte DefaultViewTypeByte { get; set; }

		[DataMember]
		public bool CalendarTentativeBookingEnabled { get; set; }

		[DataMember]
		public string EmailComposeSignatureHtml { get; set; }

		[DataMember]
		public CategorySettings[] CategorySettingsList { get; set; }
	}
}
