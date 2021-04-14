using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NullValueError : PropertyValidationError
	{
		internal NullValueError(PropertyDefinition propertyDefinition, object invalidData) : base(ServerStrings.ExValueCannotBeNull, propertyDefinition, invalidData)
		{
		}

		public override string ToString()
		{
			return string.Format("Value cannot be null for property {0}. Invalid data = {1}.", base.PropertyDefinition, base.InvalidData);
		}
	}
}
