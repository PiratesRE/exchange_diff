using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Net.XropService
{
	internal sealed class FactoryClientCredentials : ClientCredentials
	{
		public FactoryClientCredentials()
		{
			base.SupportInteractive = false;
		}

		private FactoryClientCredentials(FactoryClientCredentials other) : base(other)
		{
			base.SupportInteractive = other.SupportInteractive;
		}

		public override SecurityTokenManager CreateSecurityTokenManager()
		{
			return new FactoryClientCredentials.FactoryClientCredentialsSecurityTokenManager(this);
		}

		protected override ClientCredentials CloneCore()
		{
			return new FactoryClientCredentials(this);
		}

		private sealed class FactoryClientCredentialsSecurityTokenManager : ClientCredentialsSecurityTokenManager
		{
			public FactoryClientCredentialsSecurityTokenManager(FactoryClientCredentials factoryClientCredentials) : base(factoryClientCredentials)
			{
			}

			public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
			{
				if (tokenRequirement.TokenType == SecurityTokenTypes.UserName)
				{
					FederatedClientCredentials federatedClientCredentials = FactoryClientCredentials.FactoryClientCredentialsSecurityTokenManager.FindChannelFederatedClientCredentials(tokenRequirement);
					return new FactoryClientCredentials.SupportingSecurityTokenProvider(federatedClientCredentials);
				}
				if (tokenRequirement.TokenType == SecurityTokenTypes.Saml || tokenRequirement.TokenType == "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1")
				{
					FederatedClientCredentials federatedClientCredentials2 = FactoryClientCredentials.FactoryClientCredentialsSecurityTokenManager.FindChannelFederatedClientCredentials(tokenRequirement);
					return new FactoryClientCredentials.PrimarySecurityTokenProvider(federatedClientCredentials2);
				}
				return base.CreateSecurityTokenProvider(tokenRequirement);
			}

			internal static FederatedClientCredentials FindChannelFederatedClientCredentials(SecurityTokenRequirement tokenRequirement)
			{
				ChannelParameterCollection channelParameterCollection = null;
				if (tokenRequirement.TryGetProperty<ChannelParameterCollection>(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty, out channelParameterCollection) && channelParameterCollection != null)
				{
					foreach (object obj in channelParameterCollection)
					{
						FederatedClientCredentials federatedClientCredentials = obj as FederatedClientCredentials;
						if (federatedClientCredentials != null)
						{
							return federatedClientCredentials;
						}
					}
				}
				ExTraceGlobals.XropServiceClientTracer.TraceError(0L, "XropFactoryCredentials: No federated credentials found in channel parameters.");
				throw new InvalidOperationException();
			}
		}

		private sealed class PrimarySecurityTokenProvider : SecurityTokenProvider
		{
			public PrimarySecurityTokenProvider(FederatedClientCredentials federatedClientCredentials)
			{
				this.federatedClientCredentials = federatedClientCredentials;
			}

			protected override SecurityToken GetTokenCore(TimeSpan timeout)
			{
				RequestedToken token = this.federatedClientCredentials.GetToken();
				ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "PrimarySecurityTokenProvider issuing SAML token");
				return token.GetSecurityToken();
			}

			private FederatedClientCredentials federatedClientCredentials;
		}

		private sealed class SupportingSecurityTokenProvider : SecurityTokenProvider
		{
			public SupportingSecurityTokenProvider(FederatedClientCredentials federatedClientCredentials)
			{
				this.federatedClientCredentials = federatedClientCredentials;
			}

			protected override SecurityToken GetTokenCore(TimeSpan timeout)
			{
				if (string.IsNullOrEmpty(this.federatedClientCredentials.UserEmailAddress))
				{
					throw new ArgumentException("UserEmailAddress");
				}
				ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "SupportingSecurityTokenProvider issuing UserNameSecurityToken");
				return new UserNameSecurityToken(this.federatedClientCredentials.UserEmailAddress, null);
			}

			private FederatedClientCredentials federatedClientCredentials;
		}
	}
}
