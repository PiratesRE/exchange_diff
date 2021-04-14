using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Configuration;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.Management.Extension;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ExtensionUtility
	{
		public static string UserInstallScope
		{
			get
			{
				if (!RbacPrincipal.Current.RbacConfiguration.HasRoleOfType(RoleType.MyReadWriteMailboxApps))
				{
					return "1";
				}
				return "3";
			}
		}

		public static string OrganizationInstallScope
		{
			get
			{
				return "2";
			}
		}

		public static string Application
		{
			get
			{
				return "outlook.exe";
			}
		}

		public static string Version
		{
			get
			{
				return "15";
			}
		}

		public static string DefaultInputForQueryString
		{
			get
			{
				return "0";
			}
		}

		public static string ClickContext
		{
			get
			{
				return "4";
			}
		}

		public static string HomePageTargetCode
		{
			get
			{
				return "HP";
			}
		}

		public static string EndNodeTargetCode
		{
			get
			{
				return "WA";
			}
		}

		public static string ReviewsTargetCode
		{
			get
			{
				return "RV";
			}
		}

		public static string Clid
		{
			get
			{
				return Util.GetLCIDInDecimal();
			}
		}

		public static string ClientFullVersion
		{
			get
			{
				return ExtensionData.ClientFullVersion;
			}
		}

		public static string MarketplaceLandingPageUrl
		{
			get
			{
				return ExtensionData.LandingPageUrl;
			}
		}

		public static string MyAppsPageUrl
		{
			get
			{
				return ExtensionData.MyAppsPageUrl;
			}
		}

		public static string MarketplaceServicesUrl
		{
			get
			{
				return ExtensionData.ConfigServiceUrl;
			}
		}

		public static string OfficeCallBackUrl
		{
			get
			{
				string url = EcpFeature.InstallExtensionCallBack.GetFeatureDescriptor().AbsoluteUrl.ToEscapedString();
				return ExtensionData.AppendEncodedQueryParameterForEcpCallback(url, "DeployId", ExtensionUtility.DeploymentId);
			}
		}

		public static string OfficeCallBackUrlForOrg
		{
			get
			{
				string url = EcpFeature.OrgInstallExtensionCallBack.GetFeatureDescriptor().AbsoluteUrl.ToEscapedString();
				return ExtensionData.AppendEncodedQueryParameterForEcpCallback(url, "DeployId", ExtensionUtility.DeploymentId);
			}
		}

		public static string UrlEncodedOfficeCallBackUrl
		{
			get
			{
				return HttpUtility.UrlEncode(ExtensionUtility.OfficeCallBackUrl);
			}
		}

		public static string DeploymentId
		{
			get
			{
				ExchangeRunspaceConfiguration rbacConfiguration = RbacPrincipal.Current.RbacConfiguration;
				string domain = rbacConfiguration.ExecutingUserPrimarySmtpAddress.IsValidAddress ? rbacConfiguration.ExecutingUserPrimarySmtpAddress.Domain : string.Empty;
				return ExtensionDataHelper.GetDeploymentId(domain);
			}
		}

		public static string UrlEncodedOfficeCallBackUrlForOrg
		{
			get
			{
				return HttpUtility.UrlEncode(ExtensionUtility.OfficeCallBackUrlForOrg);
			}
		}

		public static string GetRequirementsValueString(object requirements, bool isOrgScope)
		{
			string result = string.Empty;
			RequestedCapabilities valueOrDefault = ((RequestedCapabilities?)requirements).GetValueOrDefault();
			RequestedCapabilities? requestedCapabilities;
			if (requestedCapabilities != null)
			{
				switch (valueOrDefault)
				{
				case RequestedCapabilities.Restricted:
					result = (isOrgScope ? Strings.RequirementsRestrictedValue : OwaOptionStrings.RequirementsRestrictedValue);
					break;
				case RequestedCapabilities.ReadItem:
					result = (isOrgScope ? Strings.RequirementsReadItemValue : OwaOptionStrings.RequirementsReadItemValue);
					break;
				case RequestedCapabilities.ReadWriteMailbox:
					result = (isOrgScope ? Strings.RequirementsReadWriteMailboxValue : OwaOptionStrings.RequirementsReadWriteMailboxValue);
					break;
				case RequestedCapabilities.ReadWriteItem:
					result = (isOrgScope ? Strings.RequirementsReadWriteItemValue : OwaOptionStrings.RequirementsReadWriteItemValue);
					break;
				}
			}
			return result;
		}

		public static string GetRequirementsDescriptionString(object requirements, bool isOrgScope)
		{
			string result = string.Empty;
			RequestedCapabilities valueOrDefault = ((RequestedCapabilities?)requirements).GetValueOrDefault();
			RequestedCapabilities? requestedCapabilities;
			if (requestedCapabilities != null)
			{
				switch (valueOrDefault)
				{
				case RequestedCapabilities.Restricted:
					result = (isOrgScope ? Strings.RequirementsRestrictedDescription : OwaOptionStrings.RequirementsRestrictedDescription);
					break;
				case RequestedCapabilities.ReadItem:
					result = (isOrgScope ? Strings.RequirementsReadItemDescription : OwaOptionStrings.RequirementsReadItemDescription);
					break;
				case RequestedCapabilities.ReadWriteMailbox:
					result = (isOrgScope ? Strings.RequirementsReadWriteMailboxDescription : OwaOptionStrings.RequirementsReadWriteMailboxDescription);
					break;
				case RequestedCapabilities.ReadWriteItem:
					result = (isOrgScope ? Strings.RequirementsReadWriteItemDescription : OwaOptionStrings.RequirementsReadWriteItemDescription);
					break;
				}
			}
			return result;
		}

		internal static void ExtensionGetPostAction(bool isOrgScope, DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				ExtensionType extensionType = (ExtensionType)dataRow["Type"];
				if (ExtensionType.MarketPlace == extensionType)
				{
					dataRow["IsMarketplaceExtension"] = true;
					string text = string.Format("{0}?app={1}&ver={2}&clid={3}&p1={4}&p2={5}&p3={6}&p4={7}&p5={8}&Scope={9}&CallBackURL={10}&DeployId={11}", new object[]
					{
						ExtensionUtility.MarketplaceLandingPageUrl,
						ExtensionUtility.Application,
						ExtensionUtility.Version,
						Util.GetLCIDInDecimal(),
						ExtensionUtility.ClientFullVersion,
						ExtensionUtility.ClickContext,
						ExtensionUtility.DefaultInputForQueryString,
						ExtensionUtility.EndNodeTargetCode,
						dataRow["MarketplaceContentmarket"] + "\\" + dataRow["MarketplaceAssetID"],
						isOrgScope ? ExtensionUtility.OrganizationInstallScope : ExtensionUtility.UserInstallScope,
						isOrgScope ? ExtensionUtility.UrlEncodedOfficeCallBackUrlForOrg : ExtensionUtility.UrlEncodedOfficeCallBackUrl,
						ExtensionUtility.DeploymentId
					});
					string myAppsPageLink = string.Format("{0}?app={1}&ver={2}&clid={3}&p1={4}&p2={5}&p3={6}&p4={7}&p5={8}&Scope={9}&CallBackURL={10}&DeployId={11}", new object[]
					{
						ExtensionUtility.MyAppsPageUrl,
						ExtensionUtility.Application,
						ExtensionUtility.Version,
						Util.GetLCIDInDecimal(),
						ExtensionUtility.ClientFullVersion,
						ExtensionUtility.ClickContext,
						ExtensionUtility.DefaultInputForQueryString,
						ExtensionUtility.EndNodeTargetCode,
						dataRow["MarketplaceContentmarket"] + "\\" + dataRow["MarketplaceAssetID"],
						isOrgScope ? ExtensionUtility.OrganizationInstallScope : ExtensionUtility.UserInstallScope,
						isOrgScope ? ExtensionUtility.UrlEncodedOfficeCallBackUrlForOrg : ExtensionUtility.UrlEncodedOfficeCallBackUrl,
						ExtensionUtility.DeploymentId
					});
					string directCallbackLink = string.Format("{0}&Scope={1}&lc={2}&clientToken={3}&AssetId={4}", new object[]
					{
						isOrgScope ? ExtensionUtility.OfficeCallBackUrlForOrg : ExtensionUtility.OfficeCallBackUrl,
						isOrgScope ? ExtensionUtility.OrganizationInstallScope : ExtensionUtility.UserInstallScope,
						dataRow["MarketplaceContentmarket"],
						dataRow["Etoken"],
						dataRow["MarketplaceAssetID"]
					});
					dataRow["DetailsUrl"] = text;
					dataRow["ReviewUrl"] = string.Format("{0}?app={1}&ver={2}&clid={3}&p1={4}&p2={5}&p3={6}&p4={7}&p5={8}&Scope={9}&CallBackURL={10}&DeployId={11}", new object[]
					{
						ExtensionUtility.MarketplaceLandingPageUrl,
						ExtensionUtility.Application,
						ExtensionUtility.Version,
						Util.GetLCIDInDecimal(),
						ExtensionUtility.ClientFullVersion,
						ExtensionUtility.ClickContext,
						ExtensionUtility.DefaultInputForQueryString,
						ExtensionUtility.ReviewsTargetCode,
						dataRow["MarketplaceContentMarket"] + "\\" + dataRow["MarketplaceAssetID"],
						isOrgScope ? ExtensionUtility.OrganizationInstallScope : ExtensionUtility.UserInstallScope,
						isOrgScope ? ExtensionUtility.UrlEncodedOfficeCallBackUrlForOrg : ExtensionUtility.UrlEncodedOfficeCallBackUrl,
						ExtensionUtility.DeploymentId
					});
					string text2 = (dataRow["AppStatus"] != null && !DBNull.Value.Equals(dataRow["AppStatus"])) ? ((string)dataRow["AppStatus"]) : string.Empty;
					if (!string.IsNullOrWhiteSpace(text2))
					{
						dataRow["ShowNotification"] = true;
						ExtensionUtility.SetErrorDescriptionAndNotificationLink(text2, dataRow, text, directCallbackLink, myAppsPageLink);
					}
					string value = (dataRow["LicenseType"] != null && !DBNull.Value.Equals(dataRow["LicenseType"])) ? ((string)dataRow["LicenseType"]) : string.Empty;
					if (Microsoft.Exchange.Management.Extension.LicenseType.Trial.ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						dataRow["ShowTrialReminder"] = true;
						dataRow["TrialReminderActionLinkUrl"] = text;
						dataRow["ShowTrialReminderActionLink"] = true;
					}
				}
				ExtensionInstallScope extensionInstallScope = (ExtensionInstallScope)dataRow["Scope"];
				if (isOrgScope)
				{
					dataRow["ExtensionCanBeUninstalled"] = (extensionInstallScope == ExtensionInstallScope.Organization);
					dataRow["ShowEnableDisable"] = true;
				}
				else
				{
					dataRow["ExtensionCanBeUninstalled"] = (extensionInstallScope == ExtensionInstallScope.User);
					DefaultStateForUser? defaultStateForUser = dataRow["DefaultStateForUser"] as DefaultStateForUser?;
					if (defaultStateForUser != null && defaultStateForUser.Value == DefaultStateForUser.AlwaysEnabled)
					{
						dataRow["ExtensionCanNotBeUninstalledMessage"] = OwaOptionStrings.ExtensionCanNotBeDisabledNorUninstalled;
						dataRow["ShowEnableDisable"] = false;
					}
					else
					{
						dataRow["ExtensionCanNotBeUninstalledMessage"] = OwaOptionStrings.ExtensionCanNotBeUninstalled;
						dataRow["ShowEnableDisable"] = true;
					}
					if (extensionInstallScope == ExtensionInstallScope.Organization)
					{
						dataRow["ShowNotificationLink"] = false;
						dataRow["ShowTrialReminderActionLink"] = false;
					}
				}
				if (dataRow["IconURL"].IsNullValue())
				{
					string themeResource = ThemeResource.GetThemeResource(ExtensionUtility.pagesSection.Theme, "extensiondefaulticon.png");
					dataRow["IconURL"] = new Uri(themeResource);
				}
			}
		}

		internal static void SetErrorDescriptionAndNotificationLink(string errorCode, DataRow row, string appPageLink, string directCallbackLink, string myAppsPageLink)
		{
			switch (errorCode)
			{
			case "1.0":
				row["NotificationText"] = Strings.AppErrorCode1_0;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickToUpdateAppText;
				row["NotificationLinkUrl"] = directCallbackLink;
				return;
			case "1.1":
				row["NotificationText"] = Strings.AppErrorCode1_1;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickToUpdateAppText;
				row["NotificationLinkUrl"] = directCallbackLink;
				return;
			case "1.2":
				row["NotificationText"] = Strings.AppErrorCode1_2;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickToUpdateAppText;
				row["NotificationLinkUrl"] = appPageLink;
				return;
			case "2.0":
				row["NotificationText"] = Strings.AppErrorCode2_0;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickToUpdateAppLincenseText;
				row["NotificationLinkUrl"] = myAppsPageLink;
				return;
			case "2.1":
				row["NotificationText"] = Strings.AppErrorCode2_1;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickToUpdateAppLincenseText;
				row["NotificationLinkUrl"] = myAppsPageLink;
				return;
			case "3.0":
				row["NotificationText"] = Strings.AppErrorCode3_0;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickForMoreAppDetailsText;
				row["NotificationLinkUrl"] = appPageLink;
				return;
			case "3.1":
				row["NotificationText"] = Strings.AppErrorCode3_1;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickForMoreAppDetailsText;
				row["NotificationLinkUrl"] = appPageLink;
				return;
			case "3.2":
				row["NotificationText"] = Strings.AppErrorCode3_2;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickForMoreAppDetailsText;
				row["NotificationLinkUrl"] = appPageLink;
				return;
			case "3.3":
				row["NotificationText"] = Strings.AppErrorCode3_3;
				row["ShowNotificationLink"] = true;
				row["NotificationLinkText"] = Strings.ClickForMoreAppDetailsText;
				row["NotificationLinkUrl"] = appPageLink;
				return;
			case "4.0":
				row["NotificationText"] = Strings.AppErrorCode4_0;
				row["ShowNotificationLink"] = false;
				return;
			case "4.1":
				row["NotificationText"] = Strings.AppErrorCode4_1;
				row["ShowNotificationLink"] = false;
				break;

				return;
			}
		}

		private static readonly PagesSection pagesSection = (PagesSection)ConfigurationManager.GetSection("system.web/pages");
	}
}
