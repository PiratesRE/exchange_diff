using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SendAddress")]
	public interface ISendAddress : IGetListService<SendAddressFilter, SendAddressRow>
	{
	}
}
