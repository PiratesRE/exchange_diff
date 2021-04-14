using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IRequestIndexEntryHandler
	{
		IRequestIndexEntry CreateRequestIndexEntryFromRequestJob(RequestJobBase requestJob, RequestIndexId requestIndexId);

		IRequestIndexEntry CreateRequestIndexEntryFromRequestJob(RequestJobBase requestJob, IConfigurationSession session);

		void Delete(RequestIndexEntryProvider requestIndexEntryProvider, IRequestIndexEntry instance);

		IRequestIndexEntry[] Find(RequestIndexEntryProvider requestIndexEntryProvider, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy);

		IEnumerable<IRequestIndexEntry> FindPaged(RequestIndexEntryProvider requestIndexEntryProvider, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize);

		IRequestIndexEntry Read(RequestIndexEntryProvider requestIndexEntryProvider, RequestIndexEntryObjectId identity);

		void Save(RequestIndexEntryProvider requestIndexEntryProvider, IRequestIndexEntry instance);
	}
}
