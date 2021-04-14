using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class UpdateItemType : BaseRequestType
	{
		public TargetFolderIdType SavedItemFolderId
		{
			get
			{
				return this.savedItemFolderIdField;
			}
			set
			{
				this.savedItemFolderIdField = value;
			}
		}

		[XmlArrayItem("ItemChange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ItemChangeType[] ItemChanges
		{
			get
			{
				return this.itemChangesField;
			}
			set
			{
				this.itemChangesField = value;
			}
		}

		[XmlAttribute]
		public ConflictResolutionType ConflictResolution
		{
			get
			{
				return this.conflictResolutionField;
			}
			set
			{
				this.conflictResolutionField = value;
			}
		}

		[XmlAttribute]
		public MessageDispositionType MessageDisposition
		{
			get
			{
				return this.messageDispositionField;
			}
			set
			{
				this.messageDispositionField = value;
			}
		}

		[XmlIgnore]
		public bool MessageDispositionSpecified
		{
			get
			{
				return this.messageDispositionFieldSpecified;
			}
			set
			{
				this.messageDispositionFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public CalendarItemUpdateOperationType SendMeetingInvitationsOrCancellations
		{
			get
			{
				return this.sendMeetingInvitationsOrCancellationsField;
			}
			set
			{
				this.sendMeetingInvitationsOrCancellationsField = value;
			}
		}

		[XmlIgnore]
		public bool SendMeetingInvitationsOrCancellationsSpecified
		{
			get
			{
				return this.sendMeetingInvitationsOrCancellationsFieldSpecified;
			}
			set
			{
				this.sendMeetingInvitationsOrCancellationsFieldSpecified = value;
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

		private TargetFolderIdType savedItemFolderIdField;

		private ItemChangeType[] itemChangesField;

		private ConflictResolutionType conflictResolutionField;

		private MessageDispositionType messageDispositionField;

		private bool messageDispositionFieldSpecified;

		private CalendarItemUpdateOperationType sendMeetingInvitationsOrCancellationsField;

		private bool sendMeetingInvitationsOrCancellationsFieldSpecified;

		private bool suppressReadReceiptsField;

		private bool suppressReadReceiptsFieldSpecified;
	}
}
