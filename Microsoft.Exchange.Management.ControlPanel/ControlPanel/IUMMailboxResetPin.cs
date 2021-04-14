using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMMailboxResetPin")]
	public interface IUMMailboxResetPin : IEditObjectService<SetUMMailboxPinConfiguration, SetUMMailboxPinParameters>, IGetObjectService<SetUMMailboxPinConfiguration>
	{
	}
}
