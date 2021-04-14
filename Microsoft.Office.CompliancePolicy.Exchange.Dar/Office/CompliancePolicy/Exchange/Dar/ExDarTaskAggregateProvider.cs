using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Utility;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar
{
	internal class ExDarTaskAggregateProvider : DarTaskAggregateProvider
	{
		public ExDarTaskAggregateProvider(DarServiceProvider provider)
		{
			this.provider = provider;
		}

		public override void Delete(string scopeId, string taskType)
		{
			InstanceManager.Current.TaskAggregates.Remove(scopeId, taskType, OperationContext.CorrelationId);
		}

		public override DarTaskAggregate Find(string scopeId, string taskType)
		{
			return InstanceManager.Current.TaskAggregates.Get(scopeId, taskType, OperationContext.CorrelationId);
		}

		public override IEnumerable<DarTaskAggregate> FindAll(string scopeId)
		{
			return from type in this.provider.DarTaskFactory.GetAllTaskTypes()
			select this.Find(scopeId, type);
		}

		public override void Save(DarTaskAggregate taskAggregate)
		{
			TaskHelper.Validate(taskAggregate, this.provider);
			InstanceManager.Current.TaskAggregates.Set(taskAggregate, OperationContext.CorrelationId);
		}

		private DarServiceProvider provider;
	}
}
