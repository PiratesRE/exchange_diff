using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ReadingPane")]
	public interface IReadingPane : IMessagingBase<ReadingPaneConfiguration, SetReadingPaneConfiguration>, IEditObjectService<ReadingPaneConfiguration, SetReadingPaneConfiguration>, IGetObjectService<ReadingPaneConfiguration>
	{
	}
}
