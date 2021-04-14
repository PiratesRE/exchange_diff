using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary.SharepointService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[WebServiceBinding(Name = "ListsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	internal class Lists : SoapHttpClientProtocol
	{
		public Lists(string serverUrl)
		{
			base.Url = serverUrl + "/_vti_bin/Lists.asmx";
			base.Credentials = CredentialCache.DefaultCredentials;
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListCollection", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListCollection()
		{
			object[] array = base.Invoke("GetListCollection", Array<object>.Empty);
			return (XmlNode)array[0];
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListAndView", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListAndView(string listName, string viewName)
		{
			object[] array = base.Invoke("GetListAndView", new object[]
			{
				listName,
				viewName
			});
			return (XmlNode)array[0];
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItems", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode GetListItems(string listName, string viewName, XmlNode query, XmlNode viewFields, string rowLimit, XmlNode queryOptions)
		{
			object[] array = base.Invoke("GetListItems", new object[]
			{
				listName,
				viewName,
				query,
				viewFields,
				rowLimit,
				queryOptions
			});
			return (XmlNode)array[0];
		}
	}
}
