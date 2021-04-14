using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal sealed class LogQuerySerializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("LogQuery", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterLogQuery)writer).Write28_LogQuery(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderLogQuery)reader).Read28_LogQuery();
		}
	}
}
