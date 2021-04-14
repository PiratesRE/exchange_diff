using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetMailTipsRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetMailTipsRequest : BaseRequest
	{
		[DataMember(Name = "SendingAs", IsRequired = true, Order = 1)]
		[XmlElement("SendingAs")]
		public EmailAddressWrapper SendingAs
		{
			get
			{
				return this.sendingAs;
			}
			set
			{
				this.sendingAs = value;
			}
		}

		[XmlArrayItem("Mailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray("Recipients", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "Recipients", IsRequired = true, Order = 2)]
		public EmailAddressWrapper[] Recipients
		{
			get
			{
				return this.recipients;
			}
			set
			{
				this.recipients = value;
			}
		}

		[XmlElement("MailTipsRequested")]
		[DataMember(Name = "MailTipsRequested", IsRequired = true, Order = 3)]
		public MailTipTypes MailTipsRequested
		{
			get
			{
				return this.mailTipsRequested;
			}
			set
			{
				this.mailTipsRequested = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetMailTips(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		internal const string SendingAsElementName = "SendingAs";

		internal const string RecipientsElementName = "Recipients";

		internal const string MailTipsRequestedElementName = "MailTipsRequested";

		private EmailAddressWrapper sendingAs;

		private EmailAddressWrapper[] recipients;

		private MailTipTypes mailTipsRequested;
	}
}
