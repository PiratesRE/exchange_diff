using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync
{
	internal interface IAutdStatusData
	{
		int? LastPingHeartbeat { get; set; }

		Dictionary<string, PingCommand.DPFolderInfo> DPFolderList { get; set; }

		void SaveAndDispose();
	}
}
