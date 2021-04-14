using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderLogQuery();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterLogQuery();
		}
	}
}
