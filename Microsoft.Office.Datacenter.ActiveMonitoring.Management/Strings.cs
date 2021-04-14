using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management
{
	internal static class Strings
	{
		public static LocalizedString InvalidPropertyOverrideValue(string propertyOverride)
		{
			return new LocalizedString("InvalidPropertyOverrideValue", Strings.ResourceManager, new object[]
			{
				propertyOverride
			});
		}

		public static LocalizedString InvalidMonitorIdentity(string monitorIdentity)
		{
			return new LocalizedString("InvalidMonitorIdentity", Strings.ResourceManager, new object[]
			{
				monitorIdentity
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Office.Datacenter.ActiveMonitoring.Management.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			InvalidPropertyOverrideValue,
			InvalidMonitorIdentity
		}
	}
}
