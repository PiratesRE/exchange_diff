using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class InvalidSharingRecipientsException : StoragePermanentException
	{
		internal InvalidSharingRecipientsException(string[] invalidRecipients, Exception innerException) : base(ServerStrings.InvalidSharingRecipientsException, innerException)
		{
			List<InvalidRecipient> list = new List<InvalidRecipient>(invalidRecipients.Length);
			foreach (string smtpAddress in invalidRecipients)
			{
				list.Add(new InvalidRecipient(smtpAddress, InvalidRecipientResponseCodeType.OtherError, null));
			}
			this.InvalidRecipients = list.ToArray();
		}

		internal InvalidSharingRecipientsException(InvalidRecipient[] invalidRecipients) : this(invalidRecipients, null)
		{
		}

		internal InvalidSharingRecipientsException(InvalidRecipient[] invalidRecipients, InvalidSharingRecipientsResolution resolution) : base(ServerStrings.InvalidSharingRecipientsException)
		{
			this.InvalidRecipients = invalidRecipients;
			this.InvalidRecipientsResolution = resolution;
		}

		public InvalidRecipient[] InvalidRecipients { get; private set; }

		public InvalidSharingRecipientsResolution InvalidRecipientsResolution { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendLine(base.ToString());
			stringBuilder.AppendLine("InvalidRecipients:");
			foreach (InvalidRecipient invalidRecipient in this.InvalidRecipients)
			{
				stringBuilder.AppendLine(invalidRecipient.ToString());
			}
			stringBuilder.AppendLine("InvalidRecipientsResolution:");
			if (this.InvalidRecipientsResolution != null)
			{
				stringBuilder.AppendLine(this.InvalidRecipientsResolution.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
