using System;

namespace Microsoft.Exchange.Clients.Owa.Core
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
			if (type != typeof(int) && type != typeof(bool) && type != typeof(string))
			{
				throw new ArgumentException("type must be int, bool or string only, invalid type is: " + type.ToString());
			}
			if (defaultValue.GetType() != type)
			{
				throw new ArgumentException(string.Format("type of defaultValue {0} does not match type {1}", defaultValue.GetType().ToString(), type.ToString()));
			}
			this.name = name;
			this.type = type;
			this.defaultValue = defaultValue;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		private string name;

		private Type type;

		private object defaultValue;
	}
}
