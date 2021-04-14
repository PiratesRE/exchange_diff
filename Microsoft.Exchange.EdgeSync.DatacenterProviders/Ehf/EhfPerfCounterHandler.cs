using System;
using Microsoft.Exchange.EdgeSync.Common;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfPerfCounterHandler
	{
		public virtual void OnOperationSuccessfullyCompleted(string operationName, long latency, int batchSize)
		{
			EhfPerfCountersInstance instance = EhfPerfCounterHandler.GetInstance(operationName);
			if (instance == null)
			{
				return;
			}
			instance.OperationsTotal.Increment();
			instance.SuccessfulOperationsTotal.Increment();
			instance.LastLatency.RawValue = latency;
			instance.AverageLatency.IncrementBy(latency);
			instance.AverageLatencyBase.Increment();
			instance.LastEntryCount.RawValue = (long)batchSize;
			instance.EntryCountTotal.IncrementBy((long)batchSize);
			if (batchSize > 0)
			{
				instance.AverageLatencyPerEntry.IncrementBy(latency / (long)batchSize);
				instance.AverageLatencyPerEntryBase.Increment();
			}
		}

		public virtual void OnPerEntryFailures(string operationName, int transientFailureCount, int permanentFailureCount)
		{
			EhfPerfCountersInstance instance = EhfPerfCounterHandler.GetInstance(operationName);
			if (instance == null)
			{
				return;
			}
			instance.TransientEntryFailuresTotal.IncrementBy((long)transientFailureCount);
			instance.PermanentEntryFailuresTotal.IncrementBy((long)permanentFailureCount);
		}

		public virtual void OnOperationContractViolationFailure(string operationName)
		{
			EhfPerfCountersInstance instance = EhfPerfCounterHandler.GetInstance(operationName);
			if (instance == null)
			{
				return;
			}
			instance.OperationsTotal.Increment();
			instance.ContractViolationFailedOperationsTotal.Increment();
		}

		public virtual void OnOperationTransientFailure(string operationName)
		{
			EhfPerfCountersInstance instance = EhfPerfCounterHandler.GetInstance(operationName);
			if (instance == null)
			{
				return;
			}
			instance.OperationsTotal.Increment();
			instance.TransientFailedOperationsTotal.Increment();
		}

		public virtual void OnOperationTimeoutFailure(string operationName)
		{
			EhfPerfCountersInstance instance = EhfPerfCounterHandler.GetInstance(operationName);
			if (instance == null)
			{
				return;
			}
			instance.OperationsTotal.Increment();
			instance.TimeoutFailedOperationsTotal.Increment();
		}

		public virtual void OnOperationCommunicationFailure(string operationName)
		{
			EhfPerfCountersInstance instance = EhfPerfCounterHandler.GetInstance(operationName);
			if (instance == null)
			{
				return;
			}
			instance.OperationsTotal.Increment();
			instance.CommunicationFailedOperationsTotal.Increment();
		}

		public virtual void OnOperationInvalidCredentialsFailure(string operationName)
		{
			EhfPerfCountersInstance instance = EhfPerfCounterHandler.GetInstance(operationName);
			if (instance == null)
			{
				return;
			}
			instance.OperationsTotal.Increment();
			instance.InvalidCredentialsFailedOperationsTotal.Increment();
		}

		private static EhfPerfCountersInstance GetInstance(string instanceName)
		{
			EhfPerfCountersInstance result;
			try
			{
				result = EhfPerfCounters.GetInstance(instanceName);
			}
			catch (InvalidOperationException ex)
			{
				EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_EhfPerfCountersLoadFailure, instanceName, new object[]
				{
					ex
				});
				result = null;
			}
			return result;
		}
	}
}
