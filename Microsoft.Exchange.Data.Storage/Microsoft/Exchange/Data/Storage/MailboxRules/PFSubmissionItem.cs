using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PFSubmissionItem : ISubmissionItem, IDisposable
	{
		public PFSubmissionItem(PFRuleEvaluationContext context, MessageItem item)
		{
			this.context = context;
			this.item = item;
			this.submissionTime = DateTime.UtcNow;
		}

		public string SourceServerFqdn
		{
			get
			{
				return this.context.LocalServerFqdn;
			}
		}

		public IPAddress SourceServerNetworkAddress
		{
			get
			{
				return this.context.LocalServerNetworkAddress;
			}
		}

		public DateTime OriginalCreateTime
		{
			get
			{
				return this.submissionTime;
			}
		}

		public void Submit()
		{
			this.item.Send(SubmitMessageFlags.IgnoreSendAsRight);
		}

		public void Submit(ProxyAddress sender, IEnumerable<Participant> recipients)
		{
			Result<ADRawEntry> result = this.context.RecipientCache.FindAndCacheRecipient(sender);
			if (result.Data != null)
			{
				Participant sender2 = new Participant(result.Data);
				this.item.Sender = sender2;
				foreach (Participant participant in recipients)
				{
					Recipient recipient = this.item.Recipients.Add(participant, RecipientItemType.Bcc);
					recipient[ItemSchema.Responsibility] = true;
				}
				this.item.Send(SubmitMessageFlags.IgnoreSendAsRight);
				return;
			}
			if (ProviderError.NotFound == result.Error)
			{
				this.context.TraceError<ProxyAddress>("Public folder rule submission: Sender '{0}' doesn't exist in AD.", sender);
			}
			else
			{
				this.context.TraceError<ProxyAddress, ProviderError>("Public folder rule submission: Sender '{0}' look up failed: {1}", sender, result.Error);
			}
			this.context.TraceDebug("Public folder rule submission: Message will not be submitted.");
		}

		public void Dispose()
		{
		}

		private PFRuleEvaluationContext context;

		private MessageItem item;

		private DateTime submissionTime;
	}
}
