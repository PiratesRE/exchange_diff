using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.Management.ManageDelegation1
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "ManageDelegationSoap", Namespace = "http://domains.live.com/Service/ManageDelegation/V1.0")]
	public class ManageDelegation : CustomSoapHttpClientProtocol
	{
		internal override XmlNamespaceDefinition[] PredefinedNamespaces
		{
			get
			{
				return ManageDelegation.predefinedNamespaces;
			}
		}

		protected override bool SuppressMustUnderstand
		{
			get
			{
				return true;
			}
		}

		public ManageDelegation(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback) : base(componentId, remoteCertificateValidationCallback, true)
		{
		}

		public ManageDelegation()
		{
			base.Url = "https://domains-tst.live-int.com/service/managedelegation.asmx";
		}

		public event CreateAppIdCompletedEventHandler CreateAppIdCompleted;

		public event UpdateAppIdCertificateCompletedEventHandler UpdateAppIdCertificateCompleted;

		public event UpdateAppIdPropertiesCompletedEventHandler UpdateAppIdPropertiesCompleted;

		public event AddUriCompletedEventHandler AddUriCompleted;

		public event RemoveUriCompletedEventHandler RemoveUriCompleted;

		public event ReserveDomainCompletedEventHandler ReserveDomainCompleted;

		public event ReleaseDomainCompletedEventHandler ReleaseDomainCompleted;

		public event GetDomainInfoCompletedEventHandler GetDomainInfoCompleted;

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://domains.live.com/Service/ManageDelegation/V1.0/CreateAppId", RequestNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", ResponseNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public AppIdInfo CreateAppId(string certificate, Property[] properties)
		{
			object[] array = this.Invoke("CreateAppId", new object[]
			{
				certificate,
				properties
			});
			return (AppIdInfo)array[0];
		}

		public IAsyncResult BeginCreateAppId(string certificate, Property[] properties, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateAppId", new object[]
			{
				certificate,
				properties
			}, callback, asyncState);
		}

		public AppIdInfo EndCreateAppId(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AppIdInfo)array[0];
		}

		public void CreateAppIdAsync(string certificate, Property[] properties)
		{
			this.CreateAppIdAsync(certificate, properties, null);
		}

		public void CreateAppIdAsync(string certificate, Property[] properties, object userState)
		{
			if (this.CreateAppIdOperationCompleted == null)
			{
				this.CreateAppIdOperationCompleted = new SendOrPostCallback(this.OnCreateAppIdOperationCompleted);
			}
			base.InvokeAsync("CreateAppId", new object[]
			{
				certificate,
				properties
			}, this.CreateAppIdOperationCompleted, userState);
		}

		private void OnCreateAppIdOperationCompleted(object arg)
		{
			if (this.CreateAppIdCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateAppIdCompleted(this, new CreateAppIdCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/ManageDelegation/V1.0/UpdateAppIdCertificate", RequestNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", ResponseNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHttpClientTraceExtension]
		public void UpdateAppIdCertificate(string appId, string appIdAdminKey, string newCertificate)
		{
			this.Invoke("UpdateAppIdCertificate", new object[]
			{
				appId,
				appIdAdminKey,
				newCertificate
			});
		}

		public IAsyncResult BeginUpdateAppIdCertificate(string appId, string appIdAdminKey, string newCertificate, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateAppIdCertificate", new object[]
			{
				appId,
				appIdAdminKey,
				newCertificate
			}, callback, asyncState);
		}

		public void EndUpdateAppIdCertificate(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void UpdateAppIdCertificateAsync(string appId, string appIdAdminKey, string newCertificate)
		{
			this.UpdateAppIdCertificateAsync(appId, appIdAdminKey, newCertificate, null);
		}

		public void UpdateAppIdCertificateAsync(string appId, string appIdAdminKey, string newCertificate, object userState)
		{
			if (this.UpdateAppIdCertificateOperationCompleted == null)
			{
				this.UpdateAppIdCertificateOperationCompleted = new SendOrPostCallback(this.OnUpdateAppIdCertificateOperationCompleted);
			}
			base.InvokeAsync("UpdateAppIdCertificate", new object[]
			{
				appId,
				appIdAdminKey,
				newCertificate
			}, this.UpdateAppIdCertificateOperationCompleted, userState);
		}

		private void OnUpdateAppIdCertificateOperationCompleted(object arg)
		{
			if (this.UpdateAppIdCertificateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateAppIdCertificateCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/ManageDelegation/V1.0/UpdateAppIdProperties", RequestNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", ResponseNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHttpClientTraceExtension]
		public void UpdateAppIdProperties(string appId, Property[] properties)
		{
			this.Invoke("UpdateAppIdProperties", new object[]
			{
				appId,
				properties
			});
		}

		public IAsyncResult BeginUpdateAppIdProperties(string appId, Property[] properties, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateAppIdProperties", new object[]
			{
				appId,
				properties
			}, callback, asyncState);
		}

		public void EndUpdateAppIdProperties(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void UpdateAppIdPropertiesAsync(string appId, Property[] properties)
		{
			this.UpdateAppIdPropertiesAsync(appId, properties, null);
		}

		public void UpdateAppIdPropertiesAsync(string appId, Property[] properties, object userState)
		{
			if (this.UpdateAppIdPropertiesOperationCompleted == null)
			{
				this.UpdateAppIdPropertiesOperationCompleted = new SendOrPostCallback(this.OnUpdateAppIdPropertiesOperationCompleted);
			}
			base.InvokeAsync("UpdateAppIdProperties", new object[]
			{
				appId,
				properties
			}, this.UpdateAppIdPropertiesOperationCompleted, userState);
		}

		private void OnUpdateAppIdPropertiesOperationCompleted(object arg)
		{
			if (this.UpdateAppIdPropertiesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateAppIdPropertiesCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://domains.live.com/Service/ManageDelegation/V1.0/AddUri", RequestNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", ResponseNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AddUri(string ownerAppId, string uri)
		{
			this.Invoke("AddUri", new object[]
			{
				ownerAppId,
				uri
			});
		}

		public IAsyncResult BeginAddUri(string ownerAppId, string uri, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddUri", new object[]
			{
				ownerAppId,
				uri
			}, callback, asyncState);
		}

		public void EndAddUri(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void AddUriAsync(string ownerAppId, string uri)
		{
			this.AddUriAsync(ownerAppId, uri, null);
		}

		public void AddUriAsync(string ownerAppId, string uri, object userState)
		{
			if (this.AddUriOperationCompleted == null)
			{
				this.AddUriOperationCompleted = new SendOrPostCallback(this.OnAddUriOperationCompleted);
			}
			base.InvokeAsync("AddUri", new object[]
			{
				ownerAppId,
				uri
			}, this.AddUriOperationCompleted, userState);
		}

		private void OnAddUriOperationCompleted(object arg)
		{
			if (this.AddUriCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddUriCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/ManageDelegation/V1.0/RemoveUri", RequestNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", ResponseNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHttpClientTraceExtension]
		public void RemoveUri(string ownerAppId, string uri)
		{
			this.Invoke("RemoveUri", new object[]
			{
				ownerAppId,
				uri
			});
		}

		public IAsyncResult BeginRemoveUri(string ownerAppId, string uri, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RemoveUri", new object[]
			{
				ownerAppId,
				uri
			}, callback, asyncState);
		}

		public void EndRemoveUri(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void RemoveUriAsync(string ownerAppId, string uri)
		{
			this.RemoveUriAsync(ownerAppId, uri, null);
		}

		public void RemoveUriAsync(string ownerAppId, string uri, object userState)
		{
			if (this.RemoveUriOperationCompleted == null)
			{
				this.RemoveUriOperationCompleted = new SendOrPostCallback(this.OnRemoveUriOperationCompleted);
			}
			base.InvokeAsync("RemoveUri", new object[]
			{
				ownerAppId,
				uri
			}, this.RemoveUriOperationCompleted, userState);
		}

		private void OnRemoveUriOperationCompleted(object arg)
		{
			if (this.RemoveUriCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RemoveUriCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/ManageDelegation/V1.0/ReserveDomain", RequestNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", ResponseNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHttpClientTraceExtension]
		public void ReserveDomain(string ownerAppId, string domainName, string programId)
		{
			this.Invoke("ReserveDomain", new object[]
			{
				ownerAppId,
				domainName,
				programId
			});
		}

		public IAsyncResult BeginReserveDomain(string ownerAppId, string domainName, string programId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ReserveDomain", new object[]
			{
				ownerAppId,
				domainName,
				programId
			}, callback, asyncState);
		}

		public void EndReserveDomain(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ReserveDomainAsync(string ownerAppId, string domainName, string programId)
		{
			this.ReserveDomainAsync(ownerAppId, domainName, programId, null);
		}

		public void ReserveDomainAsync(string ownerAppId, string domainName, string programId, object userState)
		{
			if (this.ReserveDomainOperationCompleted == null)
			{
				this.ReserveDomainOperationCompleted = new SendOrPostCallback(this.OnReserveDomainOperationCompleted);
			}
			base.InvokeAsync("ReserveDomain", new object[]
			{
				ownerAppId,
				domainName,
				programId
			}, this.ReserveDomainOperationCompleted, userState);
		}

		private void OnReserveDomainOperationCompleted(object arg)
		{
			if (this.ReserveDomainCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ReserveDomainCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://domains.live.com/Service/ManageDelegation/V1.0/ReleaseDomain", RequestNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", ResponseNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ReleaseDomain(string ownerAppId, string domainName)
		{
			this.Invoke("ReleaseDomain", new object[]
			{
				ownerAppId,
				domainName
			});
		}

		public IAsyncResult BeginReleaseDomain(string ownerAppId, string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ReleaseDomain", new object[]
			{
				ownerAppId,
				domainName
			}, callback, asyncState);
		}

		public void EndReleaseDomain(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ReleaseDomainAsync(string ownerAppId, string domainName)
		{
			this.ReleaseDomainAsync(ownerAppId, domainName, null);
		}

		public void ReleaseDomainAsync(string ownerAppId, string domainName, object userState)
		{
			if (this.ReleaseDomainOperationCompleted == null)
			{
				this.ReleaseDomainOperationCompleted = new SendOrPostCallback(this.OnReleaseDomainOperationCompleted);
			}
			base.InvokeAsync("ReleaseDomain", new object[]
			{
				ownerAppId,
				domainName
			}, this.ReleaseDomainOperationCompleted, userState);
		}

		private void OnReleaseDomainOperationCompleted(object arg)
		{
			if (this.ReleaseDomainCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ReleaseDomainCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://domains.live.com/Service/ManageDelegation/V1.0/GetDomainInfo", RequestNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", ResponseNamespace = "http://domains.live.com/Service/ManageDelegation/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public DomainInfo GetDomainInfo(string ownerAppId, string domainName)
		{
			object[] array = this.Invoke("GetDomainInfo", new object[]
			{
				ownerAppId,
				domainName
			});
			return (DomainInfo)array[0];
		}

		public IAsyncResult BeginGetDomainInfo(string ownerAppId, string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetDomainInfo", new object[]
			{
				ownerAppId,
				domainName
			}, callback, asyncState);
		}

		public DomainInfo EndGetDomainInfo(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DomainInfo)array[0];
		}

		public void GetDomainInfoAsync(string ownerAppId, string domainName)
		{
			this.GetDomainInfoAsync(ownerAppId, domainName, null);
		}

		public void GetDomainInfoAsync(string ownerAppId, string domainName, object userState)
		{
			if (this.GetDomainInfoOperationCompleted == null)
			{
				this.GetDomainInfoOperationCompleted = new SendOrPostCallback(this.OnGetDomainInfoOperationCompleted);
			}
			base.InvokeAsync("GetDomainInfo", new object[]
			{
				ownerAppId,
				domainName
			}, this.GetDomainInfoOperationCompleted, userState);
		}

		private void OnGetDomainInfoOperationCompleted(object arg)
		{
			if (this.GetDomainInfoCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDomainInfoCompleted(this, new GetDomainInfoCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private const string Namespace = "http://domains.live.com/Service/ManageDelegation/V1.0";

		private static readonly XmlNamespaceDefinition[] predefinedNamespaces = new XmlNamespaceDefinition[]
		{
			new XmlNamespaceDefinition("md", "http://domains.live.com/Service/ManageDelegation/V1.0")
		};

		private SendOrPostCallback CreateAppIdOperationCompleted;

		private SendOrPostCallback UpdateAppIdCertificateOperationCompleted;

		private SendOrPostCallback UpdateAppIdPropertiesOperationCompleted;

		private SendOrPostCallback AddUriOperationCompleted;

		private SendOrPostCallback RemoveUriOperationCompleted;

		private SendOrPostCallback ReserveDomainOperationCompleted;

		private SendOrPostCallback ReleaseDomainOperationCompleted;

		private SendOrPostCallback GetDomainInfoOperationCompleted;
	}
}
