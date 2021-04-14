using System;
using System.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class PerformanceCounterFactory
	{
		public static string DefaultDatabaseInstanceName
		{
			get
			{
				if (Globals.IsMultiProcess)
				{
					return PerformanceCounterFactory.defaultDatabaseInstanceName;
				}
				return null;
			}
			set
			{
				PerformanceCounterFactory.defaultDatabaseInstanceName = value;
			}
		}

		public static StorePerDatabasePerformanceCountersInstance GetDatabaseInstance(StoreDatabase database)
		{
			if (database == null)
			{
				if (Globals.IsMultiProcess)
				{
					if (PerformanceCounterFactory.cachedDefaultDatabaseInstance == null)
					{
						string text = PerformanceCounterFactory.DefaultDatabaseInstanceName;
						if (text != null)
						{
							PerformanceCounterFactory.cachedDefaultDatabaseInstance = StorePerDatabasePerformanceCounters.GetInstance(text);
						}
					}
					if (PerformanceCounterFactory.cachedDefaultDatabaseInstance != null)
					{
						return PerformanceCounterFactory.cachedDefaultDatabaseInstance;
					}
				}
				return StorePerDatabasePerformanceCounters.TotalInstance;
			}
			if (database.CachedStorePerDatabasePerformanceCountersInstance == null)
			{
				database.CachedStorePerDatabasePerformanceCountersInstance = StorePerDatabasePerformanceCounters.GetInstance(database.MdbName);
			}
			return database.CachedStorePerDatabasePerformanceCountersInstance;
		}

		public static StorePerDatabasePerformanceCountersInstance CreateDatabaseInstance(StoreDatabase database)
		{
			StorePerDatabasePerformanceCountersInstance instance = StorePerDatabasePerformanceCounters.GetInstance(database.MdbName);
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				if (instance.ProcessId.RawValue != (long)currentProcess.Id)
				{
					PerformanceCounterFactory.CloseDatabaseInstance(database);
					instance = StorePerDatabasePerformanceCounters.GetInstance(database.MdbName);
					StorePerDatabasePerformanceCounters.ResetInstance(instance.Name);
					instance.ProcessId.RawValue = (long)currentProcess.Id;
					instance.PercentRPCsInProgressBase.RawValue = (long)((ulong)Globals.MaxRPCThreadCount);
				}
			}
			database.CachedStorePerDatabasePerformanceCountersInstance = instance;
			return database.CachedStorePerDatabasePerformanceCountersInstance;
		}

		public static void CloseDatabaseInstance(StoreDatabase database)
		{
			StorePerDatabasePerformanceCounters.RemoveInstance(database.MdbName);
			database.CachedStorePerDatabasePerformanceCountersInstance = null;
		}

		public static StorePerClientTypePerformanceCountersInstance GetClientTypeInstance(ClientType clientType)
		{
			if (clientType < (ClientType)0 || (int)clientType >= PerformanceCounterFactory.perClientTypeCacheInstances.Length)
			{
				return null;
			}
			if (PerformanceCounterFactory.perClientTypeCacheInstances[(int)clientType] == null)
			{
				string instanceName = PerformanceCounterFactory.CreateClientTypeInstanceName(clientType);
				PerformanceCounterFactory.perClientTypeCacheInstances[(int)clientType] = StorePerClientTypePerformanceCounters.GetInstance(instanceName);
			}
			return PerformanceCounterFactory.perClientTypeCacheInstances[(int)clientType];
		}

		public static void CreateClientTypeInstances(bool reset)
		{
			for (int i = 1; i < 42; i++)
			{
				string instanceName = PerformanceCounterFactory.CreateClientTypeInstanceName((ClientType)i);
				StorePerClientTypePerformanceCountersInstance instance = StorePerClientTypePerformanceCounters.GetInstance(instanceName);
				if (reset)
				{
					StorePerClientTypePerformanceCounters.ResetInstance(instanceName);
				}
				PerformanceCounterFactory.perClientTypeCacheInstances[i] = instance;
			}
		}

		public static void CloseClientTypeInstances()
		{
			for (int i = 1; i < 42; i++)
			{
				if (PerformanceCounterFactory.perClientTypeCacheInstances[i] != null)
				{
					StorePerClientTypePerformanceCounters.RemoveInstance(PerformanceCounterFactory.CreateClientTypeInstanceName((ClientType)i));
					PerformanceCounterFactory.perClientTypeCacheInstances[i] = null;
				}
			}
		}

		private static string CreateClientTypeInstanceName(ClientType clientType)
		{
			return clientType.ToString();
		}

		private static StorePerClientTypePerformanceCountersInstance[] perClientTypeCacheInstances = new StorePerClientTypePerformanceCountersInstance[42];

		private static string defaultDatabaseInstanceName = null;

		private static StorePerDatabasePerformanceCountersInstance cachedDefaultDatabaseInstance = null;

		public static string ProcessInstanceName = Process.GetCurrentProcess().ProcessName;
	}
}
