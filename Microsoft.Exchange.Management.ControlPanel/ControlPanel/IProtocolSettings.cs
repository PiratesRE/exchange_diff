using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ProtocolSettings")]
	public interface IProtocolSettings : IGetObjectService<ProtocolSettingsData>
	{
	}
}
