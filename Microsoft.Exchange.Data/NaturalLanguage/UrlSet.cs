using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class UrlSet : ExtractionSet<Url>
	{
		public UrlSet() : base(new UrlSerializer())
		{
		}

		public static implicit operator UrlSet(Url[] urls)
		{
			return new UrlSet
			{
				Extractions = urls
			};
		}

		public static XmlSerializer Serializer
		{
			get
			{
				return UrlSet.serializer;
			}
		}

		public override XmlSerializer GetSerializer()
		{
			return UrlSet.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(UrlSet));
	}
}
