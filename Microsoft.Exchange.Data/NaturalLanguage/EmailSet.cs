using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class EmailSet : ExtractionSet<Email>
	{
		public EmailSet() : base(new EmailSerializer())
		{
		}

		public static implicit operator EmailSet(Email[] emails)
		{
			return new EmailSet
			{
				Extractions = emails
			};
		}

		public static XmlSerializer Serializer
		{
			get
			{
				return EmailSet.serializer;
			}
		}

		public override XmlSerializer GetSerializer()
		{
			return EmailSet.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(EmailSet));
	}
}
