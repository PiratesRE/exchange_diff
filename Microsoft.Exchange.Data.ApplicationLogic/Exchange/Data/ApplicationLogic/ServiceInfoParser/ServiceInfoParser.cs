using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.ServiceInfoParser
{
	internal static class ServiceInfoParser
	{
		public static Uri GetRootSiteUrlFromServiceInfo(XmlDocument doc, ITracer tracer)
		{
			if (doc == null)
			{
				tracer.TraceDebug(0L, "GetRootSiteUrlFromServiceInfo: doc is null");
				return null;
			}
			return ServiceInfoParser.GetRootSiteUrlFromServiceInfo(doc.DocumentElement, tracer);
		}

		public static Uri GetRootSiteUrlFromServiceInfo(XmlElement root, ITracer tracer)
		{
			return ServiceInfoParser.GetServiceParameterUrlFromServiceInfo(root, tracer, "SPO_ROOTSITEURL", "http://schemas.microsoft.com/online/serviceextensions/2009/08/ExtensibilitySchema.xsd");
		}

		private static Uri GetServiceParameterUrlFromServiceInfo(XmlElement root, ITracer tracer, string urlAttributeName, string namespaceValue)
		{
			if (root == null)
			{
				tracer.TraceDebug<string>(0L, "GetUrlFromServiceInfo for attribute: root is null", urlAttributeName);
				return null;
			}
			Exception ex = null;
			try
			{
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(root.OwnerDocument.NameTable);
				xmlNamespaceManager.AddNamespace("es", namespaceValue);
				string xpath = string.Format("//es:ServiceParameter[translate(es:Name,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')='{0}']/es:Value", urlAttributeName);
				XmlNode xmlNode = root.SelectSingleNode(xpath, xmlNamespaceManager);
				if (xmlNode != null)
				{
					tracer.TraceDebug<string, string>(0L, "Get {1} from ServiceInfo: {0}", xmlNode.InnerText, urlAttributeName);
					return new Uri(xmlNode.InnerText);
				}
			}
			catch (XmlException ex2)
			{
				ex = ex2;
			}
			catch (XPathException ex3)
			{
				ex = ex3;
			}
			catch (UriFormatException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				tracer.TraceError<Exception, string>(0L, "Failed to get {1} from ServiceInfo: {0}", ex, urlAttributeName);
			}
			return null;
		}

		public static Uri GetWindowsIntuneUrlFromServiceInfo(XmlElement element, ITracer tracer)
		{
			return ServiceInfoParser.GetServiceParameterUrlFromServiceInfo(element, tracer, "ODMSENDPOINTURL", string.Empty);
		}
	}
}
