using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Serializable]
	internal sealed class RecipientValidationError : ValidationError
	{
		internal RecipientValidationError(LocalizedString message) : base(message)
		{
		}
	}
}
