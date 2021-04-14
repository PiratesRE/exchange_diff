using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.PushNotifications;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotifyAppWipe : ServiceCommand<bool>
	{
		public NotifyAppWipe(CallContext callContext, DataWipeReason wipeReason) : base(callContext)
		{
			this.wipeReason = wipeReason;
		}

		protected override bool InternalExecute()
		{
			ExTraceGlobals.AppWipeTracer.TraceDebug(0L, "[NotifyAppWipe.InternalExecute] Acquiring the UserContext.");
			UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			ExTraceGlobals.AppWipeTracer.TraceDebug(0L, string.Format("[NotifyAppWipe.InternalExecute] Wipe reason {0}.", this.wipeReason));
			if (base.CallContext.IsRemoteWipeRequested)
			{
				ExTraceGlobals.AppWipeTracer.TraceDebug(0L, "[NotifyAppWipe.InternalExecute] Remote wipe completed.");
				base.CallContext.MarkRemoteWipeAsAcknowledged();
			}
			UnsubscribeToPushNotificationRequest request = new UnsubscribeToPushNotificationRequest(new PushNotificationSubscription
			{
				AppId = "UNKNOWN_APPWIPE",
				DeviceNotificationId = "UNKNOWN_APPWIPE",
				DeviceNotificationType = PushNotificationPlatform.APNS.ToString()
			});
			UnsubscribeToPushNotification unsubscribeToPushNotification = new UnsubscribeToPushNotification(base.CallContext, request);
			unsubscribeToPushNotification.Execute();
			return true;
		}

		private readonly DataWipeReason wipeReason;
	}
}
