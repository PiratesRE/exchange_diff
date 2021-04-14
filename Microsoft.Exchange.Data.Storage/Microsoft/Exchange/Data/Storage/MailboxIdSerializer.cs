using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MailboxIdSerializer
	{
		public static string EmailAddressFromBytes(byte[] moniker)
		{
			if (moniker.Length > 1000)
			{
				throw new InvalidIdMalformedException();
			}
			return Encoding.UTF8.GetString(moniker, 0, moniker.Length);
		}

		public static int EmailAddressToByteCount(string smtpAddress)
		{
			return Encoding.UTF8.GetByteCount(smtpAddress);
		}

		public static int EmailAddressToBytes(string smtpAddress, byte[] bytes, int index)
		{
			return Encoding.UTF8.GetBytes(smtpAddress, 0, smtpAddress.Length, bytes, index);
		}

		public static Guid MailboxGuidFromBytes(byte[] moniker)
		{
			if (moniker.Length > 50)
			{
				throw new InvalidIdMalformedException();
			}
			return new Guid(Encoding.UTF8.GetString(moniker, 0, moniker.Length));
		}

		public static int MailboxGuidToByteCount(string mailboxGuid)
		{
			return Encoding.UTF8.GetByteCount(mailboxGuid);
		}

		public static int MailboxGuidToBytes(string mailboxGuid, byte[] bytes, int index)
		{
			return Encoding.UTF8.GetBytes(mailboxGuid, 0, mailboxGuid.Length, bytes, index);
		}

		private const int MaxEmailAddressLength = 1000;

		private const int MaxMailboxGuidLength = 50;
	}
}
