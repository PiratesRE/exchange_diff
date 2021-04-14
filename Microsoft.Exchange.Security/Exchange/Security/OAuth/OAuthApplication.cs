using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class OAuthApplication
	{
		public OAuthApplication(PartnerApplication partnerApplication)
		{
			OAuthCommon.VerifyNonNullArgument("partnerApplication", partnerApplication);
			this.PartnerApplication = partnerApplication;
			this.ApplicationType = OAuthApplicationType.S2SApp;
		}

		public OAuthApplication(OfficeExtensionInfo officeExtensionInfo)
		{
			OAuthCommon.VerifyNonNullArgument("officeExtensionInfo", officeExtensionInfo);
			this.OfficeExtension = officeExtensionInfo;
			this.ApplicationType = OAuthApplicationType.CallbackApp;
		}

		public OAuthApplication(V1ProfileAppInfo v1ProfileAppInfo)
		{
			OAuthCommon.VerifyNonNullArgument("v1ProfileAppInfo", v1ProfileAppInfo);
			this.V1ProfileApp = v1ProfileAppInfo;
			this.ApplicationType = OAuthApplicationType.V1App;
		}

		public OAuthApplication(V1ProfileAppInfo v1ProfileApp, PartnerApplication partnerApplication)
		{
			OAuthCommon.VerifyNonNullArgument("v1ProfileApp", v1ProfileApp);
			OAuthCommon.VerifyNonNullArgument("partnerApplication", partnerApplication);
			this.PartnerApplication = partnerApplication;
			this.V1ProfileApp = v1ProfileApp;
			this.ApplicationType = OAuthApplicationType.V1ExchangeSelfIssuedApp;
		}

		public OAuthApplication()
		{
		}

		public PartnerApplication PartnerApplication { get; private set; }

		public OfficeExtensionInfo OfficeExtension { get; private set; }

		public V1ProfileAppInfo V1ProfileApp { get; private set; }

		public OAuthApplicationType ApplicationType { get; private set; }

		public bool IsOfficeExtension
		{
			get
			{
				return this.OfficeExtension != null;
			}
		}

		public bool? IsFromSameOrgExchange
		{
			get
			{
				return this.isFromSameOrgExchange;
			}
			set
			{
				this.isFromSameOrgExchange = value;
			}
		}

		public string Id
		{
			get
			{
				string result = "<unknown>";
				switch (this.ApplicationType)
				{
				case OAuthApplicationType.S2SApp:
					result = string.Format("S2S~{0}", this.PartnerApplication.ApplicationIdentifier);
					break;
				case OAuthApplicationType.CallbackApp:
					result = string.Format("CLB~{0}", this.OfficeExtension.ExtensionId);
					break;
				case OAuthApplicationType.V1App:
					result = string.Format("V1A~{0}", this.V1ProfileApp.AppId);
					break;
				case OAuthApplicationType.V1ExchangeSelfIssuedApp:
					result = string.Format("V1S~{0}", this.V1ProfileApp.AppId);
					break;
				}
				return result;
			}
		}

		public void AddExtensionDataToCommonAccessToken(CommonAccessToken token)
		{
			token.ExtensionData["AppType"] = this.ApplicationType.ToString();
			switch (this.ApplicationType)
			{
			case OAuthApplicationType.S2SApp:
				token.ExtensionData["AppDn"] = this.PartnerApplication.DistinguishedName;
				token.ExtensionData["AppId"] = this.PartnerApplication.ApplicationIdentifier;
				token.ExtensionData["AppRealm"] = this.PartnerApplication.Realm;
				if (this.IsFromSameOrgExchange != null && this.IsFromSameOrgExchange.Value)
				{
					token.ExtensionData["IsFromSameOrgExchange"] = bool.TrueString;
					return;
				}
				break;
			case OAuthApplicationType.CallbackApp:
				token.ExtensionData["CallbackAppId"] = this.OfficeExtension.ExtensionId;
				if (this.OfficeExtension.IsScopedToken)
				{
					token.ExtensionData["Scope"] = this.OfficeExtension.Scope;
					return;
				}
				break;
			case OAuthApplicationType.V1App:
				token.ExtensionData["V1AppId"] = this.V1ProfileApp.AppId;
				token.ExtensionData["Scope"] = this.V1ProfileApp.Scope;
				token.ExtensionData["Role"] = this.V1ProfileApp.Role;
				return;
			case OAuthApplicationType.V1ExchangeSelfIssuedApp:
				token.ExtensionData["V1AppId"] = this.V1ProfileApp.AppId;
				token.ExtensionData["Scope"] = this.V1ProfileApp.Scope;
				token.ExtensionData["Role"] = this.V1ProfileApp.Role;
				token.ExtensionData["AppDn"] = this.PartnerApplication.DistinguishedName;
				token.ExtensionData["AppId"] = this.PartnerApplication.ApplicationIdentifier;
				token.ExtensionData["AppRealm"] = this.PartnerApplication.Realm;
				break;
			default:
				return;
			}
		}

		private bool? isFromSameOrgExchange = null;
	}
}
