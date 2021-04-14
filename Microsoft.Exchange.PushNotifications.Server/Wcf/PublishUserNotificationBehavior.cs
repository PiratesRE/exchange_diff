using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.PushNotifications.Server.Wcf
{
	internal class PublishUserNotificationBehavior : WebHttpBehavior
	{
		protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			return new PublishUserNotificationBehavior.PublishUserNotificationMessageFormatter(base.GetRequestDispatchFormatter(operationDescription, endpoint));
		}

		private class PublishUserNotificationMessageFormatter : IDispatchMessageFormatter
		{
			public PublishUserNotificationMessageFormatter(IDispatchMessageFormatter innerFormatter)
			{
				this.innerFormatter = innerFormatter;
			}

			public void DeserializeRequest(Message message, object[] parameters)
			{
				this.innerFormatter.DeserializeRequest(message, parameters);
				RemoteUserNotification remoteUserNotification = parameters[0] as RemoteUserNotification;
				HttpRequestMessageProperty httpRequestMessageProperty = message.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
				if (remoteUserNotification != null && httpRequestMessageProperty != null)
				{
					remoteUserNotification.Payload.SetUserId(httpRequestMessageProperty.Headers["X-PUN-UserId"]);
					remoteUserNotification.Payload.SetTenantId(httpRequestMessageProperty.Headers["X-PUN-TenantId"]);
				}
			}

			public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
			{
				return this.innerFormatter.SerializeReply(messageVersion, parameters, result);
			}

			private IDispatchMessageFormatter innerFormatter;
		}
	}
}
