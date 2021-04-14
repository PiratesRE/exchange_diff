using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport
{
	internal class MailRecipientCollectionWrapper : EnvelopeRecipientCollection
	{
		internal MailRecipientCollectionWrapper(TransportMailItem mailItem, IMExSession mexSession, bool canAdd)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			this.mailItem = mailItem;
			this.mexSession = mexSession;
			this.canAdd = canAdd;
		}

		public override int Count
		{
			get
			{
				return this.mailItem.Recipients.CountUnprocessed();
			}
		}

		public override bool CanAdd
		{
			get
			{
				return this.canAdd;
			}
		}

		public override EnvelopeRecipient this[int index]
		{
			get
			{
				MailRecipient nonDeletedItem = this.GetNonDeletedItem(index);
				if (nonDeletedItem == null)
				{
					throw new IndexOutOfRangeException(Strings.IndexOutOfBounds(index, this.Count));
				}
				return new MailRecipientWrapper(nonDeletedItem, this.mailItem);
			}
		}

		public override void Clear()
		{
			foreach (MailRecipient mailRecipient in this.mailItem.Recipients.AllUnprocessed)
			{
				this.TrackRecipientFail(mailRecipient.Email, mailRecipient.SmtpResponse, null);
				mailRecipient.ReleaseFromActive();
			}
		}

		public override bool Contains(RoutingAddress address)
		{
			foreach (MailRecipient mailRecipient in this.mailItem.Recipients.AllUnprocessed)
			{
				if (mailRecipient.Email.Equals(address))
				{
					return true;
				}
			}
			return false;
		}

		public override void Add(RoutingAddress address)
		{
			if (!this.canAdd)
			{
				throw new NotSupportedException();
			}
			if (!address.IsValid)
			{
				throw new ArgumentException("address", Strings.InvalidSmtpAddress(address.ToString()));
			}
			string agentName = null;
			if (this.mexSession != null)
			{
				agentName = (this.mexSession.ExecutingAgentName ?? "Agent");
			}
			this.mailItem.Recipients.Add((string)address);
			MessageTrackingLog.TrackRecipientAdd(MessageTrackingSource.AGENT, this.mailItem, address, null, agentName);
		}

		public override bool Remove(EnvelopeRecipient recipient)
		{
			return this.Remove(recipient, true);
		}

		public override int Remove(RoutingAddress address)
		{
			int num = 0;
			for (int i = this.mailItem.Recipients.Count - 1; i >= 0; i--)
			{
				MailRecipient mailRecipient = this.mailItem.Recipients[i];
				if (mailRecipient.IsActive && !mailRecipient.IsProcessed && mailRecipient.Email.CompareTo(address) == 0)
				{
					this.TrackRecipientFail(mailRecipient.Email, mailRecipient.SmtpResponse, null);
					if (this.mailItem.Recipients.Remove(mailRecipient))
					{
						num++;
					}
				}
			}
			return num;
		}

		public override bool Remove(EnvelopeRecipient recipient, DsnType dsnType, SmtpResponse smtpResponse)
		{
			return this.Remove(recipient, dsnType, smtpResponse, true, null);
		}

		public override bool Remove(EnvelopeRecipient recipient, DsnType dsnType, SmtpResponse smtpResponse, string sourceContext)
		{
			return this.Remove(recipient, dsnType, smtpResponse, true, sourceContext);
		}

		public override EnvelopeRecipientCollection.Enumerator GetEnumerator()
		{
			return new EnvelopeRecipientCollection.Enumerator(this.mailItem.Recipients.AllUnprocessed, (object a) => new MailRecipientWrapper((MailRecipient)a, this.mailItem));
		}

		public bool Remove(EnvelopeRecipient recipient, bool trackRecipientFail)
		{
			MailRecipientWrapper mailRecipientWrapper = recipient as MailRecipientWrapper;
			if (mailRecipientWrapper == null)
			{
				throw new ArgumentException("Does not match any valid recipient type.", "recipient");
			}
			if (!this.mailItem.Recipients.Contains(mailRecipientWrapper.MailRecipient) || !mailRecipientWrapper.MailRecipient.IsActive || mailRecipientWrapper.MailRecipient.IsProcessed)
			{
				return false;
			}
			if (trackRecipientFail)
			{
				this.TrackRecipientFail(mailRecipientWrapper.MailRecipient.Email, mailRecipientWrapper.MailRecipient.SmtpResponse, null);
			}
			mailRecipientWrapper.MailRecipient.ReleaseFromActive();
			return true;
		}

		public bool Remove(EnvelopeRecipient recipient, DsnType dsnType, SmtpResponse smtpResponse, bool trackRecipientFail, string sourceContext)
		{
			MailRecipientWrapper mailRecipientWrapper = recipient as MailRecipientWrapper;
			if (mailRecipientWrapper == null)
			{
				throw new ArgumentException("Does not match any valid recipient type.", "recipient");
			}
			if (!this.mailItem.Recipients.Contains(mailRecipientWrapper.MailRecipient) || !mailRecipientWrapper.MailRecipient.IsActive || mailRecipientWrapper.MailRecipient.IsProcessed)
			{
				return false;
			}
			mailRecipientWrapper.MailRecipient.SmtpResponse = smtpResponse;
			if (trackRecipientFail)
			{
				this.TrackRecipientFail(mailRecipientWrapper.MailRecipient.Email, smtpResponse, sourceContext);
			}
			return this.mailItem.Recipients.Remove(mailRecipientWrapper.MailRecipient, dsnType, smtpResponse);
		}

		private MailRecipient GetNonDeletedItem(int index)
		{
			int num = 0;
			foreach (MailRecipient result in this.mailItem.Recipients.AllUnprocessed)
			{
				if (num == index)
				{
					return result;
				}
				num++;
			}
			return null;
		}

		private void TrackRecipientFail(RoutingAddress email, SmtpResponse smtpResponse, string sourceContext)
		{
			if (string.IsNullOrEmpty(sourceContext) && this.mexSession != null)
			{
				sourceContext = (this.mexSession.ExecutingAgentName ?? "Agent");
			}
			LatencyFormatter latencyFormatter = new LatencyFormatter(this.mailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
			MessageTrackingLog.TrackRecipientFail(MessageTrackingSource.AGENT, this.mailItem, email, smtpResponse.Equals(SmtpResponse.Empty) ? AckReason.MessageDeletedByTransportAgent : smtpResponse, sourceContext, latencyFormatter);
		}

		private const string DefaultAgentName = "Agent";

		private TransportMailItem mailItem;

		private IMExSession mexSession;

		private bool canAdd;
	}
}
