using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class TranscriptionCounters
	{
		public static TranscriptionCountersInstance GetInstance(string instanceName)
		{
			return (TranscriptionCountersInstance)TranscriptionCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			TranscriptionCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return TranscriptionCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return TranscriptionCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			TranscriptionCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			TranscriptionCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			TranscriptionCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new TranscriptionCountersInstance(instanceName, (TranscriptionCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new TranscriptionCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (TranscriptionCounters.counters == null)
			{
				return;
			}
			TranscriptionCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeUMVoiceMailSpeechRecognition";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeUMVoiceMailSpeechRecognition", new CreateInstanceDelegate(TranscriptionCounters.CreateInstance));
	}
}
