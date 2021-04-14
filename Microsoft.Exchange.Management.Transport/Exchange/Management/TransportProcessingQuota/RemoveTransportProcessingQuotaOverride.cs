using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Reporting;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.TransportProcessingQuota
{
	[Cmdlet("Remove", "TransportProcessingQuotaOverride", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveTransportProcessingQuotaOverride : TransportProcessingQuotaBaseTask
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public Guid ExternalDirectoryOrganizationId { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveTransportProcessingQuotaOverride(this.ExternalDirectoryOrganizationId);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			TenantThrottleInfo throttleState = base.Session.GetThrottleState(this.ExternalDirectoryOrganizationId);
			if (throttleState != null && throttleState.ThrottleState != TenantThrottleState.Auto)
			{
				base.Session.SetThrottleState(new TenantThrottleInfo
				{
					TenantId = this.ExternalDirectoryOrganizationId,
					ThrottleState = TenantThrottleState.Auto
				});
				return;
			}
			this.WriteWarning(Strings.ErrorObjectNotFound("TransportProcessingQuotaOverride"));
		}
	}
}
