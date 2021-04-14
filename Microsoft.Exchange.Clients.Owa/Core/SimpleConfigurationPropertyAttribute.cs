using System;
using System.Reflection;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal sealed class SimpleConfigurationPropertyAttribute : Attribute
	{
		internal SimpleConfigurationPropertyAttribute(string name) : this(name, false)
		{
		}

		internal SimpleConfigurationPropertyAttribute(string name, bool isRequired)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentOutOfRangeException("name", "name should not be empty");
			}
			this.name = name;
			this.isRequired = isRequired;
		}

		internal void SetValue(object entry, object value)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.propertyInfo.SetValue(entry, value, null);
		}

		internal object GetValue(object entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			return this.propertyInfo.GetValue(entry, null);
		}

		internal Type Type
		{
			get
			{
				return this.PropertyInfo.PropertyType;
			}
		}

		internal PropertyInfo PropertyInfo
		{
			get
			{
				return this.propertyInfo;
			}
			set
			{
				this.propertyInfo = value;
			}
		}

		internal ulong PropertyMask
		{
			get
			{
				return this.propertyMask;
			}
			set
			{
				this.propertyMask = value;
			}
		}

		internal bool IsRequired
		{
			get
			{
				return this.isRequired;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		private string name;

		private bool isRequired;

		private ulong propertyMask;

		private PropertyInfo propertyInfo;
	}
}
