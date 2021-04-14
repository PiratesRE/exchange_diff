using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.DxStore.HA.Events;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class PerformanceEntry
	{
		public PerformanceEntry(StoreKind storeKind, bool isPrimary)
		{
			this.StoreKind = storeKind;
			this.IsPrimary = isPrimary;
			this.LatencyMap = new Dictionary<string, PerformanceEntry.LatencyMeasure>();
			this.ExceptionCountMap = new Dictionary<string, int>();
			this.ApiMap = new Dictionary<OperationCategory, PerformanceEntry.ApiMeasure>();
		}

		public StoreKind StoreKind { get; set; }

		public bool IsPrimary { get; set; }

		public int CountTotal { get; set; }

		public int CountInProgress { get; set; }

		public int CountReadRequestsSkipped { get; set; }

		public int CountWriteRequestsSkipped { get; set; }

		public int CountStale { get; set; }

		public int CountNotReady { get; set; }

		public int CountFailedConstraints { get; set; }

		public int CountFailedServerTimeout { get; set; }

		public int CountFailedClientTimeout { get; set; }

		public Dictionary<string, PerformanceEntry.LatencyMeasure> LatencyMap { get; set; }

		public Dictionary<string, int> ExceptionCountMap { get; set; }

		public Dictionary<OperationCategory, PerformanceEntry.ApiMeasure> ApiMap { get; set; }

		public void RecordStart()
		{
			lock (this)
			{
				this.CountTotal++;
				this.CountInProgress++;
			}
		}

		public void RecordFinish(RequestInfo req, long latencyInMs, Exception exception, bool isSkipped = false)
		{
			lock (this)
			{
				this.CountInProgress--;
				if (isSkipped)
				{
					if (req.OperationType == OperationType.Write)
					{
						this.CountWriteRequestsSkipped++;
					}
					else
					{
						this.CountReadRequestsSkipped++;
					}
				}
				else
				{
					bool isSuccess = exception == null;
					this.TrackLatency(req.OperationType, latencyInMs, isSuccess);
					this.RecordApiResult(req.OperationCategory, isSuccess, latencyInMs);
					if (exception != null)
					{
						this.TrackExceptions(exception);
					}
				}
			}
		}

		public string GetApiStatsAsXml()
		{
			XElement xelement = new XElement("ApiStats");
			IOrderedEnumerable<KeyValuePair<OperationCategory, PerformanceEntry.ApiMeasure>> orderedEnumerable = from kvp in this.ApiMap
			orderby kvp.Value.Latency.Max descending
			select kvp;
			foreach (KeyValuePair<OperationCategory, PerformanceEntry.ApiMeasure> keyValuePair in orderedEnumerable)
			{
				OperationCategory key = keyValuePair.Key;
				PerformanceEntry.ApiMeasure value = keyValuePair.Value;
				XElement content = new XElement("Api", new object[]
				{
					new XAttribute("Name", key),
					new XAttribute("Succeeded", value.Succeeded),
					new XAttribute("Failed", value.Failed),
					new XAttribute("Average", value.Latency.Average.ToString(".00")),
					new XAttribute("Max", value.Latency.Max),
					new XAttribute("MaxMinusOne", value.Latency.MaxMinusOne),
					new XAttribute("Min", value.Latency.Min),
					new XAttribute("MinPlusOne", value.Latency.MinPlusOne)
				});
				xelement.Add(content);
			}
			return xelement.ToString();
		}

		public string GetLatencyStatsAsXml()
		{
			XElement xelement = new XElement("LatencyStats");
			foreach (KeyValuePair<string, PerformanceEntry.LatencyMeasure> keyValuePair in this.LatencyMap)
			{
				string key = keyValuePair.Key;
				PerformanceEntry.LatencyMeasure value = keyValuePair.Value;
				XElement content = new XElement("Item", new object[]
				{
					new XAttribute("Name", key),
					new XAttribute("Count", value.Count),
					new XAttribute("Average", value.Average.ToString(".00")),
					new XAttribute("Max", value.Max),
					new XAttribute("MaxMinusOne", value.MaxMinusOne),
					new XAttribute("Min", value.Min),
					new XAttribute("MinPlusOne", value.MinPlusOne)
				});
				xelement.Add(content);
			}
			return xelement.ToString();
		}

		public string GetExceptionStatsAsXml()
		{
			XElement xelement = new XElement("ExceptionStats");
			IEnumerable<KeyValuePair<string, int>> enumerable = (from kvp in this.ExceptionCountMap
			orderby kvp.Value descending
			select kvp).Take(10);
			foreach (KeyValuePair<string, int> keyValuePair in enumerable)
			{
				string key = keyValuePair.Key;
				int value = keyValuePair.Value;
				XElement content = new XElement("Exception", new object[]
				{
					new XAttribute("Name", key),
					new XAttribute("Count", value)
				});
				xelement.Add(content);
			}
			return xelement.ToString();
		}

		public void PublishEvent(string currentProcessName)
		{
			string latencyStatsAsXml = this.GetLatencyStatsAsXml();
			string exceptionStatsAsXml = this.GetExceptionStatsAsXml();
			string apiStatsAsXml = this.GetApiStatsAsXml();
			DxStoreHACrimsonEvents.ApiPerfStats.Log<bool, StoreKind, int, int, int, int, int, int, int, int, string, string, string, string>(this.IsPrimary, this.StoreKind, this.CountTotal, this.CountInProgress, this.CountWriteRequestsSkipped, this.CountReadRequestsSkipped, this.CountStale, this.CountNotReady, this.CountFailedConstraints, this.CountFailedServerTimeout, latencyStatsAsXml, exceptionStatsAsXml, apiStatsAsXml, currentProcessName);
		}

		private void RecordApiResult(OperationCategory category, bool isSuccess, long latencyInMs)
		{
			PerformanceEntry.ApiMeasure apiMeasure;
			if (!this.ApiMap.TryGetValue(category, out apiMeasure) || apiMeasure == null)
			{
				apiMeasure = new PerformanceEntry.ApiMeasure();
				this.ApiMap[category] = apiMeasure;
			}
			if (isSuccess)
			{
				apiMeasure.Succeeded++;
			}
			else
			{
				apiMeasure.Failed++;
			}
			apiMeasure.Latency.Update(latencyInMs);
		}

		private void TrackLatency(OperationType operationType, long latencyInMs, bool isSuccess)
		{
			string latencyItemName = this.GetLatencyItemName(operationType, isSuccess);
			PerformanceEntry.LatencyMeasure latencyMeasure;
			if (!this.LatencyMap.TryGetValue(latencyItemName, out latencyMeasure) || latencyMeasure == null)
			{
				latencyMeasure = new PerformanceEntry.LatencyMeasure();
				this.LatencyMap[latencyItemName] = latencyMeasure;
			}
			latencyMeasure.Update(latencyInMs);
		}

		private string GetLatencyItemName(OperationType operationType, bool isSucceeded)
		{
			return string.Format("{0}_{1}", operationType, isSucceeded ? "Succeeded" : "Failed");
		}

		private void TrackExceptions(Exception exception)
		{
			int num = 0;
			string name = exception.GetType().Name;
			this.ExceptionCountMap.TryGetValue(name, out num);
			num = (this.ExceptionCountMap[name] = num + 1);
			FaultException<DxStoreServerFault> faultException = exception as FaultException<DxStoreServerFault>;
			if (faultException != null)
			{
				DxStoreServerFault detail = faultException.Detail;
				if (detail != null)
				{
					switch (detail.FaultCode)
					{
					case DxStoreFaultCode.Stale:
						this.CountStale++;
						return;
					case DxStoreFaultCode.InstanceNotReady:
						this.CountNotReady++;
						return;
					case DxStoreFaultCode.ServerTimeout:
						this.CountFailedServerTimeout++;
						break;
					case DxStoreFaultCode.ConstraintNotSatisfied:
						this.CountFailedConstraints++;
						return;
					default:
						return;
					}
				}
			}
		}

		public class ApiMeasure
		{
			public ApiMeasure()
			{
				this.Latency = new PerformanceEntry.LatencyMeasure();
			}

			public int Succeeded { get; set; }

			public int Failed { get; set; }

			public PerformanceEntry.LatencyMeasure Latency { get; set; }
		}

		public class LatencyMeasure
		{
			public long Count { get; set; }

			public long Total { get; set; }

			public double Average
			{
				get
				{
					if (this.Count > 0L)
					{
						return (double)this.Total / (double)this.Count;
					}
					return 0.0;
				}
			}

			public long Max { get; set; }

			public long MaxMinusOne { get; set; }

			public long Min { get; set; }

			public long MinPlusOne { get; set; }

			public void Update(long latency)
			{
				this.Count += 1L;
				if (this.Count == 1L)
				{
					this.Max = latency;
					this.Min = latency;
				}
				this.Total += latency;
				if (latency > this.Max)
				{
					this.MaxMinusOne = this.Max;
					this.Max = latency;
				}
				else if (latency < this.Max && latency > this.MaxMinusOne)
				{
					this.MaxMinusOne = latency;
				}
				if (latency < this.Min)
				{
					this.MinPlusOne = this.Min;
					this.Min = latency;
					return;
				}
				if (latency > this.Min && latency < this.MinPlusOne)
				{
					this.MinPlusOne = latency;
				}
			}
		}
	}
}
