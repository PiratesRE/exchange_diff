using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class InvalidMultivalueElementError : PropertyValidationError
	{
		internal InvalidMultivalueElementError(PropertyValidationError elementError, object invalidMultivalueData, int invalidElementIndex) : base(new LocalizedString(ServerStrings.ExInvalidMultivalueElement(invalidElementIndex)), elementError.PropertyDefinition, invalidMultivalueData)
		{
			this.elementError = elementError;
			this.invalidElementIndex = invalidElementIndex;
		}

		public int InvalidElementIndex
		{
			get
			{
				return this.invalidElementIndex;
			}
		}

		public PropertyValidationError InvalidElementError
		{
			get
			{
				return this.elementError;
			}
		}

		public override string ToString()
		{
			return string.Format("The element with index {0} of the multivalue property {1} is invalid. Invalid data = {2}.", this.invalidElementIndex, base.PropertyDefinition, base.InvalidData);
		}

		private readonly int invalidElementIndex;

		private readonly PropertyValidationError elementError;
	}
}
