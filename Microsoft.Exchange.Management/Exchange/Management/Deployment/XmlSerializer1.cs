using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.Deployment
{
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderSetupComponentInfo();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterSetupComponentInfo();
		}
	}
}
