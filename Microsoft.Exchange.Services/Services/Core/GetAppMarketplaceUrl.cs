using System;
using System.Configuration;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetAppMarketplaceUrl : SingleStepServiceCommand<GetAppMarketplaceUrlRequest, string>
	{
		public GetAppMarketplaceUrl(CallContext callContext, GetAppMarketplaceUrlRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetAppMarketplaceUrlResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<string> Execute()
		{
			string clientExtensionMarketplaceUrl = string.Empty;
			Version schemaVersion1_ = SchemaConstants.SchemaVersion1_0;
			Version schemaVersionSupported = SchemaConstants.SchemaVersion1_0;
			if (base.Request.ApiVersionSupported != null && !Version.TryParse(base.Request.ApiVersionSupported, out schemaVersion1_))
			{
				throw new InvalidRequestException((CoreResources.IDs)2449079760U);
			}
			if (base.Request.SchemaVersionSupported != null && !Version.TryParse(base.Request.SchemaVersionSupported, out schemaVersionSupported))
			{
				throw new InvalidRequestException((CoreResources.IDs)3555230765U);
			}
			bool cannotGetEcpUrl = false;
			bool backEndLocatorFailed = false;
			ServiceError serviceError = GetExtensibilityContext.RunClientExtensionAction(delegate
			{
				Uri uri = null;
				Exception ex = null;
				try
				{
					if (this.CallContext.AccessingPrincipal == null)
					{
						throw new NameResolutionNoMailboxException();
					}
					uri = FrontEndLocator.GetFrontEndEcpUrl(this.CallContext.AccessingPrincipal);
					string text = uri.ToString();
					if (!text.EndsWith("/"))
					{
						uri = new Uri(uri + "/");
					}
				}
				catch (ServerNotFoundException ex2)
				{
					ex = ex2;
				}
				catch (ADTransientException ex3)
				{
					ex = ex3;
				}
				catch (DataSourceOperationException ex4)
				{
					ex = ex4;
				}
				catch (DataValidationException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					CallContext.Current.ProtocolLog.AppendGenericInfo("EcpNotFound", ex);
					backEndLocatorFailed = true;
					return;
				}
				if (uri == null)
				{
					cannotGetEcpUrl = true;
					return;
				}
				MailboxSession mailboxIdentityMailboxSession = this.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
				string text2 = string.Empty;
				bool withinReadWriteMailboxRole = false;
				if (this.CallContext != null && this.CallContext.EffectiveCaller != null)
				{
					if (!string.IsNullOrWhiteSpace(this.CallContext.EffectiveCaller.PrimarySmtpAddress))
					{
						SmtpAddress smtpAddress = new SmtpAddress(this.CallContext.EffectiveCaller.PrimarySmtpAddress);
						if (smtpAddress.IsValidAddress)
						{
							text2 = smtpAddress.Domain;
						}
					}
					ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(this.CallContext.EffectiveCaller, null, false);
					if (exchangeRunspaceConfiguration.HasRoleOfType(RoleType.MyReadWriteMailboxApps))
					{
						withinReadWriteMailboxRole = true;
					}
				}
				string deploymentId = ExtensionDataHelper.GetDeploymentId(text2);
				string text3 = ConfigurationManager.AppSettings["OfficeStoreUnavailable"];
				clientExtensionMarketplaceUrl = ((string.IsNullOrWhiteSpace(text3) || StringComparer.OrdinalIgnoreCase.Equals("false", text3)) ? ExtensionData.GetClientExtensionMarketplaceUrl(mailboxIdentityMailboxSession, uri, withinReadWriteMailboxRole, deploymentId, schemaVersionSupported, text2) : string.Empty);
			});
			if (cannotGetEcpUrl)
			{
				serviceError = new ServiceError(CoreResources.IDs.ErrorCannotGetExternalEcpUrl, ResponseCodeType.ErrorCannotGetExternalEcpUrl, 0, ExchangeVersion.Exchange2012);
			}
			if (backEndLocatorFailed)
			{
				serviceError = new ServiceError(CoreResources.IDs.ErrorProxyServiceDiscoveryFailed, ResponseCodeType.ErrorProxyServiceDiscoveryFailed, 0, ExchangeVersion.Exchange2012);
			}
			if (serviceError != null)
			{
				return new ServiceResult<string>(serviceError);
			}
			return new ServiceResult<string>(clientExtensionMarketplaceUrl);
		}
	}
}
