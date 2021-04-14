using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal class MigrationRpcExceptionHelper
	{
		public static void ThrowRpcException(int status, string routineName)
		{
			throw new SyncMigrationRpcTransientException(string.Format("Error 0x{0:x} ({2}) from {1}", status, routineName, new Win32Exception(status).Message), status);
		}
	}
}
