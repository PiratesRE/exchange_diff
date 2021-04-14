using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	public enum OmsErrorCode
	{
		Unrecognized,
		Ok,
		ExpiredUser,
		InvalidUser,
		UnregisteredUser,
		UnregisteredService,
		CeasedService,
		PerDayMsgLimit,
		PerMonthMsgLimit,
		LowBalance,
		InvalidRecipient,
		CrossCarrier,
		InvalidChar,
		InvalidMedia,
		LengthLimit,
		SizeLimit,
		SlidesLimit,
		ServiceUpdate,
		ServiceNetwork,
		NoScheduled,
		Other
	}
}
