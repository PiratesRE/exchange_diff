using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MessageTrackingSearch")]
	public interface IMessageTrackingSearch : IGetListService<MessageTrackingSearchFilter, MessageTrackingSearchResultRow>
	{
	}
}
