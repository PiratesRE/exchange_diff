using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.License
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "LicenseSoap", Namespace = "http://microsoft.com/DRM/LicensingService")]
	internal class LicenseWS : WsAsyncProxyWrapper
	{
		public LicenseWS()
		{
			base.Url = "https://localhost/_wmcs/licensing/license.asmx";
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

		public event AcquireLicenseCompletedEventHandler AcquireLicenseCompleted;

		public event AcquirePreLicenseCompletedEventHandler AcquirePreLicenseCompleted;

		[SoapDocumentMethod("http://microsoft.com/DRM/LicensingService/AcquireLicense", RequestNamespace = "http://microsoft.com/DRM/LicensingService", ResponseNamespace = "http://microsoft.com/DRM/LicensingService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		public AcquireLicenseResponse[] AcquireLicense(AcquireLicenseParams[] RequestParams)
		{
			object[] array = base.Invoke("AcquireLicense", new object[]
			{
				RequestParams
			});
			return (AcquireLicenseResponse[])array[0];
		}

		public IAsyncResult BeginAcquireLicense(AcquireLicenseParams[] RequestParams, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AcquireLicense", new object[]
			{
				RequestParams
			}, callback, asyncState);
		}

		public AcquireLicenseResponse[] EndAcquireLicense(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AcquireLicenseResponse[])array[0];
		}

		public void AcquireLicenseAsync(AcquireLicenseParams[] RequestParams)
		{
			this.AcquireLicenseAsync(RequestParams, null);
		}

		public void AcquireLicenseAsync(AcquireLicenseParams[] RequestParams, object userState)
		{
			if (this.AcquireLicenseOperationCompleted == null)
			{
				this.AcquireLicenseOperationCompleted = new SendOrPostCallback(this.OnAcquireLicenseOperationCompleted);
			}
			base.InvokeAsync("AcquireLicense", new object[]
			{
				RequestParams
			}, this.AcquireLicenseOperationCompleted, userState);
		}

		private void OnAcquireLicenseOperationCompleted(object arg)
		{
			if (this.AcquireLicenseCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AcquireLicenseCompleted(this, new AcquireLicenseCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://microsoft.com/DRM/LicensingService/AcquirePreLicense", RequestNamespace = "http://microsoft.com/DRM/LicensingService", ResponseNamespace = "http://microsoft.com/DRM/LicensingService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		public AcquirePreLicenseResponse[] AcquirePreLicense(AcquirePreLicenseParams[] RequestParams)
		{
			object[] array = base.Invoke("AcquirePreLicense", new object[]
			{
				RequestParams
			});
			return (AcquirePreLicenseResponse[])array[0];
		}

		public IAsyncResult BeginAcquirePreLicense(AcquirePreLicenseParams[] RequestParams, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AcquirePreLicense", new object[]
			{
				RequestParams
			}, callback, asyncState);
		}

		public AcquirePreLicenseResponse[] EndAcquirePreLicense(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AcquirePreLicenseResponse[])array[0];
		}

		public void AcquirePreLicenseAsync(AcquirePreLicenseParams[] RequestParams)
		{
			this.AcquirePreLicenseAsync(RequestParams, null);
		}

		public void AcquirePreLicenseAsync(AcquirePreLicenseParams[] RequestParams, object userState)
		{
			if (this.AcquirePreLicenseOperationCompleted == null)
			{
				this.AcquirePreLicenseOperationCompleted = new SendOrPostCallback(this.OnAcquirePreLicenseOperationCompleted);
			}
			base.InvokeAsync("AcquirePreLicense", new object[]
			{
				RequestParams
			}, this.AcquirePreLicenseOperationCompleted, userState);
		}

		private void OnAcquirePreLicenseOperationCompleted(object arg)
		{
			if (this.AcquirePreLicenseCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AcquirePreLicenseCompleted(this, new AcquirePreLicenseCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private VersionData versionDataValueField;

		private SendOrPostCallback AcquireLicenseOperationCompleted;

		private SendOrPostCallback AcquirePreLicenseOperationCompleted;
	}
}
