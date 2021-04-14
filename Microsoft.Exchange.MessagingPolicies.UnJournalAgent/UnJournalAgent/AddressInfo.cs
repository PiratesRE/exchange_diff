using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal class AddressInfo
	{
		public AddressInfo(string friendlyName, RoutingAddress address)
		{
			this.friendlyName = friendlyName;
			this.address = address;
			this.primarySmtpAddress = string.Empty;
			this.recipientType = UnjournalRecipientType.Unknown;
		}

		public AddressInfo(RoutingAddress address)
		{
			this.friendlyName = address.ToString();
			this.address = address;
			this.primarySmtpAddress = string.Empty;
			this.recipientType = UnjournalRecipientType.Unknown;
		}

		public string FriendlyName
		{
			get
			{
				return this.friendlyName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.friendlyName = value;
			}
		}

		public RoutingAddress Address
		{
			get
			{
				return this.address;
			}
		}

		public UnjournalRecipientType RecipientType
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

		public string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddress;
			}
			set
			{
				this.primarySmtpAddress = value;
			}
		}

		public bool IncludedInTo
		{
			get
			{
				return this.includedInTo;
			}
			set
			{
				this.includedInTo = value;
			}
		}

		public bool IncludedInCc
		{
			get
			{
				return this.includedInCc;
			}
			set
			{
				this.includedInCc = value;
			}
		}

		public bool IncludedInBcc
		{
			get
			{
				return this.includedInBcc;
			}
			set
			{
				this.includedInBcc = value;
			}
		}

		private string friendlyName;

		private RoutingAddress address;

		private bool includedInTo;

		private bool includedInCc;

		private bool includedInBcc;

		private string primarySmtpAddress;

		private UnjournalRecipientType recipientType;
	}
}
