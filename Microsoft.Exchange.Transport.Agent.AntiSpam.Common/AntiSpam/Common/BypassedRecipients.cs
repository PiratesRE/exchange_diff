using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	internal class BypassedRecipients
	{
		public BypassedRecipients(MultiValuedProperty<SmtpAddress> recipients, AddressBook addressBook)
		{
			if (recipients != null)
			{
				this.bypassedRecipients = new Dictionary<RoutingAddress, bool>();
				foreach (SmtpAddress smtpAddress in recipients)
				{
					this.bypassedRecipients.Add((RoutingAddress)smtpAddress.ToString(), true);
				}
			}
			this.addressBook = addressBook;
		}

		public AddressBook AddressBook
		{
			get
			{
				return this.addressBook;
			}
		}

		public int NumBypassedRecipients(EnvelopeRecipientCollection recipients)
		{
			int num = 0;
			ReadOnlyCollection<AddressBookEntry> readOnlyCollection = null;
			if (recipients == null || recipients.Count == 0)
			{
				return 0;
			}
			CommonUtils.TryAddressBookFind(this.addressBook, recipients, out readOnlyCollection);
			for (int i = 0; i < recipients.Count; i++)
			{
				EnvelopeRecipient envelopeRecipient = recipients[i];
				AddressBookEntry addressBookEntry = (readOnlyCollection != null) ? readOnlyCollection[i] : null;
				if (this.IsBypassed(envelopeRecipient.Address, addressBookEntry))
				{
					num++;
				}
			}
			return num;
		}

		public bool IsBypassed(RoutingAddress recipient)
		{
			AddressBookEntry addressBookEntry = null;
			if (this.addressBook != null)
			{
				CommonUtils.TryAddressBookFind(this.addressBook, recipient, out addressBookEntry);
			}
			return this.IsBypassed(recipient, addressBookEntry);
		}

		public bool IsBypassed(RoutingAddress recipient, AddressBookEntry addressBookEntry)
		{
			return (this.bypassedRecipients != null && this.bypassedRecipients.ContainsKey(recipient)) || (addressBookEntry != null && addressBookEntry.AntispamBypass);
		}

		public void RemoveNonBypassedRecipients(MailItem mailItem, bool reject, SmtpResponse response, string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, LogEntry logEntry)
		{
			List<EnvelopeRecipient> list = new List<EnvelopeRecipient>(mailItem.Recipients.Count);
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			foreach (EnvelopeRecipient envelopeRecipient in recipients)
			{
				if (!this.IsBypassed(envelopeRecipient.Address))
				{
					list.Add(envelopeRecipient);
				}
			}
			if (reject)
			{
				AgentLog.Instance.LogRejectRecipients(agentName, eventTopic, eventArgs, smtpSession, mailItem, list, response, logEntry);
				using (List<EnvelopeRecipient>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						EnvelopeRecipient recipient = enumerator2.Current;
						mailItem.Recipients.Remove(recipient, DsnType.Failure, response);
					}
					return;
				}
			}
			AgentLog.Instance.LogDeleteRecipients(agentName, eventTopic, eventArgs, smtpSession, mailItem, list, logEntry);
			foreach (EnvelopeRecipient recipient2 in list)
			{
				mailItem.Recipients.Remove(recipient2);
			}
		}

		private Dictionary<RoutingAddress, bool> bypassedRecipients;

		private AddressBook addressBook;
	}
}
