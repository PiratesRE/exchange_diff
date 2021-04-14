using System;
using Microsoft.Exchange.Data.Mapi.Common;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public sealed class MailboxId : MessageStoreId
	{
		public string MailboxExchangeLegacyDn
		{
			get
			{
				return this.mailboxExchangeLegacyDn;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public DatabaseId MailboxDatabaseId
		{
			get
			{
				return this.mailboxDatabaseId;
			}
		}

		public static MailboxId Parse(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new FormatException(Strings.ExceptionFormatNotSupported);
			}
			Guid guid;
			Guid guid2;
			if (73 == input.Length && '\\' == input[36] && GuidHelper.TryParseGuid(input.Substring(0, 36), out guid) && GuidHelper.TryParseGuid(input.Substring(37, 36), out guid2))
			{
				return new MailboxId(new DatabaseId(guid), guid2);
			}
			if (GuidHelper.TryParseGuid(input, out guid2))
			{
				return new MailboxId(null, guid2);
			}
			MailboxId result;
			try
			{
				result = new MailboxId((byte[])new MapiEntryId(input));
			}
			catch (FormatException)
			{
				result = new MailboxId(input);
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (this.mailboxGuid != Guid.Empty)
			{
				result = this.Equals(obj as MailboxId);
			}
			else
			{
				result = base.Equals(obj);
			}
			return result;
		}

		public override bool Equals(MapiObjectId other)
		{
			bool result = false;
			if (this.mailboxGuid != Guid.Empty)
			{
				MailboxId mailboxId = other as MailboxId;
				if (mailboxId != null)
				{
					result = this.MailboxGuid.Equals(mailboxId.MailboxGuid);
				}
			}
			else
			{
				result = base.Equals(other);
			}
			return result;
		}

		public override int GetHashCode()
		{
			Guid guid = this.MailboxGuid;
			int hashCode;
			if (this.MailboxGuid != Guid.Empty)
			{
				hashCode = this.MailboxGuid.GetHashCode();
			}
			else
			{
				hashCode = base.GetHashCode();
			}
			return hashCode;
		}

		public override string ToString()
		{
			if (Guid.Empty != this.MailboxGuid)
			{
				return this.MailboxGuid.ToString();
			}
			if (this.MailboxExchangeLegacyDn != null)
			{
				return this.MailboxExchangeLegacyDn;
			}
			if (null != base.MapiEntryId)
			{
				return base.MapiEntryId.ToString();
			}
			return null;
		}

		public MailboxId()
		{
		}

		public MailboxId(byte[] bytes) : base(bytes)
		{
		}

		public MailboxId(MapiEntryId entryId) : base(entryId)
		{
		}

		public MailboxId(string mailboxExchangeLegacyDn)
		{
			this.mailboxExchangeLegacyDn = mailboxExchangeLegacyDn;
		}

		public MailboxId(DatabaseId mailboxDatabaseId, Guid mailboxGuid)
		{
			this.mailboxDatabaseId = mailboxDatabaseId;
			this.mailboxGuid = mailboxGuid;
		}

		internal MailboxId(MapiEntryId entryId, DatabaseId mailboxDatabaseId, Guid mailboxGuid, string mailboxExchangeLegacyDn) : base(entryId)
		{
			this.mailboxDatabaseId = mailboxDatabaseId;
			this.mailboxGuid = mailboxGuid;
			this.mailboxExchangeLegacyDn = mailboxExchangeLegacyDn;
		}

		private const int LiteralGuidLength = 36;

		private readonly string mailboxExchangeLegacyDn;

		private readonly DatabaseId mailboxDatabaseId;

		private readonly Guid mailboxGuid;
	}
}
