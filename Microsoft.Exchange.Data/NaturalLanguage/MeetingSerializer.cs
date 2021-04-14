using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MeetingSerializer : BaseSerializer<Meeting>
	{
		protected override XmlSerializer GetSerializer()
		{
			return MeetingSerializer.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(Meeting[]), new XmlRootAttribute("Meetings"));
	}
}
