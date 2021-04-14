using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class VdirConfiguration : ConfigurationBase
	{
		static VdirConfiguration()
		{
			if (!VdirConfiguration.useBackendVdirConfiguration)
			{
				VdirConfiguration.instances = new Dictionary<Guid, VdirConfiguration>();
			}
		}

		private VdirConfiguration()
		{
			AttachmentPolicy attachmentPolicy = new AttachmentPolicy(new StringArrayAppSettingsEntry("BlockFileTypes", new string[]
			{
				".vsmacros",
				".msh2xml",
				".msh1xml",
				".ps2xml",
				".ps1xml",
				".mshxml",
				".mhtml",
				".psc2",
				".psc1",
				".msh2",
				".msh1",
				".aspx",
				".xml",
				".wsh",
				".wsf",
				".wsc",
				".vsw",
				".vst",
				".vss",
				".vbs",
				".vbe",
				".url",
				".tmp",
				".shs",
				".shb",
				".sct",
				".scr",
				".scf",
				".reg",
				".pst",
				".ps2",
				".ps1",
				".prg",
				".prf",
				".plg",
				".pif",
				".pcd",
				".ops",
				".mst",
				".msp",
				".msi",
				".msh",
				".msc",
				".mht",
				".mdz",
				".mdw",
				".mdt",
				".mde",
				".mdb",
				".mda",
				".maw",
				".mav",
				".mau",
				".mat",
				".mas",
				".mar",
				".maq",
				".mam",
				".mag",
				".maf",
				".mad",
				".lnk",
				".ksh",
				".jse",
				".its",
				".isp",
				".ins",
				".inf",
				".htc",
				".hta",
				".hlp",
				".fxp",
				".exe",
				".der",
				".csh",
				".crt",
				".cpl",
				".com",
				".cmd",
				".chm",
				".cer",
				".bat",
				".bas",
				".asx",
				".asp",
				".app",
				".adp",
				".ade",
				".ws",
				".vb",
				".js"
			}, null).Value, new StringArrayAppSettingsEntry("BlockMimeTypes", new string[]
			{
				"application/x-javascript",
				"application/javascript",
				"application/msaccess",
				"x-internet-signup",
				"text/javascript",
				"application/xml",
				"application/prg",
				"application/hta",
				"text/scriplet",
				"text/xml"
			}, null).Value, new StringArrayAppSettingsEntry("ForceSaveFileTypes", new string[]
			{
				".vsmacros",
				".msh2xml",
				".msh1xml",
				".ps2xml",
				".ps1xml",
				".mshxml",
				".mhtml",
				".psc2",
				".psc1",
				".msh2",
				".msh1",
				".aspx",
				".xml",
				".wsh",
				".wsf",
				".wsc",
				".vsw",
				".vst",
				".vss",
				".vbs",
				".vbe",
				".url",
				".tmp",
				".shs",
				".shb",
				".sct",
				".scr",
				".scf",
				".reg",
				".pst",
				".ps2",
				".ps1",
				".prg",
				".prf",
				".plg",
				".pif",
				".pcd",
				".ops",
				".mst",
				".msp",
				".msi",
				".msh",
				".msc",
				".mht",
				".mdz",
				".mdw",
				".mdt",
				".mde",
				".mdb",
				".mda",
				".maw",
				".mav",
				".mau",
				".mat",
				".mas",
				".mar",
				".maq",
				".mam",
				".mag",
				".maf",
				".mad",
				".lnk",
				".ksh",
				".jse",
				".its",
				".isp",
				".ins",
				".inf",
				".htc",
				".hta",
				".hlp",
				".fxp",
				".exe",
				".der",
				".csh",
				".crt",
				".cpl",
				".com",
				".cmd",
				".chm",
				".cer",
				".bat",
				".bas",
				".asx",
				".asp",
				".app",
				".adp",
				".ade",
				".ws",
				".vb",
				".js"
			}, null).Value, new StringArrayAppSettingsEntry("ForceSaveMimeTypes", new string[]
			{
				"Application/x-shockwave-flash",
				"Application/octet-stream",
				"Application/futuresplash",
				"Application/x-director"
			}, null).Value, new StringArrayAppSettingsEntry("AllowFileTypes", new string[]
			{
				".rpmsg",
				".xlsx",
				".xlsm",
				".xlsb",
				".pptx",
				".pptm",
				".ppsx",
				".ppsm",
				".docx",
				".docm",
				".zip",
				".xls",
				".wmv",
				".wma",
				".wav",
				".vsd",
				".txt",
				".tif",
				".rtf",
				".pub",
				".ppt",
				".png",
				".pdf",
				".one",
				".mp3",
				".jpg",
				".gif",
				".doc",
				".bmp",
				".avi"
			}, null).Value, new StringArrayAppSettingsEntry("AllowMimeTypes", new string[0], null).Value, (AttachmentPolicyLevel)Enum.Parse(typeof(AttachmentPolicyLevel), new StringAppSettingsEntry("ActionForUnknownFileAndMIMETypes", "ForceSave", null).Value), new BoolAppSettingsEntry("DirectFileAccessOnPublicComputersEnabled", true, null).Value, new BoolAppSettingsEntry("DirectFileAccessOnPrivateComputersEnabled", true, null).Value, new BoolAppSettingsEntry("ForceWacViewingFirstOnPublicComputers", false, null).Value, new BoolAppSettingsEntry("ForceWacViewingFirstOnPrivateComputers", false, null).Value, new BoolAppSettingsEntry("WacViewingOnPublicComputersEnabled", true, null).Value, new BoolAppSettingsEntry("WacViewingOnPrivateComputersEnabled", true, null).Value, new BoolAppSettingsEntry("ForceWebReadyDocumentViewingFirstOnPublicComputers", false, null).Value, new BoolAppSettingsEntry("ForceWebReadyDocumentViewingFirstOnPrivateComputers", false, null).Value, new BoolAppSettingsEntry("WebReadyDocumentViewingOnPublicComputersEnabled", true, null).Value, new BoolAppSettingsEntry("WebReadyDocumentViewingOnPrivateComputersEnabled", true, null).Value, new StringArrayAppSettingsEntry("WebReadyFileTypes", new string[]
			{
				".xlsx",
				".xlsm",
				".xlsb",
				".pptx",
				".pptm",
				".ppsx",
				".ppsm",
				".docx",
				".docm",
				".xls",
				".rtf",
				".pdf"
			}, null).Value, new StringArrayAppSettingsEntry("WebReadyMimeTypes", new string[0], null).Value, new StringArrayAppSettingsEntry("WebReadyDocumentViewingSupportedFileTypes", new string[]
			{
				".xlsx",
				".xlsm",
				".xlsb",
				".pptx",
				".pptm",
				".ppsx",
				".ppsm",
				".docx",
				".docm",
				".xls",
				".rtf",
				".pdf"
			}, null).Value, new StringArrayAppSettingsEntry("WebReadyDocumentViewingSupportedMimeTypes", new string[0], null).Value, new BoolAppSettingsEntry("WebReadyDocumentViewingForAllSupportedTypes", false, null).Value);
			base.AttachmentPolicy = attachmentPolicy;
			WebBeaconFilterLevels value = (WebBeaconFilterLevels)Enum.Parse(typeof(WebBeaconFilterLevels), new StringAppSettingsEntry("FilterWebBeaconsAndHtmlForms", "UserFilterChoice", null).Value);
			this.filterWebBeaconsAndHtmlForms = VdirConfiguration.GetWebBeaconFilterLevel(new WebBeaconFilterLevels?(value));
			base.DefaultTheme = new StringAppSettingsEntry("DefaultTheme", string.Empty, null).Value;
			base.SegmentationFlags = (ulong)Enum.Parse(typeof(Feature), new StringAppSettingsEntry("SegmentationFlags", "All", null).Value);
			base.OutboundCharset = (OutboundCharsetOptions)Enum.Parse(typeof(OutboundCharsetOptions), new StringAppSettingsEntry("OutboundCharset", "AutoDetect", null).Value);
			base.UseGB18030 = new BoolAppSettingsEntry("UseGB18030", false, null).Value;
			base.UseISO885915 = new BoolAppSettingsEntry("UseISO885915", false, null).Value;
			base.InstantMessagingType = (InstantMessagingTypeOptions)Enum.Parse(typeof(InstantMessagingTypeOptions), new StringAppSettingsEntry("InstantMessagingType", "Ocs", null).Value);
			base.InstantMessagingEnabled = new BoolAppSettingsEntry("InstantMessagingEnabled", true, null).Value;
			base.AllowOfflineOn = (AllowOfflineOnEnum)Enum.Parse(typeof(AllowOfflineOnEnum), new StringAppSettingsEntry("AllowOfflineOn", "AllComputers", null).Value);
			this.formsAuthenticationEnabled = new BoolAppSettingsEntry("FormsAuthenticationEnabled", true, null).Value;
			base.RecoverDeletedItemsEnabled = new BoolAppSettingsEntry("RecoverDeletedItemsEnabled", true, null).Value;
		}

		private VdirConfiguration(ITopologyConfigurationSession session, ADOwaVirtualDirectory owaVirtualDirectory)
		{
			AttachmentPolicyLevel treatUnknownTypeAs = ConfigurationBase.AttachmentActionToPolicyLevel(owaVirtualDirectory.ActionForUnknownFileAndMIMETypes);
			AttachmentPolicy attachmentPolicy = new AttachmentPolicy(owaVirtualDirectory.BlockedFileTypes.ToArray(), owaVirtualDirectory.BlockedMimeTypes.ToArray(), owaVirtualDirectory.ForceSaveFileTypes.ToArray(), owaVirtualDirectory.ForceSaveMimeTypes.ToArray(), owaVirtualDirectory.AllowedFileTypes.ToArray(), owaVirtualDirectory.AllowedMimeTypes.ToArray(), treatUnknownTypeAs, owaVirtualDirectory.DirectFileAccessOnPublicComputersEnabled.Value, owaVirtualDirectory.DirectFileAccessOnPrivateComputersEnabled.Value, owaVirtualDirectory.ForceWacViewingFirstOnPublicComputers.Value, owaVirtualDirectory.ForceWacViewingFirstOnPrivateComputers.Value, owaVirtualDirectory.WacViewingOnPublicComputersEnabled.Value, owaVirtualDirectory.WacViewingOnPrivateComputersEnabled.Value, owaVirtualDirectory.ForceWebReadyDocumentViewingFirstOnPublicComputers.Value, owaVirtualDirectory.ForceWebReadyDocumentViewingFirstOnPrivateComputers.Value, owaVirtualDirectory.WebReadyDocumentViewingOnPublicComputersEnabled.Value, owaVirtualDirectory.WebReadyDocumentViewingOnPrivateComputersEnabled.Value, owaVirtualDirectory.WebReadyFileTypes.ToArray(), owaVirtualDirectory.WebReadyMimeTypes.ToArray(), owaVirtualDirectory.WebReadyDocumentViewingSupportedFileTypes.ToArray(), owaVirtualDirectory.WebReadyDocumentViewingSupportedMimeTypes.ToArray(), owaVirtualDirectory.WebReadyDocumentViewingForAllSupportedTypes.Value);
			this.formsAuthenticationEnabled = owaVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.Fba);
			this.windowsAuthenticationEnabled = owaVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.WindowsIntegrated);
			this.basicAuthenticationEnabled = owaVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.Basic);
			this.digestAuthenticationEnabled = owaVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.Digest);
			base.AttachmentPolicy = attachmentPolicy;
			this.filterWebBeaconsAndHtmlForms = VdirConfiguration.GetWebBeaconFilterLevel(owaVirtualDirectory.FilterWebBeaconsAndHtmlForms);
			base.DefaultTheme = owaVirtualDirectory.DefaultTheme;
			int segmentationBits = (int)owaVirtualDirectory[ADOwaVirtualDirectorySchema.ADMailboxFolderSet];
			int segmentationBits2 = (int)owaVirtualDirectory[ADOwaVirtualDirectorySchema.ADMailboxFolderSet2];
			base.SegmentationFlags = ConfigurationBase.SetSegmentationFlags(segmentationBits, segmentationBits2);
			if (owaVirtualDirectory.OutboundCharset != null)
			{
				base.OutboundCharset = owaVirtualDirectory.OutboundCharset.Value;
			}
			if (owaVirtualDirectory.UseGB18030 != null && owaVirtualDirectory.UseGB18030.Value)
			{
				base.UseGB18030 = true;
			}
			else
			{
				base.UseGB18030 = false;
			}
			if (owaVirtualDirectory.UseISO885915 != null && owaVirtualDirectory.UseISO885915.Value)
			{
				base.UseISO885915 = true;
			}
			else
			{
				base.UseISO885915 = false;
			}
			base.InstantMessagingEnabled = (owaVirtualDirectory.InstantMessagingEnabled ?? false);
			base.InstantMessagingType = (owaVirtualDirectory.InstantMessagingType ?? InstantMessagingTypeOptions.None);
			base.InstantMessagingEnabled &= (base.InstantMessagingType == InstantMessagingTypeOptions.Ocs);
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.UseVdirConfigForInstantMessaging.Enabled)
			{
				this.instantMessagingServerName = owaVirtualDirectory.InstantMessagingServerName;
				this.instantMessagingCertificateThumbprint = owaVirtualDirectory.InstantMessagingCertificateThumbprint;
				if (string.IsNullOrWhiteSpace(this.instantMessagingServerName))
				{
					Organization orgContainer = session.GetOrgContainer();
					ProtocolConnectionSettings sipaccessService = orgContainer.SIPAccessService;
					if (sipaccessService == null)
					{
						this.instantMessagingServerName = string.Empty;
					}
					else
					{
						this.instantMessagingServerName = sipaccessService.Hostname.ToString();
					}
				}
			}
			base.AllowOfflineOn = owaVirtualDirectory.AllowOfflineOn;
			base.PlacesEnabled = (owaVirtualDirectory.PlacesEnabled != null && owaVirtualDirectory.PlacesEnabled.Value);
			base.WeatherEnabled = (owaVirtualDirectory.WeatherEnabled != null && owaVirtualDirectory.WeatherEnabled.Value);
			base.AllowCopyContactsToDeviceAddressBook = (owaVirtualDirectory.AllowCopyContactsToDeviceAddressBook != null && owaVirtualDirectory.AllowCopyContactsToDeviceAddressBook.Value);
			base.RecoverDeletedItemsEnabled = (owaVirtualDirectory.RecoverDeletedItemsEnabled != null && owaVirtualDirectory.RecoverDeletedItemsEnabled.Value);
			this.expirationTime = DateTime.UtcNow + VdirConfiguration.expirationPeriod;
		}

		public static VdirConfiguration Instance
		{
			get
			{
				return VdirConfiguration.GetInstance(null, null, null, null, null, new bool?(false));
			}
		}

		internal static VdirConfiguration GetInstance(NameValueCollection requestHeaders = null, string requestUserAgent = null, string rawUrl = null, Uri uri = null, string userHostAddress = null, bool? isLocal = false)
		{
			if (VdirConfiguration.useBackendVdirConfiguration || Globals.IsPreCheckinApp)
			{
				lock (VdirConfiguration.syncRoot)
				{
					if (VdirConfiguration.instance == null || VdirConfiguration.instance.expirationTime < DateTime.UtcNow)
					{
						VdirConfiguration.instance = (Globals.IsPreCheckinApp ? new VdirConfiguration() : VdirConfiguration.CreateInstance(Guid.Empty));
					}
				}
				return VdirConfiguration.instance;
			}
			if (!VdirConfiguration.IsRequestFromCafe(requestHeaders ?? HttpContext.Current.Request.Headers) && UserAgentUtilities.IsMonitoringRequest(requestUserAgent ?? HttpContext.Current.Request.UserAgent))
			{
				return new VdirConfiguration();
			}
			Guid requestVdirObjectGuid = VdirConfiguration.GetRequestVdirObjectGuid(requestHeaders, requestUserAgent, rawUrl, uri, userHostAddress, isLocal);
			VdirConfiguration result;
			lock (VdirConfiguration.syncRoot)
			{
				VdirConfiguration.RemoveEntriesIfExpired();
				VdirConfiguration vdirConfiguration;
				if (VdirConfiguration.instances.TryGetValue(requestVdirObjectGuid, out vdirConfiguration))
				{
					result = vdirConfiguration;
				}
				else
				{
					try
					{
						vdirConfiguration = VdirConfiguration.CreateInstance(requestVdirObjectGuid);
						VdirConfiguration.instances.Add(requestVdirObjectGuid, vdirConfiguration);
						if (VdirConfiguration.minExpirationTime > vdirConfiguration.expirationTime)
						{
							VdirConfiguration.minExpirationTime = vdirConfiguration.expirationTime;
						}
					}
					catch (ADConfigurationException innerException)
					{
						throw new InvalidRequestException(innerException);
					}
					result = vdirConfiguration;
				}
			}
			return result;
		}

		internal WebBeaconFilterLevels FilterWebBeaconsAndHtmlForms
		{
			get
			{
				return this.filterWebBeaconsAndHtmlForms;
			}
		}

		internal bool FormsAuthenticationEnabled
		{
			get
			{
				return this.formsAuthenticationEnabled;
			}
		}

		internal bool WindowsAuthenticationEnabled
		{
			get
			{
				return this.windowsAuthenticationEnabled;
			}
		}

		internal bool BasicAuthenticationEnabled
		{
			get
			{
				return this.basicAuthenticationEnabled;
			}
		}

		internal bool DigestAuthenticationEnabled
		{
			get
			{
				return this.digestAuthenticationEnabled;
			}
		}

		internal string InstantMessagingServerName
		{
			get
			{
				return this.instantMessagingServerName;
			}
		}

		internal string InstantMessagingCertificateThumbprint
		{
			get
			{
				return this.instantMessagingCertificateThumbprint;
			}
		}

		private static VdirConfiguration CreateInstance(Guid vDirADObjectGuid)
		{
			ITopologyConfigurationSession topologyConfigurationSession = VdirConfiguration.CreateADSystemConfigurationSessionScopedToFirstOrg();
			string text = null;
			ADObjectId adobjectId;
			if (vDirADObjectGuid == Guid.Empty)
			{
				Server server = topologyConfigurationSession.FindLocalServer();
				string text2 = HttpRuntime.AppDomainAppVirtualPath.Replace("'", "''");
				if (text2[0] == '/')
				{
					text2 = text2.Substring(1);
				}
				text = HttpRuntime.AppDomainAppId;
				if (text[0] == '/')
				{
					text = text.Substring(1);
				}
				int num = text.IndexOf('/');
				text = text.Substring(num);
				text = string.Format("IIS://{0}{1}", server.Fqdn, text);
				num = text.LastIndexOf('/');
				text = VdirConfiguration.GetWebSiteName(text.Substring(0, num));
				adobjectId = new ADObjectId(server.DistinguishedName).GetDescendantId("Protocols", "HTTP", new string[]
				{
					string.Format("{0} ({1})", text2, text)
				});
			}
			else
			{
				adobjectId = new ADObjectId(vDirADObjectGuid);
			}
			ADOwaVirtualDirectory adowaVirtualDirectory = topologyConfigurationSession.Read<ADOwaVirtualDirectory>(adobjectId);
			if (adowaVirtualDirectory == null)
			{
				ExTraceGlobals.ConfigurationManagerTracer.TraceDebug<ADObjectId, string>(0L, "Unable to retrieve the vdir configuration for {0}, website name {1}", adobjectId, text);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_OwaConfigurationNotFound, adobjectId.ToDNString(), new object[]
				{
					adobjectId.ToString()
				});
				throw new ADConfigurationException();
			}
			return new VdirConfiguration(topologyConfigurationSession, adowaVirtualDirectory);
		}

		private static ITopologyConfigurationSession CreateADSystemConfigurationSessionScopedToFirstOrg()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false);
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.FullyConsistent, sessionSettings, 638, "CreateADSystemConfigurationSessionScopedToFirstOrg", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\configuration\\VdirConfiguration.cs");
		}

		private static string GetWebSiteName(string webSiteRootPath)
		{
			string result;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(webSiteRootPath))
				{
					using (DirectoryEntry parent = directoryEntry.Parent)
					{
						if (parent != null)
						{
							result = (((string)parent.Properties["ServerComment"].Value) ?? string.Empty);
						}
						else
						{
							result = string.Empty;
						}
					}
				}
			}
			catch (DirectoryServicesCOMException ex)
			{
				ExTraceGlobals.ConfigurationManagerTracer.TraceDebug<string, DirectoryServicesCOMException>(0L, "Unable to retrieve the web site name for {0}. DirectoryServicesCOMException {1}", webSiteRootPath, ex);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_OwaConfigurationWebSiteUnavailable, webSiteRootPath, new object[]
				{
					webSiteRootPath,
					ex
				});
				throw new ADConfigurationException(ex);
			}
			catch (DirectoryNotFoundException ex2)
			{
				ExTraceGlobals.ConfigurationManagerTracer.TraceDebug<string, DirectoryNotFoundException>(0L, "Unable to retrieve the web site name for {0}. DirectoryNotFoundException {1}", webSiteRootPath, ex2);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_OwaConfigurationWebSiteUnavailable, webSiteRootPath, new object[]
				{
					webSiteRootPath,
					ex2
				});
				throw new ADConfigurationException(ex2);
			}
			return result;
		}

		private static bool IsRequestFromCafe(NameValueCollection headers)
		{
			return headers["X-vDirObjectId"] != null;
		}

		private static Guid GetRequestVdirObjectGuid(NameValueCollection requestHeaders = null, string requestUserAgent = null, string rawUrl = null, Uri uri = null, string userHostAddress = null, bool? isLocal = false)
		{
			string text = null;
			requestHeaders = (requestHeaders ?? HttpContext.Current.Request.Headers);
			Guid result;
			try
			{
				text = requestHeaders["X-vDirObjectId"];
				result = new Guid(text);
			}
			catch (Exception innerException)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}='{1}',RawUrl:'{2}',Url:'{3}',UserHostAddress:'{4}',IsLocal:{5}", new object[]
				{
					"X-vDirObjectId",
					text,
					rawUrl ?? HttpContext.Current.Request.RawUrl,
					uri ?? HttpContext.Current.Request.Url,
					userHostAddress ?? HttpContext.Current.Request.UserHostAddress,
					isLocal ?? HttpContext.Current.Request.IsLocal
				});
				string a = requestHeaders[WellKnownHeader.XIsFromCafe];
				if (string.Equals(a, "1"))
				{
					foreach (string text2 in requestHeaders.AllKeys)
					{
						stringBuilder.AppendFormat(",[{0}]='{1}'", text2, requestHeaders[text2]);
					}
					throw new InvalidRequestException(new ArgumentException(stringBuilder.ToString(), innerException));
				}
				throw new OwaInvalidRequestException(stringBuilder.ToString(), innerException);
			}
			return result;
		}

		private static void RemoveEntriesIfExpired()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (VdirConfiguration.minExpirationTime < utcNow)
			{
				VdirConfiguration.minExpirationTime = DateTime.MaxValue;
				foreach (KeyValuePair<Guid, VdirConfiguration> keyValuePair in VdirConfiguration.instances)
				{
					if (keyValuePair.Value.expirationTime < utcNow)
					{
						VdirConfiguration.expiredInstances.Add(keyValuePair.Key);
					}
					else if (keyValuePair.Value.expirationTime < VdirConfiguration.minExpirationTime)
					{
						VdirConfiguration.minExpirationTime = keyValuePair.Value.expirationTime;
					}
				}
				if (VdirConfiguration.expiredInstances.Count > 0)
				{
					foreach (Guid key in VdirConfiguration.expiredInstances)
					{
						VdirConfiguration.instances.Remove(key);
					}
					VdirConfiguration.expiredInstances.Clear();
				}
			}
		}

		private static WebBeaconFilterLevels GetWebBeaconFilterLevel(WebBeaconFilterLevels? filterLevel)
		{
			WebBeaconFilterLevels valueOrDefault = filterLevel.GetValueOrDefault();
			if (filterLevel == null)
			{
				return WebBeaconFilterLevels.UserFilterChoice;
			}
			switch (valueOrDefault)
			{
			case WebBeaconFilterLevels.UserFilterChoice:
				return WebBeaconFilterLevels.UserFilterChoice;
			case WebBeaconFilterLevels.ForceFilter:
				return WebBeaconFilterLevels.ForceFilter;
			case WebBeaconFilterLevels.DisableFilter:
				return WebBeaconFilterLevels.DisableFilter;
			default:
				return WebBeaconFilterLevels.ForceFilter;
			}
		}

		private const string VDirObjectIdHeaderName = "X-vDirObjectId";

		private readonly DateTime expirationTime;

		private readonly bool formsAuthenticationEnabled;

		private readonly bool windowsAuthenticationEnabled;

		private readonly bool digestAuthenticationEnabled;

		private readonly bool basicAuthenticationEnabled;

		private readonly string instantMessagingServerName;

		private readonly string instantMessagingCertificateThumbprint;

		private static volatile VdirConfiguration instance = null;

		private static volatile Dictionary<Guid, VdirConfiguration> instances;

		private static volatile List<Guid> expiredInstances = new List<Guid>();

		private static DateTime minExpirationTime = DateTime.MinValue;

		private static object syncRoot = new object();

		private static bool useBackendVdirConfiguration = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.UseBackendVdirConfiguration.Enabled;

		private static TimeSpan expirationPeriod = new TimeSpan(0, 3, 0, 0);

		private WebBeaconFilterLevels filterWebBeaconsAndHtmlForms;
	}
}
