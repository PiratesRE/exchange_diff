using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsChannelContext : PushNotificationChannelContext<ApnsNotification>
	{
		public ApnsChannelContext(ApnsNotification notification, CancellationToken cancellationToken, ITracer tracer, ApnsChannelSettings settings) : base(notification, cancellationToken, tracer)
		{
			this.Settings = settings;
		}

		public bool IsRetryable
		{
			get
			{
				return this.ConnectRetries < this.Settings.ConnectRetryMax && this.AuthenticateCount < this.Settings.AuthenticateRetryMax;
			}
		}

		public int CurrentRetryDelay
		{
			get
			{
				return this.Settings.ConnectRetryDelay * this.ConnectRetries;
			}
		}

		private ApnsChannelSettings Settings { get; set; }

		private int ConnectRetries { get; set; }

		private int AuthenticateCount { get; set; }

		public void IncrementConnectRetries()
		{
			this.ConnectRetries++;
			base.Tracer.TraceDebug<int, ApnsNotification>((long)this.GetHashCode(), "[IncrementConnectRetries] Connect attempts are now {0} for '{1}'", this.ConnectRetries, base.Notification);
		}

		public void ResetConnectRetries()
		{
			this.ConnectRetries = 0;
			base.Tracer.TraceDebug<ApnsNotification>((long)this.GetHashCode(), "[ResetConnectRetries] Connect attempts reset for '{0}'", base.Notification);
		}

		public void IncrementAuthenticateRetries()
		{
			this.AuthenticateCount++;
			base.Tracer.TraceDebug<int, ApnsNotification>((long)this.GetHashCode(), "[IncrementAuthenticateRetries] Authenticate attempts are now {0} for '{1}'", this.AuthenticateCount, base.Notification);
		}
	}
}
