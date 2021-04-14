using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PhoneSerializer : BaseSerializer<Phone>
	{
		protected override XmlSerializer GetSerializer()
		{
			return PhoneSerializer.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(Phone[]), new XmlRootAttribute("Phones"));
	}
}
