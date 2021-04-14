using System;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal static class AmStoreHelper
	{
		internal static void Mount(Guid mdbGuid)
		{
			AmStoreHelper.Mount(mdbGuid, MountFlags.None);
		}

		internal static void Dismount(Guid mdbGuid)
		{
			Exception ex = AmStoreHelper.Dismount(mdbGuid, UnmountFlags.SkipCacheFlush);
			if (ex != null)
			{
				throw ex;
			}
		}

		internal static bool IsMapiExceptionDueToDatabaseDismounted(Exception ex)
		{
			return ex is MapiExceptionMdbOffline || ex is MapiExceptionNotFound || ex is MapiExceptionNetworkError;
		}

		internal static void Mount(Guid mdbGuid, MountFlags flags)
		{
			bool flag = false;
			using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null))
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				bool isCheckDbStatus = true;
				bool flag2 = false;
				for (;;)
				{
					ReplayCrimsonEvents.MountStoreRpcInitiated.Log<Guid, MountFlags>(mdbGuid, flags);
					Exception ex = null;
					try
					{
						try
						{
							newStoreControllerInstance.MountDatabase(Guid.Empty, mdbGuid, (int)flags);
							AmTrace.Info("rpcAdmin.MountDatabase({0}) successful.", new object[]
							{
								mdbGuid
							});
							isCheckDbStatus = false;
							flag2 = true;
							break;
						}
						catch (MapiExceptionMountInProgress mapiExceptionMountInProgress)
						{
							ex = mapiExceptionMountInProgress;
							AmTrace.Error("rpcAdmin.MountDatabase({0}) encountered {1}.", new object[]
							{
								mdbGuid,
								mapiExceptionMountInProgress.Message
							});
							if (!flag)
							{
								ReplayCrimsonEvents.MountDelayedUntilPreviousOperationIsComplete.Log<Guid, string>(mdbGuid, mapiExceptionMountInProgress.Message);
								flag = true;
							}
							if (stopwatch.Elapsed > AmStoreHelper.defaultMapiConflictTimeout)
							{
								throw;
							}
							if (AmHelper.SleepUntilShutdown(AmStoreHelper.defaultMapiConflictRetryInterval))
							{
								AmTrace.Debug("shutdown requested - hence not retrying mount for database {0}", new object[]
								{
									mdbGuid
								});
								throw;
							}
						}
						catch (MapiPermanentException ex2)
						{
							ex = ex2;
							throw;
						}
						catch (MapiRetryableException ex3)
						{
							ex = ex3;
							throw;
						}
						continue;
					}
					finally
					{
						if (flag2)
						{
							ReplayCrimsonEvents.MountStoreRpcSucceeded.Log<Guid>(mdbGuid);
						}
						else if (ex == null || !(ex is MapiExceptionMountInProgress))
						{
							ReplayCrimsonEvents.MountStoreRpcFailed.Log<Guid, string, Exception>(mdbGuid, (ex != null) ? ex.Message : null, ex);
						}
						AmStoreHelper.UpdateIsMountedCounterNoDatabaseCache(mdbGuid, null, flag2, isCheckDbStatus);
					}
					break;
				}
			}
		}

		internal static Exception Dismount(Guid mdbGuid, UnmountFlags flags)
		{
			Exception result = null;
			if ((flags & UnmountFlags.SkipCacheFlush) == UnmountFlags.SkipCacheFlush)
			{
				result = AmStoreHelper.DismountWithKillOnTimeout(mdbGuid, flags, true);
			}
			else
			{
				try
				{
					AmStoreHelper.RemoteDismount(null, mdbGuid, flags, true);
				}
				catch (MapiRetryableException ex)
				{
					result = ex;
				}
				catch (MapiPermanentException ex2)
				{
					result = ex2;
				}
			}
			return result;
		}

		internal static void RemoteDismount(AmServerName serverName, Guid mdbGuid)
		{
			AmStoreHelper.RemoteDismount(serverName, mdbGuid, UnmountFlags.SkipCacheFlush, true);
		}

		internal static void RemoteDismount(AmServerName serverName, Guid mdbGuid, UnmountFlags flags, bool retryOnConflict)
		{
			bool isCheckDbStatus = true;
			bool isMounted = true;
			Exception ex = null;
			try
			{
				AmTrace.Debug("RemoteDismount() starting for DB {0} on server {1}. UnmountFlags = {2}, retryOnConflict = {3}", new object[]
				{
					mdbGuid,
					serverName,
					flags,
					retryOnConflict
				});
				ReplayCrimsonEvents.DismountStoreRpcInitiated.Log<AmServerName, Guid, UnmountFlags, bool>(serverName, mdbGuid, flags, retryOnConflict);
				using (IStoreMountDismount storeMountDismountInstance = Dependencies.GetStoreMountDismountInstance(AmServerName.IsNullOrEmpty(serverName) ? null : serverName.Fqdn))
				{
					if (!retryOnConflict)
					{
						storeMountDismountInstance.UnmountDatabase(Guid.Empty, mdbGuid, (int)flags);
						isCheckDbStatus = false;
						isMounted = false;
						AmTrace.Info("rpcAdmin.UnmountDatabase({0}) successful.", new object[]
						{
							mdbGuid
						});
					}
					else
					{
						bool flag = false;
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						for (;;)
						{
							try
							{
								storeMountDismountInstance.UnmountDatabase(Guid.Empty, mdbGuid, (int)flags);
								isCheckDbStatus = false;
								isMounted = false;
								AmTrace.Info("rpcAdmin.UnmountDatabase({0}) successful.", new object[]
								{
									mdbGuid
								});
							}
							catch (MapiRetryableException ex2)
							{
								AmTrace.Error("rpcAdmin.UnmountDatabase({0}) encountered {1}.", new object[]
								{
									mdbGuid,
									ex2.Message
								});
								if (!(ex2 is MapiExceptionDismountInProgress))
								{
									throw;
								}
								if (!flag)
								{
									ReplayCrimsonEvents.DismountDelayedUntilPreviousOperationIsComplete.Log<Guid, string>(mdbGuid, ex2.Message);
									flag = true;
								}
								if (stopwatch.Elapsed > AmStoreHelper.defaultMapiConflictTimeout)
								{
									throw;
								}
								if (AmHelper.SleepUntilShutdown(AmStoreHelper.defaultMapiConflictRetryInterval))
								{
									AmTrace.Debug("shutdown requested - hence not retrying dismount for database {0}", new object[]
									{
										mdbGuid
									});
									throw;
								}
								continue;
							}
							break;
						}
					}
				}
			}
			catch (MapiPermanentException ex3)
			{
				ex = ex3;
				AmTrace.Debug("Dismount encountered exception {0}", new object[]
				{
					ex3.Message
				});
				if (!AmStoreHelper.IsMapiExceptionDueToDatabaseDismounted(ex3))
				{
					throw;
				}
			}
			catch (MapiRetryableException ex4)
			{
				ex = ex4;
				AmTrace.Debug("Dismount encountered exception {0}", new object[]
				{
					ex4.Message
				});
				if (!AmStoreHelper.IsMapiExceptionDueToDatabaseDismounted(ex4))
				{
					throw;
				}
			}
			finally
			{
				ReplayCrimsonEvents.DismountStoreRpcFinished.Log<AmServerName, Guid, string>(serverName, mdbGuid, (ex != null) ? ex.Message : "<none>");
				if (AmServerName.IsNullOrEmpty(serverName) || AmServerName.IsEqual(AmServerName.LocalComputerName, serverName))
				{
					AmStoreHelper.UpdateIsMountedCounterNoDatabaseCache(mdbGuid, null, isMounted, isCheckDbStatus);
				}
			}
		}

		internal static bool IsMountedLocally(Guid mdbGuid)
		{
			return AmStoreHelper.IsMounted(null, mdbGuid);
		}

		internal static bool IsMounted(AmServerName serverName, Guid mdbGuid)
		{
			bool result = false;
			Exception ex;
			MdbStatus mdbStatus = AmStoreHelper.GetMdbStatus(serverName, mdbGuid, out ex);
			if (mdbStatus != null && (mdbStatus.Status & MdbStatusFlags.Online) == MdbStatusFlags.Online)
			{
				result = true;
			}
			return result;
		}

		internal static MountStatus GetStoreDatabaseMountStatus(AmServerName serverName, Guid mdbGuid)
		{
			Exception ex = null;
			MountStatus result = MountStatus.Unknown;
			MdbStatus mdbStatus = AmStoreHelper.GetMdbStatus(serverName, mdbGuid, out ex);
			if (ex == null)
			{
				if (mdbStatus != null)
				{
					if ((mdbStatus.Status & MdbStatusFlags.Online) == MdbStatusFlags.Online)
					{
						result = MountStatus.Mounted;
					}
					else
					{
						result = MountStatus.Dismounted;
					}
				}
				else
				{
					result = MountStatus.Dismounted;
				}
			}
			else if (AmStoreHelper.IsMapiExceptionDueToDatabaseDismounted(ex))
			{
				result = MountStatus.Dismounted;
			}
			return result;
		}

		internal static MdbStatus GetMdbStatus(string serverFqdn, Guid mdbGuid)
		{
			Exception ex;
			return AmStoreHelper.GetMdbStatus(new AmServerName(serverFqdn), mdbGuid, out ex);
		}

		internal static MdbStatus GetMdbStatus(AmServerName serverName, Guid mdbGuid, out Exception exception)
		{
			return AmStoreHelper.GetMdbStatus(serverName, mdbGuid, null, null, out exception);
		}

		internal static MdbStatus GetMdbStatus(AmServerName serverName, Guid mdbGuid, string clientTypeId, TimeSpan? timeout, out Exception exception)
		{
			MdbStatus result = null;
			exception = null;
			string text = AmServerName.IsNullOrEmpty(serverName) ? null : serverName.Fqdn;
			try
			{
				using (IListMDBStatus storeListMDBStatusInstance = Dependencies.GetStoreListMDBStatusInstance(text, clientTypeId))
				{
					MdbStatus[] array = storeListMDBStatusInstance.ListMdbStatus(new Guid[]
					{
						mdbGuid
					}, timeout);
					if (array != null && array.Length > 0)
					{
						result = array[0];
					}
				}
			}
			catch (MapiPermanentException ex)
			{
				AmTrace.Error("Caught exception in GetMdbStatus({0}, {1}): {2}", new object[]
				{
					text,
					mdbGuid,
					ex.Message
				});
				exception = ex;
			}
			catch (MapiRetryableException ex2)
			{
				AmTrace.Error("Caught exception in GetMdbStatus({0}, {1}): {2}", new object[]
				{
					text,
					mdbGuid,
					ex2.Message
				});
				exception = ex2;
			}
			return result;
		}

		public static Exception IsDatabaseMounted(Guid dbGuid, string activeNode, TimeSpan timeout, out bool isMounted)
		{
			isMounted = false;
			Exception result = null;
			bool flag = false;
			try
			{
				using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(activeNode))
				{
					Guid[] dbGuids = new Guid[]
					{
						dbGuid
					};
					MdbStatus[] array = newStoreControllerInstance.ListMdbStatus(dbGuids);
					if (array == null || array.Length == 0)
					{
						AmTrace.Error("IsDatabaseMounted got an empty result", new object[0]);
					}
					else if (array[0].Status.HasFlag(MdbStatusFlags.Online))
					{
						flag = true;
					}
					else
					{
						AmTrace.Error("IsDatabaseMounted got an non-online status for db {0} : {1}", new object[]
						{
							dbGuid,
							array[0].Status
						});
					}
				}
				isMounted = flag;
			}
			catch (MapiPermanentException ex)
			{
				result = ex;
			}
			catch (MapiRetryableException ex2)
			{
				result = ex2;
			}
			return result;
		}

		internal static bool IsStoreRunning(AmServerName serverName)
		{
			bool flag = false;
			string text = AmServerName.IsNullOrEmpty(serverName) ? null : serverName.Fqdn;
			using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(text))
			{
				LocalizedException ex;
				flag = newStoreControllerInstance.TestStoreConnectivity(newStoreControllerInstance.ConnectivityTimeout, out ex);
				if (!flag)
				{
					AmTrace.Error("Ignoring exception in IsStoreRunning({0}): {1}", new object[]
					{
						text,
						ex.Message
					});
				}
			}
			return flag;
		}

		internal static bool GetAllDatabaseStatuses(AmServerName serverName, bool isBasicInformation, out MdbStatus[] mdbStatuses)
		{
			Exception ex;
			return AmStoreHelper.GetAllDatabaseStatuses(serverName, isBasicInformation, out mdbStatuses, out ex);
		}

		internal static bool GetAllDatabaseStatuses(AmServerName serverName, bool isBasicInformation, out MdbStatus[] mdbStatuses, out Exception exception)
		{
			return AmStoreHelper.GetAllDatabaseStatuses(serverName, isBasicInformation, null, null, out mdbStatuses, out exception);
		}

		internal static bool GetAllDatabaseStatuses(AmServerName serverName, bool isBasicInformation, string clientTypeId, TimeSpan? timeout, out MdbStatus[] mdbStatuses, out Exception exception)
		{
			bool result = false;
			exception = null;
			mdbStatuses = null;
			try
			{
				using (IListMDBStatus storeListMDBStatusInstance = Dependencies.GetStoreListMDBStatusInstance(AmServerName.IsNullOrEmpty(serverName) ? null : serverName.Fqdn, clientTypeId))
				{
					mdbStatuses = storeListMDBStatusInstance.ListMdbStatus(isBasicInformation, timeout);
					result = true;
				}
			}
			catch (MapiPermanentException ex)
			{
				exception = ex;
				AmTrace.Error("GetAllDatabaseStatuses() exception: {0}", new object[]
				{
					ex.Message
				});
			}
			catch (MapiRetryableException ex2)
			{
				exception = ex2;
				AmTrace.Error("GetAllDatabaseStatuses() exception: {0}", new object[]
				{
					ex2.Message
				});
			}
			return result;
		}

		internal static void DismountAll(string hint)
		{
			AmTrace.Debug("AmStoreHelper.DismountAll( {0} ): Started dismounting all mounted databases.", new object[]
			{
				hint
			});
			lock (AmStoreHelper.ForceDismountLocker)
			{
				MdbStatus[] array = null;
				DateTime utcNow = DateTime.UtcNow;
				if (!AmStoreHelper.GetAllDatabaseStatuses(null, false, out array))
				{
					AmTrace.Error("AmStoreHelper.DismountAll( {0} ): GetAllDatabaseStatuses() failed. Now attempting to kill store to quickly get dismounted databases.", new object[]
					{
						hint
					});
					AmStoreServiceMonitor.KillStoreIfRunningBefore(utcNow, "DismountAll");
				}
				else if (array != null && array.Length > 0)
				{
					DismountDatabasesInParallel dismountDatabasesInParallel = new DismountDatabasesInParallel(array);
					dismountDatabasesInParallel.Execute(RegistryParameters.AmDismountOrKillTimeoutInSec * 1000, hint);
				}
				else
				{
					AmTrace.Debug("Dismount all skipped since there are no mounted databases", new object[0]);
				}
			}
		}

		private static bool UpdateIsMountedCounterNoDatabaseCache(Guid mdbGuid, string mdbName, bool isMounted, bool isCheckDbStatus)
		{
			Dependencies.ReplayAdObjectLookup.DatabaseLookup.FindAdObjectByGuidEx(mdbGuid, AdObjectLookupFlags.ReadThrough);
			return AmStoreHelper.UpdateIsMountedCounter(mdbGuid, mdbName, isMounted, isCheckDbStatus);
		}

		internal static bool UpdateIsMountedCounter(Guid mdbGuid, string mdbName, bool isMounted, bool isCheckDbStatus)
		{
			if (string.IsNullOrEmpty(mdbName))
			{
				IADDatabase iaddatabase = Dependencies.ReplayAdObjectLookup.DatabaseLookup.FindAdObjectByGuid(mdbGuid);
				if (iaddatabase != null)
				{
					mdbName = iaddatabase.Name;
				}
			}
			if (!string.IsNullOrEmpty(mdbName))
			{
				bool flag = false;
				try
				{
					flag = AmStoreHelper.sm_perfCounterLock.Lock(mdbGuid, AmDbLockReason.UpdatePerfCounter);
					if (isCheckDbStatus)
					{
						isMounted = AmStoreHelper.IsMounted(null, mdbGuid);
					}
					AmTrace.Debug("AmStoreHelper.UpdateIsMountedCounter: Database {0} ({1}) IsMounted is {2}", new object[]
					{
						mdbGuid,
						mdbName,
						isMounted
					});
					ActiveManagerPerfmonInstance instance = ActiveManagerPerfmon.GetInstance(mdbName);
					if (instance != null)
					{
						instance.IsMounted.RawValue = (isMounted ? 1L : 0L);
					}
					return isMounted;
				}
				finally
				{
					if (flag)
					{
						AmStoreHelper.sm_perfCounterLock.Release(mdbGuid, AmDbLockReason.UpdatePerfCounter);
					}
				}
			}
			AmTrace.Error("AmStoreHelper.UpdateIsMountedCounter: Perfmon update skipped for {0} since mdbName is null", new object[]
			{
				mdbGuid
			});
			return isMounted;
		}

		internal static void UpdateCopyRoleIsActivePerfCounter(Guid mdbGuid, string mdbName, bool copyRoleIsActive)
		{
			if (string.IsNullOrEmpty(mdbName))
			{
				IADDatabase iaddatabase = Dependencies.ReplayAdObjectLookup.DatabaseLookup.FindAdObjectByGuid(mdbGuid);
				if (iaddatabase != null)
				{
					mdbName = iaddatabase.Name;
				}
			}
			if (!string.IsNullOrEmpty(mdbName))
			{
				bool flag = false;
				try
				{
					flag = AmStoreHelper.sm_perfCounterLock.Lock(mdbGuid, AmDbLockReason.UpdatePerfCounter);
					AmTrace.Debug("AmStoreHelper.UpdateCopyRoleIsActivePerfCounter: Database {0} ({1}) CopyRoleIsActive is {2}", new object[]
					{
						mdbGuid,
						mdbName,
						copyRoleIsActive
					});
					ActiveManagerPerfmonInstance instance = ActiveManagerPerfmon.GetInstance(mdbName);
					if (instance != null)
					{
						instance.CopyRoleIsActive.RawValue = (copyRoleIsActive ? 1L : 0L);
					}
					return;
				}
				finally
				{
					if (flag)
					{
						AmStoreHelper.sm_perfCounterLock.Release(mdbGuid, AmDbLockReason.UpdatePerfCounter);
					}
				}
			}
			AmTrace.Error("AmStoreHelper.UpdateCopyRoleIsActivePerfCounter: Perfmon update skipped for {0} since mdbName is null", new object[]
			{
				mdbGuid
			});
		}

		internal static void UpdateNumberOfDatabasesCounter(int countOfDatabases)
		{
			ActiveManagerServerPerfmon.CountOfDatabases.RawValue = (long)countOfDatabases;
		}

		private static Exception DismountWithKillOnTimeout(Guid mdbGuid, UnmountFlags flags, bool retryOnConflict)
		{
			AmTrace.Debug("DismountWithKillOnTimeout {0}", new object[]
			{
				mdbGuid
			});
			Exception result = null;
			bool flag = false;
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)RegistryParameters.AmDismountOrKillTimeoutInSec);
			try
			{
				InvokeWithTimeout.Invoke(delegate()
				{
					AmStoreHelper.RemoteDismount(null, mdbGuid, flags, retryOnConflict);
				}, timeSpan);
			}
			catch (TimeoutException)
			{
				flag = true;
			}
			catch (MapiPermanentException ex)
			{
				result = ex;
			}
			catch (MapiRetryableException ex2)
			{
				result = ex2;
			}
			if (flag)
			{
				AmTrace.Debug("Dismount {0} timedOut after {1}ms", new object[]
				{
					mdbGuid,
					timeSpan.TotalMilliseconds
				});
				ReplayCrimsonEvents.DismountFailedOnTimeout.Log<Guid, TimeSpan>(mdbGuid, timeSpan);
				result = AmStoreServiceMonitor.KillStoreIfRunningBefore(utcNow, "DismountWithKillOnTimeout");
			}
			return result;
		}

		private static void DismountCompletionCallback(IAsyncResult ar)
		{
			AmStoreHelper.DismountDelegate dismountDelegate = (AmStoreHelper.DismountDelegate)ar.AsyncState;
			dismountDelegate.EndInvoke(ar);
		}

		internal static object ForceDismountLocker = new object();

		private static TimeSpan defaultMapiConflictRetryInterval = new TimeSpan(0, 0, 5);

		private static TimeSpan defaultMapiConflictTimeout = new TimeSpan(1, 0, 0);

		private static AmDbLock sm_perfCounterLock = new AmDbLock();

		internal delegate void DismountDelegate(AmServerName serverName, Guid mdbGuid, UnmountFlags flags, bool retryOnConflict);
	}
}
