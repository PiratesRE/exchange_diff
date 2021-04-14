using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ProxyLanguagePostRequest : ProxyProtocolRequest
	{
		internal static void ParseProxyLanguagePostBody(Stream bodyStream, out CultureInfo culture, out string timeZoneKeyName, out bool isOptimized, out string destination, out SerializedClientSecurityContext serializedContext)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug(0L, "ProxyLanguagePostRequest.ParseProxyLanguagePostBody");
			culture = null;
			timeZoneKeyName = string.Empty;
			isOptimized = false;
			destination = string.Empty;
			serializedContext = null;
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(bodyStream);
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;
				if (!xmlTextReader.Read() || XmlNodeType.Element != xmlTextReader.NodeType || StringComparer.OrdinalIgnoreCase.Compare(xmlTextReader.Name, ProxyLanguagePostRequest.rootElementName) != 0)
				{
					ProxyLanguagePostRequest.ThrowParserException(xmlTextReader, "Missing or invalid root node");
				}
				if (xmlTextReader.MoveToFirstAttribute())
				{
					do
					{
						if (StringComparer.OrdinalIgnoreCase.Compare(xmlTextReader.Name, ProxyLanguagePostRequest.timeZoneKeyNameAttributeName) == 0)
						{
							if (DateTimeUtilities.IsValidTimeZoneKeyName(xmlTextReader.Value))
							{
								timeZoneKeyName = xmlTextReader.Value;
								ExTraceGlobals.ProxyDataTracer.TraceDebug<string>(0L, "Found timeZoneKeyName={0}", timeZoneKeyName);
							}
							else
							{
								ProxyLanguagePostRequest.ThrowParserException(xmlTextReader, "Invalid time zone id");
							}
						}
						else if (StringComparer.OrdinalIgnoreCase.Compare(xmlTextReader.Name, ProxyLanguagePostRequest.localeIdAttributeName) == 0)
						{
							int num = -1;
							if (int.TryParse(xmlTextReader.Value, out num) && Culture.IsSupportedCulture(num))
							{
								culture = Culture.GetCultureInfoInstance(num);
								ExTraceGlobals.ProxyDataTracer.TraceDebug<int>(0L, "Found localeId={0}", num);
							}
							else
							{
								ProxyLanguagePostRequest.ThrowParserException(xmlTextReader, "Invalid locale id");
							}
						}
						else if (StringComparer.OrdinalIgnoreCase.Compare(xmlTextReader.Name, ProxyLanguagePostRequest.isOptimizedAttributeName) == 0)
						{
							int num2 = -1;
							if (int.TryParse(xmlTextReader.Value, out num2))
							{
								isOptimized = (num2 == 1);
								ExTraceGlobals.ProxyDataTracer.TraceDebug<bool>(0L, "Found isOptimized={0}", isOptimized);
							}
							else
							{
								ProxyLanguagePostRequest.ThrowParserException(xmlTextReader, "Invalid is-optimized value");
							}
						}
						else if (StringComparer.OrdinalIgnoreCase.Compare(xmlTextReader.Name, ProxyLanguagePostRequest.destinationAttributeName) == 0)
						{
							destination = xmlTextReader.Value;
						}
						else
						{
							ExTraceGlobals.ProxyTracer.TraceDebug(0L, "ProxyLanguagePostRequest.ParseProxyLanguagePostBody - Found invalid attribute, ignoring it.");
						}
					}
					while (xmlTextReader.MoveToNextAttribute());
				}
				ExTraceGlobals.ProxyTracer.TraceDebug(0L, "Deserializing client context...");
				serializedContext = SerializedClientSecurityContext.Deserialize(xmlTextReader);
				if (!xmlTextReader.Read() || XmlNodeType.EndElement != xmlTextReader.NodeType || StringComparer.OrdinalIgnoreCase.Compare(xmlTextReader.Name, ProxyLanguagePostRequest.rootElementName) != 0)
				{
					ProxyLanguagePostRequest.ThrowParserException(xmlTextReader, "Missing or invalid root node");
				}
			}
			catch (XmlException ex)
			{
				ProxyLanguagePostRequest.ThrowParserException(xmlTextReader, string.Format("Parser threw an XML exception: {0}", ex.Message));
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		internal void BeginSend(OwaContext owaContext, HttpRequest originalRequest, OwaIdentity identity, CultureInfo culture, string timeZoneKeyName, bool isOptimized, string destination, AsyncCallback callback, object extraData)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyLanguagePostRequest.BeginSend");
			StringWriter stringWriter = null;
			XmlTextWriter xmlTextWriter = null;
			string proxyRequestBody = null;
			try
			{
				stringWriter = new StringWriter();
				xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteStartElement(ProxyLanguagePostRequest.rootElementName);
				xmlTextWriter.WriteAttributeString(ProxyLanguagePostRequest.localeIdAttributeName, culture.LCID.ToString());
				xmlTextWriter.WriteAttributeString(ProxyLanguagePostRequest.timeZoneKeyNameAttributeName, timeZoneKeyName);
				xmlTextWriter.WriteAttributeString(ProxyLanguagePostRequest.isOptimizedAttributeName, isOptimized ? "1" : "0");
				xmlTextWriter.WriteAttributeString(ProxyLanguagePostRequest.destinationAttributeName, destination);
				SerializedClientSecurityContext serializedClientSecurityContext = SerializedClientSecurityContext.CreateFromOwaIdentity(identity);
				serializedClientSecurityContext.Serialize(xmlTextWriter);
				xmlTextWriter.WriteEndElement();
				stringWriter.Flush();
				proxyRequestBody = stringWriter.ToString();
				ExTraceGlobals.ProxyDataTracer.TraceDebug<int, string, bool>((long)this.GetHashCode(), "Sending xml payload with lcid={0}, tzid={1}, isOptimized={2}", culture.LCID, timeZoneKeyName, isOptimized);
			}
			finally
			{
				if (stringWriter != null)
				{
					stringWriter.Close();
				}
				if (xmlTextWriter != null)
				{
					xmlTextWriter.Close();
				}
			}
			base.BeginSend(owaContext, originalRequest, OwaUrl.LanguagePost.GetExplicitUrl(originalRequest), proxyRequestBody, callback, extraData);
		}

		private static void ThrowParserException(XmlTextReader reader, string description)
		{
			throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "Invalid language post request body. Line number: {0} Position: {1}.{2}", new object[]
			{
				reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				(description != null) ? (" " + description) : string.Empty
			}));
		}

		private static readonly string rootElementName = "r";

		private static readonly string localeIdAttributeName = "lcid";

		private static readonly string timeZoneKeyNameAttributeName = "tzid";

		private static readonly string isOptimizedAttributeName = "opt";

		private static readonly string destinationAttributeName = "destination";
	}
}
