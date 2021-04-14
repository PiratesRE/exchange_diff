using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using Microsoft.com.IPC.WSService;

[ServiceContract(Namespace = "http://microsoft.com/IPC/WSCertificationService", ConfigurationName = "IWSCertificationService")]
[GeneratedCode("System.ServiceModel", "3.0.0.0")]
public interface IWSCertificationService
{
	[FaultContract(typeof(ActiveFederationFault), Action = "http://microsoft.com/IPC/WSCertificationService/IWSCertificationService/CertifyActiveFederationFaultFault", Name = "ActiveFederationFault", Namespace = "http://microsoft.com/IPC/WSService")]
	[OperationContract(Action = "http://microsoft.com/IPC/WSCertificationService/IWSCertificationService/Certify", ReplyAction = "http://microsoft.com/IPC/WSCertificationService/IWSCertificationService/CertifyResponse")]
	CertifyResponseMessage Certify(CertifyRequestMessage request);

	[OperationContract(AsyncPattern = true, Action = "http://microsoft.com/IPC/WSCertificationService/IWSCertificationService/Certify", ReplyAction = "http://microsoft.com/IPC/WSCertificationService/IWSCertificationService/CertifyResponse")]
	IAsyncResult BeginCertify(CertifyRequestMessage request, AsyncCallback callback, object asyncState);

	CertifyResponseMessage EndCertify(IAsyncResult result);
}
