using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum InstantMessagePresenceType
	{
		None,
		Online = 3000,
		IdleOnline = 4500,
		Busy = 6000,
		IdleBusy = 7500,
		DoNotDisturb = 9000,
		BeRightBack = 12000,
		Away = 15000,
		Offline = 18000
	}
}
