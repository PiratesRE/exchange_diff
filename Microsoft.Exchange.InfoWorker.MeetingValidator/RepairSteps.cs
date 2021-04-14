using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RepairSteps
	{
		public bool InquiredMeeting { get; private set; }

		private RumInfo MasterRum { get; set; }

		private Dictionary<ExDateTime, RumInfo> OccurrenceRums { get; set; }

		public List<RumInfo> SendableRums
		{
			get
			{
				if (this.MasterRum.IsNullOp)
				{
					return this.OccurrenceRums.Values.ToList<RumInfo>();
				}
				return new List<RumInfo>
				{
					this.MasterRum
				};
			}
		}

		private RepairSteps()
		{
		}

		private void Initialize(RumInfo master, Dictionary<ExDateTime, RumInfo> occurrences)
		{
			this.MasterRum = master;
			this.OccurrenceRums = occurrences;
		}

		internal static RepairSteps CreateInstance()
		{
			RepairSteps repairSteps = new RepairSteps();
			repairSteps.Initialize(NullOpRumInfo.CreateInstance(), new Dictionary<ExDateTime, RumInfo>());
			return repairSteps;
		}

		internal void AddStep(RumInfo rum)
		{
			if (rum == null)
			{
				throw new ArgumentNullException("rum", "RUM cannot be null.");
			}
			if (!rum.IsNullOp)
			{
				if (rum.IsOccurrenceRum)
				{
					this.AddOccurrenceRum(rum);
					return;
				}
				this.MasterRum = RumInfo.Merge(this.MasterRum, rum);
				this.OccurrenceRums.Clear();
			}
		}

		internal void Merge(RepairSteps stepsToMerge)
		{
			if (stepsToMerge == null)
			{
				throw new ArgumentNullException("stepsToMerge");
			}
			this.MasterRum = RumInfo.Merge(this.MasterRum, stepsToMerge.MasterRum);
			foreach (RumInfo rum in stepsToMerge.OccurrenceRums.Values)
			{
				this.AddOccurrenceRum(rum);
			}
		}

		private void AddOccurrenceRum(RumInfo rum)
		{
			if (this.MasterRum.IsNullOp)
			{
				ExDateTime value = rum.OccurrenceOriginalStartTime.Value;
				if (this.OccurrenceRums.ContainsKey(value))
				{
					this.OccurrenceRums[value] = RumInfo.Merge(this.OccurrenceRums[value], rum);
					return;
				}
				this.OccurrenceRums.Add(rum.OccurrenceOriginalStartTime.Value, rum);
			}
		}

		internal bool SendRums(CalendarItemBase baseItem, ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent)
		{
			bool flag = false;
			if (this.MasterRum.IsNullOp)
			{
				using (Dictionary<ExDateTime, RumInfo>.ValueCollection.Enumerator enumerator = this.OccurrenceRums.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RumInfo info = enumerator.Current;
						if (RumAgent.Instance.SendRums(info, RumFactory.Instance.Policy.CopyRumsToSentItems, baseItem, ref organizerRumsSent))
						{
							flag = true;
						}
					}
					return flag;
				}
			}
			flag = RumAgent.Instance.SendRums(this.MasterRum, RumFactory.Instance.Policy.CopyRumsToSentItems, baseItem, ref organizerRumsSent);
			if (flag && this.MasterRum.Type == RumType.Inquiry)
			{
				this.InquiredMeeting = true;
			}
			return flag;
		}

		public void ForEachSendableRum(Action<RumInfo> action)
		{
			if (this.MasterRum.IsNullOp)
			{
				using (Dictionary<ExDateTime, RumInfo>.ValueCollection.Enumerator enumerator = this.OccurrenceRums.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RumInfo obj = enumerator.Current;
						action(obj);
					}
					return;
				}
			}
			action(this.MasterRum);
		}

		public int SendableRumsCount
		{
			get
			{
				if (!this.MasterRum.IsNullOp)
				{
					return 1;
				}
				return this.OccurrenceRums.Count;
			}
		}
	}
}
