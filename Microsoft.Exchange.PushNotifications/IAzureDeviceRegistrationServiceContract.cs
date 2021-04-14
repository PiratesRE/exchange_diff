using System;
using System.ServiceModel;

namespace Microsoft.Exchange.PushNotifications
{
	[ServiceContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal interface IAzureDeviceRegistrationServiceContract
	{
		[FaultContract(typeof(PushNotificationFault))]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDeviceRegistration(AzureDeviceRegistrationInfo deviceRegistrationInfo, AsyncCallback asyncCallback, object asyncState);

		void EndDeviceRegistration(IAsyncResult result);
	}
}
