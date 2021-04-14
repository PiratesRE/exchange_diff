using System;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlRoot("SharingSecurity", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[Serializable]
	public sealed class SharingSecurityHeader : SoapHeader
	{
		public SharingSecurityHeader()
		{
		}

		internal SharingSecurityHeader(XmlElement any)
		{
			this.Any = any;
		}

		[XmlAnyElement]
		public XmlElement Any;
	}
}
