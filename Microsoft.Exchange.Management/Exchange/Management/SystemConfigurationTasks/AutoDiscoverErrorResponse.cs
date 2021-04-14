using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[XmlRoot(ElementName = "Response", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006")]
	public class AutoDiscoverErrorResponse
	{
		[XmlElement(ElementName = "Error", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006")]
		public AutoDiscoverError Error;
	}
}
