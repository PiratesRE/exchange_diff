using System;
using System.Management.Automation;
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
	[Cmdlet("Set", "App", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetApp : SetTenantADTaskBase<AppIdParameter, App, App>
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

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
				return (ClientExtensionProvidedTo)base.Fields["ProvidedTo"];
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
				return (DefaultStateForUser)base.Fields["DefaultStateForUser"];
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
				return (bool)base.Fields["Enabled"];
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADUser adUser = null;
			ADObjectId executingUserId;
			if (!base.TryGetExecutingUserId(out executingUserId))
			{
				return this.CreateDataProviderForNonMailboxUser();
			}
			MailboxIdParameter mailboxIdParameter = MailboxTaskHelper.ResolveMailboxIdentity(executingUserId, new Task.ErrorLoggerDelegate(base.WriteError));
			try
			{
				adUser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			}
			catch (ManagementObjectNotFoundException)
			{
				return this.CreateDataProviderForNonMailboxUser();
			}
			if (this.Identity != null && this.Identity.InternalOWAExtensionId == null)
			{
				this.Identity.InternalOWAExtensionId = OWAExtensionHelper.CreateOWAExtensionId(this, new ADObjectId(), null, this.Identity.RawExtensionName);
			}
			if (this.Organization != null)
			{
				this.SetCurrentOrganizationId();
			}
			return GetApp.CreateOwaExtensionDataProvider(this.Organization, base.TenantGlobalCatalogSession, base.SessionSettings, false, adUser, "Set-App", false, new Task.ErrorLoggerDelegate(base.WriteError));
		}

		protected override IConfigurable PrepareDataObject()
		{
			OrgApp orgApp = (OrgApp)base.PrepareDataObject();
			if (base.Fields.IsModified("Enabled"))
			{
				orgApp.Enabled = this.Enabled;
			}
			if (base.Fields.IsModified("DefaultStateForUser"))
			{
				orgApp.DefaultStateForUser = new DefaultStateForUser?(this.DefaultStateForUser);
			}
			if (base.Fields.IsModified("ProvidedTo"))
			{
				orgApp.ProvidedTo = this.ProvidedTo;
			}
			if (base.Fields.IsModified("UserList"))
			{
				if (this.UserList != null && this.UserList.Count > 1000)
				{
					base.WriteError(new LocalizedException(Strings.ErrorTooManyUsersInUserList(1000)), ErrorCategory.InvalidArgument, null);
				}
				orgApp.UserList = OrgApp.ConvertUserListToPresentationFormat(this, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), this.UserList);
			}
			return orgApp;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!this.OrganizationApp)
			{
				base.WriteError(new LocalizedException(Strings.ErrorParameterRequired("OrganizationApp")), ErrorCategory.InvalidArgument, null);
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.FullyConsistent, sessionSettings, 230, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Extension\\setapp.cs");
			if (!tenantOrTopologyConfigurationSession.GetOrgContainer().AppsForOfficeEnabled)
			{
				this.WriteWarning(Strings.WarningExtensionFeatureDisabled);
			}
		}

		protected override void InternalProcessRecord()
		{
			OWAExtensionHelper.ProcessRecord(new Action(base.InternalProcessRecord), new Task.TaskErrorLoggingDelegate(base.WriteError), this.Identity);
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
				return Strings.ConfirmationMessageModifyOwaOrgExtension(this.Identity.ToString());
			}
		}

		private void SetCurrentOrganizationId()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 283, "SetCurrentOrganizationId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Extension\\setapp.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
			base.CurrentOrganizationId = adorganizationalUnit.OrganizationId;
		}

		private IConfigDataProvider CreateDataProviderForNonMailboxUser()
		{
			if (!this.OrganizationApp)
			{
				base.WriteError(new LocalizedException(Strings.ErrorParameterRequired("OrganizationApp")), ErrorCategory.InvalidArgument, null);
			}
			if (base.IsDebugOn)
			{
				base.WriteDebug("Creating data provider for non mailbox user.");
			}
			IConfigDataProvider result = new OWAAppDataProviderForNonMailboxUser((this.Organization == null) ? null : this.Organization.RawIdentity, base.TenantGlobalCatalogSession, base.SessionSettings, !this.OrganizationApp, "Set-App");
			if (this.Identity != null && this.Identity.InternalOWAExtensionId == null)
			{
				this.Identity.InternalOWAExtensionId = OWAExtensionHelper.CreateOWAExtensionId(this, new ADObjectId(), null, this.Identity.RawExtensionName);
			}
			return result;
		}
	}
}
