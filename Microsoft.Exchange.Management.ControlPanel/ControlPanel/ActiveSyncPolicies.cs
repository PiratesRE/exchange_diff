using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ActiveSyncPolicies : DataSourceService, IActiveSyncPolicies, IDataSourceService<ActiveSyncMailboxPolicyFilter, ActiveSyncMailboxPolicyRow, ActiveSyncMailboxPolicyObject, SetActiveSyncMailboxPolicyParams, NewActiveSyncMailboxPolicyParams>, IDataSourceService<ActiveSyncMailboxPolicyFilter, ActiveSyncMailboxPolicyRow, ActiveSyncMailboxPolicyObject, SetActiveSyncMailboxPolicyParams, NewActiveSyncMailboxPolicyParams, BaseWebServiceParameters>, IEditListService<ActiveSyncMailboxPolicyFilter, ActiveSyncMailboxPolicyRow, ActiveSyncMailboxPolicyObject, NewActiveSyncMailboxPolicyParams, BaseWebServiceParameters>, IGetListService<ActiveSyncMailboxPolicyFilter, ActiveSyncMailboxPolicyRow>, INewObjectService<ActiveSyncMailboxPolicyRow, NewActiveSyncMailboxPolicyParams>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<ActiveSyncMailboxPolicyObject, SetActiveSyncMailboxPolicyParams, ActiveSyncMailboxPolicyRow>, IGetObjectService<ActiveSyncMailboxPolicyObject>, IGetObjectForListService<ActiveSyncMailboxPolicyRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileMailboxPolicy?Identity@R:Organization")]
		public PowerShellResults<ActiveSyncMailboxPolicyObject> GetObject(Identity identity)
		{
			return base.GetObject<ActiveSyncMailboxPolicyObject>("Get-MobileMailboxPolicy", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileMailboxPolicy@R:Organization")]
		public PowerShellResults<ActiveSyncMailboxPolicyRow> GetList(ActiveSyncMailboxPolicyFilter filter, SortOptions sort)
		{
			return base.GetList<ActiveSyncMailboxPolicyRow, ActiveSyncMailboxPolicyFilter>("Get-MobileMailboxPolicy", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-MobileMailboxPolicy@W:Organization")]
		public PowerShellResults<ActiveSyncMailboxPolicyRow> NewObject(NewActiveSyncMailboxPolicyParams properties)
		{
			return base.NewObject<ActiveSyncMailboxPolicyRow, NewActiveSyncMailboxPolicyParams>("New-MobileMailboxPolicy", properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-MobileMailboxPolicy?Identity@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-MobileMailboxPolicy", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileMailboxPolicy?Identity@R:Organization+Set-MobileMailboxPolicy?Identity@W:Organization")]
		public PowerShellResults<ActiveSyncMailboxPolicyRow> SetObject(Identity identity, SetActiveSyncMailboxPolicyParams properties)
		{
			PowerShellResults<ActiveSyncMailboxPolicyObject> @object = this.GetObject(identity);
			if (@object.Failed)
			{
				PowerShellResults<ActiveSyncMailboxPolicyRow> powerShellResults = new PowerShellResults<ActiveSyncMailboxPolicyRow>();
				powerShellResults.MergeErrors<ActiveSyncMailboxPolicyObject>(@object);
				return powerShellResults;
			}
			properties.FaultIfNull();
			properties.ProcessPolicyParams(@object.Value);
			if (properties.Name != null)
			{
				return base.SetObject<ActiveSyncMailboxPolicyObject, SetActiveSyncMailboxPolicyParams, ActiveSyncMailboxPolicyRow>("Set-MobileMailboxPolicy", identity, properties, new Identity(properties.Name, properties.Name));
			}
			return base.SetObject<ActiveSyncMailboxPolicyObject, SetActiveSyncMailboxPolicyParams, ActiveSyncMailboxPolicyRow>("Set-MobileMailboxPolicy", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileMailboxPolicy?Identity@R:Organization")]
		public PowerShellResults<ActiveSyncMailboxPolicyRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<ActiveSyncMailboxPolicyRow>("Get-MobileMailboxPolicy", identity);
		}

		internal const string GetCmdlet = "Get-MobileMailboxPolicy";

		internal const string SetCmdlet = "Set-MobileMailboxPolicy";

		internal const string NewCmdlet = "New-MobileMailboxPolicy";

		internal const string RemoveCmdlet = "Remove-MobileMailboxPolicy";

		private const string GetListRole = "Get-MobileMailboxPolicy@R:Organization";

		private const string GetObjectRole = "Get-MobileMailboxPolicy?Identity@R:Organization";

		private const string SetObjectRole = "Get-MobileMailboxPolicy?Identity@R:Organization+Set-MobileMailboxPolicy?Identity@W:Organization";

		private const string NewObjectRole = "New-MobileMailboxPolicy@W:Organization";

		private const string RemoveObjectRole = "Remove-MobileMailboxPolicy?Identity@W:Organization";
	}
}
