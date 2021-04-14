using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GetPeopleConnectState : SingleStepServiceCommand<GetPeopleConnectStateRequest, PeopleConnectionState>
	{
		public GetPeopleConnectState(CallContext callContext, GetPeopleConnectStateRequest request) : base(callContext, request)
		{
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.provider = request.Provider;
		}

		internal override ServiceResult<PeopleConnectionState> Execute()
		{
			GetPeopleConnectState.Tracer.TraceDebug<string>(0L, "GetPeopleConnectState called for provider '{0}'", this.provider);
			using (IEnumerator<ConnectSubscription> enumerator = new ConnectSubscriptionsEnumerator(this.session, this.provider).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ConnectSubscription connectSubscription = enumerator.Current;
					return new ServiceResult<PeopleConnectionState>(GetPeopleConnectState.ConvertToPeopleConnectionState(connectSubscription.ConnectState));
				}
			}
			throw new PeopleConnectionNotFoundException();
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetPeopleConnectStateResponse getPeopleConnectStateResponse = new GetPeopleConnectStateResponse();
			getPeopleConnectStateResponse.AddResponse(new GetPeopleConnectStateResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value));
			return getPeopleConnectStateResponse;
		}

		private static PeopleConnectionState ConvertToPeopleConnectionState(ConnectState state)
		{
			switch (state)
			{
			case ConnectState.Connected:
				return PeopleConnectionState.Connected;
			case ConnectState.ConnectedNeedsToken:
				return PeopleConnectionState.ConnectedNeedsToken;
			}
			return PeopleConnectionState.Disconnected;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ServiceCommandBaseCallTracer;

		private readonly MailboxSession session;

		private readonly string provider;
	}
}
