using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UmPasswordManager
	{
		internal UmPasswordManager(UMMailboxRecipient mailbox)
		{
			this.impl = mailbox.ConfigFolder.OpenPassword();
			this.mailbox = mailbox;
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(this.mailbox.ADUser);
			UMMailboxPolicy policyFromRecipient = iadsystemConfigurationLookup.GetPolicyFromRecipient(this.mailbox.ADUser);
			if (policyFromRecipient == null)
			{
				throw new ADUMUserInvalidUMMailboxPolicyException(mailbox.MailAddress);
			}
			this.policy = new PasswordPolicy(policyFromRecipient);
		}

		internal UmPasswordManager(UMMailboxRecipient mailbox, UMMailboxPolicy mbxPolicy)
		{
			ValidateArgument.NotNull(mailbox, "mailbox");
			ValidateArgument.NotNull(mbxPolicy, "mbxPolicy");
			this.impl = mailbox.ConfigFolder.OpenPassword();
			this.mailbox = mailbox;
			this.policy = new PasswordPolicy(mbxPolicy);
		}

		internal bool PinResetNeeded
		{
			get
			{
				return this.policy.LogonFailuresBeforePINReset > 0 && 0 == this.impl.LockoutCount % this.policy.LogonFailuresBeforePINReset;
			}
		}

		internal bool BadChecksum
		{
			get
			{
				Checksum checksum = new Checksum(this.mailbox, this.impl);
				return !checksum.IsValid;
			}
		}

		internal bool PinWasResetRecently
		{
			get
			{
				TimeSpan timeSpan = ExDateTime.UtcNow - this.UtcTimeSet;
				return timeSpan.TotalHours < 2.0 || timeSpan.TotalDays >= 36500.0;
			}
		}

		internal ExDateTime UtcTimeSet
		{
			get
			{
				return this.impl.TimeSet;
			}
		}

		internal bool IsExpired
		{
			get
			{
				int num = this.policy.DaysBeforeExpiry;
				if (num == 0)
				{
					num = 36500;
				}
				return this.impl.TimeSet.AddDays((double)num) < ExDateTime.UtcNow;
			}
		}

		internal bool IsLocked
		{
			get
			{
				return this.policy.LogonFailuresBeforeLockout != 0 && (this.impl.LockoutCount >= this.policy.LogonFailuresBeforeLockout || this.GetOfflineLockoutCount() >= this.policy.LogonFailuresBeforeLockout);
			}
		}

		internal bool Authenticate(EncryptedBuffer digits)
		{
			PIIMessage data = PIIMessage.Create(PIIType._User, this.mailbox);
			if (this.IsLocked)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, data, "Um Logon failed because UmUser=_User has a locked mailbox.", new object[0]);
				return false;
			}
			if (this.BadChecksum)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, data, "Um Logon failed because UmUser=_User has an invalid checksum.", new object[0]);
				return false;
			}
			PasswordBlob passwordBlob = this.impl.CurrentPassword;
			if (null == passwordBlob)
			{
				throw new UserConfigurationException(Strings.CorruptedPasswordField(this.mailbox.ToString()));
			}
			bool flag = passwordBlob.Equals(digits);
			bool result;
			try
			{
				if (!flag)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, data, "Um Logon failed because UmUser=_User typed an incorrect password.", new object[0]);
					this.impl.LockoutCount++;
					this.impl.Commit();
				}
				else
				{
					this.impl.LockoutCount = 0;
					this.ClearOfflineLockoutCount();
					if (string.Compare(passwordBlob.Algorithm, "SHA256", StringComparison.OrdinalIgnoreCase) != 0 || passwordBlob.Iterations != 1000)
					{
						passwordBlob = new PasswordBlob(digits, "SHA256", 1000);
						this.impl.CurrentPassword = passwordBlob;
						this.CommitPasswordAndUpdateChecksum();
					}
					else
					{
						this.impl.Commit();
					}
				}
				result = flag;
			}
			catch (QuotaExceededException)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "Password manager could not update mailbox lockout count because the user is over quota.", new object[0]);
				if (!flag)
				{
					this.impl.LockoutCount--;
					this.IncrementOfflineLockoutCount();
				}
				result = flag;
			}
			return result;
		}

		internal void SetPassword(EncryptedBuffer digits, bool isExpired, LockOutResetMode lockoutResetMode)
		{
			PIIMessage data = PIIMessage.Create(PIIType._User, this.mailbox);
			CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, data, "In SetPassword for UmUser=_User.", new object[0]);
			PasswordBlob currentPassword = this.impl.CurrentPassword;
			PasswordBlob currentPassword2 = new PasswordBlob(digits, "SHA256", 1000);
			this.impl.CurrentPassword = currentPassword2;
			this.impl.TimeSet = ExDateTime.UtcNow;
			this.impl.TimeSet = (isExpired ? ExDateTime.UtcNow.AddDays(-36501.0) : ExDateTime.UtcNow);
			if (null != currentPassword)
			{
				ArrayList oldPasswords = this.impl.OldPasswords;
				oldPasswords.Add(currentPassword);
				int num = Math.Max(0, this.policy.PreviousPasswordsDisallowed - 1);
				while (oldPasswords.Count > num)
				{
					oldPasswords.RemoveAt(0);
				}
				this.impl.OldPasswords = oldPasswords;
			}
			if (lockoutResetMode == LockOutResetMode.LockedOut)
			{
				this.impl.LockoutCount = this.policy.LogonFailuresBeforeLockout + 1;
			}
			else if (lockoutResetMode == LockOutResetMode.Reset)
			{
				this.impl.LockoutCount = 0;
				this.ClearOfflineLockoutCount();
			}
			this.CommitPasswordAndUpdateChecksum();
		}

		internal void SetPassword(EncryptedBuffer digits)
		{
			this.SetPassword(digits, false, LockOutResetMode.Reset);
		}

		internal void RequirePasswordToChangeAtFirstUse()
		{
			this.impl.TimeSet = ExDateTime.UtcNow.AddDays(-36501.0);
			this.CommitPasswordAndUpdateChecksum();
		}

		internal bool IsValidPassword(EncryptedBuffer pwd)
		{
			return !this.IsWeak(pwd) && !this.HasAlreadyBeenUsed(pwd);
		}

		internal void UnlockMailbox()
		{
			this.impl.LockoutCount = 0;
			this.ClearOfflineLockoutCount();
			this.impl.Commit();
		}

		internal bool IsWeak(EncryptedBuffer pwd)
		{
			bool result;
			using (SafeBuffer decrypted = pwd.Decrypted)
			{
				if (this.IsTooShort(decrypted.Buffer))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "password is too short.", new object[0]);
					result = true;
				}
				else if (UmPasswordManager.HasInvalidDigits(decrypted.Buffer))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "password has invalid digits.", new object[0]);
					result = true;
				}
				else if (this.NotComplex(decrypted.Buffer))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "password is not complex.", new object[0]);
					result = true;
				}
				else if (this.IsUserExtension(decrypted.Buffer))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "password is user extension.", new object[0]);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal byte[] GenerateValidPassword()
		{
			byte[] array = null;
			int num = 100;
			while (--num > 0)
			{
				array = this.GetRandomPassword(this.policy.MinimumLength);
				EncryptedBuffer pwd = new EncryptedBuffer(array);
				if (this.IsValidPassword(pwd))
				{
					break;
				}
			}
			if (num == 0)
			{
				array = null;
			}
			return array;
		}

		private static bool HasInvalidDigits(byte[] pwd)
		{
			foreach (byte b in pwd)
			{
				if (b < 48 || b > 57)
				{
					return true;
				}
			}
			return false;
		}

		private byte[] GetRandomPassword(int len)
		{
			byte[] array = new byte[4];
			StringBuilder stringBuilder = new StringBuilder();
			while (stringBuilder.Length < len)
			{
				UmPasswordManager.rng.GetBytes(array);
				stringBuilder.Append((BitConverter.ToUInt32(array, 0) % 10U).ToString(CultureInfo.InvariantCulture));
			}
			string s = stringBuilder.ToString().Substring(0, len);
			return Encoding.ASCII.GetBytes(s);
		}

		private void CommitPasswordAndUpdateChecksum()
		{
			Checksum checksum = new Checksum(this.mailbox, this.impl);
			this.impl.Commit();
			checksum.Update();
		}

		private bool HasAlreadyBeenUsed(EncryptedBuffer pwd)
		{
			ArrayList oldPasswords = this.impl.OldPasswords;
			int num = this.policy.PreviousPasswordsDisallowed - 1;
			int num2 = oldPasswords.Count - 1;
			while (num2 >= 0 && num > 0)
			{
				PasswordBlob passwordBlob = (PasswordBlob)oldPasswords[num2];
				if (passwordBlob.Equals(pwd))
				{
					return true;
				}
				num2--;
				num--;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "password not found in old password list.", new object[0]);
			PasswordBlob currentPassword = this.impl.CurrentPassword;
			return !(null == currentPassword) && currentPassword.Equals(pwd);
		}

		private bool IsTooShort(byte[] pwd)
		{
			if (pwd.Length < this.policy.MinimumLength)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "pwd does not meet minimum length policy.", new object[0]);
				return true;
			}
			return false;
		}

		private bool NotComplex(byte[] pwd)
		{
			if (pwd == null || pwd.Length < 2)
			{
				return false;
			}
			short num = (short)pwd[0];
			short num2 = (short)pwd[1];
			int num3 = (int)(num2 - num);
			for (int i = 2; i < pwd.Length; i++)
			{
				num = num2;
				num2 = (short)pwd[i];
				if ((int)(num2 - num) != num3)
				{
					return false;
				}
			}
			return (-1 == num3 || num3 == 0 || 1 == num3) && !this.policy.AllowCommonPatterns;
		}

		private bool IsUserExtension(byte[] pwd)
		{
			if (string.IsNullOrEmpty(this.mailbox.ADRecipient.UMExtension))
			{
				return false;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(this.mailbox.ADRecipient.UMExtension);
			int num = pwd.Length - 1;
			int num2 = bytes.Length - 1;
			while (num >= 0 && num2 >= 0)
			{
				if (pwd[num--] != bytes[num2--])
				{
					return false;
				}
			}
			return 0 > num;
		}

		private int GetOfflineLockoutCount()
		{
			int result = 0;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
			{
				if (UmPasswordManager.offlineLogonFailures.ContainsKey(this.mailbox.ExchangeLegacyDN) && XsoUtil.IsOverReceiveQuota(mailboxSessionLock.Session.Mailbox, 0UL))
				{
					lock (UmPasswordManager.offlineLogonFailures)
					{
						if (!UmPasswordManager.offlineLogonFailures.TryGetValue(this.mailbox.ExchangeLegacyDN, out result))
						{
							result = 0;
						}
						goto IL_78;
					}
				}
				this.ClearOfflineLockoutCount();
				result = 0;
				IL_78:;
			}
			return result;
		}

		private void ClearOfflineLockoutCount()
		{
			lock (UmPasswordManager.offlineLogonFailures)
			{
				UmPasswordManager.offlineLogonFailures.Remove(this.mailbox.ExchangeLegacyDN);
			}
		}

		private void IncrementOfflineLockoutCount()
		{
			lock (UmPasswordManager.offlineLogonFailures)
			{
				int val = 0;
				if (!UmPasswordManager.offlineLogonFailures.TryGetValue(this.mailbox.ExchangeLegacyDN, out val))
				{
					val = 0;
				}
				UmPasswordManager.offlineLogonFailures[this.mailbox.ExchangeLegacyDN] = Math.Max(val, this.impl.LockoutCount) + 1;
			}
		}

		private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

		private static Dictionary<string, int> offlineLogonFailures = new Dictionary<string, int>();

		private IPassword impl;

		private UMMailboxRecipient mailbox;

		private PasswordPolicy policy;
	}
}
