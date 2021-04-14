using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MailboxStoreDataProvider : IConfigDataProvider
	{
		public ADUser ADUser
		{
			get
			{
				return this.adUser;
			}
		}

		public MailboxStoreDataProvider(ADUser adUser)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser");
			}
			this.adUser = adUser;
		}

		public abstract IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new();

		public abstract IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new();

		public abstract IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new();

		public abstract void Save(IConfigurable instance);

		public abstract void Delete(IConfigurable instance);

		public virtual string Source
		{
			get
			{
				return string.Empty;
			}
		}

		private ADUser adUser;
	}
}
