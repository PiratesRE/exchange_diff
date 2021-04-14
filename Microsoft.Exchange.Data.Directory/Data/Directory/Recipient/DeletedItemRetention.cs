using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public enum DeletedItemRetention
	{
		DatabaseDefault,
		RetainForCustomPeriod = 5,
		RetainUntilBackupOrCustomPeriod = 3
	}
}
