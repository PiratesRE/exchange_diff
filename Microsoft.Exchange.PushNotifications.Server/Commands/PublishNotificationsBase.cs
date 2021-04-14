using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Exchange.PushNotifications.Server.Core;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal abstract class PublishNotificationsBase<TRequest> : ServiceCommand<TRequest, ServiceCommandResultNone>
	{
		public PublishNotificationsBase(TRequest request, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(request, asyncCallback, asyncState)
		{
			this.PublisherManager = publisherManager;
			this.NotificationSource = base.Description;
		}

		private protected string NotificationSource { protected get; private set; }

		private protected PushNotificationPublisherManager PublisherManager { protected get; private set; }

		protected sealed override ServiceCommandResultNone InternalExecute(TimeSpan queueAndDelay, TimeSpan totalTime)
		{
			this.Publish();
			return ServiceCommandResultNone.Instance;
		}

		protected abstract void Publish();

		protected override ResourceKey[] InternalGetResources()
		{
			return new ResourceKey[]
			{
				ProcessorResourceKey.Local
			};
		}
	}
}
