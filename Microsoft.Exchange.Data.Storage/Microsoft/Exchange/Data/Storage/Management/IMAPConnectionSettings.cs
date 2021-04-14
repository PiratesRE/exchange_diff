using System;
using System.Xml;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class IMAPConnectionSettings : ConnectionSettingsBase
	{
		public IMAPConnectionSettings()
		{
			this[SimpleProviderObjectSchema.Identity] = MigrationBatchId.Any;
		}

		public override MigrationType Type
		{
			get
			{
				return MigrationType.IMAP;
			}
		}

		public string Server
		{
			get
			{
				return (string)this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.Server];
			}
			set
			{
				this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.Server] = value;
			}
		}

		public int Port
		{
			get
			{
				return (int)this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.Port];
			}
			set
			{
				this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.Port] = value;
			}
		}

		public IMAPAuthenticationMechanism Authentication
		{
			get
			{
				return (IMAPAuthenticationMechanism)this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.Authentication];
			}
			set
			{
				this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.Authentication] = value;
			}
		}

		public IMAPSecurityMechanism Security
		{
			get
			{
				return (IMAPSecurityMechanism)this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.Security];
			}
			set
			{
				this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.Security] = value;
			}
		}

		public MultiValuedProperty<string> ExcludedFolders
		{
			get
			{
				return (MultiValuedProperty<string>)this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.ExcludedFolders];
			}
			set
			{
				this[IMAPConnectionSettings.IMAPConnectionSettingsSchema.ExcludedFolders] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return IMAPConnectionSettings.schema;
			}
		}

		public new static implicit operator IMAPConnectionSettings(string xml)
		{
			IMAPConnectionSettings result;
			try
			{
				IMAPConnectionSettings imapconnectionSettings = MigrationXmlSerializer.Deserialize<IMAPConnectionSettings>(xml);
				result = imapconnectionSettings;
			}
			catch (MigrationDataCorruptionException ex)
			{
				throw new CouldNotDeserializeConnectionSettingsException(ex.InnerException);
			}
			return result;
		}

		public override ConnectionSettingsBase CloneForPresentation()
		{
			return new IMAPConnectionSettings
			{
				Server = this.Server,
				Port = this.Port,
				Authentication = this.Authentication,
				Security = this.Security,
				ExcludedFolders = this.ExcludedFolders
			};
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "IMAPConnectionSettings")
			{
				this.Server = reader["Server"];
				this.Port = int.Parse(reader["Port"]);
				this.Authentication = (IMAPAuthenticationMechanism)Enum.Parse(typeof(IMAPAuthenticationMechanism), reader["Authentication"]);
				this.Security = (IMAPSecurityMechanism)Enum.Parse(typeof(IMAPSecurityMechanism), reader["Security"]);
				while (reader.LocalName == "ExcludedFolders" || reader.ReadToFollowing("ExcludedFolders"))
				{
					string text = reader.ReadElementContentAsString();
					if (!string.IsNullOrEmpty(text))
					{
						this.ExcludedFolders.TryAdd(text);
					}
				}
			}
		}

		public override bool Equals(object obj)
		{
			IMAPConnectionSettings imapconnectionSettings = obj as IMAPConnectionSettings;
			return imapconnectionSettings != null && (this.Security == imapconnectionSettings.Security && StringComparer.InvariantCultureIgnoreCase.Equals(this.Server, imapconnectionSettings.Server) && this.Authentication == imapconnectionSettings.Authentication) && this.Port == imapconnectionSettings.Port;
		}

		public override int GetHashCode()
		{
			return ((this.Server == null) ? 0 : this.Server.GetHashCode()) ^ this.Port ^ this.Security.GetHashCode() ^ this.Authentication.GetHashCode();
		}

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("IMAPConnectionSettings");
			writer.WriteAttributeString("Server", this.Server);
			writer.WriteAttributeString("Port", this.Port.ToString());
			writer.WriteAttributeString("Authentication", this.Authentication.ToString());
			writer.WriteAttributeString("Security", this.Security.ToString());
			foreach (string value in this.ExcludedFolders)
			{
				writer.WriteElementString("ExcludedFolders", value);
			}
			writer.WriteEndElement();
		}

		private const string RootSerializedTag = "IMAPConnectionSettings";

		private static ObjectSchema schema = ObjectSchema.GetInstance<IMAPConnectionSettings.IMAPConnectionSettingsSchema>();

		private class IMAPConnectionSettingsSchema : SimpleProviderObjectSchema
		{
			public static readonly SimpleProviderPropertyDefinition Server = new SimpleProviderPropertyDefinition("Server", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition Port = new SimpleProviderPropertyDefinition("Port", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition Authentication = new SimpleProviderPropertyDefinition("IMAPAuthenticationMechanism", ExchangeObjectVersion.Exchange2010, typeof(IMAPAuthenticationMechanism), PropertyDefinitionFlags.TaskPopulated, IMAPAuthenticationMechanism.Basic, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition Security = new SimpleProviderPropertyDefinition("IMAPSecurityMechanism", ExchangeObjectVersion.Exchange2010, typeof(IMAPSecurityMechanism), PropertyDefinitionFlags.TaskPopulated, IMAPSecurityMechanism.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition ExcludedFolders = new SimpleProviderPropertyDefinition("ExcludedFolders", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
