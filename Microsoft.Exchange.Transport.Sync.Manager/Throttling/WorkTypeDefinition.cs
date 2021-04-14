using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WorkTypeDefinition : IComparable<WorkTypeDefinition>
	{
		internal WorkTypeDefinition(WorkType workType, TimeSpan timeTillSyncDue, byte weight, bool isSyncNow)
		{
			if (weight <= 0)
			{
				throw new ArgumentOutOfRangeException("weight");
			}
			this.workType = workType;
			this.timeTillSyncDue = timeTillSyncDue;
			this.weight = weight;
			this.isSyncNow = isSyncNow;
			this.isLightLoad = WorkTypeManager.IsLightWeightWorkType(workType);
			this.isOneOff = WorkTypeManager.IsOneOffWorkType(workType);
		}

		internal WorkType WorkType
		{
			get
			{
				return this.workType;
			}
		}

		internal TimeSpan TimeTillSyncDue
		{
			get
			{
				return this.timeTillSyncDue;
			}
		}

		internal byte Weight
		{
			get
			{
				return this.weight;
			}
		}

		internal bool IsSyncNow
		{
			get
			{
				return this.isSyncNow;
			}
		}

		internal bool IsLightLoad
		{
			get
			{
				return this.isLightLoad;
			}
		}

		internal bool IsOneOff
		{
			get
			{
				return this.isOneOff;
			}
		}

		public int CompareTo(WorkTypeDefinition workTypeDefinition)
		{
			if (this.workType == workTypeDefinition.WorkType)
			{
				return 0;
			}
			if (this.Weight <= workTypeDefinition.Weight)
			{
				return 1;
			}
			return -1;
		}

		internal void AddDiagnosticInfoTo(XElement parentElement)
		{
			parentElement.Add(new XElement("workType", this.workType.ToString()));
			parentElement.Add(new XElement("timeTillSyncDue", this.timeTillSyncDue.ToString()));
			parentElement.Add(new XElement("weight", this.weight.ToString()));
		}

		private readonly WorkType workType;

		private readonly TimeSpan timeTillSyncDue;

		private readonly byte weight;

		private readonly bool isSyncNow;

		private readonly bool isLightLoad;

		private readonly bool isOneOff;
	}
}
