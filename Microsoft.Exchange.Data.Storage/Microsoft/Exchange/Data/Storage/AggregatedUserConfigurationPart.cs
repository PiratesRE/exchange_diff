using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class AggregatedUserConfigurationPart : IReadableUserConfiguration, IDisposable
	{
		private AggregatedUserConfigurationPart(AggregatedUserConfigurationPart.MementoClass memento)
		{
			this.memento = memento;
		}

		private AggregatedUserConfigurationPart(IUserConfiguration config)
		{
			this.memento = new AggregatedUserConfigurationPart.MementoClass
			{
				ConfigurationName = config.ConfigurationName,
				DataTypes = config.DataTypes,
				FolderId = config.FolderId,
				Id = config.Id,
				VersionedId = config.VersionedId,
				LastModifiedTime = config.LastModifiedTime.ToBinary()
			};
			if ((config.DataTypes & UserConfigurationTypes.Dictionary) != (UserConfigurationTypes)0)
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					using (XmlWriter xmlWriter = AggregatedUserConfigurationPart.InternalGetXmlWriter(stringWriter))
					{
						config.GetConfigurationDictionary().WriteXml(xmlWriter);
						xmlWriter.Flush();
						this.memento.DictionaryXmlString = stringWriter.ToString();
					}
				}
			}
			if ((config.DataTypes & UserConfigurationTypes.XML) != (UserConfigurationTypes)0)
			{
				using (Stream xmlStream = config.GetXmlStream())
				{
					using (StreamReader streamReader = new StreamReader(xmlStream))
					{
						this.memento.XmlString = streamReader.ReadToEnd();
					}
				}
			}
		}

		public AggregatedUserConfigurationPart.MementoClass Memento
		{
			get
			{
				return this.memento;
			}
		}

		public string ConfigurationName
		{
			get
			{
				return this.Memento.ConfigurationName;
			}
		}

		public UserConfigurationTypes DataTypes
		{
			get
			{
				return this.Memento.DataTypes;
			}
		}

		public StoreObjectId FolderId
		{
			get
			{
				return this.Memento.FolderId;
			}
		}

		public StoreObjectId Id
		{
			get
			{
				return this.Memento.Id;
			}
		}

		public VersionedId VersionedId
		{
			get
			{
				return this.Memento.VersionedId;
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				return ExDateTime.FromBinary(this.Memento.LastModifiedTime);
			}
		}

		public static AggregatedUserConfigurationPart FromMemento(AggregatedUserConfigurationPart.MementoClass memento)
		{
			return new AggregatedUserConfigurationPart(memento);
		}

		public static AggregatedUserConfigurationPart FromConfiguration(IUserConfiguration configuration)
		{
			return new AggregatedUserConfigurationPart(configuration);
		}

		public void Dispose()
		{
		}

		public IDictionary GetDictionary()
		{
			IDictionary result;
			using (StringReader stringReader = new StringReader(this.Memento.DictionaryXmlString))
			{
				using (XmlReader xmlReader = AggregatedUserConfigurationPart.InternalGetXmlReader(stringReader))
				{
					ConfigurationDictionary configurationDictionary = new ConfigurationDictionary();
					configurationDictionary.ReadXml(xmlReader);
					result = configurationDictionary;
				}
			}
			return result;
		}

		public Stream GetXmlStream()
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(this.Memento.XmlString);
			streamWriter.Flush();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		}

		private static XmlWriter InternalGetXmlWriter(TextWriter writer)
		{
			return new XmlTextWriter(writer);
		}

		private static XmlReader InternalGetXmlReader(TextReader reader)
		{
			return SafeXmlFactory.CreateSafeXmlTextReader(reader);
		}

		private readonly AggregatedUserConfigurationPart.MementoClass memento;

		[DataContract]
		public class MementoClass
		{
			[DataMember]
			public string ConfigurationName { get; set; }

			[DataMember]
			public UserConfigurationTypes DataTypes { get; set; }

			[DataMember]
			public StoreObjectId FolderId { get; set; }

			[DataMember]
			public StoreObjectId Id { get; set; }

			[DataMember]
			public VersionedId VersionedId { get; set; }

			[DataMember]
			public long LastModifiedTime { get; set; }

			[DataMember]
			public string DictionaryXmlString { get; set; }

			[DataMember]
			public string XmlString { get; set; }
		}
	}
}
