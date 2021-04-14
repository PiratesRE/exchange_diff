using System;
using System.IO;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Extension
{
	[Cmdlet("New", "App", DefaultParameterSetName = "ExtensionFileData", SupportsShouldProcess = true)]
	public sealed class NewApp : NewTenantADTaskBase<App>
	{
		[Parameter(Mandatory = false, ParameterSetName = "ExtensionOfficeMarketplace")]
		public string MarketplaceAssetID
		{
			get
			{
				return this.DataObject.MarketplaceAssetID;
			}
			set
			{
				this.DataObject.MarketplaceAssetID = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExtensionOfficeMarketplace")]
		public string MarketplaceQueryMarket { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter DownloadOnly { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "ExtensionOfficeMarketplace")]
		public string Etoken { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "ExtensionOfficeMarketplace")]
		public string MarketplaceServicesUrl { get; set; }

		[Parameter(Mandatory = false)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExtensionFileData")]
		public byte[] FileData
		{
			get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExtensionFileStream")]
		public Stream FileStream
		{
			get
			{
				return (Stream)base.Fields["FileStream"];
			}
			set
			{
				base.Fields["FileStream"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExtensionPrivateURL")]
		public Uri Url { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter OrganizationApp
		{
			get
			{
				return (SwitchParameter)(base.Fields["OrganizationApp"] ?? false);
			}
			set
			{
				base.Fields["OrganizationApp"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ClientExtensionProvidedTo ProvidedTo
		{
			get
			{
				return (ClientExtensionProvidedTo)(base.Fields["ProvidedTo"] ?? ClientExtensionProvidedTo.Everyone);
			}
			set
			{
				base.Fields["ProvidedTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> UserList
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>>)base.Fields["UserList"];
			}
			set
			{
				base.Fields["UserList"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DefaultStateForUser DefaultStateForUser
		{
			get
			{
				return (DefaultStateForUser)(base.Fields["DefaultStateForUser"] ?? DefaultStateForUser.Disabled);
			}
			set
			{
				base.Fields["DefaultStateForUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)(base.Fields["Enabled"] ?? true);
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AllowReadWriteMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["AllowReadWriteMailbox"] ?? false);
			}
			set
			{
				base.Fields["AllowReadWriteMailbox"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId executingUserId;
			if (!base.TryGetExecutingUserId(out executingUserId) && this.Mailbox == null)
			{
				return this.CreateDataProviderForNonMailboxUser();
			}
			MailboxIdParameter mailboxIdParameter = this.Mailbox ?? MailboxTaskHelper.ResolveMailboxIdentity(executingUserId, new Task.ErrorLoggerDelegate(base.WriteError));
			try
			{
				this.adUser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			}
			catch (ManagementObjectNotFoundException)
			{
				return this.CreateDataProviderForNonMailboxUser();
			}
			this.isBposUser = CapabilityHelper.HasBposSKUCapability(this.adUser.PersistedCapabilities);
			ADScopeException ex;
			if (!TaskHelper.UnderscopeSessionToOrganization(base.TenantGlobalCatalogSession, this.adUser.OrganizationId, true).TryVerifyIsWithinScopes(this.adUser, true, out ex))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotChangeMailboxOutOfWriteScope(this.adUser.Identity.ToString(), (ex == null) ? string.Empty : ex.Message), ex), ErrorCategory.InvalidOperation, this.adUser.Identity);
			}
			IConfigDataProvider configDataProvider = GetApp.CreateOwaExtensionDataProvider(null, base.TenantGlobalCatalogSession, base.SessionSettings, !this.OrganizationApp, this.adUser, "New-App", false, new Task.ErrorLoggerDelegate(base.WriteError));
			this.mailboxOwner = ((OWAExtensionDataProvider)configDataProvider).MailboxSession.MailboxOwner.ObjectId.ToString();
			return configDataProvider;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, (this.adUser != null) ? this.adUser.OrganizationId : base.ExecutingUserOrganizationId, base.ExecutingUserOrganizationId, false);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.FullyConsistent, sessionSettings, 281, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Extension\\NewApp.cs");
			if (!tenantOrTopologyConfigurationSession.GetOrgContainer().AppsForOfficeEnabled)
			{
				this.WriteWarning(Strings.WarningExtensionFeatureDisabled);
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			base.CheckExclusiveParameters(new object[]
			{
				"Mailbox",
				"OrganizationApp"
			});
			if (!this.OrganizationApp && (base.Fields.IsModified("UserList") || base.Fields.IsModified("ProvidedTo") || base.Fields.IsModified("DefaultStateForUser")))
			{
				base.WriteError(new LocalizedException(Strings.ErrorCannotSpecifyParameterWithoutOrgExtensionParameter), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields.IsModified("UserList") && this.UserList != null && this.UserList.Count > 1000)
			{
				base.WriteError(new LocalizedException(Strings.ErrorTooManyUsersInUserList(1000)), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			try
			{
				if (this.FileData != null)
				{
					return this.InstallFromFile(new MemoryStream(this.FileData));
				}
				if (this.FileStream != null)
				{
					return this.InstallFromFile(this.FileStream);
				}
				if (this.MarketplaceAssetID != null && this.MarketplaceServicesUrl != null)
				{
					if (string.IsNullOrWhiteSpace(this.MarketplaceQueryMarket))
					{
						this.MarketplaceQueryMarket = "en-us";
					}
					using (Stream stream = this.DownloadDataFromOfficeMarketPlace(this.MarketplaceAssetID, this.MarketplaceQueryMarket, this.MarketplaceServicesUrl, this.Etoken))
					{
						return this.ReadManifest(stream, ExtensionType.MarketPlace);
					}
				}
				if (null == this.Url)
				{
					base.WriteError(new LocalizedException(Strings.ErrorNoInputForExtensionInstall), ErrorCategory.InvalidOperation, null);
				}
				using (Stream stream2 = this.DownloadDataFromUri(this.Url))
				{
					return this.ReadManifest(stream2, ExtensionType.Private);
				}
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			return null;
		}

		protected override void WriteResult()
		{
			if (this.DownloadOnly)
			{
				this.WriteResult(this.DataObject);
				return;
			}
			base.WriteResult();
		}

		private Stream DownloadDataFromOfficeMarketPlace(string marketplaceAssetID, string marketplaceQueryMarket, string marketplaceServicesUrl, string etoken)
		{
			IConfigDataProvider currentDataSession = base.DataSession;
			return this.DownloadDataFromUri(delegate()
			{
				SynchronousDownloadData synchronousDownloadData = new SynchronousDownloadData();
				string domain = ((OWAExtensionDataProvider)currentDataSession).MailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.Domain;
				string deploymentId = ExtensionDataHelper.GetDeploymentId(domain);
				return synchronousDownloadData.Execute(marketplaceServicesUrl, marketplaceAssetID, marketplaceQueryMarket, deploymentId, etoken);
			});
		}

		private Stream DownloadDataFromUri(Uri uri)
		{
			return this.DownloadDataFromUri(() => SynchronousDownloadData.DownloadDataFromUri(uri, 262144L, new Func<long, bool, bool>(ExtensionData.ValidateManifestSize), true, this.isBposUser));
		}

		private IConfigDataProvider CreateDataProviderForNonMailboxUser()
		{
			if (!this.OrganizationApp)
			{
				base.WriteError(new LocalizedException(Strings.ErrorAppTargetMailboxNotFound("OrganizationApp", "Mailbox")), ErrorCategory.InvalidArgument, null);
			}
			if (base.IsDebugOn)
			{
				base.WriteDebug("Creating data provider for non mailbox user.");
			}
			return new OWAAppDataProviderForNonMailboxUser(null, base.TenantGlobalCatalogSession, base.SessionSettings, !this.OrganizationApp, "New-App");
		}

		private Stream DownloadDataFromUri(Func<MemoryStream> downloadCallback)
		{
			MemoryStream result = null;
			try
			{
				try
				{
					result = downloadCallback();
				}
				catch (WebException ex)
				{
					if (WebExceptionStatus.TrustFailure == ex.Status)
					{
						base.WriteError(new LocalizedException(Strings.ErrorServerCertificateError), ErrorCategory.InvalidData, null);
					}
					throw;
				}
			}
			catch
			{
				base.WriteError(new LocalizedException(Strings.ErrorCanNotDownloadPackage), ErrorCategory.InvalidData, null);
			}
			return result;
		}

		private IConfigurable ReadManifest(Stream source, ExtensionType extensionType)
		{
			ExtensionData extensionData = ExtensionData.ParseOsfManifest(source, this.MarketplaceAssetID, this.MarketplaceQueryMarket, extensionType, this.OrganizationApp ? ExtensionInstallScope.Organization : ExtensionInstallScope.User, true, DisableReasonType.NotDisabled, string.Empty, null);
			if (this.OrganizationApp)
			{
				return new OrgApp(new DefaultStateForUser?(this.DefaultStateForUser), this.ProvidedTo, OrgApp.ConvertUserListToPresentationFormat(this, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), this.UserList), extensionData.MarketplaceAssetID, extensionData.MarketplaceContentMarket, extensionData.ProviderName, extensionData.IconURL, extensionData.ExtensionId, extensionData.VersionAsString, extensionData.Type, extensionData.Scope, extensionData.RequestedCapabilities, extensionData.DisplayName, extensionData.Description, this.Enabled, extensionData.Manifest.OuterXml, this.Etoken, null, extensionData.AppStatus, (this.adUser != null) ? this.adUser.Id : new ADObjectId())
				{
					IsDownloadOnly = this.DownloadOnly
				};
			}
			if (!base.ExchangeRunspaceConfig.HasRoleOfType(RoleType.OrgMarketplaceApps) && !this.AllowReadWriteMailbox && RequestedCapabilities.ReadWriteMailbox == extensionData.RequestedCapabilities)
			{
				throw new OwaExtensionOperationException(Strings.ErrorReasonUserNotAllowedToInstallReadWriteMailbox);
			}
			return new App(null, extensionData.MarketplaceAssetID, extensionData.MarketplaceContentMarket, extensionData.ProviderName, extensionData.IconURL, extensionData.ExtensionId, extensionData.VersionAsString, extensionData.Type, extensionData.Scope, extensionData.RequestedCapabilities, extensionData.DisplayName, extensionData.Description, this.Enabled, extensionData.Manifest.OuterXml, this.adUser.Id, this.Etoken, null, extensionData.AppStatus)
			{
				IsDownloadOnly = this.DownloadOnly
			};
		}

		private IConfigurable InstallFromFile(Stream manifestStream)
		{
			if (manifestStream == null || manifestStream == Stream.Null)
			{
				base.WriteError(new LocalizedException(Strings.ErrorMissingFile), ErrorCategory.InvalidOperation, null);
			}
			Exception ex = null;
			try
			{
				ExtensionData.ValidateManifestSize(manifestStream.Length, true);
				return this.ReadManifest(manifestStream, ExtensionType.Private);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			base.WriteError(new LocalizedException(Strings.ErrorCannotReadManifestStream(ex.Message)), ErrorCategory.InvalidOperation, null);
			return null;
		}

		protected override void InternalProcessRecord()
		{
			OWAExtensionHelper.ProcessRecord(new Action(base.InternalProcessRecord), new Task.TaskErrorLoggingDelegate(base.WriteError), this.DataObject.Identity);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			OWAExtensionHelper.CleanupOWAExtensionDataProvider(base.DataSession);
			GC.SuppressFinalize(this);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.OrganizationApp)
				{
					return Strings.ConfirmationMessageInstallOwaOrgExtension;
				}
				return Strings.ConfirmationMessageInstallOwaExtension(this.mailboxOwner);
			}
		}

		internal const string MailboxParameterName = "Mailbox";

		internal const string OrganizationAppParameterName = "OrganizationApp";

		internal const int MaxUserListCount = 1000;

		internal const string TypeAttributeName = "xsi:type";

		internal const string DefaultMarketplaceQueryMarket = "en-us";

		internal const string ItemTypeAttributeName = "ItemType";

		public const string FileDataParameterSetName = "ExtensionFileData";

		public const string OfficeMarketplaceParameterSetName = "ExtensionOfficeMarketplace";

		public const string PrivateURLParameterSetName = "ExtensionPrivateURL";

		public const string FileStreamParameterSetName = "ExtensionFileStream";

		private ADUser adUser;

		private string mailboxOwner;

		private bool isBposUser;
	}
}
