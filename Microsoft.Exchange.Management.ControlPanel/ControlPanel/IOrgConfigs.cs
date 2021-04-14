using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "OrgConfigs")]
	public interface IOrgConfigs : IEditObjectService<OrgConfig, SetOrgConfig>, IGetObjectService<OrgConfig>
	{
	}
}
