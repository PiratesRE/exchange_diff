using System;

namespace Microsoft.Exchange.Nspi.Rfri
{
	public enum RfriStatus
	{
		Success,
		GeneralFailure = -2147467259,
		InvalidObject = -2147221240,
		LogonFailed = -2147221231,
		NoSuchObject = -2147219168,
		AccessDenied = -2147024891,
		InvalidParameter = -2147024809
	}
}
