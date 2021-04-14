using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNSIGNALERROR : JET_RECOVERYCONTROL
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNSIGNALERROR()", new object[0]);
		}

		internal void SetFromNativeSnsignalerror(ref NATIVE_SNSIGNALERROR native)
		{
			base.SetFromNativeSnrecoverycontrol(ref native.recoveryControl);
		}
	}
}
