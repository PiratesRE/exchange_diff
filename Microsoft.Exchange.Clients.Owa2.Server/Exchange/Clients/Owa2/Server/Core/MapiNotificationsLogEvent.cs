using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class MapiNotificationsLogEvent : ILogEvent
	{
		internal MapiNotificationsLogEvent(ExchangePrincipal exchangePrincipal, string userContext, MapiNotificationHandlerBase notificationHandler, string methodName)
		{
			if (!Globals.Owa2ServerUnitTestsHook && exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			if (notificationHandler == null)
			{
				throw new ArgumentNullException("notificationHandler");
			}
			this.exchangePrincipal = exchangePrincipal;
			this.userContext = (userContext ?? string.Empty);
			this.notificationHandler = notificationHandler;
			this.MethodName = (methodName ?? string.Empty);
		}

		public bool InvalidNotification { get; set; }

		public bool NullNotification { get; set; }

		public bool RefreshPayloadSent { get; set; }

		public Exception HandledException { get; set; }

		public string MethodName { get; set; }

		public string EventId
		{
			get
			{
				return "Notifications";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			ICollection<KeyValuePair<string, object>> collection = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>(UserContextCookie.UserContextCookiePrefix, this.userContext),
				new KeyValuePair<string, object>("MG", this.exchangePrincipal.MailboxInfo.MailboxGuid.ToString()),
				new KeyValuePair<string, object>("PSA", this.exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()),
				new KeyValuePair<string, object>("MN", this.MethodName),
				new KeyValuePair<string, object>("Miss", this.notificationHandler.MissedNotifications.ToString()),
				new KeyValuePair<string, object>("NRS", this.notificationHandler.NeedToReinitSubscriptions.ToString()),
				new KeyValuePair<string, object>("NRP", this.notificationHandler.NeedRefreshPayload.ToString()),
				new KeyValuePair<string, object>("IN", this.InvalidNotification.ToString()),
				new KeyValuePair<string, object>("NN", this.NullNotification.ToString()),
				new KeyValuePair<string, object>("RPS", this.RefreshPayloadSent.ToString()),
				new KeyValuePair<string, object>("D", this.notificationHandler.IsDisposed.ToString())
			};
			if (this.HandledException != null)
			{
				collection.Add(new KeyValuePair<string, object>("Ex", this.HandledException.ToString()));
			}
			return collection;
		}

		private readonly ExchangePrincipal exchangePrincipal;

		private readonly string userContext;

		private readonly MapiNotificationHandlerBase notificationHandler;
	}
}
