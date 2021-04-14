using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class PrequarantinedMailbox
	{
		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public int CrashCount
		{
			get
			{
				return this.crashCount;
			}
		}

		public DateTime LastCrashTime
		{
			get
			{
				return this.lastCrashTime;
			}
		}

		public TimeSpan QuarantineDuration
		{
			get
			{
				return this.quarantineDuration;
			}
		}

		public string QuarantineReason
		{
			get
			{
				return this.quarantineReason;
			}
		}

		internal static string TruncateQuarantineReason(string quarantineReason)
		{
			if (quarantineReason == null)
			{
				return null;
			}
			if (quarantineReason.Length <= PrequarantinedMailbox.QuarantineReasonLengthLimit)
			{
				return quarantineReason;
			}
			return quarantineReason.Substring(0, PrequarantinedMailbox.QuarantineReasonLengthLimit);
		}

		public PrequarantinedMailbox(Guid mailboxGuid, int crashCount, DateTime lastCrashTime, TimeSpan quarantineDuration, string quarantineReason)
		{
			this.mailboxGuid = mailboxGuid;
			this.crashCount = crashCount;
			this.lastCrashTime = lastCrashTime;
			this.quarantineDuration = quarantineDuration;
			this.quarantineReason = PrequarantinedMailbox.TruncateQuarantineReason(quarantineReason);
		}

		internal static readonly int QuarantineReasonLengthLimit = 8192;

		private readonly Guid mailboxGuid;

		private readonly int crashCount;

		private readonly DateTime lastCrashTime;

		private readonly TimeSpan quarantineDuration;

		private readonly string quarantineReason;
	}
}
