using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Serializable]
	public sealed class RecipientInvalidException : LocalizedException
	{
		internal RecipientInvalidException(string errorMessage) : base(new LocalizedString(errorMessage))
		{
		}
	}
}
