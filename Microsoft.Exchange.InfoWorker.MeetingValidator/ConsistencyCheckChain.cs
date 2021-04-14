using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConsistencyCheckChain<ResultType> where ResultType : ConsistencyCheckResult
	{
		private ConsistencyCheckChain()
		{
			this.checkList = null;
		}

		protected bool PerformCheck<CheckResultType>(ConsistencyCheckBase<CheckResultType> check) where CheckResultType : ResultType
		{
			bool result;
			if (check.PreProcess(this.totalResult))
			{
				CheckResultType checkResultType = check.Run();
				if (checkResultType.ShouldBeReported)
				{
					this.totalResult.AddCheckResult(checkResultType);
				}
				result = this.ShouldContinue((ResultType)((object)checkResultType));
			}
			else
			{
				result = true;
			}
			return result;
		}

		internal ConsistencyCheckChain(MeetingComparisonResult totalResult) : this(1, totalResult)
		{
		}

		internal ConsistencyCheckChain(int capacity, MeetingComparisonResult totalResult)
		{
			this.checkList = new List<ConsistencyCheckBase<ResultType>>(capacity);
			this.totalResult = totalResult;
		}

		internal void AddCheck(ConsistencyCheckBase<ResultType> check)
		{
			this.checkList.Add(check);
		}

		internal void PerformChecks()
		{
			foreach (ConsistencyCheckBase<ResultType> check in this.checkList)
			{
				if (!this.PerformCheck<ResultType>(check))
				{
					break;
				}
			}
		}

		protected virtual bool ShouldContinue(ResultType lastCheckResult)
		{
			return true;
		}

		private List<ConsistencyCheckBase<ResultType>> checkList;

		private MeetingComparisonResult totalResult;
	}
}
