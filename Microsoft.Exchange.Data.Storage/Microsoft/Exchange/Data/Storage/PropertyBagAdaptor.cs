using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class PropertyBagAdaptor
	{
		public static PropertyBagAdaptor Create(ICoreObject coreObject)
		{
			return new PropertyBagAdaptor.CoreObjectPropertyBagAdaptor(coreObject);
		}

		public static PropertyBagAdaptor Create(IStorePropertyBag storePropertyBag)
		{
			return new PropertyBagAdaptor.StorePropertyBagAdaptor(storePropertyBag);
		}

		public T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
		{
			T result;
			try
			{
				result = this.GetValueOrDefaultInternal<T>(propertyDefinition, defaultValue);
			}
			catch (PropertyErrorException arg)
			{
				PropertyBagAdaptor.Tracer.TraceError<string, PropertyErrorException>((long)this.GetHashCode(), "Property {0} ignore due error {1}", propertyDefinition.Name, arg);
				result = defaultValue;
			}
			return result;
		}

		public abstract void SetValue(StorePropertyDefinition propertyDefinition, object value);

		public abstract void DeleteValue(StorePropertyDefinition propertyDefinition);

		protected abstract T GetValueOrDefaultInternal<T>(StorePropertyDefinition propertyDefinition, T defaultValue);

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		private sealed class CoreObjectPropertyBagAdaptor : PropertyBagAdaptor
		{
			public CoreObjectPropertyBagAdaptor(ICoreObject coreObject)
			{
				this.coreObject = coreObject;
			}

			public override void SetValue(StorePropertyDefinition propertyDefinition, object value)
			{
				this.coreObject.PropertyBag[propertyDefinition] = value;
			}

			public override void DeleteValue(StorePropertyDefinition propertyDefinition)
			{
				this.coreObject.PropertyBag.Delete(propertyDefinition);
			}

			protected override T GetValueOrDefaultInternal<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
			{
				return this.coreObject.PropertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
			}

			private ICoreObject coreObject;
		}

		private sealed class StorePropertyBagAdaptor : PropertyBagAdaptor
		{
			public StorePropertyBagAdaptor(IStorePropertyBag storePropertyBag)
			{
				this.storePropertyBag = storePropertyBag;
			}

			public override void SetValue(StorePropertyDefinition propertyDefinition, object value)
			{
				this.storePropertyBag[propertyDefinition] = value;
			}

			public override void DeleteValue(StorePropertyDefinition propertyDefinition)
			{
				this.storePropertyBag.Delete(propertyDefinition);
			}

			protected override T GetValueOrDefaultInternal<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
			{
				return this.storePropertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
			}

			private IStorePropertyBag storePropertyBag;
		}
	}
}
