using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class UpdateItemType : BaseRequestType
	{
		public TargetFolderIdType SavedItemFolderId;

		[XmlArrayItem("ItemChange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ItemChangeType[] ItemChanges;

		[XmlAttribute]
		public ConflictResolutionType ConflictResolution;

		[XmlAttribute]
		public MessageDispositionType MessageDisposition;

		[XmlIgnore]
		public bool MessageDispositionSpecified;

		[XmlAttribute]
		public CalendarItemUpdateOperationType SendMeetingInvitationsOrCancellations;

		[XmlIgnore]
		public bool SendMeetingInvitationsOrCancellationsSpecified;

		[XmlAttribute]
		public bool SuppressReadReceipts;

		[XmlIgnore]
		public bool SuppressReadReceiptsSpecified;
	}
}
