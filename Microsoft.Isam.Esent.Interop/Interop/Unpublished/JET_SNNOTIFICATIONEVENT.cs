using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNNOTIFICATIONEVENT : JET_RECOVERYCONTROL
	{
		public int EventID { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNNOTIFICATIONEVENT({0})", new object[]
			{
				this.EventID
			});
		}

		internal void SetFromNativeSnnotificationevent(ref NATIVE_SNNOTIFICATIONEVENT native)
		{
			base.SetFromNativeSnrecoverycontrol(ref native.recoveryControl);
			this.EventID = native.EventID;
		}
	}
}
