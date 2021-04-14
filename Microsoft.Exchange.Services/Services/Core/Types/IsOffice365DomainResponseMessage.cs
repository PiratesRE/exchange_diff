using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("IsOffice365DomainResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class IsOffice365DomainResponseMessage : ResponseMessage
	{
		public IsOffice365DomainResponseMessage()
		{
		}

		internal IsOffice365DomainResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
