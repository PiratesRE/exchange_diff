using System;
using System.Text;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class FedOrgCredentials
	{
		public FedOrgCredentials(DelegationTokenRequest request, SecurityTokenService tokenService)
		{
			if (request.Target == null || request.Offer == null || request.EmailAddress == null || request.FederatedIdentity == null)
			{
				throw new ArgumentException();
			}
			this.request = request;
			this.tokenService = tokenService;
			StringBuilder stringBuilder = new StringBuilder(this.request.Target.ToString().ToLowerInvariant());
			stringBuilder.Append(this.request.FederatedIdentity.Identity.ToLowerInvariant());
			stringBuilder.Append(this.request.EmailAddress.ToLowerInvariant());
			stringBuilder.Append(this.request.Offer.Name);
			if (this.request.Policy != null)
			{
				stringBuilder.Append(this.request.Policy.ToLowerInvariant());
			}
			this.cacheKey = stringBuilder.ToString();
		}

		public RequestedToken GetToken()
		{
			FedOrgCredentials.CacheableRequestedToken cacheableRequestedToken = null;
			bool flag = false;
			if (!FedOrgCredentials.tokenCache.TryGetValue(this.cacheKey, out cacheableRequestedToken, out flag) || flag)
			{
				cacheableRequestedToken = this.GetTokenForTarget();
			}
			return cacheableRequestedToken.RequestedToken;
		}

		private FedOrgCredentials.CacheableRequestedToken GetTokenForTarget()
		{
			FedOrgCredentials.CacheableRequestedToken cacheableRequestedToken = new FedOrgCredentials.CacheableRequestedToken(this.tokenService.IssueToken(this.request), this.request.Offer);
			FedOrgCredentials.tokenCache.TryAdd(this.cacheKey, cacheableRequestedToken);
			return cacheableRequestedToken;
		}

		private static Cache<string, FedOrgCredentials.CacheableRequestedToken> tokenCache = new Cache<string, FedOrgCredentials.CacheableRequestedToken>(32L, TimeSpan.FromHours(8.0), TimeSpan.FromHours(0.0), TimeSpan.FromMinutes(1.0), null, null);

		private DelegationTokenRequest request;

		private SecurityTokenService tokenService;

		private string cacheKey;

		private sealed class CacheableRequestedToken : CachableItem
		{
			public CacheableRequestedToken(RequestedToken token, Offer offer)
			{
				this.token = token;
				this.offer = offer;
			}

			public override long ItemSize
			{
				get
				{
					return 1L;
				}
			}

			public override bool IsExpired(DateTime currentTime)
			{
				long num = currentTime.ToUniversalTime().Ticks - this.token.Lifetime.Created.ToUniversalTime().Ticks;
				long ticks = this.offer.Duration.Ticks;
				return (double)num / (double)ticks > 0.9;
			}

			public RequestedToken RequestedToken
			{
				get
				{
					return this.token;
				}
			}

			private RequestedToken token;

			private Offer offer;
		}
	}
}
