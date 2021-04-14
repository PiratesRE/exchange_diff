using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal abstract class PropertyMapping
	{
		protected PropertyMapping(PropertyDefinition property)
		{
			this.PropertyDefinition = property;
		}

		public PropertyDefinition GenericPropertyDefinition
		{
			get
			{
				return this.PropertyDefinition;
			}
		}

		public abstract bool IsReadOnly { get; }

		public abstract bool IsStreamable { get; }

		protected readonly PropertyDefinition PropertyDefinition;
	}
}
