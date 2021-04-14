using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class Soap
	{
		public const string NamespaceUri = "http://www.w3.org/2003/05/soap-envelope";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("s", "http://www.w3.org/2003/05/soap-envelope");

		public static readonly XmlElementDefinition Envelope = new XmlElementDefinition("Envelope", Soap.Namespace);

		public static readonly XmlElementDefinition Header = new XmlElementDefinition("Header", Soap.Namespace);

		public static readonly XmlElementDefinition Body = new XmlElementDefinition("Body", Soap.Namespace);

		public static readonly XmlElementDefinition Fault = new XmlElementDefinition("Fault", Soap.Namespace);

		public static readonly XmlElementDefinition Code = new XmlElementDefinition("Code", Soap.Namespace);

		public static readonly XmlElementDefinition Value = new XmlElementDefinition("Value", Soap.Namespace);

		public static readonly XmlElementDefinition Subcode = new XmlElementDefinition("Subcode", Soap.Namespace);

		public static readonly XmlAttributeDefinition MustUnderstand = new XmlAttributeDefinition("mustUnderstand", Soap.Namespace);
	}
}
