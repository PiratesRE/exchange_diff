using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal class RemoteDeliveryHealthTracker
	{
		public RemoteDeliveryHealthTracker(TimeSpan refreshInterval, int threshold, IRemoteDeliveryHealthPerformanceCounters perfCounterWrapper)
		{
			this.refreshInterval = refreshInterval;
			this.messageThresholdToUpdateCounters = threshold;
			this.perfCounterWrapper = perfCounterWrapper;
			this.outboundIPPoolDictionary = new Dictionary<RiskLevel, Dictionary<int, RemoteDeliveryHealthTracker.OutboundIPPoolData>>();
		}

		public bool StartRefresh()
		{
			if (DateTime.UtcNow - this.lastRefreshedTime < this.refreshInterval)
			{
				return false;
			}
			this.outboundIPPoolDictionaryBeingUpdated = new Dictionary<RiskLevel, Dictionary<int, RemoteDeliveryHealthTracker.OutboundIPPoolData>>();
			this.subjectErrorsBeingUpdated = new int[8];
			return true;
		}

		public void UpdateHealthUsingQueueData(IRoutedMessageQueue queue)
		{
			if (queue.Key.NextHopType.IsSmtpConnectorDeliveryType)
			{
				RiskLevel riskLevel = queue.Key.RiskLevel;
				int outboundIPPool = queue.Key.OutboundIPPool;
				Dictionary<int, RemoteDeliveryHealthTracker.OutboundIPPoolData> dictionary;
				if (!this.outboundIPPoolDictionaryBeingUpdated.TryGetValue(riskLevel, out dictionary))
				{
					dictionary = new Dictionary<int, RemoteDeliveryHealthTracker.OutboundIPPoolData>();
					this.outboundIPPoolDictionaryBeingUpdated[riskLevel] = dictionary;
				}
				RemoteDeliveryHealthTracker.OutboundIPPoolData value;
				if (!dictionary.TryGetValue(outboundIPPool, out value))
				{
					value = default(RemoteDeliveryHealthTracker.OutboundIPPoolData);
				}
				if (!string.IsNullOrEmpty(queue.LastError.StatusCode))
				{
					value.QueuesWithErrors++;
				}
				int activeQueueLength = queue.ActiveQueueLength;
				value.MessageCount += activeQueueLength;
				value.QueueCount++;
				dictionary[outboundIPPool] = value;
				if (!string.IsNullOrWhiteSpace(queue.LastError.EnhancedStatusCode))
				{
					string[] array = queue.LastError.EnhancedStatusCode.Split(new char[]
					{
						'.'
					}, StringSplitOptions.RemoveEmptyEntries);
					int num;
					if (array.Length == 3 && int.TryParse(array[1], out num) && num >= 0 && num <= 7)
					{
						this.subjectErrorsBeingUpdated[num] += activeQueueLength;
					}
				}
			}
		}

		public void CompleteRefresh()
		{
			this.ProcessOutboundIPPoolDictionary(this.outboundIPPoolDictionaryBeingUpdated);
			this.ProcessSubjectErrors(this.subjectErrorsBeingUpdated);
			this.outboundIPPoolDictionary = this.outboundIPPoolDictionaryBeingUpdated;
			this.outboundIPPoolDictionaryBeingUpdated = null;
			this.subjectErrorsBeingUpdated = null;
			this.lastRefreshedTime = DateTime.UtcNow;
		}

		public XElement GetDiagnosticInfo()
		{
			Dictionary<RiskLevel, Dictionary<int, RemoteDeliveryHealthTracker.OutboundIPPoolData>> dictionary = this.outboundIPPoolDictionary;
			XElement xelement = new XElement("Pools");
			XElement xelement2 = new XElement("Total");
			xelement.Add(xelement2);
			XElement content = new XElement("UpdateTime", this.lastRefreshedTime);
			xelement.Add(content);
			int num = 0;
			foreach (RiskLevel riskLevel in dictionary.Keys)
			{
				foreach (int num2 in dictionary[riskLevel].Keys)
				{
					RemoteDeliveryHealthTracker.OutboundIPPoolData outboundIPPoolData = dictionary[riskLevel][num2];
					num++;
					XElement xelement3 = new XElement("Pool");
					xelement3.Add(new XElement("RiskLevel", riskLevel));
					xelement3.Add(new XElement("Port", num2));
					xelement3.Add(new XElement("QueueCount", outboundIPPoolData.QueueCount));
					xelement3.Add(new XElement("MessageCount", outboundIPPoolData.MessageCount));
					xelement3.Add(new XElement("QueueFailurePercentage", outboundIPPoolData.PercentageQueuesWithErrors));
					xelement.Add(xelement3);
				}
			}
			xelement2.Value = num.ToString();
			return xelement;
		}

		private void ProcessSubjectErrors(int[] subjectErrors)
		{
			for (int i = 0; i < subjectErrors.Length; i++)
			{
				this.perfCounterWrapper.UpdateSmtpResponseSubCodePerfCounter(i, subjectErrors[i]);
			}
		}

		private void ProcessOutboundIPPoolDictionary(Dictionary<RiskLevel, Dictionary<int, RemoteDeliveryHealthTracker.OutboundIPPoolData>> outboundIPPoolDictionary)
		{
			foreach (RiskLevel riskLevel in outboundIPPoolDictionary.Keys)
			{
				foreach (int num in outboundIPPoolDictionary[riskLevel].Keys)
				{
					RemoteDeliveryHealthTracker.OutboundIPPoolData outboundIPPoolData = outboundIPPoolDictionary[riskLevel][num];
					this.UpdateOutboundIPPoolPerfCounter(num, riskLevel, outboundIPPoolData.PercentageQueuesWithErrors, outboundIPPoolData.MessageCount);
				}
			}
			foreach (RiskLevel riskLevel2 in this.outboundIPPoolDictionary.Keys)
			{
				foreach (int num2 in this.outboundIPPoolDictionary[riskLevel2].Keys)
				{
					if (!outboundIPPoolDictionary.ContainsKey(riskLevel2) || !outboundIPPoolDictionary[riskLevel2].ContainsKey(num2))
					{
						this.ResetOutboundIPPoolPerfCounter(num2, riskLevel2);
					}
				}
			}
		}

		private void UpdateOutboundIPPoolPerfCounter(int pool, RiskLevel riskLevel, int percentageQueuesWithErrors, int messageCount)
		{
			if (messageCount > this.messageThresholdToUpdateCounters)
			{
				this.perfCounterWrapper.UpdateOutboundIPPoolPerfCounter(pool.ToString(), riskLevel, percentageQueuesWithErrors);
				return;
			}
			this.ResetOutboundIPPoolPerfCounter(pool, riskLevel);
		}

		private void ResetOutboundIPPoolPerfCounter(int pool, RiskLevel riskLevel)
		{
			this.perfCounterWrapper.UpdateOutboundIPPoolPerfCounter(pool.ToString(), riskLevel, 0);
		}

		private const int MaxKnownSmtpSubCode = 7;

		private readonly TimeSpan refreshInterval;

		private readonly int messageThresholdToUpdateCounters;

		private readonly IRemoteDeliveryHealthPerformanceCounters perfCounterWrapper;

		private DateTime lastRefreshedTime = DateTime.MinValue;

		private Dictionary<RiskLevel, Dictionary<int, RemoteDeliveryHealthTracker.OutboundIPPoolData>> outboundIPPoolDictionary;

		private Dictionary<RiskLevel, Dictionary<int, RemoteDeliveryHealthTracker.OutboundIPPoolData>> outboundIPPoolDictionaryBeingUpdated;

		private int[] subjectErrorsBeingUpdated;

		private struct OutboundIPPoolData
		{
			public int MessageCount { get; set; }

			public int QueueCount { get; set; }

			public int QueuesWithErrors { get; set; }

			public int PercentageQueuesWithErrors
			{
				get
				{
					if (this.QueueCount == 0)
					{
						return 0;
					}
					return this.QueuesWithErrors * 100 / this.QueueCount;
				}
			}
		}
	}
}
