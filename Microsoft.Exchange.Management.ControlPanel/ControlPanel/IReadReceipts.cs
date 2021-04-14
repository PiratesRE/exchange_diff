using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ReadReceipts")]
	public interface IReadReceipts : IMessagingBase<ReadReceiptsConfiguration, SetReadReceiptsConfiguration>, IEditObjectService<ReadReceiptsConfiguration, SetReadReceiptsConfiguration>, IGetObjectService<ReadReceiptsConfiguration>
	{
	}
}
