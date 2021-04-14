using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class MeetingExistenceConsistencyCheckResult : PrimaryConsistencyCheckResult
	{
		private MeetingExistenceConsistencyCheckResult(ConsistencyCheckType checkType, string checkDescription) : base(checkType, checkDescription, true)
		{
			this.ItemIsFound = false;
		}

		internal override bool ShouldTerminate
		{
			get
			{
				return base.ShouldTerminate || !this.ItemIsFound;
			}
		}

		internal new static MeetingExistenceConsistencyCheckResult CreateInstance(ConsistencyCheckType checkType, string checkDescription)
		{
			return new MeetingExistenceConsistencyCheckResult(checkType, checkDescription);
		}

		internal bool ItemIsFound { get; set; }
	}
}
