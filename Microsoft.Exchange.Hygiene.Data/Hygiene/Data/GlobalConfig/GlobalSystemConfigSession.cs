using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.GlobalConfig
{
	internal class GlobalSystemConfigSession
	{
		internal GlobalSystemConfigSession()
		{
			this.webStoreDataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Spam);
		}

		internal IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return this.webStoreDataProvider.Find<T>(filter, rootId, deepSearch, sortBy).ToArray<IConfigurable>();
		}

		internal void Save(IConfigurable configurable)
		{
			this.webStoreDataProvider.Save(configurable);
		}

		internal void Delete(IConfigurable configurable)
		{
			if (configurable is DataCenterSettings)
			{
				throw new ArgumentException(string.Format("Delete operation is not supported for the type {0}.", configurable.GetType().ToString()));
			}
			this.webStoreDataProvider.Delete(configurable);
		}

		private IConfigDataProvider webStoreDataProvider;
	}
}
