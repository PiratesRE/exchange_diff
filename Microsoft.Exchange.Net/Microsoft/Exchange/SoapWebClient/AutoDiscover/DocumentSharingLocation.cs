using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class DocumentSharingLocation
	{
		public string ServiceUrl;

		public string LocationUrl;

		public string DisplayName;

		[XmlArrayItem("FileExtension", IsNullable = false)]
		public string[] SupportedFileExtensions;

		public bool ExternalAccessAllowed;

		public bool AnonymousAccessAllowed;

		public bool CanModifyPermissions;

		public bool IsDefault;
	}
}
