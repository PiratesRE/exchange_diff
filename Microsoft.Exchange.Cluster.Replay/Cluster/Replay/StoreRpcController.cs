using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class StoreRpcController : SafeRefCountedTimeoutWrapper, IStoreRpc, IListMDBStatus, IStoreMountDismount, IDisposable
	{
		private ExRpcAdmin ExRpcAdmin
		{
			get
			{
				ExRpcAdmin exRpcAdmin;
				lock (this.m_lockObj)
				{
					if (this.m_exRpcAdmin == null)
					{
						this.CreateExRpcAdminWithTimeout(this.ConnectivityTimeout);
					}
					exRpcAdmin = this.m_exRpcAdmin;
				}
				return exRpcAdmin;
			}
		}

		public virtual TimeSpan ConnectivityTimeout
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.StoreRpcConnectivityTimeoutInSec);
			}
		}

		public TimeSpan RpcTimeout
		{
			get
			{
				if (!this.ShouldUseTimeout)
				{
					return InvokeWithTimeout.InfiniteTimeSpan;
				}
				return TimeSpan.FromSeconds((double)RegistryParameters.StoreRpcGenericTimeoutInSec);
			}
		}

		public TimeSpan ListMdbStatusRpcTimeout
		{
			get
			{
				if (!this.ShouldUseTimeout)
				{
					return InvokeWithTimeout.InfiniteTimeSpan;
				}
				return TimeSpan.FromSeconds((double)RegistryParameters.ListMdbStatusRpcTimeoutInSec);
			}
		}

		public string ServerNameOrFqdn
		{
			get
			{
				return this.m_serverNameOrFqdn;
			}
		}

		protected bool ShouldUseTimeout
		{
			get
			{
				return this.ConnectivityTimeout != InvokeWithTimeout.InfiniteTimeSpan;
			}
		}

		public StoreRpcController(string serverNameOrFqdn, string clientTypeId) : base("StoreRpcController")
		{
			if (string.IsNullOrEmpty(serverNameOrFqdn))
			{
				this.m_serverNameOrFqdn = null;
			}
			else
			{
				this.m_serverNameOrFqdn = serverNameOrFqdn;
			}
			if (string.IsNullOrEmpty(clientTypeId))
			{
				this.m_clientTypeId = "Client=HA";
				return;
			}
			this.m_clientTypeId = clientTypeId;
		}

		public bool TestStoreConnectivity(TimeSpan timeout, out LocalizedException ex)
		{
			ex = null;
			bool result;
			try
			{
				int verMajor;
				int verMinor;
				base.ProtectedCallWithTimeout("GetAdminVersion", timeout, delegate
				{
					this.ExRpcAdmin.GetAdminVersion(out verMajor, out verMinor);
				});
				result = true;
			}
			catch (MapiRetryableException ex2)
			{
				ex = ex2;
				result = false;
			}
			catch (MapiPermanentException ex3)
			{
				ex = ex3;
				result = false;
			}
			return result;
		}

		public void ForceNewLog(Guid guidMdb, long numLogsToRoll = 1L)
		{
			for (long num = 0L; num < numLogsToRoll; num += 1L)
			{
				base.ProtectedCallWithTimeout("ForceNewLog", this.RpcTimeout, delegate
				{
					this.ExRpcAdmin.ForceNewLog(guidMdb);
				});
			}
		}

		public void MountDatabase(Guid guidStorageGroup, Guid guidMdb, int flags)
		{
			base.ProtectedCall("MountDatabase", delegate
			{
				this.ExRpcAdmin.MountDatabase(guidStorageGroup, guidMdb, flags);
			});
		}

		public void UnmountDatabase(Guid guidStorageGroup, Guid guidMdb, int flags)
		{
			UnmountFlags flags2 = (UnmountFlags)flags;
			TimeSpan timeout = InvokeWithTimeout.InfiniteTimeSpan;
			if (this.ShouldUseTimeout && (flags2 & UnmountFlags.SkipCacheFlush) != UnmountFlags.None)
			{
				timeout = TimeSpan.FromSeconds((double)RegistryParameters.AmDismountOrKillTimeoutInSec);
			}
			base.ProtectedCallWithTimeout("UnmountDatabase", timeout, delegate
			{
				this.ExRpcAdmin.UnmountDatabase(guidStorageGroup, guidMdb, flags);
			});
		}

		public void LogReplayRequest(Guid guidMdb, uint ulgenLogReplayMax, uint ulLogReplayFlags, out uint ulgenLogReplayNext, out JET_DBINFOMISC dbinfo, out IPagePatchReply pagePatchReply, out uint[] corruptedPages)
		{
			pagePatchReply = null;
			corruptedPages = null;
			uint tmplgenLogReplayNext = 0U;
			JET_DBINFOMISC tmpdbinfo = new JET_DBINFOMISC();
			uint patchReplyPageNumber = 0U;
			byte[] patchReplyToken = null;
			byte[] patchReplyData = null;
			uint[] pagesToBePatched = null;
			base.ProtectedCallWithTimeout("LogReplayRequest2", this.RpcTimeout, delegate
			{
				this.ExRpcAdmin.LogReplayRequest(guidMdb, ulgenLogReplayMax, ulLogReplayFlags, out tmplgenLogReplayNext, out tmpdbinfo, out patchReplyPageNumber, out patchReplyToken, out patchReplyData, out pagesToBePatched);
			});
			ulgenLogReplayNext = tmplgenLogReplayNext;
			dbinfo = tmpdbinfo;
			if (patchReplyPageNumber != 0U)
			{
				pagePatchReply = new PagePatchReply
				{
					PageNumber = checked((int)patchReplyPageNumber),
					Data = patchReplyData,
					Token = patchReplyToken
				};
			}
			if (pagesToBePatched != null && pagesToBePatched.Length > 0)
			{
				corruptedPages = pagesToBePatched;
			}
		}

		public void StartBlockModeReplicationToPassive(Guid guidMdb, string passiveName, uint ulFirstGenToSend)
		{
			base.ProtectedCallWithTimeout("StartBlockModeReplicationToPassive", this.RpcTimeout, delegate
			{
				this.ExRpcAdmin.StartBlockModeReplicationToPassive(guidMdb, passiveName, ulFirstGenToSend);
			});
		}

		public MdbStatus[] ListMdbStatus(Guid[] dbGuids)
		{
			return this.ListMdbStatus(dbGuids, null);
		}

		public MdbStatus[] ListMdbStatus(Guid[] dbGuids, TimeSpan? timeout)
		{
			MdbStatus[] status = null;
			base.ProtectedCallWithTimeout("ListMdbStatus", (timeout != null) ? timeout.Value : this.ListMdbStatusRpcTimeout, delegate
			{
				status = this.ExRpcAdmin.ListMdbStatus(dbGuids);
			});
			return status;
		}

		public MdbStatus[] ListMdbStatus(bool isBasicInformation)
		{
			return this.ListMdbStatus(isBasicInformation, null);
		}

		public MdbStatus[] ListMdbStatus(bool isBasicInformation, TimeSpan? timeout)
		{
			MdbStatus[] status = null;
			base.ProtectedCallWithTimeout("ListMdbStatus", (timeout != null) ? timeout.Value : this.ListMdbStatusRpcTimeout, delegate
			{
				status = this.ExRpcAdmin.ListMdbStatus(isBasicInformation);
			});
			return status;
		}

		public void SnapshotPrepare(Guid dbGuid, uint flags)
		{
			base.ProtectedCall("SnapshotPrepare", delegate
			{
				this.ExRpcAdmin.SnapshotPrepare(dbGuid, flags);
			});
		}

		public void SnapshotFreeze(Guid dbGuid, uint flags)
		{
			base.ProtectedCall("SnapshotFreeze", delegate
			{
				this.ExRpcAdmin.SnapshotFreeze(dbGuid, flags);
			});
		}

		public void SnapshotThaw(Guid dbGuid, uint flags)
		{
			base.ProtectedCall("SnapshotThaw", delegate
			{
				this.ExRpcAdmin.SnapshotThaw(dbGuid, flags);
			});
		}

		public void SnapshotTruncateLogInstance(Guid dbGuid, uint flags)
		{
			base.ProtectedCall("SnapshotTruncateLogInstance", delegate
			{
				this.ExRpcAdmin.SnapshotTruncateLogInstance(dbGuid, flags);
			});
		}

		public void SnapshotStop(Guid dbGuid, uint flags)
		{
			base.ProtectedCall("SnapshotStop", delegate
			{
				this.ExRpcAdmin.SnapshotStop(dbGuid, flags);
			});
		}

		public void GetDatabaseInformation(Guid guidMdb, out JET_DBINFOMISC databaseInformation)
		{
			JET_DBINFOMISC tmpDbInfo = new JET_DBINFOMISC();
			base.ProtectedCallWithTimeout("GetPhysicalDatabaseInformation", this.RpcTimeout, delegate
			{
				this.ExRpcAdmin.GetPhysicalDatabaseInformation(guidMdb, out tmpDbInfo);
			});
			databaseInformation = tmpDbInfo;
		}

		public void GetDatabaseProcessInfo(Guid guidMdb, out int workerProcessId, out int minVersion, out int maxVersion, out int requestedVersion)
		{
			PropValue[][] retVals = null;
			base.ProtectedCallWithTimeout("GetDatabaseProcessInfo", this.RpcTimeout, delegate
			{
				retVals = this.ExRpcAdmin.GetDatabaseProcessInfo(guidMdb, new PropTag[]
				{
					PropTag.WorkerProcessId,
					PropTag.MinimumDatabaseSchemaVersion,
					PropTag.OverallAgeLimit,
					PropTag.RequestedDatabaseSchemaVersion
				});
			});
			workerProcessId = 0;
			minVersion = 0;
			maxVersion = 0;
			requestedVersion = 0;
			if (retVals.Length > 0)
			{
				foreach (PropValue propValue in retVals[0])
				{
					if (propValue.PropTag == PropTag.WorkerProcessId && propValue.Value is int)
					{
						workerProcessId = (int)propValue.Value;
					}
					if (propValue.PropTag == PropTag.MinimumDatabaseSchemaVersion && propValue.Value is int)
					{
						minVersion = (int)propValue.Value;
					}
					if (propValue.PropTag == PropTag.OverallAgeLimit && propValue.Value is int)
					{
						maxVersion = (int)propValue.Value;
					}
					if (propValue.PropTag == PropTag.RequestedDatabaseSchemaVersion && propValue.Value is int)
					{
						requestedVersion = (int)propValue.Value;
					}
				}
			}
		}

		public void Close()
		{
			this.Dispose();
		}

		protected override Exception GetOperationCanceledException(string operationName, OperationAbortedException abortedEx)
		{
			return MapiExceptionHelper.CancelException(ReplayStrings.ReplayStoreOperationAbortedException(operationName), abortedEx);
		}

		protected override Exception GetOperationTimedOutException(string operationName, TimeoutException timeoutEx)
		{
			return MapiExceptionHelper.TimeoutException(ReplayStrings.ReplayTestStoreConnectivityTimedoutException(operationName, timeoutEx.Message), timeoutEx);
		}

		protected override void InternalProtectedDispose()
		{
			if (this.m_exRpcAdmin != null)
			{
				this.m_exRpcAdmin.Dispose();
				this.m_exRpcAdmin = null;
			}
		}

		private void CallExRpcWithTimeout(string operationName, TimeSpan timeout, Action exRpcAction)
		{
			try
			{
				InvokeWithTimeout.Invoke(exRpcAction, timeout);
			}
			catch (TimeoutException ex)
			{
				throw MapiExceptionHelper.TimeoutException(ReplayStrings.ReplayTestStoreConnectivityTimedoutException(operationName, ex.Message), ex);
			}
		}

		private void CreateExRpcAdminWithTimeout(TimeSpan timeout)
		{
			Action operation = delegate()
			{
				this.m_exRpcAdmin = ExRpcAdmin.Create(this.m_clientTypeId, this.m_serverNameOrFqdn, null, null, null);
			};
			base.ProtectedCallWithTimeout("ExRpcAdmin.Create", timeout, operation);
		}

		private readonly object m_lockObj = new object();

		private readonly string m_serverNameOrFqdn;

		private readonly string m_clientTypeId;

		private ExRpcAdmin m_exRpcAdmin;
	}
}
