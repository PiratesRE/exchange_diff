using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class MessageSentRepresentingTypeProperty : SmartPropertyDefinition
	{
		internal MessageSentRepresentingTypeProperty() : base("MessageSentRepresentingType", typeof(MessageSentRepresentingFlags), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, MessageSentRepresentingTypeProperty.PropertyDependencies)
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			byte[] array = propertyBag.GetValue(InternalSchema.CreatorEntryId) as byte[];
			byte[] array2 = propertyBag.GetValue(InternalSchema.SenderEntryId) as byte[];
			byte[] array3 = propertyBag.GetValue(InternalSchema.SentRepresentingEntryId) as byte[];
			if (array == null || array2 == null || array3 == null)
			{
				return MessageSentRepresentingFlags.None;
			}
			if (ArrayComparer<byte>.Comparer.Equals(array, array3))
			{
				return MessageSentRepresentingFlags.None;
			}
			if (ArrayComparer<byte>.Comparer.Equals(array3, array2))
			{
				return MessageSentRepresentingFlags.SendAs;
			}
			return MessageSentRepresentingFlags.SendOnBehalfOf;
		}

		private static readonly PropertyDependency[] PropertyDependencies = new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.CreatorEntryId, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.SenderEntryId, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.SentRepresentingEntryId, PropertyDependencyType.NeedForRead)
		};
	}
}
