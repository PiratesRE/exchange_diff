using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[XmlRoot(ElementName = "Autodiscover", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006")]
	public class AutoDiscoverResponseXML
	{
		[XmlElement(IsNullable = false, ElementName = "Response", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006")]
		public AutoDiscoverErrorResponse ErrorResponse;

		[XmlElement(IsNullable = false, ElementName = "Response", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a")]
		public AutoDiscoverDataResponse DataResponse;
	}
}
