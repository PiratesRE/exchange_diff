using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DelegateUserType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class DelegateUserType
	{
		public DelegateUserType()
		{
		}

		internal DelegateUserType(UserId userId)
		{
			this.userId = userId;
		}

		[XmlElement("UserId")]
		public UserId UserId
		{
			get
			{
				return this.userId;
			}
			set
			{
				this.userId = value;
			}
		}

		[XmlElement("DelegatePermissions")]
		public DelegatePermissionsType DelegatePermissions
		{
			get
			{
				return this.delegatePermissions;
			}
			set
			{
				this.delegatePermissions = value;
			}
		}

		[XmlElement("ReceiveCopiesOfMeetingMessages")]
		public bool? ReceiveCopiesOfMeetingMessages
		{
			get
			{
				return this.receiveCopiesOfMeetingMessages;
			}
			set
			{
				this.receiveCopiesOfMeetingMessages = value;
			}
		}

		[XmlElement("ViewPrivateItems")]
		public bool? ViewPrivateItems
		{
			get
			{
				return this.viewPrivateItems;
			}
			set
			{
				this.viewPrivateItems = value;
			}
		}

		private UserId userId;

		private DelegatePermissionsType delegatePermissions;

		private bool? receiveCopiesOfMeetingMessages;

		private bool? viewPrivateItems;
	}
}
