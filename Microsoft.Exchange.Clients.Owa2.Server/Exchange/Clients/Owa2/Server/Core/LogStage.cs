using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum LogStage
	{
		ReceiveRequest,
		LoadItem,
		RefreshClassifications,
		LoadRules,
		EvaluateRules,
		LoadCustomStrings,
		SendResponse
	}
}
