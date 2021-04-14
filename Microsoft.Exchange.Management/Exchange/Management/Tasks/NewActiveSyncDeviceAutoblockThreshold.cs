using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "ActiveSyncDeviceAutoblockThreshold", SupportsShouldProcess = true)]
	public sealed class NewActiveSyncDeviceAutoblockThreshold : NewMultitenancyFixedNameSystemConfigurationObjectTask<ActiveSyncDeviceAutoblockThreshold>
	{
		[Parameter(Mandatory = true)]
		public AutoblockThresholdType BehaviorType
		{
			get
			{
				return this.DataObject.BehaviorType;
			}
			set
			{
				this.DataObject.BehaviorType = value;
			}
		}

		[Parameter(Mandatory = true)]
		public int BehaviorTypeIncidenceLimit
		{
			get
			{
				return this.DataObject.BehaviorTypeIncidenceLimit;
			}
			set
			{
				this.DataObject.BehaviorTypeIncidenceLimit = value;
			}
		}

		[Parameter(Mandatory = true)]
		public EnhancedTimeSpan BehaviorTypeIncidenceDuration
		{
			get
			{
				return this.DataObject.BehaviorTypeIncidenceDuration;
			}
			set
			{
				this.DataObject.BehaviorTypeIncidenceDuration = value;
			}
		}

		[Parameter(Mandatory = true)]
		public EnhancedTimeSpan DeviceBlockDuration
		{
			get
			{
				return this.DataObject.DeviceBlockDuration;
			}
			set
			{
				this.DataObject.DeviceBlockDuration = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AdminEmailInsert
		{
			get
			{
				return this.DataObject.AdminEmailInsert;
			}
			set
			{
				this.DataObject.AdminEmailInsert = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.DataObject.Name = this.BehaviorType.ToString();
			ActiveSyncDeviceAutoblockThreshold activeSyncDeviceAutoblockThreshold = (ActiveSyncDeviceAutoblockThreshold)base.PrepareDataObject();
			activeSyncDeviceAutoblockThreshold.SetId((IConfigurationSession)base.DataSession, this.DataObject.Name);
			return activeSyncDeviceAutoblockThreshold;
		}
	}
}
