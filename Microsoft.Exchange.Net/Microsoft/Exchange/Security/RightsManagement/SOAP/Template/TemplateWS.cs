using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Template
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[WebServiceBinding(Name = "TemplateDistributionWebServiceSoap", Namespace = "http://microsoft.com/DRM/TemplateDistributionService")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	internal class TemplateWS : WsAsyncProxyWrapper
	{
		public TemplateWS()
		{
			base.Url = "http://localhost/_wmcs/licensing/TemplateDistribution.asmx";
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

		public event AcquireTemplateInformationCompletedEventHandler AcquireTemplateInformationCompleted;

		public event AcquireTemplatesCompletedEventHandler AcquireTemplatesCompleted;

		[SoapDocumentMethod("http://microsoft.com/DRM/TemplateDistributionService/AcquireTemplateInformation", RequestNamespace = "http://microsoft.com/DRM/TemplateDistributionService", ResponseNamespace = "http://microsoft.com/DRM/TemplateDistributionService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		public TemplateInformation AcquireTemplateInformation()
		{
			object[] array = base.Invoke("AcquireTemplateInformation", new object[0]);
			return (TemplateInformation)array[0];
		}

		public IAsyncResult BeginAcquireTemplateInformation(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AcquireTemplateInformation", new object[0], callback, asyncState);
		}

		public TemplateInformation EndAcquireTemplateInformation(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (TemplateInformation)array[0];
		}

		public void AcquireTemplateInformationAsync()
		{
			this.AcquireTemplateInformationAsync(null);
		}

		public void AcquireTemplateInformationAsync(object userState)
		{
			if (this.AcquireTemplateInformationOperationCompleted == null)
			{
				this.AcquireTemplateInformationOperationCompleted = new SendOrPostCallback(this.OnAcquireTemplateInformationOperationCompleted);
			}
			base.InvokeAsync("AcquireTemplateInformation", new object[0], this.AcquireTemplateInformationOperationCompleted, userState);
		}

		private void OnAcquireTemplateInformationOperationCompleted(object arg)
		{
			if (this.AcquireTemplateInformationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AcquireTemplateInformationCompleted(this, new AcquireTemplateInformationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("VersionDataValue", Direction = SoapHeaderDirection.InOut)]
		[SoapDocumentMethod("http://microsoft.com/DRM/TemplateDistributionService/AcquireTemplates", RequestNamespace = "http://microsoft.com/DRM/TemplateDistributionService", ResponseNamespace = "http://microsoft.com/DRM/TemplateDistributionService", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public GuidTemplate[] AcquireTemplates(string[] guids)
		{
			object[] array = base.Invoke("AcquireTemplates", new object[]
			{
				guids
			});
			return (GuidTemplate[])array[0];
		}

		public IAsyncResult BeginAcquireTemplates(string[] guids, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AcquireTemplates", new object[]
			{
				guids
			}, callback, asyncState);
		}

		public GuidTemplate[] EndAcquireTemplates(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GuidTemplate[])array[0];
		}

		public void AcquireTemplatesAsync(string[] guids)
		{
			this.AcquireTemplatesAsync(guids, null);
		}

		public void AcquireTemplatesAsync(string[] guids, object userState)
		{
			if (this.AcquireTemplatesOperationCompleted == null)
			{
				this.AcquireTemplatesOperationCompleted = new SendOrPostCallback(this.OnAcquireTemplatesOperationCompleted);
			}
			base.InvokeAsync("AcquireTemplates", new object[]
			{
				guids
			}, this.AcquireTemplatesOperationCompleted, userState);
		}

		private void OnAcquireTemplatesOperationCompleted(object arg)
		{
			if (this.AcquireTemplatesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AcquireTemplatesCompleted(this, new AcquireTemplatesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private VersionData versionDataValueField;

		private SendOrPostCallback AcquireTemplateInformationOperationCompleted;

		private SendOrPostCallback AcquireTemplatesOperationCompleted;
	}
}
