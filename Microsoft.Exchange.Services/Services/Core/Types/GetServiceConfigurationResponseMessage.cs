using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetServiceConfigurationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetServiceConfigurationResponseMessage : ResponseMessage
	{
		public GetServiceConfigurationResponseMessage()
		{
		}

		internal GetServiceConfigurationResponseMessage(ServiceResultCode code, ServiceError error, ServiceConfigurationResponseMessage[] responseMessages) : base(code, error)
		{
			this.responseMessageArray = responseMessages;
		}

		[XmlArrayItem("ServiceConfigurationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", IsNullable = false)]
		public ServiceConfigurationResponseMessage[] ResponseMessages
		{
			get
			{
				return this.responseMessageArray;
			}
			set
			{
				this.responseMessageArray = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetServiceConfigurationResponseMessage;
		}

		private ServiceConfigurationResponseMessage[] responseMessageArray;
	}
}
