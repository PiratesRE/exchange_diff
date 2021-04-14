using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DisableAppResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class DisableAppResponseMessage : ResponseMessage
	{
		public DisableAppResponseMessage()
		{
		}

		internal DisableAppResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
