using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class XmlEncryption
	{
		public const string NamespaceUri = "http://www.w3.org/2001/04/xmlenc#";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("de", "http://www.w3.org/2001/04/xmlenc#");

		public static readonly XmlElementDefinition EncryptedKey = new XmlElementDefinition("EncryptedKey", XmlEncryption.Namespace);
	}
}
