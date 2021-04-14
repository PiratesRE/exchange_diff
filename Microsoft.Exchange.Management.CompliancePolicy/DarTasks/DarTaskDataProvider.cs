using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	internal sealed class DarTaskDataProvider : IConfigDataProvider
	{
		public DarTaskDataProvider(DarTaskParams darParams, string serverFqdn)
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
			return this.FindPaged<TaskStoreObject>(filter, rootId, deepSearch, sortBy, 0).ToArray<TaskStoreObject>();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			using (HostRpcClient client = new HostRpcClient(this.serverFqdn))
			{
				foreach (TaskStoreObject task in client.GetDarTask(this.darParams))
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
				DarTaskParams darTaskParams = new DarTaskParams
				{
					TaskId = this.darParams.TaskId,
					TenantId = this.darParams.TenantId
				};
				TaskStoreObject[] darTask = hostRpcClient.GetDarTask(darTaskParams);
				int num = 0;
				if (num < darTask.Length)
				{
					TaskStoreObject taskStoreObject = darTask[num];
					return (T)((object)taskStoreObject);
				}
			}
			return null;
		}

		public void Save(IConfigurable instance)
		{
			using (HostRpcClient hostRpcClient = new HostRpcClient(this.serverFqdn))
			{
				hostRpcClient.SetDarTask((TaskStoreObject)instance);
			}
		}

		private readonly DarTaskParams darParams;

		private readonly string serverFqdn;
	}
}
