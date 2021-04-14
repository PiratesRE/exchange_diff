using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AutoAttendantSettingsSerializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("AutoAttendantSettings", string.Empty);
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterAutoAttendantSettings)writer).Write5_AutoAttendantSettings(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderAutoAttendantSettings)reader).Read5_AutoAttendantSettings();
		}
	}
}
