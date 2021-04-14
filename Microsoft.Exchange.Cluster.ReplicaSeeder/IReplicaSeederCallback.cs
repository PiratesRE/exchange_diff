using System;
using System.Runtime.InteropServices;

public interface IReplicaSeederCallback
{
	void ReportProgress(string edbName, long edbSize, long bytesRead, long bytesWritten);

	[return: MarshalAs(UnmanagedType.U1)]
	bool IsBackupCancelled();
}
