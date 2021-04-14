using System;
using System.Configuration;
using System.Linq;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[ConfigurationCollection(typeof(EntityCacheConfiguration), AddItemName = "entityCache")]
	internal class EntityCacheConfigurationCollection : ConfigurationElementCollection
	{
		public EntityCacheConfiguration FindByName(string name)
		{
			return this.Cast<EntityCacheConfiguration>().FirstOrDefault((EntityCacheConfiguration c) => string.Equals(c.Name, name, StringComparison.InvariantCultureIgnoreCase));
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new EntityCacheConfiguration();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((EntityCacheConfiguration)element).Name;
		}
	}
}
