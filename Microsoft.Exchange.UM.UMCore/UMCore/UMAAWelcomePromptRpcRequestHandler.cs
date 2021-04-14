using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMAAWelcomePromptRpcRequestHandler : UMPromptRpcRequestHandler
	{
		protected override ArrayList GetPrompts(RequestBase requestBase)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Processing a PromptPreview request for UMAAWelcomePrompt.", new object[0]);
			ArrayList arrayList = new ArrayList();
			UMAAWelcomePromptRpcRequest umaawelcomePromptRpcRequest = requestBase as UMAAWelcomePromptRpcRequest;
			if (umaawelcomePromptRpcRequest != null)
			{
				AutoAttendantContext autoAttendantContext = new AutoAttendantContext(umaawelcomePromptRpcRequest.AutoAttendant, umaawelcomePromptRpcRequest.BusinessHoursFlag);
				arrayList.AddRange(VariablePrompt<AutoAttendantContext>.GetPreview<AAWelcomeGreetingPrompt>(new AAWelcomeGreetingPrompt(), autoAttendantContext.AutoAttendant.Language.Culture, autoAttendantContext));
				bool flag = umaawelcomePromptRpcRequest.BusinessHoursFlag ? umaawelcomePromptRpcRequest.AutoAttendant.BusinessHoursKeyMappingEnabled : umaawelcomePromptRpcRequest.AutoAttendant.AfterHoursKeyMappingEnabled;
				if (umaawelcomePromptRpcRequest.MenuFlag && flag)
				{
					arrayList.AddRange(VariablePrompt<AutoAttendantContext>.GetPreview<AACustomMenuPrompt>(new AACustomMenuPrompt(), autoAttendantContext.AutoAttendant.Language.Culture, autoAttendantContext));
				}
			}
			return arrayList;
		}
	}
}
