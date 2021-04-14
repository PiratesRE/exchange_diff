using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Office.CompliancePolicy.ComplianceTask
{
	public abstract class ComplianceTask : DarTask
	{
		public ComplianceTask()
		{
			this.complianceServiceProvider = null;
			this.TaskStatsData = new ComplianceTaskStatistics();
		}

		public virtual ComplianceServiceProvider ComplianceServiceProvider
		{
			get
			{
				return this.complianceServiceProvider;
			}
			set
			{
				this.complianceServiceProvider = value;
			}
		}

		public virtual string WorkloadData
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[SerializableTaskData]
		public ComplianceTaskStatistics TaskStatsData { get; set; }

		public virtual string GetWorkloadDataFromWorkload()
		{
			return string.Empty;
		}

		protected override IEnumerable<Type> GetKnownTypes()
		{
			return base.GetKnownTypes().Concat(new Type[]
			{
				typeof(ComplianceTaskStatistics)
			});
		}

		private ComplianceServiceProvider complianceServiceProvider;
	}
}
