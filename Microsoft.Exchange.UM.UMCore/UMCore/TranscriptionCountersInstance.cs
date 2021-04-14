using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class TranscriptionCountersInstance : PerformanceCounterInstance
	{
		internal TranscriptionCountersInstance(string instanceName, TranscriptionCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeUMVoiceMailSpeechRecognition")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.CurrentSessions = new ExPerformanceCounter(base.CategoryName, "Current Sessions", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentSessions);
				this.VoiceMessagesProcessed = new ExPerformanceCounter(base.CategoryName, "Voice Messages Processed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.VoiceMessagesProcessed);
				this.VoiceMessagesProcessedWithLowConfidence = new ExPerformanceCounter(base.CategoryName, "Voice Messages Processed with Low Confidence", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.VoiceMessagesProcessedWithLowConfidence);
				this.VoiceMessagesNotProcessedBecauseOfLowAvailabilityOfResources = new ExPerformanceCounter(base.CategoryName, "Voice Messages Not Processed Because of Low Availability of Resources", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.VoiceMessagesNotProcessedBecauseOfLowAvailabilityOfResources);
				this.VoiceMessagesPartiallyProcessedBecauseOfLowAvailabilityOfResources = new ExPerformanceCounter(base.CategoryName, "Voice Messages Partially Processed Because of Low Availability of Resources", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.VoiceMessagesPartiallyProcessedBecauseOfLowAvailabilityOfResources);
				this.AverageConfidence = new ExPerformanceCounter(base.CategoryName, "Average Confidence %", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageConfidence);
				long num = this.CurrentSessions.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal TranscriptionCountersInstance(string instanceName) : base(instanceName, "MSExchangeUMVoiceMailSpeechRecognition")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.CurrentSessions = new ExPerformanceCounter(base.CategoryName, "Current Sessions", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentSessions);
				this.VoiceMessagesProcessed = new ExPerformanceCounter(base.CategoryName, "Voice Messages Processed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.VoiceMessagesProcessed);
				this.VoiceMessagesProcessedWithLowConfidence = new ExPerformanceCounter(base.CategoryName, "Voice Messages Processed with Low Confidence", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.VoiceMessagesProcessedWithLowConfidence);
				this.VoiceMessagesNotProcessedBecauseOfLowAvailabilityOfResources = new ExPerformanceCounter(base.CategoryName, "Voice Messages Not Processed Because of Low Availability of Resources", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.VoiceMessagesNotProcessedBecauseOfLowAvailabilityOfResources);
				this.VoiceMessagesPartiallyProcessedBecauseOfLowAvailabilityOfResources = new ExPerformanceCounter(base.CategoryName, "Voice Messages Partially Processed Because of Low Availability of Resources", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.VoiceMessagesPartiallyProcessedBecauseOfLowAvailabilityOfResources);
				this.AverageConfidence = new ExPerformanceCounter(base.CategoryName, "Average Confidence %", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageConfidence);
				long num = this.CurrentSessions.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter CurrentSessions;

		public readonly ExPerformanceCounter VoiceMessagesProcessed;

		public readonly ExPerformanceCounter VoiceMessagesProcessedWithLowConfidence;

		public readonly ExPerformanceCounter VoiceMessagesNotProcessedBecauseOfLowAvailabilityOfResources;

		public readonly ExPerformanceCounter VoiceMessagesPartiallyProcessedBecauseOfLowAvailabilityOfResources;

		public readonly ExPerformanceCounter AverageConfidence;
	}
}
