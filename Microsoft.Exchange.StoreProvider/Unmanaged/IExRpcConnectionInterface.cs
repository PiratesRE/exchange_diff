using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExRpcConnectionInterface : IExInterface, IDisposeTrackable, IDisposable
	{
		int OpenMsgStore(int ulFlags, long ullOpenFlags, string lpszMailboxDN, byte[] pbMailboxGuid, byte[] pbMdbGuid, out string lppszWrongServerDN, IntPtr hToken, byte[] pSidUser, byte[] pSidPrimaryGroup, string lpszUserDN, int ulLcidString, int ulLcidSort, int ulCpid, bool unifiedLogon, string lpszApplicationId, byte[] pbTenantHint, int cbTenantHint, out IExMapiStore iMsgStore);

		int SendAuxBuffer(int ulFlags, int cbAuxBuffer, byte[] pbAuxBuffer, int fForceSend);

		int FlushRPCBuffer(bool fForceSend);

		int GetServerVersion(out int pulVersionMajor, out int pulVersionMinor, out int pulBuildMajor, out int pulBuildMinor);

		int IsDead(out bool pfDead);

		int RpcSentToServer(out bool pfRpcSent);

		int IsMapiMT(out bool pfMapiMT);

		int IsConnectedToMapiServer(out bool pfConnectedToMapiServer);

		void ClearStorePerRPCStats();

		uint GetStorePerRPCStats(out PerRpcStats pPerRpcPerformanceStatistics);

		void ClearRPCStats();

		int GetRPCStats(out RpcStats pRpcStats);

		int SetInternalAccess();

		int ClearInternalAccess();

		void CheckForNotifications();
	}
}
