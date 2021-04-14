using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("InstallAppResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class InstallAppResponse : ResponseMessage
	{
		public InstallAppResponse()
		{
		}

		internal InstallAppResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.InstallAppResponseMessage;
		}
	}
}
