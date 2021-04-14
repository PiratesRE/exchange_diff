using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetMailTipsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetMailTipsResponseMessage : ResponseMessage
	{
		public GetMailTipsResponseMessage()
		{
		}

		internal GetMailTipsResponseMessage(ServiceResultCode code, ServiceError error, MailTipsResponseMessage[] mailTips) : base(code, error)
		{
			this.mailTips = mailTips;
		}

		[XmlArrayItem("MailTipsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", IsNullable = false)]
		[DataMember(Name = "ResponseMessages", IsRequired = true, Order = 1)]
		public MailTipsResponseMessage[] ResponseMessages
		{
			get
			{
				return this.mailTips;
			}
			set
			{
				this.mailTips = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetMailTipsResponseMessage;
		}

		private MailTipsResponseMessage[] mailTips;
	}
}
