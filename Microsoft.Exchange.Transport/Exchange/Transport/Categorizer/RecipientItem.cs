using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class RecipientItem
	{
		protected RecipientItem(MailRecipient recipient)
		{
			this.recipient = recipient;
		}

		public RoutingAddress Email
		{
			get
			{
				return this.recipient.Email;
			}
		}

		public bool TopLevelRecipient
		{
			get
			{
				return this.topLevelRecipient;
			}
		}

		public MailRecipient Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		public static RecipientItem Create(MailRecipient recipient)
		{
			return RecipientItem.Create(recipient, false);
		}

		public static RecipientItem Create(MailRecipient recipient, bool isTopLevelRecipient)
		{
			if (!Resolver.IsResolved(recipient))
			{
				return null;
			}
			RecipientItem recipientItem = DirectoryItem.Create(recipient);
			if (recipientItem == null)
			{
				recipientItem = new OneOffItem(recipient);
			}
			recipientItem.topLevelRecipient = isTopLevelRecipient;
			return recipientItem;
		}

		public void Process(Expansion expansion)
		{
			this.PreProcess(expansion);
			if (this.Recipient.IsProcessed)
			{
				return;
			}
			this.PostProcess(expansion);
		}

		public abstract void PreProcess(Expansion expansion);

		public abstract void PostProcess(Expansion expansion);

		public virtual void AddItemVisited(Expansion expansion)
		{
		}

		public override string ToString()
		{
			return this.Email.ToString();
		}

		protected void ApplyTemplate(MessageTemplate template)
		{
			MessageTemplate messageTemplate = MessageTemplate.ReadFrom(this.recipient);
			MessageTemplate messageTemplate2 = messageTemplate.Derive(template);
			messageTemplate2.WriteTo(this.recipient);
		}

		protected void FailRecipient(SmtpResponse response)
		{
			Resolver.FailRecipient(this.recipient, response);
		}

		protected Expansion Expand(Expansion parent, HistoryType historyType)
		{
			return this.Expand(parent, MessageTemplate.Default, historyType);
		}

		protected Expansion Expand(Expansion parent, MessageTemplate template, HistoryType historyType)
		{
			Expansion result = null;
			int num;
			RecipientP2Type recipientP2Type;
			if (!this.recipient.ExtendedProperties.TryGetValue<int>("Microsoft.Exchange.Transport.RecipientP2Type", out num))
			{
				recipientP2Type = RecipientP2Type.Unknown;
			}
			else
			{
				recipientP2Type = (RecipientP2Type)num;
			}
			switch (parent.Expand(template, historyType, this.recipient.Email, recipientP2Type, out result))
			{
			case ExpansionDisposition.Expanded:
				return result;
			case ExpansionDisposition.NonreportableLoopDetected:
				this.recipient.Ack(AckStatus.SuccessNoDsn, AckReason.SilentExpansionLoopDetected);
				return null;
			}
			this.FailRecipient(AckReason.ExpansionLoopDetected);
			if (Resolver.PerfCounters != null)
			{
				Resolver.PerfCounters.LoopRecipientsTotal.Increment();
			}
			return null;
		}

		protected virtual bool CheckDeliveryRestrictions(Expansion expansion)
		{
			long num;
			RestrictionCheckResult restrictionCheckResult = DeliveryRestriction.CheckRestriction(this as RestrictedItem, expansion.Sender, expansion.Resolver.IsAuthenticated, expansion.MailItem.IsJournalReport(), expansion.Message.OriginalMessageSize, expansion.Configuration.MaxReceiveSize, expansion.Resolver.Configuration.PrivilegedSenders, null, expansion.MailItem.OrganizationId, out num);
			ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "Restriction Check returns {0}: recipient {1} sender {2} authenticated {3} stream size {4}", new object[]
			{
				(int)restrictionCheckResult,
				this.Recipient,
				expansion.Sender,
				expansion.Resolver.IsAuthenticated,
				expansion.Message.OriginalMessageSize
			});
			if (ADRecipientRestriction.Failed(restrictionCheckResult))
			{
				if (restrictionCheckResult == (RestrictionCheckResult)2147483649U)
				{
					this.recipient.AddDsnParameters("MaxRecipMessageSizeInKB", num);
					this.recipient.AddDsnParameters("CurrentMessageSizeInKB", expansion.Message.OriginalMessageSize >> 10);
				}
				this.FailRecipient(DeliveryRestriction.GetResponseForResult(restrictionCheckResult));
				return false;
			}
			return true;
		}

		protected T GetProperty<T>(string name)
		{
			T result;
			this.Recipient.ExtendedProperties.TryGetValue<T>(name, out result);
			return result;
		}

		protected T GetProperty<T>(string name, T defaultValue)
		{
			return this.Recipient.ExtendedProperties.GetValue<T>(name, defaultValue);
		}

		protected IList<ItemT> GetListProperty<ItemT>(string name)
		{
			ReadOnlyCollection<ItemT> result;
			this.Recipient.ExtendedProperties.TryGetListValue<ItemT>(name, out result);
			return result;
		}

		private MailRecipient recipient;

		private bool topLevelRecipient;
	}
}
