using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AddressSerializer : BaseSerializer<Address>
	{
		protected override XmlSerializer GetSerializer()
		{
			return AddressSerializer.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(Address[]), new XmlRootAttribute("Addresses"));
	}
}
