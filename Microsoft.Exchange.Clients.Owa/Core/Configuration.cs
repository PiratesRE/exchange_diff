using System;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class Configuration : ConfigurationBase
	{
		internal Configuration(IConfigurationSession session, string virtualDirectory, string webSiteName, ADObjectId vDirADObjectId, bool isPhoneticSupportEnabled)
		{
			base.PhoneticSupportEnabled = isPhoneticSupportEnabled;
			if (Globals.IsPreCheckinApp)
			{
				this.ExpirationTime = DateTime.UtcNow + Configuration.expirationPeriod;
				this.LoadPreCheckInVdirConfiguration();
				return;
			}
			ADOwaVirtualDirectory adowaVirtualDirectory = session.Read<ADOwaVirtualDirectory>(vDirADObjectId);
			if (adowaVirtualDirectory == null)
			{
				string message = string.Format(LocalizedStrings.GetNonEncoded(-1166886287), virtualDirectory, webSiteName);
				throw new OwaInvalidConfigurationException(message);
			}
			this.formsAuthenticationEnabled = (adowaVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.Fba) ? 1 : 0);
			AttachmentPolicy.Level treatUnknownTypeAs = ConfigurationBase.AttachmentActionToLevel(adowaVirtualDirectory.ActionForUnknownFileAndMIMETypes);
			AttachmentPolicy attachmentPolicy = new AttachmentPolicy(adowaVirtualDirectory.BlockedFileTypes.ToArray(), adowaVirtualDirectory.BlockedMimeTypes.ToArray(), adowaVirtualDirectory.ForceSaveFileTypes.ToArray(), adowaVirtualDirectory.ForceSaveMimeTypes.ToArray(), adowaVirtualDirectory.AllowedFileTypes.ToArray(), adowaVirtualDirectory.AllowedMimeTypes.ToArray(), treatUnknownTypeAs, adowaVirtualDirectory.DirectFileAccessOnPublicComputersEnabled.Value, adowaVirtualDirectory.DirectFileAccessOnPrivateComputersEnabled.Value, adowaVirtualDirectory.ForceWebReadyDocumentViewingFirstOnPublicComputers.Value, adowaVirtualDirectory.ForceWebReadyDocumentViewingFirstOnPrivateComputers.Value, adowaVirtualDirectory.WebReadyDocumentViewingOnPublicComputersEnabled.Value, adowaVirtualDirectory.WebReadyDocumentViewingOnPrivateComputersEnabled.Value, adowaVirtualDirectory.WebReadyFileTypes.ToArray(), adowaVirtualDirectory.WebReadyMimeTypes.ToArray(), adowaVirtualDirectory.WebReadyDocumentViewingSupportedFileTypes.ToArray(), adowaVirtualDirectory.WebReadyDocumentViewingSupportedMimeTypes.ToArray(), adowaVirtualDirectory.WebReadyDocumentViewingForAllSupportedTypes.Value);
			base.AttachmentPolicy = attachmentPolicy;
			base.DefaultClientLanguage = adowaVirtualDirectory.DefaultClientLanguage.Value;
			this.filterWebBeaconsAndHtmlForms = adowaVirtualDirectory.FilterWebBeaconsAndHtmlForms.Value;
			base.LogonAndErrorLanguage = adowaVirtualDirectory.LogonAndErrorLanguage;
			this.logonFormat = adowaVirtualDirectory.LogonFormat;
			this.defaultDomain = adowaVirtualDirectory.DefaultDomain;
			this.notificationInterval = (adowaVirtualDirectory.NotificationInterval ?? 120);
			this.sessionTimeout = (adowaVirtualDirectory.UserContextTimeout ?? 60);
			this.redirectToOptimalOWAServer = (adowaVirtualDirectory.RedirectToOptimalOWAServer == true);
			base.DefaultTheme = adowaVirtualDirectory.DefaultTheme;
			base.SetPhotoURL = adowaVirtualDirectory.SetPhotoURL;
			this.clientAuthCleanupLevel = adowaVirtualDirectory.ClientAuthCleanupLevel;
			this.imCertificateThumbprint = adowaVirtualDirectory.InstantMessagingCertificateThumbprint;
			this.imServerName = adowaVirtualDirectory.InstantMessagingServerName;
			this.isSMimeEnabledOnCurrentServerr = (adowaVirtualDirectory.SMimeEnabled ?? false);
			this.documentAccessAllowedServers = adowaVirtualDirectory.RemoteDocumentsAllowedServers.ToArray();
			this.documentAccessBlockedServers = adowaVirtualDirectory.RemoteDocumentsBlockedServers.ToArray();
			this.documentAccessInternalDomainSuffixList = adowaVirtualDirectory.RemoteDocumentsInternalDomainSuffixList.ToArray();
			RemoteDocumentsActions? remoteDocumentsActions = adowaVirtualDirectory.RemoteDocumentsActionForUnknownServers;
			if (remoteDocumentsActions != null)
			{
				if (remoteDocumentsActions == RemoteDocumentsActions.Allow)
				{
					this.remoteDocumentsActionForUnknownServers = RemoteDocumentsActions.Allow;
				}
				else
				{
					this.remoteDocumentsActionForUnknownServers = RemoteDocumentsActions.Block;
				}
			}
			base.InternalAuthenticationMethod = ConfigurationBase.GetAuthenticationMethod(adowaVirtualDirectory[ADVirtualDirectorySchema.InternalAuthenticationMethodFlags]);
			base.ExternalAuthenticationMethod = ConfigurationBase.GetAuthenticationMethod(adowaVirtualDirectory[ADVirtualDirectorySchema.ExternalAuthenticationMethodFlags]);
			base.Exchange2003Url = adowaVirtualDirectory.Exchange2003Url;
			base.LegacyRedirectType = LegacyRedirectTypeOptions.Silent;
			int segmentationBits = (int)adowaVirtualDirectory[ADOwaVirtualDirectorySchema.ADMailboxFolderSet];
			int segmentationBits2 = (int)adowaVirtualDirectory[ADOwaVirtualDirectorySchema.ADMailboxFolderSet2];
			base.SegmentationFlags = Utilities.SetSegmentationFlags(segmentationBits, segmentationBits2);
			if (adowaVirtualDirectory.OutboundCharset != null)
			{
				base.OutboundCharset = adowaVirtualDirectory.OutboundCharset.Value;
			}
			if (adowaVirtualDirectory.UseGB18030 != null && adowaVirtualDirectory.UseGB18030.Value)
			{
				base.UseGB18030 = true;
			}
			else
			{
				base.UseGB18030 = false;
			}
			if (adowaVirtualDirectory.UseISO885915 != null && adowaVirtualDirectory.UseISO885915.Value)
			{
				base.UseISO885915 = true;
			}
			else
			{
				base.UseISO885915 = false;
			}
			base.InstantMessagingType = ((adowaVirtualDirectory.InstantMessagingType != null) ? adowaVirtualDirectory.InstantMessagingType.Value : InstantMessagingTypeOptions.None);
			this.defaultAcceptedDomain = session.GetDefaultAcceptedDomain();
			this.publicFoldersEnabledOnThisVdir = (adowaVirtualDirectory.PublicFoldersEnabled ?? false);
			this.ExpirationTime = DateTime.UtcNow + Configuration.expirationPeriod;
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ConfigurationSettingsUpdated, string.Empty, new object[]
			{
				virtualDirectory,
				webSiteName
			});
		}

		protected Configuration()
		{
		}

		private string GetAccessProxyAddress()
		{
			string result = string.Empty;
			CachedOrganizationConfiguration instance = CachedOrganizationConfiguration.GetInstance(OrganizationId.ForestWideOrgId, CachedOrganizationConfiguration.ConfigurationTypes.All);
			Organization configuration = instance.OrganizationConfiguration.Configuration;
			ProtocolConnectionSettings sipaccessService = configuration.SIPAccessService;
			if (sipaccessService != null)
			{
				result = sipaccessService.Hostname.ToString();
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

		public LogonFormats LogonFormat
		{
			get
			{
				return this.logonFormat;
			}
		}

		public string DefaultDomain
		{
			get
			{
				return this.defaultDomain;
			}
		}

		public int NotificationInterval
		{
			get
			{
				return this.notificationInterval;
			}
		}

		public int SessionTimeout
		{
			get
			{
				return this.sessionTimeout;
			}
		}

		public bool FormsAuthenticationEnabled
		{
			get
			{
				return this.formsAuthenticationEnabled != 0;
			}
		}

		public bool RedirectToOptimalOWAServer
		{
			get
			{
				return this.redirectToOptimalOWAServer;
			}
		}

		public ClientAuthCleanupLevels ClientAuthCleanupLevel
		{
			get
			{
				return this.clientAuthCleanupLevel;
			}
		}

		public string[] BlockedDocumentStoreList
		{
			get
			{
				return this.documentAccessBlockedServers;
			}
		}

		public string[] AllowedDocumentStoreList
		{
			get
			{
				return this.documentAccessAllowedServers;
			}
		}

		internal RemoteDocumentsActions RemoteDocumentsActionForUnknownServers
		{
			get
			{
				return this.remoteDocumentsActionForUnknownServers;
			}
		}

		public string[] InternalFQDNSuffixList
		{
			get
			{
				return this.documentAccessInternalDomainSuffixList;
			}
		}

		internal virtual AcceptedDomain DefaultAcceptedDomain
		{
			get
			{
				if (this.defaultAcceptedDomain == null)
				{
					throw new OwaInvalidConfigurationException("No default accepted domain found.");
				}
				return this.defaultAcceptedDomain;
			}
		}

		internal bool IsPublicFoldersEnabledOnThisVdir
		{
			get
			{
				return this.publicFoldersEnabledOnThisVdir;
			}
		}

		internal bool IsSMimeEnabledOnCurrentServerr
		{
			get
			{
				return this.isSMimeEnabledOnCurrentServerr;
			}
		}

		internal string InstantMessagingCertificateThumbprint
		{
			get
			{
				return this.imCertificateThumbprint;
			}
		}

		internal string InstantMessagingServerName
		{
			get
			{
				if (string.IsNullOrEmpty(this.imServerName) && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.UseAccessProxyForInstantMessagingServerName.Enabled)
				{
					this.imServerName = this.GetAccessProxyAddress();
				}
				return this.imServerName;
			}
		}

		private void LoadPreCheckInVdirConfiguration()
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
			}, null).Value, new StringArrayAppSettingsEntry("AllowMimeTypes", new string[0], null).Value, (AttachmentPolicy.Level)Enum.Parse(typeof(AttachmentPolicy.Level), new StringAppSettingsEntry("ActionForUnknownFileAndMIMETypes", "ForceSave", null).Value), new BoolAppSettingsEntry("DirectFileAccessOnPublicComputersEnabled", true, null).Value, new BoolAppSettingsEntry("DirectFileAccessOnPrivateComputersEnabled", true, null).Value, new BoolAppSettingsEntry("ForceWebReadyDocumentViewingFirstOnPublicComputers", false, null).Value, new BoolAppSettingsEntry("ForceWebReadyDocumentViewingFirstOnPrivateComputers", false, null).Value, new BoolAppSettingsEntry("WebReadyDocumentViewingOnPublicComputersEnabled", true, null).Value, new BoolAppSettingsEntry("WebReadyDocumentViewingOnPrivateComputersEnabled", true, null).Value, new StringArrayAppSettingsEntry("WebReadyFileTypes", new string[]
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
			string value = new StringAppSettingsEntry("FilterWebBeaconsAndHtmlForms", "UserFilterChoice", null).Value;
			this.filterWebBeaconsAndHtmlForms = (WebBeaconFilterLevels)Enum.Parse(typeof(WebBeaconFilterLevels), value);
			base.DefaultTheme = new StringAppSettingsEntry("DefaultTheme", string.Empty, null).Value;
			base.SegmentationFlags = (ulong)Enum.Parse(typeof(Feature), new StringAppSettingsEntry("SegmentationFlags", "All", null).Value);
			base.OutboundCharset = (OutboundCharsetOptions)Enum.Parse(typeof(OutboundCharsetOptions), new StringAppSettingsEntry("OutboundCharset", "AutoDetect", null).Value);
			base.UseGB18030 = new BoolAppSettingsEntry("UseGB18030", false, null).Value;
			base.UseISO885915 = new BoolAppSettingsEntry("UseISO885915", false, null).Value;
			base.InstantMessagingType = (InstantMessagingTypeOptions)Enum.Parse(typeof(InstantMessagingTypeOptions), new StringAppSettingsEntry("InstantMessagingType", "Ocs", null).Value);
			this.imServerName = new StringAppSettingsEntry("InstantMessagingServerName", string.Empty, null).Value;
			this.formsAuthenticationEnabled = (new BoolAppSettingsEntry("FormsAuthenticationEnabled", true, null).Value ? 1 : 0);
			this.publicFoldersEnabledOnThisVdir = new BoolAppSettingsEntry("PublicFoldersEnabled", false, null).Value;
			this.notificationInterval = new IntAppSettingsEntry("NotificationInterval", 120, null).Value;
			this.sessionTimeout = new IntAppSettingsEntry("UserContextTimeout", 60, null).Value;
		}

		private const int DefaultNotificationInterval = 120;

		private const int DefaultSessionTimeOut = 60;

		private static TimeSpan expirationPeriod = new TimeSpan(0, 3, 0, 0);

		protected LogonFormats logonFormat;

		protected string defaultDomain;

		protected int notificationInterval = 120;

		protected int sessionTimeout;

		protected int formsAuthenticationEnabled;

		protected WebBeaconFilterLevels filterWebBeaconsAndHtmlForms;

		protected string[] documentAccessAllowedServers;

		protected string[] documentAccessBlockedServers;

		protected string[] documentAccessInternalDomainSuffixList;

		protected RemoteDocumentsActions remoteDocumentsActionForUnknownServers = RemoteDocumentsActions.Block;

		protected bool redirectToOptimalOWAServer = true;

		protected ClientAuthCleanupLevels clientAuthCleanupLevel;

		protected bool isSMimeEnabledOnCurrentServerr;

		protected bool publicFoldersEnabledOnThisVdir;

		protected string imCertificateThumbprint;

		protected string imServerName;

		protected AcceptedDomain defaultAcceptedDomain;

		public readonly DateTime ExpirationTime;
	}
}
