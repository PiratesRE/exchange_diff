using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(TypeName = "Request")]
	public class Request
	{
		public Request()
		{
			this.AcceptableResponseSchema = "http://schemas.microsoft.com/exchange/autodiscover/mobilesync/responseschema/2006";
		}

		[XmlElement(ElementName = "EMailAddress")]
		public string EMailAddress { get; set; }

		[XmlElement(ElementName = "AcceptableResponseSchema")]
		public string AcceptableResponseSchema { get; set; }

		private const string Schema = "http://schemas.microsoft.com/exchange/autodiscover/mobilesync/responseschema/2006";
	}
}
