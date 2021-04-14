using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("InstallAppResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class InstallAppResponseMessage : ResponseMessage
	{
		public InstallAppResponseMessage()
		{
		}

		internal InstallAppResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
