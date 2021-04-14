using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class RmsTemplates : DataSourceService, IRmsTemplates, IGetListService<RmsTemplateFilter, RmsTemplate>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RMSTemplate@R:Organization")]
		public PowerShellResults<RmsTemplate> GetList(RmsTemplateFilter filter, SortOptions sort)
		{
			return base.GetList<RmsTemplate, RmsTemplateFilter>("Get-RMSTemplate", filter, sort);
		}

		private const string Noun = "RMSTemplate";

		internal const string GetCmdlet = "Get-RMSTemplate";

		internal const string ReadScope = "@R:Organization";

		internal const string GetListRole = "Get-RMSTemplate@R:Organization";
	}
}
