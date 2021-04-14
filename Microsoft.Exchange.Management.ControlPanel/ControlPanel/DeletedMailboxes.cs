using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class DeletedMailboxes : DataSourceService, IDeletedMailboxes, IGetListService<DeletedMailboxFilter, DeletedMailboxRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RemovedMailbox@R:Organization")]
		public PowerShellResults<DeletedMailboxRow> GetList(DeletedMailboxFilter filter, SortOptions sort)
		{
			return base.GetList<DeletedMailboxRow, DeletedMailboxFilter>("Get-RemovedMailbox", filter, sort, "DeletionDate");
		}

		private const string GetListRole = "Get-RemovedMailbox@R:Organization";
	}
}
