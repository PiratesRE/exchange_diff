using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class OwnedGroups : DistributionGroupServiceBase, IOwnedGroups, IGetListService<OwnedGroupFilter, DistributionGroupRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<DistributionGroup, SetMyDistributionGroup, DistributionGroupRow>, IGetObjectService<DistributionGroup>, IGetObjectForListService<DistributionGroupRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties@R:MyGAL")]
		public PowerShellResults<DistributionGroupRow> GetList(OwnedGroupFilter filter, SortOptions sort)
		{
			return base.GetList<DistributionGroupRow, OwnedGroupFilter>("Get-Recipient", filter, sort, "DisplayName");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?Identity@R:MyGAL")]
		public PowerShellResults<DistributionGroupRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<DistributionGroupRow>("Get-Recipient", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL")]
		public PowerShellResults<DistributionGroup> GetObject(Identity identity)
		{
			return base.GetDistributionGroup<DistributionGroup>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-DistributionGroup?Identity@W:MyDistributionGroups")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-DistributionGroup", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL+Set-DistributionGroup?Identity@W:MyDistributionGroups")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL+Update-DistributionGroupMember?Identity@W:MyDistributionGroups")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL+Set-Group?Identity@W:MyDistributionGroups")]
		public PowerShellResults<DistributionGroupRow> SetObject(Identity identity, SetMyDistributionGroup properties)
		{
			if (RbacPrincipal.Current.IsInRole("Set-DistributionGroup?IgnoreNamingPolicy"))
			{
				properties.IgnoreNamingPolicy = true;
			}
			return base.SetDistributionGroup<SetMyDistributionGroup, SetMyGroup, UpdateMyDistributionGroupMember>(identity, properties);
		}

		internal const string ReadScope = "@R:MyGAL";

		internal const string WriteScope = "@W:MyDistributionGroups";

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties@R:MyGAL";

		private const string GetObjectForListRole = "Get-Recipient?Identity@R:MyGAL";

		private const string GetObjectRole = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL";

		private const string RemoveObjectsRole = "Remove-DistributionGroup?Identity@W:MyDistributionGroups";

		private const string SetObjectRole_SetGroup = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL+Set-Group?Identity@W:MyDistributionGroups";

		private const string SetObjectRole_UpdateMember = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL+Update-DistributionGroupMember?Identity@W:MyDistributionGroups";

		private const string SetObjectRole_SetDistributionGroup = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL+Set-DistributionGroup?Identity@W:MyDistributionGroups";
	}
}
