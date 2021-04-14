using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class PushNotificationChannel<TNotif> : PushNotificationDisposable where TNotif : PushNotification
	{
		protected PushNotificationChannel(string appId, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("appId", appId);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.AppId = appId;
			this.Tracer = tracer;
		}

		public event EventHandler<InvalidNotificationEventArgs> InvalidNotificationDetected;

		public string AppId { get; private set; }

		public ITracer Tracer { get; private set; }

		public abstract void Send(TNotif notification, CancellationToken cancelToken);

		protected virtual void OnInvalidNotificationFound(InvalidNotificationEventArgs e)
		{
			EventHandler<InvalidNotificationEventArgs> invalidNotificationDetected = this.InvalidNotificationDetected;
			if (invalidNotificationDetected != null)
			{
				invalidNotificationDetected(this, e);
			}
		}

		protected bool WaitUntilDoneOrCancelled(ICancelableAsyncResult asyncResult, PushNotificationChannelContext<TNotif> context, int stepTimeout)
		{
			int num = 0;
			while (!context.IsCancelled)
			{
				if (asyncResult.AsyncWaitHandle.WaitOne(stepTimeout))
				{
					return true;
				}
				num++;
				if (num % 3 == 0)
				{
					this.Tracer.TraceDebug<int>((long)this.GetHashCode(), "[WaitUntilDoneOrCancelled] Still waiting for the operation to finish: '{0}'", num);
				}
			}
			this.Tracer.TraceDebug((long)this.GetHashCode(), "[WaitUntilDoneOrCancelled] Cancellation token was signaled");
			asyncResult.Cancel();
			return false;
		}
	}
}
