using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.BackSync
{
	internal static class BackSyncPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (BackSyncPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in BackSyncPerfCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange Back Sync";

		public static readonly ExPerformanceCounter DeltaSyncTime = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncTimeSinceLast = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Time Since Last", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncResultSuccess = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Result Success", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncResultSystemError = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Result System Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncResultUserError = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Result User Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncSuccessRate = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Success Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncSuccessBase = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Success Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncSystemErrorRate = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync System Error Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncSystemErrorBase = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync System Error Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncUserErrorRate = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync User Error Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncUserErrorBase = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync User Error Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncCount = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncChangeCount = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Change Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeltaSyncRetryCookieCount = new ExPerformanceCounter("MSExchange Back Sync", "Delta Sync Retry Cookie Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncTime = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncTimeSinceLast = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync Time Since Last", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncResultSuccess = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync Result Success", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncResultSystemError = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync Result System Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncResultUserError = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync Result User Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncSuccessRate = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync Success Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncSuccessBase = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync Success Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncSystemErrorRate = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync System Error Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncSystemErrorBase = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync System Error Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncUserErrorRate = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync User Error Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncUserErrorBase = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync User Error Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ObjectFullSyncCount = new ExPerformanceCounter("MSExchange Back Sync", "Object Full Sync Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncTime = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncTimeSinceLast = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync Time Since Last", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncResultSuccess = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync Result Success", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncResultSystemError = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync Result System Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncResultUserError = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync Result User Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncSuccessRate = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync Success Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncSuccessBase = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync Success Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncSystemErrorRate = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync System Error Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncSystemErrorBase = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync System Error Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncUserErrorRate = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync User Error Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncUserErrorBase = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync User Error Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TenantFullSyncCount = new ExPerformanceCounter("MSExchange Back Sync", "Tenant Full Sync Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter BackLogCount = new ExPerformanceCounter("MSExchange Back Sync", "Back Log Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			BackSyncPerfCounters.DeltaSyncTime,
			BackSyncPerfCounters.DeltaSyncTimeSinceLast,
			BackSyncPerfCounters.DeltaSyncResultSuccess,
			BackSyncPerfCounters.DeltaSyncResultSystemError,
			BackSyncPerfCounters.DeltaSyncResultUserError,
			BackSyncPerfCounters.DeltaSyncSuccessRate,
			BackSyncPerfCounters.DeltaSyncSuccessBase,
			BackSyncPerfCounters.DeltaSyncSystemErrorRate,
			BackSyncPerfCounters.DeltaSyncSystemErrorBase,
			BackSyncPerfCounters.DeltaSyncUserErrorRate,
			BackSyncPerfCounters.DeltaSyncUserErrorBase,
			BackSyncPerfCounters.DeltaSyncCount,
			BackSyncPerfCounters.DeltaSyncChangeCount,
			BackSyncPerfCounters.DeltaSyncRetryCookieCount,
			BackSyncPerfCounters.ObjectFullSyncTime,
			BackSyncPerfCounters.ObjectFullSyncTimeSinceLast,
			BackSyncPerfCounters.ObjectFullSyncResultSuccess,
			BackSyncPerfCounters.ObjectFullSyncResultSystemError,
			BackSyncPerfCounters.ObjectFullSyncResultUserError,
			BackSyncPerfCounters.ObjectFullSyncSuccessRate,
			BackSyncPerfCounters.ObjectFullSyncSuccessBase,
			BackSyncPerfCounters.ObjectFullSyncSystemErrorRate,
			BackSyncPerfCounters.ObjectFullSyncSystemErrorBase,
			BackSyncPerfCounters.ObjectFullSyncUserErrorRate,
			BackSyncPerfCounters.ObjectFullSyncUserErrorBase,
			BackSyncPerfCounters.ObjectFullSyncCount,
			BackSyncPerfCounters.TenantFullSyncTime,
			BackSyncPerfCounters.TenantFullSyncTimeSinceLast,
			BackSyncPerfCounters.TenantFullSyncResultSuccess,
			BackSyncPerfCounters.TenantFullSyncResultSystemError,
			BackSyncPerfCounters.TenantFullSyncResultUserError,
			BackSyncPerfCounters.TenantFullSyncSuccessRate,
			BackSyncPerfCounters.TenantFullSyncSuccessBase,
			BackSyncPerfCounters.TenantFullSyncSystemErrorRate,
			BackSyncPerfCounters.TenantFullSyncSystemErrorBase,
			BackSyncPerfCounters.TenantFullSyncUserErrorRate,
			BackSyncPerfCounters.TenantFullSyncUserErrorBase,
			BackSyncPerfCounters.TenantFullSyncCount,
			BackSyncPerfCounters.BackLogCount
		};
	}
}
