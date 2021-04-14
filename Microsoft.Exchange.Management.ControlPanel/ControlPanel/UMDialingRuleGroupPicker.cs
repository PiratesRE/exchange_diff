using System;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class UMDialingRuleGroupPicker : DataSourceService, IUMDialingRuleGroupPicker, IGetListService<UMDialPlanFilterWithIdentity, UMDialingRuleGroupRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMDialPlan?Identity@R:Organization")]
		public PowerShellResults<UMDialingRuleGroupRow> GetList(UMDialPlanFilterWithIdentity filter, SortOptions sort)
		{
			PowerShellResults<UMDialingRuleGroupRow> powerShellResults = new PowerShellResults<UMDialingRuleGroupRow>();
			filter.FaultIfNull();
			filter.DialPlanIdentity.FaultIfNull();
			PowerShellResults<UMDialPlanObjectWithGroupList> @object = base.GetObject<UMDialPlanObjectWithGroupList>("Get-UMDialPlan", filter.DialPlanIdentity);
			powerShellResults.MergeErrors<UMDialPlanObjectWithGroupList>(@object);
			if (@object.SucceededWithValue)
			{
				if (filter.IsInternational)
				{
					powerShellResults.Output = (from dialGroupName in @object.Value.ConfiguredInternationalGroupNameList
					select new UMDialingRuleGroupRow(dialGroupName)).ToArray<UMDialingRuleGroupRow>();
				}
				else
				{
					powerShellResults.Output = (from dialGroupName in @object.Value.ConfiguredInCountryOrRegionGroupNameList
					select new UMDialingRuleGroupRow(dialGroupName)).ToArray<UMDialingRuleGroupRow>();
				}
			}
			return powerShellResults;
		}

		private const string GetUMDialPlanRole = "Get-UMDialPlan?Identity@R:Organization";
	}
}
