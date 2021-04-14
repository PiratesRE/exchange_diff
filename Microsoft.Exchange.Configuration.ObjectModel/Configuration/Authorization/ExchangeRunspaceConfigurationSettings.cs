using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Authorization
{
	public sealed class ExchangeRunspaceConfigurationSettings
	{
		internal ExchangeRunspaceConfigurationSettings() : this(null)
		{
		}

		internal ExchangeRunspaceConfigurationSettings(Uri connectionUri) : this(connectionUri, ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown, null, ExchangeRunspaceConfigurationSettings.SerializationLevel.Partial)
		{
		}

		internal ExchangeRunspaceConfigurationSettings(Uri connectionUri, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string tenantOrganization, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel) : this(connectionUri, clientApplication, tenantOrganization, serializationLevel, PSLanguageMode.NoLanguage)
		{
		}

		internal ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string tenantOrganization, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel) : this(null, clientApplication, tenantOrganization, serializationLevel, PSLanguageMode.NoLanguage)
		{
		}

		internal ExchangeRunspaceConfigurationSettings(Uri connectionUri, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string tenantOrganization, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel, PSLanguageMode languageMode) : this(connectionUri, clientApplication, tenantOrganization, serializationLevel, languageMode, ExchangeRunspaceConfigurationSettings.ProxyMethod.RPS)
		{
		}

		internal ExchangeRunspaceConfigurationSettings(Uri connectionUri, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string tenantOrganization, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel, PSLanguageMode languageMode, ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod) : this(connectionUri, clientApplication, tenantOrganization, serializationLevel, languageMode, proxyMethod, false, false)
		{
		}

		internal ExchangeRunspaceConfigurationSettings(Uri connectionUri, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string tenantOrganization, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel, PSLanguageMode languageMode, ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod, bool proxyFullSerialization, bool encodeDecodeKey) : this(connectionUri, clientApplication, tenantOrganization, serializationLevel, languageMode, proxyMethod, proxyFullSerialization, encodeDecodeKey, false)
		{
		}

		internal ExchangeRunspaceConfigurationSettings(Uri connectionUri, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string tenantOrganization, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel, PSLanguageMode languageMode, ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod, bool proxyFullSerialization, bool encodeDecodeKey, bool isProxy) : this(connectionUri, clientApplication, tenantOrganization, serializationLevel, languageMode, proxyMethod, proxyFullSerialization, encodeDecodeKey, isProxy, ExchangeRunspaceConfigurationSettings.ExchangeUserType.Unknown, null)
		{
		}

		internal ExchangeRunspaceConfigurationSettings(Uri connectionUri, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string tenantOrganization, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel, PSLanguageMode languageMode, ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod, bool proxyFullSerialization, bool encodeDecodeKey, bool isProxy, ExchangeRunspaceConfigurationSettings.ExchangeUserType user, IEnumerable<KeyValuePair<string, string>> additionalConstraints)
		{
			this.clientApplication = clientApplication;
			this.serializationLevel = serializationLevel;
			this.tenantOrganization = tenantOrganization;
			this.languageMode = languageMode;
			this.originalConnectionUri = connectionUri;
			this.proxyMethod = proxyMethod;
			this.proxyFullSerialization = proxyFullSerialization;
			this.EncodeDecodeKey = encodeDecodeKey;
			this.IsProxy = isProxy;
			this.UserType = user;
			this.additionalConstraints = additionalConstraints;
		}

		internal static ExchangeRunspaceConfigurationSettings GetDefaultInstance()
		{
			return ExchangeRunspaceConfigurationSettings.defaultInstance;
		}

		internal ExchangeRunspaceConfigurationSettings.ExchangeApplication ClientApplication
		{
			get
			{
				return this.clientApplication;
			}
		}

		internal ExchangeRunspaceConfigurationSettings.SerializationLevel CurrentSerializationLevel
		{
			get
			{
				return this.serializationLevel;
			}
		}

		internal IEnumerable<KeyValuePair<string, string>> AdditionalConstraints
		{
			get
			{
				return this.additionalConstraints;
			}
		}

		internal string TenantOrganization
		{
			get
			{
				return this.tenantOrganization;
			}
			set
			{
				this.tenantOrganization = value;
			}
		}

		internal PSLanguageMode LanguageMode
		{
			get
			{
				return this.languageMode;
			}
			set
			{
				this.languageMode = value;
			}
		}

		internal ExchangeRunspaceConfigurationSettings.ProxyMethod CurrentProxyMethod
		{
			get
			{
				return this.proxyMethod;
			}
			set
			{
				this.proxyMethod = value;
			}
		}

		internal bool ProxyFullSerialization
		{
			get
			{
				return this.proxyFullSerialization;
			}
			set
			{
				this.proxyFullSerialization = value;
			}
		}

		internal bool EncodeDecodeKey { get; set; }

		internal bool IsProxy { get; private set; }

		internal ExchangeRunspaceConfigurationSettings.ExchangeUserType UserType { get; private set; }

		internal UserToken UserToken { get; set; }

		internal string SiteRedirectionTemplate
		{
			get
			{
				return this.siteRedirectionTemplate;
			}
			set
			{
				this.siteRedirectionTemplate = value;
			}
		}

		internal string PodRedirectionTemplate { get; set; }

		internal Uri OriginalConnectionUri
		{
			get
			{
				return this.originalConnectionUri;
			}
		}

		internal VariantConfigurationSnapshot VariantConfigurationSnapshot { get; set; }

		internal static string GetVDirPathFromUriLocalPath(Uri uri)
		{
			string localPath = uri.LocalPath;
			if (string.IsNullOrEmpty(localPath) || localPath[0] != '/')
			{
				return localPath;
			}
			int num = localPath.IndexOf('/', 1);
			if (num == -1)
			{
				return localPath;
			}
			return localPath.Substring(0, num);
		}

		internal static ExchangeRunspaceConfigurationSettings FromUriConnectionString(string connectionString, out string vdirPath)
		{
			return ExchangeRunspaceConfigurationSettings.FromUriConnectionString(connectionString, ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown, out vdirPath);
		}

		internal static ExchangeRunspaceConfigurationSettings FromUriConnectionString(string connectionString, ExchangeRunspaceConfigurationSettings.ExchangeApplication defaultApplication, out string vdirPath)
		{
			Uri uri = new Uri(connectionString, UriKind.Absolute);
			vdirPath = ExchangeRunspaceConfigurationSettings.GetVDirPathFromUriLocalPath(uri);
			if (string.IsNullOrEmpty(uri.Query))
			{
				return ExchangeRunspaceConfigurationSettings.GetDefaultInstance();
			}
			NameValueCollection nameValueCollectionFromUri = LiveIdBasicAuthModule.GetNameValueCollectionFromUri(uri);
			return ExchangeRunspaceConfigurationSettings.CreateConfigurationSettingsFromNameValueCollection(uri, nameValueCollectionFromUri, defaultApplication);
		}

		internal static ExchangeRunspaceConfigurationSettings FromRequestHeaders(string connectionString, ExchangeRunspaceConfigurationSettings.ExchangeApplication defaultApplication)
		{
			Uri uri = new Uri(connectionString, UriKind.Absolute);
			return ExchangeRunspaceConfigurationSettings.CreateConfigurationSettingsFromNameValueCollection(uri, HttpContext.Current.Request.Headers, defaultApplication);
		}

		internal static ExchangeRunspaceConfigurationSettings CreateConfigurationSettingsFromNameValueCollection(Uri uri, NameValueCollection collection, ExchangeRunspaceConfigurationSettings.ExchangeApplication defaultApplication)
		{
			string text = collection.Get("organization");
			ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel = ExchangeRunspaceConfigurationSettings.SerializationLevel.Partial;
			if (collection.Get("serializationLevel") != null)
			{
				Enum.TryParse<ExchangeRunspaceConfigurationSettings.SerializationLevel>(collection.Get("serializationLevel"), true, out serializationLevel);
			}
			string text2 = collection.Get("clientApplication");
			ExchangeRunspaceConfigurationSettings.ExchangeApplication exchangeApplication = defaultApplication;
			if (text2 != null)
			{
				Enum.TryParse<ExchangeRunspaceConfigurationSettings.ExchangeApplication>(text2, true, out exchangeApplication);
			}
			ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod = ExchangeRunspaceConfigurationSettings.ProxyMethod.None;
			if (collection.Get("proxyMethod") != null)
			{
				Enum.TryParse<ExchangeRunspaceConfigurationSettings.ProxyMethod>(collection.Get("proxyMethod"), true, out proxyMethod);
			}
			bool flag = false;
			if (collection.Get("proxyFullSerialization") != null)
			{
				bool.TryParse(collection.Get("proxyFullSerialization"), out flag);
			}
			bool encodeDecodeKey = true;
			if (collection.Get("X-EncodeDecode-Key") != null)
			{
				bool.TryParse(collection.Get("X-EncodeDecode-Key"), out encodeDecodeKey);
			}
			bool isProxy = ExchangeRunspaceConfigurationSettings.IsCalledFromProxy(collection);
			return new ExchangeRunspaceConfigurationSettings(uri, exchangeApplication, text, serializationLevel, PSLanguageMode.NoLanguage, proxyMethod, flag, encodeDecodeKey, isProxy);
		}

		internal static string AddClientApplicationToUrl(string url, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApp)
		{
			if (ExchangeRunspaceConfigurationSettings.regExForClientApp.IsMatch(url))
			{
				return ExchangeRunspaceConfigurationSettings.regExForClientApp.Replace(url, clientApp.ToString());
			}
			if (string.IsNullOrEmpty(new Uri(url).Query))
			{
				return string.Format("{0}?clientApplication={1}", url, clientApp.ToString());
			}
			return url += string.Format("{0}clientApplication={1}", url.EndsWith(";") ? string.Empty : ";", clientApp.ToString());
		}

		internal static bool IsCalledFromProxy(NameValueCollection headers)
		{
			string a = headers.Get(WellKnownHeader.CmdletProxyIsOn);
			return a == "99C6ECEE-5A4F-47B9-AE69-49EAFB58F368";
		}

		internal const string URIPropertyOrganization = "organization";

		internal const string URIPropertySerializationLevel = "serializationLevel";

		internal const string URIPropertyClientApplication = "clientApplication";

		internal const string URIPropertyProxyMethod = "proxyMethod";

		internal const string URIPropertyProxyFullSerialization = "proxyFullSerialization";

		internal const string CmdletProxyIsOnValue = "99C6ECEE-5A4F-47B9-AE69-49EAFB58F368";

		internal const PSLanguageMode DefaultLanguageMode = PSLanguageMode.NoLanguage;

		private static ExchangeRunspaceConfigurationSettings defaultInstance = new ExchangeRunspaceConfigurationSettings();

		private ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication;

		private string tenantOrganization;

		private ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel;

		private PSLanguageMode languageMode;

		private ExchangeRunspaceConfigurationSettings.ProxyMethod proxyMethod;

		private bool proxyFullSerialization;

		private string siteRedirectionTemplate;

		private Uri originalConnectionUri;

		private readonly IEnumerable<KeyValuePair<string, string>> additionalConstraints;

		private static readonly Regex regExForClientApp = new Regex("(?<=clientApplication=)\\w+", RegexOptions.IgnoreCase);

		public enum ExchangeApplication
		{
			Unknown,
			PowerShell,
			EMC,
			ECP,
			EWS,
			ManagementShell,
			DebugUser,
			CSVParser,
			GalSync,
			MigrationService,
			SimpleDataMigration,
			ForwardSync,
			BackSync,
			TipTestCase,
			NonInteractivePowershell,
			LowPriorityScripts,
			DiscretionaryScripts,
			ReportingWebService,
			PswsClient,
			Office365Partner,
			Intune,
			CRM,
			ActiveMonitor,
			TenantUser,
			SyndicationCentral,
			LiveEDU,
			OSP
		}

		public enum SerializationLevel
		{
			Partial,
			None,
			Full
		}

		public enum ProxyMethod
		{
			None,
			RPS,
			PSWS
		}

		public enum ExchangeUserType
		{
			Unknown,
			Monitoring
		}
	}
}
