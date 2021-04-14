using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class WSAuthorization
	{
		public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2006/12/authorization";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("auth", "http://schemas.xmlsoap.org/ws/2006/12/authorization");

		public static readonly XmlElementDefinition Value = new XmlElementDefinition("Value", WSAuthorization.Namespace);

		public static readonly XmlElementDefinition AdditionalContext = new XmlElementDefinition("AdditionalContext", WSAuthorization.Namespace);

		public static readonly XmlElementDefinition ContextItem = new XmlElementDefinition("ContextItem", WSAuthorization.Namespace);

		public static readonly XmlElementDefinition ClaimType = new XmlElementDefinition("ClaimType", WSAuthorization.Namespace);
	}
}
