using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PropertyRestriction
	{
		public bool ShouldBlock(StorePropertyDefinition propertyDefinition, bool isLinked)
		{
			if (isLinked)
			{
				return this.BlockAfterLink.Contains(propertyDefinition);
			}
			return this.BlockBeforeLink.Contains(propertyDefinition);
		}

		public readonly HashSet<StorePropertyDefinition> BlockAfterLink = new HashSet<StorePropertyDefinition>();

		public readonly HashSet<StorePropertyDefinition> BlockBeforeLink = new HashSet<StorePropertyDefinition>();
	}
}
