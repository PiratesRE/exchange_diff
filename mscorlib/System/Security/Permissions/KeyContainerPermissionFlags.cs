using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum KeyContainerPermissionFlags
	{
		NoFlags = 0,
		Create = 1,
		Open = 2,
		Delete = 4,
		Import = 16,
		Export = 32,
		Sign = 256,
		Decrypt = 512,
		ViewAcl = 4096,
		ChangeAcl = 8192,
		AllFlags = 13111
	}
}
