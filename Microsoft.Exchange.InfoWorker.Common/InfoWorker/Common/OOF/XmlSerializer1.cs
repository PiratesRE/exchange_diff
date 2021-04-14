using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderUserOofSettingsSerializer();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterUserOofSettingsSerializer();
		}
	}
}
