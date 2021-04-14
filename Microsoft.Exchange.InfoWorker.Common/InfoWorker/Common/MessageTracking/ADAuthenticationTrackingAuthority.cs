using System;
using System.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal abstract class ADAuthenticationTrackingAuthority : WebServiceTrackingAuthority
	{
		protected override void SetAuthenticationMechanism(ExchangeServiceBinding ewsBinding)
		{
			if (ADAuthenticationTrackingAuthority.CanImpersonateNetworkService)
			{
				ewsBinding.Authenticator = SoapHttpClientAuthenticator.CreateNetworkService();
				return;
			}
			ewsBinding.Credentials = CredentialCache.DefaultNetworkCredentials;
		}

		private static bool CanImpersonateNetworkService
		{
			get
			{
				if (ADAuthenticationTrackingAuthority.canImpersonateNetworkService == null)
				{
					lock (ADAuthenticationTrackingAuthority.initLock)
					{
						if (ADAuthenticationTrackingAuthority.canImpersonateNetworkService == null)
						{
							NetworkServiceImpersonator.Initialize();
							ADAuthenticationTrackingAuthority.canImpersonateNetworkService = new bool?(NetworkServiceImpersonator.Exception == null);
						}
					}
				}
				return ADAuthenticationTrackingAuthority.canImpersonateNetworkService.Value;
			}
		}

		protected ADAuthenticationTrackingAuthority(TrackingAuthorityKind responsibleTracker, Uri uri) : base(responsibleTracker, uri)
		{
		}

		private static object initLock = new object();

		private static bool? canImpersonateNetworkService = null;
	}
}
