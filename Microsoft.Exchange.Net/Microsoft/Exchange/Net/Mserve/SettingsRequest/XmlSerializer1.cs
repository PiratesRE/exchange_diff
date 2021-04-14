using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderSettings();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterSettings();
		}
	}
}
