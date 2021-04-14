using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.WorkHoursInCalendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class WorkHoursInCalendarSerializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("Root", "WorkingHours.xsd");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterWorkHoursInCalendar)writer).Write9_Root(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderWorkHoursInCalendar)reader).Read9_Root();
		}
	}
}
