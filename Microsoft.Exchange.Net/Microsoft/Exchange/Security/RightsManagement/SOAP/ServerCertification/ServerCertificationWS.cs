using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.ServerCertification
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[WebServiceBinding(Name = "ServerCertificationWebServiceSoap", Namespace = "http://microsoft.com/DRM/CertificationService")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	internal class ServerCertificationWS : WsAsyncProxyWrapper
	{
		public ServerCertificationWS()
		{
			base.Url = "https://localhost/_wmcs/certification/ServerCertification.asmx";
		}

		public VersionData VersionDataValue
		{
			get
			{
				return this.versionDataValueField;
			}
			set
			{
				this.versionDataValueField = value;
			}
		}

		public event CertifyCompletedEventHandler CertifyCompleted;

		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		[SoapDocumentMethod("http://microsoft.com/DRM/CertificationService/Certify", RequestNamespace = "http://microsoft.com/DRM/CertificationService", ResponseNamespace = "http://microsoft.com/DRM/CertificationService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public CertifyResponse Certify(CertifyParams requestParams)
		{
			object[] array = base.Invoke("Certify", new object[]
			{
				requestParams
			});
			return (CertifyResponse)array[0];
		}

		public IAsyncResult BeginCertify(CertifyParams requestParams, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("Certify", new object[]
			{
				requestParams
			}, callback, asyncState);
		}

		public CertifyResponse EndCertify(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CertifyResponse)array[0];
		}

		public void CertifyAsync(CertifyParams requestParams)
		{
			this.CertifyAsync(requestParams, null);
		}

		public void CertifyAsync(CertifyParams requestParams, object userState)
		{
			if (this.CertifyOperationCompleted == null)
			{
				this.CertifyOperationCompleted = new SendOrPostCallback(this.OnCertifyOperationCompleted);
			}
			base.InvokeAsync("Certify", new object[]
			{
				requestParams
			}, this.CertifyOperationCompleted, userState);
		}

		private void OnCertifyOperationCompleted(object arg)
		{
			if (this.CertifyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CertifyCompleted(this, new CertifyCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private VersionData versionDataValueField;

		private SendOrPostCallback CertifyOperationCompleted;
	}
}
