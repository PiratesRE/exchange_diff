using System;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.HA
{
	public interface IEsebackCallbacks
	{
		int PrepareInstanceForBackup(IntPtr context, JET_INSTANCE instance, IntPtr reserved);

		int DoneWithInstanceForBackup(IntPtr context, JET_INSTANCE instance, uint complete, IntPtr reserved);

		int GetDatabasesInfo(IntPtr context, out MINSTANCE_BACKUP_INFO[] instances, uint reserved);

		int IsSGReplicated(IntPtr context, JET_INSTANCE instance, out bool isReplicated, out Guid guid, out MLOGSHIP_INFO[] info);

		int ServerAccessCheck();

		int Trace(string data);
	}
}
