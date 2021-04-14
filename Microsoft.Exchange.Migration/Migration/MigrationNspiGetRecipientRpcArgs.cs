using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationNspiGetRecipientRpcArgs : MigrationNspiRpcArgs
	{
		public MigrationNspiGetRecipientRpcArgs(ExchangeOutlookAnywhereEndpoint endpoint, string recipientSmtpAddress, long[] longPropTags) : base(endpoint, MigrationProxyRpcType.GetRecipient)
		{
			this.RecipientSmtpAddress = recipientSmtpAddress;
			this.LongPropTags = longPropTags;
		}

		public MigrationNspiGetRecipientRpcArgs(byte[] requestBlob) : base(requestBlob, MigrationProxyRpcType.GetRecipient)
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
			if (this.LongPropTags == null || this.LongPropTags.Length == 0)
			{
				errorMsg = "PropTags cannot be null.";
				return false;
			}
			errorMsg = null;
			return true;
		}
	}
}
