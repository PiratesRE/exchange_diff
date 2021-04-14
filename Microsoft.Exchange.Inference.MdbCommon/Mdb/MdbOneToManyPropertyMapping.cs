using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal abstract class MdbOneToManyPropertyMapping : MdbPropertyMapping
	{
		public MdbOneToManyPropertyMapping(PropertyDefinition propertyDefinition, StorePropertyDefinition[] storePropertyDefinitions, bool isReadOnly, bool isStreamable) : base(propertyDefinition, storePropertyDefinitions)
		{
			this.isReadOnly = isReadOnly;
			this.isStreamable = isStreamable;
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public override bool IsStreamable
		{
			get
			{
				return this.isStreamable;
			}
		}

		public override object GetPropertyValue(IDictionary<StorePropertyDefinition, object> dictionary)
		{
			throw new NotImplementedException();
		}

		public override object GetPropertyValue(IItem item, IMdbPropertyMappingContext context)
		{
			throw new NotImplementedException();
		}

		public override void SetPropertyValue(IDictionary<StorePropertyDefinition, object> dictionary, object value)
		{
			throw new NotImplementedException();
		}

		public override void SetPropertyValue(IItem item, object value, IMdbPropertyMappingContext context)
		{
			throw new NotImplementedException();
		}

		private readonly bool isReadOnly;

		private readonly bool isStreamable;
	}
}
