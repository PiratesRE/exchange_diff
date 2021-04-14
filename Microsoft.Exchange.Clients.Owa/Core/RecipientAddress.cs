using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class RecipientAddress : IComparable
	{
		public static AddressOrigin ToAddressOrigin(Participant participant)
		{
			if (participant.Origin is DirectoryParticipantOrigin)
			{
				return AddressOrigin.Directory;
			}
			if (participant.Origin is OneOffParticipantOrigin)
			{
				if (!(participant.RoutingType == "EX"))
				{
					return AddressOrigin.OneOff;
				}
				return AddressOrigin.Directory;
			}
			else
			{
				if (participant.Origin is StoreParticipantOrigin)
				{
					return AddressOrigin.Store;
				}
				return AddressOrigin.Unknown;
			}
		}

		public string Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.alias = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public bool IsDistributionList
		{
			get
			{
				return this.isDistributionList;
			}
			set
			{
				this.isDistributionList = value;
			}
		}

		public bool IsRoom
		{
			get
			{
				return this.isRoom;
			}
			set
			{
				this.isRoom = value;
			}
		}

		public StoreObjectId StoreObjectId
		{
			get
			{
				return this.storeObjectId;
			}
			set
			{
				this.storeObjectId = value;
			}
		}

		public ADObjectId ADObjectId
		{
			get
			{
				return this.adObjectId;
			}
			set
			{
				this.adObjectId = value;
			}
		}

		public AddressOrigin AddressOrigin
		{
			get
			{
				return this.addressOrigin;
			}
			set
			{
				this.addressOrigin = value;
			}
		}

		public int RecipientFlags
		{
			get
			{
				int num = 0;
				if (this.isRoom)
				{
					num |= 2;
				}
				if (this.isDistributionList)
				{
					num |= 1;
				}
				return num;
			}
		}

		public string RoutingAddress
		{
			get
			{
				return this.routingAddress;
			}
			set
			{
				this.routingAddress = value;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.routingType;
			}
			set
			{
				this.routingType = value;
			}
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
			set
			{
				this.smtpAddress = value;
			}
		}

		public EmailAddressIndex EmailAddressIndex
		{
			get
			{
				return this.emailAddressIndex;
			}
			set
			{
				this.emailAddressIndex = value;
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return this.recipientType;
			}
			set
			{
				this.recipientType = value;
			}
		}

		public string SipUri
		{
			get
			{
				return this.sipUri;
			}
			set
			{
				this.sipUri = value;
			}
		}

		public string MobilePhoneNumber
		{
			get
			{
				return this.mobilePhoneNumber;
			}
			set
			{
				this.mobilePhoneNumber = value;
			}
		}

		public int CompareTo(object value)
		{
			RecipientAddress recipientAddress = value as RecipientAddress;
			if (recipientAddress == null)
			{
				throw new ArgumentException("object is not an RecipientAddress");
			}
			if (this.displayName != null && recipientAddress.DisplayName != null)
			{
				return this.displayName.CompareTo(recipientAddress.DisplayName);
			}
			if (this.displayName == null && recipientAddress.DisplayName != null)
			{
				return -1;
			}
			if (this.displayName != null && recipientAddress.DisplayName == null)
			{
				return 1;
			}
			return 0;
		}

		private ADObjectId adObjectId;

		private StoreObjectId storeObjectId;

		private AddressOrigin addressOrigin;

		private string alias;

		private string displayName;

		private string routingAddress;

		private string routingType;

		private string smtpAddress;

		private bool isDistributionList;

		private bool isRoom;

		private EmailAddressIndex emailAddressIndex;

		private RecipientType recipientType;

		private string sipUri;

		private string mobilePhoneNumber;

		[Flags]
		public enum RecipientAddressFlags
		{
			None = 0,
			DistributionList = 1,
			Room = 2
		}
	}
}
