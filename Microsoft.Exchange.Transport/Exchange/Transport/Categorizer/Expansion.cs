using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class Expansion
	{
		public Expansion(Resolver resolver)
		{
			this.resolver = resolver;
			this.template = MessageTemplate.StripHistory;
		}

		public TransportMailItem MailItem
		{
			get
			{
				return this.resolver.MailItem;
			}
		}

		public TaskContext TaskContext
		{
			get
			{
				return this.resolver.TaskContext;
			}
		}

		public ResolverMessage Message
		{
			get
			{
				return this.resolver.Message;
			}
		}

		public Sender Sender
		{
			get
			{
				return this.resolver.Sender;
			}
		}

		public ResolverConfiguration Configuration
		{
			get
			{
				return this.Resolver.Configuration;
			}
		}

		public Resolver Resolver
		{
			get
			{
				return this.resolver;
			}
		}

		public bool BypassChildModeration
		{
			get
			{
				return this.template.BypassChildModeration;
			}
		}

		public static Expansion Resume(MailRecipient recipient, Resolver resolver)
		{
			return new Expansion(resolver)
			{
				template = MessageTemplate.ReadFrom(recipient),
				history = History.ReadFrom(recipient)
			};
		}

		public ExpansionDisposition Expand(MessageTemplate template, HistoryType historyType, RoutingAddress historyAddress, RecipientP2Type recipientP2Type, out Expansion child)
		{
			child = null;
			bool flag = false;
			bool flag2 = true;
			if (this.history != null)
			{
				flag = this.history.Contains(historyAddress, out flag2);
				if (flag)
				{
					ExTraceGlobals.ResolverTracer.TraceDebug<bool>((long)this.GetHashCode(), "loop found in recipient history. reportable = {0}", flag2);
				}
			}
			if (!flag && this.Message.History != null)
			{
				bool flag3;
				flag = this.Message.History.Contains(historyAddress, out flag3);
				flag2 = (flag2 && flag3);
				if (flag)
				{
					ExTraceGlobals.ResolverTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "loop found in message history. reportable = {0}, message reportable = {1}", flag2, flag3);
				}
			}
			if (!flag)
			{
				child = new Expansion(this.resolver);
				child.template = this.template.Derive(template);
				child.history = History.Derive(this.history, historyType, historyAddress, recipientP2Type);
				return ExpansionDisposition.Expanded;
			}
			if (flag2)
			{
				return ExpansionDisposition.ReportableLoopDetected;
			}
			return ExpansionDisposition.NonreportableLoopDetected;
		}

		public void Add(RecipientItem recipient, bool processInOriginalMailItem)
		{
			ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "adding recipient {0}. ", recipient.Recipient.Email);
			string text;
			if (!recipient.Recipient.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.Transport.MessageTemplate", out text))
			{
				this.template.WriteTo(recipient.Recipient);
			}
			if (this.history != null)
			{
				this.history.WriteTo(recipient.Recipient);
			}
			if (processInOriginalMailItem)
			{
				this.Resolver.RecipientStack.Push(recipient);
			}
		}

		public MailRecipient Add(TransportMiniRecipient entry, DsnRequestedFlags dsnRequested)
		{
			return this.Add(entry, dsnRequested, null);
		}

		public MailRecipient Add(TransportMiniRecipient entry, DsnRequestedFlags dsnRequested, string orcpt)
		{
			string primarySmtpAddress = DirectoryItem.GetPrimarySmtpAddress(entry);
			if (primarySmtpAddress == null)
			{
				return null;
			}
			bool processInOriginalMailItem;
			MailRecipient mailRecipient = this.resolver.AddRecipient(primarySmtpAddress, entry, dsnRequested, orcpt, true, out processInOriginalMailItem);
			RecipientItem recipientItem = RecipientItem.Create(mailRecipient);
			this.Add(recipientItem, processInOriginalMailItem);
			recipientItem.AddItemVisited(this);
			return mailRecipient;
		}

		public RecipientItem AddGroupExpansionItem(TransportMiniRecipient entry, DsnRequestedFlags dsnFlags)
		{
			string primarySmtpAddress = DirectoryItem.GetPrimarySmtpAddress(entry);
			if (primarySmtpAddress == null)
			{
				return null;
			}
			bool processInOriginalMailItem;
			MailRecipient recipient = this.resolver.AddRecipient(primarySmtpAddress, entry, dsnFlags, null, false, out processInOriginalMailItem);
			RecipientItem recipientItem = RecipientItem.Create(recipient);
			this.Add(recipientItem, processInOriginalMailItem);
			return recipientItem;
		}

		public void CacheSerializedHistory()
		{
			if (this.history != null)
			{
				this.history.CacheSerializedHistory();
			}
		}

		public void ClearSerializedHistory()
		{
			if (this.history != null)
			{
				this.history.ClearSerializedHistory();
			}
		}

		private Resolver resolver;

		private MessageTemplate template;

		private History history;
	}
}
