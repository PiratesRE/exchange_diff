using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(CalendarViewType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(SeekToConditionPageViewType))]
	[XmlInclude(typeof(FractionalPageViewType))]
	[XmlInclude(typeof(IndexedPageViewType))]
	[XmlInclude(typeof(ContactsViewType))]
	[Serializable]
	public abstract class BasePagingType
	{
		[XmlAttribute]
		public int MaxEntriesReturned;

		[XmlIgnore]
		public bool MaxEntriesReturnedSpecified;
	}
}
