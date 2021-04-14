using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum Importance
	{
		[LocDescription(ServerStrings.IDs.InboxRuleImportanceLow)]
		Low,
		[LocDescription(ServerStrings.IDs.InboxRuleImportanceNormal)]
		Normal,
		[LocDescription(ServerStrings.IDs.InboxRuleImportanceHigh)]
		High
	}
}
