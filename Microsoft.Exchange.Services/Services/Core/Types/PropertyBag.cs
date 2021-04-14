using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class PropertyBag
	{
		internal PropertyBag()
		{
			this.propertyBag = new Dictionary<PropertyInformation, object>();
		}

		internal object this[PropertyInformation property]
		{
			get
			{
				return this.propertyBag[property];
			}
			set
			{
				if (value == null)
				{
					if (this.propertyBag.ContainsKey(property))
					{
						this.propertyBag.Remove(property);
						return;
					}
				}
				else
				{
					this.propertyBag[property] = value;
				}
			}
		}

		internal bool Contains(PropertyInformation property)
		{
			return this.propertyBag.ContainsKey(property);
		}

		internal bool Remove(PropertyInformation property)
		{
			return this.propertyBag.Remove(property);
		}

		internal void Clear()
		{
			this.propertyBag.Clear();
		}

		internal List<PropertyInformation> LoadedProperties
		{
			get
			{
				return this.propertyBag.Keys.ToList<PropertyInformation>();
			}
		}

		internal T GetValueOrDefault<T>(PropertyInformation property, T defaultValue)
		{
			object obj;
			if (this.propertyBag.TryGetValue(property, out obj))
			{
				return (T)((object)obj);
			}
			return defaultValue;
		}

		internal T GetValueOrDefault<T>(PropertyInformation property)
		{
			return this.GetValueOrDefault<T>(property, default(T));
		}

		internal T? GetNullableValue<T>(PropertyInformation property) where T : struct
		{
			object obj;
			if (this.propertyBag.TryGetValue(property, out obj))
			{
				return new T?((T)((object)obj));
			}
			return null;
		}

		internal void SetNullableValue<T>(PropertyInformation property, T? value) where T : struct
		{
			if (value != null)
			{
				this[property] = value.Value;
				return;
			}
			this.Remove(property);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<PropertyInformation, object> keyValuePair in this.propertyBag)
			{
				stringBuilder.Append(string.Format("{0} : {1}\n", keyValuePair.Key.LocalName, (keyValuePair.Value == null) ? "null" : keyValuePair.Value.ToString()));
			}
			return stringBuilder.ToString();
		}

		private Dictionary<PropertyInformation, object> propertyBag;
	}
}
