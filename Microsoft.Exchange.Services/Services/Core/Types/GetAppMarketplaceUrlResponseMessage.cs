using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetAppMarketplaceUrlResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetAppMarketplaceUrlResponseMessage : ResponseMessage
	{
		public GetAppMarketplaceUrlResponseMessage()
		{
		}

		internal GetAppMarketplaceUrlResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
