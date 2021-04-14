using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Likers : ParticipantList
	{
		internal Likers(PropertyBag propertyBag) : this(propertyBag, true)
		{
		}

		internal Likers(PropertyBag propertyBag, bool suppressCorruptDataException) : base(propertyBag, InternalSchema.MapiLikersBlob, null, InternalSchema.MapiLikeCount, suppressCorruptDataException)
		{
		}

		internal static Likers CreateInstance(IDictionary<PropertyDefinition, object> propertyBag)
		{
			object obj;
			if (propertyBag.TryGetValue(InternalSchema.LikersBlob, out obj))
			{
				byte[] array = obj as byte[];
				if (array != null)
				{
					PropertyBag propertyBag2 = new MemoryPropertyBag();
					propertyBag2[InternalSchema.MapiLikersBlob] = array;
					return new Likers(propertyBag2);
				}
			}
			return null;
		}

		public static readonly PropertyDefinition[] RequiredProperties = new PropertyDefinition[]
		{
			MessageItemSchema.LikeCount,
			MessageItemSchema.LikersBlob
		};
	}
}
