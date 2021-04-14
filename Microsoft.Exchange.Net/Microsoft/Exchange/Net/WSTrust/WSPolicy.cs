using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class WSPolicy
	{
		public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2004/09/policy";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("wsp", "http://schemas.xmlsoap.org/ws/2004/09/policy");

		public static readonly XmlElementDefinition AppliesTo = new XmlElementDefinition("AppliesTo", WSPolicy.Namespace);

		public static readonly XmlElementDefinition PolicyReference = new XmlElementDefinition("PolicyReference", WSPolicy.Namespace);
	}
}
