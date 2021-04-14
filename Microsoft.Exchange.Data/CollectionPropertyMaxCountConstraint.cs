using System;
using System.Collections;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class CollectionPropertyMaxCountConstraint : CollectionPropertyDefinitionConstraint
	{
		public CollectionPropertyMaxCountConstraint(int maxCount)
		{
			if (maxCount < 0)
			{
				throw new ArgumentOutOfRangeException("maxCount");
			}
			this.maxCount = maxCount;
		}

		public override PropertyConstraintViolationError Validate(IEnumerable value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			ICollection collection = value as ICollection;
			int num = 0;
			if (collection != null)
			{
				num = collection.Count;
			}
			else
			{
				IEnumerator enumerator = value.GetEnumerator();
				while (enumerator.MoveNext())
				{
					num++;
				}
			}
			if (num > this.maxCount)
			{
				return new PropertyConstraintViolationError(DataStrings.CollectiionWithTooManyItemsFormat(this.maxCount.ToString()), propertyDefinition, value, this);
			}
			return null;
		}

		private int maxCount;
	}
}
