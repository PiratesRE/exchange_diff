using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	public enum OnDemandQueryType : byte
	{
		MTSummary = 1,
		MTDetail,
		DLP,
		Rule,
		AntiSpam,
		AntiVirus
	}
}
