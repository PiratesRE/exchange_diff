using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SamlSecurityTokenProvider : SecurityTokenProvider
	{
		public SamlSecurityTokenProvider(SamlClientCredentials samlCredentials)
		{
			if (samlCredentials == null)
			{
				throw new ArgumentNullException("samlCredentials");
			}
			this.identity = samlCredentials.Identity;
			this.targetUri = samlCredentials.TargetUri;
			this.offer = samlCredentials.Offer;
			this.latencyTracker = (samlCredentials.RmsLatencyTracker ?? NoopRmsLatencyTracker.Instance);
			this.securityTokenService = ExternalAuthentication.GetCurrent().GetSecurityTokenService(samlCredentials.OrganizationId);
		}

		protected override SecurityToken GetTokenCore(TimeSpan timeout)
		{
			return SamlSecurityTokenProvider.CreateToken(this.securityTokenService.IssueToken(this.CreateDelegationTokenRequest()));
		}

		protected override IAsyncResult BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if (this.cachedSecurityToken != null)
			{
				LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(null, state, callback);
				ExTraceGlobals.RightsManagementTracer.TraceDebug(0L, "Got cached saml token. Invoking callback.");
				lazyAsyncResult.InvokeCallback();
				return lazyAsyncResult;
			}
			this.latencyTracker.BeginTrackRmsLatency(RmsOperationType.RequestDelegationToken);
			return this.securityTokenService.BeginIssueToken(this.CreateDelegationTokenRequest(), callback, state);
		}

		protected override SecurityToken EndGetTokenCore(IAsyncResult result)
		{
			RmsOperationType rmsOperationType = this.offer.Equals(Offer.IPCCertificationSTS) ? RmsOperationType.AcquireB2BRac : RmsOperationType.AcquireB2BLicense;
			if (this.cachedSecurityToken != null)
			{
				this.latencyTracker.BeginTrackRmsLatency(rmsOperationType);
				return this.cachedSecurityToken;
			}
			RequestedToken rt = this.securityTokenService.EndIssueToken(result);
			this.latencyTracker.EndAndBeginTrackRmsLatency(RmsOperationType.RequestDelegationToken, rmsOperationType);
			this.cachedSecurityToken = SamlSecurityTokenProvider.CreateToken(rt);
			return this.cachedSecurityToken;
		}

		private static SecurityToken CreateToken(RequestedToken rt)
		{
			BinarySecretSecurityToken proofToken = new BinarySecretSecurityToken(rt.ProofToken.GetSymmetricKey());
			return new GenericXmlSecurityToken(rt.SecurityToken, proofToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(2.0), new SamlAssertionKeyIdentifierClause(rt.SecurityTokenReference.InnerText), new SamlAssertionKeyIdentifierClause(rt.RequestUnattachedReference.InnerText), new ReadOnlyCollection<IAuthorizationPolicy>(new List<IAuthorizationPolicy>()));
		}

		private DelegationTokenRequest CreateDelegationTokenRequest()
		{
			return new DelegationTokenRequest
			{
				FederatedIdentity = new FederatedIdentity(this.identity.Email, IdentityType.UPN),
				EmailAddress = this.identity.Email,
				Target = new TokenTarget(this.targetUri),
				Offer = this.offer,
				EmailAddresses = ((this.identity.ProxyAddresses != null) ? new List<string>(this.identity.ProxyAddresses) : null)
			};
		}

		private LicenseIdentity identity;

		private Uri targetUri;

		private Offer offer;

		private IRmsLatencyTracker latencyTracker;

		private SecurityToken cachedSecurityToken;

		private SecurityTokenService securityTokenService;
	}
}
