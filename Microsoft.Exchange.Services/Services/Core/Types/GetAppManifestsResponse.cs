using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetAppManifestsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetAppManifestsResponse : ResponseMessage
	{
		public GetAppManifestsResponse()
		{
		}

		internal GetAppManifestsResponse(ServiceResultCode code, ServiceError error, XmlElement manifests) : base(code, error)
		{
			this.Manifests = manifests;
		}

		[XmlAnyElement]
		public XmlNode Manifests { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetAppManifestsResponseMessage;
		}
	}
}
