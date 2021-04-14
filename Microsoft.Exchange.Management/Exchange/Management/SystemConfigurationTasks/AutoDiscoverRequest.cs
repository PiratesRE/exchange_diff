using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[XmlRoot(ElementName = "Request", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/outlook/requestschema/2006")]
	public class AutoDiscoverRequest
	{
		public string EMailAddress;

		public string AcceptableResponseSchema;
	}
}
