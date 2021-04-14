using System;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class EndInstantSearchSession : SingleStepServiceCommand<EndInstantSearchSessionRequest, EndInstantSearchSessionResponse>
	{
		public EndInstantSearchSession(CallContext callContext, EndInstantSearchSessionRequest request) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(request.DeviceId, "deviceId", "ServiceCommand::EndInstantSearchSession");
			ServiceCommandBase.ThrowIfNull(request.SessionId, "sessionId", "ServiceCommand::EndInstantSearchSession");
			this.instantSearchManager = PerformInstantSearch.GetManagerForCaller(callContext, request.DeviceId);
		}

		public EndInstantSearchSession(CallContext callContext, EndInstantSearchSessionRequest request, InstantSearchManager manager) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(this.instantSearchManager, "instantSearchManager", "ServiceCommand::EndInstantSearchSession");
			ServiceCommandBase.ThrowIfNull(request.SessionId, "sessionId", "ServiceCommand::EndInstantSearchSession");
			this.instantSearchManager = manager;
		}

		internal override ServiceResult<EndInstantSearchSessionResponse> Execute()
		{
			EndInstantSearchSessionResponse value = this.instantSearchManager.EndSearchSession(base.Request.SessionId);
			if (base.Request.DeviceId != null)
			{
				PerformInstantSearch.RemoveManagerForCaller(base.CallContext, base.Request.DeviceId);
			}
			return new ServiceResult<EndInstantSearchSessionResponse>(value);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		private readonly InstantSearchManager instantSearchManager;
	}
}
