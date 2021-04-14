using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class DelegateUserType
	{
		public UserIdType UserId
		{
			get
			{
				return this.userIdField;
			}
			set
			{
				this.userIdField = value;
			}
		}

		public DelegatePermissionsType DelegatePermissions
		{
			get
			{
				return this.delegatePermissionsField;
			}
			set
			{
				this.delegatePermissionsField = value;
			}
		}

		public bool ReceiveCopiesOfMeetingMessages
		{
			get
			{
				return this.receiveCopiesOfMeetingMessagesField;
			}
			set
			{
				this.receiveCopiesOfMeetingMessagesField = value;
			}
		}

		[XmlIgnore]
		public bool ReceiveCopiesOfMeetingMessagesSpecified
		{
			get
			{
				return this.receiveCopiesOfMeetingMessagesFieldSpecified;
			}
			set
			{
				this.receiveCopiesOfMeetingMessagesFieldSpecified = value;
			}
		}

		public bool ViewPrivateItems
		{
			get
			{
				return this.viewPrivateItemsField;
			}
			set
			{
				this.viewPrivateItemsField = value;
			}
		}

		[XmlIgnore]
		public bool ViewPrivateItemsSpecified
		{
			get
			{
				return this.viewPrivateItemsFieldSpecified;
			}
			set
			{
				this.viewPrivateItemsFieldSpecified = value;
			}
		}

		private UserIdType userIdField;

		private DelegatePermissionsType delegatePermissionsField;

		private bool receiveCopiesOfMeetingMessagesField;

		private bool receiveCopiesOfMeetingMessagesFieldSpecified;

		private bool viewPrivateItemsField;

		private bool viewPrivateItemsFieldSpecified;
	}
}
