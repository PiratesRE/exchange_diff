using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	[Serializable]
	public enum WindowsBuiltInRole
	{
		Administrator = 544,
		User,
		Guest,
		PowerUser,
		AccountOperator,
		SystemOperator,
		PrintOperator,
		BackupOperator,
		Replicator
	}
}
