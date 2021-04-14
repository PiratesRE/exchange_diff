using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public abstract class BaseSerializer<T> : IExtractionSerializer<T>
	{
		protected abstract XmlSerializer GetSerializer();

		public virtual T[] ReadOldXml(XmlReader reader, Version version)
		{
			reader.Skip();
			return null;
		}

		public T[] ReadXml(XmlReader reader, Version version)
		{
			if (version == ExtractionVersions.CurrentVersion)
			{
				XmlSerializer serializer = this.GetSerializer();
				return serializer.Deserialize(reader) as T[];
			}
			return this.ReadOldXml(reader, version);
		}

		public void WriteXml(XmlWriter writer, T[] t)
		{
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add(string.Empty, string.Empty);
			XmlSerializer serializer = this.GetSerializer();
			serializer.Serialize(writer, t, xmlSerializerNamespaces);
		}
	}
}
