using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Rpc.Common;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	internal abstract class AmRpcServerBase : RpcServerBase
	{
		public abstract RpcErrorExceptionInfo RpcsGetServerForDatabase(Guid guid, ref AmDbStatusInfo2 dbInfo);

		public abstract RpcErrorExceptionInfo MountDatabase(Guid guid, int storeFlags, int amFlags, int mountDialoverride);

		public abstract RpcErrorExceptionInfo DismountDatabase(Guid guid, int flags);

		public abstract RpcErrorExceptionInfo MoveDatabaseEx(Guid guid, int flags, int dismountFlags, int mountDialOverride, string fromServer, string targetServer, [MarshalAs(UnmanagedType.U1)] bool tryOtherHealthyServers, int skipValidationChecks, int actionCode, string moveComment, ref AmDatabaseMoveResult databaseMoveResult);

		public abstract RpcErrorExceptionInfo AttemptCopyLastLogsDirect(Guid guid, int mountDialOverride, int numRetries, int e00timeoutMs, int networkIOtimeoutMs, int networkConnecttimeoutMs, string sourceServer, int actionCode, int skipValidationChecks, [MarshalAs(UnmanagedType.U1)] bool mountPending, string uniqueOperationId, int subactionAttemptNumber, ref AmAcllReturnStatus acllStatus);

		public abstract RpcErrorExceptionInfo MountDatabaseDirect(Guid guid, AmMountArg mountArg);

		public abstract RpcErrorExceptionInfo DismountDatabaseDirect(Guid guid, AmDismountArg dismountArg);

		public abstract RpcErrorExceptionInfo IsRunning();

		public abstract RpcErrorExceptionInfo GetPrimaryActiveManager(ref AmPamInfo pamInfo);

		public abstract RpcErrorExceptionInfo ServerSwitchOver(string sourceServer);

		public abstract RpcErrorExceptionInfo GetActiveManagerRole(ref AmRole amRole, ref string errorMessage);

		public abstract RpcErrorExceptionInfo CheckThirdPartyListener(ref bool healthy, ref string errorMessage);

		public abstract RpcErrorExceptionInfo ReportSystemEvent(int eventCode, string reportingServer);

		public abstract RpcErrorExceptionInfo RemountDatabase(Guid guid, int mountFlags, int mountDialOverride, string fromServer);

		public abstract RpcErrorExceptionInfo ServerMoveAllDatabases(string sourceServer, string targetServer, int mountFlags, int dismountFlags, int mountDialOverride, int tryOtherHealthyServers, int skipValidationChecks, int actionCode, string moveComment, string componentName, ref List<AmDatabaseMoveResult> databaseMoveResults);

		public abstract RpcErrorExceptionInfo RpcsGetAutomountConsensusState(ref int automountConsensusState);

		public abstract RpcErrorExceptionInfo RpcsSetAutomountConsensusState(int automountConsensusState);

		public abstract RpcErrorExceptionInfo AmRefreshConfiguration(int refreshFlags, int maxSecondsToWait);

		public abstract RpcErrorExceptionInfo ReportServiceKill(string serviceName, string serverName, string timeStampStrInUtc);

		public abstract RpcErrorExceptionInfo GetDeferredRecoveryEntries(ref List<AmDeferredRecoveryEntry> entries);

		public abstract RpcErrorExceptionInfo GenericRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo);

		public AmRpcServerBase()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IActiveManagerRpc_v3_0_s_ifspec;
	}
}
