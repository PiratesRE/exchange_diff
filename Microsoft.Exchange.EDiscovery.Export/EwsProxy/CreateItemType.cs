using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class CreateItemType : BaseRequestType
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

		public NonEmptyArrayOfAllItemsType Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
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
		public CalendarItemCreateOrDeleteOperationType SendMeetingInvitations
		{
			get
			{
				return this.sendMeetingInvitationsField;
			}
			set
			{
				this.sendMeetingInvitationsField = value;
			}
		}

		[XmlIgnore]
		public bool SendMeetingInvitationsSpecified
		{
			get
			{
				return this.sendMeetingInvitationsFieldSpecified;
			}
			set
			{
				this.sendMeetingInvitationsFieldSpecified = value;
			}
		}

		private TargetFolderIdType savedItemFolderIdField;

		private NonEmptyArrayOfAllItemsType itemsField;

		private MessageDispositionType messageDispositionField;

		private bool messageDispositionFieldSpecified;

		private CalendarItemCreateOrDeleteOperationType sendMeetingInvitationsField;

		private bool sendMeetingInvitationsFieldSpecified;
	}
}
