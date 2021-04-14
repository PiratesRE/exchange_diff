using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CacheDiagnosticMethods
	{
		internal static XmlNode ClearExchangeRunspaceConfigurationCache(XmlNode param)
		{
			ExchangeRunspaceConfigurationCache.Singleton.ClearCache();
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateElement("Success", "http://schemas.microsoft.com/exchange/services/2006/types");
			xmlNode.InnerText = true.ToString();
			return xmlNode;
		}

		private const string SuccessXmlElement = "Success";
	}
}
