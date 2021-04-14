using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Contacts.ChangeLogger
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactEmailChangeLogger : IContactChangeTracker
	{
		public string Name
		{
			get
			{
				return "EmailCorruptionTracker";
			}
		}

		public bool ShouldLoadPropertiesForFurtherCheck(COWTriggerAction operation, string itemClass, StoreObjectId itemId, CoreItem item)
		{
			if (!ObjectClass.IsContact(itemClass))
			{
				return false;
			}
			switch (operation)
			{
			case COWTriggerAction.Create:
			case COWTriggerAction.Update:
			case COWTriggerAction.Copy:
				return Array.Exists<StorePropertyDefinition>(ContactEmailChangeLogger.TriggerProperties, (StorePropertyDefinition property) => item.PropertyBag.IsPropertyDirty(property));
			}
			return false;
		}

		public StorePropertyDefinition[] GetProperties(StoreObjectId itemId, CoreItem item)
		{
			return ContactEmailChangeLogger.TriggerProperties;
		}

		public bool ShouldLogContact(StoreObjectId itemId, CoreItem item)
		{
			return ContactEmailChangeLogger.DetectEmailCorruption(item);
		}

		public bool ShouldLogGroupOperation(COWTriggerAction operation, StoreSession sourceSession, StoreObjectId sourceFolderId, StoreSession destinationSession, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds)
		{
			return false;
		}

		private static bool DetectEmailCorruption(CoreItem item)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (StorePropertyDefinition propertyDefinition in ContactEmailChangeLogger.TriggerProperties)
			{
				string valueOrDefault = item.PropertyBag.GetValueOrDefault<string>(propertyDefinition, string.Empty);
				if (valueOrDefault.EndsWith("microsoft.com", StringComparison.OrdinalIgnoreCase))
				{
					int num = valueOrDefault.IndexOf('@');
					if (num != -1)
					{
						string text = valueOrDefault.Substring(0, num);
						text = text.Trim().ToLowerInvariant();
						hashSet.Add(text);
					}
				}
			}
			return hashSet.Count > 2;
		}

		private static readonly StorePropertyDefinition[] TriggerProperties = new StorePropertyDefinition[]
		{
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email3EmailAddress,
			ContactSchema.IMAddress,
			ContactSchema.IMAddress2,
			ContactSchema.IMAddress3
		};
	}
}
