using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Net.XropService
{
	[ServiceContract(Name = "XropService", Namespace = "http://schemas.microsoft.com/exchange/2010/xrop", SessionMode = SessionMode.Required)]
	[CLSCompliant(false)]
	public interface IService
	{
		[OperationContract(Action = "http://schemas.microsoft.com/exchange/2010/xrop/Connect", AsyncPattern = true, IsInitiating = true, IsTerminating = false)]
		IAsyncResult BeginConnect(ConnectRequestMessage request, AsyncCallback asyncCallback, object asyncState);

		ConnectResponseMessage EndConnect(IAsyncResult asyncResult);

		[OperationContract(Action = "http://schemas.microsoft.com/exchange/2010/xrop/Execute", AsyncPattern = true, IsInitiating = false, IsTerminating = false)]
		IAsyncResult BeginExecute(ExecuteRequestMessage request, AsyncCallback asyncCallback, object asyncState);

		ExecuteResponseMessage EndExecute(IAsyncResult asyncResult);

		[OperationContract(Action = "http://schemas.microsoft.com/exchange/2010/xrop/Disconnect", AsyncPattern = true, IsInitiating = false, IsTerminating = true)]
		IAsyncResult BeginDisconnect(DisconnectRequestMessage request, AsyncCallback asyncCallback, object asyncState);

		DisconnectResponseMessage EndDisconnect(IAsyncResult asyncResult);
	}
}
