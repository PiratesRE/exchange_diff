using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Common
{
	internal class InferencePropertyBag
	{
		public InferencePropertyBag()
		{
			this.propertyBag = new Dictionary<PropertyDefinition, object>();
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
				return this.propertyBag[propertyDefinition];
			}
			set
			{
				Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
				this.propertyBag[propertyDefinition] = value;
			}
		}

		public void Add(PropertyDefinition propertyDefinition, object value)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			this.propertyBag.Add(propertyDefinition, value);
		}

		public bool TryGetValue(PropertyDefinition propertyDefinition, out object value)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			return this.propertyBag.TryGetValue(propertyDefinition, out value);
		}

		private readonly Dictionary<PropertyDefinition, object> propertyBag;
	}
}
