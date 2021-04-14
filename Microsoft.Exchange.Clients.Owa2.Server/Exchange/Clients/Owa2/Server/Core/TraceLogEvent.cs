using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class TraceLogEvent : ILogEvent
	{
		public TraceLogEvent(string eventId, IMailboxContext userContext, string methodName, string message)
		{
			this.eventId = (eventId ?? string.Empty);
			this.userContext = userContext;
			this.methodName = (methodName ?? string.Empty);
			this.message = (message ?? string.Empty);
		}

		public string EventId
		{
			get
			{
				return this.eventId;
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			string value = string.Empty;
			string value2 = string.Empty;
			string value3 = string.Empty;
			if (this.userContext != null)
			{
				if (this.userContext.ExchangePrincipal != null)
				{
					value = this.userContext.ExchangePrincipal.MailboxInfo.MailboxGuid.ToString();
					value2 = this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
				}
				if (this.userContext.Key != null)
				{
					value3 = this.userContext.Key.ToString();
				}
			}
			return new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("MG", value),
				new KeyValuePair<string, object>("PSA", value2),
				new KeyValuePair<string, object>("MN", this.methodName),
				new KeyValuePair<string, object>(UserContextCookie.UserContextCookiePrefix, value3),
				new KeyValuePair<string, object>("MSG", this.message)
			};
		}

		private readonly string eventId;

		private readonly IMailboxContext userContext;

		private readonly string methodName;

		private readonly string message;
	}
}
