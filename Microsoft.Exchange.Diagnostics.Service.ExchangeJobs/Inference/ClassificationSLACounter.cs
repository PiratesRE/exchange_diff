using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Inference
{
	public class ClassificationSLACounter : ICalculatedCounter
	{
		public static DiagnosticMeasurement SLAMeasurement
		{
			get
			{
				return ClassificationSLACounter.slaMeasurement;
			}
		}

		internal CounterValueHistory CounterValueHistory
		{
			get
			{
				return ClassificationSLACounter.counterValueHistory;
			}
		}

		public void OnLogLine(Dictionary<DiagnosticMeasurement, float?> countersAndValues, DateTime? timestamp = null)
		{
			float? num;
			float? num2;
			float? num3;
			float? num4;
			float? num5;
			float? num6;
			if (countersAndValues.TryGetValue(ClassificationSLACounter.SuccessfulDeliveries, out num) && countersAndValues.TryGetValue(ClassificationSLACounter.FailureCountMeasurement, out num2) && countersAndValues.TryGetValue(ClassificationSLACounter.AgentFailures, out num3) && countersAndValues.TryGetValue(ClassificationSLACounter.ItemsSkipped, out num4) && countersAndValues.TryGetValue(ClassificationSLACounter.NumberOfQuotaExceptions, out num5) && countersAndValues.TryGetValue(ClassificationSLACounter.NumberOfTransientExceptions, out num6))
			{
				if (num == null || num2 == null || num4 == null || num3 == null || num6 == null || num5 == null)
				{
					return;
				}
				ClassificationSLACounter.counterValueHistory.AddCounterValues(new float[]
				{
					num.Value,
					num2.Value,
					num4.Value,
					num3.Value,
					num6.Value,
					num5.Value
				});
				DateTime dateTime = DateTime.UtcNow - ClassificationSLACounter.TimeRange;
				float[] array;
				DateTime d;
				if (!ClassificationSLACounter.counterValueHistory.TryGetClosestCounterValues(dateTime, out array, out d) || Math.Abs((d - dateTime).TotalMinutes) > 10.0)
				{
					return;
				}
				float num7 = 0f;
				float num8 = 0f;
				float num9 = 0f;
				float num10 = 0f;
				float num11 = 0f;
				float num12 = 0f;
				if (num.Value >= array[0] && num2.Value >= array[1] && num4.Value >= array[2] && num3.Value >= array[3] && num6.Value >= array[4] && num5.Value >= array[5])
				{
					num7 = array[0];
					num8 = array[1];
					num9 = array[2];
					num10 = array[3];
					num11 = array[4];
					num12 = array[5];
				}
				float num13 = num.Value - num7;
				float num14 = num2.Value - num8;
				if (num14 == 0f)
				{
					return;
				}
				float num15 = num4.Value - num9;
				float num16 = num3.Value - num10;
				float num17 = num13 - num15;
				float num18 = num6.Value - num11;
				float num19 = num5.Value - num12;
				float num20 = num18 + num19;
				if (num17 < 1000f)
				{
					return;
				}
				float num21 = 100f * (num13 - num16 - num14 - num15) / num17;
				double num22 = ((double)(num14 - num20) * 98.5 + (double)num20 * 97.0) / (double)num14;
				if ((double)num21 > num22)
				{
					return;
				}
				ClassificationSLATrigger.LastFailureCounterData = string.Format("The counter values on last failure are: SuccessfulDeliveries:{0} SkippedItems:{1} AgentFailures:{2} DocumentFailures:{3} QuotaExceededExceptions:{6} TrainsientExceptions:{7}. \\r\\n\r\n                    So the SLA is 100 *{4}/{5}.And Expected SLA is {8}\\r\\n", new object[]
				{
					num13,
					num15,
					num16,
					num14,
					num13 - num16 - num14 - num15,
					num17,
					num19,
					num18,
					num22
				});
				countersAndValues.Add(ClassificationSLACounter.slaMeasurement, new float?(num21));
			}
		}

		public void OnLogHeader(List<KeyValuePair<int, DiagnosticMeasurement>> currentInputCounters)
		{
		}

		public const string ClassificationSLACounterName = "Classification SLA";

		private const int MinimumProcessedCountNeeded = 1000;

		private const int CounterValueMinuteGapAllowed = 10;

		internal static readonly DiagnosticMeasurement FailureCountMeasurement = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchangeInference Pipeline", "Number Of Failed Documents", "classificationpipeline");

		internal static readonly DiagnosticMeasurement SuccessfulDeliveries = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchange Delivery Store Driver", "SuccessfulDeliveries", string.Empty);

		internal static readonly DiagnosticMeasurement AgentFailures = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchange Delivery Store Driver Agents", "StoreDriverDelivery Agent Failure", "inference classification agent");

		internal static readonly DiagnosticMeasurement ItemsSkipped = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchangeInference Classification Processing", "Items Skipped", string.Empty);

		internal static readonly DiagnosticMeasurement NumberOfTransientExceptions = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchangeInference Classification Processing", "Number of Transient Exceptions in Pipeline", string.Empty);

		internal static readonly DiagnosticMeasurement NumberOfQuotaExceptions = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchangeInference Classification Processing", "Number of Quota Exceeded Exceptions in Pipeline", string.Empty);

		private static readonly TimeSpan TimeRange = TimeSpan.FromMinutes(60.0);

		private static readonly CounterValueHistory counterValueHistory = new CounterValueHistory(TimeSpan.FromHours(1.5));

		private static readonly DiagnosticMeasurement slaMeasurement = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchangeInference Pipeline", "Classification SLA", "classificationpipeline");
	}
}
