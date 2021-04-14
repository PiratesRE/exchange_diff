using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CanaryLogEvent : ILogEvent
	{
		public CanaryLogEvent(HttpContext httpContext, IMailboxContext userContext, CanaryLogEvent.CanaryStatus canaryStatus, DateTime creationTime, string logData)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(httpContext);
			if (userContextCookie != null)
			{
				this.userContextCookie = userContextCookie.CookieValue;
			}
			else
			{
				this.userContextCookie = string.Empty;
			}
			if (userContext != null && userContext.ExchangePrincipal != null)
			{
				this.primarySmtpAddress = userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
				this.mailboxGuid = userContext.ExchangePrincipal.MailboxInfo.MailboxGuid.ToString();
			}
			else
			{
				this.primarySmtpAddress = string.Empty;
				this.mailboxGuid = string.Empty;
			}
			if (logData != null)
			{
				this.logData = logData;
			}
			else
			{
				this.logData = string.Empty;
			}
			this.action = (httpContext.Request.Headers[OWADispatchOperationSelector.Action] ?? string.Empty);
			this.canaryStatus = canaryStatus;
			this.creationTime = creationTime;
		}

		public string EventId
		{
			get
			{
				return "Canary";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("MG", this.mailboxGuid),
				new KeyValuePair<string, object>("PSA", ExtensibleLogger.FormatPIIValue(this.primarySmtpAddress)),
				new KeyValuePair<string, object>(UserContextCookie.UserContextCookiePrefix, this.userContextCookie),
				new KeyValuePair<string, object>("CN.A", this.action),
				new KeyValuePair<string, object>("CN.S", ((int)this.canaryStatus).ToString("X")),
				new KeyValuePair<string, object>("CN.T", this.creationTime),
				new KeyValuePair<string, object>("CN.L", this.logData)
			};
		}

		private readonly string primarySmtpAddress;

		private readonly string mailboxGuid;

		private readonly string action;

		private readonly string userContextCookie;

		private readonly CanaryLogEvent.CanaryStatus canaryStatus;

		private readonly DateTime creationTime;

		private readonly string logData;

		[Flags]
		public enum CanaryStatus
		{
			None = 0,
			IsCanaryRenewed = 16,
			IsCanaryAfterLogonRequest = 32,
			IsCanaryValid = 64,
			IsCanaryAboutToExpire = 128,
			IsCanaryNeeded = 256
		}
	}
}
