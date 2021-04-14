using System;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal interface IRemoteArchiveRequest
	{
		ExchangeServiceBinding ArchiveService { get; set; }

		bool IsRemoteArchiveRequest(CallContext callContext);

		ServiceCommandBase GetRemoteArchiveServiceCommand(CallContext callContext);
	}
}
