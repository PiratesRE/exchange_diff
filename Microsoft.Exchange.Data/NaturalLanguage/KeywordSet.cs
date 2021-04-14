using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class KeywordSet : ExtractionSet<Keyword>
	{
		public KeywordSet() : base(new KeywordSerializer())
		{
		}

		public static implicit operator KeywordSet(Keyword[] keywords)
		{
			return new KeywordSet
			{
				Extractions = keywords
			};
		}

		public static XmlSerializer Serializer
		{
			get
			{
				return KeywordSet.serializer;
			}
		}

		public override XmlSerializer GetSerializer()
		{
			return KeywordSet.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(KeywordSet));
	}
}
