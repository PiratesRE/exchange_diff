using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class WSAddressing
	{
		public const string NamespaceUri = "http://www.w3.org/2005/08/addressing";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("a", "http://www.w3.org/2005/08/addressing");

		public static readonly XmlElementDefinition To = new XmlElementDefinition("To", WSAddressing.Namespace);

		public static readonly XmlElementDefinition Action = new XmlElementDefinition("Action", WSAddressing.Namespace);

		public static readonly XmlElementDefinition MessageId = new XmlElementDefinition("MessageID", WSAddressing.Namespace);

		public static readonly XmlElementDefinition ReplyTo = new XmlElementDefinition("ReplyTo", WSAddressing.Namespace);

		public static readonly XmlElementDefinition Address = new XmlElementDefinition("Address", WSAddressing.Namespace);

		public static readonly XmlElementDefinition EndpointReference = new XmlElementDefinition("EndpointReference", WSAddressing.Namespace);
	}
}
