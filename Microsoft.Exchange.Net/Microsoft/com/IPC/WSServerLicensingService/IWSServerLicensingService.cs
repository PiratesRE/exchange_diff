using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using Microsoft.com.IPC.WSService;

namespace Microsoft.com.IPC.WSServerLicensingService
{
	[ServiceContract(Namespace = "http://microsoft.com/IPC/WSServerLicensingService", ConfigurationName = "Microsoft.com.IPC.WSServerLicensingService.IWSServerLicensingService")]
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	public interface IWSServerLicensingService
	{
		[OperationContract(Action = "http://microsoft.com/IPC/WSServerLicensingService/IWSServerLicensingService/AcquireServerLicense", ReplyAction = "http://microsoft.com/IPC/WSServerLicensingService/IWSServerLicensingService/AcquireServerLicenseResponse")]
		[FaultContract(typeof(ActiveFederationFault), Action = "http://microsoft.com/IPC/WSServerLicensingService/IWSServerLicensingService/AcquireServerLicenseActiveFederationFaultFault", Name = "ActiveFederationFault", Namespace = "http://microsoft.com/IPC/WSService")]
		AcquireServerLicenseResponseMessage AcquireServerLicense(AcquireServerLicenseRequestMessage request);

		[OperationContract(AsyncPattern = true, Action = "http://microsoft.com/IPC/WSServerLicensingService/IWSServerLicensingService/AcquireServerLicense", ReplyAction = "http://microsoft.com/IPC/WSServerLicensingService/IWSServerLicensingService/AcquireServerLicenseResponse")]
		IAsyncResult BeginAcquireServerLicense(AcquireServerLicenseRequestMessage request, AsyncCallback callback, object asyncState);

		AcquireServerLicenseResponseMessage EndAcquireServerLicense(IAsyncResult result);
	}
}
