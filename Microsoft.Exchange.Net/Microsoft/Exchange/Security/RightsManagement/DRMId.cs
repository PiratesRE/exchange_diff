using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[StructLayout(LayoutKind.Sequential)]
	internal class DRMId
	{
		public DRMId()
		{
		}

		public DRMId(string idType, string id)
		{
			this.IdType = idType;
			this.Id = id;
		}

		public uint Version;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string IdType;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string Id;
	}
}
