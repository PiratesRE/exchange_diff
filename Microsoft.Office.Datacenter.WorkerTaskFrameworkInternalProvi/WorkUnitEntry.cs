using System;
using System.Data.Linq.Mapping;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	[Table]
	public class WorkUnitEntry : TableEntity
	{
		public WorkUnitEntry()
		{
		}

		public WorkUnitEntry(string deploymentXml, int aggregationLevel, string scope, double cost, bool exclusiveMachineAccess, bool isFrameworkEntry)
		{
			this.WorkUnitId = -1;
			this.DeploymentXml = deploymentXml;
			this.AggregationLevel = aggregationLevel;
			this.Scope = scope;
			this.Cost = cost;
			this.ExclusiveMachineAccess = exclusiveMachineAccess;
			this.IsFrameworkEntry = isFrameworkEntry;
			this.IsNew = true;
		}

		public static string FrameworkEntryScope
		{
			get
			{
				return "Framework";
			}
		}

		[Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column]
		public int WorkUnitId { get; set; }

		[Column]
		public string DeploymentXml { get; set; }

		[Column]
		public int AggregationLevel { get; set; }

		[Column]
		public string Scope { get; set; }

		[Column]
		public bool Remove { get; set; }

		[Column]
		public bool RunNow { get; set; }

		[Column]
		public int WorkUnitState { get; set; }

		[Column]
		public bool IsFrameworkEntry { get; set; }

		[Column]
		public bool ExclusiveMachineAccess { get; private set; }

		public double Cost { get; private set; }

		public bool IsNew { get; set; }

		public static int Compare(WorkUnitEntry entry1, WorkUnitEntry entry2)
		{
			int num = string.Compare(entry1.Scope, entry2.Scope, StringComparison.OrdinalIgnoreCase);
			if (num == 0)
			{
				num = string.Compare(entry1.DeploymentXml, entry2.DeploymentXml, StringComparison.OrdinalIgnoreCase);
			}
			return num;
		}
	}
}
