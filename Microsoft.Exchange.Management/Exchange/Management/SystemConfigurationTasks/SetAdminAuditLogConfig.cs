using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AdminAuditLogConfig", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetAdminAuditLogConfig : SetMultitenancySingletonSystemConfigurationObjectTask<AdminAuditLogConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAdminAuditLogConfig(base.CurrentOrgContainerId.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return AdminAuditLogConfig.GetWellKnownParentLocation(base.CurrentOrgContainerId);
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Static;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.CreateSession();
			if (this.IgnoreDehydratedFlag)
			{
				configurationSession.SessionSettings.IsSharedConfigChecked = true;
			}
			configurationSession.SessionSettings.IncludeCNFObject = false;
			return configurationSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			((IConfigurationSession)base.DataSession).SessionSettings.IsSharedConfigChecked = true;
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.DataObject.IsModified(ADObjectSchema.Name))
			{
				base.WriteError(new ArgumentException(Strings.ErrorCannotChangeName), ErrorCategory.InvalidArgument, null);
			}
			if (this.DataObject.IsModified(AdminAuditLogConfigSchema.AdminAuditLogExcludedCmdlets) && this.DataObject.AdminAuditLogExcludedCmdlets != null)
			{
				foreach (string a in this.DataObject.AdminAuditLogExcludedCmdlets)
				{
					if (string.Equals(a, "*", StringComparison.OrdinalIgnoreCase))
					{
						base.WriteError(new ArgumentException(Strings.ErrorInvalidExcludedCmdletPattern), ErrorCategory.InvalidArgument, null);
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (this.DataObject.IsChanged(AdminAuditLogConfigSchema.AdminAuditLogAgeLimit) && !this.Force)
			{
				EnhancedTimeSpan t;
				if (this.DataObject.AdminAuditLogAgeLimit == EnhancedTimeSpan.Zero)
				{
					if (!base.ShouldContinue(Strings.ConfirmationMessageAdminAuditLogAgeLimitZero(base.CurrentOrgContainerId.ToString())))
					{
						return;
					}
				}
				else if (this.DataObject.TryGetOriginalValue<EnhancedTimeSpan>(AdminAuditLogConfigSchema.AdminAuditLogAgeLimit, out t))
				{
					EnhancedTimeSpan adminAuditLogAgeLimit = this.DataObject.AdminAuditLogAgeLimit;
					if (t > adminAuditLogAgeLimit && !base.ShouldContinue(Strings.ConfirmationMessageAdminAuditLogAgeLimitSmaller(base.CurrentOrgContainerId.ToString(), adminAuditLogAgeLimit.ToString())))
					{
						return;
					}
				}
			}
			if (this.IsObjectStateChanged())
			{
				this.WriteWarning(Strings.WarningSetAdminAuditLogConfigDelay(SetAdminAuditLogConfig.AuditConfigSettingsDelayTime.TotalMinutes));
			}
			if (AdminAuditLogHelper.ShouldIssueWarning(base.CurrentOrganizationId))
			{
				this.WriteWarning(Strings.WarningSetAdminAuditLogOnPreE15(base.CurrentOrganizationId.ToString()));
			}
			base.InternalProcessRecord();
			ProvisioningLayer.RefreshProvisioningBroker(this);
		}

		private static TimeSpan AuditConfigSettingsDelayTime = new TimeSpan(1, 0, 0);
	}
}
