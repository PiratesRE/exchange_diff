using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal class EnvelopeJournalReport
	{
		public EnvelopeJournalReport(AddressInfo senderAddress, List<AddressInfo> recipientAddresses, string messageIdString, bool defective)
		{
			this.sender = senderAddress;
			this.recipients = recipientAddresses;
			this.messageId = messageIdString;
			this.defective = defective;
		}

		public AddressInfo Sender
		{
			get
			{
				if (this.validSenderJournalArchiveAddressIsSet)
				{
					return this.SenderJournalArchiveAddress;
				}
				return this.sender;
			}
		}

		public AddressInfo EnvelopeSender
		{
			get
			{
				return this.sender;
			}
		}

		public AddressInfo SenderJournalArchiveAddress
		{
			get
			{
				return this.journalArchiveAddress;
			}
			set
			{
				this.journalArchiveAddress = value;
				if (this.journalArchiveAddress != null)
				{
					RoutingAddress address = this.journalArchiveAddress.Address;
					if (this.journalArchiveAddress.Address.IsValid && this.journalArchiveAddress.Address != RoutingAddress.NullReversePath)
					{
						this.validSenderJournalArchiveAddressIsSet = true;
					}
				}
			}
		}

		public bool SenderJournalArchiveAddressIsValid
		{
			get
			{
				return this.validSenderJournalArchiveAddressIsSet;
			}
		}

		public List<AddressInfo> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		public string MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public bool Defective
		{
			get
			{
				return this.defective;
			}
		}

		public List<RoutingAddress> ExternalOrUnprovisionedRecipients
		{
			get
			{
				return this.externalOrUnprovisionedRecipients;
			}
			set
			{
				this.externalOrUnprovisionedRecipients = value;
			}
		}

		public List<RoutingAddress> DistributionLists
		{
			get
			{
				return this.distributionlists;
			}
			set
			{
				this.distributionlists = value;
			}
		}

		public bool IsSenderInternal
		{
			get
			{
				return this.isSenderInternal;
			}
			set
			{
				this.isSenderInternal = value;
			}
		}

		public Attachment EmbeddedMessageAttachment
		{
			get
			{
				return this.embeddedMessageAttachment;
			}
			set
			{
				this.embeddedMessageAttachment = value;
			}
		}

		private readonly string messageId;

		private readonly bool defective;

		private AddressInfo sender;

		private AddressInfo journalArchiveAddress;

		private List<AddressInfo> recipients;

		private List<RoutingAddress> externalOrUnprovisionedRecipients = new List<RoutingAddress>();

		private List<RoutingAddress> distributionlists = new List<RoutingAddress>();

		private bool isSenderInternal;

		private Attachment embeddedMessageAttachment;

		private bool validSenderJournalArchiveAddressIsSet;
	}
}
