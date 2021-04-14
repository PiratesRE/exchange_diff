using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ConversationFamilyIdProperty : SmartPropertyDefinition
	{
		internal ConversationFamilyIdProperty() : base("ConversationFamilyId", typeof(ConversationId), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiConversationFamilyId, PropertyDependencyType.AllRead)
		})
		{
		}

		protected sealed override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (value == null)
			{
				propertyBag.Delete(InternalSchema.MapiConversationFamilyId);
				return;
			}
			ConversationId conversationId = value as ConversationId;
			if (conversationId == null)
			{
				throw new ArgumentException("value", "Must be null or ConversationId");
			}
			propertyBag.SetOrDeleteProperty(InternalSchema.MapiConversationFamilyId, conversationId.GetBytes());
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(InternalSchema.MapiConversationFamilyId);
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(InternalSchema.MapiConversationFamilyId);
			byte[] array = value as byte[];
			if (array != null)
			{
				object result;
				try
				{
					result = ConversationId.Create(array);
				}
				catch (CorruptDataException)
				{
					result = new PropertyError(this, PropertyErrorCode.CorruptedData);
				}
				return result;
			}
			PropertyError propertyError = (PropertyError)value;
			if (propertyError.PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
			{
				return new PropertyError(this, PropertyErrorCode.CorruptedData);
			}
			return new PropertyError(this, propertyError.PropertyErrorCode);
		}
	}
}
