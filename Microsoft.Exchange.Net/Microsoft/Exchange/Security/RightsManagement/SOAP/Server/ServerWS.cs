using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Server
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[WebServiceBinding(Name = "ServerSoap", Namespace = "http://microsoft.com/DRM/ServerService")]
	internal class ServerWS : WsAsyncProxyWrapper
	{
		public ServerWS()
		{
			base.Url = "https://localhost/_wmcs/licensing/server.asmx";
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

		public event GetLicensorCertificateCompletedEventHandler GetLicensorCertificateCompleted;

		public event FindServiceLocationsCompletedEventHandler FindServiceLocationsCompleted;

		public event GetServerInfoCompletedEventHandler GetServerInfoCompleted;

		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		[SoapDocumentMethod("http://microsoft.com/DRM/ServerService/GetLicensorCertificate", RequestNamespace = "http://microsoft.com/DRM/ServerService", ResponseNamespace = "http://microsoft.com/DRM/ServerService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public LicensorCertChain GetLicensorCertificate()
		{
			object[] array = base.Invoke("GetLicensorCertificate", new object[0]);
			return (LicensorCertChain)array[0];
		}

		public IAsyncResult BeginGetLicensorCertificate(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetLicensorCertificate", new object[0], callback, asyncState);
		}

		public LicensorCertChain EndGetLicensorCertificate(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (LicensorCertChain)array[0];
		}

		public void GetLicensorCertificateAsync()
		{
			this.GetLicensorCertificateAsync(null);
		}

		public void GetLicensorCertificateAsync(object userState)
		{
			if (this.GetLicensorCertificateOperationCompleted == null)
			{
				this.GetLicensorCertificateOperationCompleted = new SendOrPostCallback(this.OnGetLicensorCertificateOperationCompleted);
			}
			base.InvokeAsync("GetLicensorCertificate", new object[0], this.GetLicensorCertificateOperationCompleted, userState);
		}

		private void OnGetLicensorCertificateOperationCompleted(object arg)
		{
			if (this.GetLicensorCertificateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetLicensorCertificateCompleted(this, new GetLicensorCertificateCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		[SoapDocumentMethod("http://microsoft.com/DRM/ServerService/FindServiceLocations", RequestNamespace = "http://microsoft.com/DRM/ServerService", ResponseNamespace = "http://microsoft.com/DRM/ServerService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ServiceLocationResponse[] FindServiceLocations(ServiceLocationRequest[] ServiceNames)
		{
			object[] array = base.Invoke("FindServiceLocations", new object[]
			{
				ServiceNames
			});
			return (ServiceLocationResponse[])array[0];
		}

		public IAsyncResult BeginFindServiceLocations(ServiceLocationRequest[] ServiceNames, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("FindServiceLocations", new object[]
			{
				ServiceNames
			}, callback, asyncState);
		}

		public ServiceLocationResponse[] EndFindServiceLocations(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ServiceLocationResponse[])array[0];
		}

		public void FindServiceLocationsAsync(ServiceLocationRequest[] ServiceNames)
		{
			this.FindServiceLocationsAsync(ServiceNames, null);
		}

		public void FindServiceLocationsAsync(ServiceLocationRequest[] ServiceNames, object userState)
		{
			if (this.FindServiceLocationsOperationCompleted == null)
			{
				this.FindServiceLocationsOperationCompleted = new SendOrPostCallback(this.OnFindServiceLocationsOperationCompleted);
			}
			base.InvokeAsync("FindServiceLocations", new object[]
			{
				ServiceNames
			}, this.FindServiceLocationsOperationCompleted, userState);
		}

		private void OnFindServiceLocationsOperationCompleted(object arg)
		{
			if (this.FindServiceLocationsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.FindServiceLocationsCompleted(this, new FindServiceLocationsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://microsoft.com/DRM/ServerService/GetServerInfo", RequestNamespace = "http://microsoft.com/DRM/ServerService", ResponseNamespace = "http://microsoft.com/DRM/ServerService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetServerInfo(ServerInfoRequest[] requests)
		{
			object[] array = base.Invoke("GetServerInfo", new object[]
			{
				requests
			});
			return (XmlNode)array[0];
		}

		public IAsyncResult BeginGetServerInfo(ServerInfoRequest[] requests, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetServerInfo", new object[]
			{
				requests
			}, callback, asyncState);
		}

		public XmlNode EndGetServerInfo(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (XmlNode)array[0];
		}

		public void GetServerInfoAsync(ServerInfoRequest[] requests)
		{
			this.GetServerInfoAsync(requests, null);
		}

		public void GetServerInfoAsync(ServerInfoRequest[] requests, object userState)
		{
			if (this.GetServerInfoOperationCompleted == null)
			{
				this.GetServerInfoOperationCompleted = new SendOrPostCallback(this.OnGetServerInfoOperationCompleted);
			}
			base.InvokeAsync("GetServerInfo", new object[]
			{
				requests
			}, this.GetServerInfoOperationCompleted, userState);
		}

		private void OnGetServerInfoOperationCompleted(object arg)
		{
			if (this.GetServerInfoCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetServerInfoCompleted(this, new GetServerInfoCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private VersionData versionDataValueField;

		private SendOrPostCallback GetLicensorCertificateOperationCompleted;

		private SendOrPostCallback FindServiceLocationsOperationCompleted;

		private SendOrPostCallback GetServerInfoOperationCompleted;
	}
}
