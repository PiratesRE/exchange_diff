using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MemberOfGroups : DataSourceService, IMemberOfGroups, IGetListService<MemberOfGroupFilter, RecipientRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties@R:MyGAL")]
		public PowerShellResults<RecipientRow> GetList(MemberOfGroupFilter filter, SortOptions sort)
		{
			return base.GetList<RecipientRow, MemberOfGroupFilter>("Get-Recipient", filter, sort, "DisplayName");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-DistributionGroupMember?Identity@W:MyGAL")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			identities.FaultIfNullOrEmpty();
			Identity groupIdentityForTranslation = DistributionGroupServiceBase.GetGroupIdentityForTranslation(identities);
			PowerShellResults powerShellResults = new PowerShellResults();
			for (int i = 0; i < identities.Length; i++)
			{
				PSCommand psCommand = new PSCommand().AddCommand("Remove-DistributionGroupMember").AddParameter("Identity", identities[i]);
				PowerShellResults results = base.Invoke(psCommand, groupIdentityForTranslation, parameters);
				powerShellResults.MergeErrors(results);
			}
			return powerShellResults;
		}

		internal const string ReadScope = "@R:MyGAL";

		internal const string WriteScope = "@W:MyGAL";

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties@R:MyGAL";

		private const string RemoveObjectsRole = "Remove-DistributionGroupMember?Identity@W:MyGAL";
	}
}
