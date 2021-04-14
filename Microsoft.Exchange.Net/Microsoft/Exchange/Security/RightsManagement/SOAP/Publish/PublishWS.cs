using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Publish
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "PublishSoap", Namespace = "http://microsoft.com/DRM/PublishingService")]
	internal class PublishWS : WsAsyncProxyWrapper
	{
		public PublishWS()
		{
			base.Url = "https://localhost/_wmcs/licensing/publish.asmx";
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

		public event AcquireIssuanceLicenseCompletedEventHandler AcquireIssuanceLicenseCompleted;

		public event GetClientLicensorCertCompletedEventHandler GetClientLicensorCertCompleted;

		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		[SoapDocumentMethod("http://microsoft.com/DRM/PublishingService/AcquireIssuanceLicense", RequestNamespace = "http://microsoft.com/DRM/PublishingService", ResponseNamespace = "http://microsoft.com/DRM/PublishingService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public AcquireIssuanceLicenseResponse[] AcquireIssuanceLicense(AcquireIssuanceLicenseParams[] RequestParams)
		{
			object[] array = base.Invoke("AcquireIssuanceLicense", new object[]
			{
				RequestParams
			});
			return (AcquireIssuanceLicenseResponse[])array[0];
		}

		public IAsyncResult BeginAcquireIssuanceLicense(AcquireIssuanceLicenseParams[] RequestParams, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AcquireIssuanceLicense", new object[]
			{
				RequestParams
			}, callback, asyncState);
		}

		public AcquireIssuanceLicenseResponse[] EndAcquireIssuanceLicense(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AcquireIssuanceLicenseResponse[])array[0];
		}

		public void AcquireIssuanceLicenseAsync(AcquireIssuanceLicenseParams[] RequestParams)
		{
			this.AcquireIssuanceLicenseAsync(RequestParams, null);
		}

		public void AcquireIssuanceLicenseAsync(AcquireIssuanceLicenseParams[] RequestParams, object userState)
		{
			if (this.AcquireIssuanceLicenseOperationCompleted == null)
			{
				this.AcquireIssuanceLicenseOperationCompleted = new SendOrPostCallback(this.OnAcquireIssuanceLicenseOperationCompleted);
			}
			base.InvokeAsync("AcquireIssuanceLicense", new object[]
			{
				RequestParams
			}, this.AcquireIssuanceLicenseOperationCompleted, userState);
		}

		private void OnAcquireIssuanceLicenseOperationCompleted(object arg)
		{
			if (this.AcquireIssuanceLicenseCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AcquireIssuanceLicenseCompleted(this, new AcquireIssuanceLicenseCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		[SoapDocumentMethod("http://microsoft.com/DRM/PublishingService/GetClientLicensorCert", RequestNamespace = "http://microsoft.com/DRM/PublishingService", ResponseNamespace = "http://microsoft.com/DRM/PublishingService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public GetClientLicensorCertResponse[] GetClientLicensorCert(GetClientLicensorCertParams[] RequestParams)
		{
			object[] array = base.Invoke("GetClientLicensorCert", new object[]
			{
				RequestParams
			});
			return (GetClientLicensorCertResponse[])array[0];
		}

		public IAsyncResult BeginGetClientLicensorCert(GetClientLicensorCertParams[] RequestParams, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetClientLicensorCert", new object[]
			{
				RequestParams
			}, callback, asyncState);
		}

		public GetClientLicensorCertResponse[] EndGetClientLicensorCert(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetClientLicensorCertResponse[])array[0];
		}

		public void GetClientLicensorCertAsync(GetClientLicensorCertParams[] RequestParams)
		{
			this.GetClientLicensorCertAsync(RequestParams, null);
		}

		public void GetClientLicensorCertAsync(GetClientLicensorCertParams[] RequestParams, object userState)
		{
			if (this.GetClientLicensorCertOperationCompleted == null)
			{
				this.GetClientLicensorCertOperationCompleted = new SendOrPostCallback(this.OnGetClientLicensorCertOperationCompleted);
			}
			base.InvokeAsync("GetClientLicensorCert", new object[]
			{
				RequestParams
			}, this.GetClientLicensorCertOperationCompleted, userState);
		}

		private void OnGetClientLicensorCertOperationCompleted(object arg)
		{
			if (this.GetClientLicensorCertCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetClientLicensorCertCompleted(this, new GetClientLicensorCertCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private VersionData versionDataValueField;

		private SendOrPostCallback AcquireIssuanceLicenseOperationCompleted;

		private SendOrPostCallback GetClientLicensorCertOperationCompleted;
	}
}
