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
	[Cmdlet("New", "MsoTenantSyncRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class NewMsoTenantSyncRequest : SetSystemConfigurationObjectTask<OrganizationIdParameter, MsoTenantSyncRequest, ExchangeConfigurationUnit>
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter Full { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is InvalidMainStreamCookieException;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string syncType = this.Full ? "Full" : "Partial";
				return Strings.ConfirmationMessageStartMsoFullSync(this.Identity.ToString(), syncType);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (string.IsNullOrEmpty(this.DataObject.ExternalDirectoryOrganizationId))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNotMsoOrganization(this.DataObject.OrganizationId.ToString())), (ErrorCategory)1000, null);
			}
			this.recipientFullSyncCookieManager = new MsoRecipientFullSyncCookieManager(Guid.Parse(this.DataObject.ExternalDirectoryOrganizationId));
			this.companyFullSyncCookieManager = new MsoCompanyFullSyncCookieManager(Guid.Parse(this.DataObject.ExternalDirectoryOrganizationId));
			if (!this.Force && (this.recipientFullSyncCookieManager.ReadCookie() != null || this.companyFullSyncCookieManager.ReadCookie() != null))
			{
				base.WriteError(new ADObjectAlreadyExistsException(Strings.ErrorFullSyncInProgress(this.DataObject.ExternalDirectoryOrganizationId)), (ErrorCategory)1000, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string requestor = this.GetRequestor();
			if (this.Full)
			{
				if (base.ShouldProcess(this.ConfirmationMessage))
				{
					this.recipientFullSyncCookieManager.WriteInitialSyncCookie(TenantSyncType.Full, requestor);
					this.companyFullSyncCookieManager.WriteInitialSyncCookie(TenantSyncType.Full, requestor);
				}
			}
			else
			{
				this.recipientFullSyncCookieManager.WriteInitialSyncCookie(TenantSyncType.Partial, requestor);
				this.companyFullSyncCookieManager.WriteInitialSyncCookie(TenantSyncType.Partial, requestor);
			}
			MsoTenantCookieContainer organization = (MsoTenantCookieContainer)base.DataSession.Read<MsoTenantCookieContainer>(this.DataObject.Identity);
			MsoTenantSyncRequest sendToPipeline = new MsoTenantSyncRequest(organization, this.recipientFullSyncCookieManager.LastCookie, this.companyFullSyncCookieManager.LastCookie);
			base.WriteObject(sendToPipeline);
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
