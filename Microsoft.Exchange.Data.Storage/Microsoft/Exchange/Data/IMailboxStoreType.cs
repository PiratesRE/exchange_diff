using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Data
{
	public interface IMailboxStoreType : IConfigurable
	{
		void Save(MailboxStoreTypeProvider session);

		void Delete(MailboxStoreTypeProvider session);

		IConfigurable Read(MailboxStoreTypeProvider session, ObjectId identity);

		IConfigurable[] Find(MailboxStoreTypeProvider session, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy);

		IEnumerable<T> FindPaged<T>(MailboxStoreTypeProvider session, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize);
	}
}
