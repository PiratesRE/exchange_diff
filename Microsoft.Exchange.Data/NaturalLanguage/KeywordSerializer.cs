using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class KeywordSerializer : BaseSerializer<Keyword>
	{
		protected override XmlSerializer GetSerializer()
		{
			return KeywordSerializer.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(Keyword[]), new XmlRootAttribute("Keywords"));
	}
}
