using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Recipient : RecipientBase
	{
		internal Recipient(CoreRecipient coreRecipient) : base(coreRecipient)
		{
		}

		internal static void SetDefaultRecipientProperties(CoreRecipient coreRecipient)
		{
			RecipientBase.SetDefaultRecipientBaseProperties(coreRecipient);
			coreRecipient.RecipientItemType = RecipientItemType.To;
		}
	}
}
