using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal abstract class SimpleConfigCache<ConfigClass, SourceObject> : IConfigCache where ConfigClass : ADConfigurationObject, new()
	{
		internal static Dictionary<string, ConfigClass> CacheFactory()
		{
			return new Dictionary<string, ConfigClass>(StringComparer.CurrentCultureIgnoreCase);
		}

		internal ConfigClass GetConfigFromSourceObject(SourceObject src)
		{
			ConfigClass result = default(ConfigClass);
			string text = this.KeyFromSourceObject(src);
			if (text != null)
			{
				this.cache.TryGetValue(text, out result);
			}
			return result;
		}

		internal virtual IEnumerable<ConfigClass> StartSearch(IConfigurationSession session)
		{
			return session.FindAllPaged<ConfigClass>();
		}

		public virtual void Refresh(IConfigurationSession session)
		{
			Dictionary<string, ConfigClass> dictionary = SimpleConfigCache<ConfigClass, SourceObject>.CacheFactory();
			int num = 0;
			IEnumerable<ConfigClass> enumerable = this.StartSearch(session);
			foreach (ConfigClass configClass in enumerable)
			{
				foreach (string key in this.KeysFromConfig(configClass))
				{
					ConfigClass configClass2 = default(ConfigClass);
					this.cache.TryGetValue(key, out configClass2);
					if (configClass2 != null && configClass2.WhenChanged == configClass.WhenChanged)
					{
						dictionary[key] = configClass2;
					}
					else
					{
						dictionary[key] = configClass;
						num++;
					}
				}
			}
			if (num > 0 || dictionary.Count != this.cache.Count)
			{
				this.cache = dictionary;
			}
		}

		protected virtual string[] KeysFromConfig(ConfigClass config)
		{
			if (config != null && config.Id != null)
			{
				return new string[]
				{
					config.Id.ToString()
				};
			}
			return new string[0];
		}

		protected virtual string KeyFromSourceObject(SourceObject src)
		{
			if (src == null)
			{
				return null;
			}
			return src.ToString();
		}

		protected Dictionary<string, ConfigClass> Cache
		{
			get
			{
				return this.cache;
			}
		}

		protected Dictionary<string, ConfigClass> cache = SimpleConfigCache<ConfigClass, SourceObject>.CacheFactory();
	}
}
