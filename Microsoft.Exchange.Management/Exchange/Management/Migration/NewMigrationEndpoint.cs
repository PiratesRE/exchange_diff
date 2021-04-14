using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("New", "MigrationEndpoint", DefaultParameterSetName = "ExchangeRemoteMove", SupportsShouldProcess = true)]
	public sealed class NewMigrationEndpoint : NewMigrationObjectTaskBase<MigrationEndpoint>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				return (Unlimited<int>)(base.Fields["MaxConcurrentMigrations"] ?? new Unlimited<int>(20));
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
				Unlimited<int>? unlimited = (Unlimited<int>?)base.Fields["MaxConcurrentIncrementalSyncs"];
				if (unlimited == null)
				{
					unlimited = new Unlimited<int>?((this.MaxConcurrentMigrations < 10) ? this.MaxConcurrentMigrations : new Unlimited<int>(10));
				}
				return unlimited.Value;
			}
			set
			{
				base.Fields["MaxConcurrentIncrementalSyncs"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PSTImport")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeRemoteMove")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMoveAutoDiscover")]
		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
		[ValidateNotNull]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMoveAutoDiscover")]
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

		[Parameter(Mandatory = true, ParameterSetName = "IMAP")]
		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMove")]
		[Parameter(Mandatory = true, ParameterSetName = "PSTImport")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
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

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
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

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
		[ValidateNotNullOrEmpty]
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

		[Parameter(Mandatory = false, ParameterSetName = "IMAP")]
		public int Port
		{
			get
			{
				return (int)(base.Fields["Port"] ?? 993);
			}
			set
			{
				base.Fields["Port"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "IMAP")]
		public AuthenticationMethod Authentication
		{
			get
			{
				return (AuthenticationMethod)(base.Fields["Authentication"] ?? AuthenticationMethod.Basic);
			}
			set
			{
				base.Fields["Authentication"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "IMAP")]
		public IMAPSecurityMechanism Security
		{
			get
			{
				return (IMAPSecurityMechanism)(base.Fields["Security"] ?? IMAPSecurityMechanism.Ssl);
			}
			set
			{
				base.Fields["Security"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "IMAP")]
		public SwitchParameter IMAP
		{
			get
			{
				return (SwitchParameter)(base.Fields["IMAP"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IMAP"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PSTImport")]
		public SwitchParameter PSTImport
		{
			get
			{
				return (SwitchParameter)(base.Fields["PSTImport"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["PSTImport"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
		public SwitchParameter PublicFolder
		{
			get
			{
				return (SwitchParameter)(base.Fields["PublicFolder"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["PublicFolder"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
		public SwitchParameter ExchangeOutlookAnywhere
		{
			get
			{
				return (SwitchParameter)(base.Fields["ExchangeOutlookAnywhere"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ExchangeOutlookAnywhere"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMove")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMoveAutoDiscover")]
		public SwitchParameter ExchangeRemoteMove
		{
			get
			{
				return (SwitchParameter)(base.Fields["ExchangeRemoteMove"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ExchangeRemoteMove"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMoveAutoDiscover")]
		public SwitchParameter Autodiscover
		{
			get
			{
				return (SwitchParameter)(base.Fields["Autodiscover"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Autodiscover"] = value;
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
				return Strings.ConfirmationMessageNewMigrationEndpoint(this.Name);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "New-MigrationEndpoint";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			this.DataProvider = MigrationDataProvider.CreateProviderForMigrationMailbox(base.GetType().Name, base.TenantGlobalCatalogSession, this.partitionMailbox);
			return MigrationEndpointDataProvider.CreateDataProvider("NewMigrationEndpoint", base.TenantGlobalCatalogSession, this.partitionMailbox);
		}

		protected override void InternalValidate()
		{
			if (this.MaxConcurrentIncrementalSyncs > this.MaxConcurrentMigrations)
			{
				base.WriteError(new MigrationMaxConcurrentIncrementalSyncsVerificationFailedException(this.MaxConcurrentIncrementalSyncs, this.MaxConcurrentMigrations));
			}
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (<PrivateImplementationDetails>{37FB2C37-8946-4F00-B324-D759A1883C9F}.$$method0x60029e6-1 == null)
				{
					<PrivateImplementationDetails>{37FB2C37-8946-4F00-B324-D759A1883C9F}.$$method0x60029e6-1 = new Dictionary<string, int>(7)
					{
						{
							"ExchangeOutlookAnywhere",
							0
						},
						{
							"ExchangeOutlookAnywhereAutoDiscover",
							1
						},
						{
							"ExchangeRemoteMove",
							2
						},
						{
							"ExchangeRemoteMoveAutoDiscover",
							3
						},
						{
							"IMAP",
							4
						},
						{
							"PSTImport",
							5
						},
						{
							"PublicFolder",
							6
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{37FB2C37-8946-4F00-B324-D759A1883C9F}.$$method0x60029e6-1.TryGetValue(parameterSetName, out num))
				{
					switch (num)
					{
					case 0:
					case 1:
						this.endpointType = MigrationType.ExchangeOutlookAnywhere;
						break;
					case 2:
					case 3:
						this.endpointType = MigrationType.ExchangeRemoteMove;
						break;
					case 4:
						this.endpointType = MigrationType.IMAP;
						break;
					case 5:
						this.endpointType = MigrationType.PSTImport;
						break;
					case 6:
						this.endpointType = MigrationType.PublicFolder;
						break;
					default:
						goto IL_10C;
					}
					base.InternalValidate();
					this.DataObject.Identity = new MigrationEndpointId(this.Name, Guid.Empty);
					this.DataObject.EndpointType = this.endpointType;
					this.DataObject.MaxConcurrentIncrementalSyncs = this.MaxConcurrentIncrementalSyncs;
					this.DataObject.MaxConcurrentMigrations = this.MaxConcurrentMigrations;
					this.DataObject.Credentials = this.Credentials;
					string parameterSetName2;
					switch (parameterSetName2 = base.ParameterSetName)
					{
					case "ExchangeOutlookAnywhereAutoDiscover":
					case "ExchangeRemoteMoveAutoDiscover":
						this.PopulateAutoDiscover();
						break;
					case "ExchangeOutlookAnywhere":
						this.PopulateExchangeOutlookAnywhere();
						break;
					case "ExchangeRemoteMove":
						this.PopulateExchangeRemoteMove();
						break;
					case "IMAP":
						this.PopulateImap();
						break;
					case "PSTImport":
						this.PopulatePSTImport();
						break;
					case "PublicFolder":
						this.PopulatePublicFolder();
						break;
					}
					if (this.ExchangeOutlookAnywhere)
					{
						this.targetMailbox = TestMigrationServerAvailability.DiscoverTestMailbox(this.TestMailbox, ((MigrationADProvider)this.DataProvider.ADProvider).RecipientSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
					}
					bool flag = this.ExchangeOutlookAnywhere && !base.IsFieldSet("MailboxPermission");
					if (flag)
					{
						if (this.endpointObject == null)
						{
							this.endpointObject = MigrationEndpointBase.CreateFrom(this.DataObject);
						}
						try
						{
							ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = (ExchangeOutlookAnywhereEndpoint)this.endpointObject;
							TestMigrationServerAvailability.VerifyExchangeOutlookAnywhereConnection(this.DataProvider, exchangeOutlookAnywhereEndpoint, (string)this.EmailAddress, this.SourceMailboxLegacyDN, this.targetMailbox, true);
							this.DataObject.MailboxPermission = exchangeOutlookAnywhereEndpoint.MailboxPermission;
						}
						catch (LocalizedException innerException)
						{
							base.WriteError(new UnableToDiscoverMailboxPermissionException(innerException));
						}
					}
					if (!this.SkipVerification)
					{
						if (this.endpointObject == null)
						{
							this.endpointObject = MigrationEndpointBase.CreateFrom(this.DataObject);
						}
						this.endpointObject.VerifyConnectivity();
						if (this.ExchangeOutlookAnywhere && !flag)
						{
							ExchangeOutlookAnywhereEndpoint outlookAnywhereEndpoint = (ExchangeOutlookAnywhereEndpoint)this.endpointObject;
							TestMigrationServerAvailability.VerifyExchangeOutlookAnywhereConnection(this.DataProvider, outlookAnywhereEndpoint, (string)this.EmailAddress, this.SourceMailboxLegacyDN, this.targetMailbox, false);
						}
						else if (this.PublicFolder)
						{
							this.targetMailbox = TestMigrationServerAvailability.DiscoverPublicFolderTestMailbox(this.TestMailbox, this.ConfigurationSession, ((MigrationADProvider)this.DataProvider.ADProvider).RecipientSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
							PublicFolderEndpoint publicFolderEndpoint = (PublicFolderEndpoint)this.endpointObject;
							TestMigrationServerAvailability.VerifyPublicFolderConnection(this.DataProvider, publicFolderEndpoint, this.SourceMailboxLegacyDN, this.PublicFolderDatabaseServerLegacyDN, this.targetMailbox);
						}
					}
					this.DataObject.LastModifiedTime = (DateTime)ExDateTime.UtcNow;
					return;
				}
			}
			IL_10C:
			throw new ArgumentException("Unexpected parameter set!");
		}

		protected override bool IsKnownException(Exception exception)
		{
			return MigrationBatchDataProvider.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						this.DisposeSession();
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void PopulateImap()
		{
			this.ValidateImap();
			this.DataObject.RemoteServer = this.RemoteServer;
			this.DataObject.Port = new int?(this.Port);
			this.DataObject.Security = new IMAPSecurityMechanism?(this.Security);
			this.DataObject.Authentication = new AuthenticationMethod?(this.Authentication);
		}

		private void PopulateExchangeOutlookAnywhere()
		{
			this.ValidateExchangeOutlookAnywhere();
			this.DataObject.ExchangeServer = this.ExchangeServer;
			this.DataObject.RpcProxyServer = this.RpcProxyServer;
			this.DataObject.NspiServer = this.NspiServer;
			this.DataObject.Authentication = new AuthenticationMethod?(this.Authentication);
			this.DataObject.MailboxPermission = this.MailboxPermission;
		}

		private void PopulatePublicFolder()
		{
			this.ValidatePublicFolder();
			this.DataObject.Authentication = new AuthenticationMethod?(this.Authentication);
			this.DataObject.RpcProxyServer = (this.DataObject.RemoteServer = this.RpcProxyServer);
			this.DataObject.PublicFolderDatabaseServerLegacyDN = this.PublicFolderDatabaseServerLegacyDN;
			this.DataObject.SourceMailboxLegacyDN = this.SourceMailboxLegacyDN;
		}

		private void PopulateExchangeRemoteMove()
		{
			this.ValidateExchangeRemoteMove();
			this.DataObject.RemoteServer = this.RemoteServer;
		}

		private void PopulateAutoDiscover()
		{
			ValidationError validationError = MigrationConstraints.NameLengthConstraint.Validate(this.EmailAddress.ToString(), MigrationBatchMessageSchema.MigrationJobExchangeEmailAddress, null);
			if (validationError != null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationRemoteServerTooLongException("EmailAddress")), (ErrorCategory)1000, null);
			}
			if (this.ExchangeOutlookAnywhere)
			{
				this.DataObject.MailboxPermission = this.MailboxPermission;
			}
			this.endpointObject = MigrationEndpointBase.CreateFrom(this.DataObject);
			this.endpointObject.InitializeFromAutoDiscover(this.EmailAddress, this.Credentials);
			this.DataObject = this.endpointObject.ToMigrationEndpoint();
		}

		private void PopulatePSTImport()
		{
			this.ValidatePSTImport();
			this.DataObject.RemoteServer = this.RemoteServer;
		}

		private void ValidatePSTImport()
		{
			this.ValidateRemoteServerConstraint(this.RemoteServer.ToString(), MigrationBatchMessageSchema.MigrationJobRemoteServerHostName, "RemoteServer");
		}

		private void ValidateImap()
		{
			this.ValidateRemoteServerConstraint(this.RemoteServer.ToString(), MigrationBatchMessageSchema.MigrationJobRemoteServerHostName, "RemoteServer");
			ValidationError validationError = MigrationConstraints.PortRangeConstraint.Validate(this.Port, MigrationBatchMessageSchema.MigrationJobRemoteServerPortNumber, null);
			if (validationError != null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationPortVerificationFailed(this.Port, MigrationConstraints.PortRangeConstraint.MinimumValue, MigrationConstraints.PortRangeConstraint.MaximumValue)));
			}
		}

		private void ValidateExchangeOutlookAnywhere()
		{
			this.ValidateRemoteServerConstraint(this.RpcProxyServer, MigrationBatchMessageSchema.MigrationJobExchangeRPCProxyServerHostName, "RpcProxyServer");
			this.ValidateRemoteServerConstraint(this.ExchangeServer, MigrationBatchMessageSchema.MigrationJobExchangeRemoteServerHostName, "ExchangeServer");
			bool flag = this.EmailAddress == SmtpAddress.Empty && string.IsNullOrEmpty(this.SourceMailboxLegacyDN);
			if (!base.IsFieldSet("MailboxPermission") && flag)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationMustSpecifyEmailOrMailboxDNOrMailboxPermission));
			}
			if (!this.SkipVerification && flag)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationMustSpecifyEmailOrMailboxDNOrSkipVerification));
			}
		}

		private void ValidatePublicFolder()
		{
			this.ValidateRemoteServerConstraint(this.RpcProxyServer, MigrationBatchMessageSchema.MigrationJobExchangeRPCProxyServerHostName, "RpcProxyServer");
			if (!LegacyDN.IsValidLegacyDN(this.SourceMailboxLegacyDN))
			{
				base.WriteError(new InvalidLegacyExchangeDnValueException("SourceMailboxLegacyDN"));
			}
			if (!LegacyDN.IsValidLegacyDN(this.PublicFolderDatabaseServerLegacyDN))
			{
				base.WriteError(new InvalidLegacyExchangeDnValueException("PublicFolderDatabaseServerLegacyDN"));
			}
		}

		private void ValidateExchangeRemoteMove()
		{
			this.ValidateRemoteServerConstraint(this.RemoteServer.ToString(), MigrationBatchMessageSchema.MigrationJobRemoteServerHostName, "RemoteServer");
		}

		private void ValidateRemoteServerConstraint(string remoteServer, StorePropertyDefinition propertyDefinition, string propertyName)
		{
			if (remoteServer == null)
			{
				base.WriteError(new MissingParameterException(propertyName));
			}
			ValidationError validationError = MigrationConstraints.RemoteServerNameConstraint.Validate(remoteServer, propertyDefinition, null);
			if (validationError != null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationRemoteServerTooLongException(propertyName)));
			}
		}

		private void DisposeSession()
		{
			IDisposable disposable = base.DataSession as IDisposable;
			if (disposable != null)
			{
				MigrationLogger.Close();
				disposable.Dispose();
			}
			if (this.DataProvider != null)
			{
				this.DataProvider.Dispose();
				this.DataProvider = null;
			}
		}

		private const int DefaultMaxConcurrentMigrations = 20;

		private const int DefaultMaxConcurrentIncrementalSyncs = 10;

		private const string ParameterNameIdentity = "Identity";

		private const string ParameterNameMaxConcurrentMigrations = "MaxConcurrentMigrations";

		private const string ParameterNameAutodiscover = "Autodiscover";

		private const string ParameterNameTestMailbox = "TestMailbox";

		private const string ParameterNameSourceMailboxLegacyDN = "SourceMailboxLegacyDN";

		private const string ParameterNamePublicFolderDatabaseServerLegacyDN = "PublicFolderDatabaseServerLegacyDN";

		private const string ParameterNameMailboxPermission = "MailboxPermission";

		private const string ParameterNameCredentials = "Credentials";

		private const string ParameterNameRemoteServer = "RemoteServer";

		private const string ParameterNameExchangeServer = "ExchangeServer";

		private const string ParameterNameRpcProxyServer = "RPCProxyServer";

		private const string ParameterNameNspiServer = "NspiServer";

		private const string ParameterNameImap = "IMAP";

		private const string ParameterNamePstImport = "PSTImport";

		private const string ParameterNamePublicFolder = "PublicFolder";

		private const string ParameterNameExchangeOutlookAnywhere = "ExchangeOutlookAnywhere";

		private const string ParameterNameExchangeRemoteMove = "ExchangeRemoteMove";

		private const string ParameterNameMaxConcurrentIncrementalSyncs = "MaxConcurrentIncrementalSyncs";

		private const string ParameterNamePort = "Port";

		private const string ParameterNameAuthentication = "Authentication";

		private const string ParameterNameEmailAddress = "EmailAddress";

		private const string ParameterNameSecurity = "Security";

		private const string ParameterSetNameImap = "IMAP";

		private const string ParameterNameSkipVerification = "SkipVerification";

		private const string ParameterSetNameExchangeOutlookAnywhere = "ExchangeOutlookAnywhere";

		private const string ParameterSetNameExchangeOutlookAnywhereAutoDiscover = "ExchangeOutlookAnywhereAutoDiscover";

		private const string ParameterSetNameExchangeRemoteMove = "ExchangeRemoteMove";

		private const string ParameterSetNameExchangeRemoteMoveAutoDiscover = "ExchangeRemoteMoveAutoDiscover";

		private const string ParameterSetNamePstImport = "PSTImport";

		private const string ParameterSetNamePublicFolder = "PublicFolder";

		private MailboxData targetMailbox;

		private MigrationType endpointType;

		private MigrationEndpointBase endpointObject;

		private bool disposed;
	}
}
