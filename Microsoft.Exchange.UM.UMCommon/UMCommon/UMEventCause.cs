using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public enum UMEventCause
	{
		None,
		UserBusy,
		NoAnswer,
		Unavailable,
		Other
	}
}
