using System;
using System.ServiceModel;

namespace Microsoft.Exchange.PushNotifications
{
	[ServiceContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal interface IPublisherServiceContract
	{
		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(PushNotificationFault))]
		IAsyncResult BeginPublishNotifications(MailboxNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState);

		void EndPublishNotifications(IAsyncResult result);
	}
}
