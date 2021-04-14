using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Win32;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class InstrumentationCollector
	{
		internal static InstrumentationBaseStrategy CurrentStrategy
		{
			get
			{
				return InstrumentationCollector.collectionStrategy;
			}
		}

		internal static bool Start(InstrumentationBaseStrategy strategy)
		{
			ValidateArgument.NotNull(strategy, "strategy");
			bool result = false;
			try
			{
				InstrumentationCollector.TraceDebug("In InstrumentationCollector: Start", new object[0]);
				if (InstrumentationCollector.IsDartMachine())
				{
					lock (InstrumentationCollector.synclock)
					{
						if (InstrumentationCollector.workerThread == null)
						{
							InstrumentationCollector.collectionStrategy = strategy;
							InstrumentationCollector.workerThread = new Thread(new ParameterizedThreadStart(InstrumentationCollector.Run));
							InstrumentationCollector.workerThread.IsBackground = true;
							InstrumentationCollector.workerThread.Start();
							InstrumentationCollector.TraceDebug("InstrumentationCollector: Successfully Started.", new object[0]);
							result = true;
						}
						goto IL_9A;
					}
				}
				InstrumentationCollector.TraceDebug("InstrumentationCollector: Not a dart box.", new object[0]);
				IL_9A:;
			}
			catch (Exception ex)
			{
				InstrumentationCollector.TraceDebug("InstrumentationCollector: Start encountered error={0}", new object[]
				{
					ex
				});
			}
			return result;
		}

		internal static bool Stop()
		{
			bool result = false;
			lock (InstrumentationCollector.synclock)
			{
				try
				{
					if (InstrumentationCollector.workerThread != null)
					{
						InstrumentationCollector.workerThread.Abort();
						InstrumentationCollector.TraceDebug("InstrumentationCollector: Successfully Stopped ", new object[0]);
						result = true;
					}
				}
				catch (Exception ex)
				{
					InstrumentationCollector.TraceDebug("InstrumentationCollector: Stop encountered error={0}", new object[]
					{
						ex
					});
				}
				finally
				{
					InstrumentationCollector.workerThread = null;
					InstrumentationCollector.collectionStrategy = null;
				}
			}
			return result;
		}

		private static void Run(object state)
		{
			try
			{
				InstrumentationCollector.collectionStrategy.Initialize();
				StringBuilder stringBuilder = new StringBuilder(150);
				for (;;)
				{
					Thread.Sleep(InstrumentationCollector.Interval);
					stringBuilder.Length = 0;
					stringBuilder.AppendFormat("{0},", ExDateTime.UtcNow.ToString("T"));
					InstrumentationCollector.collectionStrategy.CollectData(stringBuilder);
					stringBuilder.Length--;
					InstrumentationCollector.TraceDebug(stringBuilder.ToString(), new object[0]);
				}
			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception ex)
			{
				InstrumentationCollector.TraceDebug("InstrumentationCollector: Run encountered error={0}. Stopping the thread", new object[]
				{
					ex
				});
			}
		}

		private static bool IsDartMachine()
		{
			bool result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\DART\\ClientRecovery"))
			{
				result = (registryKey != null && registryKey.GetValue("Alias") != null);
			}
			return result;
		}

		private static void TraceDebug(string message, params object[] args)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceTracer, 0, message, args);
		}

		private static readonly TimeSpan Interval = TimeSpan.FromSeconds(15.0);

		private static object synclock = new object();

		private static Thread workerThread;

		private static InstrumentationBaseStrategy collectionStrategy;
	}
}
