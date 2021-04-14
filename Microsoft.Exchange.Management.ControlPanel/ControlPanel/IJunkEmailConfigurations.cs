using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "JunkEmailConfigurations")]
	public interface IJunkEmailConfigurations : IEditObjectService<JunkEmailConfiguration, SetJunkEmailConfiguration>, IGetObjectService<JunkEmailConfiguration>
	{
	}
}
