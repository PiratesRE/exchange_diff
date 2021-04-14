using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationPublisherManager : PushNotificationDisposable
	{
		public PushNotificationPublisherManager(IPushNotificationOptics optics = null)
		{
			this.optics = (optics ?? PushNotificationOptics.Default);
			this.registeredPublishers = new Dictionary<string, PushNotificationPublisherBase>();
			this.unsuitablePublishers = new HashSet<string>();
		}

		public void RegisterPublisher(PushNotificationPublisherBase publisher)
		{
			ArgumentValidator.ThrowIfNull("publisher", publisher);
			if (this.HasPublisher(publisher.AppId))
			{
				throw new ArgumentException("Publisher already registered :" + publisher.AppId);
			}
			this.registeredPublishers[publisher.AppId] = publisher;
		}

		public virtual void RegisterUnsuitablePublisher(string publisherName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("publisherName", publisherName);
			if (this.HasPublisher(publisherName))
			{
				throw new ArgumentException("Publisher already registered :" + publisherName);
			}
			this.unsuitablePublishers.Add(publisherName);
		}

		public bool HasPublisher(string appId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("appId", appId);
			return this.registeredPublishers.ContainsKey(appId) || this.HasUnsuitablePublisher(appId);
		}

		public bool HasUnsuitablePublisher(string appId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("appId", appId);
			return this.unsuitablePublishers.Contains(appId);
		}

		public void Publish(MulticastNotification notification, PushNotificationPublishingContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			this.optics.ReportReceived(notification, context);
			if (notification != null && notification.IsValid)
			{
				using (IEnumerator<Notification> enumerator = notification.GetFragments().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Notification notification2 = enumerator.Current;
						this.Publish(notification2, context);
					}
					return;
				}
			}
			this.optics.ReportDiscardedByValidation(notification);
		}

		public void Publish(Notification notification, PushNotificationPublishingContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			this.optics.ReportReceived(notification, context);
			if (notification == null || string.IsNullOrWhiteSpace(notification.AppId))
			{
				this.optics.ReportDiscardedByValidation(notification);
				return;
			}
			PushNotificationPublisherBase pushNotificationPublisherBase;
			if (this.registeredPublishers.TryGetValue(notification.AppId, out pushNotificationPublisherBase))
			{
				try
				{
					pushNotificationPublisherBase.Publish(notification, context);
					return;
				}
				catch (ObjectDisposedException)
				{
					this.optics.ReportDiscardedByDisposedPublisher(notification);
					return;
				}
			}
			if (notification.AppId == "MonitoringProbeAppId")
			{
				PushNotificationsMonitoring.PublishSuccessNotification("EnterpriseNotificationProcessed", "");
				return;
			}
			if (this.HasUnsuitablePublisher(notification.AppId))
			{
				this.optics.ReportDiscardedByUnsuitablePublisher(notification);
				return;
			}
			this.optics.ReportDiscardedByUnknownPublisher(notification);
		}

		public void Publish(PushNotification notification)
		{
			if (notification == null || string.IsNullOrWhiteSpace(notification.AppId))
			{
				this.optics.ReportDiscardedByValidation(notification, null);
				return;
			}
			PushNotificationPublisherBase pushNotificationPublisherBase;
			if (this.registeredPublishers.TryGetValue(notification.AppId, out pushNotificationPublisherBase))
			{
				try
				{
					pushNotificationPublisherBase.Publish(notification);
					return;
				}
				catch (ObjectDisposedException)
				{
					this.optics.ReportDiscardedByDisposedPublisher(notification);
					return;
				}
			}
			if (this.HasUnsuitablePublisher(notification.AppId))
			{
				this.optics.ReportDiscardedByUnsuitablePublisher(notification);
				return;
			}
			this.optics.ReportDiscardedByUnknownPublisher(notification);
		}

		public override string ToString()
		{
			return string.Format("{{publishers:[{0}]; unsuitablePublishers:[{1}]}}", string.Join<PushNotificationPublisherBase>("; ", this.registeredPublishers.Values), string.Join<string>("; ", this.unsuitablePublishers));
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				foreach (PushNotificationPublisherBase pushNotificationPublisherBase in this.registeredPublishers.Values)
				{
					pushNotificationPublisherBase.Dispose();
				}
				stopwatch.Stop();
				PushNotificationsCrimsonEvents.PushNotificationPublisherDisposed.Log<long>(stopwatch.ElapsedMilliseconds);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PushNotificationPublisherManager>(this);
		}

		private Dictionary<string, PushNotificationPublisherBase> registeredPublishers;

		private HashSet<string> unsuitablePublishers;

		private IPushNotificationOptics optics;
	}
}
