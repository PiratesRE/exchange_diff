using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum Sensitivity
	{
		[LocDescription(ServerStrings.IDs.InboxRuleSensitivityNormal)]
		Normal,
		[LocDescription(ServerStrings.IDs.InboxRuleSensitivityPersonal)]
		Personal,
		[LocDescription(ServerStrings.IDs.InboxRuleSensitivityPrivate)]
		Private,
		[LocDescription(ServerStrings.IDs.InboxRuleSensitivityCompanyConfidential)]
		CompanyConfidential
	}
}
