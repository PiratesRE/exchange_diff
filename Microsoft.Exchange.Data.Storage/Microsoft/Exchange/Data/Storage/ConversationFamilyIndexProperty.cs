using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ConversationFamilyIndexProperty : SmartPropertyDefinition
	{
		internal ConversationFamilyIndexProperty() : base("ConversationFamilyIndexProperty", typeof(byte[]), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiConversationFamilyId, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ConversationIndex, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.MapiConversationFamilyId, null);
			byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.ConversationIndex);
			if (valueOrDefault2 == null || valueOrDefault == null)
			{
				return null;
			}
			ConversationIndex conversationIndex;
			bool flag = ConversationIndex.TryCreate(valueOrDefault2, out conversationIndex);
			ConversationId conversationId = ConversationId.Create(valueOrDefault);
			if (!flag)
			{
				return null;
			}
			return conversationIndex.UpdateGuid(conversationId).ToByteArray();
		}
	}
}
