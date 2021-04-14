using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public abstract class VersionedXmlBase
	{
		protected VersionedXmlBase()
		{
		}

		protected VersionedXmlBase(Version version)
		{
			if (null == version)
			{
				throw new ArgumentNullException("version");
			}
			this.Version = version.ToString(2);
		}

		[XmlAttribute("Version")]
		public string Version { get; set; }

		internal static void Serialize(Stream stream, VersionedXmlBase obj)
		{
			if (stream == null)
			{
				throw new CustomSerializationException(ServerStrings.ErrorInvalidConfigurationXml, new ArgumentNullException("stream"));
			}
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			VersionedXmlBase.Serialize(stream, obj);
			try
			{
				stream.SetLength(stream.Position);
			}
			catch (NotSupportedException)
			{
			}
		}

		internal static VersionedXmlBase Deserialize(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("stream");
			}
			long position = stream.Position;
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
			string text = null;
			Version version = null;
			using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
			{
				while (xmlReader.Read())
				{
					if (XmlNodeType.Element == xmlReader.NodeType)
					{
						text = xmlReader.Name;
						string attribute = xmlReader.GetAttribute("Version");
						if (!string.IsNullOrEmpty(attribute))
						{
							version = new Version(attribute);
							break;
						}
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(text) || null == version)
			{
				return null;
			}
			stream.Seek(position, SeekOrigin.Begin);
			return (VersionedXmlBase)VersionedXmlBase.Deserialize(VersionedXmlTypeFactory.GetTypeInstance(text, version), stream);
		}

		private static string GetXmlString(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				VersionedXmlBase.Serialize(memoryStream, obj);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		private static string GetBase64XmlString(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				VersionedXmlBase.Serialize(memoryStream, obj);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		public static VersionedXmlBase Parse(string xml)
		{
			if (string.IsNullOrEmpty(xml))
			{
				throw new CustomSerializationException(ServerStrings.ErrorInvalidConfigurationXml, new ArgumentNullException("xml"));
			}
			VersionedXmlBase result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
			{
				result = VersionedXmlBase.Deserialize(memoryStream);
			}
			return result;
		}

		public static VersionedXmlBase ParseFromBase64(string base64)
		{
			if (string.IsNullOrEmpty(base64))
			{
				throw new CustomSerializationException(ServerStrings.ErrorInvalidConfigurationXml, new ArgumentNullException("base64"));
			}
			VersionedXmlBase result;
			using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64)))
			{
				result = VersionedXmlBase.Deserialize(memoryStream);
			}
			return result;
		}

		public override string ToString()
		{
			return VersionedXmlBase.GetXmlString(this);
		}

		public string ToBase64String()
		{
			return VersionedXmlBase.GetBase64XmlString(this);
		}

		private static void Serialize(Stream stream, object obj)
		{
			VersionedXmlTypeFactory.GetXmlSerializer(obj.GetType()).Serialize(stream, obj);
		}

		private static object Deserialize(Type type, Stream stream)
		{
			XmlReaderSettings xmlReaderSettings = null;
			XmlSchema xmlSchema = null;
			XmlSerializer xmlSerializer = VersionedXmlTypeFactory.GetXmlSerializer(type, out xmlSchema);
			if (xmlSchema != null)
			{
				xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ValidationFlags = (XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.AllowXmlAttributes);
				xmlReaderSettings.Schemas.Add(xmlSchema);
			}
			object result;
			using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
			{
				result = xmlSerializer.Deserialize(xmlReader);
			}
			return result;
		}
	}
}
