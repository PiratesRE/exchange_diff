using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AbstractMailboxInfo : IMailboxInfo
	{
		public virtual string DisplayName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual SmtpAddress PrimarySmtpAddress
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ProxyAddress ExternalEmailAddress
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IEnumerable<ProxyAddress> EmailAddresses
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual OrganizationId OrganizationId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual Guid MailboxGuid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ADObjectId MailboxDatabase
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual DateTime? WhenMailboxCreated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string ArchiveName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsArchive
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsAggregated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ArchiveState ArchiveState
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual SmtpAddress? RemoteIdentity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsRemote
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IMailboxLocation Location
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IMailboxConfiguration Configuration
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual MailboxLocationType MailboxType
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
