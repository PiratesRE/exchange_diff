using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NoSurroundingWhiteSpaceConstraint : PropertyDefinitionConstraint
	{
		internal NoSurroundingWhiteSpaceConstraint()
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = (string)value;
			if (text.Length > 0 && (char.IsWhiteSpace(text[0]) || char.IsWhiteSpace(text[text.Length - 1])))
			{
				return new PropertyConstraintViolationError(new LocalizedString(ServerStrings.ExStringContainsSurroundingWhiteSpace), propertyDefinition, value, this);
			}
			return null;
		}
	}
}
