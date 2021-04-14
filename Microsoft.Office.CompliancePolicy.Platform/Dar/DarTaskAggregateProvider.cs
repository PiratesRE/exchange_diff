using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public abstract class DarTaskAggregateProvider
	{
		public abstract DarTaskAggregate Find(string scopeId, string taskType);

		public abstract IEnumerable<DarTaskAggregate> FindAll(string scopeId);

		public abstract void Save(DarTaskAggregate taskAggregate);

		public abstract void Delete(string scopeId, string id);
	}
}
