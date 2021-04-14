using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	internal interface ITenantRepository
	{
		ADRecipient GetOnPremUser(SmtpAddress emailAddress);

		IAutodMiniRecipient GetNextUserFromSortedList(SmtpAddress emailAddress);
	}
}
