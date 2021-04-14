using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class PhoneSet : ExtractionSet<Phone>
	{
		public PhoneSet() : base(new PhoneSerializer())
		{
		}

		public static implicit operator PhoneSet(Phone[] phones)
		{
			return new PhoneSet
			{
				Extractions = phones
			};
		}

		public static XmlSerializer Serializer
		{
			get
			{
				return PhoneSet.serializer;
			}
		}

		public override XmlSerializer GetSerializer()
		{
			return PhoneSet.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(PhoneSet));
	}
}
