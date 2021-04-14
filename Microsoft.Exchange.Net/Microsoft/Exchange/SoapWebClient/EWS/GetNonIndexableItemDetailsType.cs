using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetNonIndexableItemDetailsType : BaseRequestType
	{
		[XmlArrayItem("LegacyDN", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Mailboxes;

		public int PageSize;

		[XmlIgnore]
		public bool PageSizeSpecified;

		public string PageItemReference;

		public SearchPageDirectionType PageDirection;

		[XmlIgnore]
		public bool PageDirectionSpecified;

		public bool SearchArchiveOnly;

		[XmlIgnore]
		public bool SearchArchiveOnlySpecified;
	}
}
