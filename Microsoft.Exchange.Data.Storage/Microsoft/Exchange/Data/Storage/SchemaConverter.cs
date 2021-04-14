using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SchemaConverter
	{
		internal SchemaConverter.Getter GetGetter(PropertyDefinition source)
		{
			KeyValuePair<SchemaConverter.Getter, SchemaConverter.Setter> keyValuePair;
			if (!this.mapping.TryGetValue(source, out keyValuePair))
			{
				return null;
			}
			return keyValuePair.Key;
		}

		internal SchemaConverter.Setter GetSetter(PropertyDefinition source)
		{
			KeyValuePair<SchemaConverter.Getter, SchemaConverter.Setter> keyValuePair;
			if (!this.mapping.TryGetValue(source, out keyValuePair))
			{
				return null;
			}
			return keyValuePair.Value;
		}

		protected void Add(PropertyDefinition source, PropertyDefinition destination)
		{
			this.Add(source, (IReadOnlyPropertyBag propertyBag) => propertyBag.GetProperties(new PropertyDefinition[]
			{
				destination
			})[0], delegate(IPropertyBag propertyBag, object value)
			{
				propertyBag.SetProperties(new PropertyDefinition[]
				{
					destination
				}, new object[]
				{
					value
				});
			});
		}

		protected void Add(PropertyDefinition source, SchemaConverter.Getter getter, SchemaConverter.Setter setter)
		{
			this.mapping.Add(source, new KeyValuePair<SchemaConverter.Getter, SchemaConverter.Setter>(getter, setter));
		}

		private readonly Dictionary<PropertyDefinition, KeyValuePair<SchemaConverter.Getter, SchemaConverter.Setter>> mapping = new Dictionary<PropertyDefinition, KeyValuePair<SchemaConverter.Getter, SchemaConverter.Setter>>();

		public delegate object Getter(IReadOnlyPropertyBag propertyBag);

		public delegate void Setter(IPropertyBag propertyBag, object value);
	}
}
