using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Write", "AdminAuditLog", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class WriteAdminAuditLog : GetMultitenancySingletonSystemConfigurationObjectTask<AdminAuditLogConfig>
	{
		[Parameter(Mandatory = true)]
		public string Comment
		{
			get
			{
				return (string)base.Fields["Comment"];
			}
			set
			{
				base.Fields["Comment"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddAdminAuditLog(base.CurrentOrgContainerId.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return AdminAuditLogConfig.GetWellKnownParentLocation(base.CurrentOrgContainerId);
			}
		}

		protected override void InternalProvisioningValidation()
		{
			ProvisioningValidationError[] array = ProvisioningLayer.Validate(this, null);
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					ProvisioningValidationException exception = new ProvisioningValidationException(array[i].Description, array[i].AgentName, array[i].Exception);
					this.WriteError(exception, (ErrorCategory)array[i].ErrorCategory, null, array.Length - 1 == i);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
		}

		protected override IConfigDataProvider CreateSession()
		{
			return null;
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
		}
	}
}
