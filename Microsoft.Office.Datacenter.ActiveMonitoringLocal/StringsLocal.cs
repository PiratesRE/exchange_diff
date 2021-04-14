using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal static class StringsLocal
	{
		static StringsLocal()
		{
			StringsLocal.stringIDs.Add(3314367409U, "GenericServiceProbeTargetResource");
		}

		public static LocalizedString GenericServiceProbeTargetResource
		{
			get
			{
				return new LocalizedString("GenericServiceProbeTargetResource", StringsLocal.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(StringsLocal.IDs key)
		{
			return new LocalizedString(StringsLocal.stringIDs[(uint)key], StringsLocal.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Office.Datacenter.ActiveMonitoringLocal.StringsLocal", typeof(StringsLocal).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			GenericServiceProbeTargetResource = 3314367409U
		}
	}
}
