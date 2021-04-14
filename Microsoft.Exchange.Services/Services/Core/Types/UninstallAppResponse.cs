using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UninstallAppResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UninstallAppResponse : ResponseMessage
	{
		public UninstallAppResponse()
		{
		}

		internal UninstallAppResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.UninstallAppResponseMessage;
		}
	}
}
