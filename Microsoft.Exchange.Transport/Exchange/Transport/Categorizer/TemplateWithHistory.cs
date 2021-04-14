using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class TemplateWithHistory : IEquatable<TemplateWithHistory>, IComparable<TemplateWithHistory>
	{
		public TemplateWithHistory()
		{
		}

		public TemplateWithHistory(MessageTemplate template, History history)
		{
			this.template = template;
			this.history = history;
		}

		public MessageTemplate Template
		{
			get
			{
				return this.template;
			}
			set
			{
				this.template = value;
			}
		}

		public static TemplateWithHistory ReadFrom(MailRecipient recipient)
		{
			MessageTemplate messageTemplate = MessageTemplate.ReadFrom(recipient);
			History history = History.ReadFrom(recipient);
			if (messageTemplate.TransmitHistory)
			{
				return new TemplateWithHistory(messageTemplate, history);
			}
			if (history == null)
			{
				return new TemplateWithHistory(messageTemplate, null);
			}
			if (history.RecipientType == RecipientP2Type.Bcc)
			{
				return new TemplateWithHistory(messageTemplate, history);
			}
			return new TemplateWithHistory(messageTemplate, null);
		}

		public void Normalize(ResolverMessage message)
		{
			this.template.Normalize(message);
		}

		public bool Equals(TemplateWithHistory other)
		{
			return this.CompareTo(other) == 0;
		}

		public int CompareTo(TemplateWithHistory other)
		{
			int num = this.template.CompareTo(other.template);
			if (num != 0)
			{
				return num;
			}
			if (!(this.history == null))
			{
				return this.history.CompareTo(other.history);
			}
			if (!(other.history == null))
			{
				return 1;
			}
			return 0;
		}

		public void ApplyTo(TransportMailItem mailItem)
		{
			ResolverMessage message = new ResolverMessage(mailItem.Message, mailItem.MimeSize);
			this.template.ApplyTo(mailItem, message);
			if (this.history != null)
			{
				this.history.WriteTo(mailItem.RootPart.Headers);
			}
		}

		public static readonly TemplateWithHistory Default = new TemplateWithHistory(MessageTemplate.Default, null);

		private MessageTemplate template;

		private History history;
	}
}
