using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "RegionalSettings")]
	public interface IRegionalSettings : IEditObjectService<RegionalSettingsConfiguration, SetRegionalSettingsConfiguration>, IGetObjectService<RegionalSettingsConfiguration>
	{
	}
}
