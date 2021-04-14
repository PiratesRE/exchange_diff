using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class RecipientEwsConverter
	{
		internal static Recipient ToRecipient(this SingleRecipientType singleRecipientType)
		{
			if (singleRecipientType == null)
			{
				return null;
			}
			return singleRecipientType.Mailbox.ToRecipient();
		}

		internal static SingleRecipientType ToSingleRecipientType(this Recipient recipient)
		{
			if (recipient == null)
			{
				return null;
			}
			return new SingleRecipientType
			{
				Mailbox = recipient.ToEmailAddressWrapper()
			};
		}

		internal static Recipient ToRecipient(this EmailAddressWrapper emailAddressWrapper)
		{
			if (emailAddressWrapper == null)
			{
				return null;
			}
			return new Recipient
			{
				Name = emailAddressWrapper.Name,
				Address = emailAddressWrapper.EmailAddress
			};
		}

		internal static EmailAddressWrapper ToEmailAddressWrapper(this Recipient recipient)
		{
			if (recipient == null)
			{
				return null;
			}
			return new EmailAddressWrapper
			{
				EmailAddress = recipient.Address,
				Name = recipient.Name
			};
		}
	}
}
