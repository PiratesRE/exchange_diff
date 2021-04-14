using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MailTips
	{
		public EmailAddressType RecipientAddress;

		public MailTipTypes PendingMailTips;

		public OutOfOfficeMailTip OutOfOffice;

		public bool MailboxFull;

		[XmlIgnore]
		public bool MailboxFullSpecified;

		public string CustomMailTip;

		public int TotalMemberCount;

		[XmlIgnore]
		public bool TotalMemberCountSpecified;

		public int ExternalMemberCount;

		[XmlIgnore]
		public bool ExternalMemberCountSpecified;

		public int MaxMessageSize;

		[XmlIgnore]
		public bool MaxMessageSizeSpecified;

		public bool DeliveryRestricted;

		[XmlIgnore]
		public bool DeliveryRestrictedSpecified;

		public bool IsModerated;

		[XmlIgnore]
		public bool IsModeratedSpecified;

		public bool InvalidRecipient;

		[XmlIgnore]
		public bool InvalidRecipientSpecified;

		public int Scope;

		[XmlIgnore]
		public bool ScopeSpecified;
	}
}
