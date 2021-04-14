using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class SyntheticCounters
	{
		private SyntheticCounters()
		{
			this.totalsByOperationType = new Dictionary<OperationType, double>(50);
			this.totalsByClientType = new Dictionary<ClientType, double>(50);
			this.totalsByActivity = new Dictionary<uint, double>(200);
			this.totalsByOperation = new Dictionary<OperationType, Dictionary<byte, double>>(5);
			this.totalsByWorkLoad = new Dictionary<WorkLoadType, double>(5);
		}

		public static SyntheticCounters Create()
		{
			return new SyntheticCounters();
		}

		public void Add(OperationType operationType, double value)
		{
			if (this.totalsByOperationType.ContainsKey(operationType))
			{
				Dictionary<OperationType, double> dictionary;
				(dictionary = this.totalsByOperationType)[operationType] = dictionary[operationType] + value;
				return;
			}
			this.totalsByOperationType[operationType] = value;
		}

		public void Add(ClientType clientType, double value)
		{
			if (this.totalsByClientType.ContainsKey(clientType))
			{
				Dictionary<ClientType, double> dictionary;
				(dictionary = this.totalsByClientType)[clientType] = dictionary[clientType] + value;
				return;
			}
			this.totalsByClientType[clientType] = value;
		}

		public void Add(uint activityId, double value)
		{
			if (this.totalsByActivity.ContainsKey(activityId))
			{
				Dictionary<uint, double> dictionary;
				(dictionary = this.totalsByActivity)[activityId] = dictionary[activityId] + value;
				return;
			}
			this.totalsByActivity[activityId] = value;
		}

		public void Add(OperationType operationType, byte operationId, double value)
		{
			if (!this.totalsByOperation.ContainsKey(operationType))
			{
				this.totalsByOperation[operationType] = new Dictionary<byte, double>(200);
				this.totalsByOperation[operationType][operationId] = value;
				return;
			}
			if (this.totalsByOperation[operationType].ContainsKey(operationId))
			{
				Dictionary<byte, double> dictionary;
				(dictionary = this.totalsByOperation[operationType])[operationId] = dictionary[operationId] + value;
				return;
			}
			this.totalsByOperation[operationType][operationId] = value;
		}

		public void Add(WorkLoadType workLoadType, double value)
		{
			if (this.totalsByWorkLoad.ContainsKey(workLoadType))
			{
				Dictionary<WorkLoadType, double> dictionary;
				(dictionary = this.totalsByWorkLoad)[workLoadType] = dictionary[workLoadType] + value;
				return;
			}
			this.totalsByWorkLoad[workLoadType] = value;
		}

		public void WriteTrace()
		{
			IBinaryLogger logger = LoggerManager.GetLogger(LoggerType.SyntheticCounters);
			if (logger == null || !logger.IsLoggingEnabled)
			{
				return;
			}
			foreach (OperationType operationType in this.totalsByOperationType.Keys)
			{
				using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.SyntheticCounters, false, false, StoreEnvironment.MachineName, "ProcessorByOperationType", operationType.ToString(), this.totalsByOperationType[operationType]))
				{
					logger.TryWrite(traceBuffer);
				}
			}
			foreach (ClientType clientType in this.totalsByClientType.Keys)
			{
				using (TraceBuffer traceBuffer2 = TraceRecord.Create(LoggerManager.TraceGuids.SyntheticCounters, false, false, StoreEnvironment.MachineName, "ProcessorByClient", clientType.ToString(), this.totalsByClientType[clientType]))
				{
					logger.TryWrite(traceBuffer2);
				}
			}
			foreach (uint num in this.totalsByActivity.Keys)
			{
				using (TraceBuffer traceBuffer3 = TraceRecord.Create(LoggerManager.TraceGuids.SyntheticCounters, false, false, StoreEnvironment.MachineName, "ProcessorByActivity", ClientActivityStrings.GetString(num), this.totalsByActivity[num]))
				{
					logger.TryWrite(traceBuffer3);
				}
			}
			foreach (OperationType operationType2 in this.totalsByOperation.Keys)
			{
				foreach (KeyValuePair<byte, double> keyValuePair in this.totalsByOperation[operationType2])
				{
					string arg = RopSummaryResolver.ContainsKey(operationType2) ? RopSummaryResolver.Get(operationType2)(keyValuePair.Key) : "Unknown";
					string strValue = string.Format("{0}.{1}", operationType2, arg);
					using (TraceBuffer traceBuffer4 = TraceRecord.Create(LoggerManager.TraceGuids.SyntheticCounters, false, false, StoreEnvironment.MachineName, "ProcessorByOperation", strValue, keyValuePair.Value))
					{
						logger.TryWrite(traceBuffer4);
					}
				}
			}
			foreach (WorkLoadType workLoadType in this.totalsByWorkLoad.Keys)
			{
				using (TraceBuffer traceBuffer5 = TraceRecord.Create(LoggerManager.TraceGuids.SyntheticCounters, false, false, StoreEnvironment.MachineName, "ProcessorByWorkLoadType", workLoadType.ToString(), this.totalsByWorkLoad[workLoadType]))
				{
					logger.TryWrite(traceBuffer5);
				}
			}
			this.totalsByOperationType.Clear();
			this.totalsByClientType.Clear();
			this.totalsByActivity.Clear();
			this.totalsByOperation.Clear();
			this.totalsByWorkLoad.Clear();
		}

		private const string ProcessorByOperationType = "ProcessorByOperationType";

		private const string ProcessorByClient = "ProcessorByClient";

		private const string ProcessorByActivity = "ProcessorByActivity";

		private const string ProcessorByOperation = "ProcessorByOperation";

		private const string ProcessorByWorkLoadType = "ProcessorByWorkLoadType";

		private readonly Dictionary<OperationType, double> totalsByOperationType;

		private readonly Dictionary<ClientType, double> totalsByClientType;

		private readonly Dictionary<uint, double> totalsByActivity;

		private readonly Dictionary<OperationType, Dictionary<byte, double>> totalsByOperation;

		private readonly Dictionary<WorkLoadType, double> totalsByWorkLoad;
	}
}
