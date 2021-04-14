using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "MailTipTypes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Flags]
	[Serializable]
	public enum MailTipTypes
	{
		All = 1,
		OutOfOfficeMessage = 2,
		MailboxFullStatus = 4,
		CustomMailTip = 8,
		ExternalMemberCount = 16,
		TotalMemberCount = 32,
		MaxMessageSize = 64,
		DeliveryRestriction = 128,
		ModerationStatus = 256,
		InvalidRecipient = 512,
		Scope = 1024
	}
}
