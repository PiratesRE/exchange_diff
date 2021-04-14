using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Management.Metabase
{
	[ComVisible(false)]
	internal sealed class SafeMetadataHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeMetadataHandle() : base(true)
		{
		}

		internal SafeMetadataHandle(IntPtr handle, IMSAdminBase adminBase) : base(true)
		{
			base.SetHandle(handle);
			this.adminBase = adminBase;
		}

		public static SafeMetadataHandle MetadataMasterRootHandle
		{
			get
			{
				return new SafeMetadataHandle(IntPtr.Zero, null);
			}
		}

		protected override bool ReleaseHandle()
		{
			if (this.adminBase != null)
			{
				bool result = this.adminBase.CloseKey(this.handle) == 0;
				this.adminBase = null;
				return result;
			}
			return false;
		}

		private IMSAdminBase adminBase;
	}
}
