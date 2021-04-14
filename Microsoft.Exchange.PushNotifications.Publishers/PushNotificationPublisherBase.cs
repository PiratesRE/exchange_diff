using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class PushNotificationPublisherBase : PushNotificationDisposable
	{
		protected PushNotificationPublisherBase(PushNotificationPublisherSettings settings, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			settings.Validate();
			this.Settings = settings;
			this.Tracer = tracer;
		}

		public string AppId
		{
			get
			{
				return this.Settings.AppId;
			}
		}

		public PushNotificationPublisherSettings Settings { get; private set; }

		public ITracer Tracer { get; private set; }

		public abstract void Publish(Notification notification, PushNotificationPublishingContext context);

		public abstract void Publish(PushNotification notification);

		public override string ToString()
		{
			if (this.toStringCache == null)
			{
				this.toStringCache = string.Format("{{appId:{0}}}", this.AppId);
			}
			return this.toStringCache;
		}

		private string toStringCache;
	}
}
