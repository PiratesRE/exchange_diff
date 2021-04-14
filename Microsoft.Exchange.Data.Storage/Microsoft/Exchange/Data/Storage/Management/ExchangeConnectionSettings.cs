using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class ExchangeConnectionSettings : ConnectionSettingsBase
	{
		public ExchangeConnectionSettings()
		{
			this[SimpleProviderObjectSchema.Identity] = MigrationBatchId.Any;
		}

		private ExchangeConnectionSettings(SmtpAddress incomingEmailAddress, string userName, string domain, string encryptedPassword, bool useAutodiscovery, string incomingRPCProxyServer, string incomingExchangeServer, AuthenticationMethod incomingAuthentication, bool hasAdminPrivilege, bool hasMrsProxy) : this()
		{
			this.IncomingUserName = userName;
			this.IncomingDomain = domain;
			this.EncryptedIncomingPassword = encryptedPassword;
			this.HasAutodiscovery = useAutodiscovery;
			this.IncomingEmailAddress = incomingEmailAddress;
			this.IncomingRPCProxyServer = incomingRPCProxyServer;
			this.IncomingExchangeServer = incomingExchangeServer;
			this.IncomingAuthentication = incomingAuthentication;
			this.HasAdminPrivilege = hasAdminPrivilege;
			this.HasMrsProxy = hasMrsProxy;
		}

		public static ExchangeConnectionSettings Create(string userName, string domain, string encryptedPassword, string incomingRpcProxyServer, string incomingExchangeServer, AuthenticationMethod incomingAuthentication, bool hasAdminPrivilege)
		{
			return ExchangeConnectionSettings.Create(userName, domain, encryptedPassword, incomingRpcProxyServer, incomingExchangeServer, incomingAuthentication, hasAdminPrivilege, false);
		}

		public static ExchangeConnectionSettings Create(string userName, string domain, string encryptedPassword, string incomingRpcProxyServer, string incomingExchangeServer, AuthenticationMethod incomingAuthentication, bool hasAdminPrivilege, bool hasMrsProxy)
		{
			return new ExchangeConnectionSettings(SmtpAddress.Empty, userName, domain, encryptedPassword, false, incomingRpcProxyServer, incomingExchangeServer, incomingAuthentication, hasAdminPrivilege, hasMrsProxy);
		}

		public static ExchangeConnectionSettings Create(string userName, string domain, string encryptedPassword, SmtpAddress incomingEmailAddress, string incomingRpcProxyServer, string incomingExchangeServer, AuthenticationMethod incomingAuthentication, bool hasAdminPrivilege)
		{
			return new ExchangeConnectionSettings(incomingEmailAddress, userName, domain, encryptedPassword, true, incomingRpcProxyServer, incomingExchangeServer, incomingAuthentication, hasAdminPrivilege, false);
		}

		public static ExchangeConnectionSettings Create(string userName, string domain, string encryptedPassword, string incomingRpcProxyServer, string sourceMailboxLegDn, string publicFolderDatabaseServerLegacyDN, AuthenticationMethod incomingAuthentication)
		{
			return new ExchangeConnectionSettings(SmtpAddress.Empty, userName, domain, encryptedPassword, false, incomingRpcProxyServer, null, incomingAuthentication, true, false)
			{
				SourceMailboxLegDn = sourceMailboxLegDn,
				PublicFolderDatabaseServerLegacyDN = publicFolderDatabaseServerLegacyDN
			};
		}

		public override MigrationType Type
		{
			get
			{
				if (this.HasMrsProxy)
				{
					return MigrationType.ExchangeRemoteMove;
				}
				if (!string.IsNullOrEmpty(this.PublicFolderDatabaseServerLegacyDN))
				{
					return MigrationType.PublicFolder;
				}
				return MigrationType.ExchangeOutlookAnywhere;
			}
		}

		public bool HasAdminPrivilege
		{
			get
			{
				return (bool)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.HasAdminPrivilege];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.HasAdminPrivilege] = value;
			}
		}

		public bool HasAutodiscovery
		{
			get
			{
				return (bool)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.HasAutodiscovery];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.HasAutodiscovery] = value;
			}
		}

		public string IncomingRPCProxyServer
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingRPCProxyServer];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingRPCProxyServer] = value;
			}
		}

		public string IncomingExchangeServer
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingExchangeServer];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingExchangeServer] = value;
			}
		}

		public string IncomingNSPIServer
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingNSPIServer];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingNSPIServer] = value;
			}
		}

		public string IncomingDomain
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingDomain];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingDomain] = value;
			}
		}

		public string IncomingUserName
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingUserName];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingUserName] = value;
			}
		}

		public string EncryptedIncomingPassword
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.EncryptedIncomingPassword];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.EncryptedIncomingPassword] = value;
			}
		}

		public string AutodiscoverUrl
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.AutodiscoverUrl];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.AutodiscoverUrl] = value;
			}
		}

		public SmtpAddress IncomingEmailAddress
		{
			get
			{
				return (SmtpAddress)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingEmailAddress];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingEmailAddress] = value;
			}
		}

		public AuthenticationMethod IncomingAuthentication
		{
			get
			{
				return (AuthenticationMethod)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingAuthentication];
			}
			set
			{
				if (value != AuthenticationMethod.Basic && value != AuthenticationMethod.Ntlm)
				{
					throw new MigrationPermanentException(ServerStrings.MigrationJobConnectionSettingsInvalid("IncomingAuthentication", value.ToString()));
				}
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.IncomingAuthentication] = value;
			}
		}

		public string ServerVersion
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.ServerVersion];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.ServerVersion] = value;
			}
		}

		public string SourceMailboxLegDn
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.SourceMailboxLegDn];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.SourceMailboxLegDn] = value;
			}
		}

		public string PublicFolderDatabaseServerLegacyDN
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.PublicFolderDatabaseServerLegacyDN];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.PublicFolderDatabaseServerLegacyDN] = value;
			}
		}

		public string TargetDomainName
		{
			get
			{
				return (string)this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.TargetDomainName];
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.TargetDomainName] = value;
			}
		}

		public bool HasMrsProxy
		{
			get
			{
				return (bool)(this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.HasMrsProxy] ?? false);
			}
			set
			{
				this[ExchangeConnectionSettings.ExchangeConnectionSettingsSchema.HasMrsProxy] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ExchangeConnectionSettings.schema;
			}
		}

		public new static implicit operator ExchangeConnectionSettings(string xml)
		{
			ExchangeConnectionSettings result;
			try
			{
				ExchangeConnectionSettings exchangeConnectionSettings = MigrationXmlSerializer.Deserialize<ExchangeConnectionSettings>(xml);
				result = exchangeConnectionSettings;
			}
			catch (MigrationDataCorruptionException ex)
			{
				throw new CouldNotDeserializeConnectionSettingsException(ex.InnerException);
			}
			return result;
		}

		public override ConnectionSettingsBase CloneForPresentation()
		{
			return new ExchangeConnectionSettings
			{
				HasAdminPrivilege = this.HasAdminPrivilege,
				HasAutodiscovery = this.HasAutodiscovery,
				AutodiscoverUrl = this.AutodiscoverUrl,
				IncomingEmailAddress = this.IncomingEmailAddress,
				IncomingRPCProxyServer = this.IncomingRPCProxyServer,
				IncomingExchangeServer = this.IncomingExchangeServer,
				IncomingNSPIServer = this.IncomingNSPIServer,
				IncomingDomain = this.IncomingDomain,
				IncomingUserName = this.IncomingUserName,
				EncryptedIncomingPassword = this.EncryptedIncomingPassword,
				IncomingAuthentication = this.IncomingAuthentication,
				ServerVersion = this.ServerVersion,
				TargetDomainName = this.TargetDomainName,
				HasMrsProxy = this.HasMrsProxy,
				SourceMailboxLegDn = this.SourceMailboxLegDn,
				PublicFolderDatabaseServerLegacyDN = this.PublicFolderDatabaseServerLegacyDN
			};
		}

		public override void ReadXml(XmlReader reader)
		{
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ExchangeConnectionSettings")
			{
				this.HasAdminPrivilege = bool.Parse(reader["HasAdminPrivilege"]);
				this.HasAutodiscovery = bool.Parse(reader["HasAutodiscovery"]);
				this.HasMrsProxy = bool.Parse(reader["HasMrsProxy"] ?? false.ToString(CultureInfo.InvariantCulture));
				this.AutodiscoverUrl = reader["AutodiscoverUrl"];
				this.IncomingEmailAddress = new SmtpAddress(reader["IncomingEmailAddress"]);
				this.IncomingRPCProxyServer = reader["IncomingRPCProxyServer"];
				this.IncomingExchangeServer = reader["IncomingExchangeServer"];
				this.IncomingNSPIServer = reader["IncomingNSPIServer"];
				this.IncomingDomain = reader["IncomingDomain"];
				this.IncomingUserName = reader["IncomingUserName"];
				this.EncryptedIncomingPassword = reader["EncryptedIncomingPassword"];
				this.IncomingAuthentication = (AuthenticationMethod)Enum.Parse(typeof(AuthenticationMethod), reader["IncomingAuthentication"]);
				this.SourceMailboxLegDn = reader["SourceMailboxLegDn"];
				this.PublicFolderDatabaseServerLegacyDN = reader["PublicFolderDatabaseServerLegacyDN"];
				this.ServerVersion = reader["ServerVersion"];
				this.TargetDomainName = reader["TargetDomainName"];
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("ExchangeConnectionSettings");
			writer.WriteAttributeString("HasAdminPrivilege", this.HasAdminPrivilege.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("HasAutodiscovery", this.HasAutodiscovery.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("HasMrsProxy", this.HasMrsProxy.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("AutodiscoverUrl", this.AutodiscoverUrl);
			writer.WriteAttributeString("IncomingEmailAddress", this.IncomingEmailAddress.ToString());
			writer.WriteAttributeString("IncomingRPCProxyServer", this.IncomingRPCProxyServer);
			writer.WriteAttributeString("IncomingExchangeServer", this.IncomingExchangeServer);
			writer.WriteAttributeString("IncomingNSPIServer", this.IncomingNSPIServer);
			writer.WriteAttributeString("IncomingDomain", this.IncomingDomain);
			writer.WriteAttributeString("IncomingUserName", this.IncomingUserName);
			writer.WriteAttributeString("EncryptedIncomingPassword", this.EncryptedIncomingPassword);
			writer.WriteAttributeString("IncomingAuthentication", this.IncomingAuthentication.ToString());
			writer.WriteAttributeString("ServerVersion", this.ServerVersion);
			writer.WriteAttributeString("TargetDomainName", this.TargetDomainName);
			writer.WriteAttributeString("SourceMailboxLegDn", this.SourceMailboxLegDn);
			writer.WriteAttributeString("PublicFolderDatabaseServerLegacyDN", this.PublicFolderDatabaseServerLegacyDN);
			writer.WriteEndElement();
		}

		public override bool Equals(object obj)
		{
			ExchangeConnectionSettings exchangeConnectionSettings = obj as ExchangeConnectionSettings;
			if (exchangeConnectionSettings == null)
			{
				return false;
			}
			bool flag = this.HasMrsProxy == exchangeConnectionSettings.HasMrsProxy;
			flag = (flag && this.IncomingAuthentication == exchangeConnectionSettings.IncomingAuthentication);
			flag = (flag && this.HasAutodiscovery == exchangeConnectionSettings.HasAutodiscovery);
			flag = (flag && ExchangeConnectionSettings.AreStringEqual(this.AutodiscoverUrl, exchangeConnectionSettings.AutodiscoverUrl));
			flag = (flag && ExchangeConnectionSettings.AreStringEqual(this.IncomingDomain, exchangeConnectionSettings.IncomingDomain));
			flag = (flag && this.IncomingEmailAddress.Equals(exchangeConnectionSettings.IncomingEmailAddress));
			if (!this.HasMrsProxy)
			{
				flag = (flag && ExchangeConnectionSettings.AreStringEqual(this.IncomingExchangeServer, exchangeConnectionSettings.IncomingExchangeServer));
				flag = (flag && ExchangeConnectionSettings.AreStringEqual(this.IncomingNSPIServer, exchangeConnectionSettings.IncomingNSPIServer));
				flag = (flag && ExchangeConnectionSettings.AreStringEqual(this.SourceMailboxLegDn, exchangeConnectionSettings.SourceMailboxLegDn));
				flag = (flag && ExchangeConnectionSettings.AreStringEqual(this.PublicFolderDatabaseServerLegacyDN, exchangeConnectionSettings.PublicFolderDatabaseServerLegacyDN));
			}
			flag = (flag && ExchangeConnectionSettings.AreStringEqual(this.IncomingRPCProxyServer, exchangeConnectionSettings.IncomingRPCProxyServer));
			flag = (flag && ExchangeConnectionSettings.AreStringEqual(this.IncomingUserName, exchangeConnectionSettings.IncomingUserName));
			return flag && ExchangeConnectionSettings.AreStringEqual(this.TargetDomainName, exchangeConnectionSettings.TargetDomainName);
		}

		public override int GetHashCode()
		{
			int num = 31;
			num ^= (this.HasMrsProxy ? 47 : 0);
			num ^= (this.HasAutodiscovery ? 109 : 0);
			if (!this.HasMrsProxy)
			{
				num ^= ExchangeConnectionSettings.SafeGetHashCode(this.IncomingExchangeServer);
				num ^= ExchangeConnectionSettings.SafeGetHashCode(this.IncomingNSPIServer);
				num ^= ExchangeConnectionSettings.SafeGetHashCode(this.SourceMailboxLegDn);
			}
			num ^= ExchangeConnectionSettings.SafeGetHashCode(this.IncomingAuthentication);
			num ^= ExchangeConnectionSettings.SafeGetHashCode(this.AutodiscoverUrl);
			num ^= ExchangeConnectionSettings.SafeGetHashCode(this.IncomingDomain);
			num ^= ExchangeConnectionSettings.SafeGetHashCode(this.IncomingEmailAddress);
			num ^= ExchangeConnectionSettings.SafeGetHashCode(this.IncomingRPCProxyServer);
			num ^= ExchangeConnectionSettings.SafeGetHashCode(this.IncomingUserName);
			return num ^ ExchangeConnectionSettings.SafeGetHashCode(this.TargetDomainName);
		}

		private static bool AreStringEqual(string first, string second)
		{
			return (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second)) || StringComparer.OrdinalIgnoreCase.Equals(first, second);
		}

		private static int SafeGetHashCode(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetHashCode();
		}

		private const string RootSerializedTag = "ExchangeConnectionSettings";

		private static ObjectSchema schema = ObjectSchema.GetInstance<ExchangeConnectionSettings.ExchangeConnectionSettingsSchema>();

		private class ExchangeConnectionSettingsSchema : SimpleProviderObjectSchema
		{
			public static readonly SimpleProviderPropertyDefinition HasAdminPrivilege = new SimpleProviderPropertyDefinition("HasAdminPrivilege", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition HasAutodiscovery = new SimpleProviderPropertyDefinition("HasAutodiscovery", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition AutodiscoverUrl = new SimpleProviderPropertyDefinition("AutodiscoverUrl", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IncomingEmailAddress = new SimpleProviderPropertyDefinition("IncomingEmailAddress", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), PropertyDefinitionFlags.TaskPopulated, SmtpAddress.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IncomingRPCProxyServer = new SimpleProviderPropertyDefinition("IncomingRPCProxyServer", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IncomingExchangeServer = new SimpleProviderPropertyDefinition("IncomingExchangeServer", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IncomingNSPIServer = new SimpleProviderPropertyDefinition("IncomingNSPIServer", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IncomingDomain = new SimpleProviderPropertyDefinition("IncomingDomain", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IncomingUserName = new SimpleProviderPropertyDefinition("IncomingUserName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition EncryptedIncomingPassword = new SimpleProviderPropertyDefinition("EncryptedIncomingPassword", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IncomingAuthentication = new SimpleProviderPropertyDefinition("IncomingAuthentication", ExchangeObjectVersion.Exchange2010, typeof(AuthenticationMethod), PropertyDefinitionFlags.TaskPopulated, AuthenticationMethod.Basic, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition ServerVersion = new SimpleProviderPropertyDefinition("ServerVersion", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition SourceMailboxLegDn = new SimpleProviderPropertyDefinition("SourceMailboxLegDn", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition PublicFolderDatabaseServerLegacyDN = new SimpleProviderPropertyDefinition("PublicFolderDatabaseServerLegacyDN", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition TargetDomainName = new SimpleProviderPropertyDefinition("TargetDomainName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition HasMrsProxy = new SimpleProviderPropertyDefinition("HasMrsProxy", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
