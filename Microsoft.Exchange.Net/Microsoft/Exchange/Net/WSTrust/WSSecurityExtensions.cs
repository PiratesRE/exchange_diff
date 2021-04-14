using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class WSSecurityExtensions
	{
		public const string NamespaceUri = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("o", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");

		public static readonly XmlElementDefinition Security = new XmlElementDefinition("Security", WSSecurityExtensions.Namespace);

		public static readonly XmlElementDefinition BinarySecurityToken = new XmlElementDefinition("BinarySecurityToken", WSSecurityExtensions.Namespace);

		public static readonly XmlElementDefinition SecurityTokenReference = new XmlElementDefinition("SecurityTokenReference", WSSecurityExtensions.Namespace);

		public static readonly XmlElementDefinition Reference = new XmlElementDefinition("Reference", WSSecurityExtensions.Namespace);

		public static readonly XmlElementDefinition KeyIdentifier = new XmlElementDefinition("KeyIdentifier", WSSecurityExtensions.Namespace);
	}
}
