using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Exchange.PushNotifications
{
	[ServiceContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal interface IRemoteUserNotificationPublisherServiceContract
	{
		[WebInvoke(Method = "POST", UriTemplate = "PublishUserNotification", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(PushNotificationFault))]
		IAsyncResult BeginPublishUserNotification(RemoteUserNotification notification, AsyncCallback asyncCallback, object asyncState);

		void EndPublishUserNotification(IAsyncResult result);
	}
}
