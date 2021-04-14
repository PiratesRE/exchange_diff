using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EdgeSync.Common
{
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderStatus();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterStatus();
		}
	}
}
