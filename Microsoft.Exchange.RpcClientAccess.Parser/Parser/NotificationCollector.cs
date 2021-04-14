using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NotificationCollector : BaseObject
	{
		internal NotificationCollector(int maxSize)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.maxSize = maxSize;
				this.notifyResults = new List<NotifyResult>();
				disposeGuard.Success();
			}
		}

		internal int MaxSize
		{
			get
			{
				return this.maxSize;
			}
		}

		internal List<NotifyResult> NotifyResults
		{
			get
			{
				return this.notifyResults;
			}
		}

		public bool TryAddNotification(ServerObjectHandle notificationHandle, byte logonId, Encoding string8Encoding, Notification notification)
		{
			base.CheckDisposed();
			if (notification == null)
			{
				throw new ArgumentNullException("notification");
			}
			NotifyResult notifyResult = new NotifyResult(notificationHandle, logonId, string8Encoding, notification);
			notifyResult.Serialize(this.writer);
			if (this.writer.Position > (long)this.maxSize)
			{
				if (ExTraceGlobals.NotificationDeliveryTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.NotificationDeliveryTracer.Information<string>(50457L, "Fail pack notif: {0}", notification.ToString());
				}
				return false;
			}
			if (ExTraceGlobals.NotificationDeliveryTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.NotificationDeliveryTracer.Information<string>(32676L, "Pack notif: {0}", notification.ToString());
			}
			this.notifyResults.Add(notifyResult);
			return true;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationCollector>(this);
		}

		protected override void InternalDispose()
		{
			this.writer.Dispose();
			base.InternalDispose();
		}

		private readonly int maxSize;

		private readonly CountWriter writer = new CountWriter();

		private List<NotifyResult> notifyResults;
	}
}
