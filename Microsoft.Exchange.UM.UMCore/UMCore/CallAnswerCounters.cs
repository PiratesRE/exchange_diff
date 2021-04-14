using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class CallAnswerCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (CallAnswerCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in CallAnswerCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeUMCallAnswer";

		public static readonly ExPerformanceCounter CallAnsweringCalls = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Call Answering Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallAnsweringVoiceMessages = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Call Answering Voice Messages", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallAnsweringProtectedVoiceMessages = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Call Answering Protected Voice Messages", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallAnsweringVoiceMessageProtectionFailures = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Call Answering Voice Message Protection Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallAnsweringMissedCalls = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Call Answering Missed Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallAnsweringEscapes = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Call Answering Escapes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageVoiceMessageSize = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Average Voice Message Size", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRecentVoiceMessageSize = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Average Recent Voice Message Size", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageGreetingSize = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Average Greeting Size", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsWithoutPersonalGreetings = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Calls Without Personal Greetings", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FetchGreetingTimedOut = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Fetch Greeting Timed Out", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsDisconnectedByCallersDuringUMAudioHourglass = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Calls Disconnected by Callers During UM Audio Hourglass", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DivertedExtensionNotProvisioned = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Diverted Extension Not Provisioned", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsDisconnectedOnIrrecoverableExternalError = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Calls Disconnected by UM on Irrecoverable External Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsMissedBecausePipelineNotHealthy = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Pipeline Not Healthy", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsForSubscribersHavingOneOrMoreCARConfigured = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Total Calls to Subscribers with One or More Call Answering Rules Configured", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CARTimedOutEvaluations = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Total Number of Timed-out Call Answering Rule Evaluations", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CAREvaluationAverageTime = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Average Time Taken for Call Answering Rule Evaluations", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalCARCalls = new ExPerformanceCounter("MSExchangeUMCallAnswer", "Total Number of Call Answering Rules Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			CallAnswerCounters.CallAnsweringCalls,
			CallAnswerCounters.CallAnsweringVoiceMessages,
			CallAnswerCounters.CallAnsweringProtectedVoiceMessages,
			CallAnswerCounters.CallAnsweringVoiceMessageProtectionFailures,
			CallAnswerCounters.CallAnsweringMissedCalls,
			CallAnswerCounters.CallAnsweringEscapes,
			CallAnswerCounters.AverageVoiceMessageSize,
			CallAnswerCounters.AverageRecentVoiceMessageSize,
			CallAnswerCounters.AverageGreetingSize,
			CallAnswerCounters.CallsWithoutPersonalGreetings,
			CallAnswerCounters.FetchGreetingTimedOut,
			CallAnswerCounters.CallsDisconnectedByCallersDuringUMAudioHourglass,
			CallAnswerCounters.DivertedExtensionNotProvisioned,
			CallAnswerCounters.CallsDisconnectedOnIrrecoverableExternalError,
			CallAnswerCounters.CallsMissedBecausePipelineNotHealthy,
			CallAnswerCounters.CallsForSubscribersHavingOneOrMoreCARConfigured,
			CallAnswerCounters.CARTimedOutEvaluations,
			CallAnswerCounters.CAREvaluationAverageTime,
			CallAnswerCounters.TotalCARCalls
		};
	}
}
