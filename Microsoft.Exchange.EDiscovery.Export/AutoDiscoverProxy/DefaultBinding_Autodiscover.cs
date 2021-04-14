using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DebuggerStepThrough]
	[XmlInclude(typeof(AutodiscoverRequest))]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "DefaultBinding_Autodiscover", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[XmlInclude(typeof(AutodiscoverResponse))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class DefaultBinding_Autodiscover : SoapHttpClientProtocol, IServiceBinding
	{
		public OpenAsAdminOrSystemServiceType OpenAsAdminOrSystemService { get; set; }

		public Action Action { get; set; }

		public To To { get; set; }

		public ServiceHttpContext HttpContext { get; set; }

		protected override XmlWriter GetWriterForMessage(SoapClientMessage message, int bufferSize)
		{
			if (this.Action != null)
			{
				message.Headers.Add(this.Action);
			}
			if (this.To != null)
			{
				message.Headers.Add(this.To);
			}
			if (this.OpenAsAdminOrSystemService != null)
			{
				this.OpenAsAdminOrSystemService.BudgetType = 1;
				this.OpenAsAdminOrSystemService.BudgetTypeSpecified = true;
				message.Headers.Add(this.OpenAsAdminOrSystemService);
			}
			return base.GetWriterForMessage(message, bufferSize);
		}

		protected override XmlReader GetReaderForMessage(SoapClientMessage message, int bufferSize)
		{
			XmlReader readerForMessage = base.GetReaderForMessage(message, bufferSize);
			XmlTextReader xmlTextReader = readerForMessage as XmlTextReader;
			if (xmlTextReader != null)
			{
				xmlTextReader.Normalization = false;
				xmlTextReader.DtdProcessing = DtdProcessing.Ignore;
				xmlTextReader.XmlResolver = null;
			}
			return new AutoDiscoverClientXmlReader(readerForMessage);
		}

		protected override WebRequest GetWebRequest(Uri url)
		{
			WebRequest webRequest = base.GetWebRequest(url);
			if (this.HttpContext != null)
			{
				this.HttpContext.SetRequestHttpHeaders(webRequest);
			}
			return webRequest;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse webResponse = base.GetWebResponse(request);
			if (this.HttpContext != null)
			{
				this.HttpContext.UpdateContextFromResponse(webResponse);
			}
			return webResponse;
		}

		public RequestedServerVersion RequestedServerVersionValue
		{
			get
			{
				return this.requestedServerVersionValueField;
			}
			set
			{
				this.requestedServerVersionValueField = value;
			}
		}

		public ServerVersionInfo ServerVersionInfoValue
		{
			get
			{
				return this.serverVersionInfoValueField;
			}
			set
			{
				this.serverVersionInfoValueField = value;
			}
		}

		public event GetUserSettingsCompletedEventHandler GetUserSettingsCompleted;

		public event GetDomainSettingsCompletedEventHandler GetDomainSettingsCompleted;

		public event GetFederationInformationCompletedEventHandler GetFederationInformationCompleted;

		public event GetOrganizationRelationshipSettingsCompletedEventHandler GetOrganizationRelationshipSettingsCompleted;

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetUserSettings", RequestElementName = "GetUserSettingsRequestMessage", RequestNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", ResponseElementName = "GetUserSettingsResponseMessage", ResponseNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("RequestedServerVersionValue")]
		[return: XmlElement("Response", IsNullable = true)]
		public GetUserSettingsResponse GetUserSettings([XmlElement(IsNullable = true)] GetUserSettingsRequest Request)
		{
			object[] array = base.Invoke("GetUserSettings", new object[]
			{
				Request
			});
			return (GetUserSettingsResponse)array[0];
		}

		public IAsyncResult BeginGetUserSettings(GetUserSettingsRequest Request, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUserSettings", new object[]
			{
				Request
			}, callback, asyncState);
		}

		public GetUserSettingsResponse EndGetUserSettings(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUserSettingsResponse)array[0];
		}

		public void GetUserSettingsAsync(GetUserSettingsRequest Request)
		{
			this.GetUserSettingsAsync(Request, null);
		}

		public void GetUserSettingsAsync(GetUserSettingsRequest Request, object userState)
		{
			if (this.GetUserSettingsOperationCompleted == null)
			{
				this.GetUserSettingsOperationCompleted = new SendOrPostCallback(this.OnGetUserSettingsOperationCompleted);
			}
			base.InvokeAsync("GetUserSettings", new object[]
			{
				Request
			}, this.GetUserSettingsOperationCompleted, userState);
		}

		private void OnGetUserSettingsOperationCompleted(object arg)
		{
			if (this.GetUserSettingsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUserSettingsCompleted(this, new GetUserSettingsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetDomainSettings", RequestElementName = "GetDomainSettingsRequestMessage", RequestNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", ResponseElementName = "GetDomainSettingsResponseMessage", ResponseNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestedServerVersionValue")]
		[return: XmlElement("Response", IsNullable = true)]
		public GetDomainSettingsResponse GetDomainSettings([XmlElement(IsNullable = true)] GetDomainSettingsRequest Request)
		{
			object[] array = base.Invoke("GetDomainSettings", new object[]
			{
				Request
			});
			return (GetDomainSettingsResponse)array[0];
		}

		public IAsyncResult BeginGetDomainSettings(GetDomainSettingsRequest Request, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetDomainSettings", new object[]
			{
				Request
			}, callback, asyncState);
		}

		public GetDomainSettingsResponse EndGetDomainSettings(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetDomainSettingsResponse)array[0];
		}

		public void GetDomainSettingsAsync(GetDomainSettingsRequest Request)
		{
			this.GetDomainSettingsAsync(Request, null);
		}

		public void GetDomainSettingsAsync(GetDomainSettingsRequest Request, object userState)
		{
			if (this.GetDomainSettingsOperationCompleted == null)
			{
				this.GetDomainSettingsOperationCompleted = new SendOrPostCallback(this.OnGetDomainSettingsOperationCompleted);
			}
			base.InvokeAsync("GetDomainSettings", new object[]
			{
				Request
			}, this.GetDomainSettingsOperationCompleted, userState);
		}

		private void OnGetDomainSettingsOperationCompleted(object arg)
		{
			if (this.GetDomainSettingsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDomainSettingsCompleted(this, new GetDomainSettingsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestedServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetFederationInformation", RequestElementName = "GetFederationInformationRequestMessage", RequestNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", ResponseElementName = "GetFederationInformationResponseMessage", ResponseNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("Response", IsNullable = true)]
		public GetFederationInformationResponse GetFederationInformation([XmlElement(IsNullable = true)] GetFederationInformationRequest Request)
		{
			object[] array = base.Invoke("GetFederationInformation", new object[]
			{
				Request
			});
			return (GetFederationInformationResponse)array[0];
		}

		public IAsyncResult BeginGetFederationInformation(GetFederationInformationRequest Request, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetFederationInformation", new object[]
			{
				Request
			}, callback, asyncState);
		}

		public GetFederationInformationResponse EndGetFederationInformation(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetFederationInformationResponse)array[0];
		}

		public void GetFederationInformationAsync(GetFederationInformationRequest Request)
		{
			this.GetFederationInformationAsync(Request, null);
		}

		public void GetFederationInformationAsync(GetFederationInformationRequest Request, object userState)
		{
			if (this.GetFederationInformationOperationCompleted == null)
			{
				this.GetFederationInformationOperationCompleted = new SendOrPostCallback(this.OnGetFederationInformationOperationCompleted);
			}
			base.InvokeAsync("GetFederationInformation", new object[]
			{
				Request
			}, this.GetFederationInformationOperationCompleted, userState);
		}

		private void OnGetFederationInformationOperationCompleted(object arg)
		{
			if (this.GetFederationInformationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetFederationInformationCompleted(this, new GetFederationInformationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetOrganizationRelationshipSettings", RequestElementName = "GetOrganizationRelationshipSettingsRequestMessage", RequestNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", ResponseElementName = "GetOrganizationRelationshipSettingsResponseMessage", ResponseNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("RequestedServerVersionValue")]
		[return: XmlElement("Response", IsNullable = true)]
		public GetOrganizationRelationshipSettingsResponse GetOrganizationRelationshipSettings([XmlElement(IsNullable = true)] GetOrganizationRelationshipSettingsRequest Request)
		{
			object[] array = base.Invoke("GetOrganizationRelationshipSettings", new object[]
			{
				Request
			});
			return (GetOrganizationRelationshipSettingsResponse)array[0];
		}

		public IAsyncResult BeginGetOrganizationRelationshipSettings(GetOrganizationRelationshipSettingsRequest Request, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetOrganizationRelationshipSettings", new object[]
			{
				Request
			}, callback, asyncState);
		}

		public GetOrganizationRelationshipSettingsResponse EndGetOrganizationRelationshipSettings(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetOrganizationRelationshipSettingsResponse)array[0];
		}

		public void GetOrganizationRelationshipSettingsAsync(GetOrganizationRelationshipSettingsRequest Request)
		{
			this.GetOrganizationRelationshipSettingsAsync(Request, null);
		}

		public void GetOrganizationRelationshipSettingsAsync(GetOrganizationRelationshipSettingsRequest Request, object userState)
		{
			if (this.GetOrganizationRelationshipSettingsOperationCompleted == null)
			{
				this.GetOrganizationRelationshipSettingsOperationCompleted = new SendOrPostCallback(this.OnGetOrganizationRelationshipSettingsOperationCompleted);
			}
			base.InvokeAsync("GetOrganizationRelationshipSettings", new object[]
			{
				Request
			}, this.GetOrganizationRelationshipSettingsOperationCompleted, userState);
		}

		private void OnGetOrganizationRelationshipSettingsOperationCompleted(object arg)
		{
			if (this.GetOrganizationRelationshipSettingsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetOrganizationRelationshipSettingsCompleted(this, new GetOrganizationRelationshipSettingsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private RequestedServerVersion requestedServerVersionValueField;

		private ServerVersionInfo serverVersionInfoValueField;

		private SendOrPostCallback GetUserSettingsOperationCompleted;

		private SendOrPostCallback GetDomainSettingsOperationCompleted;

		private SendOrPostCallback GetFederationInformationOperationCompleted;

		private SendOrPostCallback GetOrganizationRelationshipSettingsOperationCompleted;
	}
}
