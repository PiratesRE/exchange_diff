﻿using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.ProvisionResponse
{
	internal sealed class ProvisionSerializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("Provision", "DeltaSyncV2:");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterProvision)writer).Write6_Provision(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderProvision)reader).Read6_Provision();
		}
	}
}
