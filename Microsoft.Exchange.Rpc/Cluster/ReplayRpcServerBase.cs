using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal abstract class ReplayRpcServerBase : RpcServerBase
	{
		public abstract RpcErrorExceptionInfo RequestSuspend(Guid guid, string suspendComment);

		public abstract RpcErrorExceptionInfo RequestResume(Guid guid);

		public abstract RpcErrorExceptionInfo GetCopyStatus(RpcGetDatabaseCopyStatusFlags collectionFlags, Guid[] dbGuids, ref RpcDatabaseCopyStatus[] dbStatuses);

		public abstract RpcErrorExceptionInfo GetCopyStatus2(RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, out int nCopyStatusesReturned, out byte[] dbStatuses);

		public abstract RpcErrorExceptionInfo GetCopyStatusBasic(RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, ref RpcDatabaseCopyStatusBasic[] dbStatuses);

		public abstract RpcErrorExceptionInfo RunConfigurationUpdater();

		public abstract RpcErrorExceptionInfo GetDagNetworkConfig(ref byte[] dagNetConfig);

		public abstract RpcErrorExceptionInfo SetDagNetwork(byte[] networkChange);

		public abstract RpcErrorExceptionInfo SetDagNetworkConfig(byte[] networkConfigChange);

		public abstract RpcErrorExceptionInfo RemoveDagNetwork(byte[] deleteRequest);

		public abstract RpcErrorExceptionInfo CancelDbSeed(Guid dbGuid);

		public abstract RpcErrorExceptionInfo EndDbSeed(Guid dbGuid);

		public abstract RpcErrorExceptionInfo RpcsGetDatabaseSeedStatus(Guid dbGuid, ref RpcSeederStatus pSeederStatus);

		public abstract RpcErrorExceptionInfo RpcsPrepareDatabaseSeedAndBegin(RpcSeederArgs seederArgs, ref RpcSeederStatus seederStatus);

		public abstract RpcErrorExceptionInfo RequestSuspend2(Guid guid, string suspendComment, uint flags);

		public abstract RpcErrorExceptionInfo RequestSuspend3(Guid guid, string suspendComment, uint flags, uint actionInitiator);

		public abstract RpcErrorExceptionInfo RequestResume2(Guid guid, uint flags);

		public abstract RpcErrorExceptionInfo RpcsDisableReplayLag2(Guid dbGuid, string disableReason, ActionInitiatorType actionInitiator);

		public abstract RpcErrorExceptionInfo RpcsEnableReplayLag2(Guid dbGuid, ActionInitiatorType actionInitiator);

		public abstract RpcErrorExceptionInfo RpcsNotifyChangedReplayConfiguration(Guid guid, [MarshalAs(UnmanagedType.U1)] bool waitForCompletion, [MarshalAs(UnmanagedType.U1)] bool exitAfterEnqueueing, [MarshalAs(UnmanagedType.U1)] bool isHighPriority, int changeHint);

		public abstract RpcErrorExceptionInfo RpcsInstallFailoverClustering(ref string verboseLog);

		public abstract RpcErrorExceptionInfo RpcsCreateCluster(string clusterName, string firstNodeName, string[] ipAddresses, uint[] rgNetMasks, ref string verboseLog);

		public abstract RpcErrorExceptionInfo RpcsDestroyCluster(string clusterName, ref string verboseLog);

		public abstract RpcErrorExceptionInfo RpcsAddNodeToCluster(string newNode, ref string verboseLog);

		public abstract RpcErrorExceptionInfo RpcsEvictNodeFromCluster(string convictedNode, ref string verboseLog);

		public abstract RpcErrorExceptionInfo RpcsForceCleanupNode(ref string verboseLog);

		public abstract RpcErrorExceptionInfo GetCopyStatusWithHealthState(RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, ref RpcCopyStatusContainer container);

		public ReplayRpcServerBase()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IReplayRpc_v3_0_s_ifspec;

		public byte[] m_sdBaseSystemBinaryForm;
	}
}
