using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationContentVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarNotificationContentVersion1Point0Serializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("CalendarNotificationContent", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterCalendarNotificationContentVersion1Point0)writer).Write7_CalendarNotificationContent(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderCalendarNotificationContentVersion1Point0)reader).Read7_CalendarNotificationContent();
		}
	}
}
