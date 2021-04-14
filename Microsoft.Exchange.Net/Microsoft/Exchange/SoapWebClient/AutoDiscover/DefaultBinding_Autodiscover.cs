using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[WebServiceBinding(Name = "DefaultBinding_Autodiscover", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[XmlInclude(typeof(AutodiscoverResponse))]
	[XmlInclude(typeof(AutodiscoverRequest))]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class DefaultBinding_Autodiscover : CustomSoapHttpClientProtocol
	{
		public DefaultBinding_Autodiscover()
		{
		}

		public event GetUserSettingsCompletedEventHandler GetUserSettingsCompleted;

		public event GetDomainSettingsCompletedEventHandler GetDomainSettingsCompleted;

		public event GetFederationInformationCompletedEventHandler GetFederationInformationCompleted;

		public event GetOrganizationRelationshipSettingsCompletedEventHandler GetOrganizationRelationshipSettingsCompleted;

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetUserSettings", RequestElementName = "GetUserSettingsRequestMessage", RequestNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", ResponseElementName = "GetUserSettingsResponseMessage", ResponseNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestedServerVersionValue")]
		[return: XmlElement("Response", IsNullable = true)]
		public GetUserSettingsResponse GetUserSettings([XmlElement(IsNullable = true)] GetUserSettingsRequest Request)
		{
			object[] array = this.Invoke("GetUserSettings", new object[]
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

		[SoapHeader("RequestedServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetDomainSettings", RequestElementName = "GetDomainSettingsRequestMessage", RequestNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", ResponseElementName = "GetDomainSettingsResponseMessage", ResponseNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("Response", IsNullable = true)]
		public GetDomainSettingsResponse GetDomainSettings([XmlElement(IsNullable = true)] GetDomainSettingsRequest Request)
		{
			object[] array = this.Invoke("GetDomainSettings", new object[]
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

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestedServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetFederationInformation", RequestElementName = "GetFederationInformationRequestMessage", RequestNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", ResponseElementName = "GetFederationInformationResponseMessage", ResponseNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("Response", IsNullable = true)]
		public GetFederationInformationResponse GetFederationInformation([XmlElement(IsNullable = true)] GetFederationInformationRequest Request)
		{
			object[] array = this.Invoke("GetFederationInformation", new object[]
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

		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetOrganizationRelationshipSettings", RequestElementName = "GetOrganizationRelationshipSettingsRequestMessage", RequestNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", ResponseElementName = "GetOrganizationRelationshipSettingsResponseMessage", ResponseNamespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("RequestedServerVersionValue")]
		[return: XmlElement("Response", IsNullable = true)]
		public GetOrganizationRelationshipSettingsResponse GetOrganizationRelationshipSettings([XmlElement(IsNullable = true)] GetOrganizationRelationshipSettingsRequest Request)
		{
			object[] array = this.Invoke("GetOrganizationRelationshipSettings", new object[]
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

		public DefaultBinding_Autodiscover(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback) : base(componentId, remoteCertificateValidationCallback, true)
		{
			base.Authenticator = SoapHttpClientAuthenticator.CreateAnonymous();
		}

		internal override XmlNamespaceDefinition[] PredefinedNamespaces
		{
			get
			{
				return Constants.EwsNamespaces;
			}
		}

		public RequestedServerVersion RequestedServerVersionValue;

		public ServerVersionInfo ServerVersionInfoValue;

		private SendOrPostCallback GetUserSettingsOperationCompleted;

		private SendOrPostCallback GetDomainSettingsOperationCompleted;

		private SendOrPostCallback GetFederationInformationOperationCompleted;

		private SendOrPostCallback GetOrganizationRelationshipSettingsOperationCompleted;

		internal static RequestedServerVersion Exchange2010RequestedServerVersion = new RequestedServerVersion
		{
			Text = new string[]
			{
				"Exchange2010"
			}
		};
	}
}
