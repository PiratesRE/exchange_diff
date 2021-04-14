using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	public interface IConfigDataProvider
	{
		IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new();

		IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new();

		IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new();

		void Save(IConfigurable instance);

		void Delete(IConfigurable instance);

		string Source { get; }
	}
}
