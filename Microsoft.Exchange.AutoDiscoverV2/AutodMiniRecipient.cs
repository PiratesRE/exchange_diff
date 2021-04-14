using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	public class AutodMiniRecipient : IAutodMiniRecipient
	{
		public AutodMiniRecipient(IADRawEntry rawEntry)
		{
			this.ExternalEmailAddress = (SmtpProxyAddress)rawEntry[ADRecipientSchema.ExternalEmailAddress];
			this.UserPrincipalName = (string)rawEntry[ADUserSchema.UserPrincipalName];
			this.PrimarySmtpAddress = (SmtpAddress)rawEntry[ADRecipientSchema.PrimarySmtpAddress];
			if (rawEntry[ADRecipientSchema.RecipientType] != null)
			{
				this.RecipientType = (RecipientType)rawEntry[ADRecipientSchema.RecipientType];
			}
		}

		public SmtpProxyAddress ExternalEmailAddress { get; private set; }

		public SmtpAddress PrimarySmtpAddress { get; private set; }

		public string UserPrincipalName { get; private set; }

		public RecipientType RecipientType { get; private set; }
	}
}
