using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class DeviceAccessRules : DataSourceService, IDeviceAccessRules, IDataSourceService<DeviceAccessRuleFilter, DeviceAccessRuleRow, DeviceAccessRule, SetDeviceAccessRule, NewDeviceAccessRule>, IDataSourceService<DeviceAccessRuleFilter, DeviceAccessRuleRow, DeviceAccessRule, SetDeviceAccessRule, NewDeviceAccessRule, BaseWebServiceParameters>, IEditListService<DeviceAccessRuleFilter, DeviceAccessRuleRow, DeviceAccessRule, NewDeviceAccessRule, BaseWebServiceParameters>, IGetListService<DeviceAccessRuleFilter, DeviceAccessRuleRow>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<DeviceAccessRule, SetDeviceAccessRule, DeviceAccessRuleRow>, IGetObjectService<DeviceAccessRule>, IGetObjectForListService<DeviceAccessRuleRow>, INewGetObjectService<DeviceAccessRuleRow, NewDeviceAccessRule, NewDeviceAccessRuleData>, INewObjectService<DeviceAccessRuleRow, NewDeviceAccessRule>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ActiveSyncDeviceAccessRule@C:OrganizationConfig")]
		public PowerShellResults<DeviceAccessRuleRow> GetList(DeviceAccessRuleFilter filter, SortOptions sort)
		{
			return base.GetList<DeviceAccessRuleRow, DeviceAccessRuleFilter>("Get-ActiveSyncDeviceAccessRule", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig")]
		public PowerShellResults<DeviceAccessRule> GetObject(Identity identity)
		{
			return base.GetObject<DeviceAccessRule>("Get-ActiveSyncDeviceAccessRule", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig")]
		public PowerShellResults<DeviceAccessRuleRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<DeviceAccessRuleRow>("Get-ActiveSyncDeviceAccessRule", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDevice?Identity@R:Organization")]
		public PowerShellResults<NewDeviceAccessRuleData> GetObjectForNew(Identity identity)
		{
			if (identity == null)
			{
				return null;
			}
			return base.GetObject<NewDeviceAccessRuleData>("Get-MobileDevice", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-ActiveSyncDeviceAccessRule@C:OrganizationConfig")]
		public PowerShellResults<DeviceAccessRuleRow> NewObject(NewDeviceAccessRule properties)
		{
			if (properties.DeviceTypeQueryString == null)
			{
				throw new FaultException(Strings.DeviceTypeRequired);
			}
			if (properties.DeviceModelQueryString == null)
			{
				throw new FaultException(Strings.DeviceModelRequired);
			}
			PSCommand pscommand = new PSCommand().AddCommand("New-ActiveSyncDeviceAccessRule").AddParameters(properties);
			if (!properties.DeviceTypeQueryString.IsWildcard && properties.DeviceModelQueryString.IsWildcard)
			{
				pscommand.AddParameter(ActiveSyncDeviceAccessRuleSchema.Characteristic.Name, DeviceAccessCharacteristic.DeviceType);
				pscommand.AddParameter(ActiveSyncDeviceAccessRuleSchema.QueryString.Name, properties.DeviceTypeQueryString.QueryString);
			}
			else
			{
				if (!properties.DeviceTypeQueryString.IsWildcard || properties.DeviceModelQueryString.IsWildcard)
				{
					throw new FaultException(Strings.InvalidDeviceAccessCharacteristic);
				}
				pscommand.AddParameter(ActiveSyncDeviceAccessRuleSchema.Characteristic.Name, DeviceAccessCharacteristic.DeviceModel);
				pscommand.AddParameter(ActiveSyncDeviceAccessRuleSchema.QueryString.Name, properties.DeviceModelQueryString.QueryString);
			}
			return base.Invoke<DeviceAccessRuleRow>(pscommand);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-ActiveSyncDeviceAccessRule", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig+Set-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig")]
		public PowerShellResults<DeviceAccessRuleRow> SetObject(Identity identity, SetDeviceAccessRule properties)
		{
			return base.SetObject<DeviceAccessRule, SetDeviceAccessRule, DeviceAccessRuleRow>("Set-ActiveSyncDeviceAccessRule", identity, properties);
		}

		private const string Noun = "ActiveSyncDeviceAccessRule";

		internal const string GetCmdlet = "Get-ActiveSyncDeviceAccessRule";

		internal const string NewCmdlet = "New-ActiveSyncDeviceAccessRule";

		internal const string RemoveCmdlet = "Remove-ActiveSyncDeviceAccessRule";

		internal const string SetCmdlet = "Set-ActiveSyncDeviceAccessRule";

		internal const string ReadScope = "@C:OrganizationConfig";

		internal const string WriteScope = "@C:OrganizationConfig";

		internal const string GetDeviceCmdlet = "Get-MobileDevice";

		internal const string DeviceReadScope = "@R:Organization";

		private const string GetListRole = "Get-ActiveSyncDeviceAccessRule@C:OrganizationConfig";

		private const string GetObjectRole = "Get-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig";

		private const string GetObjectForNewRole = "Get-MobileDevice?Identity@R:Organization";

		private const string NewObjectRole = "New-ActiveSyncDeviceAccessRule@C:OrganizationConfig";

		private const string RemoveObjectsRole = "Remove-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig";

		private const string SetObjectRole = "Get-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig+Set-ActiveSyncDeviceAccessRule?Identity@C:OrganizationConfig";
	}
}
