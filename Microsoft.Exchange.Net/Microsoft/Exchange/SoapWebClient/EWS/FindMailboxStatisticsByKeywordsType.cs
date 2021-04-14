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
	public class FindMailboxStatisticsByKeywordsType : BaseRequestType
	{
		[XmlArrayItem("UserMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public UserMailboxType[] Mailboxes;

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Keywords;

		public string Language;

		[XmlArrayItem("SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Senders;

		[XmlArrayItem("SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Recipients;

		public DateTime FromDate;

		[XmlIgnore]
		public bool FromDateSpecified;

		public DateTime ToDate;

		[XmlIgnore]
		public bool ToDateSpecified;

		[XmlArrayItem("SearchItemKind", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public SearchItemKindType[] MessageTypes;

		public bool SearchDumpster;

		[XmlIgnore]
		public bool SearchDumpsterSpecified;

		public bool IncludePersonalArchive;

		[XmlIgnore]
		public bool IncludePersonalArchiveSpecified;

		public bool IncludeUnsearchableItems;

		[XmlIgnore]
		public bool IncludeUnsearchableItemsSpecified;
	}
}
