using System;
using System.Collections;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMAABusinessLocationPromptRpcRequestHandler : UMPromptRpcRequestHandler
	{
		protected override ArrayList GetPrompts(RequestBase requestBase)
		{
			UMAABusinessLocationPromptRpcRequest umaabusinessLocationPromptRpcRequest = requestBase as UMAABusinessLocationPromptRpcRequest;
			return VariablePrompt<AutoAttendantLocationContext>.GetPreview<AABusinessLocationPrompt>(new AABusinessLocationPrompt(), umaabusinessLocationPromptRpcRequest.AutoAttendant.Language.Culture, new AutoAttendantLocationContext(umaabusinessLocationPromptRpcRequest.AutoAttendant, Strings.BusinessLocationDefaultMenuName));
		}
	}
}
