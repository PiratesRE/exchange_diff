using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[XmlRoot(ElementName = "User", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a")]
	public class AutoDiscoverUser
	{
		public string DisplayName;

		public string LegacyDN;

		public string AutoDiscoverSMTPAddress;

		public string DefaultABView;

		public string DeploymentId;
	}
}
