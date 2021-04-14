using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "ActiveSyncOrganizationSettings", SupportsShouldProcess = true, DefaultParameterSetName = "Default")]
	public sealed class SetActiveSyncOrganizationSettings : SetSystemConfigurationObjectTask<ActiveSyncOrganizationSettingsIdParameter, ActiveSyncOrganizationSettings>
	{
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRuleForAllDevices", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetRemoveDeviceFilter", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetRemoveDeviceFilterRule", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = false, ParameterSetName = "Default", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRule", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override ActiveSyncOrganizationSettingsIdParameter Identity
		{
			get
			{
				return (ActiveSyncOrganizationSettingsIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRule")]
		public SwitchParameter AddDeviceFilterRule
		{
			get
			{
				return (SwitchParameter)(base.Fields["ParameterSetAddDeviceFilterRule"] ?? false);
			}
			set
			{
				base.Fields["ParameterSetAddDeviceFilterRule"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetRemoveDeviceFilterRule")]
		public SwitchParameter RemoveDeviceFilterRule
		{
			get
			{
				return (SwitchParameter)(base.Fields["ParameterSetRemoveDeviceFilterRule"] ?? false);
			}
			set
			{
				base.Fields["ParameterSetRemoveDeviceFilterRule"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetRemoveDeviceFilter")]
		public SwitchParameter RemoveDeviceFilter
		{
			get
			{
				return (SwitchParameter)(base.Fields["ParameterSetRemoveDeviceFilter"] ?? false);
			}
			set
			{
				base.Fields["ParameterSetRemoveDeviceFilter"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRuleForAllDevices")]
		public SwitchParameter AddDeviceFilterRuleForAllDevices
		{
			get
			{
				return (SwitchParameter)(base.Fields["ParameterSetAddDeviceFilterRuleForAllDevices"] ?? false);
			}
			set
			{
				base.Fields["ParameterSetAddDeviceFilterRuleForAllDevices"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRuleForAllDevices")]
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetRemoveDeviceFilterRule")]
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRule")]
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetRemoveDeviceFilter")]
		public string DeviceFilterName
		{
			get
			{
				return (string)base.Fields["DeviceFilterName"];
			}
			set
			{
				base.Fields["DeviceFilterName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRule")]
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetRemoveDeviceFilterRule")]
		public string DeviceFilterRuleName
		{
			get
			{
				return (string)base.Fields["DeviceFilterRuleName"];
			}
			set
			{
				base.Fields["DeviceFilterRuleName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetRemoveDeviceFilterRule")]
		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRule")]
		public DeviceAccessCharacteristic DeviceFilterCharacteristic
		{
			get
			{
				return (DeviceAccessCharacteristic)base.Fields["DeviceFilterCharacteristic"];
			}
			set
			{
				base.Fields["DeviceFilterCharacteristic"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRule")]
		public DeviceFilterOperator DeviceFilterOperator
		{
			get
			{
				return (DeviceFilterOperator)base.Fields["DeviceFilterOperator"];
			}
			set
			{
				base.Fields["DeviceFilterOperator"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ParameterSetAddDeviceFilterRule")]
		public string DeviceFilterValue
		{
			get
			{
				return (string)base.Fields["DeviceFilterValue"];
			}
			set
			{
				base.Fields["DeviceFilterValue"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (Datacenter.IsMultiTenancyEnabled() && this.DataObject.OrganizationId == OrganizationId.ForestWideOrgId && this.DataObject.DefaultAccessLevel != DeviceAccessLevel.Allow)
			{
				base.WriteError(new ArgumentException(Strings.ErrorOnlyForestWideAllowIsAllowed), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetActiveSyncOrganizationSettings;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			if (this.Identity == null)
			{
				IConfigurable[] array = null;
				try
				{
					array = base.DataSession.Find<ActiveSyncOrganizationSettings>(null, this.RootId, false, null);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, (ErrorCategory)1002, null);
				}
				if (array == null)
				{
					array = new IConfigurable[0];
				}
				IConfigurable result = null;
				switch (array.Length)
				{
				case 0:
					base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(null, typeof(ActiveSyncOrganizationSettings).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, null);
					break;
				case 1:
					result = array[0];
					break;
				default:
					base.WriteError(new ManagementObjectAmbiguousException(Strings.ActiveSyncOrganizationSettingsAmbiguous), (ErrorCategory)1003, null);
					break;
				}
				return result;
			}
			return base.ResolveDataObject();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ActiveSyncOrganizationSettings activeSyncOrganizationSettings = (ActiveSyncOrganizationSettings)base.PrepareDataObject();
			if (base.Fields.IsModified("ParameterSetAddDeviceFilterRule") && this.AddDeviceFilterRule)
			{
				this.ValidateDeviceFilterNameParameter();
				this.ValidateDeviceFilterRuleNameParameter();
				ActiveSyncDeviceFilterRule newRule = new ActiveSyncDeviceFilterRule(this.DeviceFilterRuleName, this.DeviceFilterCharacteristic, this.DeviceFilterOperator, this.DeviceFilterValue);
				activeSyncOrganizationSettings.DeviceFiltering = this.AddDeviceFilterRuleToExisting(activeSyncOrganizationSettings.DeviceFiltering, this.DeviceFilterName, newRule, false);
			}
			else if (base.Fields.IsModified("ParameterSetAddDeviceFilterRuleForAllDevices") && this.AddDeviceFilterRuleForAllDevices)
			{
				this.ValidateDeviceFilterNameParameter();
				activeSyncOrganizationSettings.DeviceFiltering = this.AddDeviceFilterRuleToExisting(activeSyncOrganizationSettings.DeviceFiltering, this.DeviceFilterName, null, true);
			}
			else if (base.Fields.IsModified("ParameterSetRemoveDeviceFilterRule") && this.RemoveDeviceFilterRule)
			{
				this.ValidateDeviceFilterNameParameter();
				this.ValidateDeviceFilterRuleNameParameter();
				activeSyncOrganizationSettings.DeviceFiltering = this.RemoveDeviceFilterRuleFromExisting(activeSyncOrganizationSettings.DeviceFiltering, this.DeviceFilterName, this.DeviceFilterRuleName, this.DeviceFilterCharacteristic);
			}
			else if (base.Fields.IsModified("ParameterSetRemoveDeviceFilter") && this.RemoveDeviceFilter)
			{
				this.ValidateDeviceFilterNameParameter();
				activeSyncOrganizationSettings.DeviceFiltering = this.RemoveDeviceFilterFromExisting(activeSyncOrganizationSettings.DeviceFiltering, this.DeviceFilterName);
			}
			TaskLogger.LogExit();
			return activeSyncOrganizationSettings;
		}

		private void ValidateDeviceFilterNameParameter()
		{
			if (string.IsNullOrEmpty(this.DeviceFilterName))
			{
				base.WriteError(new ArgumentException(Strings.NullOrEmptyStringNotAllowed("DeviceFilterName")), ErrorCategory.InvalidArgument, null);
			}
		}

		private void ValidateDeviceFilterRuleNameParameter()
		{
			if (string.IsNullOrEmpty(this.DeviceFilterRuleName))
			{
				base.WriteError(new ArgumentException(Strings.NullOrEmptyStringNotAllowed("DeviceFilterRuleName")), ErrorCategory.InvalidArgument, null);
			}
		}

		private ActiveSyncDeviceFilterArray AddDeviceFilterRuleToExisting(ActiveSyncDeviceFilterArray existingDeviceFilterArray, string deviceFilterName, ActiveSyncDeviceFilterRule newRule, bool applyFilterForAllDevices)
		{
			ActiveSyncDeviceFilterArray activeSyncDeviceFilterArray = new ActiveSyncDeviceFilterArray();
			if (existingDeviceFilterArray != null)
			{
				activeSyncDeviceFilterArray.DeviceFilters = existingDeviceFilterArray.DeviceFilters;
			}
			List<ActiveSyncDeviceFilter> list = activeSyncDeviceFilterArray.DeviceFilters;
			if (list == null)
			{
				list = new List<ActiveSyncDeviceFilter>();
				activeSyncDeviceFilterArray.DeviceFilters = list;
			}
			ActiveSyncDeviceFilter activeSyncDeviceFilter = list.FirstOrDefault((ActiveSyncDeviceFilter existingDeviceFilter) => existingDeviceFilter.Name.Equals(deviceFilterName, StringComparison.InvariantCultureIgnoreCase));
			if (activeSyncDeviceFilter == null)
			{
				activeSyncDeviceFilter = new ActiveSyncDeviceFilter(deviceFilterName, new List<ActiveSyncDeviceFilterRule>());
				list.Add(activeSyncDeviceFilter);
			}
			if (applyFilterForAllDevices)
			{
				activeSyncDeviceFilter.ApplyForAllDevices = applyFilterForAllDevices;
				base.WriteVerbose(Strings.AddedDeviceFilterRule(deviceFilterName));
				return activeSyncDeviceFilterArray;
			}
			if (activeSyncDeviceFilter.ApplyForAllDevices)
			{
				base.WriteError(new InvalidOperationException(Strings.CantAddDeviceFilterRuleSinceApplyForAllDevicesSetToTrue(deviceFilterName)), ErrorCategory.InvalidOperation, null);
			}
			List<ActiveSyncDeviceFilterRule> rules = activeSyncDeviceFilter.Rules;
			if (!rules.Any((ActiveSyncDeviceFilterRule rule) => rule.Characteristic == newRule.Characteristic && rule.Name.Equals(newRule.Name, StringComparison.InvariantCultureIgnoreCase)))
			{
				rules.Add(newRule);
				base.WriteVerbose(Strings.AddedDeviceFilterRule(deviceFilterName));
			}
			else
			{
				base.WriteError(new ArgumentException(Strings.DeviceFilterRuleAlreadyExists(deviceFilterName)), ErrorCategory.InvalidArgument, null);
			}
			return activeSyncDeviceFilterArray;
		}

		private ActiveSyncDeviceFilterArray RemoveDeviceFilterRuleFromExisting(ActiveSyncDeviceFilterArray existingDeviceFilterArray, string deviceFilterName, string deviceFilterRuleName, DeviceAccessCharacteristic deviceFilterCharacteristic)
		{
			ActiveSyncDeviceFilterArray activeSyncDeviceFilterArray = new ActiveSyncDeviceFilterArray();
			if (existingDeviceFilterArray == null || existingDeviceFilterArray.DeviceFilters == null)
			{
				base.WriteVerbose(new LocalizedString("There are no device filters to remove"));
				return existingDeviceFilterArray;
			}
			activeSyncDeviceFilterArray.DeviceFilters = existingDeviceFilterArray.DeviceFilters;
			ActiveSyncDeviceFilter activeSyncDeviceFilter = activeSyncDeviceFilterArray.DeviceFilters.FirstOrDefault((ActiveSyncDeviceFilter existingDeviceFilter) => existingDeviceFilter.Name.Equals(deviceFilterName, StringComparison.InvariantCultureIgnoreCase));
			if (activeSyncDeviceFilter == null || activeSyncDeviceFilter.Rules == null)
			{
				base.WriteError(new ArgumentException(Strings.NoDeviceFilterRuleByName(deviceFilterName)), ErrorCategory.InvalidArgument, null);
			}
			List<ActiveSyncDeviceFilterRule> rules = activeSyncDeviceFilter.Rules;
			int num = rules.FindIndex((ActiveSyncDeviceFilterRule rule) => rule.Characteristic == deviceFilterCharacteristic && rule.Name.Equals(deviceFilterRuleName, StringComparison.InvariantCultureIgnoreCase));
			if (num >= 0)
			{
				rules.RemoveAt(num);
				base.WriteVerbose(Strings.RemovedDeviceFilterRuleByNameAndCharacteristic(deviceFilterName, deviceFilterRuleName, deviceFilterCharacteristic.ToString()));
			}
			else
			{
				base.WriteVerbose(Strings.NoDeviceFilterRuleByNameAndCharacteristic(deviceFilterName, deviceFilterRuleName, deviceFilterCharacteristic.ToString()));
			}
			if (activeSyncDeviceFilter.Rules.Count == 0)
			{
				base.WriteVerbose(Strings.EmptyDeviceFilterRemoved(deviceFilterName));
				activeSyncDeviceFilterArray.DeviceFilters.Remove(activeSyncDeviceFilter);
			}
			return activeSyncDeviceFilterArray;
		}

		private ActiveSyncDeviceFilterArray RemoveDeviceFilterFromExisting(ActiveSyncDeviceFilterArray existingDeviceFilterArray, string deviceFilterName)
		{
			ActiveSyncDeviceFilterArray activeSyncDeviceFilterArray = new ActiveSyncDeviceFilterArray();
			if (existingDeviceFilterArray == null || existingDeviceFilterArray.DeviceFilters == null)
			{
				base.WriteVerbose(Strings.NoDeviceFilters);
				return existingDeviceFilterArray;
			}
			activeSyncDeviceFilterArray.DeviceFilters = existingDeviceFilterArray.DeviceFilters;
			int num = activeSyncDeviceFilterArray.DeviceFilters.RemoveAll((ActiveSyncDeviceFilter existingDeviceFilter) => existingDeviceFilter.Name.Equals(deviceFilterName, StringComparison.InvariantCultureIgnoreCase));
			if (num > 0)
			{
				base.WriteVerbose(Strings.RemovedDeviceFilter(deviceFilterName));
			}
			else
			{
				base.WriteVerbose(Strings.NoDeviceFilterByName(deviceFilterName));
			}
			return activeSyncDeviceFilterArray;
		}

		private const string ParameterSetAddDeviceFilterRule = "ParameterSetAddDeviceFilterRule";

		private const string ParameterSetAddDeviceFilterRuleForAllDevices = "ParameterSetAddDeviceFilterRuleForAllDevices";

		private const string ParameterSetRemoveDeviceFilterRule = "ParameterSetRemoveDeviceFilterRule";

		private const string ParameterSetRemoveDeviceFilter = "ParameterSetRemoveDeviceFilter";

		private const string DeviceFilterNameParameter = "DeviceFilterName";

		private const string DeviceFilterRuleNameParameter = "DeviceFilterRuleName";

		private const string DeviceFilterCharacteristicParameter = "DeviceFilterCharacteristic";

		private const string DeviceFilterOperatorParameter = "DeviceFilterOperator";

		private const string DeviceFilterValueParameter = "DeviceFilterValue";
	}
}
