using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class WSSecurityUtility
	{
		public const string NamespaceUri = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("u", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");

		public static readonly XmlAttributeDefinition Id = new XmlAttributeDefinition("Id", WSSecurityUtility.Namespace);

		public static readonly XmlElementDefinition Timestamp = new XmlElementDefinition("Timestamp", WSSecurityUtility.Namespace);

		public static readonly XmlElementDefinition Created = new XmlElementDefinition("Created", WSSecurityUtility.Namespace);

		public static readonly XmlElementDefinition Expires = new XmlElementDefinition("Expires", WSSecurityUtility.Namespace);
	}
}
