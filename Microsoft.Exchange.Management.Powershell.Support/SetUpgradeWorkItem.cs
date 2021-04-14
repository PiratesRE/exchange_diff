using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Set", "UpgradeWorkItem", SupportsShouldProcess = true)]
	public class SetUpgradeWorkItem : SymphonyTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetUpgradeWorkItemConfirmationMessage(this.Identity, this.modifiedProperties);
			}
		}

		[Parameter(Mandatory = false)]
		public WorkItemStatus Status { get; set; }

		[Parameter(Mandatory = false)]
		public string Comment { get; set; }

		[Parameter(Mandatory = false)]
		public int CompletedCount { get; set; }

		[Parameter(Mandatory = false)]
		public string HandlerState { get; set; }

		[Parameter(Mandatory = false)]
		public string StatusDetailsUri { get; set; }

		[Parameter(Mandatory = false)]
		public int TotalCount { get; set; }

		[Parameter(Mandatory = false)]
		public Guid Tenant { get; set; }

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public string Identity { get; set; }

		protected override void InternalValidate()
		{
			if (base.UserSpecifiedParameters.Contains("Tenant"))
			{
				if (this.Tenant == Guid.Empty)
				{
					throw new InvalidTenantGuidException(this.Tenant.ToString());
				}
				this.retrievedWorkItem = base.GetWorkitemByIdAndTenantId(this.Identity, this.Tenant);
			}
			else
			{
				this.retrievedWorkItem = base.GetWorkItemById(this.Identity);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (base.UserSpecifiedParameters.Contains("Status"))
			{
				this.retrievedWorkItem.WorkItemStatus.Status = this.Status;
				stringBuilder.AppendFormat("Status: {0} ", this.Status.ToString());
			}
			if (base.UserSpecifiedParameters.Contains("HandlerState"))
			{
				this.retrievedWorkItem.WorkItemStatus.HandlerState = this.HandlerState;
				stringBuilder.AppendFormat("HandlerState: {0} ", this.HandlerState);
			}
			if (base.UserSpecifiedParameters.Contains("Comment"))
			{
				this.retrievedWorkItem.WorkItemStatus.Comment = this.Comment;
				stringBuilder.AppendFormat("Comment: {0} ", this.Comment);
			}
			if (base.UserSpecifiedParameters.Contains("CompletedCount"))
			{
				this.retrievedWorkItem.WorkItemStatus.CompletedCount = this.CompletedCount;
				stringBuilder.AppendFormat("CompletedCount: {0} ", this.CompletedCount);
			}
			if (base.UserSpecifiedParameters.Contains("TotalCount"))
			{
				this.retrievedWorkItem.WorkItemStatus.TotalCount = this.TotalCount;
				stringBuilder.AppendFormat("TotalCount: {0} ", this.TotalCount);
			}
			if (base.UserSpecifiedParameters.Contains("StatusDetailsUri"))
			{
				Uri statusDetails;
				if (!Uri.TryCreate(this.StatusDetailsUri, UriKind.Absolute, out statusDetails))
				{
					throw new InvalidStatusDetailException(this.StatusDetailsUri);
				}
				this.retrievedWorkItem.WorkItemStatus.StatusDetails = statusDetails;
				stringBuilder.AppendFormat("StatusDetails: {0} ", this.StatusDetailsUri);
			}
			this.modifiedProperties = stringBuilder.ToString();
		}

		protected override void InternalProcessRecord()
		{
			using (ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler> workloadClient = new ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler>(base.WorkloadUri, base.Certificate))
			{
				workloadClient.CallSymphony(delegate
				{
					workloadClient.Proxy.UpdateWorkItem(this.retrievedWorkItem.WorkItemId, this.retrievedWorkItem.WorkItemStatus);
				}, base.WorkloadUri.ToString());
			}
		}

		private WorkItemInfo retrievedWorkItem;

		private string modifiedProperties = string.Empty;
	}
}
