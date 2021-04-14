using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Exchange.PushNotifications
{
	[ServiceContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal interface IOnPremPublisherServiceContract
	{
		[WebInvoke(Method = "POST", UriTemplate = "PublishOnPremNotifications", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(PushNotificationFault))]
		IAsyncResult BeginPublishOnPremNotifications(MailboxNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState);

		void EndPublishOnPremNotifications(IAsyncResult result);
	}
}
