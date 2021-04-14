using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DisableAppResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class DisableAppResponse : ResponseMessage
	{
		public DisableAppResponse()
		{
		}

		internal DisableAppResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.DisableAppResponseMessage;
		}
	}
}
