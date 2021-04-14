using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActivitySchema : Schema
	{
		static ActivitySchema()
		{
			Dictionary<string, PropertyDefinition> dictionary = new Dictionary<string, PropertyDefinition>(ActivitySchema.hookablePropertyCollection.Value.Count);
			foreach (PropertyDefinition propertyDefinition in ActivitySchema.hookablePropertyCollection.Value)
			{
				dictionary.Add(propertyDefinition.Name, propertyDefinition);
			}
			ActivitySchema.hookablePropertyNameToPropertyDefinitionMapping = Hookable<ReadOnlyDictionary<string, PropertyDefinition>>.Create(true, new ReadOnlyDictionary<string, PropertyDefinition>(dictionary));
		}

		public new static ActivitySchema Instance
		{
			get
			{
				return ActivitySchema.instance;
			}
		}

		public static Hookable<ReadOnlyCollection<PropertyDefinition>> HookablePropertyCollection
		{
			get
			{
				return ActivitySchema.hookablePropertyCollection;
			}
		}

		public static Hookable<ReadOnlyDictionary<string, PropertyDefinition>> HookablePropertyNameToPropertyDefinitionMapping
		{
			get
			{
				return ActivitySchema.hookablePropertyNameToPropertyDefinitionMapping;
			}
		}

		public static ReadOnlyCollection<PropertyDefinition> PropertyCollection
		{
			get
			{
				return ActivitySchema.hookablePropertyCollection.Value;
			}
		}

		public static ReadOnlyDictionary<string, PropertyDefinition> PropertyNameToPropertyDefinitionMapping
		{
			get
			{
				return ActivitySchema.hookablePropertyNameToPropertyDefinitionMapping.Value;
			}
		}

		public static readonly StorePropertyDefinition ActivityId = InternalSchema.ActivityId;

		public static readonly StorePropertyDefinition ClientId = InternalSchema.ActivityClientId;

		public static readonly StorePropertyDefinition ItemId = InternalSchema.ActivityItemId;

		public static readonly StorePropertyDefinition TimeStamp = InternalSchema.ActivityTimeStamp;

		public static readonly StorePropertyDefinition SessionId = InternalSchema.ActivitySessionId;

		public static readonly StorePropertyDefinition LocaleId = InternalSchema.ActivityLocaleId;

		public static readonly StorePropertyDefinition PreviousItemId = InternalSchema.ActivityGlobalObjectIdBytes;

		private static readonly StorePropertyDefinition DeleteType = InternalSchema.ActivityDeleteType;

		private static readonly StorePropertyDefinition WindowId = InternalSchema.ActivityWindowId;

		private static readonly StorePropertyDefinition FolderId = InternalSchema.ActivityFolderId;

		private static readonly StorePropertyDefinition OofEnabled = InternalSchema.ActivityOofEnabled;

		private static readonly StorePropertyDefinition Browser = InternalSchema.ActivityBrowser;

		private static readonly StorePropertyDefinition Location = InternalSchema.ActivityLocation;

		private static readonly StorePropertyDefinition ConversationId = InternalSchema.ActivityConversationId;

		private static readonly StorePropertyDefinition IpAddress = InternalSchema.ActivityIpAddress;

		private static readonly StorePropertyDefinition TimeZone = InternalSchema.ActivityTimeZone;

		private static readonly StorePropertyDefinition Category = InternalSchema.ActivityCategory;

		public static readonly StorePropertyDefinition CustomProperties = InternalSchema.ActivityAttachmentIdBytes;

		private static readonly StorePropertyDefinition ModuleSelected = InternalSchema.ActivityModuleSelected;

		private static readonly StorePropertyDefinition LayoutType = InternalSchema.ActivityLayoutType;

		private static ActivitySchema instance = new ActivitySchema();

		private static readonly Hookable<ReadOnlyCollection<PropertyDefinition>> hookablePropertyCollection = Hookable<ReadOnlyCollection<PropertyDefinition>>.Create(true, new ReadOnlyCollection<PropertyDefinition>(ActivitySchema.instance.AllProperties.ToList<PropertyDefinition>()));

		private static readonly Hookable<ReadOnlyDictionary<string, PropertyDefinition>> hookablePropertyNameToPropertyDefinitionMapping;
	}
}
