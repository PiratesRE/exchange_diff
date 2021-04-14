using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "AdminAuditLogConfig", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveAdminAuditLogConfig : RemoveSystemConfigurationObjectTask<AdminAuditLogIdParameter, AdminAuditLogConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveAdminAuditLogConfig(base.CurrentOrgContainerId.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, Position = 0)]
		public override AdminAuditLogIdParameter Identity
		{
			get
			{
				return ((AdminAuditLogIdParameter)base.Fields["Identity"]) ?? AdminAuditLogIdParameter.Parse("Admin Audit Log Settings");
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}
	}
}
