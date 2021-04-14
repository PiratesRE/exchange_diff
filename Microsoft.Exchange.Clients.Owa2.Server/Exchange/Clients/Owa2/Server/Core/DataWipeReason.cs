using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum DataWipeReason
	{
		None,
		AccountDeletedInMowa,
		PasswordAttemptsExceeded,
		MowaDisabled,
		RemoteWipe,
		MaxDevicePartnerships,
		AccountDeletedInAccountManager,
		PartnerAppNotifiesOfWipe
	}
}
