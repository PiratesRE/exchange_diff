using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "MicrosoftExchangeRecipient", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMicrosoftExchangeRecipient : DataAccessTask<ADMicrosoftExchangeRecipient>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMicrosoftExchangeRecipient;
			}
		}

		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 82, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\MicrosoftExchangeRecipient\\RemoveMicrosoftExchangeRecipient.cs");
			tenantOrRootOrgRecipientSession.LinkResolutionServer = ADSession.GetCurrentConfigDC(adsessionSettings.GetAccountOrResourceForestFqdn());
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
			return tenantOrRootOrgRecipientSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.recipient = MailboxTaskHelper.FindMicrosoftExchangeRecipient((IRecipientSession)base.DataSession, DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.CurrentOrganizationId), 114, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\MicrosoftExchangeRecipient\\RemoveMicrosoftExchangeRecipient.cs"));
			if (this.recipient == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorMicrosoftExchangeRecipientNotFound), ErrorCategory.InvalidOperation, null);
			}
			IVersionable versionable = this.recipient;
			if (versionable != null && versionable.MaximumSupportedExchangeObjectVersion.IsOlderThan(versionable.ExchangeVersion))
			{
				base.WriteError(new TaskException(Strings.ErrorRemoveNewerObject(this.recipient.Identity.ToString(), versionable.ExchangeVersion.ExchangeBuild.ToString())), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.WriteVerbose(TaskVerboseStringHelper.GetDeleteObjectVerboseString(this.recipient.Identity, base.DataSession, typeof(ADMicrosoftExchangeRecipient)));
			base.DataSession.Delete(this.recipient);
			TaskLogger.LogExit();
		}

		private ADRecipient recipient;
	}
}
