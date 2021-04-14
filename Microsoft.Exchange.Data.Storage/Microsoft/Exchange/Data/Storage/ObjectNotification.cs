using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ObjectNotification : Notification
	{
		internal ObjectNotification(StoreObjectId notifyingItemId, StoreObjectId parentFolderId, StoreObjectId previousId, StoreObjectId previousParentFolderId, NotificationObjectType objectType, UnresolvedPropertyDefinition[] propertyDefinitions, NotificationType type) : base(type)
		{
			this.notifyingItemId = notifyingItemId;
			this.parentFolderId = parentFolderId;
			this.previousId = previousId;
			this.previousParentFolderId = previousParentFolderId;
			this.objectType = objectType;
			this.propertyDefinitions = propertyDefinitions;
		}

		public StoreObjectId NotifyingItemId
		{
			get
			{
				return this.notifyingItemId;
			}
		}

		public StoreObjectId ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
		}

		public StoreObjectId PreviousId
		{
			get
			{
				return this.previousId;
			}
		}

		public StoreObjectId PreviousParentFolderId
		{
			get
			{
				return this.previousParentFolderId;
			}
		}

		public NotificationObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		public UnresolvedPropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return this.propertyDefinitions;
			}
		}

		private readonly StoreObjectId notifyingItemId;

		private readonly StoreObjectId parentFolderId;

		private readonly StoreObjectId previousId;

		private readonly StoreObjectId previousParentFolderId;

		private readonly NotificationObjectType objectType;

		private readonly UnresolvedPropertyDefinition[] propertyDefinitions;
	}
}
