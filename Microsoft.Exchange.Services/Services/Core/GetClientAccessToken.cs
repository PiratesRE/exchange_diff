using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetClientAccessToken : MultiStepServiceCommand<GetClientAccessTokenRequest, ClientAccessTokenResponseType>
	{
		public GetClientAccessToken(CallContext callContext, GetClientAccessTokenRequest request) : base(callContext, request)
		{
			this.request = request;
		}

		internal static bool IsKnownClientAccessType(ClientAccessTokenType tokenType)
		{
			return tokenType == ClientAccessTokenType.CallerIdentity || tokenType == ClientAccessTokenType.ExtensionCallback || tokenType == ClientAccessTokenType.ScopedToken;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetClientAccessTokenResponse getClientAccessTokenResponse = new GetClientAccessTokenResponse();
			getClientAccessTokenResponse.BuildForResults<ClientAccessTokenResponseType>(base.Results);
			return getClientAccessTokenResponse;
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			if (this.StepCount == 0)
			{
				return;
			}
			this.PrepareForExtensionRelatedTokens();
		}

		internal override int StepCount
		{
			get
			{
				return this.request.TokenRequests.Length;
			}
		}

		internal override ServiceResult<ClientAccessTokenResponseType> Execute()
		{
			ClientAccessTokenRequestType clientAccessTokenRequestType = this.request.TokenRequests[base.CurrentStep];
			Uri requestUriForToken = this.GetRequestUriForToken(clientAccessTokenRequestType.TokenType);
			if (!GetClientAccessToken.IsKnownClientAccessType(clientAccessTokenRequestType.TokenType))
			{
				throw new InvalidClientAccessTokenRequestException(CoreResources.IDs.MessageTokenRequestUnauthorized);
			}
			ExtensionData extensionData = null;
			if (this.extensionDataList != null)
			{
				foreach (ExtensionData extensionData2 in this.extensionDataList)
				{
					if (extensionData2.Enabled && extensionData2.ExtensionId.Equals(ExtensionDataHelper.FormatExtensionId(clientAccessTokenRequestType.Id), StringComparison.OrdinalIgnoreCase))
					{
						extensionData = extensionData2;
						break;
					}
				}
			}
			if (extensionData == null)
			{
				throw new InvalidClientAccessTokenRequestException(CoreResources.IDs.GetClientExtensionTokenFailed);
			}
			if ((clientAccessTokenRequestType.TokenType == ClientAccessTokenType.ExtensionCallback && extensionData.RequestedCapabilities != RequestedCapabilities.ReadWriteMailbox) || (clientAccessTokenRequestType.TokenType == ClientAccessTokenType.ScopedToken && extensionData.RequestedCapabilities < RequestedCapabilities.ReadItem))
			{
				throw new InvalidClientAccessTokenRequestException(CoreResources.IDs.MessageTokenRequestUnauthorized);
			}
			return GetClientAccessToken.CreateTokenForExtension(clientAccessTokenRequestType.Id, extensionData.IdentityAndEwsTokenId, this.organizationId, this.adUser, clientAccessTokenRequestType.TokenType, requestUriForToken, clientAccessTokenRequestType.Scope);
		}

		private static ServiceResult<ClientAccessTokenResponseType> CreateTokenForExtension(string extensionId, string authIdForExtension, OrganizationId organizationId, ADUser adUser, ClientAccessTokenType type, Uri requestUri, string scope)
		{
			TokenResult tokenResult;
			try
			{
				LocalTokenIssuer localTokenIssuer = new LocalTokenIssuer(organizationId);
				if (type == ClientAccessTokenType.ExtensionCallback)
				{
					tokenResult = localTokenIssuer.GetExtensionCallbackToken(extensionId, authIdForExtension, requestUri, adUser, null);
				}
				else if (type == ClientAccessTokenType.ScopedToken)
				{
					if (string.IsNullOrWhiteSpace(scope))
					{
						throw new InvalidClientAccessTokenRequestException((CoreResources.IDs)3308334241U);
					}
					tokenResult = localTokenIssuer.GetExtensionCallbackToken(extensionId, authIdForExtension, requestUri, adUser, scope);
				}
				else
				{
					tokenResult = localTokenIssuer.GetCallerIdentityToken(authIdForExtension, new Dictionary<string, string>(3)
					{
						{
							Constants.ClaimTypes.MsExchImmutableId,
							adUser.ExchangeGuid.ToString()
						},
						{
							Constants.ClaimTypes.Version,
							Constants.ClaimValues.ExIdTokV1
						},
						{
							Constants.ClaimTypes.AuthMetaDocumentUrl,
							GetClientAccessToken.GetAuthMetaDocumentUrl(requestUri)
						}
					});
				}
			}
			catch (OAuthTokenRequestFailedException innerException)
			{
				throw new InvalidClientAccessTokenRequestException(CoreResources.IDs.GetClientExtensionTokenFailed, innerException);
			}
			return new ServiceResult<ClientAccessTokenResponseType>(new ClientAccessTokenResponseType
			{
				Id = extensionId,
				TokenType = type,
				TokenValue = tokenResult.TokenString,
				TTL = (int)(tokenResult.ExpirationDate - DateTime.UtcNow).TotalMinutes
			});
		}

		private static string GetAuthMetaDocumentUrl(Uri requestUri)
		{
			return new UriBuilder(requestUri)
			{
				Path = "/autodiscover/metadata/json/1",
				Query = string.Empty
			}.ToString();
		}

		private void PrepareForExtensionRelatedTokens()
		{
			HashSet<string> formattedRequestedExtensionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (ClientAccessTokenRequestType clientAccessTokenRequestType in this.request.TokenRequests)
			{
				if (GetClientAccessToken.IsKnownClientAccessType(clientAccessTokenRequestType.TokenType))
				{
					formattedRequestedExtensionIds.Add(ExtensionDataHelper.FormatExtensionId(clientAccessTokenRequestType.Id));
				}
			}
			if (0 < formattedRequestedExtensionIds.Count)
			{
				this.organizationId = ((base.CallContext.AccessingPrincipal == null) ? OrganizationId.ForestWideOrgId : base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId);
				if (!ADIdentityInformationCache.Singleton.TryGetADUser(base.CallContext.EffectiveCallerSid, base.CallContext.ADRecipientSessionContext, out this.adUser))
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorInvalidUserSid);
				}
				Exception ex = InstalledExtensionTable.RunClientExtensionAction(delegate
				{
					this.extensionDataList = GetExtensibilityContext.GetUserExtensionDataList(this.CallContext, formattedRequestedExtensionIds);
				});
				if (ex != null)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.MessageTokenRequestUnauthorized, ex);
				}
			}
			try
			{
				this.externalEwsUrl = new Uri(EwsHelper.DiscoverExternalEwsUrl(base.CallContext.AccessingPrincipal));
			}
			catch (ArgumentNullException innerException)
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.GetClientExtensionTokenFailed, innerException);
			}
			catch (UriFormatException innerException2)
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.GetClientExtensionTokenFailed, innerException2);
			}
		}

		private Uri GetRequestUriForToken(ClientAccessTokenType tokenType)
		{
			if (tokenType == ClientAccessTokenType.ExtensionCallback)
			{
				if (base.CallContext.IsOwa && VariantConfiguration.InvariantNoFlightingSnapshot.Ews.UseInternalEwsUrlForExtensionEwsProxyInOwa.Enabled)
				{
					Uri uri = new Uri(EwsHelper.DiscoverEwsUrl(base.CallContext.AccessingPrincipal));
					string uriString = string.Format("https://{0}/ews/exchange.asmx", uri.Host);
					return new Uri(uriString);
				}
				if (!base.CallContext.IsOwa)
				{
					return new Uri(base.CallContext.HttpContext.Request.Headers[WellKnownHeader.MsExchProxyUri]);
				}
			}
			return this.externalEwsUrl;
		}

		private const string EwsUrlFormat = "https://{0}/ews/exchange.asmx";

		private GetClientAccessTokenRequest request;

		private ADUser adUser;

		private List<ExtensionData> extensionDataList;

		private OrganizationId organizationId;

		private Uri externalEwsUrl;
	}
}
