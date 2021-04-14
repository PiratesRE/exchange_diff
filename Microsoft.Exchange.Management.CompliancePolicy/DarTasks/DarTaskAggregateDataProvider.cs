using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	internal sealed class DarTaskAggregateDataProvider : IConfigDataProvider
	{
		public DarTaskAggregateDataProvider(DarTaskAggregateParams darParams, string serverFqdn)
		{
			this.darParams = darParams;
			this.serverFqdn = serverFqdn;
		}

		public string Source
		{
			get
			{
				return typeof(HostRpcClient).FullName;
			}
		}

		public void Delete(IConfigurable instance)
		{
			throw new NotSupportedException();
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return this.FindPaged<TaskAggregateStoreObject>(filter, rootId, deepSearch, sortBy, 0).ToArray<TaskAggregateStoreObject>();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			using (HostRpcClient client = new HostRpcClient(this.serverFqdn))
			{
				foreach (TaskAggregateStoreObject task in client.GetDarTaskAggregate(this.darParams))
				{
					yield return (T)((object)task);
				}
			}
			yield break;
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			using (HostRpcClient hostRpcClient = new HostRpcClient(this.serverFqdn))
			{
				DarTaskAggregateParams darTaskAggregateParams = new DarTaskAggregateParams
				{
					TaskId = this.darParams.TaskId,
					TenantId = this.darParams.TenantId
				};
				TaskAggregateStoreObject[] darTaskAggregate = hostRpcClient.GetDarTaskAggregate(darTaskAggregateParams);
				int num = 0;
				if (num < darTaskAggregate.Length)
				{
					TaskAggregateStoreObject taskAggregateStoreObject = darTaskAggregate[num];
					return (T)((object)taskAggregateStoreObject);
				}
			}
			return null;
		}

		public void Save(IConfigurable instance)
		{
			using (HostRpcClient hostRpcClient = new HostRpcClient(this.serverFqdn))
			{
				hostRpcClient.SetDarTaskAggregate((TaskAggregateStoreObject)instance);
			}
		}

		private readonly DarTaskAggregateParams darParams;

		private readonly string serverFqdn;
	}
}
