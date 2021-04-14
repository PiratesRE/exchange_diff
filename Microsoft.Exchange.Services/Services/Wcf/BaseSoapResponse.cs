using System;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public abstract class BaseSoapResponse
	{
		public BaseSoapResponse()
		{
			this.ServerVersionInfo = ServerVersionInfo.CurrentAssemblyVersion;
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[MessageHeader(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public ServerVersionInfo ServerVersionInfo;
	}
}
