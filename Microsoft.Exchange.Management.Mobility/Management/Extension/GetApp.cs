using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Extension
{
	[Cmdlet("Get", "App", DefaultParameterSetName = "Identity")]
	public sealed class GetApp : GetTenantADObjectWithIdentityTaskBase<AppIdParameter, App>
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

		internal static OWAExtensionDataProvider CreateOwaExtensionDataProvider(OrganizationIdParameter organizationIdParameter, IRecipientSession tenantGlobalCatalogSession, ADSessionSettings sessionSettings, bool isUserScope, ADUser adUser, string taskName, bool isDebugOn, Task.ErrorLoggerDelegate writeErrorDelegate)
		{
			OWAExtensionDataProvider result = null;
			LocalizedException ex = null;
			try
			{
				result = new OWAExtensionDataProvider((organizationIdParameter == null) ? null : organizationIdParameter.RawIdentity, tenantGlobalCatalogSession, sessionSettings, isUserScope, adUser, taskName, isDebugOn);
			}
			catch (StorageTransientException ex2)
			{
				ex = ex2;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				writeErrorDelegate(ex, ExchangeErrorCategory.Client, adUser.Identity);
			}
			return result;
		}

		protected override IConfigDataProvider CreateSession()
		{
			MailboxIdParameter mailboxIdParameter = null;
			ADUser aduser = null;
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
				ADObjectId adobjectId;
				base.TryGetExecutingUserId(out adobjectId);
				if (adobjectId == null && this.Mailbox == null)
				{
					return this.CreateDataProviderForNonMailboxUser();
				}
				mailboxIdParameter = (this.Mailbox ?? MailboxTaskHelper.ResolveMailboxIdentity(adobjectId, new Task.ErrorLoggerDelegate(base.WriteError)));
			}
			try
			{
				aduser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			}
			catch (ManagementObjectNotFoundException)
			{
				return this.CreateDataProviderForNonMailboxUser();
			}
			if (this.Identity != null && this.Identity.InternalOWAExtensionId == null)
			{
				this.Identity.InternalOWAExtensionId = OWAExtensionHelper.CreateOWAExtensionId(this, aduser.Id, null, this.Identity.RawExtensionName);
			}
			ADScopeException ex;
			if (!base.TenantGlobalCatalogSession.TryVerifyIsWithinScopes(aduser, true, out ex))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotChangeMailboxOutOfWriteScope(aduser.Identity.ToString(), (ex == null) ? string.Empty : ex.Message), ex), ErrorCategory.InvalidOperation, aduser.Identity);
			}
			if (this.Organization != null)
			{
				this.SetCurrentOrganizationId();
			}
			return GetApp.CreateOwaExtensionDataProvider(this.Organization, base.TenantGlobalCatalogSession, base.SessionSettings, !this.OrganizationApp, aduser, "Get-App", base.IsDebugOn, new Task.ErrorLoggerDelegate(base.WriteError));
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			base.CheckExclusiveParameters(new object[]
			{
				"Mailbox",
				"OrganizationApp"
			});
			if (this.Organization != null && !this.OrganizationApp)
			{
				base.WriteError(new LocalizedException(Strings.ErrorParameterRequired("OrganizationApp")), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			OWAExtensionHelper.ProcessRecord(new Action(base.InternalProcessRecord), new Task.TaskErrorLoggingDelegate(base.WriteError), this.Identity);
			if (base.IsDebugOn)
			{
				base.WriteDebug(((OWAExtensionDataProvider)base.DataSession).RawMasterTableXml);
				base.WriteDebug(((OWAExtensionDataProvider)base.DataSession).RawOrgMasterTableXml);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			OWAExtensionHelper.CleanupOWAExtensionDataProvider(base.DataSession);
			GC.SuppressFinalize(this);
		}

		private IConfigDataProvider CreateDataProviderForNonMailboxUser()
		{
			if (base.IsDebugOn)
			{
				base.WriteDebug("Creating data provider for non mailbox user.");
			}
			IConfigDataProvider result = new OWAAppDataProviderForNonMailboxUser((this.Organization == null) ? null : this.Organization.RawIdentity, base.TenantGlobalCatalogSession, base.SessionSettings, !this.OrganizationApp, "Get-App");
			if (this.Identity != null && this.Identity.InternalOWAExtensionId == null)
			{
				this.Identity.InternalOWAExtensionId = OWAExtensionHelper.CreateOWAExtensionId(this, new ADObjectId(), null, this.Identity.RawExtensionName);
			}
			return result;
		}

		private void SetCurrentOrganizationId()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 313, "SetCurrentOrganizationId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Extension\\getapp.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
			base.CurrentOrganizationId = adorganizationalUnit.OrganizationId;
		}
	}
}
