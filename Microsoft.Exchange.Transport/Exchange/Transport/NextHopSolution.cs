using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	internal class NextHopSolution : IReadOnlyMailRecipientCollection, IEnumerable<MailRecipient>, IEnumerable
	{
		public NextHopSolution(NextHopSolutionKey key)
		{
			this.nextHopSolutionKey = key;
		}

		public List<MailRecipient> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		public NextHopSolutionKey NextHopSolutionKey
		{
			get
			{
				return this.nextHopSolutionKey;
			}
		}

		public DeliveryStatus DeliveryStatus
		{
			get
			{
				return this.deliveryStatus;
			}
			set
			{
				this.deliveryStatus = value;
			}
		}

		public bool IsInactive
		{
			get
			{
				return this.deliveryStatus == DeliveryStatus.Complete || this.deliveryStatus == DeliveryStatus.PendingResubmit;
			}
		}

		public bool IsDeletedByAdmin
		{
			get
			{
				return this.AdminActionStatus == AdminActionStatus.PendingDeleteWithNDR || this.AdminActionStatus == AdminActionStatus.PendingDeleteWithOutNDR;
			}
		}

		public bool Deferred
		{
			get
			{
				return this.inDeferredQueue;
			}
			set
			{
				this.inDeferredQueue = value;
			}
		}

		public DateTime DeferUntil
		{
			get
			{
				return this.deferUntil;
			}
			set
			{
				this.deferUntil = value;
			}
		}

		public AdminActionStatus AdminActionStatus
		{
			get
			{
				if (this.adminActionRecipient == null)
				{
					return AdminActionStatus.None;
				}
				return this.adminActionRecipient.AdminActionStatus;
			}
			set
			{
				if (this.adminActionRecipient != null)
				{
					this.adminActionRecipient.AdminActionStatus = value;
					if (value == AdminActionStatus.None)
					{
						this.adminActionRecipient = null;
						return;
					}
				}
				else if (value != AdminActionStatus.None)
				{
					MailRecipient mailRecipient = this.recipients.Find(new Predicate<MailRecipient>(NextHopSolution.IsNotProcessed));
					if (mailRecipient == null)
					{
						throw new InvalidOperationException("Attempting to set the admin status of a solution but all recipients have been processed");
					}
					this.adminActionRecipient = mailRecipient;
					this.adminActionRecipient.AdminActionStatus = value;
				}
			}
		}

		public AccessToken AccessToken
		{
			get
			{
				return this.accessToken;
			}
			internal set
			{
				this.accessToken = value;
			}
		}

		public string LockReason
		{
			get
			{
				return this.lockReason;
			}
			internal set
			{
				this.lockReason = value;
			}
		}

		public DateTimeOffset LockExpirationTime
		{
			get
			{
				return this.lockExpirationTime;
			}
			internal set
			{
				this.lockExpirationTime = value;
			}
		}

		public WaitCondition CurrentCondition
		{
			get
			{
				return this.currentCondition;
			}
			internal set
			{
				this.currentCondition = value;
			}
		}

		int IReadOnlyMailRecipientCollection.Count
		{
			get
			{
				return this.recipients.Count;
			}
		}

		IEnumerable<MailRecipient> IReadOnlyMailRecipientCollection.All
		{
			get
			{
				return this.recipients;
			}
		}

		IEnumerable<MailRecipient> IReadOnlyMailRecipientCollection.AllUnprocessed
		{
			get
			{
				return from recipient in this.recipients
				where recipient.IsActive && !recipient.IsProcessed
				select recipient;
			}
		}

		MailRecipient IReadOnlyMailRecipientCollection.this[int index]
		{
			get
			{
				return this.recipients[index];
			}
		}

		public void AddRecipient(MailRecipient recipient)
		{
			this.recipients.Add(recipient);
			if (recipient.AdminActionStatus != AdminActionStatus.None)
			{
				if (this.adminActionRecipient != null)
				{
					if (this.adminActionRecipient.AdminActionStatus < recipient.AdminActionStatus)
					{
						this.adminActionRecipient.AdminActionStatus = AdminActionStatus.None;
						this.adminActionRecipient = recipient;
						return;
					}
					recipient.AdminActionStatus = AdminActionStatus.None;
					return;
				}
				else
				{
					this.adminActionRecipient = recipient;
				}
			}
		}

		public void AddRecipients(IEnumerable<MailRecipient> recipients)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			foreach (MailRecipient recipient in recipients)
			{
				this.AddRecipient(recipient);
			}
		}

		public void CheckAdminAction()
		{
			if (this.AdminActionStatus == AdminActionStatus.Suspended && this.deliveryStatus != DeliveryStatus.Complete && this.adminActionRecipient.IsProcessed)
			{
				MailRecipient mailRecipient = this.recipients.Find(new Predicate<MailRecipient>(NextHopSolution.IsNotProcessed));
				if (mailRecipient == null)
				{
					throw new InvalidOperationException("Attempting to switch the admin recipient of a solution but all recipients have been processed");
				}
				AdminActionStatus adminActionStatus = this.adminActionRecipient.AdminActionStatus;
				this.adminActionRecipient.AdminActionStatus = AdminActionStatus.None;
				this.adminActionRecipient = mailRecipient;
				this.adminActionRecipient.AdminActionStatus = adminActionStatus;
			}
		}

		public int GetOutboundIPPool()
		{
			if (this.recipients.Count == 0)
			{
				return 0;
			}
			int outboundIPPool = this.recipients[0].OutboundIPPool;
			foreach (MailRecipient mailRecipient in this.recipients)
			{
				if (mailRecipient.OutboundIPPool != outboundIPPool)
				{
					throw new InvalidOperationException("NextHopSolution has recipients with different Outbound IP Pools");
				}
			}
			return outboundIPPool;
		}

		bool IReadOnlyMailRecipientCollection.Contains(MailRecipient item)
		{
			return this.recipients.Contains(item);
		}

		public IEnumerator<MailRecipient> GetEnumerator()
		{
			return this.recipients.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private static bool IsNotProcessed(MailRecipient item)
		{
			return !item.IsProcessed || item.NextHop.NextHopType.DeliveryType == DeliveryType.ShadowRedundancy;
		}

		private MailRecipient adminActionRecipient;

		private List<MailRecipient> recipients = new List<MailRecipient>();

		private NextHopSolutionKey nextHopSolutionKey;

		private DeliveryStatus deliveryStatus;

		private bool inDeferredQueue;

		private AccessToken accessToken;

		private DateTimeOffset lockExpirationTime;

		private string lockReason;

		private WaitCondition currentCondition;

		private DateTime deferUntil = DateTime.MinValue;
	}
}
