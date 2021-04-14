using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync.CookieManager;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Start", "MsoFullSync", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class StartMsoFullSync : SetSystemConfigurationObjectTask<OrganizationIdParameter, ExchangeConfigurationUnit>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override OrganizationIdParameter Identity
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStartMsoFullSync((this.Identity == null) ? base.CurrentOrganizationId.ToString() : this.Identity.ToString(), "full");
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(InvalidMainStreamCookieException).IsInstanceOfType(exception);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.Identity == null && (base.CurrentOrganizationId == null || base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId)))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCouldNotStartFullSyncForFirstOrg), (ErrorCategory)1000, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			if (this.Identity == null)
			{
				OrganizationIdParameter identity = new OrganizationIdParameter(base.CurrentOrgContainerId);
				this.Identity = identity;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (string.IsNullOrEmpty(this.DataObject.ExternalDirectoryOrganizationId))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNotMsoOrganization(this.DataObject.OrganizationId.ToString())), (ErrorCategory)1000, null);
			}
			if (this.DataObject.OrganizationStatus != OrganizationStatus.Active)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNotActiveOrganization(this.DataObject.OrganizationId.ToString())), (ErrorCategory)1000, null);
			}
			this.recipientFullSyncCookieManager = new MsoRecipientFullSyncCookieManager(Guid.Parse(this.DataObject.ExternalDirectoryOrganizationId));
			this.companyFullSyncCookieManager = new MsoCompanyFullSyncCookieManager(Guid.Parse(this.DataObject.ExternalDirectoryOrganizationId));
			if (this.recipientFullSyncCookieManager.ReadCookie() != null || this.companyFullSyncCookieManager.ReadCookie() != null)
			{
				base.WriteError(new ADObjectAlreadyExistsException(Strings.ErrorFullSyncInProgress(this.DataObject.ExternalDirectoryOrganizationId)), (ErrorCategory)1000, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string requestor = this.GetRequestor() + " via Start-MsoFullSync";
			this.recipientFullSyncCookieManager.WriteInitialSyncCookie(TenantSyncType.Full, requestor);
			this.companyFullSyncCookieManager.WriteInitialSyncCookie(TenantSyncType.Full, requestor);
			TaskLogger.LogExit();
		}

		private string GetRequestor()
		{
			ADObjectId adobjectId;
			if (base.TryGetExecutingUserId(out adobjectId))
			{
				return adobjectId.ToString();
			}
			if (base.ExchangeRunspaceConfig != null)
			{
				if (!string.IsNullOrEmpty(base.ExchangeRunspaceConfig.ExecutingUserDisplayName))
				{
					return base.ExchangeRunspaceConfig.ExecutingUserDisplayName;
				}
				if (!string.IsNullOrEmpty(base.ExchangeRunspaceConfig.IdentityName))
				{
					return base.ExchangeRunspaceConfig.IdentityName;
				}
			}
			return Environment.MachineName;
		}

		private MsoRecipientFullSyncCookieManager recipientFullSyncCookieManager;

		private MsoCompanyFullSyncCookieManager companyFullSyncCookieManager;
	}
}
