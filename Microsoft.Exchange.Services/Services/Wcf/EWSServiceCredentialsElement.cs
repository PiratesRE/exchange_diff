using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.PartnerToken;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	public sealed class EWSServiceCredentialsElement : ServiceCredentialsElement
	{
		protected override object CreateBehavior()
		{
			object obj;
			ServiceCredentials serviceCredentials;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Ews.EwsServiceCredentials.Enabled)
			{
				obj = new EWSServiceCredentialsElement.EWSServiceCredentials();
				serviceCredentials = (obj as ServiceCredentials);
				base.ApplyConfiguration(serviceCredentials);
			}
			else
			{
				obj = base.CreateBehavior();
				serviceCredentials = (obj as ServiceCredentials);
			}
			if (serviceCredentials == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "Not adding Exchange certificates to ServiceCredentials. Behavior is not a ServiceCredentials - instead, it is of type {0}", obj.GetType().FullName);
				return obj;
			}
			if (serviceCredentials.IssuedTokenAuthentication == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "Not adding Exchange certificates to ServiceCredentials. ServiceCredentials.IssuedTokenAuthentication is null");
				return obj;
			}
			if (serviceCredentials.IssuedTokenAuthentication.KnownCertificates == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "Not adding Exchange certificates to ServiceCredentials. ServiceCredentials.IssuedTokenAuthentication.KnownCertificates is null");
				return obj;
			}
			ExternalAuthentication current = ExternalAuthentication.GetCurrent();
			ApplicationPoolRecycler.EnableOnFederationTrustCertificateChange();
			if (!current.Enabled)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.GetHashCode(), "Not adding Exchange certificates to ServiceCredentials. ExternalAuthentication.Enabled is false");
				return obj;
			}
			serviceCredentials.IssuedTokenAuthentication.AllowedAudienceUris.Add(current.TokenValidator.TargetUri.OriginalString);
			foreach (X509Certificate2 x509Certificate in current.Certificates)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<X509Certificate2>((long)this.GetHashCode(), "Adding Exchange certificate to ServiceCredentials: {0}", x509Certificate);
				serviceCredentials.IssuedTokenAuthentication.KnownCertificates.Add(x509Certificate);
			}
			return obj;
		}

		internal class EWSServiceCredentials : ServiceCredentials
		{
			protected override ServiceCredentials CloneCore()
			{
				return new EWSServiceCredentialsElement.EWSServiceCredentials();
			}

			public override SecurityTokenManager CreateSecurityTokenManager()
			{
				return new EWSServiceCredentialsElement.EWSServiceCredentialsSecurityTokenManager(this);
			}
		}

		internal class EWSServiceCredentialsSecurityTokenManager : ServiceCredentialsSecurityTokenManager
		{
			public EWSServiceCredentialsSecurityTokenManager(EWSServiceCredentialsElement.EWSServiceCredentials serviceCredentials) : base(serviceCredentials)
			{
			}

			public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver)
			{
				SecurityTokenAuthenticator securityTokenAuthenticator = base.CreateSecurityTokenAuthenticator(tokenRequirement, out outOfBandTokenResolver);
				if (securityTokenAuthenticator is SamlSecurityTokenAuthenticator)
				{
					outOfBandTokenResolver = new EWSServiceCredentialsElement.EWSSecurityTokenResolver(outOfBandTokenResolver);
				}
				return securityTokenAuthenticator;
			}
		}

		internal class EWSSecurityTokenResolver : SecurityTokenResolver
		{
			public EWSSecurityTokenResolver(SecurityTokenResolver underlyingTokenResolver)
			{
				this.underlyingTokenResolver = underlyingTokenResolver;
			}

			public static ExactTimeoutCache<EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper, EWSServiceCredentialsElement.EWSSecurityTokenResolver.SecurityKeyAndToken> Cache
			{
				get
				{
					if (EWSServiceCredentialsElement.EWSSecurityTokenResolver.cache == null)
					{
						lock (EWSServiceCredentialsElement.EWSSecurityTokenResolver.lockObj)
						{
							if (EWSServiceCredentialsElement.EWSSecurityTokenResolver.cache == null)
							{
								EWSServiceCredentialsElement.EWSSecurityTokenResolver.cache = new ExactTimeoutCache<EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper, EWSServiceCredentialsElement.EWSSecurityTokenResolver.SecurityKeyAndToken>(delegate(EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper key, EWSServiceCredentialsElement.EWSSecurityTokenResolver.SecurityKeyAndToken value, RemoveReason reason)
								{
									ExTraceGlobals.AuthenticationTracer.TraceDebug<EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper, RemoveReason>(0L, "Removing the cached entry with key {0} due to {1}", key, reason);
									PerformanceMonitor.UpdatePartnerTokenCacheEntries(EWSServiceCredentialsElement.EWSSecurityTokenResolver.Cache.Count);
								}, null, null, EWSServiceCredentialsElement.EWSSecurityTokenResolver.cacheSize.Value, false);
							}
						}
					}
					return EWSServiceCredentialsElement.EWSSecurityTokenResolver.cache;
				}
			}

			protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
			{
				if (keyIdentifier.Count == 1 && keyIdentifier[0] is EncryptedKeyIdentifierClause)
				{
					EncryptedKeyIdentifierClause encryptedKeyIdentifierClause = keyIdentifier[0] as EncryptedKeyIdentifierClause;
					EWSServiceCredentialsElement.EWSSecurityTokenResolver.SecurityKeyAndToken securityKeyAndToken;
					if (this.IsPartnerTokenEncryptedKeyIdentifierClause(encryptedKeyIdentifierClause) && EWSServiceCredentialsElement.EWSSecurityTokenResolver.Cache.TryGetValue(new EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper(encryptedKeyIdentifierClause), out securityKeyAndToken))
					{
						token = securityKeyAndToken.SecurityToken;
						return true;
					}
				}
				return this.underlyingTokenResolver.TryResolveToken(keyIdentifier, out token);
			}

			protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
			{
				EncryptedKeyIdentifierClause encryptedKeyIdentifierClause = keyIdentifierClause as EncryptedKeyIdentifierClause;
				EWSServiceCredentialsElement.EWSSecurityTokenResolver.SecurityKeyAndToken securityKeyAndToken;
				if (this.IsPartnerTokenEncryptedKeyIdentifierClause(encryptedKeyIdentifierClause) && EWSServiceCredentialsElement.EWSSecurityTokenResolver.Cache.TryGetValue(new EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper(encryptedKeyIdentifierClause), out securityKeyAndToken))
				{
					token = securityKeyAndToken.SecurityToken;
					return true;
				}
				return this.underlyingTokenResolver.TryResolveToken(keyIdentifierClause, out token);
			}

			protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
			{
				EncryptedKeyIdentifierClause encryptedKeyIdentifierClause = keyIdentifierClause as EncryptedKeyIdentifierClause;
				if (this.IsPartnerTokenEncryptedKeyIdentifierClause(encryptedKeyIdentifierClause))
				{
					EWSServiceCredentialsElement.EWSSecurityTokenResolver.SecurityKeyAndToken securityKeyAndToken;
					if (EWSServiceCredentialsElement.EWSSecurityTokenResolver.Cache.TryGetValue(new EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper(encryptedKeyIdentifierClause), out securityKeyAndToken))
					{
						key = securityKeyAndToken.SecurityKey;
						return true;
					}
					SecurityKeyIdentifier encryptingKeyIdentifier = encryptedKeyIdentifierClause.EncryptingKeyIdentifier;
					SecurityToken securityToken;
					if (base.TryResolveToken(encryptingKeyIdentifier, out securityToken))
					{
						byte[] encryptedKey = encryptedKeyIdentifierClause.GetEncryptedKey();
						string encryptionMethod = encryptedKeyIdentifierClause.EncryptionMethod;
						SecurityKey securityKey = securityToken.SecurityKeys[0];
						byte[] array = securityKey.DecryptKey(encryptionMethod, encryptedKey);
						key = new InMemorySymmetricSecurityKey(array, false);
						SecurityToken token = new WrappedKeySecurityToken("uuid-" + Guid.NewGuid().ToString(), array, encryptionMethod, securityToken, encryptingKeyIdentifier);
						EWSServiceCredentialsElement.EWSSecurityTokenResolver.Cache.TryAddAbsolute(new EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper(encryptedKeyIdentifierClause), new EWSServiceCredentialsElement.EWSSecurityTokenResolver.SecurityKeyAndToken(key, token), EWSServiceCredentialsElement.EWSSecurityTokenResolver.cacheTimeToLive.Value);
						ExTraceGlobals.AuthenticationTracer.TraceDebug<EncryptedKeyIdentifierClause>((long)this.GetHashCode(), "Adding a new entry with key: {0}", encryptedKeyIdentifierClause);
						PerformanceMonitor.UpdatePartnerTokenCacheEntries(EWSServiceCredentialsElement.EWSSecurityTokenResolver.Cache.Count);
						return true;
					}
					ExTraceGlobals.AuthenticationTracer.TraceDebug<SecurityKeyIdentifierClause>((long)this.GetHashCode(), "Calling the underlying TokenResolver.TryResolveSecurityKey, the clause is {0}", keyIdentifierClause);
				}
				return this.underlyingTokenResolver.TryResolveSecurityKey(keyIdentifierClause, out key);
			}

			private bool IsPartnerTokenEncryptedKeyIdentifierClause(EncryptedKeyIdentifierClause keyClause)
			{
				return keyClause != null && keyClause.CarriedKeyName != null && keyClause.CarriedKeyName.Equals(PartnerInfo.KeyName);
			}

			private static readonly TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("PartnerEncryptedKeyIdentifierClauseCacheTimeToLive", TimeSpanUnit.Minutes, TimeSpan.FromHours(4.0), ExTraceGlobals.CommonAlgorithmTracer);

			private static readonly IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("PartnerEncryptedKeyIdentifierClauseCacheLimit", 1000, ExTraceGlobals.CommonAlgorithmTracer);

			private static readonly object lockObj = new object();

			private static ExactTimeoutCache<EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper, EWSServiceCredentialsElement.EWSSecurityTokenResolver.SecurityKeyAndToken> cache;

			private SecurityTokenResolver underlyingTokenResolver;

			public sealed class EncryptedKeyIdentifierClauseWrapper
			{
				public EncryptedKeyIdentifierClauseWrapper(EncryptedKeyIdentifierClause clause)
				{
					this.clause = clause;
					byte[] buffer = this.clause.GetBuffer();
					this.hashCode = ((int)buffer[0] << 24 | (int)buffer[1] << 16 | (int)buffer[2] << 8 | (int)buffer[3]);
				}

				public override bool Equals(object obj)
				{
					EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper encryptedKeyIdentifierClauseWrapper = obj as EWSServiceCredentialsElement.EWSSecurityTokenResolver.EncryptedKeyIdentifierClauseWrapper;
					return encryptedKeyIdentifierClauseWrapper != null && this.clause.Matches(encryptedKeyIdentifierClauseWrapper.clause);
				}

				public override int GetHashCode()
				{
					return this.hashCode;
				}

				private readonly EncryptedKeyIdentifierClause clause;

				private readonly int hashCode;
			}

			public class SecurityKeyAndToken
			{
				public SecurityKeyAndToken(SecurityKey key, SecurityToken token)
				{
					this.key = key;
					this.token = token;
				}

				public SecurityKey SecurityKey
				{
					get
					{
						return this.key;
					}
				}

				public SecurityToken SecurityToken
				{
					get
					{
						return this.token;
					}
				}

				private readonly SecurityKey key;

				private readonly SecurityToken token;
			}
		}
	}
}
