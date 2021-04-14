using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationChannelContext<TNotif> where TNotif : PushNotification
	{
		public PushNotificationChannelContext(TNotif notification, CancellationToken cancellationToken, ITracer tracer)
		{
			this.Notification = notification;
			this.CancellationToken = cancellationToken;
			this.Tracer = tracer;
		}

		public string AppId
		{
			get
			{
				TNotif notification = this.Notification;
				return notification.AppId;
			}
		}

		public TNotif Notification { get; private set; }

		public bool IsActive
		{
			get
			{
				return this.Notification != null;
			}
		}

		public bool IsCancelled
		{
			get
			{
				return this.CancellationToken.IsCancellationRequested;
			}
		}

		public CancellationToken CancellationToken { get; private set; }

		protected ITracer Tracer { get; set; }

		public void Done()
		{
			this.Tracer.TraceDebug<TNotif>((long)this.GetHashCode(), "[Done] Done with notification '{0}'", this.Notification);
			this.Notification = default(TNotif);
		}

		public void Drop(string traces = null)
		{
			this.Tracer.TraceWarning<TNotif>((long)this.GetHashCode(), "[Drop] Dropping notification '{0}'", this.Notification);
			PushNotificationTracker.ReportDropped(this.Notification, traces ?? string.Empty);
			this.Notification = default(TNotif);
		}

		public override string ToString()
		{
			TNotif notification = this.Notification;
			return notification.ToString();
		}
	}
}
