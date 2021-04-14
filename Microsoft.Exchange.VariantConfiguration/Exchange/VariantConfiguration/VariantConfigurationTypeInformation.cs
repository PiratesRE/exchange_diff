using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.VariantConfiguration
{
	public class VariantConfigurationTypeInformation
	{
		internal VariantConfigurationTypeInformation(Type type, IDictionary<string, Type> properties)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			this.properties = properties;
			this.Type = type;
		}

		public Type Type { get; private set; }

		public IEnumerable<string> ValidPropertyNames
		{
			get
			{
				return this.properties.Keys;
			}
		}

		public static VariantConfigurationTypeInformation Create(Type type)
		{
			return new VariantConfigurationTypeInformation(type, VariantConfigurationTypeInformation.GetProperties(type));
		}

		public bool IsValidPropertyName(string propertyName)
		{
			return this.properties.ContainsKey(propertyName);
		}

		public bool IsValidPropertyValue(string propertyName, string propertyValue)
		{
			bool result;
			try
			{
				Type type = this.properties[propertyName];
				if (type == typeof(bool))
				{
					bool.Parse(propertyValue);
				}
				else if (type.IsEnum)
				{
					Enum.Parse(type, propertyValue);
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public Type GetParameterType(string parameterName)
		{
			if (!this.IsValidPropertyName(parameterName))
			{
				throw new ArgumentException(string.Format("{0} is not a valid parameter", parameterName));
			}
			return this.properties[parameterName];
		}

		public override string ToString()
		{
			return string.Join<KeyValuePair<string, Type>>(" : ", this.properties);
		}

		private static IDictionary<string, Type> GetProperties(Type type)
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			List<Type> list = new List<Type>();
			Queue<Type> queue = new Queue<Type>();
			queue.Enqueue(type);
			while (queue.Count > 0)
			{
				Type type2 = queue.Dequeue();
				foreach (Type item in type2.GetInterfaces())
				{
					if (!list.Contains(item) && !queue.Contains(item))
					{
						queue.Enqueue(item);
					}
				}
				foreach (PropertyInfo propertyInfo in type2.GetProperties())
				{
					dictionary[propertyInfo.Name] = propertyInfo.PropertyType;
				}
				list.Add(type2);
			}
			return dictionary;
		}

		private IDictionary<string, Type> properties;
	}
}
