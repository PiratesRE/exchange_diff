using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal class AddressBookEntryImpl : AddressBookEntry
	{
		internal AddressBookEntryImpl(TransportMiniRecipient entry)
		{
			this.entry = entry;
		}

		internal AddressBookEntryImpl(TransportMiniRecipient entry, RoutingAddress primaryAddress) : this(entry)
		{
			this.primaryAddress = primaryAddress;
		}

		public override RoutingAddress PrimaryAddress
		{
			get
			{
				if (RoutingAddress.Empty == this.primaryAddress)
				{
					ProxyAddress proxyAddress = this.entry.EmailAddresses.FindPrimary(ProxyAddressPrefix.Smtp);
					if (null != proxyAddress)
					{
						this.primaryAddress = (RoutingAddress)proxyAddress.AddressString;
					}
				}
				return this.primaryAddress;
			}
		}

		public override bool RequiresAuthentication
		{
			get
			{
				return this.entry.RequireAllSendersAreAuthenticated;
			}
		}

		public override bool AntispamBypass
		{
			get
			{
				return this.entry.AntispamBypassEnabled;
			}
		}

		public override Microsoft.Exchange.Data.Transport.RecipientType RecipientType
		{
			get
			{
				Microsoft.Exchange.Data.Transport.RecipientType result = Microsoft.Exchange.Data.Transport.RecipientType.Unknown;
				MultiValuedProperty<string> objectClass = this.entry.ObjectClass;
				if (objectClass.Contains("user"))
				{
					RecipientDisplayType? recipientDisplayType = this.entry.RecipientDisplayType;
					if (recipientDisplayType == RecipientDisplayType.ConferenceRoomMailbox || recipientDisplayType == RecipientDisplayType.SyncedConferenceRoomMailbox)
					{
						result = Microsoft.Exchange.Data.Transport.RecipientType.ConferenceRoom;
					}
					else if (recipientDisplayType == RecipientDisplayType.EquipmentMailbox || recipientDisplayType == RecipientDisplayType.SyncedEquipmentMailbox)
					{
						result = Microsoft.Exchange.Data.Transport.RecipientType.Equipment;
					}
					else
					{
						result = Microsoft.Exchange.Data.Transport.RecipientType.User;
					}
				}
				else if (objectClass.Contains("contact"))
				{
					result = Microsoft.Exchange.Data.Transport.RecipientType.Contact;
				}
				else if (objectClass.Contains("group"))
				{
					result = Microsoft.Exchange.Data.Transport.RecipientType.DistributionList;
				}
				else if (objectClass.Contains("msExchDynamicDistributionList"))
				{
					result = Microsoft.Exchange.Data.Transport.RecipientType.DynamicDistributionList;
				}
				else if (objectClass.Contains("publicFolder"))
				{
					result = Microsoft.Exchange.Data.Transport.RecipientType.PublicFolder;
				}
				return result;
			}
		}

		public override SecurityIdentifier UserAccountSid
		{
			get
			{
				return this.entry.Sid;
			}
		}

		public override SecurityIdentifier MasterAccountSid
		{
			get
			{
				return this.entry.MasterAccountSid;
			}
		}

		public override string WindowsLiveId
		{
			get
			{
				return this.entry.WindowsLiveID.ToString();
			}
		}

		public override int GetSpamConfidenceLevelThreshold(SpamAction action, int defaultValue)
		{
			if (0 > defaultValue || (9 < defaultValue && 2147483647 != defaultValue))
			{
				throw new ArgumentOutOfRangeException("defaultValue", Strings.GetSclThresholdDefaultValueOutOfRange);
			}
			bool? flag = null;
			int? num = null;
			switch (action)
			{
			case SpamAction.Quarantine:
				flag = this.entry.SCLQuarantineEnabled;
				num = this.entry.SCLQuarantineThreshold;
				break;
			case SpamAction.Reject:
				flag = this.entry.SCLRejectEnabled;
				num = this.entry.SCLRejectThreshold;
				break;
			case SpamAction.Delete:
				flag = this.entry.SCLDeleteEnabled;
				num = this.entry.SCLDeleteThreshold;
				break;
			default:
				throw new ArgumentOutOfRangeException("action");
			}
			if (flag == false)
			{
				return int.MaxValue;
			}
			int? num2 = num;
			if (num2 == null)
			{
				return defaultValue;
			}
			return num2.GetValueOrDefault();
		}

		public override bool IsSafeSender(RoutingAddress senderAddress)
		{
			if (this.safeSenders == null && this.entry.SafeSendersHash != null)
			{
				this.safeSenders = new AddressHashes(this.entry.SafeSendersHash);
			}
			return this.safeSenders != null && this.safeSenders.Contains(senderAddress);
		}

		public override bool IsSafeRecipient(RoutingAddress recipientAddress)
		{
			if (this.safeRecipients == null && this.entry.SafeRecipientsHash != null)
			{
				this.safeRecipients = new AddressHashes(this.entry.SafeRecipientsHash);
			}
			return this.safeRecipients != null && this.safeRecipients.Contains(recipientAddress);
		}

		public override bool IsBlockedSender(RoutingAddress senderAddress)
		{
			if (this.blockedSenders == null && this.entry.BlockedSendersHash != null)
			{
				this.blockedSenders = new AddressHashes(this.entry.BlockedSendersHash);
			}
			return this.blockedSenders != null && this.blockedSenders.Contains(senderAddress);
		}

		public override string ToString()
		{
			return (string)this.PrimaryAddress;
		}

		private TransportMiniRecipient entry;

		private RoutingAddress primaryAddress = RoutingAddress.Empty;

		private AddressHashes safeSenders;

		private AddressHashes safeRecipients;

		private AddressHashes blockedSenders;
	}
}
