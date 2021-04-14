using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetAppMarketplaceUrlResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetAppMarketplaceUrlResponse : ResponseMessage
	{
		public GetAppMarketplaceUrlResponse()
		{
		}

		internal GetAppMarketplaceUrlResponse(ServiceResultCode code, ServiceError error, string appMarketplaceUrl) : base(code, error)
		{
			this.AppMarketplaceUrl = appMarketplaceUrl;
		}

		[IgnoreDataMember]
		public string AppMarketplaceUrl { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetAppMarketplaceUrlResponseMessage;
		}
	}
}
