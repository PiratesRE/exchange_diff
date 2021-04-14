using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class LinkRejectHistoryProperty : SmartPropertyDefinition
	{
		internal LinkRejectHistoryProperty() : base("LinkRejectHistory", typeof(PersonId[]), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.LinkRejectHistoryRaw, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(InternalSchema.LinkRejectHistoryRaw);
			byte[][] array = value as byte[][];
			if (array != null)
			{
				try
				{
					PersonId[] array2 = new PersonId[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = PersonId.Create(array[i]);
					}
					return array2;
				}
				catch (ArgumentException)
				{
				}
				return new PropertyError(this, PropertyErrorCode.CorruptedData);
			}
			if (value is PropertyError)
			{
				return value;
			}
			return new PropertyError(this, PropertyErrorCode.CorruptedData);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			PersonId[] array = value as PersonId[];
			if (array == null)
			{
				throw new ArgumentException("value");
			}
			byte[][] array2 = new byte[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].GetBytes();
			}
			propertyBag.SetValue(InternalSchema.LinkRejectHistoryRaw, array2);
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(InternalSchema.LinkRejectHistoryRaw);
		}
	}
}
