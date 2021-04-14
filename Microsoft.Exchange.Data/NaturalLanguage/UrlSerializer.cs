using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UrlSerializer : BaseSerializer<Url>
	{
		protected override XmlSerializer GetSerializer()
		{
			return UrlSerializer.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(Url[]), new XmlRootAttribute("Urls"));
	}
}
