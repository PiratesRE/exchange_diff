using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(FractionalPageViewType))]
	[XmlInclude(typeof(ContactsViewType))]
	[XmlInclude(typeof(CalendarViewType))]
	[XmlInclude(typeof(SeekToConditionPageViewType))]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(IndexedPageViewType))]
	[Serializable]
	public abstract class BasePagingType
	{
		[XmlAttribute]
		public int MaxEntriesReturned
		{
			get
			{
				return this.maxEntriesReturnedField;
			}
			set
			{
				this.maxEntriesReturnedField = value;
			}
		}

		[XmlIgnore]
		public bool MaxEntriesReturnedSpecified
		{
			get
			{
				return this.maxEntriesReturnedFieldSpecified;
			}
			set
			{
				this.maxEntriesReturnedFieldSpecified = value;
			}
		}

		private int maxEntriesReturnedField;

		private bool maxEntriesReturnedFieldSpecified;
	}
}
