using System;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal interface IMigrationProxyRpc : IDisposable
	{
		int NspiQueryRows(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle);

		int NspiGetRecipient(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle);

		int NspiSetRecipient(int version, byte[] inBlob, out byte[] outBlob);

		int NspiGetGroupMembers(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle);

		int NspiRfrGetNewDSA(int version, byte[] inBlob, out byte[] outBlob);

		void AutodiscoverGetUserSettings(int version, byte[] inBlob, out byte[] outBlob);
	}
}
