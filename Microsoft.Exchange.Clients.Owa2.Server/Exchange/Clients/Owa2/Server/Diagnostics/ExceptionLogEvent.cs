using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ExceptionLogEvent : ILogEvent
	{
		public ExceptionLogEvent(string eventId, IMailboxContext userContext, Exception exception)
		{
			if (eventId == null)
			{
				throw new ArgumentNullException("eventId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			this.eventId = eventId;
			this.userContext = userContext;
			this.exceptionText = exception.ToString();
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
			return new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("PSA", ExtensibleLogger.FormatPIIValue(this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString())),
				new KeyValuePair<string, object>(UserContextCookie.UserContextCookiePrefix, this.userContext.Key.UserContextId.ToString(CultureInfo.InvariantCulture)),
				new KeyValuePair<string, object>("EX", this.exceptionText)
			};
		}

		private readonly IMailboxContext userContext;

		private readonly string eventId;

		private readonly string exceptionText;
	}
}
