using System;

namespace Microsoft.Exchange.Services.Core
{
	internal static class XmlNamespaces
	{
		public const string XmlDeclaration = "<?xml version=\"1.0\"?>";

		public const string NamespaceBase = "http://schemas.microsoft.com/exchange/services/2006";

		public const string TypeNamespace = "http://schemas.microsoft.com/exchange/services/2006/types";

		public const string MessageNamespace = "http://schemas.microsoft.com/exchange/services/2006/messages";

		public const string BindingNamespace = "http://schemas.microsoft.com/exchange/services/2006/binding";

		public const string ErrorNamespace = "http://schemas.microsoft.com/exchange/services/2006/errors";

		public const string TypePrefix = "t";

		public const string MessagePrefix = "m";

		public const string ErrorPrefix = "e";

		public const string Soap11Namespace = "http://schemas.xmlsoap.org/soap/envelope/";

		public const string Soap12Namespace = "http://www.w3.org/2003/05/soap-envelope";

		public const string Soap11Prefix = "soap11";

		public const string Soap12Prefix = "soap12";

		public const string XmlSchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

		public const string XmlSchemaInstanceNilElementName = "nil";

		public const string WSAddressing200505Namespace = "http://schemas.microsoft.com/ws/2005/05/addressing/none";

		public const string WSAddressing10Namespace = "http://www.w3.org/2005/08/addressing";

		public const string WSSecurity200401SNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

		public const string XmlDigitalSignatureNamespace = "http://www.w3.org/2000/09/xmldsig#";

		public const string XmlEncryptionNamespace = "http://www.w3.org/2001/04/xmlenc#";
	}
}
