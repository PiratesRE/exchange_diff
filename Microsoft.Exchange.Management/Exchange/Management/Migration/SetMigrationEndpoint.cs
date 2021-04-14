using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Set", "MigrationEndpoint", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetMigrationEndpoint : SetMigrationObjectTaskBase<MigrationEndpointIdParameter, MigrationEndpoint, MigrationEndpoint>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override MigrationEndpointIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				return (Unlimited<int>)(base.Fields["MaxConcurrentMigrations"] ?? Unlimited<int>.UnlimitedValue);
			}
			set
			{
				base.Fields["MaxConcurrentMigrations"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxConcurrentIncrementalSyncs
		{
			get
			{
				return (Unlimited<int>)(base.Fields["MaxConcurrentIncrementalSyncs"] ?? Unlimited<int>.UnlimitedValue);
			}
			set
			{
				base.Fields["MaxConcurrentIncrementalSyncs"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential Credentials
		{
			get
			{
				return (PSCredential)base.Fields["Credentials"];
			}
			set
			{
				base.Fields["Credentials"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MigrationMailboxPermission MailboxPermission
		{
			get
			{
				return (MigrationMailboxPermission)(base.Fields["MailboxPermission"] ?? MigrationMailboxPermission.Admin);
			}
			set
			{
				base.Fields["MailboxPermission"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string ExchangeServer
		{
			get
			{
				return (string)base.Fields["ExchangeServer"];
			}
			set
			{
				base.Fields["ExchangeServer"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public Fqdn RemoteServer
		{
			get
			{
				return (Fqdn)base.Fields["RemoteServer"];
			}
			set
			{
				base.Fields["RemoteServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public Fqdn RpcProxyServer
		{
			get
			{
				return (Fqdn)base.Fields["RPCProxyServer"];
			}
			set
			{
				base.Fields["RPCProxyServer"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string NspiServer
		{
			get
			{
				return (string)base.Fields["NspiServer"];
			}
			set
			{
				base.Fields["NspiServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Port
		{
			get
			{
				return (int)base.Fields["Port"];
			}
			set
			{
				base.Fields["Port"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AuthenticationMethod Authentication
		{
			get
			{
				return (AuthenticationMethod)base.Fields["Authentication"];
			}
			set
			{
				base.Fields["Authentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IMAPSecurityMechanism Security
		{
			get
			{
				return (IMAPSecurityMechanism)base.Fields["Security"];
			}
			set
			{
				base.Fields["Security"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public MailboxIdParameter TestMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["TestMailbox"];
			}
			set
			{
				base.Fields["TestMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress EmailAddress
		{
			get
			{
				return (SmtpAddress)(base.Fields["EmailAddress"] ?? SmtpAddress.Empty);
			}
			set
			{
				base.Fields["EmailAddress"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string SourceMailboxLegacyDN
		{
			get
			{
				return (string)base.Fields["SourceMailboxLegacyDN"];
			}
			set
			{
				base.Fields["SourceMailboxLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string PublicFolderDatabaseServerLegacyDN
		{
			get
			{
				return (string)base.Fields["PublicFolderDatabaseServerLegacyDN"];
			}
			set
			{
				base.Fields["PublicFolderDatabaseServerLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipVerification
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipVerification"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipVerification"] = value;
			}
		}

		internal MigrationDataProvider DataProvider { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMigrationEndpoint(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Set-MigrationEndpoint";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			this.DataProvider = MigrationDataProvider.CreateProviderForMigrationMailbox(base.GetType().Name, base.TenantGlobalCatalogSession, this.partitionMailbox);
			return MigrationEndpointDataProvider.CreateDataProvider("SetMigrationEndpoint", base.TenantGlobalCatalogSession, this.partitionMailbox);
		}

		protected override bool IsObjectStateChanged()
		{
			return this.changed;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			bool flag = false;
			MigrationType endpointType = this.DataObject.EndpointType;
			if (endpointType <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (endpointType == MigrationType.IMAP)
				{
					this.WriteParameterErrorIfSet("Credentials");
					this.WriteParameterErrorIfSet("MailboxPermission");
					this.WriteParameterErrorIfSet("ExchangeServer");
					this.WriteParameterErrorIfSet("RPCProxyServer");
					this.WriteParameterErrorIfSet("NspiServer");
					this.WriteParameterErrorIfSet("PublicFolderDatabaseServerLegacyDN");
					this.WriteParameterErrorIfSet("TestMailbox", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					this.WriteParameterErrorIfSet("SourceMailboxLegacyDN", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					this.WriteParameterErrorIfSet("EmailAddress", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					goto IL_2B4;
				}
				if (endpointType == MigrationType.ExchangeOutlookAnywhere)
				{
					this.WriteParameterErrorIfSet("Port");
					this.WriteParameterErrorIfSet("Security");
					this.WriteParameterErrorIfSet("PublicFolderDatabaseServerLegacyDN");
					goto IL_2B4;
				}
			}
			else
			{
				if (endpointType == MigrationType.ExchangeRemoteMove)
				{
					this.WriteParameterErrorIfSet("ExchangeServer");
					this.WriteParameterErrorIfSet("RPCProxyServer");
					this.WriteParameterErrorIfSet("Port");
					this.WriteParameterErrorIfSet("MailboxPermission");
					this.WriteParameterErrorIfSet("Authentication");
					this.WriteParameterErrorIfSet("Security");
					this.WriteParameterErrorIfSet("NspiServer");
					this.WriteParameterErrorIfSet("PublicFolderDatabaseServerLegacyDN");
					this.WriteParameterErrorIfSet("TestMailbox", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					this.WriteParameterErrorIfSet("SourceMailboxLegacyDN", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					this.WriteParameterErrorIfSet("EmailAddress", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					goto IL_2B4;
				}
				if (endpointType == MigrationType.PSTImport)
				{
					this.WriteParameterErrorIfSet("ExchangeServer");
					this.WriteParameterErrorIfSet("RPCProxyServer");
					this.WriteParameterErrorIfSet("Port");
					this.WriteParameterErrorIfSet("MailboxPermission");
					this.WriteParameterErrorIfSet("Authentication");
					this.WriteParameterErrorIfSet("Security");
					this.WriteParameterErrorIfSet("NspiServer");
					this.WriteParameterErrorIfSet("PublicFolderDatabaseServerLegacyDN");
					this.WriteParameterErrorIfSet("TestMailbox", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					this.WriteParameterErrorIfSet("SourceMailboxLegacyDN", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					this.WriteParameterErrorIfSet("EmailAddress", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					goto IL_2B4;
				}
				if (endpointType == MigrationType.PublicFolder)
				{
					this.WriteParameterErrorIfSet("RemoteServer");
					this.WriteParameterErrorIfSet("ExchangeServer");
					this.WriteParameterErrorIfSet("Port");
					this.WriteParameterErrorIfSet("MailboxPermission");
					this.WriteParameterErrorIfSet("Security");
					this.WriteParameterErrorIfSet("NspiServer");
					this.WriteParameterErrorIfSet("EmailAddress", new LocalizedString?(Strings.ErrorInvalidEndpointParameterReasonUsedForConnectionTest));
					goto IL_2B4;
				}
			}
			base.WriteError(new InvalidEndpointTypeException(this.Identity.RawIdentity, this.DataObject.EndpointType.ToString()));
			IL_2B4:
			if (base.IsFieldSet("MaxConcurrentMigrations") || base.IsFieldSet("MaxConcurrentIncrementalSyncs"))
			{
				Unlimited<int> unlimited = base.IsFieldSet("MaxConcurrentMigrations") ? this.MaxConcurrentMigrations : this.DataObject.MaxConcurrentMigrations;
				Unlimited<int> unlimited2 = base.IsFieldSet("MaxConcurrentIncrementalSyncs") ? this.MaxConcurrentIncrementalSyncs : this.DataObject.MaxConcurrentIncrementalSyncs;
				if (unlimited2 > unlimited)
				{
					base.WriteError(new MigrationMaxConcurrentIncrementalSyncsVerificationFailedException(unlimited2, unlimited));
				}
			}
			if (base.IsFieldSet("MaxConcurrentMigrations") && !this.MaxConcurrentMigrations.Equals(this.DataObject.MaxConcurrentMigrations))
			{
				this.DataObject.MaxConcurrentMigrations = this.MaxConcurrentMigrations;
				this.changed = true;
			}
			if (base.IsFieldSet("MaxConcurrentIncrementalSyncs") && !this.MaxConcurrentIncrementalSyncs.Equals(this.DataObject.MaxConcurrentIncrementalSyncs))
			{
				this.DataObject.MaxConcurrentIncrementalSyncs = this.MaxConcurrentIncrementalSyncs;
				this.changed = true;
			}
			if (base.IsFieldSet("Credentials") && (this.Credentials == null || !this.Credentials.Equals(this.DataObject.Credentials)))
			{
				this.DataObject.Credentials = this.Credentials;
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("MailboxPermission") && this.MailboxPermission != this.DataObject.MailboxPermission)
			{
				this.DataObject.MailboxPermission = this.MailboxPermission;
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("ExchangeServer") && !this.ExchangeServer.Equals(this.DataObject.ExchangeServer))
			{
				this.DataObject.ExchangeServer = this.ExchangeServer;
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("RPCProxyServer") && !this.RpcProxyServer.Equals(this.DataObject.RpcProxyServer))
			{
				this.DataObject.RpcProxyServer = this.RpcProxyServer;
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("Port") && !this.Port.Equals(this.DataObject.Port))
			{
				this.DataObject.Port = new int?(this.Port);
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("Authentication") && !this.Authentication.Equals(this.DataObject.Authentication))
			{
				this.DataObject.Authentication = new AuthenticationMethod?(this.Authentication);
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("Security") && !this.Security.Equals(this.DataObject.Security))
			{
				this.DataObject.Security = new IMAPSecurityMechanism?(this.Security);
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("RemoteServer") && !this.RemoteServer.Equals(this.DataObject.RemoteServer))
			{
				this.DataObject.RemoteServer = this.RemoteServer;
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("NspiServer") && !this.NspiServer.Equals(this.DataObject.NspiServer))
			{
				this.DataObject.NspiServer = this.NspiServer;
				this.changed = true;
				flag = true;
			}
			if (this.DataObject.EndpointType == MigrationType.PublicFolder && base.IsFieldSet("SourceMailboxLegacyDN") && !this.SourceMailboxLegacyDN.Equals(this.DataObject.SourceMailboxLegacyDN))
			{
				if (!LegacyDN.IsValidLegacyDN(this.SourceMailboxLegacyDN))
				{
					base.WriteError(new InvalidLegacyExchangeDnValueException("SourceMailboxLegacyDN"));
				}
				this.DataObject.SourceMailboxLegacyDN = this.SourceMailboxLegacyDN;
				this.changed = true;
				flag = true;
			}
			if (base.IsFieldSet("PublicFolderDatabaseServerLegacyDN") && !this.PublicFolderDatabaseServerLegacyDN.Equals(this.DataObject.PublicFolderDatabaseServerLegacyDN))
			{
				if (!LegacyDN.IsValidLegacyDN(this.PublicFolderDatabaseServerLegacyDN))
				{
					base.WriteError(new InvalidLegacyExchangeDnValueException("PublicFolderDatabaseServerLegacyDN"));
				}
				this.DataObject.PublicFolderDatabaseServerLegacyDN = this.PublicFolderDatabaseServerLegacyDN;
				this.changed = true;
				flag = true;
			}
			if (flag)
			{
				this.DataObject.LastModifiedTime = (DateTime)ExDateTime.UtcNow;
			}
			if (!this.SkipVerification)
			{
				MigrationEndpointBase migrationEndpointBase = MigrationEndpointBase.CreateFrom(this.DataObject);
				migrationEndpointBase.VerifyConnectivity();
				if (this.DataObject.EndpointType == MigrationType.ExchangeOutlookAnywhere)
				{
					ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = (ExchangeOutlookAnywhereEndpoint)migrationEndpointBase;
					if (!string.IsNullOrEmpty(this.SourceMailboxLegacyDN) || this.EmailAddress != SmtpAddress.Empty || !string.IsNullOrEmpty(exchangeOutlookAnywhereEndpoint.EmailAddress))
					{
						MailboxData targetMailbox = TestMigrationServerAvailability.DiscoverTestMailbox(this.TestMailbox, ((MigrationADProvider)this.DataProvider.ADProvider).RecipientSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
						string text = (string)this.EmailAddress;
						if (string.IsNullOrEmpty(text))
						{
							text = exchangeOutlookAnywhereEndpoint.EmailAddress;
						}
						TestMigrationServerAvailability.VerifyExchangeOutlookAnywhereConnection(this.DataProvider, exchangeOutlookAnywhereEndpoint, text, this.SourceMailboxLegacyDN, targetMailbox, false);
						return;
					}
				}
				else if (this.DataObject.EndpointType == MigrationType.PublicFolder)
				{
					MailboxData mailboxData = TestMigrationServerAvailability.DiscoverPublicFolderTestMailbox(this.TestMailbox, this.ConfigurationSession, ((MigrationADProvider)this.DataProvider.ADProvider).RecipientSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
					TestMigrationServerAvailability.VerifyPublicFolderConnection(this.DataProvider, (PublicFolderEndpoint)migrationEndpointBase, this.SourceMailboxLegacyDN, this.PublicFolderDatabaseServerLegacyDN, mailboxData);
				}
			}
		}

		protected override void DisposeSession()
		{
			base.DisposeSession();
			if (this.DataProvider != null)
			{
				this.DataProvider.Dispose();
				this.DataProvider = null;
			}
		}

		private void WriteParameterErrorIfSet(string parameterName)
		{
			this.WriteParameterErrorIfSet(parameterName, null);
		}

		private void WriteParameterErrorIfSet(string parameterName, LocalizedString? reason)
		{
			if (base.IsFieldSet(parameterName))
			{
				base.WriteError(new InvalidEndpointParameterException(parameterName, this.DataObject.EndpointType.ToString(), reason ?? Strings.ErrorInvalidEndpointParameterReasonInvalidProperty));
			}
		}

		private const string ParameterNameMaxConcurrentMigrations = "MaxConcurrentMigrations";

		private const string ParameterNameMailboxPermission = "MailboxPermission";

		private const string ParameterNameCredentials = "Credentials";

		private const string ParameterNameExchangeServer = "ExchangeServer";

		private const string ParameterNameRpcProxyServer = "RPCProxyServer";

		private const string ParameterNameNspiServer = "NspiServer";

		private const string ParameterNameMaxConcurrentIncrementalSyncs = "MaxConcurrentIncrementalSyncs";

		private const string ParameterNamePort = "Port";

		private const string ParameterNameAuthentication = "Authentication";

		private const string ParameterNameSecurity = "Security";

		private const string ParameterNameRemoteServer = "RemoteServer";

		private const string ParameterNameTestMailbox = "TestMailbox";

		private const string ParameterNameEmailAddress = "EmailAddress";

		private const string ParameterNameSourceMailboxLegacyDN = "SourceMailboxLegacyDN";

		private const string ParameterNamePublicFolderDatabaseServerLegacyDN = "PublicFolderDatabaseServerLegacyDN";

		private const string ParameterNameSkipVerification = "SkipVerification";

		private bool changed;
	}
}
