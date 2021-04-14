using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationReportSet : ConfigurableObject, IXmlSerializable
	{
		public MigrationReportSet(DateTime creationTimeUTC, string successUrl, string errorUrl = null) : this()
		{
			this.CreationTimeUTC = creationTimeUTC;
			this.SuccessUrl = successUrl;
			this.ErrorUrl = errorUrl;
		}

		public MigrationReportSet() : base(new SimpleProviderPropertyBag())
		{
		}

		public static implicit operator MigrationReportSet(string xml)
		{
			return (MigrationReportSet)MigrationXmlSerializer.Deserialize(xml, typeof(MigrationReportSet));
		}

		public DateTime CreationTimeUTC
		{
			get
			{
				return (DateTime)this[MigrationReportSet.MigrationReportSetSchema.CreationTimeUTC];
			}
			private set
			{
				this[MigrationReportSet.MigrationReportSetSchema.CreationTimeUTC] = value;
			}
		}

		public string SuccessUrl
		{
			get
			{
				return (string)this[MigrationReportSet.MigrationReportSetSchema.SuccessUrl];
			}
			private set
			{
				this[MigrationReportSet.MigrationReportSetSchema.SuccessUrl] = value;
			}
		}

		public string ErrorUrl
		{
			get
			{
				return (string)this[MigrationReportSet.MigrationReportSetSchema.ErrorUrl];
			}
			private set
			{
				this[MigrationReportSet.MigrationReportSetSchema.ErrorUrl] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MigrationReportSet.schema;
			}
		}

		public static bool TryCreate(XmlReader reader, out MigrationReportSet report)
		{
			report = null;
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "MigrationReportSet")
			{
				report = new MigrationReportSet();
				report.Initialize(reader);
				return true;
			}
			return false;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "MigrationReportSet")
			{
				this.Initialize(reader);
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("MigrationReportSet");
			writer.WriteAttributeString(MigrationReportSet.MigrationReportSetSchema.SuccessUrl.Name, this.SuccessUrl);
			writer.WriteAttributeString(MigrationReportSet.MigrationReportSetSchema.ErrorUrl.Name, this.ErrorUrl);
			writer.WriteAttributeString(MigrationReportSet.MigrationReportSetSchema.CreationTimeUTC.Name, this.CreationTimeUTC.Ticks.ToString());
			writer.WriteEndElement();
		}

		public override string ToString()
		{
			return Strings.MigrationReportSetString(this.CreationTimeUTC.ToString(), this.SuccessUrl, this.ErrorUrl);
		}

		private void Initialize(XmlReader reader)
		{
			this.SuccessUrl = reader[MigrationReportSet.MigrationReportSetSchema.SuccessUrl.Name];
			this.ErrorUrl = reader[MigrationReportSet.MigrationReportSetSchema.ErrorUrl.Name];
			string text = reader[MigrationReportSet.MigrationReportSetSchema.CreationTimeUTC.Name];
			try
			{
				long ticks = long.Parse(text);
				this.CreationTimeUTC = new DateTime(ticks);
			}
			catch (ArgumentException innerException)
			{
				throw new MigrationDataCorruptionException("cannot parse xml date:" + text, innerException);
			}
			catch (FormatException innerException2)
			{
				throw new MigrationDataCorruptionException("cannot parse xml date:" + text, innerException2);
			}
			catch (OverflowException innerException3)
			{
				throw new MigrationDataCorruptionException("cannot parse xml date:" + text, innerException3);
			}
		}

		public const string RootSerializedTag = "MigrationReportSet";

		private static ObjectSchema schema = ObjectSchema.GetInstance<MigrationReportSet.MigrationReportSetSchema>();

		private class MigrationReportSetSchema : SimpleProviderObjectSchema
		{
			public static readonly SimpleProviderPropertyDefinition CreationTimeUTC = new SimpleProviderPropertyDefinition("CreationTimeUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.TaskPopulated, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition ErrorUrl = new SimpleProviderPropertyDefinition("ErrorUrl", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition SuccessUrl = new SimpleProviderPropertyDefinition("SuccessUrl", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
