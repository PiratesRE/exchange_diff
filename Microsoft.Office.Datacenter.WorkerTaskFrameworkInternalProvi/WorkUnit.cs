using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	[Table]
	public class WorkUnit : TableEntity
	{
		public WorkUnit()
		{
		}

		public WorkUnit(int id)
		{
			this.WorkUnitId = id;
			this.IsExclusive = false;
			this.Entries = new List<WorkUnitEntry>();
			this.EntryDeploymentXmlFiles = new HashSet<string>();
			this.Cost = 0.0;
		}

		[Column]
		public int WorkUnitId { get; set; }

		[Column]
		public bool RunNow { get; set; }

		public List<WorkUnitEntry> Entries { get; set; }

		public HashSet<string> EntryDeploymentXmlFiles { get; set; }

		public bool IsExclusive
		{
			get
			{
				return this.isExclusive;
			}
			internal set
			{
				this.isExclusive = value;
			}
		}

		public double Cost { get; set; }

		internal static int UnassignedWorkUnitId
		{
			get
			{
				return WorkUnit.unassignedWorkUnitIdInternal;
			}
		}

		internal static int ToRemoveWorkUnitId
		{
			get
			{
				return WorkUnit.toRemoveWorkUnitIdInternal;
			}
		}

		internal bool IsUnassignedWorkUnit
		{
			get
			{
				return this.WorkUnitId == WorkUnit.unassignedWorkUnitIdInternal;
			}
		}

		internal bool IsToRemoveWorkUnit
		{
			get
			{
				return this.WorkUnitId == WorkUnit.toRemoveWorkUnitIdInternal;
			}
		}

		internal bool IsNew
		{
			get
			{
				return this.WorkUnitId < 0 && !this.IsUnassignedWorkUnit && !this.IsToRemoveWorkUnit;
			}
		}

		public bool AddWorkUnitEntry(WorkUnitEntry entry)
		{
			bool result = false;
			bool flag = this.Cost + entry.Cost <= (double)Settings.MaxWorkUnitCost;
			bool flag2 = flag;
			if (entry.ExclusiveMachineAccess)
			{
				bool flag3 = this.EntryDeploymentXmlFiles.Count == 0 || this.EntryDeploymentXmlFiles.Contains(entry.DeploymentXml);
				flag2 = (flag2 && flag3);
			}
			if (this.IsExclusive)
			{
				bool flag4 = this.EntryDeploymentXmlFiles.Contains(entry.DeploymentXml);
				flag2 = (flag2 && flag4);
			}
			flag2 = ((flag2 && (this.aggregationLevel == -1 || this.aggregationLevel == entry.AggregationLevel)) || this.IsUnassignedWorkUnit || this.IsToRemoveWorkUnit);
			if (flag2)
			{
				if (!this.IsToRemoveWorkUnit)
				{
					entry.WorkUnitId = this.WorkUnitId;
				}
				this.Entries.Add(entry);
				this.EntryDeploymentXmlFiles.Add(entry.DeploymentXml);
				this.Cost += entry.Cost;
				this.isExclusive = entry.ExclusiveMachineAccess;
				this.aggregationLevel = entry.AggregationLevel;
				result = true;
			}
			return result;
		}

		public void RemoveWorkUnitEntry(WorkUnitEntry entry)
		{
			this.Entries.Remove(entry);
			this.EntryDeploymentXmlFiles.Remove(entry.DeploymentXml);
			this.Cost -= entry.Cost;
		}

		private const int AggregationLevelNotSet = -1;

		private static int unassignedWorkUnitIdInternal = -1;

		private static int toRemoveWorkUnitIdInternal = -2;

		private bool isExclusive;

		private int aggregationLevel = -1;
	}
}
