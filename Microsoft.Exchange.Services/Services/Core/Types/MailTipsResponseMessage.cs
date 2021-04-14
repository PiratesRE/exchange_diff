using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("MailTipsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class MailTipsResponseMessage : ResponseMessage
	{
		public MailTipsResponseMessage()
		{
		}

		internal MailTipsResponseMessage(ServiceResultCode code, ServiceError error, XmlNode mailTips) : base(code, error)
		{
			this.mailTips = mailTips;
			base.MessageText = mailTips.OuterXml;
		}

		[XmlAnyElement]
		public XmlNode MailTips
		{
			get
			{
				return this.mailTips;
			}
			set
			{
				this.mailTips = value;
				base.MessageText = this.mailTips.OuterXml;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetMailTipsResponseMessage;
		}

		private XmlNode mailTips;
	}
}
