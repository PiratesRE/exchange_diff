using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class DistributionGroups : DistributionGroupServiceBase, IDistributionGroups, IDataSourceService<DistributionGroupFilter, DistributionGroupRow, DistributionGroup, SetDistributionGroup, NewDistributionGroupParameters>, IDataSourceService<DistributionGroupFilter, DistributionGroupRow, DistributionGroup, SetDistributionGroup, NewDistributionGroupParameters, BaseWebServiceParameters>, IEditListService<DistributionGroupFilter, DistributionGroupRow, DistributionGroup, NewDistributionGroupParameters, BaseWebServiceParameters>, IGetListService<DistributionGroupFilter, DistributionGroupRow>, INewObjectService<DistributionGroupRow, NewDistributionGroupParameters>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<DistributionGroup, SetDistributionGroup, DistributionGroupRow>, IGetObjectService<DistributionGroup>, IGetObjectForListService<DistributionGroupRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties@R:Organization")]
		public PowerShellResults<DistributionGroupRow> GetList(DistributionGroupFilter filter, SortOptions sort)
		{
			return base.GetList<DistributionGroupRow, DistributionGroupFilter>("Get-Recipient", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?Identity@R:Organization")]
		public PowerShellResults<DistributionGroupRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<DistributionGroupRow>("Get-Recipient", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization")]
		public PowerShellResults<DistributionGroup> GetObject(Identity identity)
		{
			return base.GetDistributionGroup<DistributionGroup>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+New-DistributionGroup?Name&Alias&PrimarySmtpAddress@W:MyDistributionGroups|Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Enterprise+New-DistributionGroup?Name&Alias@W:MyDistributionGroups|Organization")]
		public PowerShellResults<DistributionGroupRow> NewObject(NewDistributionGroupParameters properties)
		{
			if (RbacPrincipal.Current.IsInRole("New-DistributionGroup?IgnoreNamingPolicy"))
			{
				properties.IgnoreNamingPolicy = true;
			}
			PowerShellResults<DistributionGroupRow> powerShellResults = base.NewObject<DistributionGroupRow, NewDistributionGroupParameters>("New-DistributionGroup", properties);
			if (powerShellResults.SucceededWithValue && string.Compare(powerShellResults.Value.DisplayName, properties.Name) != 0)
			{
				string text = Strings.GroupNameWithNamingPolciy(powerShellResults.Value.DisplayName);
				powerShellResults.Informations = new string[]
				{
					text
				};
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-DistributionGroup?Identity@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-DistributionGroup", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+MultiTenant+Add-RecipientPermission?Identity@W:Organization+Remove-RecipientPermission?Identity@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Enterprise+Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+Enterprise+Get-ADPermission?Identity@R:Organization+Add-ADPermission?Identity@W:Organization+Remove-ADPermission?Identity@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+Set-DistributionGroup?Identity@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+Update-DistributionGroupMember?Identity@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+Set-Group?Identity@W:Organization")]
		public PowerShellResults<DistributionGroupRow> SetObject(Identity identity, SetDistributionGroup properties)
		{
			if (RbacPrincipal.Current.IsInRole("Set-DistributionGroup?IgnoreNamingPolicy"))
			{
				properties.IgnoreNamingPolicy = true;
			}
			return base.SetDistributionGroup<SetDistributionGroup, SetGroup, UpdateDistributionGroupMember>(identity, properties);
		}

		internal const string ReadScope = "@R:Organization";

		internal const string WriteScope = "@W:Organization";

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties@R:Organization";

		private const string GetObjectForListRole = "Get-Recipient?Identity@R:Organization";

		private const string GetObjectRole = "Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization";

		internal const string NewDGScope = "@W:MyDistributionGroups|Organization";

		private const string NewObjectRole_Enterprise = "Enterprise+New-DistributionGroup?Name&Alias@W:MyDistributionGroups|Organization";

		private const string NewObjectRole_MultiTenant = "MultiTenant+New-DistributionGroup?Name&Alias&PrimarySmtpAddress@W:MyDistributionGroups|Organization";

		private const string RemoveObjectsRole = "Remove-DistributionGroup?Identity@W:Organization";

		private const string SetObjectRole_SetGroup = "Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+Set-Group?Identity@W:Organization";

		private const string SetObjectRole_UpdateMember = "Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+Update-DistributionGroupMember?Identity@W:Organization";

		private const string SetObjectRole_SetDistributionGroup = "Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+Set-DistributionGroup?Identity@W:Organization";

		private const string SetObjectRole_DelegatePermissionEnt = "Enterprise+Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+Enterprise+Get-ADPermission?Identity@R:Organization+Add-ADPermission?Identity@W:Organization+Remove-ADPermission?Identity@W:Organization";

		private const string SetObjectRole_DelegatePermissionDC = "MultiTenant+Get-DistributionGroup?Identity@R:Organization+Get-Group?Identity@R:Organization+MultiTenant+Add-RecipientPermission?Identity@W:Organization+Remove-RecipientPermission?Identity@W:Organization";

		internal const string ConfigureDelegateEnt = "Enterprise+Get-ADPermission?Identity@R:Organization+Add-ADPermission?Identity@W:Organization+Remove-ADPermission?Identity@W:Organization";

		internal const string ConfigureDelegateDC = "MultiTenant+Add-RecipientPermission?Identity@W:Organization+Remove-RecipientPermission?Identity@W:Organization";
	}
}
