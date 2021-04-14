using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class RequestIndexEntryHandler<T> : IRequestIndexEntryHandler where T : class, IRequestIndexEntry
	{
		public virtual T CreateRequestIndexEntryFromRequestJob(RequestJobBase requestJob, IConfigurationSession session)
		{
			throw new NotSupportedException("Unless overridden, a handler does not support creating IRequestIndexEntries using IConfigurationSession.");
		}

		public virtual T CreateRequestIndexEntryFromRequestJob(RequestJobBase requestJob, RequestIndexId requestIndexId)
		{
			throw new NotSupportedException();
		}

		public abstract void Delete(RequestIndexEntryProvider requestIndexEntryProvider, T instance);

		public virtual T[] Find(RequestIndexEntryProvider requestIndexEntryProvider, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			return Array<T>.Empty;
		}

		public virtual IEnumerable<T> FindPaged(RequestIndexEntryProvider requestIndexEntryProvider, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			return Array<T>.Empty;
		}

		public abstract T Read(RequestIndexEntryProvider requestIndexEntryProvider, RequestIndexEntryObjectId identity);

		public abstract void Save(RequestIndexEntryProvider requestIndexEntryProvider, T instance);

		IRequestIndexEntry IRequestIndexEntryHandler.CreateRequestIndexEntryFromRequestJob(RequestJobBase requestJob, IConfigurationSession session)
		{
			return this.CreateRequestIndexEntryFromRequestJob(requestJob, session);
		}

		IRequestIndexEntry IRequestIndexEntryHandler.CreateRequestIndexEntryFromRequestJob(RequestJobBase requestJob, RequestIndexId requestIndexId)
		{
			return this.CreateRequestIndexEntryFromRequestJob(requestJob, requestIndexId);
		}

		void IRequestIndexEntryHandler.Delete(RequestIndexEntryProvider requestIndexEntryProvider, IRequestIndexEntry instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			this.Delete(requestIndexEntryProvider, (T)((object)instance));
		}

		IRequestIndexEntry[] IRequestIndexEntryHandler.Find(RequestIndexEntryProvider requestIndexEntryProvider, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			return (IRequestIndexEntry[])this.Find(requestIndexEntryProvider, filter, rootId, deepSearch, sortBy);
		}

		IEnumerable<IRequestIndexEntry> IRequestIndexEntryHandler.FindPaged(RequestIndexEntryProvider requestIndexEntryProvider, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			return this.FindPaged(requestIndexEntryProvider, filter, rootId, deepSearch, sortBy, pageSize);
		}

		IRequestIndexEntry IRequestIndexEntryHandler.Read(RequestIndexEntryProvider requestIndexEntryProvider, RequestIndexEntryObjectId identity)
		{
			Type typeFromHandle = typeof(T);
			if (identity.IndexId.RequestIndexEntryType != typeFromHandle)
			{
				throw new ArgumentException(string.Format("The provided identity is requesting an IRequestIndexEntry of type {0}, but this handler only supports type {1}.", identity.IndexId.RequestIndexEntryType.Name, typeFromHandle.Name), "identity");
			}
			return this.Read(requestIndexEntryProvider, identity);
		}

		void IRequestIndexEntryHandler.Save(RequestIndexEntryProvider requestIndexEntryProvider, IRequestIndexEntry instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			this.Save(requestIndexEntryProvider, (T)((object)instance));
		}
	}
}
