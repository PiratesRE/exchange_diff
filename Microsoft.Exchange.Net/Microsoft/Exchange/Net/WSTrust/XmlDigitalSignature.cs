using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class XmlDigitalSignature
	{
		public const string NamespaceUri = "http://www.w3.org/2000/09/xmldsig#";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("ds", "http://www.w3.org/2000/09/xmldsig#");

		public static readonly XmlElementDefinition KeyInfo = new XmlElementDefinition("KeyInfo", XmlDigitalSignature.Namespace);
	}
}
