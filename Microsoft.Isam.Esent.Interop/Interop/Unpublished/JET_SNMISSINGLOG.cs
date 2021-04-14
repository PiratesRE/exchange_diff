using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNMISSINGLOG : JET_RECOVERYCONTROL
	{
		public int lGenMissing { get; internal set; }

		public bool fCurrentLog { get; internal set; }

		public JET_RECOVERYACTIONS eNextAction { get; internal set; }

		public string wszLogFile { get; internal set; }

		public int cdbinfomisc { get; internal set; }

		public JET_DBINFOMISC[] rgdbinfomisc { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNMISSINGLOG({0})", new object[]
			{
				this.wszLogFile
			});
		}

		internal void SetFromNativeSnmissinglog(ref NATIVE_SNMISSINGLOG native)
		{
			base.SetFromNativeSnrecoverycontrol(ref native.recoveryControl);
			checked
			{
				this.lGenMissing = (int)native.lGenMissing;
				this.fCurrentLog = (native.fCurrentLog != 0);
				this.eNextAction = (JET_RECOVERYACTIONS)native.eNextAction;
				this.wszLogFile = Marshal.PtrToStringUni(native.wszLogFile);
				this.cdbinfomisc = (int)native.cdbinfomisc;
				if (this.cdbinfomisc > 0)
				{
					NATIVE_DBINFOMISC7[] array = new NATIVE_DBINFOMISC7[this.cdbinfomisc];
					array[0] = (NATIVE_DBINFOMISC7)Marshal.PtrToStructure(native.rgdbinfomisc, typeof(NATIVE_DBINFOMISC7));
				}
			}
		}
	}
}
