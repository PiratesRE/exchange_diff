using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "ResourceConfig")]
	public sealed class InstallResourceConfig : NewMultitenancyFixedNameSystemConfigurationObjectTask<ResourceBookingConfig>
	{
		protected override ObjectId RootId
		{
			get
			{
				return ResourceBookingConfig.GetWellKnownParentLocation(base.CurrentOrgContainerId);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ResourceBookingConfig resourceBookingConfig = (ResourceBookingConfig)base.PrepareDataObject();
			resourceBookingConfig.SetId(ResourceBookingConfig.GetWellKnownLocation(base.CurrentOrgContainerId));
			return resourceBookingConfig;
		}

		protected override void InternalProcessRecord()
		{
			if (base.DataSession.Read<ResourceBookingConfig>(this.DataObject.Id) == null)
			{
				base.InternalProcessRecord();
			}
		}
	}
}
