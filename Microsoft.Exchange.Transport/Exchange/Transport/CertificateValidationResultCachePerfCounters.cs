using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class CertificateValidationResultCachePerfCounters
	{
		public static CertificateValidationResultCachePerfCountersInstance GetInstance(string instanceName)
		{
			return (CertificateValidationResultCachePerfCountersInstance)CertificateValidationResultCachePerfCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			CertificateValidationResultCachePerfCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return CertificateValidationResultCachePerfCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return CertificateValidationResultCachePerfCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			CertificateValidationResultCachePerfCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			CertificateValidationResultCachePerfCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			CertificateValidationResultCachePerfCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new CertificateValidationResultCachePerfCountersInstance(instanceName, (CertificateValidationResultCachePerfCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new CertificateValidationResultCachePerfCountersInstance(instanceName);
		}

		public static void SetCategoryName(string categoryName)
		{
			if (CertificateValidationResultCachePerfCounters.counters == null)
			{
				CertificateValidationResultCachePerfCounters.CategoryName = categoryName;
				CertificateValidationResultCachePerfCounters.counters = new PerformanceCounterMultipleInstance(CertificateValidationResultCachePerfCounters.CategoryName, new CreateInstanceDelegate(CertificateValidationResultCachePerfCounters.CreateInstance));
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (CertificateValidationResultCachePerfCounters.counters == null)
			{
				return;
			}
			CertificateValidationResultCachePerfCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public static string CategoryName;

		private static PerformanceCounterMultipleInstance counters;
	}
}
