using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class DeleteItemType : BaseRequestType
	{
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseItemIdType[] ItemIds
		{
			get
			{
				return this.itemIdsField;
			}
			set
			{
				this.itemIdsField = value;
			}
		}

		[XmlAttribute]
		public DisposalType DeleteType
		{
			get
			{
				return this.deleteTypeField;
			}
			set
			{
				this.deleteTypeField = value;
			}
		}

		[XmlAttribute]
		public CalendarItemCreateOrDeleteOperationType SendMeetingCancellations
		{
			get
			{
				return this.sendMeetingCancellationsField;
			}
			set
			{
				this.sendMeetingCancellationsField = value;
			}
		}

		[XmlIgnore]
		public bool SendMeetingCancellationsSpecified
		{
			get
			{
				return this.sendMeetingCancellationsFieldSpecified;
			}
			set
			{
				this.sendMeetingCancellationsFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public AffectedTaskOccurrencesType AffectedTaskOccurrences
		{
			get
			{
				return this.affectedTaskOccurrencesField;
			}
			set
			{
				this.affectedTaskOccurrencesField = value;
			}
		}

		[XmlIgnore]
		public bool AffectedTaskOccurrencesSpecified
		{
			get
			{
				return this.affectedTaskOccurrencesFieldSpecified;
			}
			set
			{
				this.affectedTaskOccurrencesFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool SuppressReadReceipts
		{
			get
			{
				return this.suppressReadReceiptsField;
			}
			set
			{
				this.suppressReadReceiptsField = value;
			}
		}

		[XmlIgnore]
		public bool SuppressReadReceiptsSpecified
		{
			get
			{
				return this.suppressReadReceiptsFieldSpecified;
			}
			set
			{
				this.suppressReadReceiptsFieldSpecified = value;
			}
		}

		private BaseItemIdType[] itemIdsField;

		private DisposalType deleteTypeField;

		private CalendarItemCreateOrDeleteOperationType sendMeetingCancellationsField;

		private bool sendMeetingCancellationsFieldSpecified;

		private AffectedTaskOccurrencesType affectedTaskOccurrencesField;

		private bool affectedTaskOccurrencesFieldSpecified;

		private bool suppressReadReceiptsField;

		private bool suppressReadReceiptsFieldSpecified;
	}
}
