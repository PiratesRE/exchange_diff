using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AutoDiscoverUserSettingsClient : DisposableObject
	{
		public static AutoDiscoverUserSettingsClient CreateInstance(ITopologyConfigurationSession topologyConfigurationSession, FedOrgCredentials credentials, SmtpAddress identity, Uri autoDiscoveryEndpoint, string[] requestedSettings)
		{
			Util.ThrowOnNullArgument(credentials, "credentials");
			RequestedToken token;
			try
			{
				token = credentials.GetToken();
			}
			catch (WSTrustException ex)
			{
				string text = identity.ToString();
				string text2 = ex.ToString();
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_AutoDiscoverFailedToAquireSecurityToken, text, new object[]
				{
					text,
					text2
				});
				ExTraceGlobals.XtcTracer.TraceError<string, string>(0L, "AutoDiscover request failed for {0}, failed to aquire security token. Exception: {1}.", text, text2);
				throw new AutoDAccessException(ServerStrings.AutoDFailedToGetToken, ex);
			}
			return new AutoDiscoverUserSettingsClient(topologyConfigurationSession, SoapHttpClientAuthenticator.Create(token), EwsWsSecurityUrl.Fix(autoDiscoveryEndpoint), identity, requestedSettings);
		}

		public static AutoDiscoverUserSettingsClient CreateInstance(ITopologyConfigurationSession topologyConfigurationSession, NetworkCredential credentials, SmtpAddress identity, string[] requestedSettings)
		{
			return new AutoDiscoverUserSettingsClient(topologyConfigurationSession, SoapHttpClientAuthenticator.CreateForSoap(credentials), null, identity, requestedSettings);
		}

		private AutoDiscoverUserSettingsClient(ITopologyConfigurationSession topologyConfigurationSession, SoapHttpClientAuthenticator authentificator, Uri autoDiscoveryEndpoint, SmtpAddress identity, string[] requestedSettings)
		{
			Util.ThrowOnNullArgument(topologyConfigurationSession, "topologyConfigurationSession");
			Util.ThrowOnNullArgument(identity, "identity");
			Util.ThrowOnNullArgument(requestedSettings, "requestedSettings");
			if (requestedSettings.Length == 0)
			{
				throw new ArgumentException("Requested settings array must be not empty.");
			}
			foreach (string value in requestedSettings)
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Cannot use null or empty string as a requested setting.");
				}
			}
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.requestedSettings = requestedSettings;
				this.autoDiscoveryUri = autoDiscoveryEndpoint;
				this.identity = identity;
				this.client = new AutodiscoverClient();
				this.client.RequestedServerVersion = DefaultBinding_Autodiscover.Exchange2010RequestedServerVersion;
				this.client.UserAgent = "ExchangeMiddleTierStorage";
				this.client.Authenticator = authentificator;
				string[] autodiscoverTrustedHosters = topologyConfigurationSession.GetAutodiscoverTrustedHosters();
				if (autodiscoverTrustedHosters != null)
				{
					this.client.AllowedHostnames.AddRange(autodiscoverTrustedHosters);
				}
				Server localServer = LocalServerCache.LocalServer;
				if (localServer != null && localServer.InternetWebProxy != null)
				{
					this.client.Proxy = new WebProxy(localServer.InternetWebProxy);
				}
				else
				{
					this.client.Proxy = new WebProxy();
				}
				disposeGuard.Success();
			}
		}

		public string AnchorMailbox
		{
			get
			{
				return this.client.AnchorMailbox;
			}
			set
			{
				this.client.AnchorMailbox = value;
			}
		}

		public UserSettings Discover()
		{
			this.CheckDisposed(null);
			SingleGetUserSettings singleGetUserSettings = new SingleGetUserSettings(this.requestedSettings);
			UserSettings userSettings;
			try
			{
				userSettings = singleGetUserSettings.Discover(this.client, this.identity.ToString(), this.autoDiscoveryUri);
			}
			catch (GetUserSettingsException ex)
			{
				string text = this.identity.ToString();
				string arg = ex.ToString();
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_AutoDiscoverFailed, text, new object[]
				{
					text,
					ex.ToString()
				});
				ExTraceGlobals.XtcTracer.TraceError<string, string>(0L, "The Autodiscover request failed for {0}. Error: {1}.", text, arg);
				throw new AutoDAccessException(ServerStrings.AutoDRequestFailed, ex);
			}
			bool flag = false;
			foreach (string text2 in this.requestedSettings)
			{
				if (userSettings.IsSettingError(text2))
				{
					string text3 = this.identity.ToString();
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_AutoDiscoverFailedForSetting, text3, new object[]
					{
						text3,
						text2
					});
					ExTraceGlobals.XtcTracer.TraceError<string, string>(0L, "The Autodiscover request failed for {0}. Setting: {1}.", text3, text2);
					flag = true;
				}
			}
			if (flag)
			{
				throw new AutoDAccessException(ServerStrings.AutoDRequestFailed);
			}
			return userSettings;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.client != null)
			{
				this.client.Dispose();
			}
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AutoDiscoverUserSettingsClient>(this);
		}

		private const string ComponentId = "ExchangeMiddleTierStorage";

		private readonly string[] requestedSettings;

		private AutodiscoverClient client;

		private SmtpAddress identity;

		private Uri autoDiscoveryUri;
	}
}
