using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Storage
{
	public sealed class MailboxStoreTypeProvider : IConfigDataProvider
	{
		public ADUser ADUser
		{
			get
			{
				return this.adUser;
			}
		}

		internal MailboxSession MailboxSession { get; set; }

		public MailboxStoreTypeProvider(ADUser adUser)
		{
			this.adUser = adUser;
			this.source = string.Empty;
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			IMailboxStoreType mailboxStoreType = t as IMailboxStoreType;
			return mailboxStoreType.Read(this, identity);
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			IMailboxStoreType mailboxStoreType = t as IMailboxStoreType;
			return mailboxStoreType.Find(this, filter, rootId, deepSearch, sortBy);
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			IMailboxStoreType mailboxStoreType = t as IMailboxStoreType;
			return mailboxStoreType.FindPaged<T>(this, filter, rootId, deepSearch, sortBy, pageSize);
		}

		public void Save(IConfigurable instance)
		{
			IMailboxStoreType mailboxStoreType = instance as IMailboxStoreType;
			mailboxStoreType.Save(this);
		}

		public void Delete(IConfigurable instance)
		{
			IMailboxStoreType mailboxStoreType = instance as IMailboxStoreType;
			mailboxStoreType.Delete(this);
		}

		public string Source
		{
			get
			{
				return this.source ?? string.Empty;
			}
		}

		private string source;

		private ADUser adUser;
	}
}
