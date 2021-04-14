using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class Unqualified
	{
		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition(null, null);

		public static readonly XmlAttributeDefinition Id = new XmlAttributeDefinition("Id", Unqualified.Namespace);

		public static readonly XmlAttributeDefinition URI = new XmlAttributeDefinition("URI", Unqualified.Namespace);

		public static readonly XmlAttributeDefinition Uri = new XmlAttributeDefinition("Uri", Unqualified.Namespace);

		public static readonly XmlAttributeDefinition Type = new XmlAttributeDefinition("Type", Unqualified.Namespace);

		public static readonly XmlAttributeDefinition ValueType = new XmlAttributeDefinition("ValueType", Unqualified.Namespace);

		public static readonly XmlAttributeDefinition Dialect = new XmlAttributeDefinition("Dialect", Unqualified.Namespace);

		public static readonly XmlAttributeDefinition Name = new XmlAttributeDefinition("Name", Unqualified.Namespace);

		public static readonly XmlAttributeDefinition Scope = new XmlAttributeDefinition("Scope", Unqualified.Namespace);
	}
}
