using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ConsistencyCheckBase<ResultType> where ResultType : ConsistencyCheckResult
	{
		protected virtual void Initialize(ConsistencyCheckType type, string description, SeverityType severity, CalendarValidationContext context, IEnumerable<ConsistencyCheckType> dependsOnCheckPassList)
		{
			this.Type = type;
			this.Description = description;
			this.Severity = severity;
			this.Context = context;
			this.dependsOnCheckPassList = dependsOnCheckPassList;
		}

		internal ConsistencyCheckType Type { get; private set; }

		internal string Description { get; private set; }

		internal SeverityType Severity { get; set; }

		internal CalendarValidationContext Context { get; private set; }

		internal ResultType Run()
		{
			ResultType result = this.DetectInconsistencies();
			this.ProcessResult(result);
			result.Severity = this.Severity;
			return result;
		}

		internal virtual bool PreProcess(MeetingComparisonResult totalResults)
		{
			if (this.dependsOnCheckPassList != null)
			{
				foreach (ConsistencyCheckType check in this.dependsOnCheckPassList)
				{
					CheckStatusType? checkStatusType = totalResults[check];
					if (checkStatusType == null || checkStatusType.Value != CheckStatusType.Passed)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		protected abstract ResultType DetectInconsistencies();

		protected abstract void ProcessResult(ResultType result);

		private IEnumerable<ConsistencyCheckType> dependsOnCheckPassList;
	}
}
