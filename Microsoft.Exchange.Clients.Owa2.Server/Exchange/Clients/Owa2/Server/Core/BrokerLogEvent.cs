using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class BrokerLogEvent : ILogEvent
	{
		public ExchangePrincipal Principal { get; set; }

		public string UserContextKey { get; set; }

		public Exception HandledException { get; set; }

		public string SubscriptionId { get; set; }

		public Guid BrokerSubscriptionId { get; set; }

		public string EventName { get; set; }

		public string EventId
		{
			get
			{
				return "BrokerNotification";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			list.Add(new KeyValuePair<string, object>("MG", (this.Principal == null || this.Principal.MailboxInfo == null) ? string.Empty : this.Principal.MailboxInfo.MailboxGuid.ToString()));
			List<KeyValuePair<string, object>> list2 = list;
			string key = "PSA";
			object value;
			if (this.Principal != null && this.Principal.MailboxInfo != null)
			{
				SmtpAddress primarySmtpAddress = this.Principal.MailboxInfo.PrimarySmtpAddress;
				value = this.Principal.MailboxInfo.PrimarySmtpAddress.ToString();
			}
			else
			{
				value = string.Empty;
			}
			list2.Add(new KeyValuePair<string, object>(key, value));
			list.Add(new KeyValuePair<string, object>("UCK", this.UserContextKey ?? string.Empty));
			list.Add(new KeyValuePair<string, object>("SID", this.SubscriptionId ?? string.Empty));
			list.Add(new KeyValuePair<string, object>("BSID", this.BrokerSubscriptionId));
			list.Add(new KeyValuePair<string, object>("EN", this.EventName ?? string.Empty));
			list.Add(new KeyValuePair<string, object>("EX", (this.HandledException == null) ? string.Empty : this.HandledException.ToString()));
			return list;
		}
	}
}
