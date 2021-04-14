using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[XmlRoot(ElementName = "Autodiscover", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/outlook/requestschema/2006")]
	public class AutoDiscoverRequestXML
	{
		public static AutoDiscoverRequestXML NewRequest(string emailAddress)
		{
			AutoDiscoverRequest autoDiscoverRequest = new AutoDiscoverRequest();
			autoDiscoverRequest.EMailAddress = emailAddress;
			autoDiscoverRequest.AcceptableResponseSchema = "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a";
			return new AutoDiscoverRequestXML
			{
				Request = autoDiscoverRequest
			};
		}

		[XmlElement(IsNullable = false)]
		public AutoDiscoverRequest Request;
	}
}
