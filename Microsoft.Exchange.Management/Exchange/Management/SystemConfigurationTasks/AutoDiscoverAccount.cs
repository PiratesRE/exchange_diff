using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[XmlRoot(ElementName = "Account", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a")]
	public class AutoDiscoverAccount
	{
		public string AccountType;

		public string Action;

		public string RedirectAddr;

		[XmlElement]
		public AutoDiscoverProtocol[] Protocol;

		public string SSL;

		public string AuthPackage;
	}
}
