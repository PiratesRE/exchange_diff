using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlType(Namespace = "DeltaSyncV2:", IncludeInSchema = false)]
	[Serializable]
	public enum ItemsChoiceType1
	{
		Categories,
		Flag,
		Size,
		TotalMessageCount,
		UnreadMessageCount,
		[XmlEnum("EMAIL::DateReceived")]
		DateReceived,
		[XmlEnum("EMAIL::From")]
		From,
		[XmlEnum("EMAIL::Importance")]
		Importance,
		[XmlEnum("EMAIL::MessageClass")]
		MessageClass,
		[XmlEnum("EMAIL::Read")]
		Read,
		[XmlEnum("EMAIL::Subject")]
		Subject,
		[XmlEnum("HMFOLDER::DisplayName")]
		DisplayName,
		[XmlEnum("HMFOLDER::ParentId")]
		ParentId,
		[XmlEnum("HMFOLDER::Version")]
		Version,
		[XmlEnum("HMMAIL::ConfirmedJunk")]
		ConfirmedJunk,
		[XmlEnum("HMMAIL::ConversationIndex")]
		ConversationIndex,
		[XmlEnum("HMMAIL::ConversationTopic")]
		ConversationTopic,
		[XmlEnum("HMMAIL::FolderId")]
		FolderId,
		[XmlEnum("HMMAIL::HasAttachments")]
		HasAttachments,
		[XmlEnum("HMMAIL::IsBondedSender")]
		IsBondedSender,
		[XmlEnum("HMMAIL::IsFromSomeoneAddressBook")]
		IsFromSomeoneAddressBook,
		[XmlEnum("HMMAIL::IsToAllowList")]
		IsToAllowList,
		[XmlEnum("HMMAIL::LegacyId")]
		LegacyId,
		[XmlEnum("HMMAIL::Message")]
		Message,
		[XmlEnum("HMMAIL::PopAccountID")]
		PopAccountID,
		[XmlEnum("HMMAIL::ReplyToOrForwardState")]
		ReplyToOrForwardState,
		[XmlEnum("HMMAIL::Sensitivity")]
		Sensitivity,
		[XmlEnum("HMMAIL::Size")]
		Size1,
		[XmlEnum("HMMAIL::TrustedSource")]
		TrustedSource,
		[XmlEnum("HMMAIL::TypeData")]
		TypeData,
		[XmlEnum("HMMAIL::Version")]
		Version1
	}
}
