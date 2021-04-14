using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class SendAddress : DataSourceService, ISendAddress, IGetListService<SendAddressFilter, SendAddressRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-SendAddress?Mailbox@R:Self")]
		public PowerShellResults<SendAddressRow> GetList(SendAddressFilter filter, SortOptions sort)
		{
			return base.GetList<SendAddressRow, SendAddressFilter>("Get-SendAddress", filter, sort);
		}

		internal const string GetCmdlet = "Get-SendAddress";

		internal const string ReadScope = "@R:Self";

		private const string GetListRole = "MultiTenant+Get-SendAddress?Mailbox@R:Self";
	}
}
