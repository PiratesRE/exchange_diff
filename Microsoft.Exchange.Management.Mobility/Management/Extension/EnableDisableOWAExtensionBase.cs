using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Extension
{
	public abstract class EnableDisableOWAExtensionBase : ObjectActionTenantADTask<AppIdParameter, App>
	{
		public EnableDisableOWAExtensionBase(bool enabled)
		{
			this.enabled = enabled;
		}

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

		protected override IConfigDataProvider CreateSession()
		{
			MailboxIdParameter mailboxIdParameter = null;
			if (this.Identity != null)
			{
				if (this.Identity.InternalOWAExtensionId != null)
				{
					mailboxIdParameter = new MailboxIdParameter(this.Identity.InternalOWAExtensionId.MailboxOwnerId);
				}
				else
				{
					mailboxIdParameter = this.Identity.RawMailbox;
				}
			}
			if (mailboxIdParameter != null && this.Mailbox != null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorConflictingMailboxes), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (mailboxIdParameter == null)
			{
				ADObjectId executingUserId;
				if (!base.TryGetExecutingUserId(out executingUserId) && this.Mailbox == null)
				{
					base.WriteError(new LocalizedException(Strings.ErrorParameterRequired("Mailbox")), ErrorCategory.InvalidArgument, null);
				}
				mailboxIdParameter = (this.Mailbox ?? MailboxTaskHelper.ResolveMailboxIdentity(executingUserId, new Task.ErrorLoggerDelegate(base.WriteError)));
			}
			this.adUser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			if (this.Identity != null && this.Identity.InternalOWAExtensionId == null)
			{
				this.Identity.InternalOWAExtensionId = OWAExtensionHelper.CreateOWAExtensionId(this, this.adUser.Id, null, this.Identity.RawExtensionName);
			}
			ADScopeException ex;
			if (!base.TenantGlobalCatalogSession.TryVerifyIsWithinScopes(this.adUser, true, out ex))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotChangeMailboxOutOfWriteScope(this.adUser.Identity.ToString(), (ex == null) ? string.Empty : ex.Message), ex), ErrorCategory.InvalidOperation, this.adUser.Identity);
			}
			OWAExtensionDataProvider owaextensionDataProvider = GetApp.CreateOwaExtensionDataProvider(null, base.TenantGlobalCatalogSession, base.SessionSettings, true, this.adUser, "EnableDisable-App", false, new Task.ErrorLoggerDelegate(base.WriteError));
			this.mailboxOwner = owaextensionDataProvider.MailboxSession.MailboxOwner.ObjectId.ToString();
			return owaextensionDataProvider;
		}

		protected override IConfigurable PrepareDataObject()
		{
			App app = (App)base.PrepareDataObject();
			app.Enabled = this.enabled;
			return app;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.enabled)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, this.adUser.OrganizationId, base.ExecutingUserOrganizationId, false);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.FullyConsistent, sessionSettings, 184, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Extension\\EnableDisableOWAExtensionBase.cs");
				if (!tenantOrTopologyConfigurationSession.GetOrgContainer().AppsForOfficeEnabled)
				{
					this.WriteWarning(Strings.WarningExtensionFeatureDisabled);
				}
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

		private ADUser adUser;

		protected string mailboxOwner;

		private readonly bool enabled;
	}
}
