using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ResourceConfig", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetResourceConfig : SetMultitenancySingletonSystemConfigurationObjectTask<ResourceBookingConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetResourceConfig;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return ResourceBookingConfig.GetWellKnownParentLocation(base.CurrentOrgContainerId);
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.ResourcePropertySchema != null)
			{
				foreach (string text in this.DataObject.ResourcePropertySchema)
				{
					if (!text.StartsWith("Room/", StringComparison.OrdinalIgnoreCase) && !text.StartsWith("Equipment/", StringComparison.OrdinalIgnoreCase))
					{
						string[] array = text.Split(new char[]
						{
							'/'
						});
						base.WriteError(new ErrorResourceRoomOrEquipmentOnlyException("Room", "Equipment", text, (array.Length > 0) ? array[0] : text), ErrorCategory.InvalidData, this.DataObject);
					}
				}
			}
			TaskLogger.LogExit();
		}
	}
}
