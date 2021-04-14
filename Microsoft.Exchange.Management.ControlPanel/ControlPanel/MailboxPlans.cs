using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MailboxPlans : DataSourceService, IMailboxPlans, IGetListService<MailboxPlanFilter, MailboxPlan>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Dedicated+Get-MailboxPlan@R:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-MailboxPlan@R:Organization")]
		public PowerShellResults<MailboxPlan> GetList(MailboxPlanFilter filter, SortOptions sort)
		{
			return base.GetList<MailboxPlan, MailboxPlanFilter>("Get-MailboxPlan", filter, sort);
		}

		private const string Noun = "MailboxPlan";

		internal const string GetCmdlet = "Get-MailboxPlan";

		internal const string ReadScope = "@R:Organization";

		internal const string GetListRole_MultiTenant = "MultiTenant+Get-MailboxPlan@R:Organization";

		internal const string GetListRole_Dedicated = "Dedicated+Get-MailboxPlan@R:Organization";
	}
}
