using System;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class RemoveObjectTask<TIdentity, TDataObject> : RemoveTaskBase<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : ConfigObject, new()
	{
		protected override IConfigDataProvider CreateSession()
		{
			DataSourceManager[] dataSourceManagers = DataSourceManager.GetDataSourceManagers(typeof(TDataObject), "Identity");
			return dataSourceManagers[0];
		}
	}
}
