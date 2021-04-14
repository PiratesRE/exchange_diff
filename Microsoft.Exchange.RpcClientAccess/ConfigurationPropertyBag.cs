using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ConfigurationPropertyBag
	{
		internal ConfigurationPropertyBag(ConfigurationPropertyBag previousConfiguration, IDictionary<ConfigurationSchema.Property, object> overrides)
		{
			if (previousConfiguration != null)
			{
				this.propertyStore = new Dictionary<ConfigurationSchema.Property, object>(previousConfiguration.propertyStore);
				this.isValid = previousConfiguration.isValid;
			}
			else
			{
				this.propertyStore = new Dictionary<ConfigurationSchema.Property, object>();
			}
			if (overrides != null)
			{
				foreach (KeyValuePair<ConfigurationSchema.Property, object> keyValuePair in overrides)
				{
					this.propertyStore[keyValuePair.Key] = keyValuePair.Value;
				}
				this.protectedProperties = overrides.Keys;
				return;
			}
			this.protectedProperties = Array<ConfigurationSchema.Property>.Empty;
		}

		internal bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		internal void Delete(IEnumerable<ConfigurationSchema.Property> properties)
		{
			this.EnsureNotFrozen();
			foreach (ConfigurationSchema.Property property in properties)
			{
				if (!this.protectedProperties.Contains(property))
				{
					this.propertyStore.Remove(property);
				}
			}
		}

		internal void Freeze()
		{
			this.isFrozen = true;
			this.protectedProperties = null;
		}

		internal TValue Get<TValue>(ConfigurationSchema.Property<TValue> property)
		{
			object obj;
			if (!this.propertyStore.TryGetValue(property, out obj))
			{
				return property.DefaultValue;
			}
			return (TValue)((object)obj);
		}

		internal void MarkInvalid()
		{
			this.isValid = false;
		}

		internal void Set<TValue>(ConfigurationSchema.Property<TValue> property, TValue value)
		{
			this.EnsureNotFrozen();
			if (!this.protectedProperties.Contains(property))
			{
				this.propertyStore[property] = value;
			}
		}

		private void EnsureNotFrozen()
		{
			if (this.isFrozen)
			{
				throw new InvalidOperationException("ConfigurationPropertyBag cannot be modified once it's frozen");
			}
		}

		private readonly Dictionary<ConfigurationSchema.Property, object> propertyStore;

		private ICollection<ConfigurationSchema.Property> protectedProperties;

		private bool isFrozen;

		private bool isValid = true;
	}
}
