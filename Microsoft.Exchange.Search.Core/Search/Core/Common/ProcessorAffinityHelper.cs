using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.EventLog;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal static class ProcessorAffinityHelper
	{
		public static void SetProcessorAffinityForCts()
		{
			if (!ProcessorAffinityHelper.affinitySet)
			{
				lock (ProcessorAffinityHelper.lockObject)
				{
					if (!ProcessorAffinityHelper.affinitySet)
					{
						int num = Environment.ProcessorCount * SearchConfig.Instance.CtsProcessorAffinityPercentage / 100;
						if (num > 0)
						{
							using (Process currentProcess = Process.GetCurrentProcess())
							{
								if (currentProcess.ProcessName.IndexOf("NodeRunner", StringComparison.OrdinalIgnoreCase) >= 0 && Environment.CommandLine.IndexOf("ContentEngineNode", StringComparison.OrdinalIgnoreCase) >= 0)
								{
									try
									{
										ProcessorAffinityHelper.SetProcessorAffinity(currentProcess, num, 1);
									}
									catch (Exception ex)
									{
										ExEventLog exEventLog = new ExEventLog(ProcessorAffinityHelper.eventLogComponentGuid, "MSExchangeFastSearch");
										exEventLog.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_SetProcessorAffinityUnexpectedException, string.Empty, new object[]
										{
											ex.ToString()
										});
									}
								}
							}
						}
						ProcessorAffinityHelper.affinitySet = true;
					}
				}
			}
		}

		private static void SetProcessorAffinity(Process process, int affinityCount, int avoidProcessorCount)
		{
			int processorCount = Environment.ProcessorCount;
			ulong num = 0UL;
			Random random = new Random();
			int num2 = 0;
			if (avoidProcessorCount > processorCount - affinityCount)
			{
				avoidProcessorCount = processorCount - affinityCount;
			}
			for (int i = 0; i < processorCount; i++)
			{
				if (i >= avoidProcessorCount)
				{
					if (random.Next(processorCount - i) < affinityCount - num2)
					{
						num |= 1UL << i;
						num2++;
					}
					if (num2 == affinityCount)
					{
						break;
					}
				}
			}
			process.ProcessorAffinity = (IntPtr)((long)num);
		}

		private const string NodeRunner = "NodeRunner";

		private const string ConentEngineNode = "ContentEngineNode";

		private const int ProcessorsToAvoid = 1;

		private const string EventLogServiceName = "MSExchangeFastSearch";

		private static readonly Guid eventLogComponentGuid = Guid.Parse("c87fb454-7dfe-4559-af8c-3905438e1398");

		private static readonly object lockObject = new object();

		private static bool affinitySet;
	}
}
