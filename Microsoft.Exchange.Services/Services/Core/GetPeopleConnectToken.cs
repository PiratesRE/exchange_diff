using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GetPeopleConnectToken : SingleStepServiceCommand<GetPeopleConnectTokenRequest, PeopleConnectionToken>
	{
		public GetPeopleConnectToken(CallContext callContext, GetPeopleConnectTokenRequest request) : base(callContext, request)
		{
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.provider = request.Provider;
		}

		internal override ServiceResult<PeopleConnectionToken> Execute()
		{
			GetPeopleConnectToken.Tracer.TraceDebug<string>(0L, "GetPeopleConnectToken called for provider '{0}'", this.provider);
			using (IEnumerator<ConnectSubscription> enumerator = new ConnectSubscriptionsEnumerator(this.session, this.provider).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ConnectSubscription connectSubscription = enumerator.Current;
					if (!this.HasAccessToken(connectSubscription))
					{
						throw new PeopleConnectionNoTokenException();
					}
					return new ServiceResult<PeopleConnectionToken>(new PeopleConnectionToken
					{
						AccessToken = connectSubscription.AccessTokenInClearText,
						ApplicationId = connectSubscription.AppId,
						ApplicationSecret = this.ReadAppSecret()
					});
				}
			}
			throw new PeopleConnectionNotFoundException();
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetPeopleConnectTokenResponse getPeopleConnectTokenResponse = new GetPeopleConnectTokenResponse();
			getPeopleConnectTokenResponse.AddResponse(new GetPeopleConnectTokenResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value));
			return getPeopleConnectTokenResponse;
		}

		private bool HasAccessToken(ConnectSubscription subscription)
		{
			if (!subscription.HasAccessToken)
			{
				return false;
			}
			switch (subscription.ConnectState)
			{
			case ConnectState.Connected:
				return true;
			case ConnectState.ConnectedNeedsToken:
				return false;
			}
			return false;
		}

		private string ReadAppSecret()
		{
			string appSecretClearText;
			try
			{
				appSecretClearText = CachedPeopleConnectApplicationConfig.Instance.ReadProvider(this.provider).AppSecretClearText;
			}
			catch (ExchangeConfigurationException innerException)
			{
				throw new PeopleConnectApplicationConfigException(innerException);
			}
			return appSecretClearText;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ServiceCommandBaseCallTracer;

		private readonly MailboxSession session;

		private readonly string provider;
	}
}
