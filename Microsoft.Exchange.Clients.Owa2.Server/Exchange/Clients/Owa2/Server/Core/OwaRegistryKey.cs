using System;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaRegistryKey
	{
		public OwaRegistryKey(string name, Type type, object defaultValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name may not be null or empty string");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (defaultValue == null)
			{
				throw new ArgumentNullException("defaultValue");
			}
			if (type != typeof(int) && type != typeof(uint) && type != typeof(bool) && type != typeof(string))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "type must be int, uint, bool or string only, invalid type is: {0}", new object[]
				{
					type
				}));
			}
			if (defaultValue.GetType() != type)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "type of defaultValue {0} does not match type {1}", new object[]
				{
					defaultValue.GetType().ToString(),
					type.ToString()
				}));
			}
			this.Name = name;
			this.KeyType = type;
			this.DefaultValue = defaultValue;
		}

		public string Name { get; private set; }

		public Type KeyType { get; private set; }

		public object DefaultValue { get; private set; }
	}
}
