using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class SearchAllGroups : DistributionGroupServiceBase, ISearchAllGroups, IGetListService<SearchAllGroupFilter, GroupRecipientRow>, IGetObjectService<ViewDistributionGroupData>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties@R:MyGAL")]
		public PowerShellResults<GroupRecipientRow> GetList(SearchAllGroupFilter filter, SortOptions sort)
		{
			return base.GetList<GroupRecipientRow, SearchAllGroupFilter>("Get-Recipient", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL")]
		public PowerShellResults<ViewDistributionGroupData> GetObject(Identity identity)
		{
			return base.GetDistributionGroup<ViewDistributionGroupData>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Add-DistributionGroupMember?Identity@W:MyGAL|MyDistributionGroups")]
		public PowerShellResults JoinGroups(Identity[] identities)
		{
			identities.FaultIfNullOrEmpty();
			Identity groupIdentityForTranslation = DistributionGroupServiceBase.GetGroupIdentityForTranslation(identities);
			PowerShellResults powerShellResults = new PowerShellResults();
			int num = 0;
			int num2 = -1;
			for (int i = 0; i < identities.Length; i++)
			{
				PSCommand psCommand = new PSCommand().AddCommand("Add-DistributionGroupMember").AddParameter("Identity", identities[i]);
				PowerShellResults powerShellResults2 = base.Invoke(psCommand, groupIdentityForTranslation, null);
				if (powerShellResults2.SucceededWithoutWarnings)
				{
					num++;
					if (num == 1)
					{
						num2 = i;
					}
				}
				powerShellResults.MergeErrors(powerShellResults2);
			}
			if (num > 0)
			{
				string text = (num == 1) ? OwaOptionStrings.JoinDlSuccess(identities[num2].DisplayName) : ((num == identities.Length) ? OwaOptionStrings.JoinDlsSuccess(num) : OwaOptionStrings.JoinOtherDlsSuccess(num));
				powerShellResults.Informations = new string[]
				{
					text
				};
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Add-DistributionGroupMember?Identity@W:MyGAL|MyDistributionGroups")]
		public PowerShellResults<ViewDistributionGroupData> JoinGroup(Identity identity)
		{
			return this.JoinOrLeaveGroup(identity, "Add-DistributionGroupMember");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-DistributionGroupMember?Identity@W:MyGAL|MyDistributionGroups")]
		public PowerShellResults<ViewDistributionGroupData> LeaveGroup(Identity identity)
		{
			return this.JoinOrLeaveGroup(identity, "Remove-DistributionGroupMember");
		}

		private PowerShellResults<ViewDistributionGroupData> JoinOrLeaveGroup(Identity identity, string cmdlet)
		{
			PowerShellResults<ViewDistributionGroupData> powerShellResults = new PowerShellResults<ViewDistributionGroupData>();
			PSCommand psCommand = new PSCommand().AddCommand(cmdlet).AddParameter("Identity", identity);
			powerShellResults.MergeErrors(base.Invoke(psCommand, identity, null));
			return powerShellResults;
		}

		internal const string ReadScope = "@R:MyGAL";

		internal const string WriteScope = "@W:MyGAL|MyDistributionGroups";

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties@R:MyGAL";

		private const string GetObjectRole = "Get-DistributionGroup?Identity@R:MyGAL+Get-Group?Identity@R:MyGAL";

		private const string JoinGroupRole = "Add-DistributionGroupMember?Identity@W:MyGAL|MyDistributionGroups";

		private const string LeaveGroupRole = "Remove-DistributionGroupMember?Identity@W:MyGAL|MyDistributionGroups";
	}
}
