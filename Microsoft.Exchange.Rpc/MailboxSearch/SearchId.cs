using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	internal sealed class SearchId
	{
		public SearchId(string mailboxDsName, Guid mailboxGuid, string searchName)
		{
			this.m_mailboxDsName = mailboxDsName;
			this.m_mailboxGuid = mailboxGuid;
			this.m_searchName = searchName;
		}

		public string MailboxDsName
		{
			get
			{
				return this.m_mailboxDsName;
			}
			set
			{
				this.m_mailboxDsName = value;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.m_mailboxGuid;
			}
			set
			{
				this.m_mailboxGuid = value;
			}
		}

		public string SearchName
		{
			get
			{
				return this.m_searchName;
			}
			set
			{
				this.m_searchName = value;
			}
		}

		public sealed override int GetHashCode()
		{
			int hashCode = this.m_mailboxGuid.GetHashCode();
			if (this.m_mailboxGuid.Equals(Guid.Empty) && !string.IsNullOrEmpty(this.m_mailboxDsName))
			{
				hashCode = this.m_mailboxDsName.ToLowerInvariant().GetHashCode();
			}
			return this.m_searchName.ToLowerInvariant().GetHashCode() ^ hashCode;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public sealed override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			SearchId searchId = (SearchId)obj;
			string searchName = searchId.m_searchName;
			if (!this.m_searchName.Equals(searchName, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			Guid mailboxGuid = this.m_mailboxGuid;
			if (!mailboxGuid.Equals(Guid.Empty))
			{
				Guid mailboxGuid2 = searchId.m_mailboxGuid;
				if (!mailboxGuid2.Equals(Guid.Empty))
				{
					Guid mailboxGuid3 = searchId.m_mailboxGuid;
					Guid mailboxGuid4 = this.m_mailboxGuid;
					return mailboxGuid4.Equals(mailboxGuid3);
				}
			}
			if (this.m_mailboxDsName == null)
			{
				return searchId.m_mailboxDsName == null;
			}
			string mailboxDsName = searchId.m_mailboxDsName;
			return this.m_mailboxDsName.Equals(mailboxDsName, StringComparison.InvariantCultureIgnoreCase);
		}

		private string m_mailboxDsName;

		private Guid m_mailboxGuid;

		private string m_searchName;
	}
}
