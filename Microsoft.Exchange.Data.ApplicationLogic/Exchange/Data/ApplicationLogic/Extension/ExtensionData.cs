using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class ExtensionData : ICloneable
	{
		public static string ClientFullVersion
		{
			get
			{
				string installedOwaVersion = DefaultExtensionTable.GetInstalledOwaVersion();
				if (installedOwaVersion == null)
				{
					return installedOwaVersion;
				}
				return installedOwaVersion.Replace('.', 'd');
			}
		}

		public static string OfficeCallBackUrl
		{
			get
			{
				return "~/Extension/installFromURL.slab";
			}
		}

		public static string ConfigServiceUrl
		{
			get
			{
				if (ExtensionData.configServiceUrl == null)
				{
					string text = ConfigurationManager.AppSettings["MarketplaceConfigServiceUrl"];
					ExtensionData.configServiceUrl = (string.IsNullOrWhiteSpace(text) ? "https://o15.officeredir.microsoft.com/r/rlidMktplcWSConfig15" : text);
				}
				return ExtensionData.configServiceUrl;
			}
		}

		public static string LandingPageUrl
		{
			get
			{
				if (ExtensionData.landingPageUrl == null)
				{
					string text = ConfigurationManager.AppSettings["MarketplaceLandingPageUrl"];
					ExtensionData.landingPageUrl = (string.IsNullOrWhiteSpace(text) ? "https://o15.officeredir.microsoft.com/r/rlidMktplcExchRedirect" : text);
				}
				return ExtensionData.landingPageUrl;
			}
		}

		public static string MyAppsPageUrl
		{
			get
			{
				if (ExtensionData.myAppsPageUrl == null)
				{
					string text = ConfigurationManager.AppSettings["MarketplaceMyAppsPageUrl"];
					ExtensionData.myAppsPageUrl = (string.IsNullOrWhiteSpace(text) ? "https://o15.officeredir.microsoft.com/r/rlidMktplcMUXMyOfficeApps" : text);
				}
				return ExtensionData.myAppsPageUrl;
			}
		}

		public string MarketplaceContentMarket { get; set; }

		public string MarketplaceAssetID { get; set; }

		public string ProviderName { get; set; }

		public Uri IconURL { get; set; }

		public Uri HighResolutionIconURL
		{
			get
			{
				return this.highResolutionIconURL ?? this.IconURL;
			}
			private set
			{
				this.highResolutionIconURL = value;
			}
		}

		public string ExtensionId { get; set; }

		public string Etoken { get; set; }

		public string AppStatus { get; set; }

		public EntitlementTokenData EtokenData { get; set; }

		public string VersionAsString
		{
			get
			{
				return this.versionAsString;
			}
			set
			{
				this.versionAsString = value;
				if (!ExtensionData.TryParseVersion(this.versionAsString, out this.version))
				{
					ExtensionData.Tracer.TraceError<string>(0L, "ExtensionData.VersionAsString: TryParseVersion failed for: {0}", this.versionAsString);
				}
			}
		}

		public Version Version
		{
			get
			{
				return this.version;
			}
		}

		public ExtensionType? Type { get; set; }

		public ExtensionInstallScope? Scope { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool Enabled { get; set; }

		public DisableReasonType DisableReason { get; set; }

		public string IdentityAndEwsTokenId { get; set; }

		public RequestedCapabilities? RequestedCapabilities { get; set; }

		public SafeXmlDocument Manifest { get; private set; }

		public bool IsAvailable
		{
			get
			{
				return this.Enabled;
			}
			set
			{
				this.Enabled = value;
			}
		}

		public bool IsMandatory { get; set; }

		public bool IsEnabledByDefault { get; set; }

		public ClientExtensionProvidedTo ProvidedTo { get; set; }

		public string[] SpecificUsers { get; set; }

		public Version InstalledByVersion
		{
			get
			{
				return this.installedByVersion ?? ExtensionData.MinimumInstalledByVersion;
			}
			set
			{
				this.installedByVersion = value;
			}
		}

		public XmlNode MasterTableNode { get; set; }

		public Version SchemaVersion
		{
			get
			{
				if (this.SchemaParser == null)
				{
					return null;
				}
				return this.SchemaParser.SchemaVersion;
			}
		}

		public Version MinApiVersion
		{
			get
			{
				return this.minApiVersion ?? SchemaConstants.Exchange2013RtmApiVersion;
			}
			private set
			{
				this.minApiVersion = value;
			}
		}

		private SchemaParser SchemaParser
		{
			get
			{
				if (this.Manifest == null)
				{
					return null;
				}
				ExtensionInstallScope extensionInstallScope = (this.Scope != null) ? this.Scope.Value : ExtensionInstallScope.None;
				if (this.schemaParser == null)
				{
					this.schemaParser = ExtensionDataHelper.GetSchemaParser(this.Manifest, extensionInstallScope);
				}
				else
				{
					this.schemaParser.ExtensionInstallScope = extensionInstallScope;
				}
				return this.schemaParser;
			}
			set
			{
				this.schemaParser = value;
			}
		}

		private ExtensionData(string marketplaceAssetID, string marketplaceContentMarket, string providerName, Uri iconURL, string extensionId, string version, ExtensionType? type, ExtensionInstallScope? scope, string displayName, string description, bool enabled, DisableReasonType disableReason, string identityAndEwsTokenId, RequestedCapabilities? requestedCapabilities, Version installedByVersion, SafeXmlDocument manifest, string appStatus, string etoken = null, Uri highResolutionIconUrl = null, Version minApiVersion = null, SchemaParser schemaParser = null)
		{
			this.MarketplaceAssetID = marketplaceAssetID;
			this.MarketplaceContentMarket = marketplaceContentMarket;
			this.ProviderName = providerName;
			this.IconURL = iconURL;
			this.ExtensionId = extensionId;
			this.VersionAsString = version;
			this.Type = type;
			this.Scope = scope;
			this.DisplayName = displayName;
			this.Description = description;
			this.Enabled = enabled;
			this.DisableReason = disableReason;
			this.AppStatus = appStatus;
			this.IdentityAndEwsTokenId = identityAndEwsTokenId;
			this.RequestedCapabilities = requestedCapabilities;
			this.HighResolutionIconURL = highResolutionIconUrl;
			this.SchemaParser = schemaParser;
			this.MinApiVersion = minApiVersion;
			this.InstalledByVersion = installedByVersion;
			this.Manifest = manifest;
			this.Etoken = etoken;
		}

		public ExtensionData(string marketplaceAssetID, string marketplaceContentMarket, string providerName, Uri iconURL, string extensionId, string version, ExtensionType? type, ExtensionInstallScope? scope, string displayName, string description, bool enabled, DisableReasonType disableReason, string identityAndEwsTokenId, RequestedCapabilities? requestedCapabilities, Version installedByVersion, string manifestString)
		{
			this.MarketplaceAssetID = marketplaceAssetID;
			this.MarketplaceContentMarket = marketplaceContentMarket;
			this.ProviderName = providerName;
			this.IconURL = iconURL;
			this.ExtensionId = extensionId;
			this.VersionAsString = version;
			this.Type = type;
			this.Scope = scope;
			this.DisplayName = displayName;
			this.Description = description;
			this.IdentityAndEwsTokenId = identityAndEwsTokenId;
			this.RequestedCapabilities = requestedCapabilities;
			this.Enabled = enabled;
			this.DisableReason = disableReason;
			this.InstalledByVersion = installedByVersion;
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.PreserveWhitespace = true;
			safeXmlDocument.LoadXml(manifestString);
			this.Manifest = safeXmlDocument;
		}

		public static ExtensionData CreateForXmlStorage(string extensionId, string marketplaceAssetID, string marketplaceContentMarket, ExtensionType? type, ExtensionInstallScope? scope, bool enabled, string version, DisableReasonType disableReason, SafeXmlDocument manifest, string appStatus, string etoken = null)
		{
			return new ExtensionData(marketplaceAssetID, marketplaceContentMarket, null, null, extensionId, version, type, scope, null, null, enabled, disableReason, null, null, null, manifest, appStatus, etoken, null, null, null);
		}

		public static ExtensionData CreateFromClientExtension(ClientExtension clientExtension)
		{
			ExtensionData extensionData = ExtensionData.ParseOsfManifest(clientExtension.ManifestStream, clientExtension.MarketplaceAssetID, clientExtension.MarketplaceContentMarket, clientExtension.Type, clientExtension.Scope, clientExtension.IsAvailable, DisableReasonType.NotDisabled, clientExtension.AppStatus, clientExtension.Etoken);
			extensionData.IsMandatory = clientExtension.IsMandatory;
			extensionData.IsEnabledByDefault = clientExtension.IsEnabledByDefault;
			extensionData.ProvidedTo = clientExtension.ProvidedTo;
			if (clientExtension.SpecificUsers != null)
			{
				extensionData.SpecificUsers = new string[clientExtension.SpecificUsers.Count];
				for (int i = 0; i < clientExtension.SpecificUsers.Count; i++)
				{
					extensionData.SpecificUsers[i] = clientExtension.SpecificUsers[i];
				}
			}
			extensionData.EtokenData = ExtensionData.ParseEtoken(extensionData.Etoken, extensionData.ExtensionId, null, extensionData.MarketplaceAssetID, true, false);
			return extensionData;
		}

		public static EntitlementTokenData ParseEtoken(string etoken, string appId, string domain, string assetId, bool skipVerification, bool isSiteLicenseRequired)
		{
			EntitlementTokenData result = null;
			if (!string.IsNullOrWhiteSpace(etoken))
			{
				XmlNode xmlNode = new SafeXmlDocument
				{
					PreserveWhitespace = true
				}.CreateNode(XmlNodeType.Element, "entitlementToken", null);
				xmlNode.InnerXml = etoken;
				result = ExtensionData.ParseEntitlementTokenData(xmlNode, appId, domain, assetId, skipVerification, isSiteLicenseRequired);
			}
			return result;
		}

		public static ExtensionData ConvertFromMasterTableXml(XmlNode xmlNode, bool isOrgMasterTable, string domain)
		{
			ExtensionData extensionData = null;
			bool boolTagValue = ExtensionData.GetBoolTagValue(xmlNode, "enabled");
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("appstatus");
			string appStatus = (xmlNode2 != null) ? xmlNode2.InnerXml : string.Empty;
			DisableReasonType enumTagValue = ExtensionData.GetEnumTagValue<DisableReasonType>(xmlNode, "disablereason", null);
			Version versionTagValue = ExtensionData.GetVersionTagValue(xmlNode, "installedByVersion");
			XmlNode xmlNode3 = xmlNode.SelectSingleNode("manifest");
			if (xmlNode3 != null)
			{
				ExtensionType enumTagValue2 = ExtensionData.GetEnumTagValue<ExtensionType>(xmlNode, "type", null);
				ExtensionInstallScope enumTagValue3 = ExtensionData.GetEnumTagValue<ExtensionInstallScope>(xmlNode, "scope", null);
				string text = null;
				string marketplaceContentMarket = null;
				if (ExtensionType.MarketPlace == enumTagValue2)
				{
					text = ExtensionData.GetTagStringValue(xmlNode, "marketplaceAssetID", null);
					marketplaceContentMarket = ExtensionData.GetTagStringValue(xmlNode, "marketplaceContentMarket", null);
				}
				SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
				safeXmlDocument.PreserveWhitespace = true;
				safeXmlDocument.InnerXml = xmlNode3.InnerXml;
				XmlNode xmlNode4 = xmlNode.SelectSingleNode("entitlementToken");
				string etoken = (xmlNode4 != null) ? xmlNode4.InnerXml : string.Empty;
				extensionData = ExtensionData.ParseOsfManifest(safeXmlDocument, text, marketplaceContentMarket, enumTagValue2, enumTagValue3, boolTagValue, enumTagValue, versionTagValue, appStatus, etoken);
				extensionData.EtokenData = ExtensionData.ParseEntitlementTokenData(xmlNode4, extensionData.ExtensionId, domain, text, false, isOrgMasterTable);
				if (isOrgMasterTable)
				{
					if (ExtensionInstallScope.Organization != enumTagValue3)
					{
						ExtensionData.Tracer.TraceError<ExtensionInstallScope, string>(0L, "Org's master table has non-org scope '{0}' extension with manifest node, id is: {1}", enumTagValue3, extensionData.ExtensionId);
						throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonOrgMasterTableInvalidScope(enumTagValue3.ToString(), extensionData.ExtensionId)));
					}
				}
				else if (ExtensionInstallScope.User != enumTagValue3)
				{
					ExtensionData.Tracer.TraceError<ExtensionInstallScope, string>(0L, "User's master table has non-user scope '{0}' extension with manifest node, id is: {1}", enumTagValue3, extensionData.ExtensionId);
					throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonUserMasterTableInvalidScope(enumTagValue3.ToString(), extensionData.ExtensionId)));
				}
			}
			else
			{
				extensionData = ExtensionData.CreateForXmlStorage(ExtensionData.GetTagStringValue(xmlNode, "ExtensionId", null), null, null, null, null, boolTagValue, null, enumTagValue, null, null, null);
			}
			if (isOrgMasterTable)
			{
				extensionData.IsMandatory = ExtensionData.GetBoolTagValue(xmlNode, "./isMandatory");
				extensionData.IsEnabledByDefault = ExtensionData.GetBoolTagValue(xmlNode, "./isEnabledByDefault");
				extensionData.ProvidedTo = ExtensionData.GetEnumTagValue<ClientExtensionProvidedTo>(xmlNode, "./providedTo", null);
				using (XmlNodeList xmlNodeList = xmlNode.SelectNodes("./users/user"))
				{
					if (xmlNodeList != null)
					{
						List<string> list = new List<string>(xmlNodeList.Count);
						foreach (object obj in xmlNodeList)
						{
							XmlNode xmlNode5 = (XmlNode)obj;
							list.Add(xmlNode5.InnerText);
						}
						extensionData.SpecificUsers = list.ToArray();
					}
				}
			}
			return extensionData;
		}

		public static ExtensionData ParseOsfManifest(byte[] manifestBytes, int byteCount, string marketplaceAssetID, string marketplaceContentMarket, ExtensionType extensionType, ExtensionInstallScope extensionScope, bool isEnabled, DisableReasonType disableReason, string appStatus, string etoken = null)
		{
			if (manifestBytes == null || manifestBytes.Length == 0)
			{
				throw new ArgumentNullException("manifestBytes");
			}
			ExtensionData result = null;
			using (Stream stream = new MemoryStream(manifestBytes, 0, byteCount))
			{
				result = ExtensionData.ParseOsfManifest(stream, marketplaceAssetID, marketplaceContentMarket, extensionType, extensionScope, isEnabled, disableReason, appStatus, etoken);
			}
			return result;
		}

		public static ExtensionData ParseOsfManifest(Stream manifestStream, string marketplaceAssetID, string marketplaceContentMarket, ExtensionType extensionType, ExtensionInstallScope extensionScope, bool isEnabled, DisableReasonType disableReason, string appStatus, string etoken = null)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.PreserveWhitespace = true;
			SafeXmlDocument xmlDoc = null;
			manifestStream.Position = 0L;
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(manifestStream))
				{
					safeXmlDocument.Load(xmlReader);
				}
				xmlDoc = ExtensionDataHelper.GetManifest(safeXmlDocument);
			}
			catch (InvalidOperationException ex)
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonInvalidXml(ex.Message)));
			}
			catch (XmlException ex2)
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonInvalidXml(ex2.Message)));
			}
			return ExtensionData.ParseOsfManifest(xmlDoc, marketplaceAssetID, marketplaceContentMarket, extensionType, extensionScope, isEnabled, disableReason, null, appStatus, etoken);
		}

		public static string GetClientExtensionMarketplaceUrl(MailboxSession mailboxSession, Uri ecpUrl, bool withinReadWriteMailboxRole, string deploymentId, Version schemaVersionSupported, string realm = null)
		{
			string fullEncodedOfficeCallbackUrl = ExtensionData.GenerateFullEncodedOfficeCallbackUrl(ecpUrl, ExtensionData.OfficeCallBackUrl, realm, deploymentId);
			return ExtensionData.GetClientExtensionMarketplaceUrl(mailboxSession.Culture.LCID, withinReadWriteMailboxRole, fullEncodedOfficeCallbackUrl, deploymentId, schemaVersionSupported);
		}

		public static string GetClientExtensionMarketplaceUrl(int lcid, HttpRequest httpRequest, bool withinReadWriteMailboxRole, string deploymentId, string realm = null)
		{
			Uri ecpUrl = ExtensionData.GetEcpUrl(httpRequest);
			if (ecpUrl == null)
			{
				return null;
			}
			string fullEncodedOfficeCallbackUrl = ExtensionData.GenerateFullEncodedOfficeCallbackUrl(ecpUrl, ExtensionData.OfficeCallBackUrl, realm, deploymentId);
			return ExtensionData.GetClientExtensionMarketplaceUrl(lcid, withinReadWriteMailboxRole, fullEncodedOfficeCallbackUrl, deploymentId, null);
		}

		public static string GetClientExtensionMarketplaceUrl(int lcid, bool withinOrgMarketplaceRole, string fullEncodedOfficeCallbackUrl, string deploymentId, Version schemaVersionSupported = null)
		{
			bool flag = schemaVersionSupported != null && schemaVersionSupported == SchemaConstants.SchemaVersion1_0;
			return string.Format("{0}?app={1}&ver={2}&clid={3}&p1={4}&p2={5}&p3={6}&p4={7}&p5={8}&Scope={9}&CallBackURL={10}&DeployId={11}", new object[]
			{
				ExtensionData.LandingPageUrl,
				"outlook.exe",
				"15",
				lcid,
				flag ? "15d0d516d32" : ExtensionData.ClientFullVersion,
				"4",
				"0",
				"HP",
				"0",
				withinOrgMarketplaceRole ? "3" : "1",
				fullEncodedOfficeCallbackUrl,
				deploymentId
			});
		}

		public static string GetClientExtensionAppDetailsUrl(int lcid, HttpRequest httpRequest, bool withinOrgMarketplaceRole, string deploymentId, string market, string assetId, string realm = null)
		{
			return ExtensionData.GetOmexUrlWithParameters(ExtensionData.LandingPageUrl, lcid, httpRequest, withinOrgMarketplaceRole, deploymentId, market, assetId, realm);
		}

		public static string GetClientExtensionMyAppsUrl(int lcid, HttpRequest httpRequest, bool withinOrgMarketplaceRole, string deploymentId, string market, string assetId, string realm = null)
		{
			return ExtensionData.GetOmexUrlWithParameters(ExtensionData.MyAppsPageUrl, lcid, httpRequest, withinOrgMarketplaceRole, deploymentId, market, assetId, realm);
		}

		internal static bool TryParseVersion(string versionAsString, out Version version)
		{
			version = null;
			if (!string.IsNullOrWhiteSpace(versionAsString))
			{
				if (versionAsString.Length == 1)
				{
					versionAsString += ".0";
				}
				if (!Version.TryParse(versionAsString, out version))
				{
					version = null;
				}
			}
			return version != null;
		}

		internal static bool ValidateManifestSize(long size, bool shouldThrowOnFailure = true)
		{
			if (size <= 262144L)
			{
				return true;
			}
			if (shouldThrowOnFailure)
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonManifestTooLarge(256)));
			}
			return false;
		}

		internal static bool ValidateManifestDownloadSize(long size, bool shouldThrowOnFailure = true)
		{
			if (size <= 393216L)
			{
				return true;
			}
			if (shouldThrowOnFailure)
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonManifestTooLarge(384)));
			}
			return false;
		}

		internal static T GetEnumTagValue<T>(XmlNode xmlNode, string tagName, XmlNamespaceManager mgr) where T : struct
		{
			string tagStringValue = ExtensionData.GetTagStringValue(xmlNode, tagName, mgr);
			T result;
			if (!EnumValidator.TryParse<T>(tagStringValue, EnumParseOptions.IgnoreCase, out result))
			{
				ExtensionData.Tracer.TraceError(0L, tagName + " tag value is invalid: " + tagStringValue);
				throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonTagValueInvalid(tagName, tagStringValue)));
			}
			return result;
		}

		internal static int CompareCapabilities(RequestedCapabilities capabilitiesA, RequestedCapabilities capabilitiesB)
		{
			if (!ExtensionData.IsCapabilitiesKnown(capabilitiesA))
			{
				throw new ArgumentOutOfRangeException("capabilitiesA");
			}
			if (!ExtensionData.IsCapabilitiesKnown(capabilitiesB))
			{
				throw new ArgumentOutOfRangeException("capabilitiesB");
			}
			if (capabilitiesA == capabilitiesB)
			{
				return 0;
			}
			if (capabilitiesA == Microsoft.Exchange.Data.RequestedCapabilities.ReadWriteMailbox)
			{
				return 1;
			}
			if (capabilitiesB == Microsoft.Exchange.Data.RequestedCapabilities.ReadWriteMailbox)
			{
				return -1;
			}
			return capabilitiesA.CompareTo(capabilitiesB);
		}

		private static string GetOmexUrlWithParameters(string targetOmexUrl, int lcid, HttpRequest httpRequest, bool withinReadWriteMailboxRole, string deploymentId, string market, string assetId, string realm = null)
		{
			Uri ecpUrl = ExtensionData.GetEcpUrl(httpRequest);
			if (ecpUrl == null)
			{
				return null;
			}
			string text = ExtensionData.GenerateFullEncodedOfficeCallbackUrl(ecpUrl, ExtensionData.OfficeCallBackUrl, realm, deploymentId);
			return string.Format("{0}?app={1}&ver={2}&clid={3}&p1={4}&p2={5}&p3={6}&p4={7}&p5={8}&Scope={9}&CallBackURL={10}&DeployId={11}", new object[]
			{
				targetOmexUrl,
				"outlook.exe",
				"15",
				lcid,
				ExtensionData.ClientFullVersion,
				"4",
				"0",
				"WA",
				market + "\\" + assetId,
				withinReadWriteMailboxRole ? "3" : "1",
				text,
				deploymentId
			});
		}

		private static Uri GetEcpUrl(HttpRequest httpRequest)
		{
			Uri result;
			try
			{
				string text = httpRequest.Headers["msExchProxyUri"];
				if (string.IsNullOrEmpty(text))
				{
					ExtensionData.Tracer.TraceError(0L, "No request uri to create ecp with, skipping");
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_EcpUriRetrievalFailed, null, new object[]
					{
						"GetEcpUrl"
					});
					result = null;
				}
				else
				{
					result = new UriBuilder(text)
					{
						Path = "/ecp/",
						Query = string.Empty
					}.Uri;
				}
			}
			catch (UriFormatException arg)
			{
				ExtensionData.Tracer.TraceError<UriFormatException>(0L, "Caught exception when trying to access request uri: {0}", arg);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_EcpUriRetrievalFailed, null, new object[]
				{
					"GetEcpUrl"
				});
				result = null;
			}
			return result;
		}

		private static bool IsCapabilitiesKnown(RequestedCapabilities capabilities)
		{
			bool result;
			switch (capabilities)
			{
			case Microsoft.Exchange.Data.RequestedCapabilities.Restricted:
			case Microsoft.Exchange.Data.RequestedCapabilities.ReadItem:
			case Microsoft.Exchange.Data.RequestedCapabilities.ReadWriteMailbox:
			case Microsoft.Exchange.Data.RequestedCapabilities.ReadWriteItem:
				result = true;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		private static ExtensionData ParseOsfManifest(SafeXmlDocument xmlDoc, string marketplaceAssetID, string marketplaceContentMarket, ExtensionType extensionType, ExtensionInstallScope extensionScope, bool isEnabled, DisableReasonType disableReason, Version installedByVersion, string appStatus, string etoken = null)
		{
			if (xmlDoc == null)
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonMissingOfficeApp));
			}
			SchemaParser schemaParser = ExtensionDataHelper.GetSchemaParser(xmlDoc, extensionScope);
			CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
			string andValidateExtensionId = schemaParser.GetAndValidateExtensionId();
			RequestedCapabilities requestedCapabilities = schemaParser.GetRequestedCapabilities();
			schemaParser.ValidateRules();
			Uri andValidateIconUrl = schemaParser.GetAndValidateIconUrl(currentUICulture);
			Uri andValidateHighResolutionIconUrl = schemaParser.GetAndValidateHighResolutionIconUrl(currentUICulture);
			schemaParser.ValidateSourceLocations();
			schemaParser.ValidateHosts();
			schemaParser.ValidateFormSettings();
			Version v = schemaParser.GetMinApiVersion();
			Version schemaVersion = schemaParser.SchemaVersion;
			if (v > SchemaConstants.HighestSupportedApiVersion)
			{
				throw new OwaExtensionOperationException(Strings.ErrorReasonMinApiVersionNotSupported(v, ExchangeSetupContext.InstalledVersion));
			}
			string oweStringElement = schemaParser.GetOweStringElement("ProviderName");
			string oweStringElement2 = schemaParser.GetOweStringElement("Version");
			string idForTokenRequests = schemaParser.GetIdForTokenRequests();
			string oweLocaleAwareSetting = schemaParser.GetOweLocaleAwareSetting("DisplayName", currentUICulture);
			string oweLocaleAwareSetting2 = schemaParser.GetOweLocaleAwareSetting("Description", currentUICulture);
			return new ExtensionData(marketplaceAssetID, marketplaceContentMarket, oweStringElement, andValidateIconUrl, andValidateExtensionId, oweStringElement2, new ExtensionType?(extensionType), new ExtensionInstallScope?(extensionScope), oweLocaleAwareSetting, oweLocaleAwareSetting2, isEnabled, disableReason, idForTokenRequests, new RequestedCapabilities?(requestedCapabilities), installedByVersion, xmlDoc, appStatus, etoken, andValidateHighResolutionIconUrl, v, schemaParser);
		}

		public XmlNode ConvertToXml(bool shouldIncludeManifest, bool shouldIncludeOrgNodes)
		{
			XmlElement xmlElement = this.Manifest.CreateElement("Extension");
			this.AppendXmlElement(xmlElement, "ExtensionId", this.ExtensionId);
			this.AppendXmlElement(xmlElement, "enabled", this.Enabled.ToString());
			this.AppendXmlElement(xmlElement, "disablereason", this.DisableReason.ToString());
			this.AppendXmlElement(xmlElement, "installedByVersion", this.InstalledByVersion.ToString());
			if (shouldIncludeManifest)
			{
				this.AppendXmlElement(xmlElement, "marketplaceAssetID", this.MarketplaceAssetID);
				this.AppendXmlElement(xmlElement, "marketplaceContentMarket", this.MarketplaceContentMarket);
				this.AppendXmlElement(xmlElement, "type", this.Type.ToString());
				this.AppendXmlElement(xmlElement, "scope", this.Scope.ToString());
				this.AppendXmlElement(xmlElement, "manifest", this.Manifest.LastChild.Clone());
				if (!string.IsNullOrWhiteSpace(this.Etoken))
				{
					this.AppendXmlElement(xmlElement, "entitlementToken", this.Etoken);
				}
			}
			if (shouldIncludeOrgNodes)
			{
				this.AppendXmlElement(xmlElement, "isMandatory", this.IsMandatory.ToString());
				this.AppendXmlElement(xmlElement, "isEnabledByDefault", this.IsEnabledByDefault.ToString());
				this.AppendXmlElement(xmlElement, "providedTo", this.ProvidedTo.ToString());
				ExtensionData.AppendXmlElement(this.Manifest, xmlElement, "users", "user", this.SpecificUsers);
			}
			return xmlElement;
		}

		public byte[] GetManifestBytes()
		{
			string outerXml = this.Manifest.DocumentElement.OuterXml;
			return Encoding.UTF8.GetBytes(outerXml);
		}

		public static string GenerateOfficeCallbackUrlForReconsent(HttpRequest httpRequest, string realm, string assetId, string marketplace, ExtensionInstallScope scope, string etoken)
		{
			Uri ecpUrl = ExtensionData.GetEcpUrl(httpRequest);
			string str = ExtensionData.GenerateOfficeCallbackBaseUrl(ecpUrl, ExtensionData.OfficeCallBackUrl);
			return str + string.Format("&realm={0}&scope={1}&lc={2}&clientToken={3}&AssetId={4}", new object[]
			{
				realm,
				scope.ToString(),
				marketplace,
				etoken,
				assetId
			});
		}

		internal List<FormSettings> GetFormSettings(FormFactor formFactor)
		{
			return this.SchemaParser.GetFormSettings(formFactor, CultureInfo.CurrentUICulture, this.Etoken);
		}

		internal bool GetDisableEntityHighlighting()
		{
			return this.SchemaParser.GetDisableEntityHighlighting();
		}

		internal bool TryGetActivationRule(out ActivationRule activationRule)
		{
			return this.SchemaParser.TryCreateActivationRule(out activationRule);
		}

		internal bool TryUpdateSourceLocation(IExchangePrincipal exchangePrincipal, string elementName, out Exception exception, ExtensionDataHelper.TryModifySourceLocationDelegate tryModifySourceLocationDelegate)
		{
			return this.SchemaParser.TryUpdateSourceLocation(exchangePrincipal, elementName, this, out exception, tryModifySourceLocationDelegate);
		}

		internal static bool GetBoolTagValue(XmlNode xmlNode, string tagName)
		{
			string tagStringValue = ExtensionData.GetTagStringValue(xmlNode, tagName, null);
			bool result;
			if (!bool.TryParse(tagStringValue, out result))
			{
				ExtensionData.Tracer.TraceError(0L, tagName + " tag value is invalid: " + tagStringValue);
				throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonTagValueInvalid(tagName, tagStringValue)));
			}
			return result;
		}

		internal static string GetTagStringValue(XmlNode xmlNode, string tagName, XmlNamespaceManager mgr)
		{
			XmlNode xmlNode2 = (mgr == null) ? xmlNode.SelectSingleNode(tagName) : xmlNode.SelectSingleNode(tagName, mgr);
			if (xmlNode2 == null)
			{
				ExtensionData.Tracer.TraceError(0L, tagName + " tag is missing from the given node.");
				throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonTagMissing(tagName)));
			}
			return xmlNode2.InnerText;
		}

		private static Version GetVersionTagValue(XmlNode xmlNode, string tagName)
		{
			XmlNode xmlNode2 = xmlNode.SelectSingleNode(tagName);
			if (xmlNode2 == null)
			{
				ExtensionData.Tracer.TraceWarning(0L, tagName + " tag is missing from the given node. Returning null");
				return null;
			}
			string innerText = xmlNode2.InnerText;
			if (string.IsNullOrEmpty(innerText))
			{
				ExtensionData.Tracer.TraceWarning(0L, tagName + " version string is missing from the given node. Returning null");
				return null;
			}
			Version result = null;
			if (Version.TryParse(innerText, out result))
			{
				return result;
			}
			ExtensionData.Tracer.TraceWarning(0L, tagName + " Failed to parse version, returning null.");
			return null;
		}

		internal static string GetAttributeStringValue(XmlNode xmlNode, string attributeName)
		{
			return ExtensionData.GetAttributeStringValue(xmlNode, attributeName, true);
		}

		internal static string GetOptionalAttributeStringValue(XmlNode xmlNode, string attributeName, string defaultValue)
		{
			string attributeStringValue = ExtensionData.GetAttributeStringValue(xmlNode, attributeName, false);
			if (attributeStringValue != null)
			{
				return attributeStringValue;
			}
			return defaultValue;
		}

		private static string GetAttributeStringValue(XmlNode xmlNode, string attributeName, bool throwOnNull)
		{
			if (xmlNode.Attributes == null)
			{
				if (throwOnNull)
				{
					ExtensionData.Tracer.TraceError(0L, "Given node has no attributes.");
					throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonNoAttributes));
				}
				return null;
			}
			else
			{
				XmlAttribute xmlAttribute = xmlNode.Attributes[attributeName];
				if (xmlAttribute != null)
				{
					return xmlAttribute.Value;
				}
				if (throwOnNull)
				{
					ExtensionData.Tracer.TraceError(0L, attributeName + " attribute is missing from the given node.");
					throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonAttributeMissing(attributeName)));
				}
				return null;
			}
		}

		private static T GetAttributeEnumValue<T>(XmlNode xmlNode, string attributeName) where T : struct
		{
			string attributeStringValue = ExtensionData.GetAttributeStringValue(xmlNode, attributeName);
			T result;
			if (!EnumValidator.TryParse<T>(attributeStringValue, EnumParseOptions.IgnoreCase, out result))
			{
				ExtensionData.Tracer.TraceError(0L, attributeName + " attribute value is invalid: " + attributeStringValue);
				throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonAttributeValueInvalid(attributeName, attributeStringValue)));
			}
			return result;
		}

		private static string GetAuthTokenValue(XmlNode xmlNode, XmlNamespaceManager mgr)
		{
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("SourceLocation", mgr);
			if (xmlNode2 == null)
			{
				ExtensionData.Tracer.TraceError(0L, "SourceLocation tag is missing from the given node.");
				throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonSourceLocationTagMissing));
			}
			return ExtensionData.GetAttributeStringValue(xmlNode2, "DefaultValue");
		}

		internal static void AppendXmlElement(SafeXmlDocument document, XmlNode parent, string newChildName, string innerTagName, string[] innerTagValues)
		{
			if (innerTagValues != null && innerTagValues.Length > 0)
			{
				XmlElement xmlElement = document.CreateElement(newChildName);
				foreach (string innerText in innerTagValues)
				{
					XmlElement xmlElement2 = document.CreateElement(innerTagName);
					xmlElement2.InnerText = innerText;
					xmlElement.AppendChild(xmlElement2);
				}
				parent.AppendChild(xmlElement);
			}
		}

		private void AppendXmlElement(XmlNode parent, string newChildName, XmlNode newChildValue)
		{
			if (newChildValue != null)
			{
				XmlElement xmlElement = this.Manifest.CreateElement(newChildName);
				xmlElement.AppendChild(newChildValue);
				parent.AppendChild(xmlElement);
			}
		}

		private void AppendXmlElement(XmlNode parent, string newChildName, string newChildValue)
		{
			if (newChildValue != null)
			{
				XmlElement xmlElement = this.Manifest.CreateElement(newChildName);
				xmlElement.InnerText = newChildValue;
				parent.AppendChild(xmlElement);
			}
		}

		private static string GenerateFullEncodedOfficeCallbackUrl(Uri ecpUrl, string officeCallBackUrl, string realm, string deploymentId)
		{
			string text = ExtensionData.GenerateOfficeCallbackBaseUrl(ecpUrl, officeCallBackUrl);
			if (!string.IsNullOrWhiteSpace(realm))
			{
				text = ExtensionData.AppendEncodedQueryParameterForEcpCallback(text, "realm", realm);
			}
			if (!string.IsNullOrWhiteSpace(deploymentId))
			{
				text = ExtensionData.AppendEncodedQueryParameterForEcpCallback(text, "deployId", deploymentId);
			}
			return HttpUtility.UrlEncode(text);
		}

		private static string GenerateOfficeCallbackBaseUrl(Uri ecpUrl, string officeCallBackUrl)
		{
			officeCallBackUrl = VirtualPathUtility.ToAbsolute(officeCallBackUrl);
			officeCallBackUrl = Regex.Replace(officeCallBackUrl, "/ews/", string.Empty, RegexOptions.IgnoreCase);
			officeCallBackUrl = Regex.Replace(officeCallBackUrl, "/owa/", string.Empty, RegexOptions.IgnoreCase);
			officeCallBackUrl = ExtensionData.AppendEncodedQueryParameterForEcpCallback(officeCallBackUrl, "exsvurl", "1");
			return new Uri(ecpUrl, officeCallBackUrl).ToString();
		}

		public static string AppendEncodedQueryParameterForEcpCallback(string url, string name, string value)
		{
			StringBuilder stringBuilder = new StringBuilder(url, url.Length + name.Length + value.Length + 4);
			stringBuilder.Append((url.IndexOf('?') >= 0) ? HttpUtility.UrlEncode("&") : "?");
			stringBuilder.Append(HttpUtility.UrlEncode(name));
			stringBuilder.Append('=');
			stringBuilder.Append(HttpUtility.UrlEncode(value));
			return stringBuilder.ToString();
		}

		public static string AppendUnencodedQueryParameter(string url, string name, string value)
		{
			StringBuilder stringBuilder = new StringBuilder(url, url.Length + name.Length + value.Length + 2);
			stringBuilder.Append((url.IndexOf('?') >= 0) ? "&" : "?");
			stringBuilder.Append(name);
			stringBuilder.Append('=');
			stringBuilder.Append(value);
			return stringBuilder.ToString();
		}

		private static EntitlementTokenData ParseEntitlementTokenData(XmlNode etokenNode, string appId, string domain, string assetId, bool skipVerification, bool isSiteLicenseRequired = false)
		{
			string text = (etokenNode != null) ? etokenNode.InnerXml : string.Empty;
			EntitlementTokenData entitlementTokenData = null;
			if (!string.IsNullOrWhiteSpace(text))
			{
				try
				{
					XmlNode xmlNode = etokenNode.Clone();
					xmlNode.InnerXml = HttpUtility.UrlDecode(text);
					XmlNode xmlNode2 = xmlNode.SelectSingleNode("r");
					if (xmlNode2 == null)
					{
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MissingNodeInEtoken, null, new object[]
						{
							"ProcessEntitlementToken",
							"r",
							appId
						});
						throw new OwaExtensionOperationException(Strings.ErrorMissingNodeInEtoken("r"));
					}
					XmlNode xmlNode3 = xmlNode2.SelectSingleNode("t");
					if (xmlNode3 == null)
					{
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MissingNodeInEtoken, null, new object[]
						{
							"ProcessEntitlementToken",
							"t",
							appId
						});
						throw new OwaExtensionOperationException(Strings.ErrorMissingNodeInEtoken("t"));
					}
					string attributeStringValue = ExtensionData.GetAttributeStringValue(xmlNode3, "cid");
					LicenseType attributeEnumValue = ExtensionData.GetAttributeEnumValue<LicenseType>(xmlNode3, "et");
					DateTime etokenExpirationDate = DateTime.Parse(ExtensionData.GetAttributeStringValue(xmlNode3, "te"));
					int seatsPurchased = Convert.ToInt32(ExtensionData.GetAttributeStringValue(xmlNode3, "ts"));
					string attributeStringValue2 = ExtensionData.GetAttributeStringValue(xmlNode3, "did");
					string attributeStringValue3 = ExtensionData.GetAttributeStringValue(xmlNode3, "aid");
					if (isSiteLicenseRequired && attributeEnumValue == LicenseType.Paid)
					{
						string optionalAttributeStringValue = ExtensionData.GetOptionalAttributeStringValue(xmlNode3, "sl", null);
						bool flag = false;
						bool flag2 = bool.TryParse(optionalAttributeStringValue, out flag);
						if (!flag2)
						{
							ExtensionData.Tracer.TraceError(0L, "sl tag value is invalid: " + optionalAttributeStringValue);
						}
						if (!flag2 || !flag)
						{
							ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_OrgLevelEtokenMustBeSiteLicense, null, new object[]
							{
								"ProcessEntitlementToken",
								optionalAttributeStringValue,
								appId
							});
							throw new OwaExtensionOperationException(Strings.ErrorOrgLevelAppMustBeSiteLicense);
						}
					}
					if (string.IsNullOrWhiteSpace(assetId) || !assetId.Equals(attributeStringValue3, StringComparison.OrdinalIgnoreCase))
					{
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_AssetIdNotMatchInEtoken, null, new object[]
						{
							"ProcessEntitlementToken",
							assetId,
							attributeStringValue3
						});
						throw new OwaExtensionOperationException(Strings.ErrorAssetIdNotMatchInEtoken(assetId, attributeStringValue3));
					}
					if (!skipVerification && !ExtensionDataHelper.VerifyDeploymentId(attributeStringValue2, domain))
					{
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_InvalidDeploymentIdInEtoken, null, new object[]
						{
							"ProcessEntitlementToken",
							attributeStringValue2,
							appId
						});
						throw new OwaExtensionOperationException(Strings.ErrorEtokenWithInvalidDeploymentId(attributeStringValue2));
					}
					entitlementTokenData = new EntitlementTokenData(attributeStringValue, attributeEnumValue, seatsPurchased, etokenExpirationDate);
				}
				catch (FormatException innerException)
				{
					throw new OwaExtensionOperationException(innerException);
				}
				catch (OverflowException innerException2)
				{
					throw new OwaExtensionOperationException(innerException2);
				}
				catch (ArgumentNullException innerException3)
				{
					throw new OwaExtensionOperationException(innerException3);
				}
				finally
				{
					if (entitlementTokenData == null)
					{
						ExtensionData.Tracer.TraceError<string>(0L, "Failed to parse the stored etoken for app {0} since it is corrupted.", appId);
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_StoredEtokenCorrupted, null, new object[]
						{
							"ProcessEntitlementToken",
							appId
						});
					}
					else
					{
						ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_ParseEtokenSuccess, null, new object[]
						{
							"ProcessEntitlementToken"
						});
					}
				}
			}
			return entitlementTokenData;
		}

		public object Clone()
		{
			ExtensionData extensionData = ExtensionData.ParseOsfManifest(this.Manifest, this.MarketplaceAssetID, this.MarketplaceContentMarket, this.Type.Value, this.Scope.Value, this.Enabled, this.DisableReason, this.InstalledByVersion, this.AppStatus, this.Etoken);
			extensionData.IsMandatory = this.IsMandatory;
			extensionData.IsEnabledByDefault = this.IsEnabledByDefault;
			extensionData.ProvidedTo = this.ProvidedTo;
			if (this.SpecificUsers != null)
			{
				extensionData.SpecificUsers = (this.SpecificUsers.Clone() as string[]);
			}
			return extensionData;
		}

		public const string Application = "outlook.exe";

		public const string ApplicationVersion = "15";

		public const string RTMClientVersion = "15d0d516d32";

		public const string DefaultInputForQueryString = "0";

		public const string ClickContext = "4";

		public const string HomePageTargetCode = "HP";

		public const string EndNodeTargetCode = "WA";

		public const string OmexUserScope = "1";

		public const string OmexOrganizationScope = "2";

		public const string OmexUserWithinReadWriteMailboxRoleScope = "3";

		private const int BytesInKB = 1024;

		internal const int MaxManifestSizeInKB = 256;

		internal const int MaxManifestSize = 262144;

		internal const int MaxManifestDownloadSizeInKB = 384;

		internal const int MaxManifestDownloadSize = 393216;

		internal const int MaxTokenDownloadSizeInKB = 30;

		internal const int MaxTokenDownloadSize = 30720;

		private const string MarketplaceAssetIDTagName = "marketplaceAssetID";

		private const string MarketplaceContentMarketTagName = "marketplaceContentMarket";

		internal const string ExtensionIdTagName = "ExtensionId";

		internal const string IsMandatoryTagName = "isMandatory";

		internal const string IsMandatoryTagPath = "./isMandatory";

		internal const string IsEnabledByDefaultTagName = "isEnabledByDefault";

		internal const string IsEnabledByDefaultTagPath = "./isEnabledByDefault";

		internal const string ProvidedToTagName = "providedTo";

		internal const string ProvidedToTagPath = "./providedTo";

		internal const string SpecificUsersTagName = "users";

		internal const string SpecificUserTagName = "user";

		internal const string SpecificUserTagPath = "./users/user";

		internal const string EnabledTagName = "enabled";

		internal const string DisableReasonTagName = "disablereason";

		internal const string AppStatusTagName = "appstatus";

		internal const string InstalledByVersionTagName = "installedByVersion";

		internal const string EtokenTagName = "entitlementToken";

		internal const string ConfigServiceUrlKey = "MarketplaceConfigServiceUrl";

		internal const string LandingPageUrlKey = "MarketplaceLandingPageUrl";

		internal const string MyAppsPageUrlKey = "MarketplaceMyAppsPageUrl";

		internal const string XmlSchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

		internal const string ManifestTagName = "manifest";

		private const string ConfigServiceUrlDefault = "https://o15.officeredir.microsoft.com/r/rlidMktplcWSConfig15";

		private const string LandingPageUrlDefault = "https://o15.officeredir.microsoft.com/r/rlidMktplcExchRedirect";

		private const string MyAppsPageUrlDefault = "https://o15.officeredir.microsoft.com/r/rlidMktplcMUXMyOfficeApps";

		private const string TypeTagName = "type";

		private const string ScopeTagName = "scope";

		private static string configServiceUrl;

		private static string landingPageUrl;

		private static string myAppsPageUrl;

		private string versionAsString;

		private Version version;

		private SchemaParser schemaParser;

		private Uri highResolutionIconURL;

		private Version minApiVersion;

		private Version installedByVersion;

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		internal static readonly Version MinimumInstalledByVersion = new Version(15, 0, 516, 0);
	}
}
