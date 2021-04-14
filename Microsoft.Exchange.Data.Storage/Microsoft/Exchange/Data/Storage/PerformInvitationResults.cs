using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PerformInvitationResults
	{
		internal PerformInvitationResultType Result
		{
			get
			{
				return this.result;
			}
		}

		internal ValidRecipient[] SucceededRecipients
		{
			get
			{
				if (this.succeededRecipients == null)
				{
					switch (this.Result)
					{
					case PerformInvitationResultType.Success:
						this.succeededRecipients = this.allRecipients;
						goto IL_AB;
					case PerformInvitationResultType.PartiallySuccess:
					{
						HashSet<string> failedRecipientSet = new HashSet<string>();
						foreach (InvalidRecipient invalidRecipient in this.FailedRecipients)
						{
							failedRecipientSet.Add(invalidRecipient.SmtpAddress);
						}
						this.succeededRecipients = Array.FindAll<ValidRecipient>(this.allRecipients, (ValidRecipient recipient) => !failedRecipientSet.Contains(recipient.SmtpAddress));
						goto IL_AB;
					}
					case PerformInvitationResultType.Failed:
						this.succeededRecipients = ValidRecipient.EmptyRecipients;
						goto IL_AB;
					}
					throw new InvalidOperationException();
				}
				IL_AB:
				return this.succeededRecipients;
			}
		}

		internal InvalidRecipient[] FailedRecipients
		{
			get
			{
				if (this.Result == PerformInvitationResultType.Ignored)
				{
					throw new InvalidOperationException();
				}
				if (this.exception != null)
				{
					return this.exception.InvalidRecipients;
				}
				return null;
			}
		}

		private PerformInvitationResults()
		{
			this.result = PerformInvitationResultType.Ignored;
		}

		internal PerformInvitationResults(ValidRecipient[] allRecipients)
		{
			Util.ThrowOnNullArgument(allRecipients, "allRecipients");
			if (allRecipients.Length == 0)
			{
				throw new ArgumentException("allRecipients");
			}
			this.result = PerformInvitationResultType.Success;
			this.allRecipients = allRecipients;
		}

		internal PerformInvitationResults(InvalidSharingRecipientsException exception)
		{
			Util.ThrowOnNullArgument(exception, "exception");
			this.result = PerformInvitationResultType.Failed;
			this.exception = exception;
		}

		internal PerformInvitationResults(ValidRecipient[] allRecipients, InvalidSharingRecipientsException exception)
		{
			Util.ThrowOnNullArgument(exception, "exception");
			Util.ThrowOnNullArgument(allRecipients, "allRecipients");
			if (allRecipients.Length == 0)
			{
				throw new ArgumentException("allRecipients");
			}
			this.result = PerformInvitationResultType.PartiallySuccess;
			this.exception = exception;
			this.allRecipients = allRecipients;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("Result: " + this.Result.ToString());
			if (this.Result == PerformInvitationResultType.Success || this.result == PerformInvitationResultType.PartiallySuccess)
			{
				stringBuilder.AppendLine(", SucceededRecipients:");
				foreach (ValidRecipient validRecipient in this.SucceededRecipients)
				{
					stringBuilder.AppendLine(validRecipient.ToString() + ";");
				}
			}
			if (this.Result == PerformInvitationResultType.PartiallySuccess || this.Result == PerformInvitationResultType.Failed)
			{
				stringBuilder.AppendLine(", FailedRecipients:");
				foreach (InvalidRecipient invalidRecipient in this.FailedRecipients)
				{
					stringBuilder.AppendLine(invalidRecipient.ToString() + ";");
				}
			}
			return stringBuilder.ToString();
		}

		private readonly PerformInvitationResultType result;

		private readonly ValidRecipient[] allRecipients;

		private readonly InvalidSharingRecipientsException exception;

		private ValidRecipient[] succeededRecipients;

		internal static PerformInvitationResults Ignored = new PerformInvitationResults();
	}
}
