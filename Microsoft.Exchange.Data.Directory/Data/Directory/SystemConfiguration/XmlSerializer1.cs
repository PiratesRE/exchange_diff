using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderAutoAttendantSettings();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterAutoAttendantSettings();
		}
	}
}
