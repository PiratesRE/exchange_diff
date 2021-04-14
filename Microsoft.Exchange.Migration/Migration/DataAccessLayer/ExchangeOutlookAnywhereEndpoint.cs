using System;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;

namespace Microsoft.Exchange.Migration.DataAccessLayer
{
	internal class ExchangeOutlookAnywhereEndpoint : MigrationEndpointBase
	{
		public ExchangeOutlookAnywhereEndpoint(MigrationEndpoint presentationObject) : base(presentationObject)
		{
		}

		public ExchangeOutlookAnywhereEndpoint() : base(MigrationType.ExchangeOutlookAnywhere)
		{
		}

		public string ExchangeServer { get; set; }

		public Fqdn RpcProxyServer
		{
			get
			{
				return this.RemoteServer;
			}
			set
			{
				this.RemoteServer = value;
			}
		}

		public string NspiServer
		{
			get
			{
				return base.ExtendedProperties.Get<string>("NspiServer");
			}
			set
			{
				base.ExtendedProperties.Set<string>("NspiServer", value);
			}
		}

		public string EmailAddress
		{
			get
			{
				return base.ExtendedProperties.Get<string>("EmailAddress");
			}
			set
			{
				base.ExtendedProperties.Set<string>("EmailAddress", value);
			}
		}

		public bool UseAutoDiscover
		{
			get
			{
				return base.ExtendedProperties.Get<bool>("UseAutoDiscover", false);
			}
			set
			{
				base.ExtendedProperties.Set<bool>("UseAutoDiscover", value);
			}
		}

		public MigrationMailboxPermission MailboxPermission
		{
			get
			{
				return base.ExtendedProperties.Get<MigrationMailboxPermission>("MailboxPermission", MigrationMailboxPermission.Admin);
			}
			set
			{
				base.ExtendedProperties.Set<MigrationMailboxPermission>("MailboxPermission", value);
			}
		}

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				PropertyDefinition[] second = new StorePropertyDefinition[]
				{
					MigrationEndpointMessageSchema.ExchangeServer
				};
				return base.PropertyDefinitions.Union(second).ToArray<PropertyDefinition>();
			}
		}

		public override ConnectionSettingsBase ConnectionSettings
		{
			get
			{
				ExchangeConnectionSettings exchangeConnectionSettings;
				if (this.UseAutoDiscover)
				{
					exchangeConnectionSettings = ExchangeConnectionSettings.Create(base.Username, base.Domain, base.EncryptedPassword, (SmtpAddress)this.EmailAddress, this.RpcProxyServer.ToString(), this.ExchangeServer, base.AuthenticationMethod, this.MailboxPermission == MigrationMailboxPermission.Admin);
				}
				else
				{
					exchangeConnectionSettings = ExchangeConnectionSettings.Create(base.Username, base.Domain, base.EncryptedPassword, this.RpcProxyServer.ToString(), this.ExchangeServer, base.AuthenticationMethod, this.MailboxPermission == MigrationMailboxPermission.Admin);
				}
				if (this.NspiServer != null)
				{
					exchangeConnectionSettings.IncomingNSPIServer = this.NspiServer;
				}
				return exchangeConnectionSettings;
			}
		}

		public override MigrationType PreferredMigrationType
		{
			get
			{
				return MigrationType.ExchangeOutlookAnywhere;
			}
		}

		public static void ValidateEndpoint(ExchangeOutlookAnywhereEndpoint endpoint)
		{
			if (string.IsNullOrEmpty(endpoint.Username))
			{
				throw new MigrationPermanentException(ServerStrings.MigrationJobConnectionSettingsIncomplete("Username"), "empty user name");
			}
			if (endpoint.RpcProxyServer == null)
			{
				throw new MigrationPermanentException(ServerStrings.MigrationJobConnectionSettingsIncomplete("RpcProxyServer"), "no rpc proxy server");
			}
			if (endpoint.ExchangeServer == null)
			{
				throw new MigrationPermanentException(ServerStrings.MigrationJobConnectionSettingsIncomplete("ExchangeServer"), "no exchange server");
			}
			if (endpoint.NspiServer == null)
			{
				throw new MigrationPermanentException(ServerStrings.MigrationJobConnectionSettingsIncomplete("NspiServer"), "no nspi server");
			}
			if (endpoint.UseAutoDiscover && string.IsNullOrEmpty(endpoint.EmailAddress))
			{
				throw new MigrationPermanentException(ServerStrings.MigrationJobConnectionSettingsIncomplete("EmailAddress"), "autodiscovery used, but no email");
			}
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			message[MigrationEndpointMessageSchema.ExchangeServer] = this.ExchangeServer;
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.ExchangeServer = (string)message[MigrationEndpointMessageSchema.ExchangeServer];
			return base.ReadFromMessageItem(message);
		}

		public override NspiMigrationDataReader GetNspiDataReader(MigrationJob job = null)
		{
			if (string.IsNullOrEmpty(this.NspiServer))
			{
				IMigrationNspiClient nspiClient = MigrationServiceFactory.Instance.GetNspiClient((job != null) ? job.ReportData : null);
				this.NspiServer = nspiClient.GetNewDSA(this);
			}
			return new NspiMigrationDataReader(this, job);
		}

		public override void VerifyConnectivity()
		{
			this.GetNspiDataReader(null).Ping();
		}

		public override void InitializeFromAutoDiscover(SmtpAddress emailAddress, PSCredential credentials)
		{
			base.InitializeFromAutoDiscover(emailAddress, credentials);
			this.EmailAddress = (string)emailAddress;
		}

		protected override void ApplyAdditionalProperties(MigrationEndpoint presentationObject)
		{
			presentationObject.RpcProxyServer = this.RpcProxyServer;
			presentationObject.ExchangeServer = this.ExchangeServer;
			presentationObject.Credentials = this.Credentials;
			presentationObject.UseAutoDiscover = new bool?(this.UseAutoDiscover);
			presentationObject.MailboxPermission = this.MailboxPermission;
			presentationObject.NspiServer = this.NspiServer;
			presentationObject.Authentication = new AuthenticationMethod?(base.AuthenticationMethod);
			presentationObject.EmailAddress = (SmtpAddress)this.EmailAddress;
		}

		protected override void ApplyAutodiscoverSettings(AutodiscoverClientResponse response)
		{
			this.RpcProxyServer = new Fqdn(response.RPCProxyServer);
			this.ExchangeServer = response.ExchangeServer;
			base.AuthenticationMethod = (response.AuthenticationMethod ?? base.DefaultAuthenticationMethod);
			this.UseAutoDiscover = true;
		}

		protected override void InitializeFromPresentationObject(MigrationEndpoint endpoint)
		{
			base.InitializeFromPresentationObject(endpoint);
			this.ExchangeServer = endpoint.ExchangeServer;
			this.RpcProxyServer = endpoint.RpcProxyServer;
			this.NspiServer = endpoint.NspiServer;
			this.UseAutoDiscover = (endpoint.UseAutoDiscover ?? false);
			this.MailboxPermission = endpoint.MailboxPermission;
			this.EmailAddress = (string)endpoint.EmailAddress;
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			base.AddDiagnosticInfoToElement(dataProvider, parent, argument);
			parent.Add(new XElement("ExchangeServer", this.ExchangeServer));
		}
	}
}
