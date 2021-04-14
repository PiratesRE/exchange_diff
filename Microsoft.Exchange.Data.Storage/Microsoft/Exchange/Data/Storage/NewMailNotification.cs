using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NewMailNotification : Notification
	{
		internal NewMailNotification(StoreObjectId newMailItemId, StoreObjectId parentFolderId, string messageClass, MessageFlags messageFlags) : base(NotificationType.NewMail)
		{
			this.newMailItemId = newMailItemId;
			this.parentFolderId = parentFolderId;
			this.messageClass = messageClass;
			this.messageFlags = messageFlags;
		}

		public StoreObjectId NewMailItemId
		{
			get
			{
				return this.newMailItemId;
			}
		}

		public StoreObjectId ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		public MessageFlags MessageFlags
		{
			get
			{
				return this.messageFlags;
			}
		}

		private readonly StoreObjectId newMailItemId;

		private readonly StoreObjectId parentFolderId;

		private readonly string messageClass;

		private readonly MessageFlags messageFlags;
	}
}
