using System;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RequestTypeHeader : SoapHeader
	{
		[XmlElement]
		public ProxyRequestType RequestType;
	}
}
