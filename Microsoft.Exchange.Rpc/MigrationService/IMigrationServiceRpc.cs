using System;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal interface IMigrationServiceRpc : IDisposable
	{
		byte[] InvokeMigrationServiceEndPoint(int version, byte[] pInBytes);
	}
}
