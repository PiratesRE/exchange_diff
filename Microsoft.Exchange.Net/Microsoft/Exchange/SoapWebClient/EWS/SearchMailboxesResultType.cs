using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SearchMailboxesResultType
	{
		[XmlArrayItem("MailboxQuery", IsNullable = false)]
		public MailboxQueryType[] SearchQueries;

		public SearchResultType ResultType;

		public long ItemCount;

		public long Size;

		public int PageItemCount;

		public long PageItemSize;

		[XmlArrayItem("KeywordStat", IsNullable = false)]
		public KeywordStatisticsSearchResultType[] KeywordStats;

		[XmlArrayItem("SearchPreviewItem", IsNullable = false)]
		public SearchPreviewItemType[] Items;

		[XmlArrayItem("FailedMailbox", IsNullable = false)]
		public FailedSearchMailboxType[] FailedMailboxes;

		[XmlArrayItem("Refiner", IsNullable = false)]
		public SearchRefinerItemType[] Refiners;

		[XmlArrayItem("MailboxStat", IsNullable = false)]
		public MailboxStatisticsItemType[] MailboxStats;
	}
}
