using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.com.IPC.WSService;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "3.0.0.0")]
public class WSCertificationServiceClient : ClientBase<IWSCertificationService>, IWSCertificationService
{
	public WSCertificationServiceClient()
	{
	}

	public WSCertificationServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public WSCertificationServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public WSCertificationServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public WSCertificationServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public event EventHandler<CertifyCompletedEventArgs> CertifyCompleted;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	CertifyResponseMessage IWSCertificationService.Certify(CertifyRequestMessage request)
	{
		return base.Channel.Certify(request);
	}

	public XrmlCertificateChain Certify(ref VersionData VersionData, XrmlCertificateChain MachineCertificate)
	{
		CertifyResponseMessage certifyResponseMessage = ((IWSCertificationService)this).Certify(new CertifyRequestMessage
		{
			VersionData = VersionData,
			MachineCertificate = MachineCertificate
		});
		VersionData = certifyResponseMessage.VersionData;
		return certifyResponseMessage.GroupIdentityCredential;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	IAsyncResult IWSCertificationService.BeginCertify(CertifyRequestMessage request, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginCertify(request, callback, asyncState);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IAsyncResult BeginCertify(VersionData VersionData, XrmlCertificateChain MachineCertificate, AsyncCallback callback, object asyncState)
	{
		return ((IWSCertificationService)this).BeginCertify(new CertifyRequestMessage
		{
			VersionData = VersionData,
			MachineCertificate = MachineCertificate
		}, callback, asyncState);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	CertifyResponseMessage IWSCertificationService.EndCertify(IAsyncResult result)
	{
		return base.Channel.EndCertify(result);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public VersionData EndCertify(IAsyncResult result, out XrmlCertificateChain GroupIdentityCredential)
	{
		CertifyResponseMessage certifyResponseMessage = ((IWSCertificationService)this).EndCertify(result);
		GroupIdentityCredential = certifyResponseMessage.GroupIdentityCredential;
		return certifyResponseMessage.VersionData;
	}

	private IAsyncResult OnBeginCertify(object[] inValues, AsyncCallback callback, object asyncState)
	{
		VersionData versionData = (VersionData)inValues[0];
		XrmlCertificateChain machineCertificate = (XrmlCertificateChain)inValues[1];
		return this.BeginCertify(versionData, machineCertificate, callback, asyncState);
	}

	private object[] OnEndCertify(IAsyncResult result)
	{
		XrmlCertificateChain defaultValueForInitialization = base.GetDefaultValueForInitialization<XrmlCertificateChain>();
		VersionData versionData = this.EndCertify(result, out defaultValueForInitialization);
		return new object[]
		{
			defaultValueForInitialization,
			versionData
		};
	}

	private void OnCertifyCompleted(object state)
	{
		if (this.CertifyCompleted != null)
		{
			ClientBase<IWSCertificationService>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<IWSCertificationService>.InvokeAsyncCompletedEventArgs)state;
			this.CertifyCompleted(this, new CertifyCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
		}
	}

	public void CertifyAsync(VersionData VersionData, XrmlCertificateChain MachineCertificate)
	{
		this.CertifyAsync(VersionData, MachineCertificate, null);
	}

	public void CertifyAsync(VersionData VersionData, XrmlCertificateChain MachineCertificate, object userState)
	{
		if (this.onBeginCertifyDelegate == null)
		{
			this.onBeginCertifyDelegate = new ClientBase<IWSCertificationService>.BeginOperationDelegate(this.OnBeginCertify);
		}
		if (this.onEndCertifyDelegate == null)
		{
			this.onEndCertifyDelegate = new ClientBase<IWSCertificationService>.EndOperationDelegate(this.OnEndCertify);
		}
		if (this.onCertifyCompletedDelegate == null)
		{
			this.onCertifyCompletedDelegate = new SendOrPostCallback(this.OnCertifyCompleted);
		}
		base.InvokeAsync(this.onBeginCertifyDelegate, new object[]
		{
			VersionData,
			MachineCertificate
		}, this.onEndCertifyDelegate, this.onCertifyCompletedDelegate, userState);
	}

	private ClientBase<IWSCertificationService>.BeginOperationDelegate onBeginCertifyDelegate;

	private ClientBase<IWSCertificationService>.EndOperationDelegate onEndCertifyDelegate;

	private SendOrPostCallback onCertifyCompletedDelegate;
}
