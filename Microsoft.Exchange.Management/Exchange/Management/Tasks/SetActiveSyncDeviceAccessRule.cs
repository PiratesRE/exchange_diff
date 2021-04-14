using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "ActiveSyncDeviceAccessRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetActiveSyncDeviceAccessRule : SetSystemConfigurationObjectTask<ActiveSyncDeviceAccessRuleIdParameter, ActiveSyncDeviceAccessRule>
	{
		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (Datacenter.IsMultiTenancyEnabled() && this.DataObject.OrganizationId == OrganizationId.ForestWideOrgId && this.DataObject.AccessLevel != DeviceAccessLevel.Block)
			{
				base.WriteError(new ArgumentException(Strings.ErrorOnlyForestWideBlockIsAllowed), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetActiveSyncDeviceAccessRule(this.Identity.ToString());
			}
		}
	}
}
