using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal struct MailItemTraceFilter : IDisposable
	{
		public MailItemTraceFilter(TransportMailItem mailItem)
		{
			this.filteredMailItem = null;
			this.traceEnabled = false;
			this.traceConfig = null;
			this.SetMailItem(mailItem);
		}

		public void SetMailItem(TransportMailItem mailItem)
		{
			if (this.filteredMailItem != null)
			{
				throw new InvalidOperationException("Transport MailItem should be set only once!");
			}
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			this.traceConfig = TraceConfigurationSingleton<TransportTraceConfiguration>.Instance;
			this.filteredMailItem = mailItem;
			if (!this.traceConfig.FilteredTracingEnabled)
			{
				return;
			}
			this.traceEnabled = false;
			if (!this.filteredMailItem.ExtendedProperties.TryGetValue<bool>("Microsoft.Exchange.Transport.MailItemTracing", out this.traceEnabled) || !this.traceEnabled)
			{
				this.traceEnabled = (this.IsSenderFiltered(mailItem.From.ToString()) || this.IsSubjectFiltered(mailItem.Subject) || this.IsAnyRecipientFiltered(mailItem.Recipients.AllUnprocessed));
				if (this.traceEnabled)
				{
					this.filteredMailItem.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.MailItemTracing", true);
				}
			}
			if (this.traceEnabled)
			{
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
		}

		public void Dispose()
		{
			if (this.traceEnabled)
			{
				BaseTrace.CurrentThreadSettings.DisableTracing();
			}
		}

		private static bool MatchFilters(List<string> filters, string stringToMatch)
		{
			if (string.IsNullOrEmpty(stringToMatch))
			{
				return false;
			}
			foreach (string value in filters)
			{
				if (stringToMatch.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsSenderFiltered(string sender)
		{
			return MailItemTraceFilter.MatchFilters(this.traceConfig.FilteredSenders, sender) || MailItemTraceFilter.MatchFilters(this.traceConfig.FilteredUsers, sender);
		}

		private bool IsAnyRecipientFiltered(IEnumerable<MailRecipient> recipients)
		{
			foreach (MailRecipient mailRecipient in recipients)
			{
				if (MailItemTraceFilter.MatchFilters(this.traceConfig.FilteredRecipients, mailRecipient.Email.ToString()) || MailItemTraceFilter.MatchFilters(this.traceConfig.FilteredUsers, mailRecipient.Email.ToString()))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsSubjectFiltered(string subject)
		{
			return MailItemTraceFilter.MatchFilters(this.traceConfig.FilteredSubjects, subject);
		}

		public const string MailItemTracingPropName = "Microsoft.Exchange.Transport.MailItemTracing";

		private bool traceEnabled;

		private TransportMailItem filteredMailItem;

		private TransportTraceConfiguration traceConfig;
	}
}
