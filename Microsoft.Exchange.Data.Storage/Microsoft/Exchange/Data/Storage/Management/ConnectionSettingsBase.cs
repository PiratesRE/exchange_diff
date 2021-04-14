using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public abstract class ConnectionSettingsBase : ConfigurableObject, IXmlSerializable
	{
		public ConnectionSettingsBase() : base(new SimpleProviderPropertyBag())
		{
		}

		public abstract MigrationType Type { get; }

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public static implicit operator ConnectionSettingsBase(string xml)
		{
			Dictionary<string, Func<string, object>> dictionary = new Dictionary<string, Func<string, object>>
			{
				{
					typeof(ExchangeConnectionSettings).Name,
					new Func<string, object>(MigrationXmlSerializer.Deserialize<ExchangeConnectionSettings>)
				},
				{
					typeof(IMAPConnectionSettings).Name,
					new Func<string, object>(MigrationXmlSerializer.Deserialize<IMAPConnectionSettings>)
				}
			};
			ConnectionSettingsBase result;
			using (StringReader stringReader = new StringReader(xml))
			{
				using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(stringReader))
				{
					xmlTextReader.MoveToContent();
					Func<string, object> func;
					if (!dictionary.TryGetValue(xmlTextReader.LocalName, out func))
					{
						throw new UnknownConnectionSettingsTypeException(xmlTextReader.LocalName);
					}
					result = (ConnectionSettingsBase)func(xml);
				}
			}
			return result;
		}

		public abstract ConnectionSettingsBase CloneForPresentation();

		public XmlSchema GetSchema()
		{
			return null;
		}

		public override string ToString()
		{
			return MigrationXmlSerializer.Serialize(this);
		}

		public abstract void ReadXml(XmlReader reader);

		public abstract void WriteXml(XmlWriter writer);
	}
}
