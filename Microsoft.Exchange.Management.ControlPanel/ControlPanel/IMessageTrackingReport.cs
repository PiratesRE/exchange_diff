using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MessageTrackingReport")]
	public interface IMessageTrackingReport : IGetListService<RecipientTrackingEventsFilter, RecipientStatusRow>, IGetObjectService<MessageTrackingReportRow>
	{
	}
}
