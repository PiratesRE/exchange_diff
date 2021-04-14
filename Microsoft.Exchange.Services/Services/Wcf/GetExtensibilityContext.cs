using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetExtensibilityContext : ServiceCommand<ExtensibilityContext>
	{
		public GetExtensibilityContext(CallContext callContext, GetExtensibilityContextParameters request) : base(callContext)
		{
			this.request = request;
			OwsLogRegistry.Register(base.GetType().Name, typeof(GetExtensionsMetadata), new Type[0]);
		}

		internal static bool TryCreateExtension(GetExtensibilityContextParameters request, ExtensionData extensionData, out Extension extension)
		{
			extension = new Extension();
			extension.Type = extensionData.Type.Value;
			extension.Origin = extensionData.Scope.Value;
			extension.RequestedCapabilities = extensionData.RequestedCapabilities.Value;
			extension.ProviderName = extensionData.ProviderName;
			extension.IconUrl = ((extensionData.IconURL != null) ? extensionData.IconURL.ToString() : string.Empty);
			extension.HighResolutionIconUrl = ((extensionData.HighResolutionIconURL != null) ? extensionData.HighResolutionIconURL.ToString() : string.Empty);
			extension.AppStatus = extensionData.AppStatus;
			extension.LicenseType = ((extensionData.EtokenData != null) ? extensionData.EtokenData.LicenseType : LicenseType.Free);
			XmlDocument manifest = extensionData.Manifest;
			if (manifest == null || manifest.ChildNodes == null || manifest.ChildNodes.Count == 0 || manifest.ChildNodes[0].ChildNodes == null)
			{
				return false;
			}
			List<FormSettings> formSettings = extensionData.GetFormSettings(request.FormFactor);
			if (formSettings.Count == 0)
			{
				return false;
			}
			extension.FormSettingsList = formSettings.ToArray();
			extension.DisableEntityHighlighting = extensionData.GetDisableEntityHighlighting();
			ActivationRule activationRule = null;
			if (!extensionData.TryGetActivationRule(out activationRule))
			{
				return false;
			}
			extension.ActivationRule = activationRule;
			extension.DisplayName = extensionData.DisplayName;
			extension.Id = extensionData.ExtensionId;
			extension.Version = extensionData.VersionAsString;
			extension.AuthTokenId = extensionData.IdentityAndEwsTokenId;
			return true;
		}

		internal static ServiceError RunClientExtensionAction(Action action)
		{
			Exception ex = InstalledExtensionTable.RunClientExtensionAction(action);
			if (ex == null)
			{
				return null;
			}
			ResponseCodeType messageKey = (ex is TransientException) ? ResponseCodeType.ErrorInternalServerTransientError : ResponseCodeType.ErrorInternalServerError;
			string exceptionMessages = ExtensionDataHelper.GetExceptionMessages(ex);
			return new ServiceError(exceptionMessages, messageKey, 0, ExchangeVersion.Exchange2012);
		}

		internal static List<ExtensionData> GetUserExtensionDataListWithUpdateCheck(CallContext callContext, bool shouldFailOnGetOrgExtensionsTimeout = true, bool retrieveOnly1_0 = false)
		{
			return GetExtensibilityContext.GetUserExtensions(callContext, true, true, shouldFailOnGetOrgExtensionsTimeout, ExtensionsCache.Singleton, null, retrieveOnly1_0);
		}

		internal static List<ExtensionData> GetUserExtensionDataList(CallContext callContext, HashSet<string> formattedRequestedExtensionIds)
		{
			return GetExtensibilityContext.GetUserExtensions(callContext, true, true, true, null, formattedRequestedExtensionIds, false);
		}

		internal static List<ExtensionData> GetOrgExtensionDataList(CallContext callContext, bool isUserScope, bool isRawXmlRequired, out string masterTableRawXml)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			List<ExtensionData> list = null;
			OrganizationId organizationId = GetExtensibilityContext.GetOrganizationId(callContext);
			masterTableRawXml = string.Empty;
			if (OrgEmptyMasterTableCache.Singleton.IsEmpty(organizationId))
			{
				Dictionary<string, ExtensionData> defaultExtensions = InstalledExtensionTable.GetDefaultExtensions(callContext.MailboxIdentityPrincipal);
				list = new List<ExtensionData>();
				using (Dictionary<string, ExtensionData>.Enumerator enumerator = defaultExtensions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, ExtensionData> keyValuePair = enumerator.Current;
						list.Add(keyValuePair.Value);
					}
					goto IL_8D;
				}
			}
			list = GetExtensibilityContext.GetExtensions(callContext, isUserScope, false, true, OrgEmptyMasterTableCache.Singleton, ExtensionsCache.Singleton, null, isRawXmlRequired, out masterTableRawXml, false, false);
			IL_8D:
			stopwatch.Stop();
			callContext.ProtocolLog.Set(GetExtensionsMetadata.GetOrgExtensionsTime, stopwatch.ElapsedMilliseconds);
			return list;
		}

		internal static string GetDeploymentId(CallContext callContext)
		{
			string domain = string.Empty;
			if (callContext.EffectiveCaller != null && !string.IsNullOrWhiteSpace(callContext.EffectiveCaller.PrimarySmtpAddress))
			{
				SmtpAddress smtpAddress = new SmtpAddress(callContext.EffectiveCaller.PrimarySmtpAddress);
				if (smtpAddress.IsValidAddress)
				{
					domain = smtpAddress.Domain;
				}
			}
			return ExtensionDataHelper.GetDeploymentId(domain);
		}

		internal static string GetMarketPlaceEndNodeUrl(ExtensionData extensionData, HttpRequest httpRequest, int lcid, bool withinMarketplaceRole, string deploymentId)
		{
			if (extensionData.Type == ExtensionType.MarketPlace)
			{
				return ExtensionData.GetClientExtensionAppDetailsUrl(lcid, httpRequest, withinMarketplaceRole, deploymentId, extensionData.MarketplaceContentMarket, extensionData.MarketplaceAssetID, null);
			}
			return null;
		}

		internal static string GetErrorUXActionLinkUrl(ExtensionData extensionData, HttpRequest httpRequest, int lcid, bool withinReadWriteMailboxRole, string deploymentId)
		{
			if ("1.0".Equals(extensionData.AppStatus, StringComparison.OrdinalIgnoreCase) || "1.1".Equals(extensionData.AppStatus, StringComparison.OrdinalIgnoreCase))
			{
				return ExtensionData.GenerateOfficeCallbackUrlForReconsent(httpRequest, null, extensionData.MarketplaceAssetID, extensionData.MarketplaceContentMarket, extensionData.Scope ?? ExtensionInstallScope.User, extensionData.Etoken);
			}
			if ("2.0".Equals(extensionData.AppStatus, StringComparison.OrdinalIgnoreCase) || "2.1".Equals(extensionData.AppStatus, StringComparison.OrdinalIgnoreCase))
			{
				return ExtensionData.GetClientExtensionMyAppsUrl(lcid, httpRequest, withinReadWriteMailboxRole, deploymentId, extensionData.MarketplaceContentMarket, extensionData.MarketplaceAssetID, null);
			}
			return GetExtensibilityContext.GetMarketPlaceEndNodeUrl(extensionData, httpRequest, lcid, withinReadWriteMailboxRole, deploymentId);
		}

		private static OrganizationId GetOrganizationId(CallContext callContext)
		{
			if (callContext.MailboxIdentityPrincipal == null)
			{
				return OrganizationId.ForestWideOrgId;
			}
			return callContext.MailboxIdentityPrincipal.MailboxInfo.OrganizationId;
		}

		private static List<ExtensionData> GetUserExtensions(CallContext callContext, bool isUserScope, bool shouldReturnEnabledOnly, bool shouldFailOnGetOrgExtensionsTimeout, ExtensionsCache extensionsCache, HashSet<string> formattedRequestedExtensionIds, bool retrieveOnly1_0 = false)
		{
			OrganizationId organizationId = GetExtensibilityContext.GetOrganizationId(callContext);
			Organization organization = OrganizationCache.Singleton.Get(organizationId);
			if (!shouldReturnEnabledOnly || organization.AppsForOfficeEnabled)
			{
				string text;
				return GetExtensibilityContext.GetExtensions(callContext, isUserScope, shouldReturnEnabledOnly, shouldFailOnGetOrgExtensionsTimeout, null, extensionsCache, formattedRequestedExtensionIds, false, out text, retrieveOnly1_0, true);
			}
			return null;
		}

		private static List<ExtensionData> GetExtensions(CallContext callContext, bool isUserScope, bool shouldReturnEnabledOnly, bool shouldFailOnGetOrgExtensionsTimeout, OrgEmptyMasterTableCache orgEmptyMasterTableCache, ExtensionsCache extensionsCache, HashSet<string> formattedRequestedExtensionIds, bool isRawXmlRequired, out string masterTableRawXml, bool retrieveOnly1_0 = false, bool filterOutDuplicateMasterTableExtensions = true)
		{
			List<ExtensionData> result = null;
			Dictionary<string, string> dictionary = null;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			MailboxSession mailboxIdentityMailboxSession = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(null, isUserScope, orgEmptyMasterTableCache, extensionsCache, mailboxIdentityMailboxSession, retrieveOnly1_0))
			{
				masterTableRawXml = (isRawXmlRequired ? installedExtensionTable.MasterTableXml.InnerXml : string.Empty);
				result = installedExtensionTable.GetExtensions(formattedRequestedExtensionIds, shouldReturnEnabledOnly, shouldFailOnGetOrgExtensionsTimeout, isRawXmlRequired, out masterTableRawXml, filterOutDuplicateMasterTableExtensions);
				dictionary = installedExtensionTable.RequestData;
			}
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				GetExtensionsMetadata getExtensionsMetadata = GetExtensibilityContext.RequestDataKeyToGetExtensionsMetaDataDictionary[keyValuePair.Key];
				callContext.ProtocolLog.Set(getExtensionsMetadata, keyValuePair.Value);
			}
			stopwatch.Stop();
			callContext.ProtocolLog.Set(GetExtensionsMetadata.GetExtensionsTotalTime, stopwatch.ElapsedMilliseconds);
			return result;
		}

		protected override ExtensibilityContext InternalExecute()
		{
			List<Extension> list = new List<Extension>();
			Stopwatch stopwatch = new Stopwatch();
			long num = 0L;
			string marketplaceUrl = null;
			List<ExtensionData> userExtensionDataListWithUpdateCheck = GetExtensibilityContext.GetUserExtensionDataListWithUpdateCheck(base.CallContext, false, false);
			stopwatch.Start();
			string deploymentId = GetExtensibilityContext.GetDeploymentId(base.CallContext);
			bool flag = false;
			bool flag2 = false;
			if (GetExtensibilityContext.IsNotRunningDfpowa)
			{
				ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(base.CallContext.EffectiveCaller, null, false);
				flag = exchangeRunspaceConfiguration.HasRoleOfType(RoleType.MyReadWriteMailboxApps);
				flag2 = exchangeRunspaceConfiguration.HasRoleOfType(RoleType.MyMarketplaceApps);
			}
			if (userExtensionDataListWithUpdateCheck != null)
			{
				int lcid = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession().Culture.LCID;
				foreach (ExtensionData extensionData in userExtensionDataListWithUpdateCheck)
				{
					Extension extension;
					if (extensionData.Enabled && GetExtensibilityContext.TryCreateExtension(this.request, extensionData, out extension))
					{
						extension.Settings = this.LoadSettings(extension.Id, extension.Version);
						if (!string.IsNullOrWhiteSpace(extensionData.AppStatus))
						{
							extension.EndNodeUrl = GetExtensibilityContext.GetErrorUXActionLinkUrl(extensionData, base.CallContext.HttpContext.Request, lcid, flag, deploymentId);
						}
						else
						{
							extension.EndNodeUrl = GetExtensibilityContext.GetMarketPlaceEndNodeUrl(extensionData, base.CallContext.HttpContext.Request, lcid, flag, deploymentId);
						}
						list.Add(extension);
					}
				}
				num = stopwatch.ElapsedMilliseconds;
			}
			if (flag2 && (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["OfficeStoreUnavailable"]) || StringComparer.OrdinalIgnoreCase.Equals("false", ConfigurationManager.AppSettings["OfficeStoreUnavailable"])))
			{
				marketplaceUrl = ExtensionData.GetClientExtensionMarketplaceUrl(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession().Culture.LCID, base.CallContext.HttpContext.Request, flag, deploymentId, null);
			}
			string ewsUrl = EwsHelper.DiscoverExternalEwsUrl(base.CallContext.AccessingPrincipal);
			stopwatch.Stop();
			base.CallContext.ProtocolLog.Set(GetExtensionsMetadata.CreateExtensionsTime, num);
			base.CallContext.ProtocolLog.Set(GetExtensionsMetadata.GetMarketplaceUrlTime, stopwatch.ElapsedMilliseconds - num);
			return new ExtensibilityContext(list.ToArray(), marketplaceUrl, ewsUrl, flag);
		}

		private string LoadSettings(string extensionId, string extensionVersion)
		{
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			string result;
			using (UserConfiguration folderConfiguration = UserConfigurationHelper.GetFolderConfiguration(mailboxIdentityMailboxSession, ExtensionPackageManager.GetExtensionFolderId(mailboxIdentityMailboxSession), ExtensionPackageManager.GetFaiName(extensionId, extensionVersion), UserConfigurationTypes.Dictionary, true, true))
			{
				IDictionary dictionary = folderConfiguration.GetDictionary();
				string text = dictionary["ExtensionSettings"] as string;
				if (text == null || text.Length > 32768)
				{
					text = "{}";
				}
				result = text;
			}
			return result;
		}

		private static readonly Version InstalledServerVersion = new Version(ServerVersion.InstalledVersion.Major, ServerVersion.InstalledVersion.Minor);

		private static readonly bool IsNotRunningDfpowa = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IsPreCheckinApp"]) || StringComparer.OrdinalIgnoreCase.Equals("false", ConfigurationManager.AppSettings["IsPreCheckinApp"]);

		private static readonly Dictionary<string, GetExtensionsMetadata> RequestDataKeyToGetExtensionsMetaDataDictionary = new Dictionary<string, GetExtensionsMetadata>
		{
			{
				"GE",
				GetExtensionsMetadata.GetExtensionsTime
			},
			{
				"GM",
				GetExtensionsMetadata.GetMasterTableTime
			},
			{
				"GP",
				GetExtensionsMetadata.GetProvidedExtensionsTime
			},
			{
				"AM",
				GetExtensionsMetadata.AddMasterTableTime
			},
			{
				"CU",
				GetExtensionsMetadata.CheckUpdatesTime
			},
			{
				"SU",
				GetExtensionsMetadata.SaveMasterTableTime
			},
			{
				"OrgHost",
				GetExtensionsMetadata.OrgMailboxEwsUrlHost
			},
			{
				"EWSReqId",
				GetExtensionsMetadata.OrgMailboxEwsRequestId
			},
			{
				"GO",
				GetExtensionsMetadata.GetOrgExtensionsTime
			},
			{
				"GET",
				GetExtensionsMetadata.GetExtensionsTotalTime
			},
			{
				"CES",
				GetExtensionsMetadata.CreateExchangeServiceTime
			},
			{
				"GCE",
				GetExtensionsMetadata.GetClientExtensionTime
			},
			{
				"OAD",
				GetExtensionsMetadata.OrgMailboxAdUserLookupTime
			},
			{
				"WSUrl",
				GetExtensionsMetadata.WebServiceUrlLookupTime
			}
		};

		private GetExtensibilityContextParameters request;
	}
}
