using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SignInLogEvent : ILogEvent
	{
		public SignInLogEvent(IMailboxContext userContext, string userContextKey, UserContextStatistics userContextStatistics, Uri requestUri)
		{
			if (userContextStatistics == null)
			{
				throw new ArgumentNullException("userContextStatistics");
			}
			this.userContext = userContext;
			this.userContextKey = (userContextKey ?? string.Empty);
			this.userContextStatistics = userContextStatistics;
			this.requestUri = requestUri;
		}

		public string EventId
		{
			get
			{
				return "SignIn";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{
					UserContextCookie.UserContextCookiePrefix,
					this.userContextKey
				},
				{
					"ActID",
					(currentActivityScope == null) ? Guid.Empty : currentActivityScope.ActivityId
				},
				{
					"C",
					this.userContextStatistics.Created.ToString(CultureInfo.InvariantCulture)
				},
				{
					"CT",
					this.userContextStatistics.AcquireLatency
				},
				{
					"EPT",
					this.userContextStatistics.ExchangePrincipalCreationTime
				},
				{
					"MR",
					this.userContextStatistics.MiniRecipientCreationTime
				},
				{
					"SKU",
					this.userContextStatistics.SKUCapabilityTestTime
				},
				{
					"IL",
					this.userContextStatistics.CookieCreated ? 1 : 0
				},
				{
					"Err",
					(int)this.userContextStatistics.Error
				},
				{
					"CAN",
					this.requestUri.AbsolutePath
				}
			};
			if (this.userContext != null && this.userContext.ExchangePrincipal != null)
			{
				dictionary.Add("MG", this.userContext.ExchangePrincipal.MailboxInfo.MailboxGuid);
				dictionary.Add("PSA", ExtensibleLogger.FormatPIIValue(this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()));
			}
			return dictionary;
		}

		private const string SignInEventId = "SignIn";

		private readonly IMailboxContext userContext;

		private readonly string userContextKey;

		private readonly UserContextStatistics userContextStatistics;

		private readonly Uri requestUri;
	}
}
