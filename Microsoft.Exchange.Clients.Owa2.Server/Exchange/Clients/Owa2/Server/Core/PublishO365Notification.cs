using System;
using Microsoft.Exchange.Data.PushNotifications;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PublishO365Notification : ServiceCommand<IAsyncResult>
	{
		public PublishO365Notification(CallContext callContext, O365Notification notification, AsyncCallback asyncCallback, object asyncState) : base(callContext)
		{
			this.notification = notification;
			this.asyncCallback = asyncCallback;
			this.asyncState = asyncState;
		}

		protected override IAsyncResult InternalExecute()
		{
			this.asyncResult = new ServiceAsyncResult<bool>();
			this.asyncResult.AsyncState = this.asyncState;
			this.asyncResult.AsyncCallback = this.asyncCallback;
			if (PushNotificationsCrimsonEvents.MonitoringO365Notification.IsEnabled(PushNotificationsCrimsonEvent.Provider) && this.notification.ChannelId.StartsWith("::AE82E53440744F2798C276818CE8BD5C::"))
			{
				PushNotificationsCrimsonEvents.MonitoringO365Notification.Log<string>(this.notification.Data);
			}
			this.asyncResult.Data = true;
			this.asyncResult.Complete(this);
			return this.asyncResult;
		}

		private O365Notification notification;

		private AsyncCallback asyncCallback;

		private object asyncState;

		private ServiceAsyncResult<bool> asyncResult;
	}
}
