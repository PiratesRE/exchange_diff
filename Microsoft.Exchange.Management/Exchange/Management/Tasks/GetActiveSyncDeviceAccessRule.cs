using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ActiveSyncDeviceAccessRule", DefaultParameterSetName = "Identity")]
	public sealed class GetActiveSyncDeviceAccessRule : GetMultitenancySystemConfigurationObjectTask<ActiveSyncDeviceAccessRuleIdParameter, ActiveSyncDeviceAccessRule>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (((IConfigurationSession)base.DataSession).GetOrgContainer().OrganizationId == OrganizationId.ForestWideOrgId)
			{
				base.InternalProcessRecord();
				return;
			}
			if (this.Identity == null)
			{
				base.InternalProcessRecord();
				try
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.GlobalConfigSession, typeof(ActiveSyncDeviceAccessRule), this.InternalFilter, base.GlobalConfigSession.GetOrgContainerId(), this.DeepSearch));
					IEnumerable<ActiveSyncDeviceAccessRule> dataObjects = base.GlobalConfigSession.FindPaged<ActiveSyncDeviceAccessRule>(base.GlobalConfigSession.GetOrgContainerId(), QueryScope.SubTree, this.InternalFilter, this.InternalSortBy, this.PageSize);
					this.WriteResult<ActiveSyncDeviceAccessRule>(dataObjects);
					return;
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, (ErrorCategory)1002, null);
					return;
				}
			}
			LocalizedString? localizedString;
			IEnumerable<ActiveSyncDeviceAccessRule> dataObjects2 = base.GetDataObjects<ActiveSyncDeviceAccessRule>(this.Identity, base.GlobalConfigSession, base.GlobalConfigSession.GetOrgContainerId(), base.OptionalIdentityData, out localizedString);
			this.WriteResult<ActiveSyncDeviceAccessRule>(dataObjects2);
			if (!base.HasErrors && base.WriteObjectCount == 0U)
			{
				base.InternalProcessRecord();
			}
		}
	}
}
