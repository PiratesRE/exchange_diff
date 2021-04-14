using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Test", "MigrationServerAvailability", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class TestMigrationServerAvailability : MigrationOrganizationTaskBase
	{
		[Parameter(Mandatory = true, ParameterSetName = "IMAP")]
		public SwitchParameter Imap
		{
			get
			{
				return (SwitchParameter)(base.Fields["Imap"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Imap"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "IMAP")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "PSTImport")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMove")]
		public Fqdn RemoteServer
		{
			get
			{
				return Fqdn.Parse((string)base.Fields["RemoteServer"]);
			}
			set
			{
				base.Fields["RemoteServer"] = value.ToString();
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "IMAP")]
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

		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
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

		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMoveAutoDiscover")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMove")]
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

		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = true, ParameterSetName = "PSTImport")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeRemoteMove")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeRemoteMoveAutoDiscover")]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhere")]
		public Fqdn RPCProxyServer
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
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeOutlookAnywhere")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhereAutoDiscover")]
		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "ExchangeOutlookAnywhere")]
		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
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
		[Parameter(Mandatory = true, ParameterSetName = "TestEndpoint")]
		public MigrationEndpointIdParameter Endpoint
		{
			get
			{
				return (MigrationEndpointIdParameter)base.Fields["Endpoint"];
			}
			set
			{
				base.Fields["Endpoint"] = value;
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

		[Parameter(Mandatory = false, ParameterSetName = "PSTImport")]
		public string FilePath
		{
			get
			{
				return (string)base.Fields["FilePath"];
			}
			set
			{
				base.Fields["FilePath"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestMigrationServerAvailability;
			}
		}

		private bool SupportsCutover { get; set; }

		private IRecipientSession RecipientSession
		{
			get
			{
				return base.DataSession as IRecipientSession;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			string parameterSetName;
			switch (parameterSetName = base.ParameterSetName)
			{
			case "IMAP":
			case "ExchangeOutlookAnywhere":
			case "ExchangeOutlookAnywhereAutoDiscover":
			case "ExchangeRemoteMove":
			case "ExchangeRemoteMoveAutoDiscover":
			case "PSTImport":
			case "PublicFolder":
				goto IL_D4;
			case "TestEndpoint":
				this.ValidateEndpoint();
				return;
			}
			this.WriteError(new TestMigrationServerAvailabilityProtocolArgumentException());
			IL_D4:
			if (this.Imap)
			{
				this.ValidateImapParameters();
				return;
			}
			if (this.ExchangeOutlookAnywhere)
			{
				this.ValidateExchangeOutlookAnywhereParameters();
				return;
			}
			if (this.ExchangeRemoteMove)
			{
				this.ValidateExchangeRemoteMoveParameters();
				return;
			}
			if (this.PSTImport)
			{
				this.ValidatePSTImportParameters();
				return;
			}
			if (this.PublicFolder)
			{
				this.ValidatePublicFolderParameters();
				return;
			}
			this.WriteError(new TestMigrationServerAvailabilityProtocolArgumentException());
		}

		protected override void InternalProcessRecord()
		{
			this.SupportsCutover = false;
			if (this.Imap || this.ExchangeOutlookAnywhere)
			{
				this.SupportsCutover = MigrationSession.SupportsCutover(base.DataProvider);
			}
			if (this.ExchangeOutlookAnywhere)
			{
				this.InternalProcessExchangeOutlookAnywhere(base.DataProvider);
				return;
			}
			if (this.ExchangeRemoteMove && this.Autodiscover)
			{
				this.InternalProcessExchangeRemoteMoveAutoDiscover();
				return;
			}
			if (this.PSTImport)
			{
				this.InternalProcessPSTImport();
				return;
			}
			if (this.PublicFolder)
			{
				this.InternalProcessPublicFolder();
				return;
			}
			if (this.endpoint != null)
			{
				this.InternalProcessEndpoint(false);
				return;
			}
			MigrationLogger.Log(MigrationEventType.Error, "TestMigrationServerAvailability.InternalProcessRecord: Wrong protocol", new object[0]);
			this.WriteError(new TestMigrationServerAvailabilityProtocolArgumentException());
		}

		private TestMigrationServerAvailabilityOutcome CreateAutodsicoverFailedOutcome(LocalizedException exception)
		{
			LocalizedString message = exception.Data.Contains("AutoDiscoverResponseMessage") ? ((LocalizedString)exception.Data["AutoDiscoverResponseMessage"]) : exception.LocalizedString;
			string errorDetail = exception.Data.Contains("AutoDiscoverResponseErrorDetail") ? ((string)exception.Data["AutoDiscoverResponseErrorDetail"]) : exception.ToString();
			return TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Failed, this.SupportsCutover, message, errorDetail);
		}

		private void ValidateEndpoint()
		{
			using (MigrationEndpointDataProvider migrationEndpointDataProvider = MigrationEndpointDataProvider.CreateDataProvider("Test-MigrationServerAvailability", this.RecipientSession, this.partitionMailbox))
			{
				this.endpoint = (MigrationEndpoint)base.GetDataObject<MigrationEndpoint>(this.Endpoint, migrationEndpointDataProvider, null, new LocalizedString?(Strings.MigrationEndpointNotFound(this.Endpoint.RawIdentity)), new LocalizedString?(Strings.MigrationEndpointIdentityAmbiguous(this.Endpoint.RawIdentity)));
			}
		}

		private void InternalProcessEndpoint(bool fromAutoDiscover)
		{
			TestMigrationServerAvailabilityOutcome sendToPipeline;
			try
			{
				this.endpoint.VerifyConnectivity();
				ExchangeConnectionSettings connectionSettings = this.endpoint.ConnectionSettings as ExchangeConnectionSettings;
				sendToPipeline = TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Success, this.SupportsCutover, connectionSettings);
			}
			catch (LocalizedException ex)
			{
				if (fromAutoDiscover)
				{
					ex = new MigrationRemoteEndpointSettingsCouldNotBeAutodiscoveredException(this.endpoint.RemoteServer.ToString(), ex);
				}
				sendToPipeline = TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Failed, this.SupportsCutover, ex.LocalizedString, ex.ToString());
			}
			base.WriteObject(sendToPipeline);
		}

		private void InternalProcessExchangeRemoteMoveAutoDiscover()
		{
			try
			{
				ExchangeRemoteMoveEndpoint exchangeRemoteMoveEndpoint = new ExchangeRemoteMoveEndpoint();
				exchangeRemoteMoveEndpoint.InitializeFromAutoDiscover(this.EmailAddress, this.Credentials);
				this.endpoint = exchangeRemoteMoveEndpoint;
			}
			catch (LocalizedException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex, "Failed to determine remote endpoint via auto-discover.", new object[0]);
				base.WriteObject(TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Failed, this.SupportsCutover, ex.LocalizedString, ex.ToString()));
				return;
			}
			this.InternalProcessEndpoint(true);
		}

		private void InternalProcessPSTImport()
		{
			this.InternalProcessEndpoint(false);
			if (!string.IsNullOrEmpty(this.FilePath))
			{
				PSTImportEndpoint pstimportEndpoint = this.endpoint as PSTImportEndpoint;
				this.TestPstImportSubscription(base.DataProvider, pstimportEndpoint, this.targetMailbox);
			}
		}

		private void InternalProcessPublicFolder()
		{
			TestMigrationServerAvailabilityOutcome sendToPipeline;
			try
			{
				TestMigrationServerAvailability.VerifyPublicFolderConnection(base.DataProvider, (PublicFolderEndpoint)this.endpoint, this.SourceMailboxLegacyDN, this.PublicFolderDatabaseServerLegacyDN, this.targetMailbox);
				ExchangeConnectionSettings connectionSettings = this.endpoint.ConnectionSettings as ExchangeConnectionSettings;
				sendToPipeline = TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Success, this.SupportsCutover, connectionSettings);
			}
			catch (LocalizedException ex)
			{
				sendToPipeline = TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Failed, this.SupportsCutover, ex.LocalizedString, ex.ToString());
			}
			base.WriteObject(sendToPipeline);
		}

		private void ValidateRemoteServerConstraint(string remoteServer, StorePropertyDefinition propertyDefinition, string propertyName)
		{
			if (remoteServer == null)
			{
				this.WriteError(new MissingParameterException(propertyName));
			}
			ValidationError validationError = MigrationConstraints.RemoteServerNameConstraint.Validate(remoteServer, propertyDefinition, null);
			if (validationError != null)
			{
				this.WriteError(new MigrationPermanentException(Strings.MigrationRemoteServerTooLongException(propertyName)));
			}
		}

		private void InternalProcessExchangeOutlookAnywhere(IMigrationDataProvider dataProvider)
		{
			ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = new ExchangeOutlookAnywhereEndpoint();
			try
			{
				if (this.Autodiscover)
				{
					TestMigrationServerAvailabilityOutcome testMigrationServerAvailabilityOutcome = null;
					try
					{
						exchangeOutlookAnywhereEndpoint.InitializeFromAutoDiscover(this.EmailAddress, this.Credentials);
					}
					catch (AutoDiscoverFailedConfigurationErrorException exception)
					{
						testMigrationServerAvailabilityOutcome = this.CreateAutodsicoverFailedOutcome(exception);
					}
					catch (AutoDiscoverFailedInternalErrorException exception2)
					{
						testMigrationServerAvailabilityOutcome = this.CreateAutodsicoverFailedOutcome(exception2);
					}
					if (testMigrationServerAvailabilityOutcome != null)
					{
						MigrationLogger.Log(MigrationEventType.Information, testMigrationServerAvailabilityOutcome.ToString(), new object[0]);
						base.WriteObject(testMigrationServerAvailabilityOutcome);
						return;
					}
				}
				else
				{
					exchangeOutlookAnywhereEndpoint.RpcProxyServer = this.RPCProxyServer;
					exchangeOutlookAnywhereEndpoint.Credentials = this.Credentials;
					exchangeOutlookAnywhereEndpoint.ExchangeServer = this.ExchangeServer;
					exchangeOutlookAnywhereEndpoint.AuthenticationMethod = this.Authentication;
				}
				IMigrationNspiClient nspiClient = MigrationServiceFactory.Instance.GetNspiClient(null);
				exchangeOutlookAnywhereEndpoint.NspiServer = nspiClient.GetNewDSA(exchangeOutlookAnywhereEndpoint);
				exchangeOutlookAnywhereEndpoint.MailboxPermission = this.MailboxPermission;
				NspiMigrationDataReader nspiDataReader = exchangeOutlookAnywhereEndpoint.GetNspiDataReader(null);
				nspiDataReader.Ping();
				ExchangeOutlookAnywhereEndpoint.ValidateEndpoint(exchangeOutlookAnywhereEndpoint);
			}
			catch (MigrationTransientException ex)
			{
				MigrationLogger.Log(MigrationEventType.Warning, MigrationLogger.GetDiagnosticInfo(ex, null), new object[0]);
				base.WriteObject(TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Failed, this.SupportsCutover, ex.LocalizedString, ex.InternalError));
				return;
			}
			catch (MigrationPermanentException ex2)
			{
				MigrationLogger.Log(MigrationEventType.Error, MigrationLogger.GetDiagnosticInfo(ex2, null), new object[0]);
				base.WriteObject(TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Failed, this.SupportsCutover, ex2.LocalizedString, ex2.InternalError));
				return;
			}
			TestMigrationServerAvailabilityOutcome testMigrationServerAvailabilityOutcome2;
			try
			{
				TestMigrationServerAvailability.VerifyExchangeOutlookAnywhereConnection(dataProvider, exchangeOutlookAnywhereEndpoint, (string)this.EmailAddress, this.SourceMailboxLegacyDN, this.targetMailbox, !this.IsFieldSet("MailboxPermission"));
				testMigrationServerAvailabilityOutcome2 = TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Success, this.SupportsCutover, (ExchangeConnectionSettings)exchangeOutlookAnywhereEndpoint.ConnectionSettings);
			}
			catch (LocalizedException ex3)
			{
				string diagnosticInfo = MigrationLogger.GetDiagnosticInfo(ex3, null);
				MigrationLogger.Log(MigrationEventType.Error, diagnosticInfo, new object[0]);
				testMigrationServerAvailabilityOutcome2 = TestMigrationServerAvailabilityOutcome.Create(TestMigrationServerAvailabilityResult.Failed, this.SupportsCutover, ex3.LocalizedString, diagnosticInfo);
				testMigrationServerAvailabilityOutcome2.ConnectionSettings = (ExchangeConnectionSettings)exchangeOutlookAnywhereEndpoint.ConnectionSettings;
			}
			base.WriteObject(testMigrationServerAvailabilityOutcome2);
		}

		internal static MailboxData DiscoverTestMailbox(IIdentityParameter identity, IRecipientSession adSession, ADServerSettings serverSettings, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, Task.TaskVerboseLoggingDelegate writeVerbose, Task.ErrorLoggerDelegate writeError)
		{
			if (identity == null)
			{
				MigrationADProvider migrationADProvider = new MigrationADProvider(adSession);
				return migrationADProvider.GetMailboxDataForManagementMailbox();
			}
			ADUser aduser = RequestTaskHelper.ResolveADUser(adSession, adSession, serverSettings, identity, null, null, getDataObject, writeVerbose, writeError, true);
			MailboxData mailboxData = new MailboxData(aduser.ExchangeGuid, new Fqdn(aduser.ServerName), aduser.LegacyExchangeDN, aduser.Id, aduser.ExchangeObjectId);
			mailboxData.Update(identity.RawIdentity, aduser.OrganizationId);
			return mailboxData;
		}

		internal static MailboxData DiscoverPublicFolderTestMailbox(IIdentityParameter identity, IConfigurationSession configurationSession, IRecipientSession recipientSession, ADServerSettings serverSettings, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, Task.TaskVerboseLoggingDelegate writeVerbose, Task.ErrorLoggerDelegate writeError)
		{
			if (identity == null)
			{
				Organization orgContainer = configurationSession.GetOrgContainer();
				if (orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid != default(Guid))
				{
					identity = new MailboxIdParameter(orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid.ToString());
				}
				else
				{
					writeError(new MigrationPermanentException(Strings.ErrorUnableToFindValidPublicFolderMailbox), ExchangeErrorCategory.Client, null);
				}
			}
			ADUser aduser = RequestTaskHelper.ResolveADUser(recipientSession, recipientSession, serverSettings, identity, new OptionalIdentityData(), null, getDataObject, writeVerbose, writeError, true);
			if (aduser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
			{
				writeError(new MigrationPermanentException(Strings.ErrorNotPublicFolderMailbox(identity.RawIdentity)), ExchangeErrorCategory.Client, null);
			}
			return MailboxData.CreateFromADUser(aduser);
		}

		internal static bool VerifyExchangeOutlookAnywhereConnection(IMigrationDataProvider dataProvider, ExchangeOutlookAnywhereEndpoint outlookAnywhereEndpoint, string emailAddress, string sourceMailboxLegacyDN, MailboxData targetMailbox, bool discoverAdminPrivilege)
		{
			string mailboxDN;
			if (!string.IsNullOrEmpty(sourceMailboxLegacyDN))
			{
				mailboxDN = sourceMailboxLegacyDN;
			}
			else if (!string.IsNullOrEmpty(emailAddress) && (!outlookAnywhereEndpoint.UseAutoDiscover || outlookAnywhereEndpoint.AutodiscoverResponse == null))
			{
				NspiMigrationDataReader nspiDataReader = outlookAnywhereEndpoint.GetNspiDataReader(null);
				IMigrationDataRow recipient = nspiDataReader.GetRecipient(emailAddress);
				NspiMigrationDataRow nspiMigrationDataRow = recipient as NspiMigrationDataRow;
				if (nspiMigrationDataRow == null)
				{
					LocalizedString localizedErrorMessage = LocalizedString.Empty;
					InvalidDataRow invalidDataRow = recipient as InvalidDataRow;
					if (invalidDataRow != null && invalidDataRow.Error != null)
					{
						LocalizedString localizedErrorMessage2 = invalidDataRow.Error.LocalizedErrorMessage;
						localizedErrorMessage = invalidDataRow.Error.LocalizedErrorMessage;
					}
					throw new MigrationPermanentException(localizedErrorMessage);
				}
				mailboxDN = nspiMigrationDataRow.Recipient.GetPropertyValue<string>(PropTag.EmailAddress);
			}
			else
			{
				mailboxDN = outlookAnywhereEndpoint.AutodiscoverResponse.MailboxDN;
			}
			ExchangeJobItemSubscriptionSettings subscriptionSettings = ExchangeJobItemSubscriptionSettings.CreateFromProperties(mailboxDN, outlookAnywhereEndpoint.ExchangeServer, outlookAnywhereEndpoint.ExchangeServer, outlookAnywhereEndpoint.RpcProxyServer);
			bool flag = false;
			try
			{
				IL_C3:
				MRSMergeRequestAccessor mrsmergeRequestAccessor = new MRSMergeRequestAccessor(dataProvider, null, false);
				mrsmergeRequestAccessor.TestCreateSubscription(outlookAnywhereEndpoint, null, subscriptionSettings, targetMailbox, null, false);
			}
			catch (LocalizedException ex)
			{
				if (discoverAdminPrivilege && !flag && CommonUtils.GetExceptionSide(ex) == ExceptionSide.Source)
				{
					Exception ex2 = ex;
					while (ex2.InnerException != null)
					{
						ex2 = ex2.InnerException;
					}
					if (CommonUtils.ExceptionIs(ex2, new WellKnownException[]
					{
						WellKnownException.MapiLogonFailed
					}))
					{
						if (outlookAnywhereEndpoint.MailboxPermission == MigrationMailboxPermission.Admin)
						{
							outlookAnywhereEndpoint.MailboxPermission = MigrationMailboxPermission.FullAccess;
						}
						else
						{
							outlookAnywhereEndpoint.MailboxPermission = MigrationMailboxPermission.Admin;
						}
						flag = true;
						goto IL_C3;
					}
				}
				throw;
			}
			return flag;
		}

		internal static void VerifyPublicFolderConnection(IMigrationDataProvider dataProvider, PublicFolderEndpoint publicFolderEndpoint, string sourceMailboxLegacyDn, string publicFolderDatabaseServerLegacyDn, MailboxData mailboxData)
		{
			MrsPublicFolderAccessor mrsPublicFolderAccessor = new MrsPublicFolderAccessor(dataProvider, "TestMigrationServerAvailability");
			using (Stream stream = new MemoryStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					streamWriter.WriteLine("\"{0}\",\"{1}\"", "FolderPath", "TargetMailbox");
					streamWriter.WriteLine("\"\\\",\"{0}\"", mailboxData.Name);
					streamWriter.Flush();
					stream.Seek(0L, SeekOrigin.Begin);
					mrsPublicFolderAccessor.TestCreateSubscription(publicFolderEndpoint, mailboxData, stream);
				}
			}
		}

		private void ValidateAutodiscoverParameters()
		{
			ValidationError validationError = MigrationConstraints.NameLengthConstraint.Validate(this.EmailAddress.ToString(), MigrationBatchMessageSchema.MigrationJobExchangeEmailAddress, null);
			if (validationError != null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationRemoteServerTooLongException("EmailAddress")), (ErrorCategory)1000, null);
			}
		}

		private void ValidateImapParameters()
		{
			if (base.CurrentOrganizationId == null || OrganizationId.ForestWideOrgId.Equals(base.CurrentOrganizationId))
			{
				this.WriteError(new InvalidOrganizationException());
			}
			this.ValidateRemoteServerConstraint(this.RemoteServer.ToString(), MigrationBatchMessageSchema.MigrationJobRemoteServerHostName, "RemoteServer");
			ValidationError validationError = MigrationConstraints.PortRangeConstraint.Validate(this.Port, MigrationBatchMessageSchema.MigrationJobRemoteServerPortNumber, null);
			if (validationError != null)
			{
				this.WriteError(new MigrationPermanentException(Strings.MigrationPortVerificationFailed(this.Port, MigrationConstraints.PortRangeConstraint.MinimumValue, MigrationConstraints.PortRangeConstraint.MaximumValue)));
			}
			this.endpoint = new ImapEndpoint
			{
				RemoteServer = this.RemoteServer,
				Credentials = this.Credentials,
				Port = this.Port,
				Security = this.Security,
				AuthenticationMethod = this.Authentication
			};
		}

		private void ValidateExchangeOutlookAnywhereParameters()
		{
			if (!this.Autodiscover)
			{
				this.ValidateRemoteServerConstraint(this.RPCProxyServer.ToString(), MigrationBatchMessageSchema.MigrationJobExchangeRPCProxyServerHostName, "RPCProxyServer");
				this.ValidateRemoteServerConstraint(this.ExchangeServer, MigrationBatchMessageSchema.MigrationJobExchangeRemoteServerHostName, "ExchangeServer");
				if (this.EmailAddress == SmtpAddress.Empty && string.IsNullOrEmpty(this.SourceMailboxLegacyDN))
				{
					this.WriteError(new MigrationPermanentException(Strings.MigrationMustSpecifyEmailOrMailboxDN));
				}
			}
			else
			{
				this.ValidateAutodiscoverParameters();
			}
			if (base.CurrentOrganizationId == null || OrganizationId.ForestWideOrgId.Equals(base.CurrentOrganizationId))
			{
				this.WriteError(new InvalidOrganizationException());
			}
			this.targetMailbox = TestMigrationServerAvailability.DiscoverTestMailbox(this.TestMailbox, this.RecipientSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
		}

		private void ValidateExchangeRemoteMoveParameters()
		{
			if (!this.Autodiscover)
			{
				this.ValidateRemoteServerConstraint(this.RemoteServer.ToString(), MigrationBatchMessageSchema.MigrationJobRemoteServerHostName, "RemoteServer");
				this.endpoint = new ExchangeRemoteMoveEndpoint
				{
					RemoteServer = this.RemoteServer,
					Credentials = this.Credentials
				};
				return;
			}
			this.ValidateAutodiscoverParameters();
		}

		private void ValidatePSTImportParameters()
		{
			this.ValidateRemoteServerConstraint(this.RemoteServer.ToString(), MigrationBatchMessageSchema.MigrationJobRemoteServerHostName, "RemoteServer");
			this.endpoint = new PSTImportEndpoint
			{
				RemoteServer = this.RemoteServer,
				Credentials = this.Credentials
			};
			this.targetMailbox = TestMigrationServerAvailability.DiscoverTestMailbox(this.TestMailbox, this.RecipientSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
		}

		private void ValidatePublicFolderParameters()
		{
			this.ValidateRemoteServerConstraint(this.RPCProxyServer, MigrationBatchMessageSchema.MigrationJobExchangeRPCProxyServerHostName, "RpcProxyServer");
			if (!LegacyDN.IsValidLegacyDN(this.SourceMailboxLegacyDN))
			{
				this.WriteError(new InvalidLegacyExchangeDnValueException("SourceMailboxLegacyDN"));
			}
			if (!LegacyDN.IsValidLegacyDN(this.PublicFolderDatabaseServerLegacyDN))
			{
				this.WriteError(new InvalidLegacyExchangeDnValueException("PublicFolderDatabaseServerLegacyDN"));
			}
			this.endpoint = new PublicFolderEndpoint
			{
				RpcProxyServer = this.RPCProxyServer,
				AuthenticationMethod = this.Authentication,
				SourceMailboxLegacyDN = this.SourceMailboxLegacyDN,
				PublicFolderDatabaseServerLegacyDN = this.PublicFolderDatabaseServerLegacyDN,
				Credentials = this.Credentials
			};
			this.targetMailbox = TestMigrationServerAvailability.DiscoverPublicFolderTestMailbox(this.TestMailbox, this.ConfigurationSession, this.RecipientSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
		}

		private bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		private void TestPstImportSubscription(IMigrationDataProvider dataProvider, PSTImportEndpoint endpoint, MailboxData targetMailbox)
		{
			PSTJobItemSubscriptionSettings jobItemSettings = PSTJobItemSubscriptionSettings.CreateFromProperties(this.FilePath);
			PSTImportAccessor pstimportAccessor = new PSTImportAccessor(dataProvider, null);
			pstimportAccessor.TestCreateSubscription(endpoint, null, jobItemSettings, targetMailbox, "PSTImport");
		}

		private const string ParameterSetNameTestEndpoint = "TestEndpoint";

		private const string ParameterSetNameImap = "IMAP";

		private const string ParameterSetNameExchangeOutlookAnywhere = "ExchangeOutlookAnywhere";

		private const string ParameterSetNameExchangeOutlookAnywhereAutoDiscover = "ExchangeOutlookAnywhereAutoDiscover";

		private const string ParameterSetNameExchangeRemoteMove = "ExchangeRemoteMove";

		private const string ParameterSetNameExchangeRemoteMoveAutoDiscover = "ExchangeRemoteMoveAutoDiscover";

		private const string ParameterSetNamePstImport = "PSTImport";

		private const string ParameterSetNamePublicFolder = "PublicFolder";

		private const string ParameterNameMailboxPermission = "MailboxPermission";

		private const string ParameterNameEmailAddress = "EmailAddress";

		private const string ParameterNameSourceMailboxLegacyDN = "SourceMailboxLegacyDN";

		private const string ParameterNamePublicFolderDatabaseServerLegacyDN = "PublicFolderDatabaseServerLegacyDN";

		private MailboxData targetMailbox;

		private MigrationEndpointBase endpoint;
	}
}
