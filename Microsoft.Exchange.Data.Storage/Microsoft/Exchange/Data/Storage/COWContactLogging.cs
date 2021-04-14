using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Storage.Contacts.ChangeLogger;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class COWContactLogging : ICOWNotification
	{
		internal COWContactLogging()
		{
		}

		public bool SkipItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, bool onBeforeNotification, bool onDumpster, bool success, CallbackContext callbackContext)
		{
			if (!COWContactLogging.COWContactLoggingConfiguration.Instance.IsLoggingEnabled())
			{
				return true;
			}
			Util.ThrowOnNullArgument(session, "session");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			if (item == null)
			{
				COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.SkipItemOperation: Item is null");
				return true;
			}
			if (!onBeforeNotification)
			{
				COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.SkipItemOperation: Not onBeforeNotification");
				return true;
			}
			string valueOrDefault = item.PropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
			COWContactLogging.Tracer.TraceDebug<string>((long)this.GetHashCode(), "COWContactLogging.SkipItemOperation: ItemClass: {0}", valueOrDefault);
			if (ObjectClass.IsPlace(valueOrDefault))
			{
				return true;
			}
			if (!ObjectClass.IsContact(valueOrDefault) && !ObjectClass.IsDistributionList(valueOrDefault) && !ObjectClass.IsContactsFolder(valueOrDefault))
			{
				return true;
			}
			foreach (IContactChangeTracker contactChangeTracker in COWContactLogging.ChangeTrackers)
			{
				if (contactChangeTracker.ShouldLoadPropertiesForFurtherCheck(operation, valueOrDefault, itemId, item))
				{
					COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.SkipItemOperation: A tracker interested.");
					return false;
				}
			}
			return true;
		}

		public void ItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, CoreFolder folder, bool onBeforeNotification, OperationResult result, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(item, "item");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			EnumValidator.ThrowIfInvalid<OperationResult>(result, "result");
			COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.ItemOperation: Start.");
			string valueOrDefault = item.PropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
			COWContactLogging.Tracer.TraceDebug<string>((long)this.GetHashCode(), "COWContactLogging.ItemOperation: ItemClass: {0}", valueOrDefault);
			HashSet<StorePropertyDefinition> hashSet = new HashSet<StorePropertyDefinition>();
			List<IContactChangeTracker> list = new List<IContactChangeTracker>(COWContactLogging.ChangeTrackers.Length);
			foreach (IContactChangeTracker contactChangeTracker in COWContactLogging.ChangeTrackers)
			{
				if (contactChangeTracker.ShouldLoadPropertiesForFurtherCheck(operation, valueOrDefault, itemId, item))
				{
					COWContactLogging.Tracer.TraceDebug<string>((long)this.GetHashCode(), "COWContactLogging.ItemOperation: Tracker {0} interested.", contactChangeTracker.Name);
					list.Add(contactChangeTracker);
					StorePropertyDefinition[] properties = contactChangeTracker.GetProperties(itemId, item);
					if (properties != null && properties.Length > 0)
					{
						COWContactLogging.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "COWContactLogging.ItemOperation: Tracker {0} returned {1} properties for tracking", contactChangeTracker.Name, properties.Length);
						hashSet.UnionWith(properties);
					}
				}
			}
			if (hashSet.Count == 0)
			{
				COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.ItemOperation: No properties marked as interesting by any tracker.");
				return;
			}
			hashSet.Add(ItemSchema.Id);
			hashSet.Add(StoreObjectSchema.ItemClass);
			StorePropertyDefinition[] array = new StorePropertyDefinition[hashSet.Count];
			hashSet.CopyTo(array, 0, hashSet.Count);
			item.PropertyBag.Load(array);
			bool flag = false;
			foreach (IContactChangeTracker contactChangeTracker2 in list)
			{
				if (contactChangeTracker2.ShouldLogContact(itemId, item))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.ItemOperation: No trackers are interested after doing further checks.");
				return;
			}
			COWContactLogging.ContactChangeLogEvent contactChangeLogEvent = new COWContactLogging.ContactChangeLogEvent();
			contactChangeLogEvent.Add("ClientInfo", session.ClientInfoString);
			contactChangeLogEvent.Add("Action", operation.ToString());
			COWContactLogging.Tracer.TraceDebug<COWTriggerAction, string, int>((long)this.GetHashCode(), "COWContactLogging.ItemOperation: Tracking change {0} made by client {1} across a total of {2} properties", operation, session.ClientInfoString, hashSet.Count);
			this.LogInterestingPropertyValues(contactChangeLogEvent, operation, item, hashSet);
			ContactChangeLogger contactChangeLogger = new ContactChangeLogger(session);
			contactChangeLogger.LogEvent(contactChangeLogEvent);
		}

		public CowClientOperationSensitivity SkipGroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, bool onBeforeNotification, bool onDumpster, CallbackContext callbackContext)
		{
			if (!COWContactLogging.COWContactLoggingConfiguration.Instance.IsLoggingEnabled())
			{
				return CowClientOperationSensitivity.Skip;
			}
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			Util.ThrowOnNullArgument(sourceSession, "sourceSession");
			if (!onBeforeNotification)
			{
				COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.SkipGroupOperation: not OnBeforeNotification");
				return CowClientOperationSensitivity.Skip;
			}
			COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.SkipGroupOperation: SourceFolderId.ObjectType={0}, DestinationFolderId.ObjectType={1}", new object[]
			{
				(sourceFolderId == null) ? "<<Null>>" : sourceFolderId.ObjectType,
				(destinationFolderId == null) ? "<<Null>>" : destinationFolderId.ObjectType
			});
			bool flag = true;
			if ((sourceFolderId != null && sourceFolderId.ObjectType == StoreObjectType.ContactsFolder) || (destinationFolderId != null && destinationFolderId.ObjectType == StoreObjectType.ContactsFolder))
			{
				flag = false;
			}
			else if (itemIds != null)
			{
				foreach (StoreObjectId storeObjectId in itemIds)
				{
					COWContactLogging.Tracer.TraceDebug<StoreObjectType>((long)this.GetHashCode(), "COWContactLogging.SkipGroupOperation: ItemId.ObjectType = {0}", storeObjectId.ObjectType);
					if (storeObjectId.ObjectType == StoreObjectType.Contact || storeObjectId.ObjectType == StoreObjectType.DistributionList)
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				foreach (IContactChangeTracker contactChangeTracker in COWContactLogging.ChangeTrackers)
				{
					if (contactChangeTracker.ShouldLogGroupOperation(operation, sourceSession, sourceFolderId, destinationSession, destinationFolderId, itemIds))
					{
						COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.SkipGroupOperation: A tracker interested.");
						COWContactLogging.ContactChangeLogEvent contactChangeLogEvent = new COWContactLogging.ContactChangeLogEvent();
						contactChangeLogEvent.Add("Action", operation.ToString());
						contactChangeLogEvent.Add("FolderChangeOperationFlags", flags.ToString());
						contactChangeLogEvent.Add("ClientInfo", sourceSession.ClientInfoString);
						if (destinationSession != null && destinationSession.ClientInfoString != sourceSession.ClientInfoString)
						{
							contactChangeLogEvent.Add("DestinationClientInfo", destinationSession.ClientInfoString);
						}
						if (sourceFolderId != null)
						{
							contactChangeLogEvent.Add("FolderId", sourceFolderId.ToString());
						}
						if (destinationFolderId != null && !object.Equals(sourceFolderId, destinationFolderId))
						{
							contactChangeLogEvent.Add("DestinationFolderId", destinationFolderId.ToString());
						}
						if (destinationSession != null && !object.Equals(sourceSession.MailboxGuid, destinationSession.MailboxGuid))
						{
							contactChangeLogEvent.Add("DestinationMailboxGuid", destinationSession.MailboxGuid.ToString());
						}
						string value = this.SanitizePropertyValueForLogging(itemIds);
						contactChangeLogEvent.Add("ItemIds", value);
						ContactChangeLogger contactChangeLogger = new ContactChangeLogger(sourceSession);
						contactChangeLogger.LogEvent(contactChangeLogEvent);
						break;
					}
				}
			}
			return CowClientOperationSensitivity.Skip;
		}

		public void GroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, bool onBeforeNotification, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
		}

		private void LogInterestingPropertyValues(COWContactLogging.ContactChangeLogEvent loggingEvent, COWTriggerAction operation, CoreItem item, IEnumerable<StorePropertyDefinition> allUniqueInterestingProperties)
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			foreach (StorePropertyDefinition storePropertyDefinition in allUniqueInterestingProperties)
			{
				stringBuilder.Clear();
				object obj = this.GetPropertyValueForLogging(item, storePropertyDefinition);
				obj = this.SanitizePropertyValueForLogging(obj);
				stringBuilder.Append("[New=");
				stringBuilder.Append(obj);
				bool flag = item.PropertyBag.IsPropertyDirty(storePropertyDefinition);
				if (flag && operation != COWTriggerAction.Create)
				{
					object obj2 = this.GetOriginalPropertyValueForLogging(item, storePropertyDefinition);
					obj2 = this.SanitizePropertyValueForLogging(obj2);
					if (obj2 != obj)
					{
						stringBuilder.Append(",Old=");
						stringBuilder.Append(obj2);
					}
				}
				stringBuilder.Append("]");
				string key = SpecialCharacters.SanitizeForLogging(storePropertyDefinition.Name);
				loggingEvent.Add(key, stringBuilder.ToString());
			}
		}

		private object GetPropertyValueForLogging(CoreItem item, StorePropertyDefinition interestingProperty)
		{
			if ((interestingProperty.PropertyFlags & PropertyFlags.Streamable) == PropertyFlags.Streamable)
			{
				COWContactLogging.Tracer.TraceDebug<string>((long)this.GetHashCode(), "COWContactLogging.GetPropertyValueForLogging: Skipping retrieval of value for streamable property {0}", interestingProperty.Name);
				return null;
			}
			return item.PropertyBag.GetValueOrDefault<object>(interestingProperty, null);
		}

		private object GetOriginalPropertyValueForLogging(CoreItem item, StorePropertyDefinition interestingProperty)
		{
			if ((interestingProperty.PropertyFlags & PropertyFlags.Streamable) == PropertyFlags.Streamable)
			{
				COWContactLogging.Tracer.TraceDebug<string>((long)this.GetHashCode(), "COWContactLogging.GetOriginalPropertyValueForLogging: Skipping retrieval of value for streamable property {0}", interestingProperty.Name);
				return null;
			}
			IValidatablePropertyBag validatablePropertyBag = item.PropertyBag as IValidatablePropertyBag;
			if (validatablePropertyBag == null)
			{
				COWContactLogging.Tracer.TraceDebug((long)this.GetHashCode(), "COWContactLogging.GetOriginalPropertyValueForLogging: Skipping retrieval of value as property bag doesn't track original values.");
				return null;
			}
			PropertyValueTrackingData originalPropertyInformation = validatablePropertyBag.GetOriginalPropertyInformation(interestingProperty);
			return originalPropertyInformation.OriginalPropertyValue;
		}

		private string SanitizePropertyValueForLogging(object propertyValue)
		{
			if (propertyValue == null)
			{
				return string.Empty;
			}
			return ContactLinkingStrings.GetValueString(propertyValue);
		}

		private static readonly Trace Tracer = ExTraceGlobals.ContactChangeLoggingTracer;

		private static readonly IContactChangeTracker[] ChangeTrackers = new IContactChangeTracker[]
		{
			new ContactEmailChangeLogger(),
			new UcsTracker(),
			new ContactTracker()
		};

		private sealed class ContactChangeLogEvent : ILogEvent
		{
			public ContactChangeLogEvent()
			{
				this.data = new Dictionary<string, object>(StringComparer.Ordinal);
			}

			public string EventId
			{
				get
				{
					return "ChangeTracker";
				}
			}

			public ICollection<KeyValuePair<string, object>> GetEventData()
			{
				return this.data;
			}

			public void Add(string key, string value)
			{
				this.data.Add(key, value);
			}

			private readonly Dictionary<string, object> data;
		}

		private sealed class COWContactLoggingConfiguration
		{
			private COWContactLoggingConfiguration()
			{
			}

			public static COWContactLogging.COWContactLoggingConfiguration Instance
			{
				get
				{
					return COWContactLogging.COWContactLoggingConfiguration.instance;
				}
			}

			public bool IsLoggingEnabled()
			{
				if (this.loggingEnabled == null)
				{
					this.loggingEnabled = new int?(Util.GetRegistryValueOrDefault(COWContactLogging.COWContactLoggingConfiguration.RegistryKeysLocation, COWContactLogging.COWContactLoggingConfiguration.EnableCOWContactLogging, 1, COWContactLogging.Tracer));
				}
				return this.loggingEnabled.Value == 1;
			}

			private const int LoggingEnabledValue = 1;

			private static readonly string RegistryKeysLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\People";

			private static readonly string EnableCOWContactLogging = "EnableCOWContactLogging";

			private static COWContactLogging.COWContactLoggingConfiguration instance = new COWContactLogging.COWContactLoggingConfiguration();

			private int? loggingEnabled;
		}
	}
}
