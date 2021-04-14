using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class WorkItem
	{
		public WorkItem(IRuleEvaluationContext context, int actionIndex)
		{
			this.context = context;
			this.rule = this.context.CurrentRule;
			this.actionIndex = actionIndex;
		}

		public IRuleEvaluationContext Context
		{
			get
			{
				return this.context;
			}
		}

		public Rule Rule
		{
			get
			{
				return this.rule;
			}
		}

		public int ActionIndex
		{
			get
			{
				return this.actionIndex;
			}
		}

		public virtual bool ShouldExecuteOnThisStage
		{
			get
			{
				return (this.context.ExecutionStage == ExecutionStage.OnPromotedMessage && this.context.DeliveryFolder == null) || (this.context.ExecutionStage == ExecutionStage.OnDeliveredMessage && this.context.DeliveryFolder != null);
			}
		}

		public abstract ExecutionStage Stage { get; }

		public abstract void Execute();

		protected MessageItem OpenMessage(byte[] messageEntryId)
		{
			if (messageEntryId == null)
			{
				throw new InvalidRuleException(string.Format("Rule {0} is invalid since its message template id is null.", this.rule.Name));
			}
			StoreId itemId = StoreObjectId.FromProviderSpecificId(messageEntryId);
			return Item.BindAsMessage(this.Context.StoreSession, itemId, StoreObjectSchema.ContentConversionProperties);
		}

		protected void SetRecipientsResponsibility(MessageItem message)
		{
			foreach (Recipient recipient in message.Recipients)
			{
				recipient[ItemSchema.Responsibility] = true;
			}
		}

		protected void SubmitMessage(MessageItem message)
		{
			this.context.TraceDebug<string>("Submitting message with subject {0}", message.Subject);
			this.AppendRuleHistory(message);
			using (ISubmissionItem submissionItem = this.Context.GenerateSubmissionItem(message, this))
			{
				submissionItem.Submit();
			}
		}

		protected void SubmitMessage(MessageItem message, ProxyAddress sender, IEnumerable<Participant> recipients)
		{
			this.context.TraceDebug<string>("Submitting message with subject {0}", message.Subject);
			this.AppendRuleHistory(message);
			using (ISubmissionItem submissionItem = this.Context.GenerateSubmissionItem(message, this))
			{
				submissionItem.Submit(sender, recipients);
			}
		}

		protected IList<ProxyAddress> GetSenderProxyAddresses()
		{
			if (this.Context.Sender == null)
			{
				return Array<ProxyAddress>.Empty;
			}
			Result<ADRawEntry> result = this.Context.RecipientCache.FindAndCacheRecipient(this.Context.Sender);
			if (result.Data == null)
			{
				return new ProxyAddress[]
				{
					this.Context.Sender
				};
			}
			List<ProxyAddress> list = new List<ProxyAddress>((IList<ProxyAddress>)result.Data[ADRecipientSchema.EmailAddresses]);
			try
			{
				list.Add(new CustomProxyAddress((CustomProxyAddressPrefix)ProxyAddressPrefix.LegacyDN, (string)result.Data[ADRecipientSchema.LegacyExchangeDN], true));
			}
			catch (ArgumentOutOfRangeException argument)
			{
				this.Context.TraceError<ArgumentOutOfRangeException>("Invalid LegacyDN. Exception: {0}", argument);
			}
			return list;
		}

		private void AppendRuleHistory(MessageItem message)
		{
			byte[] array = this.Context.Message.TryGetProperty(ItemSchema.RuleTriggerHistory) as byte[];
			if (array != null)
			{
				message[ItemSchema.RuleTriggerHistory] = array;
			}
			RuleHistory ruleHistory = message.GetRuleHistory(this.Context.StoreSession);
			ruleHistory.Add(this.rule.ID);
			ruleHistory.Save();
		}

		private IRuleEvaluationContext context;

		private Rule rule;

		private int actionIndex;
	}
}
