using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ColumnPropertyDefinitions
	{
		public ColumnPropertyDefinitions(ICollection<PropertyDefinition> propertyDefinitions, ICollection<PropertyDefinition> simplePropertyDefinitions, ICollection<PropTag> propertyTags)
		{
			this.PropertyDefinitions = propertyDefinitions;
			this.SimplePropertyDefinitions = simplePropertyDefinitions;
			this.PropertyTags = propertyTags;
		}

		public readonly ICollection<PropertyDefinition> PropertyDefinitions;

		public readonly ICollection<PropertyDefinition> SimplePropertyDefinitions;

		public readonly ICollection<PropTag> PropertyTags;
	}
}
