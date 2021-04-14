using System;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal delegate void UserNotificationEventHandler(PlatformCallInfo callinfo, byte[] messageBody, UserNotificationEventContext context);
}
