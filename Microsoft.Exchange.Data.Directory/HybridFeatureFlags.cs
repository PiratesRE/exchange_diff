using System;

[Flags]
internal enum HybridFeatureFlags
{
	None = 0,
	FreeBusy = 1,
	MoveMailbox = 2,
	Mailtips = 4,
	MessageTracking = 8,
	OwaRedirection = 16,
	OnlineArchive = 32,
	SecureMail = 64,
	CentralizedTransportOnPrem = 128,
	CentralizedTransportInCloud = 256,
	Photos = 512,
	All = 1023
}
