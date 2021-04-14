using System;
using System.ComponentModel;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal class ClusterWriter : IClusterWriter
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LastLogWriterTracer;
			}
		}

		public static IClusterWriter Instance
		{
			get
			{
				return ClusterWriter.hookableInstance.Value;
			}
		}

		public static TimeSpan RefreshCheckDuration
		{
			get
			{
				int value = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "LastLogRefreshCheckDurationInSec", DefaultSettings.Get.LastLogRefreshCheckDurationInSeconds);
				return TimeSpan.FromSeconds((double)value);
			}
		}

		public static long LastLogUpdateAdvancingLimit
		{
			get
			{
				return RegistryReader.Instance.GetValue<long>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "LastLogUpdateAdvancingLimit", DefaultSettings.Get.LastLogUpdateAdvancingLimit);
			}
		}

		public static long LastLogUpdateLaggingLimit
		{
			get
			{
				return RegistryReader.Instance.GetValue<long>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "LastLogUpdateLaggingLimit", DefaultSettings.Get.LastLogUpdateLaggingLimit);
			}
		}

		public bool IsClusterRunning()
		{
			bool result = false;
			using (Context context = Context.CreateForSystem())
			{
				try
				{
					result = AmCluster.IsRunning();
				}
				catch (ClusterException ex)
				{
					ClusterWriter.Tracer.TraceError<ClusterException>(0L, "IsClusterRunning failed: {0}", ex);
					context.OnExceptionCatch(ex);
				}
			}
			return result;
		}

		public Exception TryWriteLastLog(Guid dbGuid, long lastLogGen)
		{
			Exception result = null;
			using (Context context = Context.CreateForSystem())
			{
				try
				{
					using (IClusterDB clusterDB = ClusterDB.Open())
					{
						if (clusterDB != null && clusterDB.IsInitialized)
						{
							string text = dbGuid.ToString();
							string text2 = text + "_Time";
							ExDateTime lastUpdateTimeFromClusdbUtc;
							if (this.IsClusterDatabaseUpdateRequired(clusterDB, text, text2, out lastUpdateTimeFromClusdbUtc))
							{
								this.CheckLogGenerationSanity(clusterDB, text, lastLogGen, lastUpdateTimeFromClusdbUtc);
								using (IClusterDBWriteBatch clusterDBWriteBatch = clusterDB.CreateWriteBatch(ClusterWriter.lastLogUpdateKeyName))
								{
									clusterDBWriteBatch.SetValue(text, lastLogGen.ToString());
									clusterDBWriteBatch.SetValue(text2, ExDateTime.UtcNow.ToString("s"));
									clusterDBWriteBatch.Execute();
								}
							}
						}
					}
				}
				catch (Win32Exception ex)
				{
					context.OnExceptionCatch(ex);
					ClusterWriter.Tracer.TraceError<Win32Exception>(0L, "TryWriteLastLog failed: {0}", ex);
					result = ex;
				}
				catch (ClusterException ex2)
				{
					context.OnExceptionCatch(ex2);
					ClusterWriter.Tracer.TraceError<ClusterException>(0L, "TryWriteLastLog failed: {0}", ex2);
					result = ex2;
				}
			}
			return result;
		}

		internal static IDisposable SetTestHook(IClusterWriter testHook)
		{
			return ClusterWriter.hookableInstance.SetTestHook(testHook);
		}

		private bool IsClusterDatabaseUpdateRequired(IClusterDB iClusterDb, string dbGuidString, string timeStampProperty, out ExDateTime lastUpdateTimeFromClusdbUtc)
		{
			bool result = true;
			lastUpdateTimeFromClusdbUtc = ExDateTime.MinValue.ToUtc();
			string value = iClusterDb.GetValue<string>(ClusterWriter.lastLogUpdateKeyName, timeStampProperty, string.Empty);
			ExDateTime dt;
			if (!string.IsNullOrEmpty(value) && ExDateTime.TryParse(value, out dt) && ClusterWriter.RefreshCheckDuration != TimeSpan.Zero && ExDateTime.UtcNow - dt < ClusterWriter.RefreshCheckDuration)
			{
				result = false;
			}
			return result;
		}

		private void CheckLogGenerationSanity(IClusterDB iClusterDb, string dbGuidString, long lastLogGenFromEse, ExDateTime lastUpdateTimeFromClusdbUtc)
		{
			string value = iClusterDb.GetValue<string>(ClusterWriter.lastLogUpdateKeyName, dbGuidString, string.Empty);
			long num;
			if (long.TryParse(value, out num))
			{
				if (num > lastLogGenFromEse)
				{
					long num2 = num - lastLogGenFromEse;
					if (num2 > ClusterWriter.LastLogUpdateAdvancingLimit)
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.LogPeriodicEvent(dbGuidString, MSExchangeISEventLogConstants.Tuple_LastLogUpdateTooAdvanced, new object[]
						{
							dbGuidString,
							num,
							lastLogGenFromEse,
							lastUpdateTimeFromClusdbUtc
						});
						return;
					}
				}
				else
				{
					long num3 = lastLogGenFromEse - num;
					if (num3 > ClusterWriter.LastLogUpdateLaggingLimit)
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.LogPeriodicEvent(dbGuidString, MSExchangeISEventLogConstants.Tuple_LastLogUpdateLaggingBehind, new object[]
						{
							dbGuidString,
							num,
							lastLogGenFromEse,
							lastUpdateTimeFromClusdbUtc
						});
					}
				}
			}
		}

		private static Hookable<IClusterWriter> hookableInstance = Hookable<IClusterWriter>.Create(false, new ClusterWriter());

		private static string lastLogUpdateKeyName = "ExchangeActiveManager\\LastLog";
	}
}
