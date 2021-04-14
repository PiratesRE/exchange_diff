using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary.SharepointService
{
	[DesignerCategory("code")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[WebServiceBinding(Name = "WebsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
	[DebuggerStepThrough]
	internal class Webs : SoapHttpClientProtocol
	{
		public Webs(string url)
		{
			this.Url = url + "/_vti_bin/webs.asmx";
			this.UseDefaultCredentials = true;
		}

		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				base.Url = value;
			}
		}

		public new bool UseDefaultCredentials
		{
			get
			{
				return base.UseDefaultCredentials;
			}
			set
			{
				base.UseDefaultCredentials = value;
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/WebUrlFromPageUrl", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string WebUrlFromPageUrl(string pageUrl)
		{
			object[] array = base.Invoke("WebUrlFromPageUrl", new object[]
			{
				pageUrl
			});
			return (string)array[0];
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetWebCollection", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetWebCollection()
		{
			object[] array = base.Invoke("GetWebCollection", Array<object>.Empty);
			return (XmlNode)array[0];
		}
	}
}
