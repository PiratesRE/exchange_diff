using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MessageCategories : DataSourceService, IMessageCategories, IGetListService<MessageCategoryFilter, MessageCategory>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MessageCategory@R:Self")]
		public PowerShellResults<MessageCategory> GetList(MessageCategoryFilter filter, SortOptions sort)
		{
			return base.GetList<MessageCategory, MessageCategoryFilter>("Get-MessageCategory", filter, sort);
		}

		private const string Noun = "MessageCategory";

		internal const string GetCmdlet = "Get-MessageCategory";

		internal const string ReadScope = "@R:Self";

		private const string GetListRole = "Get-MessageCategory@R:Self";
	}
}
