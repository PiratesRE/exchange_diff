using System;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	internal delegate bool CreateException(string exceptionType, ref Exception exception);
}
