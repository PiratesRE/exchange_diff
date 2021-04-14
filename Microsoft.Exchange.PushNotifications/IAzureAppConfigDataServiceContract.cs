using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Exchange.PushNotifications
{
	[ServiceContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal interface IAzureAppConfigDataServiceContract
	{
		[FaultContract(typeof(PushNotificationFault))]
		[WebInvoke(Method = "POST", UriTemplate = "GetAppConfigData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetAppConfigData(AzureAppConfigRequestInfo requestConfig, AsyncCallback asyncCallback, object asyncState);

		AzureAppConfigResponseInfo EndGetAppConfigData(IAsyncResult result);
	}
}
