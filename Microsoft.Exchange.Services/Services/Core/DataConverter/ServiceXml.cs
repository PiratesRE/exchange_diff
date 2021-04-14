using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal static class ServiceXml
	{
		static ServiceXml()
		{
			ServiceXml.namespacePrefixes.Add("http://schemas.microsoft.com/exchange/services/2006/types", "t");
			ServiceXml.namespacePrefixes.Add("http://schemas.microsoft.com/exchange/services/2006/messages", "m");
			ServiceXml.namespacePrefixes.Add("http://schemas.microsoft.com/exchange/services/2006/errors", "e");
			ServiceXml.namespacePrefixes.Add("http://schemas.xmlsoap.org/soap/envelope/", "soap11");
			ServiceXml.namespacePrefixes.Add("http://www.w3.org/2003/05/soap-envelope", "soap12");
			ServiceXml.InitNamespaceManager();
		}

		public static string GetFullyQualifiedName(string name, string namespaceUri)
		{
			return namespaceUri + ":" + name;
		}

		public static string GetFullyQualifiedName(string name)
		{
			return ServiceXml.GetFullyQualifiedName(name, ServiceXml.DefaultNamespaceUri);
		}

		public static string DefaultNamespaceUri
		{
			get
			{
				return "http://schemas.microsoft.com/exchange/services/2006/types";
			}
		}

		private static bool IsTextOnlyWhitespace(string nodeValue)
		{
			foreach (char c in nodeValue)
			{
				if (!char.IsWhiteSpace(c))
				{
					return false;
				}
			}
			return true;
		}

		public static string ConvertToString(object value)
		{
			return BaseConverter.GetConverterForType(value.GetType()).ConvertToString(value);
		}

		public static void InitNamespaceManager()
		{
			ServiceXml.namespaceManager = new XmlNamespaceManager(new NameTable());
			ServiceXml.namespaceManager.AddNamespace("t", "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.namespaceManager.AddNamespace("m", "http://schemas.microsoft.com/exchange/services/2006/messages");
			ServiceXml.namespaceManager.AddNamespace("e", "http://schemas.microsoft.com/exchange/services/2006/errors");
			ServiceXml.namespaceManager.AddNamespace("soap11", "http://schemas.xmlsoap.org/soap/envelope/");
			ServiceXml.namespaceManager.AddNamespace("soap12", "http://www.w3.org/2003/05/soap-envelope");
		}

		public static XmlNamespaceManager NamespaceManager
		{
			get
			{
				return ServiceXml.namespaceManager;
			}
		}

		public static XmlElement CreateElement(XmlDocument xmlDocument, string localName, string namespaceUri)
		{
			return xmlDocument.CreateElement(ServiceXml.namespacePrefixes[namespaceUri], localName, namespaceUri);
		}

		public static string GetXmlTextNodeValue(XmlElement textNodeParent)
		{
			return textNodeParent.InnerText;
		}

		public static string GetXmlTextNodeValue(XmlElement xmlParentNode, string xmlElementName, string typeNamespace)
		{
			XmlElement xmlElement = xmlParentNode[xmlElementName, typeNamespace];
			if (xmlElement != null)
			{
				return xmlElement.InnerText;
			}
			return null;
		}

		public static string GetXmlElementAttributeValueOptional(XmlElement xmlParentNode, string xmlAttributeName)
		{
			if (xmlParentNode.HasAttribute(xmlAttributeName))
			{
				return xmlParentNode.Attributes[xmlAttributeName].Value;
			}
			return null;
		}

		public static ServiceXml.NormalizationAction GetNormalizationAction(XmlNode node)
		{
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
				if (ServiceXml.IsLeafNode(node))
				{
					return ServiceXml.NormalizationAction.LeaveAsIs;
				}
				return ServiceXml.NormalizationAction.Normalize;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				return ServiceXml.NormalizationAction.Remove;
			}
			return ServiceXml.NormalizationAction.Remove;
		}

		public static void NormalizeXml(XmlNode xmlNode)
		{
			for (int i = xmlNode.ChildNodes.Count - 1; i >= 0; i--)
			{
				XmlNode xmlNode2 = xmlNode.ChildNodes[i];
				switch (ServiceXml.GetNormalizationAction(xmlNode2))
				{
				case ServiceXml.NormalizationAction.Normalize:
					ServiceXml.NormalizeXml(xmlNode2);
					break;
				case ServiceXml.NormalizationAction.Remove:
					xmlNode.RemoveChild(xmlNode2);
					break;
				}
			}
		}

		public static void NormalizeXmlHandleNullNode(XmlNode xmlNode)
		{
			if (xmlNode != null)
			{
				ServiceXml.NormalizeXml(xmlNode);
			}
		}

		public static XmlElement CreateElement(XmlElement parentElement, string localName, string namespaceUri)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement.OwnerDocument, localName, namespaceUri);
			parentElement.AppendChild(xmlElement);
			return xmlElement;
		}

		public static XmlElement CreateElement(XmlElement parentElement, string localName)
		{
			return ServiceXml.CreateElement(parentElement, localName, parentElement.NamespaceURI);
		}

		public static XmlElement CreateTextElement(XmlElement parentElement, string localName, string textValue, string namespaceUri)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement, localName, namespaceUri);
			ServiceXml.AppendText(xmlElement, textValue);
			return xmlElement;
		}

		public static XmlElement CreateTextElement(XmlElement parentElement, string localName, XmlText textNode, string namespaceUri)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement, localName, namespaceUri);
			xmlElement.AppendChild(textNode);
			return xmlElement;
		}

		private static void AppendText(XmlElement parentElement, string textValue)
		{
			if (!string.IsNullOrEmpty(textValue))
			{
				XmlText newChild = parentElement.OwnerDocument.CreateTextNode(textValue);
				parentElement.AppendChild(newChild);
			}
		}

		public static XmlElement CreateTextElement(XmlElement parentElement, string localName, string textValue)
		{
			return ServiceXml.CreateTextElement(parentElement, localName, textValue, parentElement.NamespaceURI);
		}

		public static XmlElement CreateNonEmptyTextElement(XmlElement xmlElement, string xmlElementName, string xmlElementValue)
		{
			if (string.IsNullOrEmpty(xmlElementValue))
			{
				return null;
			}
			return ServiceXml.CreateTextElement(xmlElement, xmlElementName, xmlElementValue);
		}

		public static XmlAttribute CreateAttribute(XmlElement parentElement, string attributeName, string attributeValue)
		{
			XmlAttribute xmlAttribute = parentElement.OwnerDocument.CreateAttribute(string.Empty, attributeName, string.Empty);
			xmlAttribute.Value = attributeValue;
			parentElement.Attributes.Append(xmlAttribute);
			return xmlAttribute;
		}

		public static bool IsLeafNode(XmlNode node)
		{
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					return false;
				}
			}
			return true;
		}

		private const string NamespaceUriDelimiter = ":";

		private static Dictionary<string, string> namespacePrefixes = new Dictionary<string, string>(10);

		private static XmlNamespaceManager namespaceManager;

		public enum NormalizationAction
		{
			Normalize,
			LeaveAsIs,
			Remove
		}
	}
}
