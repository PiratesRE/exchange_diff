using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UpdateInboxRules : InboxRulesCommandBase<UpdateInboxRulesRequest, UpdateInboxRulesResponse>
	{
		public UpdateInboxRules(CallContext callContext, UpdateInboxRulesRequest request) : base(callContext, ExTraceGlobals.UpdateInboxRulesCallTracer, (FaultInjection.LIDs)2972069181U, request)
		{
		}

		protected override UpdateInboxRulesResponse Execute(Rules serverRules, MailboxSession mailboxSession)
		{
			ServiceCommandBase.ThrowIfNullOrEmpty<RuleOperation>(base.Request.Operations, "Request.Operations", "UpdateInboxRules.ValidateRequest");
			if (!base.Request.RemoveOutlookRuleBlob && serverRules.LegacyOutlookRulesCacheExists)
			{
				throw new OutlookRuleBlobExistsException();
			}
			RuleOperationParser ruleOperationParser = new RuleOperationParser(base.Request.Operations.Length, base.CallContext, mailboxSession, serverRules, ExTraceGlobals.UpdateInboxRulesCallTracer, base.HashCode);
			Rules rules = ruleOperationParser.Parse(base.Request.Operations);
			UpdateInboxRulesResponse updateInboxRulesResponse = new UpdateInboxRulesResponse();
			if (!ruleOperationParser.HasOperationError)
			{
				rules.SaveBatch();
			}
			else
			{
				updateInboxRulesResponse.Initialize(ServiceResultCode.Error, new ServiceError((CoreResources.IDs)2296308088U, ResponseCodeType.ErrorInboxRulesValidationError, 0, ExchangeVersion.Current));
				updateInboxRulesResponse.RuleOperationErrors = ruleOperationParser.RuleOperationErrorList.ToArray();
			}
			return updateInboxRulesResponse;
		}
	}
}
