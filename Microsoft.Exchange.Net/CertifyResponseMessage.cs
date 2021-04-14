using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.com.IPC.WSService;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "3.0.0.0")]
[MessageContract(WrapperName = "CertifyResponseMessage", WrapperNamespace = "http://microsoft.com/IPC/WSCertificationService", IsWrapped = true)]
public class CertifyResponseMessage
{
	public CertifyResponseMessage()
	{
	}

	public CertifyResponseMessage(VersionData VersionData, XrmlCertificateChain GroupIdentityCredential)
	{
		this.VersionData = VersionData;
		this.GroupIdentityCredential = GroupIdentityCredential;
	}

	[MessageHeader(Namespace = "http://microsoft.com/IPC/WSService")]
	public VersionData VersionData;

	[MessageBodyMember(Namespace = "http://microsoft.com/IPC/WSCertificationService", Order = 0)]
	public XrmlCertificateChain GroupIdentityCredential;
}
