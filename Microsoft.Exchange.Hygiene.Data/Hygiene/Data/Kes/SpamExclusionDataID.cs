using System;

namespace Microsoft.Exchange.Hygiene.Data.Kes
{
	public enum SpamExclusionDataID : byte
	{
		URL = 1,
		IP,
		BannedSender
	}
}
