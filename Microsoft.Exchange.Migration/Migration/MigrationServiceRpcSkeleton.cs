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
	internal class MigrationServiceRpcSkeleton : MigrationServiceRpcServer
	{
		public override byte[] InvokeMigrationServiceEndPoint(int version, byte[] inputBlob)
		{
			byte[] result;
			try
			{
				result = MigrationServiceRpcSkeleton.InvokeMigrationServiceEndPoint(this.implementation, version, inputBlob);
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = null;
			}
			return result;
		}

		internal static void StartServer(IMigrationService implementation)
		{
			lock (MigrationServiceRpcSkeleton.startStopLock)
			{
				if (MigrationServiceRpcSkeleton.instance != null)
				{
					throw new InvalidOperationException("Cannot invoke TryStartServer when another instance of MigrationServiceRpcSkeleton already exists and is running");
				}
				MigrationServiceRpcSkeleton.instance = (MigrationServiceRpcSkeleton)RpcServerBase.RegisterServer(typeof(MigrationServiceRpcSkeleton), MigrationServiceRpcSkeleton.GetSecuritySettings(), 1, false, (uint)ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationServiceRpcSkeletonMaxThreads"));
				MigrationServiceRpcSkeleton.instance.implementation = implementation;
				MigrationLogger.Log(MigrationEventType.Verbose, "MigrationService RPC server started", new object[0]);
			}
		}

		internal static void StopServer()
		{
			lock (MigrationServiceRpcSkeleton.startStopLock)
			{
				if (MigrationServiceRpcSkeleton.instance != null)
				{
					RpcServerBase.StopServer(MigrationServiceRpcServer.RpcIntfHandle);
					MigrationServiceRpcSkeleton.instance.implementation = null;
					MigrationServiceRpcSkeleton.instance = null;
					MigrationLogger.Log(MigrationEventType.Verbose, "MigrationService RPC server stopped and deregistered from RPC", new object[0]);
				}
			}
		}

		internal static byte[] InvokeMigrationServiceEndPoint(IMigrationService implementation, int version, byte[] inputBlob)
		{
			if (implementation == null)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "MigrationService RPC server will return {0} since the IsServiceStarted has not been set.", new object[]
				{
					MigrationServiceRpcResultCode.ServerShutdown
				});
				return MigrationRpcHelper.CreateResponsePropertyCollection(MigrationServiceRpcResultCode.ServerShutdown).GetBytes();
			}
			if (version > 2)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "MigrationService RPC server will return {0} since the current version is {1} and the requested version was {2}.", new object[]
				{
					MigrationServiceRpcResultCode.VersionMismatchError,
					2,
					version
				});
				return MigrationRpcHelper.CreateResponsePropertyCollection(MigrationServiceRpcResultCode.VersionMismatchError).GetBytes();
			}
			MdbefPropertyCollection mdbefPropertyCollection = null;
			try
			{
				mdbefPropertyCollection = MdbefPropertyCollection.Create(inputBlob, 0, inputBlob.Length);
			}
			catch (MdbefException exception)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception, "MigrationService RPC server will return {0} since inputblob could not be parsed", new object[]
				{
					MigrationServiceRpcResultCode.PropertyBagMissingError
				});
				return MigrationRpcHelper.CreateResponsePropertyCollection(MigrationServiceRpcResultCode.PropertyBagMissingError).GetBytes();
			}
			MigrationServiceRpcMethodCode migrationServiceRpcMethodCode = (MigrationServiceRpcMethodCode)((int)mdbefPropertyCollection[2684420099U]);
			MdbefPropertyCollection mdbefPropertyCollection2;
			switch (migrationServiceRpcMethodCode)
			{
			case MigrationServiceRpcMethodCode.CreateSyncSubscription:
				mdbefPropertyCollection2 = MigrationServiceRpcSkeleton.CreateSyncSubscription(implementation, version, mdbefPropertyCollection);
				break;
			case MigrationServiceRpcMethodCode.UpdateSyncSubscription:
				mdbefPropertyCollection2 = MigrationServiceRpcSkeleton.UpdateSyncSubscription(implementation, mdbefPropertyCollection);
				break;
			case MigrationServiceRpcMethodCode.GetSyncSubscriptionState:
				mdbefPropertyCollection2 = MigrationServiceRpcSkeleton.GetSyncSubscriptionState(implementation, mdbefPropertyCollection);
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
			return mdbefPropertyCollection2.GetBytes();
		}

		private static MdbefPropertyCollection CreateSyncSubscription(IMigrationService implementation, int version, MdbefPropertyCollection inputArgs)
		{
			MdbefPropertyCollection result2;
			try
			{
				AbstractCreateSyncSubscriptionArgs args = AbstractCreateSyncSubscriptionArgs.Create(inputArgs, version);
				CreateSyncSubscriptionResult result = null;
				MigrationServiceHelper.SafeInvokeImplMethod(delegate
				{
					result = implementation.CreateSyncSubscription(args);
				}, MigrationServiceRpcMethodCode.CreateSyncSubscription);
				result2 = result.Marshal();
			}
			catch (MigrationServiceRpcException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex, "CreateSyncSubscription failed", new object[0]);
				result2 = new CreateSyncSubscriptionResult(MigrationServiceRpcMethodCode.CreateSyncSubscription, ex.ResultCode, ex.Message).Marshal();
			}
			return result2;
		}

		private static MdbefPropertyCollection UpdateSyncSubscription(IMigrationService implementation, MdbefPropertyCollection inputArgs)
		{
			MdbefPropertyCollection result2;
			try
			{
				UpdateSyncSubscriptionArgs args = UpdateSyncSubscriptionArgs.UnMarshal(inputArgs);
				UpdateSyncSubscriptionResult result = null;
				MigrationServiceHelper.SafeInvokeImplMethod(delegate
				{
					result = implementation.UpdateSyncSubscription(args);
				}, MigrationServiceRpcMethodCode.UpdateSyncSubscription);
				result2 = result.Marshal();
			}
			catch (MigrationServiceRpcException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex, "UpdateSyncSubscription failed", new object[0]);
				result2 = new UpdateSyncSubscriptionResult(MigrationServiceRpcMethodCode.UpdateSyncSubscription, ex.ResultCode, ex.Message).Marshal();
			}
			return result2;
		}

		private static MdbefPropertyCollection GetSyncSubscriptionState(IMigrationService implementation, MdbefPropertyCollection inputArgs)
		{
			MdbefPropertyCollection result2;
			try
			{
				GetSyncSubscriptionStateArgs args = GetSyncSubscriptionStateArgs.UnMarshal(inputArgs);
				GetSyncSubscriptionStateResult result = null;
				MigrationServiceHelper.SafeInvokeImplMethod(delegate
				{
					result = implementation.GetSyncSubscriptionState(args);
				}, MigrationServiceRpcMethodCode.GetSyncSubscriptionState);
				result2 = result.Marshal();
			}
			catch (MigrationServiceRpcException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex, "GetSyncSubscriptionState failed", new object[0]);
				result2 = new GetSyncSubscriptionStateResult(MigrationServiceRpcMethodCode.GetSyncSubscriptionState, ex.ResultCode, ex.Message).Marshal();
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

		private const int CurrentVersion = 2;

		private static readonly object startStopLock = new object();

		private static MigrationServiceRpcSkeleton instance;

		private IMigrationService implementation;
	}
}
