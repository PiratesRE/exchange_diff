using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.WorkHoursInCalendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class XmlSerializer1 : SafeXmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderWorkHoursInCalendar();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterWorkHoursInCalendar();
		}
	}
}
