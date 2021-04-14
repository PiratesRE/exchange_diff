using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ServiceConfigurationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ServiceConfigurationResponseMessage : ResponseMessage
	{
		public ServiceConfigurationResponseMessage()
		{
		}

		internal ServiceConfigurationResponseMessage(ServiceResultCode code, ServiceError error, XmlElement[] configurationElements) : base(code, error)
		{
			this.configurationElements = configurationElements;
		}

		[XmlAnyElement]
		public XmlElement[] ServiceConfiguration
		{
			get
			{
				return this.configurationElements;
			}
			set
			{
				this.configurationElements = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetServiceConfigurationResponseMessage;
		}

		private XmlElement[] configurationElements;
	}
}
