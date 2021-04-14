using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNOPENLOG : JET_RECOVERYCONTROL
	{
		public int lGenNext { get; internal set; }

		public bool fCurrentLog { get; internal set; }

		public JET_OpenLog eReason { get; internal set; }

		public string wszLogFile { get; internal set; }

		public int cdbinfomisc { get; internal set; }

		public JET_DBINFOMISC[] rgdbinfomisc { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNOPENLOG({0})", new object[]
			{
				this.wszLogFile
			});
		}

		internal void SetFromNativeSnopenlog(ref NATIVE_SNOPENLOG native)
		{
			base.SetFromNativeSnrecoverycontrol(ref native.recoveryControl);
			checked
			{
				this.lGenNext = (int)native.lGenNext;
				this.fCurrentLog = (native.fCurrentLog != 0);
				this.eReason = (JET_OpenLog)native.eReason;
				this.wszLogFile = Marshal.PtrToStringUni(native.wszLogFile);
				this.cdbinfomisc = (int)native.cdbinfomisc;
				if (this.cdbinfomisc > 0)
				{
					NATIVE_DBINFOMISC7[] array = new NATIVE_DBINFOMISC7[this.cdbinfomisc];
					array[0] = (NATIVE_DBINFOMISC7)Marshal.PtrToStructure(native.rgdbinfomisc, typeof(NATIVE_DBINFOMISC7));
					this.rgdbinfomisc = new JET_DBINFOMISC[this.cdbinfomisc];
					this.rgdbinfomisc[0] = new JET_DBINFOMISC();
					this.rgdbinfomisc[0].SetFromNativeDbinfoMisc(ref array[0]);
				}
			}
		}
	}
}
