using System;
using System.Collections;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class InStringArrayConstraint : PropertyDefinitionConstraint
	{
		public InStringArrayConstraint(string[] targetArray, bool ignoreCase)
		{
			this.targetArray = targetArray;
			this.stringComparer = (ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
		}

		public IList TargetArray
		{
			get
			{
				return this.targetArray;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.targetArray != null)
			{
				for (int i = 0; i < this.targetArray.Length; i++)
				{
					if (this.stringComparer.Compare(this.targetArray[i], value as string) == 0)
					{
						return null;
					}
				}
				if (this.targetArray.Length > 0)
				{
					stringBuilder.Append(this.targetArray[0]);
					for (int j = 1; j < this.targetArray.Length; j++)
					{
						stringBuilder.AppendFormat(",{0}", this.targetArray[j]);
					}
				}
			}
			return new PropertyConstraintViolationError(DataStrings.ConstraintViolationValueIsNotInGivenStringArray(stringBuilder.ToString(), value.ToString()), propertyDefinition, value, this);
		}

		private string[] targetArray;

		private StringComparer stringComparer;
	}
}
