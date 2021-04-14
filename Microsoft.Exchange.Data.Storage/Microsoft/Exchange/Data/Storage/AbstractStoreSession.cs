using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractStoreSession : IStoreSession, IDisposable
	{
		public bool IsMoveUser
		{
			get
			{
				return false;
			}
		}

		public virtual IExchangePrincipal MailboxOwner
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IActivitySession ActivitySession
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual CultureInfo Culture
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string DisplayAddress
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

		public virtual IdConverter IdConverter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual AggregateOperationResult Delete(DeleteItemFlags deleteFlags, params StoreId[] ids)
		{
			throw new NotImplementedException();
		}

		public virtual void Dispose()
		{
			throw new NotImplementedException();
		}

		public virtual IXSOMailbox Mailbox
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

		public virtual LogonType LogonType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual SessionCapabilities Capabilities
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual Guid MdbGuid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExTimeZone ExTimeZone
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual StoreObjectId GetParentFolderId(StoreObjectId objectId)
		{
			throw new NotImplementedException();
		}

		public IRecipientSession GetADRecipientSession(bool isReadOnly, ConsistencyMode consistencyMode)
		{
			throw new NotImplementedException();
		}

		public IConfigurationSession GetADConfigurationSession(bool isReadOnly, ConsistencyMode consistencyMode)
		{
			throw new NotImplementedException();
		}
	}
}
