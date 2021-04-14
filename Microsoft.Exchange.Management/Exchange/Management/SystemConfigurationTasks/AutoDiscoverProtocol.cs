using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[XmlRoot(ElementName = "Protocol", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a")]
	public class AutoDiscoverProtocol
	{
		public string Type;

		public string Server;

		public string ServerDN;

		public string ServerVersion;

		public string MdbDN;

		public string ASUrl;

		public string EwsUrl;

		public string OOFUrl;

		public string OABUrl;

		public string UMUrl;

		public int Port;

		public int DirectoryPort;

		public int ReferralPort;

		public string FBPublish;

		public string SSL;

		public string TTL;

		public string AuthPackage;

		public string CertPincipalName;

		[XmlAnyElement]
		public object[] OtherXml;
	}
}
