using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class ContactSet : ExtractionSet<Contact>
	{
		public ContactSet() : base(new ContactSerializer())
		{
		}

		public static implicit operator ContactSet(Contact[] contacts)
		{
			return new ContactSet
			{
				Extractions = contacts
			};
		}

		public static XmlSerializer Serializer
		{
			get
			{
				return ContactSet.serializer;
			}
		}

		public override XmlSerializer GetSerializer()
		{
			return ContactSet.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(ContactSet));
	}
}
