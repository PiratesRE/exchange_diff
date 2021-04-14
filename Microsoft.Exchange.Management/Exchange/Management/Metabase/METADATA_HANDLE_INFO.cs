using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Metabase
{
	[StructLayout(LayoutKind.Sequential)]
	internal class METADATA_HANDLE_INFO
	{
		private METADATA_HANDLE_INFO()
		{
			this.dwMDPermissions = 0;
			this.dwMDSystemChangeNumber = 0;
		}

		internal int dwMDPermissions;

		internal int dwMDSystemChangeNumber;
	}
}
