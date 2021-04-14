using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal delegate int CallbackDelegate(StatusMessage status, int hr, IntPtr pvParam, IntPtr pvContext);
}
