using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class LikeCountProperty : SmartPropertyDefinition
	{
		internal LikeCountProperty() : base("LikersCount", typeof(int), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiLikeCount, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			MessageItem messageItem = propertyBag.Context.StoreObject as MessageItem;
			if (messageItem == null)
			{
				return propertyBag.GetValue(InternalSchema.MapiLikeCount);
			}
			int count = ((Likers)messageItem.Likers).Count;
			if (count == 0)
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return count;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			propertyBag.SetValueWithFixup(InternalSchema.MapiLikeCount, value);
		}
	}
}
