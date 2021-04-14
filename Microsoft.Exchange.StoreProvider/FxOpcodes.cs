using System;

namespace Microsoft.Mapi
{
	internal enum FxOpcodes
	{
		None,
		Config,
		TransferBuffer,
		IsInterfaceOk,
		TellPartnerVersion,
		StartMdbEventsImport = 11,
		FinishMdbEventsImport,
		AddMdbEvents,
		SetWatermarks,
		SetReceiveFolder,
		SetPerUser,
		SetProps
	}
}
