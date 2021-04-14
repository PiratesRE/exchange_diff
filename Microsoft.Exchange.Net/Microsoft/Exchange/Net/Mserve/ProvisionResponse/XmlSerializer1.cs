using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.ProvisionResponse
{
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderProvision();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterProvision();
		}
	}
}
