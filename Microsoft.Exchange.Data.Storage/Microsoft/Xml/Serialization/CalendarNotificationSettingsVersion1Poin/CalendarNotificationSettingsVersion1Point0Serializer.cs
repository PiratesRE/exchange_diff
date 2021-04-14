using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationSettingsVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarNotificationSettingsVersion1Point0Serializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("CalendarNotificationSettings", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterCalendarNotificationSettingsVersion1Point0)writer).Write16_CalendarNotificationSettings(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderCalendarNotificationSettingsVersion1Point0)reader).Read16_CalendarNotificationSettings();
		}
	}
}
