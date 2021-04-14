using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Cmdlet("Install", "CannedPushNotificationApp")]
	public sealed class InstallCannedPushNotificationApp : DataAccessTask<PushNotificationApp>
	{
		[Parameter(Mandatory = false)]
		public Hashtable AuthenticationKeys
		{
			get
			{
				return (Hashtable)base.Fields["AuthenticationKeys"];
			}
			set
			{
				base.Fields["AuthenticationKeys"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PushNotificationSetupEnvironment Environment
		{
			get
			{
				return (PushNotificationSetupEnvironment)(base.Fields["Environment"] ?? PushNotificationSetupEnvironment.None);
			}
			set
			{
				base.Fields["Environment"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AcsUser
		{
			get
			{
				return (string)base.Fields["AcsUser"];
			}
			set
			{
				base.Fields["AcsUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDedicated
		{
			get
			{
				return (bool)(base.Fields["IsDedicated"] ?? false);
			}
			set
			{
				base.Fields["IsDedicated"] = value;
			}
		}

		private PushNotificationSetupConfig PushNotificationSetupConfiguration { get; set; }

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			this.PushNotificationSetupConfiguration = PushNotificationCannedSet.PushNotificationSetupEnvironmentConfig[this.Environment];
			foreach (string name in this.ResolveRetiredApps())
			{
				PushNotificationApp pushNotificationApp = this.FindPushNotificationApp(name);
				if (pushNotificationApp != null)
				{
					this.RemovePushNotificationApp(pushNotificationApp);
				}
			}
			foreach (PushNotificationApp pushNotificationApp2 in this.ResolveInstallableBySetupApps())
			{
				PushNotificationApp pushNotificationApp3 = this.FindPushNotificationApp(pushNotificationApp2.Name);
				if (pushNotificationApp3 != null && pushNotificationApp3.Platform != pushNotificationApp2.Platform)
				{
					base.WriteVerbose(Strings.PushNotificationAppPlatformMismatch(pushNotificationApp3.Name, pushNotificationApp3.ToFullString()));
					this.RemovePushNotificationApp(pushNotificationApp3);
					pushNotificationApp3 = null;
				}
				if (pushNotificationApp3 == null)
				{
					base.WriteVerbose(Strings.PushNotificationAppNotFound(pushNotificationApp2.Name));
					pushNotificationApp3 = new PushNotificationApp();
					pushNotificationApp3.CopyChangesFrom(pushNotificationApp2);
					this.SavePushNotificationApp(pushNotificationApp3);
				}
				else
				{
					base.WriteVerbose(Strings.PushNotificationAppFound(pushNotificationApp2.Name, pushNotificationApp3.ToFullString()));
					SecureString authKey;
					if (this.TryGetAuthenticationKeyFromParameters(pushNotificationApp3, out authKey))
					{
						this.SetAuthenticationKey(pushNotificationApp3, authKey);
						this.UpdatePushNotificationApp(pushNotificationApp3);
					}
					else if (PushNotificationPlatform.APNS == pushNotificationApp3.Platform && (pushNotificationApp3.AuthenticationKey == null || !pushNotificationApp3.AuthenticationKey.Equals(pushNotificationApp2.AuthenticationKey)))
					{
						this.CopyAuthenticationKeys(pushNotificationApp2, pushNotificationApp3);
						this.UpdatePushNotificationApp(pushNotificationApp3);
					}
				}
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return base.ReadWriteRootOrgGlobalConfigSession;
		}

		private PushNotificationApp[] ResolveInstallableBySetupApps()
		{
			if (!this.IsDedicated)
			{
				return this.PushNotificationSetupConfiguration.InstallableBySetup;
			}
			return this.PushNotificationSetupConfiguration.InstallableBySetupDedicated;
		}

		private string[] ResolveRetiredApps()
		{
			if (!this.IsDedicated)
			{
				return this.PushNotificationSetupConfiguration.RetiredBySetup;
			}
			return this.PushNotificationSetupConfiguration.RetiredBySetupDedicated;
		}

		private void SavePushNotificationApp(PushNotificationApp app)
		{
			app.SetId(base.ReadWriteRootOrgGlobalConfigSession, app.Name);
			SecureString authKey;
			if (this.TryGetAuthenticationKeyFromParameters(app, out authKey))
			{
				this.SetAuthenticationKey(app, authKey);
			}
			switch (app.Platform)
			{
			case PushNotificationPlatform.Azure:
				this.PrepareAzureApp(app);
				break;
			case PushNotificationPlatform.AzureHubCreation:
				this.PrepareHubCreationApp(app);
				break;
			}
			this.UpdatePushNotificationApp(app);
		}

		private void UpdatePushNotificationApp(PushNotificationApp app)
		{
			base.ReadWriteRootOrgGlobalConfigSession.Save(app);
		}

		private void RemovePushNotificationApp(PushNotificationApp app)
		{
			base.ReadWriteRootOrgGlobalConfigSession.Delete(app);
		}

		private PushNotificationApp FindPushNotificationApp(string name)
		{
			return new PushNotificationAppIdParameter(name).GetObjects<PushNotificationApp>(null, base.ReadWriteRootOrgGlobalConfigSession).FirstOrDefault<PushNotificationApp>();
		}

		private bool TryGetAuthenticationKeyFromParameters(PushNotificationApp app, out SecureString authenticationKey)
		{
			authenticationKey = null;
			if (app.Platform == PushNotificationPlatform.Azure)
			{
				return false;
			}
			if (this.AuthenticationKeys != null && this.AuthenticationKeys.ContainsKey(app.Name))
			{
				authenticationKey = (this.AuthenticationKeys[app.Name] as SecureString);
			}
			return authenticationKey != null;
		}

		private void SetAuthenticationKey(PushNotificationApp app, SecureString authKey)
		{
			string authenticationKey = string.Empty;
			try
			{
				authenticationKey = this.dkm.Encrypt(authKey.AsUnsecureString());
			}
			catch (PushNotificationConfigurationException ex)
			{
				base.WriteWarning(Strings.PushNotificationAppSecretEncryptionWarning(app.Name), (ex.InnerException == null) ? ex.Message : ex.InnerException.Message);
			}
			app.AuthenticationKey = authenticationKey;
			app.IsAuthenticationKeyEncrypted = new bool?(true);
		}

		private void CopyAuthenticationKeys(PushNotificationApp srcApp, PushNotificationApp destApp)
		{
			destApp.AuthenticationKey = srcApp.AuthenticationKey;
			destApp.AuthenticationKeyFallback = srcApp.AuthenticationKeyFallback;
		}

		private void PrepareAzureApp(PushNotificationApp app)
		{
			string partitionName;
			app.IsDefaultPartitionName = new bool?(!this.ResolvePartitionName(app, out partitionName));
			app.PartitionName = partitionName;
			app.UriTemplate = string.Format("https://{{0}}-{{1}}.servicebus.windows.net/{0}/{{2}}/{{3}}", this.PushNotificationSetupConfiguration.AcsHierarchyNode);
			this.SetAuthenticationKey(app, AzureSasKey.GenerateRandomKey(AzureSasKey.ClaimType.Send | AzureSasKey.ClaimType.Listen | AzureSasKey.ClaimType.Manage, null).KeyValue);
		}

		private void PrepareHubCreationApp(PushNotificationApp app)
		{
			app.AuthenticationId = this.AcsUser;
			app.UriTemplate = string.Format("https://{{0}}-{{1}}.servicebus.windows.net/{0}/{{2}}{{3}}", this.PushNotificationSetupConfiguration.AcsHierarchyNode);
			app.Url = "https://{0}-{1}-sb.accesscontrol.windows.net/";
			app.SecondaryUrl = string.Format("https://{{0}}-{{1}}.servicebus.windows.net/{0}", this.PushNotificationSetupConfiguration.AcsHierarchyNode);
		}

		private bool ResolvePartitionName(PushNotificationApp app, out string partitionName)
		{
			bool flag = false;
			partitionName = null;
			string text = this.ResolveCurrentForestName();
			Uri url = new Uri(string.Format("https://{0}-{1}.servicebus.windows.net", AzureUriTemplate.ConvertAppIdToValidNamespace(app.Name), text));
			using (HttpClient httpClient = new HttpClient())
			{
				DownloadResult downloadResult = httpClient.EndDownload(httpClient.BeginDownload(url, new HttpSessionConfig(2000)
				{
					Method = "GET"
				}, null, null));
				flag = downloadResult.IsSucceeded;
				if (downloadResult.ResponseStream != null)
				{
					downloadResult.ResponseStream.Dispose();
				}
			}
			if (flag)
			{
				partitionName = text;
				base.WriteVerbose(Strings.PushNotificationSucceededToValidatePartition(app.Name, partitionName));
			}
			else
			{
				if (this.PushNotificationSetupConfiguration.FallbackPartitionMapping.ContainsKey(app.Name))
				{
					partitionName = this.PushNotificationSetupConfiguration.FallbackPartitionMapping[app.Name];
				}
				this.WriteWarning(Strings.PushNotificationFailedToValidatePartitionWarning(app.Name, text, partitionName));
			}
			if (string.IsNullOrWhiteSpace(partitionName))
			{
				base.WriteError(new CannotResolveFallbackPartition(app.Name, text), ExchangeErrorCategory.ServerOperation, null);
			}
			return flag;
		}

		private string ResolveCurrentForestName()
		{
			return base.ReadWriteRootOrgGlobalConfigSession.GetRootDomainNamingContext().Name;
		}

		private const string NamespaceCheckTemplate = "https://{0}-{1}.servicebus.windows.net";

		private const string UriTemplateModel = "https://{{0}}-{{1}}.servicebus.windows.net/{0}/{{2}}/{{3}}";

		private const string AcsScopeUriTemplateModel = "https://{{0}}-{{1}}.servicebus.windows.net/{0}";

		private const string HubCreationUriTemplateModel = "https://{{0}}-{{1}}.servicebus.windows.net/{0}/{{2}}{{3}}";

		private PushNotificationDataProtector dkm = new PushNotificationDataProtector(null);
	}
}
