using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class TransportConfigs : DataSourceService, ITransportConfigs, IGetListService<TransportConfigFilter, SupervisionTag>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportConfig@C:OrganizationConfig")]
		public PowerShellResults<SupervisionTag> GetList(TransportConfigFilter filter, SortOptions sort)
		{
			PowerShellResults<SupervisionTag> powerShellResults = new PowerShellResults<SupervisionTag>();
			PowerShellResults<TransportConfigContainer> powerShellResults2 = powerShellResults.MergeErrors<TransportConfigContainer>(base.GetList<TransportConfigContainer, TransportConfigFilter>("Get-TransportConfig", filter, sort));
			if (powerShellResults2.HasValue)
			{
				string[] array = powerShellResults2.Value.SupervisionTags.ToArray();
				if (array != null && array.Length > 0)
				{
					SupervisionTag[] array2 = new SupervisionTag[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = new SupervisionTag(array[i]);
					}
					powerShellResults.Output = array2;
				}
			}
			return powerShellResults;
		}

		private const string Noun = "TransportConfig";

		internal const string GetCmdlet = "Get-TransportConfig";

		internal const string ReadScope = "@C:OrganizationConfig";

		internal const string GetListRole = "Get-TransportConfig@C:OrganizationConfig";
	}
}
