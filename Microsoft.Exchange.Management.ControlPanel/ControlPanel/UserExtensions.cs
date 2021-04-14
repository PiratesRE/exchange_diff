using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class UserExtensions : DataSourceService, IUserExtensions, IGetListService<UserExtensionsFilter, UMMailboxExtension>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox@R:Self")]
		public PowerShellResults<UMMailboxExtension> GetList(UserExtensionsFilter filter, SortOptions sort)
		{
			PowerShellResults<UMMailboxObject> @object = base.GetObject<UMMailboxObject>("Get-UMMailbox", Identity.FromExecutingUserId());
			if (@object.Output.Length == 1)
			{
				UMMailboxExtension[] array = new UMMailboxExtension[@object.Output[0].CallAnsweringRulesExtensions.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new UMMailboxExtension
					{
						DisplayName = @object.Output[0].CallAnsweringRulesExtensions[i]
					};
				}
				return new PowerShellResults<UMMailboxExtension>
				{
					Output = array
				};
			}
			return new PowerShellResults<UMMailboxExtension>();
		}

		internal const string ReadScope = "@R:Self";

		internal const string GetUMMailbox = "Get-UMMailbox";

		internal const string GetListRole = "Get-UMMailbox@R:Self";
	}
}
