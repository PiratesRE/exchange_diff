using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution
{
	public class ActiveTasks
	{
		public bool Contains(string tenantId)
		{
			return this.backingStore.ContainsKey(tenantId);
		}

		public IEnumerable<string> GetKnownTenants()
		{
			return this.backingStore.Keys;
		}

		public IEnumerable<DarTask> GetByTenantOrAll(string tenantId = null)
		{
			if (tenantId != null)
			{
				return this.backingStore[tenantId].Values;
			}
			return this.backingStore.Values.SelectMany((Dictionary<string, DarTask> dic) => dic.Values);
		}

		public Dictionary<string, DarTask> GetByTenant(string tenantId)
		{
			Dictionary<string, DarTask> dictionary;
			this.backingStore.TryGetValue(tenantId, out dictionary);
			return dictionary ?? new Dictionary<string, DarTask>();
		}

		public void Update(string tenantId, Dictionary<string, DarTask> newGroup)
		{
			if (newGroup.Count == 0)
			{
				Dictionary<string, DarTask> dictionary;
				this.backingStore.TryRemove(tenantId, out dictionary);
				return;
			}
			this.backingStore[tenantId] = newGroup;
		}

		private ConcurrentDictionary<string, Dictionary<string, DarTask>> backingStore = new ConcurrentDictionary<string, Dictionary<string, DarTask>>();
	}
}
