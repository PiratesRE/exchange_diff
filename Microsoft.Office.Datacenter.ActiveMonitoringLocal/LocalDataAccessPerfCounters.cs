using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal static class LocalDataAccessPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (LocalDataAccessPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in LocalDataAccessPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeWorkerTaskFrameworkLocalDataAccess";

		public static readonly ExPerformanceCounter LastProbeResultId = new ExPerformanceCounter("MSExchangeWorkerTaskFrameworkLocalDataAccess", "Last Probe Result ID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastMonitorResultId = new ExPerformanceCounter("MSExchangeWorkerTaskFrameworkLocalDataAccess", "Last Monitor Result ID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastResponderResultId = new ExPerformanceCounter("MSExchangeWorkerTaskFrameworkLocalDataAccess", "Last Responder Result ID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastMaintenanceResultId = new ExPerformanceCounter("MSExchangeWorkerTaskFrameworkLocalDataAccess", "Last Maintenance Result ID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DurationPersistentStateWrite = new ExPerformanceCounter("MSExchangeWorkerTaskFrameworkLocalDataAccess", "Duration of PersistentState Write", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DurationPersistentStateRead = new ExPerformanceCounter("MSExchangeWorkerTaskFrameworkLocalDataAccess", "Duration of PersistentState Read", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberofResultsPersistentStateWrite = new ExPerformanceCounter("MSExchangeWorkerTaskFrameworkLocalDataAccess", "Number of PersistentState results write into the file", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberofResultsPersistentStateRead = new ExPerformanceCounter("MSExchangeWorkerTaskFrameworkLocalDataAccess", "Number of PersistentState results read from the file", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			LocalDataAccessPerfCounters.LastProbeResultId,
			LocalDataAccessPerfCounters.LastMonitorResultId,
			LocalDataAccessPerfCounters.LastResponderResultId,
			LocalDataAccessPerfCounters.LastMaintenanceResultId,
			LocalDataAccessPerfCounters.DurationPersistentStateWrite,
			LocalDataAccessPerfCounters.DurationPersistentStateRead,
			LocalDataAccessPerfCounters.NumberofResultsPersistentStateWrite,
			LocalDataAccessPerfCounters.NumberofResultsPersistentStateRead
		};
	}
}
