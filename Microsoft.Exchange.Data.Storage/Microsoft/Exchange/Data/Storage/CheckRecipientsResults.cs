using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CheckRecipientsResults
	{
		internal ValidRecipient[] ValidRecipients { get; private set; }

		internal string[] InvalidRecipients { get; private set; }

		internal string TargetRecipients
		{
			get
			{
				return string.Join(";", ValidRecipient.ConvertToStringArray(this.ValidRecipients));
			}
		}

		internal CheckRecipientsResults(ValidRecipient[] validRecipients) : this(validRecipients, Array<string>.Empty)
		{
		}

		internal CheckRecipientsResults(string[] invalidRecipients) : this(ValidRecipient.EmptyRecipients, invalidRecipients)
		{
		}

		internal CheckRecipientsResults(ValidRecipient[] validRecipients, string[] invalidRecipients)
		{
			this.ValidRecipients = validRecipients;
			this.InvalidRecipients = invalidRecipients;
		}
	}
}
