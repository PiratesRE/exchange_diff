using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.com.IPC.WSService;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "3.0.0.0")]
public class CertifyCompletedEventArgs : AsyncCompletedEventArgs
{
	public CertifyCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
	{
		this.results = results;
	}

	public XrmlCertificateChain GroupIdentityCredential
	{
		get
		{
			base.RaiseExceptionIfNecessary();
			return (XrmlCertificateChain)this.results[0];
		}
	}

	public VersionData Result
	{
		get
		{
			base.RaiseExceptionIfNecessary();
			return (VersionData)this.results[1];
		}
	}

	private object[] results;
}
