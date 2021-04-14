using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors
{
	internal class DefaultStoragePropertyAccessor<TStoreObject, TValue> : StoragePropertyAccessor<TStoreObject, TValue>, IPropertyValueCollectionAccessor<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, TValue>, IPropertyAccessor<TStoreObject, TValue> where TStoreObject : IStorePropertyBag
	{
		public DefaultStoragePropertyAccessor(StorePropertyDefinition property, bool forceReadonly = false) : base(forceReadonly || (property.PropertyFlags & PropertyFlags.ReadOnly) == PropertyFlags.ReadOnly, null, new StorePropertyDefinition[]
		{
			property
		})
		{
			base.PropertyChangeMetadataGroup = PropertyChangeMetadata.GetGroupForProperty(property);
			this.Property = property;
		}

		private protected Microsoft.Exchange.Data.PropertyDefinition Property { protected get; private set; }

		protected ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CommonTracer;
			}
		}

		public bool TryGetValue(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, out TValue value)
		{
			if (propertyIndices == null)
			{
				throw new ArgumentNullException("propertyIndices");
			}
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num;
			if (!propertyIndices.TryGetValue(this.Property, out num))
			{
				value = default(TValue);
				return false;
			}
			if (num >= values.Count || num < 0)
			{
				string message = string.Format("Property index ({0}) is out of range (# Values: {1}).", num, values.Count);
				throw new ArgumentException(message, "propertyIndices");
			}
			object givenValue = values[num];
			return this.TryCastValue(givenValue, out value);
		}

		protected override bool PerformTryGetValue(TStoreObject container, out TValue value)
		{
			object givenValue = container.TryGetProperty(this.Property);
			return this.TryCastValue(givenValue, out value);
		}

		protected override void PerformSet(TStoreObject container, TValue value)
		{
			this.Trace.TraceDebug<string, TValue>((long)this.GetHashCode(), "DefaultStoragePropertyAccessor::PerformSet on {0} with {1}", this.Property.Name, value);
			container.SetOrDeleteProperty(this.Property, value);
		}

		protected virtual bool TryCastValue(object givenValue, out TValue value)
		{
			if (givenValue == null)
			{
				value = default(TValue);
				return false;
			}
			PropertyError propertyError = givenValue as PropertyError;
			if (propertyError != null)
			{
				value = default(TValue);
				return false;
			}
			bool result;
			try
			{
				value = (TValue)((object)givenValue);
				result = true;
			}
			catch (InvalidCastException)
			{
				this.Trace.TraceError((long)this.GetHashCode(), "[{0}::TryCastValue] InvalidCastException - Property: {1}; Type: {2}; Value Type: {3}; Value: {4}", new object[]
				{
					base.GetType().Name,
					this.Property.Name,
					this.Property.Type.FullName,
					givenValue.GetType().FullName,
					givenValue
				});
				value = default(TValue);
				result = false;
			}
			return result;
		}
	}
}
