using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class MailboxState : IEquatable<MailboxState>
	{
		public MailboxState()
		{
		}

		public MailboxState(int mailboxNumber, int rawState)
		{
			this.MailboxNumber = mailboxNumber;
			this.RawState = rawState;
		}

		public int MailboxNumber { get; internal set; }

		public int RawState { get; internal set; }

		public bool IsCompleted
		{
			get
			{
				return this.RawState == int.MaxValue || this.RawState == -4;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MailboxState);
		}

		public override int GetHashCode()
		{
			return this.MailboxNumber.GetHashCode() ^ this.RawState.GetHashCode();
		}

		public bool Equals(MailboxState other)
		{
			return other != null && this.MailboxNumber == other.MailboxNumber && this.RawState == other.RawState;
		}

		public const int InitialState = -1;

		public const int RecrawlState = -2;

		public const int RecrawlFailureRetryState = -3;

		public const int RecrawlFailurePermanentState = -4;

		public const int CompleteState = 2147483647;
	}
}
