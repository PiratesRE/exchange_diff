using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class TypeMismatchError : PropertyValidationError
	{
		internal TypeMismatchError(PropertyDefinition propertyDefinition, object invalidData) : base(new LocalizedString(ServerStrings.ExValueOfWrongType(invalidData, propertyDefinition.Type)), propertyDefinition, invalidData)
		{
		}

		public override string ToString()
		{
			return string.Format("Invalid data type for property {0}. Expected type = {1}. Invalid data = {2}.", base.PropertyDefinition, base.PropertyDefinition.Type, base.InvalidData);
		}
	}
}
