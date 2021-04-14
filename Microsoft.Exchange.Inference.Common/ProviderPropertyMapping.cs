using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Common
{
	internal abstract class ProviderPropertyMapping<TProviderPropertyDefinition, TProviderItem, TContext> : PropertyMapping
	{
		protected ProviderPropertyMapping(PropertyDefinition propertyDefinition, TProviderPropertyDefinition[] providerPropertyDefinitions) : base(propertyDefinition)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			Util.ThrowOnNullOrEmptyArgument<TProviderPropertyDefinition>(providerPropertyDefinitions, "providerPropertyDefinitions");
			this.ProviderSpecificPropertyDefinitions = providerPropertyDefinitions;
		}

		protected TProviderPropertyDefinition[] ProviderPropertyDefinitions
		{
			get
			{
				return this.ProviderSpecificPropertyDefinitions;
			}
		}

		public abstract object GetPropertyValue(TProviderItem item, TContext context);

		public abstract void SetPropertyValue(TProviderItem item, object value, TContext context);

		public abstract object GetPropertyValue(IDictionary<TProviderPropertyDefinition, object> dictionary);

		public abstract void SetPropertyValue(IDictionary<TProviderPropertyDefinition, object> dictionary, object value);

		protected readonly TProviderPropertyDefinition[] ProviderSpecificPropertyDefinitions;
	}
}
