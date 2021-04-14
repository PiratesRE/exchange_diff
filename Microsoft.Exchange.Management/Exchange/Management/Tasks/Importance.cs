using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public enum Importance
	{
		[LocDescription(RulesTasksStrings.IDs.ImportanceLow)]
		Low,
		[LocDescription(RulesTasksStrings.IDs.ImportanceNormal)]
		Normal,
		[LocDescription(RulesTasksStrings.IDs.ImportanceHigh)]
		High
	}
}
