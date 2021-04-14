using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[XmlRoot(ElementName = "Error", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006")]
	public class AutoDiscoverError
	{
		[XmlAttribute]
		public string Time;

		[XmlAttribute]
		public string Id;

		public string ErrorCode;

		public string Message;

		public string DebugData;
	}
}
