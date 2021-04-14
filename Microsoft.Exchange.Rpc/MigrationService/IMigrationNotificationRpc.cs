using System;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal interface IMigrationNotificationRpc : IDisposable
	{
		byte[] UpdateMigrationRequest(int version, byte[] pInBytes);
	}
}
