using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Compliance.Xml
{
	internal class SafeXmlSerializer : XmlSerializer
	{
		public SafeXmlSerializer()
		{
		}

		public SafeXmlSerializer(Type type) : base(type)
		{
		}

		public SafeXmlSerializer(XmlTypeMapping xmlTypeMapping) : base(xmlTypeMapping)
		{
		}

		public SafeXmlSerializer(Type type, string defaultNamespace) : base(type, defaultNamespace)
		{
		}

		public SafeXmlSerializer(Type type, Type[] extraTypes) : base(type, extraTypes)
		{
		}

		public SafeXmlSerializer(Type type, XmlAttributeOverrides overrides) : base(type, overrides)
		{
		}

		public SafeXmlSerializer(Type type, XmlRootAttribute root) : base(type, root)
		{
		}

		public SafeXmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace) : base(type, overrides, extraTypes, root, defaultNamespace)
		{
		}

		public new object Deserialize(Stream stream)
		{
			return base.Deserialize(new XmlTextReader(stream)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			});
		}

		public new object Deserialize(TextReader txtReader)
		{
			return base.Deserialize(new XmlTextReader(txtReader)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			});
		}

		public new object Deserialize(XmlReader reader)
		{
			if (reader.Settings != null && reader.Settings.DtdProcessing != DtdProcessing.Prohibit)
			{
				throw new XmlDtdException();
			}
			return base.Deserialize(reader);
		}

		public new object Deserialize(XmlReader reader, string encodingStyle)
		{
			if (reader.Settings != null && reader.Settings.DtdProcessing != DtdProcessing.Prohibit)
			{
				throw new XmlDtdException();
			}
			return base.Deserialize(reader, encodingStyle);
		}

		public new object Deserialize(XmlReader reader, XmlDeserializationEvents events)
		{
			if (reader.Settings != null && reader.Settings.DtdProcessing != DtdProcessing.Prohibit)
			{
				throw new XmlDtdException();
			}
			return base.Deserialize(reader, events);
		}
	}
}
