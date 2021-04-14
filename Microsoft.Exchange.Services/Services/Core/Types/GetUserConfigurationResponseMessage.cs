using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetUserConfigurationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUserConfigurationResponseMessage : ResponseMessage
	{
		public GetUserConfigurationResponseMessage()
		{
		}

		internal GetUserConfigurationResponseMessage(ServiceResultCode code, ServiceError error, ServiceUserConfiguration serviceUserConfiguration) : base(code, error)
		{
			this.UserConfiguration = serviceUserConfiguration;
		}

		[XmlElement("UserConfiguration")]
		[DataMember(Name = "UserConfiguration", EmitDefaultValue = false)]
		public ServiceUserConfiguration UserConfiguration { get; set; }
	}
}
