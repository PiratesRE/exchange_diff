using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MigrationService;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationNotificationRpcSkeleton : MigrationNotificationRpcServer
	{
		public override byte[] UpdateMigrationRequest(int version, byte[] inputBlob)
		{
			return MigrationNotificationRpcSkeleton.UpdateMigrationRequest(this.implementation, version, inputBlob);
		}

		internal static void StartServer(IMigrationNotification implementation)
		{
			lock (MigrationNotificationRpcSkeleton.startStopLock)
			{
				if (MigrationNotificationRpcSkeleton.instance != null)
				{
					throw new InvalidOperationException("Cannot Start the server since it is already started. Call StopServer() before calling TryStartServer()");
				}
				MigrationNotificationRpcSkeleton.instance = (MigrationNotificationRpcSkeleton)RpcServerBase.RegisterServer(typeof(MigrationNotificationRpcSkeleton), MigrationNotificationRpcSkeleton.GetSecuritySettings(), 1, false, (uint)ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationNotificationRpcSkeletonMaxThreads"));
				MigrationNotificationRpcSkeleton.instance.implementation = implementation;
				MigrationLogger.Log(MigrationEventType.Verbose, "MigrationService RPC server started", new object[0]);
			}
		}

		internal static void StopServer()
		{
			lock (MigrationNotificationRpcSkeleton.startStopLock)
			{
				if (MigrationNotificationRpcSkeleton.instance != null)
				{
					RpcServerBase.StopServer(MigrationNotificationRpcServer.RpcIntfHandle);
					MigrationNotificationRpcSkeleton.instance.implementation = null;
					MigrationNotificationRpcSkeleton.instance = null;
					MigrationLogger.Log(MigrationEventType.Verbose, "MigrationNotification RPC Server stopped and deregistered from RPC", new object[0]);
				}
			}
		}

		internal static byte[] UpdateMigrationRequest(IMigrationNotification implementation, int version, byte[] inputBlob)
		{
			byte[] result;
			try
			{
				if (implementation == null)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "MigrationNotification RPC Server will return {0} since the the server has not been started.", new object[]
					{
						MigrationServiceRpcResultCode.ServerShutdown
					});
					result = MigrationRpcHelper.CreateResponsePropertyCollection(MigrationServiceRpcResultCode.ServerShutdown).GetBytes();
				}
				else if (version != 1)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "MigrationNotification RPC Server will return {0} since the current version is {1} and the requested version was {2}.", new object[]
					{
						MigrationServiceRpcResultCode.VersionMismatchError,
						1,
						version
					});
					result = MigrationRpcHelper.CreateResponsePropertyCollection(MigrationServiceRpcResultCode.VersionMismatchError).GetBytes();
				}
				else
				{
					MdbefPropertyCollection mdbefPropertyCollection = null;
					try
					{
						mdbefPropertyCollection = MdbefPropertyCollection.Create(inputBlob, 0, inputBlob.Length);
					}
					catch (MdbefException exception)
					{
						MigrationLogger.Log(MigrationEventType.Error, exception, "MigrationNotification RPC Server will return {0} since inputblob could not be parsed", new object[]
						{
							MigrationServiceRpcResultCode.PropertyBagMissingError
						});
						return MigrationRpcHelper.CreateResponsePropertyCollection(MigrationServiceRpcResultCode.PropertyBagMissingError).GetBytes();
					}
					MigrationServiceRpcMethodCode migrationServiceRpcMethodCode = (MigrationServiceRpcMethodCode)((int)mdbefPropertyCollection[2684420099U]);
					MdbefPropertyCollection mdbefPropertyCollection2;
					switch (migrationServiceRpcMethodCode)
					{
					case MigrationServiceRpcMethodCode.RegisterMigrationBatch:
						mdbefPropertyCollection2 = MigrationNotificationRpcSkeleton.RegisterMigrationBatch(implementation, mdbefPropertyCollection);
						break;
					case MigrationServiceRpcMethodCode.SubscriptionStatusChanged:
						mdbefPropertyCollection2 = MigrationNotificationRpcSkeleton.SubscriptionStatusChanged(implementation, mdbefPropertyCollection);
						break;
					default:
						MigrationLogger.Log(MigrationEventType.Error, "MigrationService RPC server will return {0} since an invalid method {1} was requested", new object[]
						{
							MigrationServiceRpcResultCode.UnknownMethodError,
							migrationServiceRpcMethodCode
						});
						mdbefPropertyCollection2 = MigrationRpcHelper.CreateResponsePropertyCollection(MigrationServiceRpcResultCode.UnknownMethodError);
						break;
					}
					result = mdbefPropertyCollection2.GetBytes();
				}
			}
			catch (Exception exception2)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception2);
				result = null;
			}
			return result;
		}

		private static MdbefPropertyCollection SubscriptionStatusChanged(IMigrationNotification implementation, MdbefPropertyCollection inputArgs)
		{
			MdbefPropertyCollection result2;
			try
			{
				UpdateMigrationRequestArgs args = UpdateMigrationRequestArgs.UnMarshal(inputArgs);
				UpdateMigrationRequestResult result = null;
				MigrationServiceHelper.SafeInvokeImplMethod(delegate
				{
					result = implementation.UpdateMigrationRequest(args);
				}, MigrationServiceRpcMethodCode.SubscriptionStatusChanged);
				MdbefPropertyCollection mdbefPropertyCollection = result.Marshal();
				result2 = mdbefPropertyCollection;
			}
			catch (MigrationServiceRpcException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex, "SubscriptionStatusChanged failed", new object[0]);
				result2 = new UpdateMigrationRequestResult(MigrationServiceRpcMethodCode.SubscriptionStatusChanged, ex.ResultCode, ex.Message).Marshal();
			}
			return result2;
		}

		private static MdbefPropertyCollection RegisterMigrationBatch(IMigrationNotification implementation, MdbefPropertyCollection inputArgs)
		{
			MdbefPropertyCollection result2;
			try
			{
				RegisterMigrationBatchArgs args = RegisterMigrationBatchArgs.UnMarshal(inputArgs);
				RegisterMigrationBatchResult result = null;
				MigrationServiceHelper.SafeInvokeImplMethod(delegate
				{
					result = implementation.RegisterMigrationBatch(args);
				}, MigrationServiceRpcMethodCode.RegisterMigrationBatch);
				result2 = result.Marshal();
			}
			catch (MigrationServiceRpcException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex, "RegisterMigrationBatch failed", new object[0]);
				result2 = new RegisterMigrationBatchResult(MigrationServiceRpcMethodCode.RegisterMigrationBatch, ex.ResultCode, ex.Message).Marshal();
			}
			return result2;
		}

		private static FileSecurity GetSecuritySettings()
		{
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			FileSystemAccessRule accessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
			FileSecurity fileSecurity = new FileSecurity();
			fileSecurity.SetOwner(securityIdentifier);
			fileSecurity.SetAccessRule(accessRule);
			return fileSecurity;
		}

		private const int CurrentVersion = 1;

		private static readonly object startStopLock = new object();

		private static MigrationNotificationRpcSkeleton instance;

		private IMigrationNotification implementation;
	}
}
