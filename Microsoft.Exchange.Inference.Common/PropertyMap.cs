using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal class PropertyMap<TPropertyMapping> : IDictionary<PropertyDefinition, TPropertyMapping>, ICollection<KeyValuePair<PropertyDefinition, TPropertyMapping>>, IEnumerable<KeyValuePair<PropertyDefinition, TPropertyMapping>>, IEnumerable where TPropertyMapping : PropertyMapping
	{
		protected internal PropertyMap()
		{
			this.map = new Dictionary<PropertyDefinition, TPropertyMapping>();
			FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in fields)
			{
				PropertyMappingAttribute[] array2 = fieldInfo.GetCustomAttributes(typeof(PropertyMappingAttribute), true) as PropertyMappingAttribute[];
				if (array2 != null && array2.Length > 0)
				{
					object value = fieldInfo.GetValue(null);
					TPropertyMapping tpropertyMapping = value as TPropertyMapping;
					if (tpropertyMapping != null)
					{
						this.map.Add(tpropertyMapping.GenericPropertyDefinition, tpropertyMapping);
					}
				}
			}
		}

		public int Count
		{
			get
			{
				return this.map.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ICollection<PropertyDefinition> Keys
		{
			get
			{
				return this.map.Keys;
			}
		}

		public ICollection<TPropertyMapping> Values
		{
			get
			{
				return this.map.Values;
			}
		}

		public TPropertyMapping this[PropertyDefinition key]
		{
			get
			{
				return this.map[key];
			}
			set
			{
				this.map[key] = value;
			}
		}

		public void Add(PropertyDefinition key, TPropertyMapping value)
		{
			this.map.Add(key, value);
		}

		public bool ContainsKey(PropertyDefinition key)
		{
			return this.map.ContainsKey(key);
		}

		public bool Remove(PropertyDefinition key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(PropertyDefinition key, out TPropertyMapping value)
		{
			return this.map.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<PropertyDefinition, TPropertyMapping> item)
		{
			this.Add(item);
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<PropertyDefinition, TPropertyMapping> item)
		{
			return this.map.Contains(item);
		}

		public void CopyTo(KeyValuePair<PropertyDefinition, TPropertyMapping>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<PropertyDefinition, TPropertyMapping> item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<PropertyDefinition, TPropertyMapping>> GetEnumerator()
		{
			return this.map.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.map.GetEnumerator();
		}

		private const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

		private IDictionary<PropertyDefinition, TPropertyMapping> map;
	}
}
