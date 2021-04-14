using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfSynchronizer
	{
		public EhfSynchronizer(EhfTargetConnection ehfConnection)
		{
			this.ehfConnection = ehfConnection;
		}

		protected EhfTargetConnection EhfConnection
		{
			get
			{
				return this.ehfConnection;
			}
		}

		protected EhfProvisioningService ProvisioningService
		{
			get
			{
				return this.ehfConnection.ProvisioningService;
			}
		}

		protected EhfADAdapter ADAdapter
		{
			get
			{
				return this.ehfConnection.ADAdapter;
			}
		}

		protected EhfTargetServerConfig Config
		{
			get
			{
				return this.ehfConnection.Config;
			}
		}

		protected EdgeSyncDiag DiagSession
		{
			get
			{
				return this.ehfConnection.DiagSession;
			}
		}

		public virtual void ClearBatches()
		{
		}

		public virtual bool FlushBatches()
		{
			return true;
		}

		protected static bool LoadFullEntry(ExSearchResultEntry entry, string[] attributeNames, EhfTargetConnection ehfConnection)
		{
			ExSearchResultEntry exSearchResultEntry = ehfConnection.ADAdapter.ReadObjectEntry(entry.DistinguishedName, true, attributeNames);
			if (exSearchResultEntry == null)
			{
				ehfConnection.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Could not load object with DN <{0}>; ignoring the object", new object[]
				{
					entry.DistinguishedName
				});
				return false;
			}
			foreach (KeyValuePair<string, DirectoryAttribute> keyValuePair in exSearchResultEntry.Attributes)
			{
				if (!entry.Attributes.ContainsKey(keyValuePair.Key))
				{
					entry.Attributes.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return true;
		}

		protected static T[] CombineArrays<T>(T[] array1, T[] array2)
		{
			T[] array3 = new T[array1.Length + array2.Length];
			array1.CopyTo(array3, 0);
			array2.CopyTo(array3, array1.Length);
			return array3;
		}

		protected bool AddItemToBatch<ItemT>(ItemT item, ref List<ItemT> batch)
		{
			this.AddItemToLazyList<ItemT>(item, ref batch);
			return batch.Count >= this.Config.EhfSyncAppConfig.BatchSize;
		}

		protected void AddItemToLazyList<ItemT>(ItemT item, ref List<ItemT> list)
		{
			if (list == null)
			{
				list = new List<ItemT>(this.Config.EhfSyncAppConfig.BatchSize);
			}
			list.Add(item);
		}

		protected void InvokeProvisioningService(string operationName, EhfSynchronizer.ProvisioningServiceCall serviceCall, int numberOfEntries)
		{
			ExEventLog.EventTuple eventTuple = default(ExEventLog.EventTuple);
			Exception ex = null;
			EhfProvisioningService.MessageSecurityExceptionReason messageSecurityExceptionReason = EhfProvisioningService.MessageSecurityExceptionReason.Other;
			this.DiagSession.Tracer.TraceDebug<string>((long)this.DiagSession.GetHashCode(), "Executing EHF provisioning operation {0}", operationName);
			int transientExceptionRetryCount = this.Config.EhfSyncAppConfig.TransientExceptionRetryCount;
			bool flag = true;
			do
			{
				if (ex != null)
				{
					this.LogAndTraceException(operationName, ex, messageSecurityExceptionReason.ToString(), transientExceptionRetryCount + 1);
				}
				try
				{
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					serviceCall();
					stopwatch.Stop();
					this.ehfConnection.PerfCounterHandler.OnOperationSuccessfullyCompleted(operationName, stopwatch.ElapsedMilliseconds, numberOfEntries);
					this.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.High, "Successfully executed EHF provisioning operation {0}", new object[]
					{
						operationName
					});
					return;
				}
				catch (FaultException<ServiceFault> faultException)
				{
					ex = faultException;
					ServiceFault detail = faultException.Detail;
					if (detail.Id == FaultId.UnableToConnectToDatabase)
					{
						eventTuple = EdgeSyncEventLogConstants.Tuple_EhfTransientFailure;
						this.ehfConnection.PerfCounterHandler.OnOperationTransientFailure(operationName);
					}
					else
					{
						eventTuple = EdgeSyncEventLogConstants.Tuple_EhfCommunicationFailure;
						this.ehfConnection.PerfCounterHandler.OnOperationCommunicationFailure(operationName);
					}
				}
				catch (MessageSecurityException ex2)
				{
					messageSecurityExceptionReason = EhfProvisioningService.DecodeMessageSecurityException(ex2);
					switch (messageSecurityExceptionReason)
					{
					case EhfProvisioningService.MessageSecurityExceptionReason.DatabaseFailure:
						ex = ex2.InnerException;
						eventTuple = EdgeSyncEventLogConstants.Tuple_EhfTransientFailure;
						this.ehfConnection.PerfCounterHandler.OnOperationTransientFailure(operationName);
						goto IL_186;
					case EhfProvisioningService.MessageSecurityExceptionReason.InvalidCredentials:
						ex = ex2.InnerException;
						eventTuple = EdgeSyncEventLogConstants.Tuple_EhfInvalidCredentials;
						this.ehfConnection.PerfCounterHandler.OnOperationInvalidCredentialsFailure(operationName);
						flag = false;
						goto IL_186;
					}
					ex = ex2;
					eventTuple = EdgeSyncEventLogConstants.Tuple_EhfCommunicationFailure;
					this.ehfConnection.PerfCounterHandler.OnOperationCommunicationFailure(operationName);
					IL_186:;
				}
				catch (CommunicationException ex3)
				{
					ex = ex3;
					eventTuple = EdgeSyncEventLogConstants.Tuple_EhfCommunicationFailure;
					this.ehfConnection.PerfCounterHandler.OnOperationCommunicationFailure(operationName);
				}
				catch (TimeoutException ex4)
				{
					ex = ex4;
					eventTuple = EdgeSyncEventLogConstants.Tuple_EhfOperationTimedOut;
					this.ehfConnection.PerfCounterHandler.OnOperationTimeoutFailure(operationName);
					flag = false;
				}
				catch (EhfProvisioningService.ContractViolationException ex5)
				{
					ex = ex5;
					eventTuple = EdgeSyncEventLogConstants.Tuple_EhfServiceContractViolation;
					this.ehfConnection.PerfCounterHandler.OnOperationContractViolationFailure(operationName);
					flag = false;
				}
			}
			while (flag && transientExceptionRetryCount-- > 0);
			if (ex != null)
			{
				this.EventLogAndTraceException(operationName, eventTuple, ex, messageSecurityExceptionReason.ToString());
				this.ehfConnection.AbortSyncCycle(ex);
			}
		}

		protected void EventLogAndTraceException(string operationName, ExEventLog.EventTuple eventTuple, Exception exception, string exceptionReason)
		{
			this.LogAndTraceException(operationName, exception, exceptionReason);
			string periodicKey = (eventTuple.Period == ExEventLog.EventPeriod.LogPeriodic) ? operationName : null;
			this.DiagSession.EventLog.LogEvent(eventTuple, periodicKey, new object[]
			{
				operationName,
				exceptionReason,
				exception
			});
		}

		protected void LogAndTraceException(string operationName, Exception exception, string exceptionReason)
		{
			this.DiagSession.LogAndTraceException(exception, "Exception occurred while executing EHF provisioning operation {0}; Exception Reason {1}", new object[]
			{
				operationName,
				exceptionReason
			});
		}

		protected void LogAndTraceException(string operationName, Exception exception, string exceptionReason, int remainingRetryCount)
		{
			this.DiagSession.LogAndTraceException(exception, "Exception occurred while executing EHF provisioning operation {0}; Exception Reason {1}; Remaining Retry Count {2}", new object[]
			{
				operationName,
				exceptionReason,
				remainingRetryCount
			});
		}

		protected void HandlePerEntryFailureCounts(string operationName, int batchSize, int transientFailureCount, int permanentFailureCount, bool criticalOperation)
		{
			string text = null;
			if (transientFailureCount > 0)
			{
				text = this.DiagSession.LogAndTraceError("{0} completed with {1} per-entry transient failure(s); aborting this sync cycle", new object[]
				{
					operationName,
					transientFailureCount
				});
			}
			else if (permanentFailureCount > 0)
			{
				if (criticalOperation)
				{
					text = this.DiagSession.LogAndTraceError("Critical operation {0} completed with {1} per-entry permanent failures; aborting this sync cycle", new object[]
					{
						operationName,
						permanentFailureCount
					});
				}
				else if (permanentFailureCount == batchSize && batchSize >= 10)
				{
					text = this.DiagSession.LogAndTraceError("{0} completed with {1} per-entry permanent failures; all entries in batch failed; aborting this sync cycle", new object[]
					{
						operationName,
						permanentFailureCount
					});
				}
				else
				{
					this.DiagSession.LogAndTraceError("{0} completed with {1} per-entry permanent failures for batch size {2}; sync cycle will proceed", new object[]
					{
						operationName,
						permanentFailureCount,
						batchSize
					});
				}
			}
			if (transientFailureCount > 0 || permanentFailureCount > 0)
			{
				this.ehfConnection.PerfCounterHandler.OnPerEntryFailures(operationName, transientFailureCount, permanentFailureCount);
			}
			if (text != null)
			{
				this.DiagSession.EventLog.LogEvent(EdgeSyncEventLogConstants.Tuple_EhfPerEntryFailuresInBatch, null, new object[]
				{
					text
				});
				this.ehfConnection.AbortSyncCycle(new EdgeSyncCycleFailedException(text));
			}
		}

		private EhfTargetConnection ehfConnection;

		protected delegate void ProvisioningServiceCall();
	}
}
