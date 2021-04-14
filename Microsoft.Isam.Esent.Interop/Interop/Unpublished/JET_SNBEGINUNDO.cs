using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNBEGINUNDO : JET_RECOVERYCONTROL
	{
		public int cdbinfomisc { get; private set; }

		public JET_DBINFOMISC[] rgdbinfomisc { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNBEGINUNDO({0})", new object[]
			{
				this.cdbinfomisc
			});
		}

		internal void SetFromNativeSnbeginundo(ref NATIVE_SNBEGINUNDO native)
		{
			base.SetFromNativeSnrecoverycontrol(ref native.recoveryControl);
			this.cdbinfomisc = checked((int)native.cdbinfomisc);
			if (this.cdbinfomisc > 0)
			{
				NATIVE_DBINFOMISC7[] array = new NATIVE_DBINFOMISC7[this.cdbinfomisc];
				array[0] = (NATIVE_DBINFOMISC7)Marshal.PtrToStructure(native.rgdbinfomisc, typeof(NATIVE_DBINFOMISC7));
			}
		}
	}
}
