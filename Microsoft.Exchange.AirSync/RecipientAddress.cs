using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class RecipientAddress : IComparable
	{
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

		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
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

		private AddressOrigin addressOrigin;

		private string displayName;

		private int index;

		private string routingAddress;

		private string routingType;

		private string smtpAddress;

		private StoreObjectId storeObjectId;
	}
}
