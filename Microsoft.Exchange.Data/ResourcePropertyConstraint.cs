using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ResourcePropertyConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = (string)value;
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					'/'
				});
				if (array.Length != 2)
				{
					return new PropertyConstraintViolationError(DataStrings.InvalidResourcePropertySyntax, propertyDefinition, value, this);
				}
				if (text.IndexOf('/') != text.LastIndexOf('/'))
				{
					return new PropertyConstraintViolationError(DataStrings.InvalidResourcePropertySyntax, propertyDefinition, value, this);
				}
				if (array[0].Length == 0 || array[1].Length == 0)
				{
					return new PropertyConstraintViolationError(DataStrings.InvalidResourcePropertySyntax, propertyDefinition, value, this);
				}
				if (!ResourcePropertyConstraint.IsLetterNumString(array[0]) || !ResourcePropertyConstraint.IsLetterNumString(array[1]))
				{
					return new PropertyConstraintViolationError(DataStrings.InvalidResourcePropertySyntax, propertyDefinition, value, this);
				}
			}
			return null;
		}

		private static bool IsLetterNumString(string s)
		{
			foreach (char c in s)
			{
				if (!char.IsNumber(c) && !char.IsLetter(c))
				{
					return false;
				}
			}
			return true;
		}
	}
}
