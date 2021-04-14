using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Contacts.ChangeLogger
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactTracker : IContactChangeTracker
	{
		public string Name
		{
			get
			{
				return "ContactTracker";
			}
		}

		public bool ShouldLoadPropertiesForFurtherCheck(COWTriggerAction operation, string itemClass, StoreObjectId itemId, CoreItem item)
		{
			if (ObjectClass.IsContact(itemClass))
			{
				return true;
			}
			ContactTracker.Tracer.TraceDebug<string, StoreObjectId>((long)this.GetHashCode(), "ContactTracker.ShouldLoadPropertiesForFurtherCheck: Skipping Item with Class - {0}, Id - {1}", itemClass, itemId);
			return false;
		}

		public StorePropertyDefinition[] GetProperties(StoreObjectId itemId, CoreItem item)
		{
			List<StorePropertyDefinition> list = new List<StorePropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in ContactSchema.Instance.InternalAllProperties)
			{
				if (!item.PropertyBag.IsPropertyDirty(propertyDefinition))
				{
					ContactTracker.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ContactTracker.ShouldLogContact: Skipping property as it is not dirty: {0}", propertyDefinition.Name);
				}
				else
				{
					StorePropertyDefinition storePropertyDefinition = propertyDefinition as StorePropertyDefinition;
					if (storePropertyDefinition == null)
					{
						ContactTracker.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ContactTracker.ShouldLogContact: Skipping property as it is not a StorePropertyDefinition: {0}", propertyDefinition.Name);
					}
					else
					{
						list.Add(storePropertyDefinition);
					}
				}
			}
			return list.ToArray();
		}

		public bool ShouldLogContact(StoreObjectId itemId, CoreItem item)
		{
			return true;
		}

		public bool ShouldLogGroupOperation(COWTriggerAction operation, StoreSession sourceSession, StoreObjectId sourceFolderId, StoreSession destinationSession, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds)
		{
			if (sourceSession != null && sourceFolderId != null && (this.IsDefaultFolder(sourceSession, sourceFolderId, DefaultFolderType.Conflicts) || this.IsDefaultFolder(sourceSession, sourceFolderId, DefaultFolderType.SyncIssues)))
			{
				ContactTracker.Tracer.TraceDebug((long)this.GetHashCode(), "ContactTracker.ShouldLogGroupOperation: SourceFolder is a Conflicts/SyncIssues folder.");
				return false;
			}
			if (destinationSession != null && destinationFolderId != null && (this.IsDefaultFolder(destinationSession, destinationFolderId, DefaultFolderType.Conflicts) || this.IsDefaultFolder(destinationSession, destinationFolderId, DefaultFolderType.SyncIssues)))
			{
				ContactTracker.Tracer.TraceDebug((long)this.GetHashCode(), "ContactTracker.ShouldLogGroupOperation: DestinationFolder is a Conflicts/SyncIssues folder.");
				return false;
			}
			ContactTracker.Tracer.TraceDebug<COWTriggerAction>((long)this.GetHashCode(), "ContactTracker.ShouldLogGroupOperation: Invoked for operation: {0}", operation);
			switch (operation)
			{
			case COWTriggerAction.Create:
			case COWTriggerAction.Update:
			case COWTriggerAction.Copy:
			case COWTriggerAction.Move:
			case COWTriggerAction.MoveToDeletedItems:
			case COWTriggerAction.SoftDelete:
			case COWTriggerAction.HardDelete:
				return true;
			}
			return false;
		}

		private bool IsDefaultFolder(StoreSession session, StoreObjectId folderId, DefaultFolderType defaultFolder)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				ContactTracker.Tracer.TraceDebug<StoreSession>((long)this.GetHashCode(), "ContactTracker.IsDefaultFolder: Invoked for session that is not a mailbox session: {0}", session);
				return false;
			}
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(defaultFolder);
			bool flag = object.Equals(defaultFolder, folderId);
			ContactTracker.Tracer.TraceDebug((long)this.GetHashCode(), "ContactTracker.IsDefaultFolder: Given folderId {0}, DefaultFolderType {1}'s Id {2} are same: {3}", new object[]
			{
				folderId,
				defaultFolder,
				defaultFolderId,
				flag
			});
			return flag;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ContactChangeLoggingTracer;
	}
}
