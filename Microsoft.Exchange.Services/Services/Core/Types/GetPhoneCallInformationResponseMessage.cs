using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetPhoneCallInformationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetPhoneCallInformationResponseMessage : ResponseMessage
	{
		public GetPhoneCallInformationResponseMessage()
		{
		}

		internal GetPhoneCallInformationResponseMessage(ServiceResultCode code, ServiceError error, GetPhoneCallInformationResponseMessage response) : base(code, error)
		{
			this.callInfo = null;
			if (response != null)
			{
				this.callInfo = response.callInfo;
			}
		}

		[XmlAnyElement]
		public XmlNode PhoneCallInformation
		{
			get
			{
				return this.callInfo;
			}
			set
			{
				this.callInfo = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetPhoneCallInformationResponseMessage;
		}

		private XmlNode callInfo;
	}
}
