using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class UMCallDataRecordService : DataSourceService, IUMCallDataRecordService, IGetListService<UMCallDataRecordFilter, UMCallDataRecordRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMCallDataRecord?Mailbox@R:Organization")]
		public PowerShellResults<UMCallDataRecordRow> GetList(UMCallDataRecordFilter filter, SortOptions sort)
		{
			return base.GetList<UMCallDataRecordRow, UMCallDataRecordFilter>("Get-UMCallDataRecord", filter, sort);
		}

		internal const string GetCmdlet = "Get-UMCallDataRecord";

		internal const string ReadScope = "@R:Organization";

		private const string GetListRole = "Get-UMCallDataRecord?Mailbox@R:Organization";
	}
}
