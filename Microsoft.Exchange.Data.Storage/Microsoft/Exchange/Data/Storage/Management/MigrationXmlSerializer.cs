using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MigrationXmlSerializer
	{
		private static XmlWriterSettings XmlWriterSettings
		{
			get
			{
				if (MigrationXmlSerializer.xmlWriterSettings == null)
				{
					MigrationXmlSerializer.xmlWriterSettings = new XmlWriterSettings
					{
						OmitXmlDeclaration = true,
						Encoding = new UTF8Encoding(false),
						CheckCharacters = false
					};
				}
				return MigrationXmlSerializer.xmlWriterSettings;
			}
		}

		private static XmlReaderSettings XmlReaderSettings
		{
			get
			{
				if (MigrationXmlSerializer.xmlReaderSettings == null)
				{
					MigrationXmlSerializer.xmlReaderSettings = new XmlReaderSettings
					{
						CheckCharacters = false,
						DtdProcessing = DtdProcessing.Ignore,
						XmlResolver = null
					};
				}
				return MigrationXmlSerializer.xmlReaderSettings;
			}
		}

		public static string Serialize(object obj)
		{
			Util.ThrowOnNullArgument(obj, "obj");
			StringBuilder stringBuilder = new StringBuilder(2048);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, MigrationXmlSerializer.XmlWriterSettings))
			{
				try
				{
					IXmlSerializable xmlSerializable = obj as IXmlSerializable;
					if (xmlSerializable != null)
					{
						xmlSerializable.WriteXml(xmlWriter);
					}
					else
					{
						XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
						xmlSerializer.Serialize(xmlWriter, obj);
					}
				}
				catch (InvalidOperationException innerException)
				{
					throw new MigrationDataCorruptionException(string.Format("Couldn't serialize object of type {0}", obj.GetType()), innerException);
				}
				catch (ArgumentException innerException2)
				{
					throw new MigrationDataCorruptionException(string.Format("Couldn't serialize object of type {0}", obj.GetType()), innerException2);
				}
			}
			return stringBuilder.ToString();
		}

		public static T Deserialize<T>(string rawXml) where T : IXmlSerializable, new()
		{
			Util.ThrowOnNullOrEmptyArgument(rawXml, "rawXml");
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			MigrationXmlSerializer.Deserialize(rawXml, new Action<XmlReader>(t.ReadXml));
			return t;
		}

		public static object Deserialize(string rawXml, Type type)
		{
			Util.ThrowOnNullOrEmptyArgument(rawXml, "rawXml");
			Util.ThrowOnNullArgument(type, "type");
			object obj = null;
			MigrationXmlSerializer.Deserialize(rawXml, delegate(XmlReader xmlReader)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(type);
				obj = xmlSerializer.Deserialize(xmlReader);
			});
			if (obj == null)
			{
				throw new MigrationDataCorruptionException("couldn't deserialize object of type " + type);
			}
			return obj;
		}

		private static void Deserialize(string rawXml, Action<XmlReader> deserialize)
		{
			using (StringReader stringReader = new StringReader(rawXml))
			{
				using (XmlReader xmlReader = XmlReader.Create(stringReader, MigrationXmlSerializer.XmlReaderSettings))
				{
					try
					{
						deserialize(xmlReader);
					}
					catch (InvalidOperationException innerException)
					{
						string internalDetails = string.Format("Couldn't deserialize string: '{0}'", (rawXml.Length > 128) ? (rawXml.Substring(0, 128) + "...") : rawXml);
						throw new MigrationDataCorruptionException(internalDetails, innerException);
					}
				}
			}
		}

		private const int DefaultXmlSize = 2048;

		private static XmlWriterSettings xmlWriterSettings;

		private static XmlReaderSettings xmlReaderSettings;
	}
}
