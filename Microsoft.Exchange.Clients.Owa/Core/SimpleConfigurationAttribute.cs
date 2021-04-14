using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class SimpleConfigurationAttribute : Attribute
	{
		internal SimpleConfigurationAttribute(string configurationName) : this(configurationName, configurationName)
		{
		}

		internal SimpleConfigurationAttribute(string configurationName, string configurationRootNodeName)
		{
			if (configurationName == null)
			{
				throw new ArgumentNullException("configurationName");
			}
			if (configurationName.Length == 0)
			{
				throw new ArgumentException("configurationName is empty", "configurationName");
			}
			if (configurationRootNodeName == null)
			{
				throw new ArgumentNullException("ConfigurationRootNodeName");
			}
			if (configurationRootNodeName.Length == 0)
			{
				throw new ArgumentException("ConfigurationRootNodeName is empty", "ConfigurationRootNodeName");
			}
			this.configurationName = configurationName;
			this.configurationRootNodeName = configurationRootNodeName;
		}

		internal void AddProperty(SimpleConfigurationPropertyAttribute property)
		{
			if (this.propertyCount >= 64)
			{
				throw new OwaNotSupportedException(string.Format("SimpleConfiguration doesn't support types with more than {0} properties", 64));
			}
			ulong num = 1UL << this.propertyCount;
			property.PropertyMask = num;
			if (property.IsRequired)
			{
				this.requiredMask |= num;
			}
			this.propertyTable.Add(property.Name, property);
			this.propertyCount++;
		}

		internal SimpleConfigurationPropertyAttribute TryGetProperty(string propertyName)
		{
			SimpleConfigurationPropertyAttribute result = null;
			if (this.propertyTable.TryGetValue(propertyName, out result))
			{
				return result;
			}
			return null;
		}

		internal IEnumerable<SimpleConfigurationPropertyAttribute> GetPropertyCollection()
		{
			return this.propertyTable.Values;
		}

		internal string ConfigurationName
		{
			get
			{
				return this.configurationName;
			}
		}

		internal string ConfigurationRootNodeName
		{
			get
			{
				return this.configurationRootNodeName;
			}
		}

		internal ulong RequiredMask
		{
			get
			{
				return this.requiredMask;
			}
		}

		private string configurationName;

		private string configurationRootNodeName;

		private ulong requiredMask;

		private int propertyCount;

		private Dictionary<string, SimpleConfigurationPropertyAttribute> propertyTable = new Dictionary<string, SimpleConfigurationPropertyAttribute>();
	}
}
