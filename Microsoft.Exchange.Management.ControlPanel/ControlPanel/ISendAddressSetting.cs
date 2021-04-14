using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SendAddressSetting")]
	public interface ISendAddressSetting : IEditObjectService<SendAddressConfiguration, SetSendAddressDefaultConfiguration>, IGetObjectService<SendAddressConfiguration>
	{
	}
}
