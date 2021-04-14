using System;

namespace Microsoft.Exchange.Data
{
	internal enum ServerEditionValue
	{
		None = -1,
		Standard,
		Enterprise,
		Evaluation,
		Sam,
		BackOffice,
		Select,
		UpgradedStandard = 8,
		UpgradedEnterprise,
		Coexistence,
		UpgradeCoexistence
	}
}
