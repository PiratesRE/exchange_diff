using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SinglePropertyBag : IReadOnlyPropertyBag
	{
		public SinglePropertyBag(PropertyDefinition property, object propertyValue)
		{
			this.property = property;
			this.propertyValue = propertyValue;
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				if (propertyDefinition == this.property)
				{
					return this.propertyValue;
				}
				return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Format("{0}={1}", this.property, this.propertyValue);
		}

		private PropertyDefinition property;

		private object propertyValue;
	}
}
