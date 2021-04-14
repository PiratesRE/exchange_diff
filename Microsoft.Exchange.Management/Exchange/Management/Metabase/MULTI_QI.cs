using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Metabase
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct MULTI_QI : IDisposable
	{
		public void SetIID(Guid IID)
		{
			this.pIID = Marshal.AllocCoTaskMem(Marshal.SizeOf(IID));
			Marshal.StructureToPtr(IID, this.pIID, false);
		}

		public void Dispose()
		{
			if (this.pIID != IntPtr.Zero)
			{
				Marshal.DestroyStructure(this.pIID, typeof(Guid));
				Marshal.FreeCoTaskMem(this.pIID);
			}
		}

		private IntPtr pIID;

		public IntPtr pItf;

		public ulong hr;
	}
}
