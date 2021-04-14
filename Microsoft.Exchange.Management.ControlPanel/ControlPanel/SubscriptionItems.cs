using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class SubscriptionItems : DataSourceService, ISubscriptionItems, IGetListService<SubscriptionItemFilter, SubscriptionItem>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-Subscription@R:Self")]
		public PowerShellResults<SubscriptionItem> GetList(SubscriptionItemFilter filter, SortOptions sort)
		{
			PowerShellResults<SubscriptionItem> list = base.GetList<SubscriptionItem, SubscriptionItemFilter>("Get-Subscription", filter, sort);
			if (list.Output != null && list.Output.Length > 0)
			{
				list.Output = Array.FindAll<SubscriptionItem>(list.Output, (SubscriptionItem x) => x.IsValid);
			}
			return list;
		}

		internal const string GetCmdlet = "Get-Subscription";

		internal const string ReadScope = "@R:Self";

		internal const string GetListRole = "MultiTenant+Get-Subscription@R:Self";

		private const string Noun = "Subscription";
	}
}
