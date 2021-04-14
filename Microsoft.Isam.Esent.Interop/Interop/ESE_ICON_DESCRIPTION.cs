using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	public class ESE_ICON_DESCRIPTION
	{
		public int ulSize
		{
			get
			{
				if (this.pvData == null)
				{
					return 0;
				}
				return this.pvData.Length;
			}
		}

		public byte[] pvData { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ESE_ICON_DESCRIPTION({0})", new object[]
			{
				(this.pvData == null) ? 0 : this.pvData.Length
			});
		}

		internal void SetFromNative(ref NATIVE_ESE_ICON_DESCRIPTION native)
		{
			this.pvData = new byte[native.ulSize];
			Marshal.Copy(native.pvData, this.pvData, 0, this.pvData.Length);
		}

		internal NATIVE_ESE_ICON_DESCRIPTION GetNativeEseIconDescription()
		{
			int num = this.pvData.Length;
			IntPtr intPtr = IntPtr.Zero;
			if (this.pvData.Length > 0)
			{
				intPtr = Marshal.AllocHGlobal(num);
				Marshal.Copy(this.pvData, 0, intPtr, num);
			}
			return new NATIVE_ESE_ICON_DESCRIPTION
			{
				ulSize = (uint)num,
				pvData = intPtr
			};
		}
	}
}
