using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OWARecipient : IComparable<OWARecipient>
	{
		public ADObjectId Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
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

		public string PhoneticDisplayName
		{
			get
			{
				return this.phoneticDisplayName;
			}
			set
			{
				this.phoneticDisplayName = value;
			}
		}

		public RecipientType UserRecipientType
		{
			get
			{
				return this.recipientType;
			}
			set
			{
				this.recipientType = value;
				this.isDistributionList = Utilities.IsADDistributionList(this.recipientType);
			}
		}

		public string LegacyDN
		{
			get
			{
				return this.legacyExchangeDN;
			}
			set
			{
				this.legacyExchangeDN = value;
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

		public bool IsDistributionList
		{
			get
			{
				return this.isDistributionList;
			}
		}

		public bool HasValidDigitalId
		{
			get
			{
				return this.hasValidDigitalId;
			}
			set
			{
				this.hasValidDigitalId = value;
			}
		}

		public int CompareTo(OWARecipient x)
		{
			return string.Compare(this.DisplayName, x.DisplayName, StringComparison.CurrentCulture);
		}

		private string displayName;

		private string phoneticDisplayName;

		private RecipientType recipientType;

		private string alias;

		private string legacyExchangeDN;

		private ADObjectId id;

		private bool isDistributionList;

		private bool hasValidDigitalId;
	}
}
