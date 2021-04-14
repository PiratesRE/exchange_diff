using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class ExtendedPropertyStore<T> : IExtendedPropertyStore<T> where T : PropertyBase
	{
		public ExtendedPropertyStore()
		{
			this.propertyStore = new Dictionary<string, Dictionary<string, T>>(StringComparer.OrdinalIgnoreCase);
			this.propertiesList = new List<T>();
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.propertiesList.Count;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out T extendedProperty)
		{
			if (nameSpace == null)
			{
				throw new ArgumentNullException("nameSpace");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Dictionary<string, T> dictionary = null;
			if (this.propertyStore.TryGetValue(nameSpace, out dictionary) && dictionary.TryGetValue(name, out extendedProperty))
			{
				if (!MessageTraceCollapsedProperty.IsCollapsableProperty(nameSpace, name))
				{
					return true;
				}
				IEnumerable<PropertyBase> source;
				if (ExtendedPropertyStore<T>.TryGetCollapsedProperties(dictionary, nameSpace, name, out source))
				{
					extendedProperty = (source.LastOrDefault((PropertyBase p) => string.Equals(p.PropertyName, name, StringComparison.OrdinalIgnoreCase)) as T);
					if (extendedProperty != null)
					{
						return true;
					}
				}
			}
			extendedProperty = default(T);
			return false;
		}

		public T GetExtendedProperty(string nameSpace, string name)
		{
			if (nameSpace == null)
			{
				throw new ArgumentNullException("nameSpace");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			T result = default(T);
			Dictionary<string, T> dictionary = this.propertyStore[nameSpace];
			if (MessageTraceCollapsedProperty.IsCollapsableProperty(nameSpace, name))
			{
				IEnumerable<PropertyBase> source;
				if (!ExtendedPropertyStore<T>.TryGetCollapsedProperties(dictionary, nameSpace, name, out source))
				{
					throw new KeyNotFoundException(string.Format(CultureInfo.InvariantCulture, "The key '{0}' was not found in the property bag", new object[]
					{
						name
					}));
				}
				result = (T)((object)source.LastOrDefault((PropertyBase p) => string.Equals(p.PropertyName, name, StringComparison.OrdinalIgnoreCase)));
			}
			else
			{
				result = dictionary[name];
			}
			return result;
		}

		public IEnumerable<T> GetExtendedPropertiesEnumerable()
		{
			return this.propertiesList;
		}

		public void AddExtendedProperty(T extendedProperty)
		{
			if (extendedProperty == null)
			{
				throw new ArgumentNullException("extendedProperty");
			}
			Dictionary<string, T> orAdd = this.propertyStore.GetOrAdd(extendedProperty.Namespace, () => new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase));
			if (MessageTraceCollapsedProperty.IsCollapsableProperty(extendedProperty.Namespace, extendedProperty.PropertyName))
			{
				byte[] array = null;
				T t;
				if (orAdd.TryGetValue(MessageTraceCollapsedProperty.PropertyDefinition.Name, out t))
				{
					array = Convert.FromBase64String(t.PropertyValueBlob.Value);
				}
				array = MessageTraceCollapsedProperty.Collapse(array, extendedProperty.Namespace, extendedProperty);
				if (t == null)
				{
					t = extendedProperty;
					t.PropertyName = MessageTraceCollapsedProperty.PropertyDefinition.Name;
					t.PropertyValueGuid = Guid.Empty;
					t.PropertyValueInteger = null;
					t.PropertyValueString = null;
					t.PropertyValueDatetime = null;
					t.PropertyValueDecimal = null;
					t.PropertyValueBit = null;
					t.PropertyValueLong = null;
					orAdd[extendedProperty.PropertyName] = t;
					t.PropertyIndex = this.propertiesList.Count;
					this.propertiesList.Add(extendedProperty);
				}
				t.PropertyValueBlob = new BlobType(Convert.ToBase64String(array));
				return;
			}
			orAdd[extendedProperty.PropertyName] = extendedProperty;
			extendedProperty.PropertyIndex = this.propertiesList.Count;
			this.propertiesList.Add(extendedProperty);
		}

		private static bool TryGetCollapsedProperties(IDictionary<string, T> nameToPropDict, string nameSpace, string name, out IEnumerable<PropertyBase> expandedProperties)
		{
			expandedProperties = null;
			T t;
			bool result;
			if (result = nameToPropDict.TryGetValue(MessageTraceCollapsedProperty.PropertyDefinition.Name, out t))
			{
				byte[] data = Convert.FromBase64String(t.PropertyValueBlob.Value);
				expandedProperties = MessageTraceCollapsedProperty.Expand<MessageEventProperty>(data, name, () => new MessageEventProperty(nameSpace, name, false));
			}
			return result;
		}

		private Dictionary<string, Dictionary<string, T>> propertyStore;

		private List<T> propertiesList;
	}
}
