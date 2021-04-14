using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Reporting;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.TransportProcessingQuota
{
	[Cmdlet("Set", "TransportProcessingQuotaOverride", SupportsShouldProcess = true)]
	public sealed class SetTransportProcessingQuotaOverride : TransportProcessingQuotaBaseTask
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public Guid ExternalDirectoryOrganizationId { get; set; }

		[Parameter(Mandatory = true)]
		public bool Throttle { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetTransportProcessingQuotaOverride(this.ExternalDirectoryOrganizationId, this.Throttle);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			TenantThrottleState tenantThrottleState = this.Throttle ? TenantThrottleState.Throttled : TenantThrottleState.Unthrottled;
			TenantThrottleInfo throttleState = base.Session.GetThrottleState(this.ExternalDirectoryOrganizationId);
			if (throttleState == null || throttleState.ThrottleState != tenantThrottleState)
			{
				base.Session.SetThrottleState(new TenantThrottleInfo
				{
					TenantId = this.ExternalDirectoryOrganizationId,
					ThrottleState = tenantThrottleState
				});
			}
		}
	}
}
