using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.com.IPC.WSService;

[DebuggerStepThrough]
[MessageContract(WrapperName = "CertifyRequestMessage", WrapperNamespace = "http://microsoft.com/IPC/WSCertificationService", IsWrapped = true)]
[GeneratedCode("System.ServiceModel", "3.0.0.0")]
public class CertifyRequestMessage
{
	public CertifyRequestMessage()
	{
	}

	public CertifyRequestMessage(VersionData VersionData, XrmlCertificateChain MachineCertificate)
	{
		this.VersionData = VersionData;
		this.MachineCertificate = MachineCertificate;
	}

	[MessageHeader(Namespace = "http://microsoft.com/IPC/WSService")]
	public VersionData VersionData;

	[MessageBodyMember(Namespace = "http://microsoft.com/IPC/WSCertificationService", Order = 0)]
	public XrmlCertificateChain MachineCertificate;
}
