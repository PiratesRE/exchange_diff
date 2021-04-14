using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	internal enum DirectoryBackendType : byte
	{
		None = 0,
		AD = 1,
		MServ = 2,
		Mbx = 4,
		SQL = 8
	}
}
