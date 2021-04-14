using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationNspiSetRecipientRpcArgs : MigrationNspiRpcArgs
	{
		public MigrationNspiSetRecipientRpcArgs(ExchangeOutlookAnywhereEndpoint endpoint, string recipientSmtpAddress, string recipientLegDN, string[] propTagValues, long[] longPropTags) : base(endpoint, MigrationProxyRpcType.SetRecipient)
		{
			this.RecipientSmtpAddress = recipientSmtpAddress;
			this.RecipientLegDN = recipientLegDN;
			this.PropTagValues = propTagValues;
			this.LongPropTags = longPropTags;
			this.ExchangeServer = endpoint.ExchangeServer;
		}

		public MigrationNspiSetRecipientRpcArgs(byte[] requestBlob) : base(requestBlob, MigrationProxyRpcType.SetRecipient)
		{
		}

		public string RecipientSmtpAddress
		{
			get
			{
				return base.GetProperty<string>(2416508959U);
			}
			set
			{
				base.SetPropertyAsString(2416508959U, value);
			}
		}

		public string RecipientLegDN
		{
			get
			{
				return base.GetProperty<string>(2416967711U);
			}
			set
			{
				base.SetPropertyAsString(2416967711U, value);
			}
		}

		public string ExchangeServer
		{
			get
			{
				return base.GetProperty<string>(2416902175U);
			}
			set
			{
				base.SetPropertyAsString(2416902175U, value);
			}
		}

		public string[] PropTagValues
		{
			get
			{
				return base.GetProperty<string[]>(2416775199U);
			}
			set
			{
				base.SetProperty(2416775199U, value);
			}
		}

		public long[] LongPropTags
		{
			private get
			{
				return base.GetProperty<long[]>(2416447508U);
			}
			set
			{
				base.SetProperty(2416447508U, value);
			}
		}

		public PropTag[] PropTags
		{
			get
			{
				long[] longPropTags = this.LongPropTags;
				if (longPropTags == null)
				{
					return null;
				}
				PropTag[] array = new PropTag[longPropTags.Length];
				for (int i = 0; i < longPropTags.Length; i++)
				{
					array[i] = (PropTag)longPropTags[i];
				}
				return array;
			}
		}

		public override bool Validate(out string errorMsg)
		{
			if (!base.Validate(out errorMsg))
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.RecipientSmtpAddress))
			{
				errorMsg = "Recipient Smtp Address cannot be null or empty.";
				return false;
			}
			if (string.IsNullOrEmpty(this.ExchangeServer))
			{
				errorMsg = "Exchange Server cannot be null or empty.";
				return false;
			}
			if (this.LongPropTags == null || this.LongPropTags.Length == 0)
			{
				errorMsg = "PropTags cannot be null.";
				return false;
			}
			if (this.PropTagValues == null || this.PropTagValues.Length == 0)
			{
				errorMsg = "PropTagValues cannot be null.";
				return false;
			}
			if (this.PropTagValues.Length != this.LongPropTags.Length)
			{
				errorMsg = "PropTagValues has to be same in size as PropTags.";
				return false;
			}
			errorMsg = null;
			return true;
		}
	}
}
