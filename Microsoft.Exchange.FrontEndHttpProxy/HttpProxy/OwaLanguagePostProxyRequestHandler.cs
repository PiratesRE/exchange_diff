using System;
using System.IO;
using System.Net;
using System.Xml;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OwaLanguagePostProxyRequestHandler : OwaProxyRequestHandler
	{
		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			if (base.ProxyToDownLevel)
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
					{
						SerializedClientSecurityContext serializedClientSecurityContext = base.HttpContext.GetSerializedClientSecurityContext();
						serializedClientSecurityContext.Serialize(xmlTextWriter);
						stringWriter.Flush();
						headers["X-OwaLanguageProxySerializedSecurityContext"] = stringWriter.ToString();
					}
				}
			}
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return !string.Equals(headerName, "X-OwaLanguageProxySerializedSecurityContext", StringComparison.OrdinalIgnoreCase) && base.ShouldCopyHeaderToServerRequest(headerName);
		}

		private const string LanguageProxySerializedSecurityContextHeaderName = "X-OwaLanguageProxySerializedSecurityContext";
	}
}
