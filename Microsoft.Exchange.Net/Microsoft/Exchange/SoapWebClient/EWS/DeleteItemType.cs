﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class DeleteItemType : BaseRequestType
	{
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseItemIdType[] ItemIds;

		[XmlAttribute]
		public DisposalType DeleteType;

		[XmlAttribute]
		public CalendarItemCreateOrDeleteOperationType SendMeetingCancellations;

		[XmlIgnore]
		public bool SendMeetingCancellationsSpecified;

		[XmlAttribute]
		public AffectedTaskOccurrencesType AffectedTaskOccurrences;

		[XmlIgnore]
		public bool AffectedTaskOccurrencesSpecified;

		[XmlAttribute]
		public bool SuppressReadReceipts;

		[XmlIgnore]
		public bool SuppressReadReceiptsSpecified;
	}
}
