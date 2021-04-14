using System;
using System.Collections;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.ClientAccess.Messages;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMAABusinessHoursPromptRpcRequestHandler : UMPromptRpcRequestHandler
	{
		protected override ArrayList GetPrompts(RequestBase requestBase)
		{
			UMAABusinessHoursPromptRpcRequest umaabusinessHoursPromptRpcRequest = requestBase as UMAABusinessHoursPromptRpcRequest;
			BusinessHoursPrompt prompt = new BusinessHoursPrompt();
			return VariablePrompt<UMAutoAttendant>.GetPreview<BusinessHoursPrompt>(prompt, umaabusinessHoursPromptRpcRequest.AutoAttendant.Language.Culture, umaabusinessHoursPromptRpcRequest.AutoAttendant);
		}
	}
}
