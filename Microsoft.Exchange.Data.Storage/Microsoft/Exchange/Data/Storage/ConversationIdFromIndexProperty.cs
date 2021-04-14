using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ConversationIdFromIndexProperty : SmartPropertyDefinition
	{
		internal ConversationIdFromIndexProperty() : base("ConversationId", typeof(ConversationId), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.ConversationTopic, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ConversationIndexTracking, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ConversationIndex, PropertyDependencyType.AllRead)
		})
		{
		}

		protected sealed override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			ConversationId conversationId = value as ConversationId;
			if (conversationId == null)
			{
				throw new ArgumentException("value", "Must be a non-null ConversationId instance");
			}
			ConversationIndex conversationIndex;
			if (!ConversationIndex.TryCreate(propertyBag.GetValueOrDefault<byte[]>(InternalSchema.ConversationIndex), out conversationIndex))
			{
				conversationIndex = ConversationIndex.Create(conversationId);
			}
			else
			{
				conversationIndex = conversationIndex.UpdateGuid(new Guid(conversationId.GetBytes()));
			}
			byte[] array = conversationIndex.ToByteArray();
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (valueOrDefault != null && ObjectClass.IsOfClass(valueOrDefault, "IPM.ConversationAction"))
			{
				array[0] = 1;
				array[1] = (array[2] = (array[3] = (array[4] = (array[5] = 0))));
			}
			propertyBag.SetValueWithFixup(InternalSchema.ConversationIndex, array);
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.ConversationIndex);
			if (valueOrDefault == null)
			{
				return null;
			}
			ConversationIndex conversationIndex;
			if (!ConversationIndex.TryCreate(valueOrDefault, out conversationIndex))
			{
				return null;
			}
			bool? valueAsNullable = propertyBag.GetValueAsNullable<bool>(InternalSchema.ConversationIndexTracking);
			if (valueAsNullable == null || !valueAsNullable.Value)
			{
				string topic = propertyBag.GetValueOrDefault<string>(InternalSchema.ConversationTopic) ?? string.Empty;
				byte[] bytes = this.ComputeHashTopic(topic);
				return ConversationId.Create(bytes);
			}
			return ConversationId.Create(conversationIndex.Guid);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return this.NativeFilterToConversationIdBasedSmartFilter(filter, InternalSchema.MapiConversationId);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return this.ConversationIdBasedSmartFilterToNativeFilter(filter, InternalSchema.MapiConversationId);
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return InternalSchema.MapiConversationId;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		internal byte[] ComputeHashTopic(string topic)
		{
			byte[] result;
			using (Md5Hasher md5Hasher = new Md5Hasher())
			{
				byte[] bytes = Encoding.Unicode.GetBytes(topic.ToUpper());
				byte[] array = md5Hasher.ComputeHash(bytes);
				result = array;
			}
			return result;
		}

		internal static bool CheckExclusionList(string itemClass)
		{
			if (!string.IsNullOrEmpty(itemClass))
			{
				if (ObjectClass.IsOfClass(itemClass, "IPM.Activity"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.Appointment"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.Contact"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.ContentClassDef"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.DistList"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.Document"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.Microsoft.ScheduleData.FreeBusy"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.InfoPathForm"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.InkNotes"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.StickyNote"))
				{
					return true;
				}
				if (ObjectClass.IsOfClass(itemClass, "IPM.AuditLog"))
				{
					return true;
				}
			}
			return false;
		}
	}
}
