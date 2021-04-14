using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class MeetingSet : ExtractionSet<Meeting>
	{
		public MeetingSet() : base(new MeetingSerializer())
		{
		}

		public static implicit operator MeetingSet(Meeting[] meetings)
		{
			return new MeetingSet
			{
				Extractions = meetings
			};
		}

		public static XmlSerializer Serializer
		{
			get
			{
				return MeetingSet.serializer;
			}
		}

		public override XmlSerializer GetSerializer()
		{
			return MeetingSet.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(MeetingSet));
	}
}
