using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class AddressSet : ExtractionSet<Address>
	{
		public AddressSet() : base(new AddressSerializer())
		{
		}

		public static implicit operator AddressSet(Address[] addresses)
		{
			return new AddressSet
			{
				Extractions = addresses
			};
		}

		public static XmlSerializer Serializer
		{
			get
			{
				return AddressSet.serializer;
			}
		}

		public override XmlSerializer GetSerializer()
		{
			return AddressSet.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(AddressSet));
	}
}
