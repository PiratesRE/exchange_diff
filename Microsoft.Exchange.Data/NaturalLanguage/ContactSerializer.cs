using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContactSerializer : BaseSerializer<Contact>
	{
		protected override XmlSerializer GetSerializer()
		{
			return ContactSerializer.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(Contact[]), new XmlRootAttribute("Contacts"));
	}
}
