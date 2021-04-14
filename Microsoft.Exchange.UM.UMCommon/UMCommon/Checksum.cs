using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class Checksum
	{
		internal Checksum(UMMailboxRecipient user, IPassword pwdImpl)
		{
			this.pwdImpl = pwdImpl;
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromADRecipient(user.ADRecipient, false);
			ADRecipient adrecipient = iadrecipientLookup.LookupByObjectId(user.ADRecipient.Id);
			this.adUser = (adrecipient as ADUser);
			if (this.adUser == null)
			{
				throw new UmUserException(Strings.ADAccessFailed);
			}
		}

		internal bool IsValid
		{
			get
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "Validating checksum.", new object[0]);
				byte[] umpinChecksum = this.adUser.UMPinChecksum;
				byte[] array = this.Calculate();
				if (umpinChecksum == null)
				{
					return false;
				}
				bool flag = umpinChecksum.Length == array.Length;
				int num = 0;
				while (flag && num < umpinChecksum.Length)
				{
					flag = (umpinChecksum[num] == array[num]);
					num++;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "Checksum.IsValid returning {0}.", new object[]
				{
					flag
				});
				return flag;
			}
		}

		internal void Update()
		{
			this.Update(this.Calculate());
		}

		private void Update(byte[] calculated)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "Updating checksum.", new object[0]);
			if (this.adUser.UMPinChecksum != calculated)
			{
				this.adUser.UMPinChecksum = calculated;
				this.adUser.Session.Save(this.adUser);
			}
		}

		private byte[] Calculate()
		{
			SHA1Cng sha1Cng = new SHA1Cng();
			byte[] result;
			using (CryptoStream cryptoStream = new CryptoStream(Stream.Null, sha1Cng, CryptoStreamMode.Write))
			{
				PasswordBlob currentPassword = this.pwdImpl.CurrentPassword;
				if (null != currentPassword)
				{
					cryptoStream.Write(currentPassword.Blob, 0, currentPassword.Blob.Length);
				}
				foreach (object obj in this.pwdImpl.OldPasswords)
				{
					PasswordBlob passwordBlob = (PasswordBlob)obj;
					cryptoStream.Write(passwordBlob.Blob, 0, passwordBlob.Blob.Length);
				}
				long utcTicks = this.pwdImpl.TimeSet.UtcTicks;
				byte[] bytes = BitConverter.GetBytes(utcTicks);
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
				byte[] array = new byte[160];
				sha1Cng.Hash.CopyTo(array, 0);
				result = array;
			}
			return result;
		}

		private ADUser adUser;

		private IPassword pwdImpl;
	}
}
