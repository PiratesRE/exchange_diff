using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetAppManifests : SingleStepServiceCommand<GetAppManifestsRequest, XmlElement>
	{
		public GetAppManifests(CallContext callContext, GetAppManifestsRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register(base.GetType().Name, typeof(GetExtensionsMetadata), new Type[0]);
		}

		internal static string GetEncodedManifestString(ExtensionData extensionData)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(extensionData.Manifest.OuterXml);
			return Convert.ToBase64String(bytes);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetAppManifestsResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<XmlElement> Execute()
		{
			Version apiVersionSupported = SchemaConstants.Exchange2013RtmApiVersion;
			Version schemaVersionSupported = SchemaConstants.SchemaVersion1_0;
			bool useE15Sp1Schema = ExchangeVersion.Current.Supports(ExchangeVersion.ExchangeV2_6);
			if (base.Request.ApiVersionSupported != null && !Version.TryParse(base.Request.ApiVersionSupported, out apiVersionSupported))
			{
				throw new InvalidRequestException((CoreResources.IDs)2449079760U);
			}
			if (base.Request.SchemaVersionSupported != null && !Version.TryParse(base.Request.SchemaVersionSupported, out schemaVersionSupported))
			{
				throw new InvalidRequestException((CoreResources.IDs)3555230765U);
			}
			XmlElement collectionContainerElement = ServiceXml.CreateElement(base.XmlDocument, useE15Sp1Schema ? "Apps" : "Manifests", "http://schemas.microsoft.com/exchange/services/2006/messages");
			ServiceError serviceError = GetExtensibilityContext.RunClientExtensionAction(delegate
			{
				List<ExtensionData> userExtensionDataListWithUpdateCheck = GetExtensibilityContext.GetUserExtensionDataListWithUpdateCheck(this.CallContext, true, schemaVersionSupported == SchemaConstants.SchemaVersion1_0);
				if (userExtensionDataListWithUpdateCheck != null)
				{
					ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(this.CallContext.EffectiveCaller, null, false);
					string deploymentId = GetExtensibilityContext.GetDeploymentId(this.CallContext);
					int lcid = this.CallContext.SessionCache.GetMailboxIdentityMailboxSession().Culture.LCID;
					bool withinOrgMarketplaceRole = exchangeRunspaceConfiguration.HasRoleOfType(RoleType.OrgMarketplaceApps);
					foreach (ExtensionData extensionData in userExtensionDataListWithUpdateCheck)
					{
						if (extensionData.Enabled && apiVersionSupported >= extensionData.MinApiVersion && schemaVersionSupported >= extensionData.SchemaVersion)
						{
							if (ExtensionInstallScope.Default == extensionData.Scope.GetValueOrDefault())
							{
								if (this.CallContext.AccessingPrincipal == null)
								{
									throw new NameResolutionNoMailboxException();
								}
								Exception ex;
								if (!DefaultExtensionTable.TryUpdateDefaultExtensionPath(this.CallContext.AccessingPrincipal, "SourceLocation", extensionData, out ex))
								{
									if (ex == null)
									{
										ex = new ServiceInvalidOperationException(ResponseCodeType.ErrorServiceDiscoveryFailed);
									}
									throw FaultExceptionUtilities.CreateFault(new ServiceDiscoveryFailedException(ex), FaultParty.Receiver);
								}
							}
							EntitlementTokenData.UpdatePaidAppSourceLocation("SourceLocation", extensionData);
							if (useE15Sp1Schema)
							{
								this.WriteExtensionNodeWithE15Sp1Schema(collectionContainerElement, extensionData, lcid, withinOrgMarketplaceRole, deploymentId);
							}
							else
							{
								ServiceXml.CreateTextElement(collectionContainerElement, "Manifest", GetAppManifests.GetEncodedManifestString(extensionData));
							}
						}
					}
				}
			});
			if (serviceError != null)
			{
				return new ServiceResult<XmlElement>(collectionContainerElement, serviceError);
			}
			return new ServiceResult<XmlElement>(collectionContainerElement);
		}

		private void WriteExtensionNodeWithE15Sp1Schema(XmlElement parentElement, ExtensionData extensionData, int lcid, bool withinOrgMarketplaceRole, string deploymentId)
		{
			XmlElement parentElement2 = ServiceXml.CreateElement(parentElement, "App", "http://schemas.microsoft.com/exchange/services/2006/types");
			string text = string.Empty;
			string textValue = string.Empty;
			if (extensionData.Type != null && extensionData.Type.Value == ExtensionType.MarketPlace)
			{
				XmlElement parentElement3 = ServiceXml.CreateElement(parentElement2, "Metadata", "http://schemas.microsoft.com/exchange/services/2006/types");
				string marketPlaceEndNodeUrl = GetExtensibilityContext.GetMarketPlaceEndNodeUrl(extensionData, base.CallContext.HttpContext.Request, lcid, withinOrgMarketplaceRole, deploymentId);
				if (!string.IsNullOrWhiteSpace(extensionData.AppStatus) && ((extensionData.Scope == ExtensionInstallScope.Organization && withinOrgMarketplaceRole) || extensionData.Scope == ExtensionInstallScope.User))
				{
					text = extensionData.AppStatus;
					textValue = GetExtensibilityContext.GetErrorUXActionLinkUrl(extensionData, base.CallContext.HttpContext.Request, lcid, withinOrgMarketplaceRole, deploymentId);
				}
				ServiceXml.CreateTextElement(parentElement3, "EndNodeUrl", marketPlaceEndNodeUrl, "http://schemas.microsoft.com/exchange/services/2006/types");
				if (!string.IsNullOrEmpty(text))
				{
					ServiceXml.CreateTextElement(parentElement3, "AppStatus", text, "http://schemas.microsoft.com/exchange/services/2006/types");
					ServiceXml.CreateTextElement(parentElement3, "ActionUrl", textValue, "http://schemas.microsoft.com/exchange/services/2006/types");
				}
				else if (extensionData.EtokenData != null && extensionData.EtokenData.LicenseType == LicenseType.Trial)
				{
					ServiceXml.CreateTextElement(parentElement3, "AppStatus", "5.0", "http://schemas.microsoft.com/exchange/services/2006/types");
					ServiceXml.CreateTextElement(parentElement3, "ActionUrl", marketPlaceEndNodeUrl, "http://schemas.microsoft.com/exchange/services/2006/types");
				}
			}
			ServiceXml.CreateTextElement(parentElement2, "Manifest", GetAppManifests.GetEncodedManifestString(extensionData), "http://schemas.microsoft.com/exchange/services/2006/types");
		}
	}
}
