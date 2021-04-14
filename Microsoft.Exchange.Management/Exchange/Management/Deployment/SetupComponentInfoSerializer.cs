using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class SetupComponentInfoSerializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("SetupComponentInfo", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterSetupComponentInfo)writer).Write13_SetupComponentInfo(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderSetupComponentInfo)reader).Read13_SetupComponentInfo();
		}
	}
}
