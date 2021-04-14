using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingComparisonResult : IEnumerable<ConsistencyCheckResult>, IEnumerable
	{
		private MeetingComparisonResult()
		{
			this.checkStatusTable = new Dictionary<ConsistencyCheckType, CheckStatusType>(Enum.GetValues(typeof(ConsistencyCheckType)).Length);
		}

		public string AttendeePrimarySmtpAddress { get; private set; }

		public MeetingData Meeting { get; private set; }

		public RepairSteps RepairInfo { get; private set; }

		public bool InquiredMeeting
		{
			get
			{
				return this.RepairInfo.InquiredMeeting;
			}
		}

		public int CheckResultCount
		{
			get
			{
				return this.CheckResultList.Count;
			}
		}

		public List<ConsistencyCheckResult> CheckResultList { get; private set; }

		internal CheckStatusType? this[ConsistencyCheckType check]
		{
			get
			{
				if (!this.checkStatusTable.ContainsKey(check))
				{
					return null;
				}
				return new CheckStatusType?(this.checkStatusTable[check]);
			}
		}

		public void ForEachCheckResult(Action<ConsistencyCheckResult> action)
		{
			this.CheckResultList.ForEach(action);
		}

		internal static MeetingComparisonResult CreateInstance(UserObject attendee, MeetingData meeting)
		{
			return new MeetingComparisonResult
			{
				CheckResultList = new List<ConsistencyCheckResult>(),
				RepairInfo = RepairSteps.CreateInstance(),
				AttendeePrimarySmtpAddress = ((attendee.ExchangePrincipal == null) ? attendee.EmailAddress : attendee.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()),
				Meeting = meeting
			};
		}

		internal void AddCheckResult(ConsistencyCheckResult result)
		{
			this.checkStatusTable.Add(result.CheckType, result.Status);
			this.CheckResultList.Add(result);
			this.RepairInfo.Merge(result.RepairInfo);
		}

		public IEnumerator<ConsistencyCheckResult> GetEnumerator()
		{
			return this.CheckResultList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private Dictionary<ConsistencyCheckType, CheckStatusType> checkStatusTable;
	}
}
