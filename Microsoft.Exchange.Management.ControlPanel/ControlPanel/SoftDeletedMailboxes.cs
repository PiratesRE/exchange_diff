using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class SoftDeletedMailboxes : DataSourceService, ISoftDeletedMailboxes, IGetListService<SoftDeletedMailboxFilter, SoftDeletedMailboxRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Mailbox@R:Organization")]
		public PowerShellResults<SoftDeletedMailboxRow> GetList(SoftDeletedMailboxFilter filter, SortOptions sort)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Get-Mailbox");
			pscommand.AddParameter("SoftDeletedMailbox");
			return base.GetList<SoftDeletedMailboxRow, SoftDeletedMailboxFilter>(pscommand, filter, sort, "DeletionDate");
		}

		private const string GetListRole = "Get-Mailbox@R:Organization";
	}
}
