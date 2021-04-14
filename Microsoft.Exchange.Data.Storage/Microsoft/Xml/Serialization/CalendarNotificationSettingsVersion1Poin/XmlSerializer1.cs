using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationSettingsVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderCalendarNotificationSettingsVersion1Point0();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterCalendarNotificationSettingsVersion1Point0();
		}
	}
}
