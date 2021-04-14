using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotificationRegistration : ServerObject
	{
		internal NotificationRegistration(NotificationHandler notificationHandler, Logon logon, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, StoreId folderId, StoreId messageId, ServerObjectHandle returnNotificationHandleValue) : base(logon)
		{
			this.notificationSink = logon.NotificationQueue.Register(notificationHandler, flags, eventFlags, wantGlobalScope, folderId, messageId, returnNotificationHandleValue, this.String8Encoding);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationRegistration>(this);
		}

		protected override void InternalDispose()
		{
			this.notificationSink.Dispose();
			base.InternalDispose();
		}

		private readonly NotificationSink notificationSink;
	}
}
