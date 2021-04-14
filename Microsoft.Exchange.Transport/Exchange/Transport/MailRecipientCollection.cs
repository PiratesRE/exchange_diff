using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal class MailRecipientCollection : IDataExternalComponent, IDataObjectComponent, ICollection<MailRecipient>, IReadOnlyMailRecipientCollection, IEnumerable<MailRecipient>, IEnumerable, IMailRecipientCollectionFacade
	{
		public MailRecipientCollection(TransportMailItem mailItem) : this(mailItem, new List<MailRecipient>())
		{
		}

		public MailRecipientCollection(TransportMailItem mailItem, List<MailRecipient> recipients)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			this.mailItem = mailItem;
			this.recipients = recipients;
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public int Count
		{
			get
			{
				return this.recipients.Count;
			}
		}

		public IEnumerable<MailRecipient> All
		{
			get
			{
				return this.recipients;
			}
		}

		public bool PendingDatabaseUpdates
		{
			get
			{
				return this.recipients.Any((MailRecipient recipient) => recipient.PendingDatabaseUpdates);
			}
		}

		public int PendingDatabaseUpdateCount
		{
			get
			{
				return this.recipients.Count((MailRecipient recipient) => recipient.PendingDatabaseUpdates);
			}
		}

		public IEnumerable<MailRecipient> AllUnprocessed
		{
			get
			{
				return from recipient in this.recipients
				where recipient.IsActive && !recipient.IsProcessed
				select recipient;
			}
		}

		public MailRecipient this[int index]
		{
			get
			{
				return this.recipients[index];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<MailRecipient> GetEnumerator()
		{
			return this.recipients.GetEnumerator();
		}

		public int CountUnprocessed()
		{
			return this.recipients.Count((MailRecipient recipient) => recipient.IsActive && !recipient.IsProcessed);
		}

		public void CopyTo(Array array, int index)
		{
			this.CopyTo(array as MailRecipient[], index);
		}

		public void CopyTo(MailRecipient[] array, int index)
		{
			this.recipients.CopyTo(array, index);
		}

		public void Clear()
		{
			foreach (MailRecipient mailRecipient in this.recipients)
			{
				mailRecipient.ReleaseFromActive();
			}
		}

		public void CloneFrom(IDataObjectComponent other)
		{
			throw new NotSupportedException();
		}

		void IDataExternalComponent.MarkToDelete()
		{
			foreach (MailRecipient mailRecipient in this.recipients)
			{
				mailRecipient.MarkToDelete();
			}
		}

		public void SaveToExternalRow(Transaction transaction)
		{
			for (int i = this.recipients.Count - 1; i >= 0; i--)
			{
				MailRecipient mailRecipient = this.recipients[i];
				mailRecipient.Commit(transaction);
				if (mailRecipient.IsRowDeleted || !mailRecipient.IsActive)
				{
					this.recipients.RemoveAt(i);
				}
			}
		}

		public void MinimizeMemory()
		{
			foreach (MailRecipient mailRecipient in this.recipients)
			{
				mailRecipient.MinimizeMemory();
			}
		}

		internal void Sort(IComparer<MailRecipient> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			this.recipients.Sort(comparer);
		}

		internal void RemoveInternal(MailRecipient item)
		{
			this.recipients.Remove(item);
		}

		public bool Contains(MailRecipient item)
		{
			return this.recipients.Contains(item);
		}

		public MailRecipient Add(string smtpAddress)
		{
			MailRecipient mailRecipient = MailRecipient.NewMessageRecipient(this.mailItem);
			mailRecipient.Email = new RoutingAddress(smtpAddress);
			return mailRecipient;
		}

		public void Add(MailRecipient recip)
		{
			if (recip == null)
			{
				throw new ArgumentNullException("recip");
			}
			recip.Attach(this.mailItem);
			this.recipients.Add(recip);
		}

		public void Prepend(MailRecipient recip)
		{
			if (recip == null)
			{
				throw new ArgumentNullException("recip");
			}
			recip.Attach(this.mailItem);
			this.recipients.Insert(0, recip);
		}

		public bool Remove(MailRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (!this.recipients.Contains(recipient) || !recipient.IsActive)
			{
				return false;
			}
			bool result = !recipient.IsProcessed;
			recipient.ReleaseFromActive();
			return result;
		}

		public bool Remove(MailRecipient recipient, DsnType dsnType, SmtpResponse smtpResponse)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (!this.recipients.Contains(recipient) || !recipient.IsActive || recipient.IsProcessed)
			{
				return false;
			}
			AckStatus ackStatus = AckStatus.Success;
			if (dsnType == DsnType.Failure)
			{
				ackStatus = AckStatus.Fail;
			}
			else if (dsnType == DsnType.Expanded)
			{
				ackStatus = AckStatus.Expand;
			}
			recipient.Ack(ackStatus, smtpResponse);
			return true;
		}

		public void RemoveDuplicates()
		{
			if (this.recipients.Count < 2)
			{
				return;
			}
			HashSet<RoutingAddress> hashSet = new HashSet<RoutingAddress>();
			foreach (MailRecipient mailRecipient in this.AllUnprocessed)
			{
				if (!hashSet.Add(mailRecipient.Email))
				{
					MailRecipientCollection.Tracer.TraceDebug<RoutingAddress>(0L, "Removing duplicate recipient {0}", mailRecipient.Email);
					mailRecipient.Ack(AckStatus.SuccessNoDsn, AckReason.DuplicateRecipient);
				}
			}
		}

		public void RemoveDuplicatesToSameRoute()
		{
			if (this.recipients.Count < 2)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (MailRecipient mailRecipient in this.AllUnprocessed)
			{
				string text = mailRecipient.Email.ToString();
				RoutingOverride routingOverride = mailRecipient.RoutingOverride;
				if (routingOverride != null)
				{
					text = text + "@" + routingOverride.ToString();
				}
				if (!hashSet.Add(text))
				{
					MailRecipientCollection.Tracer.TraceDebug<RoutingAddress>(0L, "Removing duplicate recipient {0}", mailRecipient.Email);
					mailRecipient.Ack(AckStatus.SuccessNoDsn, AckReason.DuplicateRecipient);
				}
			}
		}

		void IDataExternalComponent.ParentPrimaryKeyChanged()
		{
			foreach (MailRecipient mailRecipient in this.recipients)
			{
				if (!mailRecipient.IsRowDeleted)
				{
					mailRecipient.UpdateOwnerId();
				}
			}
		}

		void IMailRecipientCollectionFacade.Add(string smtpAddress)
		{
			this.Add(smtpAddress);
		}

		void IMailRecipientCollectionFacade.AddWithoutDsnRequested(string smtpAddress)
		{
			MailRecipient mailRecipient = this.Add(smtpAddress);
			mailRecipient.DsnRequested = DsnRequestedFlags.Never;
		}

		private static readonly Trace Tracer = ExTraceGlobals.GeneralTracer;

		private readonly List<MailRecipient> recipients;

		private readonly TransportMailItem mailItem;
	}
}
