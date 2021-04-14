using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("IsOffice365DomainResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class IsOffice365DomainResponse : ResponseMessage
	{
		public IsOffice365DomainResponse()
		{
		}

		internal IsOffice365DomainResponse(ServiceResultCode code, ServiceError error, bool isOffice365Domain) : base(code, error)
		{
			this.IsOffice365Domain = isOffice365Domain;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.IsOffice365DomainResponseMessage;
		}

		[XmlElement("IsOffice365Domain")]
		public bool IsOffice365Domain { get; set; }
	}
}
