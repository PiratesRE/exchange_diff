using System;
using System.ServiceModel;

namespace Microsoft.Exchange.PushNotifications
{
	[ServiceContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal interface IOutlookPublisherServiceContract
	{
		[FaultContract(typeof(PushNotificationFault))]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginPublishOutlookNotifications(OutlookNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState);

		void EndPublishOutlookNotifications(IAsyncResult result);
	}
}
