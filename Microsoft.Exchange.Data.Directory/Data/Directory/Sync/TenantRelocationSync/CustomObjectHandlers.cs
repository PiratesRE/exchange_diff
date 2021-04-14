using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class CustomObjectHandlers
	{
		internal static CustomObjectHandlers Instance
		{
			get
			{
				return CustomObjectHandlers.instance.Value;
			}
		}

		internal bool TryGetValue(TenantRelocationSyncObject obj, out ICustomObjectHandler handler)
		{
			bool flag = false;
			handler = null;
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)obj[ADObjectSchema.ObjectClass];
			if (multiValuedProperty != null)
			{
				foreach (string key in multiValuedProperty)
				{
					flag = this.handlers.TryGetValue(key, out handler);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		private static readonly Lazy<CustomObjectHandlers> instance = new Lazy<CustomObjectHandlers>(() => new CustomObjectHandlers());

		private Dictionary<string, ICustomObjectHandler> handlers = new Dictionary<string, ICustomObjectHandler>(StringComparer.InvariantCultureIgnoreCase)
		{
			{
				ExchangeConfigurationUnit.MostDerivedClass,
				ExchangeConfigurationUnitHandler.Instance
			}
		};
	}
}
