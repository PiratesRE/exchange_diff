using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxStoreIdParameter : IIdentityParameter
	{
		protected string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
			set
			{
				this.rawIdentity = value;
			}
		}

		public MailboxStoreIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity.Length == 0)
			{
				throw new ArgumentException(Strings.ErrorEmptyParameter(base.GetType().ToString()), "identity");
			}
			this.rawIdentity = identity;
			this.name = identity;
		}

		public MailboxStoreIdParameter(Mailbox mailbox)
		{
			if (mailbox == null)
			{
				throw new ArgumentNullException("mailbox");
			}
			if (mailbox.Identity == null)
			{
				throw new ArgumentNullException("mailbox.Identity");
			}
			this.rawIdentity = mailbox.Id.ToString();
			this.name = mailbox.Id.Name;
		}

		public MailboxStoreIdParameter(ADObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			this.rawIdentity = objectId.ToString();
			this.name = objectId.Name;
		}

		public MailboxStoreIdParameter(MailboxStoreIdentity objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			this.rawIdentity = objectId.ToString();
			this.name = objectId.MailboxOwnerId.Name;
		}

		public MailboxStoreIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		public MailboxStoreIdParameter()
		{
		}

		public static MailboxStoreIdParameter Parse(string identity)
		{
			return new MailboxStoreIdParameter(identity);
		}

		public virtual bool IsFullyQualified
		{
			get
			{
				return !string.IsNullOrEmpty(this.name);
			}
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		public override string ToString()
		{
			return this.RawIdentity;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (this.IsFullyQualified)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
			}
			else
			{
				notFoundReason = null;
			}
			QueryFilter queryFilter = this.GetQueryFilter(session);
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					optionalData.AdditionalFilter
				});
			}
			return session.FindPaged<T>(queryFilter, rootId, false, null, 0);
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			MailboxStoreIdentity mailboxStoreIdentity = objectId as MailboxStoreIdentity;
			if (mailboxStoreIdentity == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterType("objectId", typeof(MailboxStoreIdentity).Name), "objectId");
			}
			this.rawIdentity = mailboxStoreIdentity.ToString();
			this.name = mailboxStoreIdentity.MailboxOwnerId.Name;
		}

		public virtual string GetADUserName()
		{
			return this.RawIdentity;
		}

		public virtual QueryFilter GetQueryFilter(IConfigDataProvider session)
		{
			return null;
		}

		private string name;

		private string rawIdentity;
	}
}
