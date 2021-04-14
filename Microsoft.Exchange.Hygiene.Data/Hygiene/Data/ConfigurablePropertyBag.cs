using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal abstract class ConfigurablePropertyBag : IPropertyBag, IReadOnlyPropertyBag, IConfigurable
	{
		public abstract ObjectId Identity { get; }

		public bool IsValid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ObjectState ObjectState { get; set; }

		public object PhysicalInstanceID
		{
			get
			{
				return this[ConfigurablePropertyBag.PhysicalInstanceKeyProp];
			}
			set
			{
				this[ConfigurablePropertyBag.PhysicalInstanceKeyProp] = value;
			}
		}

		internal IPropertyBag PropertyBag
		{
			get
			{
				IPropertyBag result;
				if ((result = this.propertyBag) == null)
				{
					result = (this.propertyBag = new ConfigurablePropertyBag.DictionaryBasedPropertyBag());
				}
				return result;
			}
			set
			{
				this.propertyBag = value;
			}
		}

		public virtual object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.PropertyBag[propertyDefinition];
			}
			set
			{
				this.PropertyBag[propertyDefinition] = value;
			}
		}

		public virtual IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return ((Dictionary<PropertyDefinition, object>)this.PropertyBag).Keys;
			}
			return ConfigurablePropertyBag.propertyDefinitions.GetOrAdd(this.GetSchemaType(), (Type type) => (from field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
			select field.GetValue(null)).OfType<PropertyDefinition>().ToArray<PropertyDefinition>());
		}

		public virtual Type GetSchemaType()
		{
			return base.GetType();
		}

		public ValidationError[] Validate()
		{
			throw new NotImplementedException();
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
		{
			throw new NotImplementedException();
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			throw new NotImplementedException();
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(PropertyDefinition property, out object value)
		{
			return ((Dictionary<PropertyDefinition, object>)this.PropertyBag).TryGetValue(property, out value);
		}

		public virtual void Expand()
		{
		}

		public virtual void Collapse()
		{
		}

		public void RemoveProperty(PropertyDefinition propertyDefinition)
		{
			((Dictionary<PropertyDefinition, object>)this.PropertyBag).Remove(propertyDefinition);
		}

		public static readonly HygienePropertyDefinition PhysicalInstanceKeyProp = DalHelper.PhysicalInstanceKeyProp;

		public static readonly HygienePropertyDefinition FssCopyIdProp = DalHelper.FssCopyIdProp;

		private static readonly ConcurrentDictionary<Type, PropertyDefinition[]> propertyDefinitions = new ConcurrentDictionary<Type, PropertyDefinition[]>();

		private IPropertyBag propertyBag;

		private class DictionaryBasedPropertyBag : Dictionary<PropertyDefinition, object>, IPropertyBag, IReadOnlyPropertyBag
		{
			public new object this[PropertyDefinition propertyDefinition]
			{
				get
				{
					object result;
					if (base.TryGetValue(propertyDefinition, out result))
					{
						return result;
					}
					if (propertyDefinition is ProviderPropertyDefinition && ((ProviderPropertyDefinition)propertyDefinition).IsMultivalued)
					{
						return null;
					}
					if (!propertyDefinition.Type.IsValueType)
					{
						return null;
					}
					return Activator.CreateInstance(propertyDefinition.Type);
				}
				set
				{
					base[propertyDefinition] = value;
				}
			}

			public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
			{
				throw new NotImplementedException();
			}

			public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
			{
				throw new NotImplementedException();
			}
		}
	}
}
