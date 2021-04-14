using System;

namespace System.Security.Principal
{
	[Serializable]
	internal enum KerbLogonSubmitType
	{
		KerbInteractiveLogon = 2,
		KerbSmartCardLogon = 6,
		KerbWorkstationUnlockLogon,
		KerbSmartCardUnlockLogon,
		KerbProxyLogon,
		KerbTicketLogon,
		KerbTicketUnlockLogon,
		KerbS4ULogon
	}
}
