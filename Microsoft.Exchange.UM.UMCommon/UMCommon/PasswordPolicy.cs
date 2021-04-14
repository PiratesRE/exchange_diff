using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class PasswordPolicy
	{
		internal PasswordPolicy(UMMailboxPolicy policy)
		{
			this.minimumLength = policy.MinPINLength;
			this.daysBeforeExpiry = (policy.PINLifetime.IsUnlimited ? 0 : Convert.ToInt32(policy.PINLifetime.Value.TotalDays));
			this.logonFailuresBeforeLockout = (policy.MaxLogonAttempts.IsUnlimited ? 0 : policy.MaxLogonAttempts.Value);
			this.previousPasswordsDisallowed = policy.PINHistoryCount;
			this.allowCommonPatterns = policy.AllowCommonPatterns;
			this.logonFailuresBeforePINReset = (policy.LogonFailuresBeforePINReset.IsUnlimited ? 0 : policy.LogonFailuresBeforePINReset.Value);
		}

		internal int MinimumLength
		{
			get
			{
				return this.minimumLength;
			}
			set
			{
				this.minimumLength = value;
			}
		}

		internal int DaysBeforeExpiry
		{
			get
			{
				return this.daysBeforeExpiry;
			}
			set
			{
				this.daysBeforeExpiry = value;
			}
		}

		internal int LogonFailuresBeforePINReset
		{
			get
			{
				return this.logonFailuresBeforePINReset;
			}
		}

		internal int LogonFailuresBeforeLockout
		{
			get
			{
				return this.logonFailuresBeforeLockout;
			}
		}

		internal int PreviousPasswordsDisallowed
		{
			get
			{
				return this.previousPasswordsDisallowed;
			}
		}

		internal bool AllowCommonPatterns
		{
			get
			{
				return this.allowCommonPatterns;
			}
		}

		private int minimumLength;

		private int daysBeforeExpiry;

		private int logonFailuresBeforeLockout;

		private int logonFailuresBeforePINReset;

		private int previousPasswordsDisallowed;

		private bool allowCommonPatterns;
	}
}
