using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal sealed class ConnectionRetryDiscoverer
	{
		static ConnectionRetryDiscoverer()
		{
			string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools";
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
				{
					if (registryKey != null)
					{
						bool.TryParse(registryKey.GetValue("EMC.SkipCertificateCheck") as string, out ConnectionRetryDiscoverer.skipCertificateCheckSetting);
					}
				}
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		public ConnectionRetryDiscoverer(UIInteractionHandler uiInteractionHandler, OrganizationType orgType, string displayName, Uri uri, bool logonWithDefaultCredential, string credentialKey, PSCredential proposedCredential)
		{
			this.uiInteractionHandler = uiInteractionHandler;
			this.orgType = orgType;
			this.displayName = displayName;
			this.uri = uri;
			this.logonWithDefaultCredential = logonWithDefaultCredential;
			this.credentialKey = credentialKey;
			this.proposedCredential = proposedCredential;
		}

		public ConnectionRetryDiscoverer(IUIService uiService, OrganizationType orgType, string displayName, Uri uri, bool logonWithDefaultCredential) : this(new WindowUIInteractionHandler((Control)uiService.GetDialogOwnerWindow()), orgType, displayName, uri, logonWithDefaultCredential, null, null)
		{
		}

		public ConnectionRetryDiscoverer(PSConnectionInfoSingleton connInfoSingleton) : this((connInfoSingleton.UISyncContext == null) ? null : new SyncContextUIInteractionHandler(connInfoSingleton.UISyncContext), connInfoSingleton.Type, connInfoSingleton.DisplayName, connInfoSingleton.Uri, connInfoSingleton.LogonWithDefaultCredential, connInfoSingleton.CredentialKey, connInfoSingleton.ProposedCredential)
		{
		}

		public ConnectionRetryDiscoverer(OrganizationSetting orgSetting, IUIService uiService) : this(new WindowUIInteractionHandler((Control)uiService.GetDialogOwnerWindow()), orgSetting.Type, orgSetting.DisplayName, orgSetting.Uri, orgSetting.LogonWithDefaultCredential, orgSetting.CredentialKey, null)
		{
		}

		public MonadConnectionInfo DiscoverConnectionInfo(out SupportedVersionList supportedVersionList, string slotVersion)
		{
			if (string.IsNullOrEmpty(this.credentialKey) && !this.logonWithDefaultCredential)
			{
				throw new NotSupportedException();
			}
			bool flag = false;
			MonadConnectionInfo monadConnectionInfo = this.DiscoverConnectionInfo(ref flag, out supportedVersionList, slotVersion);
			if (flag && monadConnectionInfo.Credentials != null)
			{
				CredentialHelper.SaveCredential(this.credentialKey, monadConnectionInfo.Credentials);
			}
			return monadConnectionInfo;
		}

		public MonadConnectionInfo DiscoverConnectionInfo(ref bool rememberCredentialChecked, out SupportedVersionList supportedVersionList, string slotVersion)
		{
			this.rememberCredentialChecked = rememberCredentialChecked;
			this.slotVersion = slotVersion;
			MonadConnectionInfo result = this.DiscoverConnectionInfoInternal();
			rememberCredentialChecked = this.rememberCredentialChecked;
			supportedVersionList = this.supportedVersionList;
			return result;
		}

		private MonadConnectionInfo DiscoverConnectionInfoInternal()
		{
			if (!this.logonWithDefaultCredential)
			{
				return this.RetryCredentialsFromLoadOrInput((PSCredential cred) => this.TryConnectWithExplicitCredential(cred));
			}
			MonadConnectionInfo monadConnectionInfo = this.TryConnectWithDefaultCredential();
			if (monadConnectionInfo != null)
			{
				return monadConnectionInfo;
			}
			throw new OperationCanceledException(this.GetErrorMessage());
		}

		private MonadConnectionInfo RetryCredentialsFromLoadOrInput(Func<PSCredential, MonadConnectionInfo> tryConnectWithCredential)
		{
			PSCredential cred = this.proposedCredential;
			if (cred == null && !string.IsNullOrEmpty(this.credentialKey))
			{
				cred = CredentialHelper.ReadCredential(this.credentialKey);
			}
			MonadConnectionInfo monadConnectionInfo = null;
			while (monadConnectionInfo == null)
			{
				if (cred == null && this.uiInteractionHandler != null)
				{
					if (WinformsHelper.IsWin7OrLater() && this.orgType == OrganizationType.Cloud)
					{
						CredentialHelper.ForceConnection(this.uri);
					}
					this.uiInteractionHandler.DoActionSynchronizely(delegate(IWin32Window window)
					{
						cred = CredentialHelper.PromptForCredentials((window == null) ? IntPtr.Zero : window.Handle, this.displayName, this.uri, this.GetErrorMessage(), ref this.rememberCredentialChecked);
					});
					if (cred != null && !string.IsNullOrEmpty(this.credentialKey))
					{
						CredentialHelper.RemoveCredential(this.credentialKey);
					}
				}
				if (cred == null)
				{
					throw new OperationCanceledException(Strings.UnableToConnectExchangeForest(this.displayName));
				}
				monadConnectionInfo = tryConnectWithCredential(cred);
				cred = null;
			}
			return monadConnectionInfo;
		}

		private MonadConnectionInfo TryConnectWithDefaultCredential()
		{
			if (this.orgType == OrganizationType.Cloud)
			{
				return this.ConnectTenantWithDefaultCredential();
			}
			return this.ConnectOnPremiseWithDefaultCredential();
		}

		private MonadConnectionInfo TryConnectWithExplicitCredential(PSCredential cred)
		{
			if (this.orgType == OrganizationType.Cloud)
			{
				return this.ConnectTenantWithExplicitCredential(cred);
			}
			if (this.uri.IsHttps())
			{
				return this.ConnectRemoteOnPremiseWithHttps(cred);
			}
			return this.ConnectRemoteOnPremiseWithHttp(cred);
		}

		private MonadConnectionInfo ConnectOnPremiseWithDefaultCredential()
		{
			AuthenticationMechanism[] authMechanisms = new AuthenticationMechanism[]
			{
				AuthenticationMechanism.Kerberos
			};
			return this.TryAuthenticationMechanisms(null, authMechanisms);
		}

		private MonadConnectionInfo ConnectTenantWithDefaultCredential()
		{
			AuthenticationMechanism[] authMechanisms = new AuthenticationMechanism[]
			{
				AuthenticationMechanism.NegotiateWithImplicitCredential
			};
			return this.TryAuthenticationMechanisms(null, authMechanisms);
		}

		private MonadConnectionInfo ConnectRemoteOnPremiseWithHttp(PSCredential cred)
		{
			AuthenticationMechanism[] authMechanisms = new AuthenticationMechanism[]
			{
				AuthenticationMechanism.Kerberos
			};
			return this.TryAuthenticationMechanisms(cred, authMechanisms);
		}

		private MonadConnectionInfo ConnectRemoteOnPremiseWithHttps(PSCredential cred)
		{
			AuthenticationMechanism[] authMechanisms = new AuthenticationMechanism[]
			{
				AuthenticationMechanism.Basic
			};
			return this.TryAuthenticationMechanisms(cred, authMechanisms);
		}

		private MonadConnectionInfo ConnectTenantWithExplicitCredential(PSCredential cred)
		{
			AuthenticationMechanism[] array = new AuthenticationMechanism[]
			{
				AuthenticationMechanism.Basic
			};
			AuthenticationMechanism[] array2 = new AuthenticationMechanism[]
			{
				AuthenticationMechanism.Negotiate
			};
			return this.TryAuthenticationMechanisms(cred, cred.IsLiveId() ? array : array2);
		}

		private bool CanSkipCertificateCheck()
		{
			return ConnectionRetryDiscoverer.skipCertificateCheckSetting || !this.uri.IsHttps();
		}

		private MonadConnectionInfo TryAuthenticationMechanisms(PSCredential cred, AuthenticationMechanism[] authMechanisms)
		{
			this.exceptionList.Clear();
			foreach (AuthenticationMechanism authenticationMechanism in authMechanisms)
			{
				try
				{
					bool skipCertificateCheck = this.CanSkipCertificateCheck();
					this.supportedVersionList = MonadRemoteRunspaceFactory.TestConnection(this.uri, "http://schemas.microsoft.com/powershell/Microsoft.Exchange", cred, authenticationMechanism, this.MaxRedirectionCount, skipCertificateCheck);
					return new MonadConnectionInfo(this.uri, cred, "http://schemas.microsoft.com/powershell/Microsoft.Exchange", null, authenticationMechanism, ExchangeRunspaceConfigurationSettings.SerializationLevel.Full, ExchangeRunspaceConfigurationSettings.ExchangeApplication.EMC, (this.orgType == OrganizationType.Cloud) ? this.slotVersion : string.Empty, this.MaxRedirectionCount, skipCertificateCheck);
				}
				catch (PSRemotingTransportException value)
				{
					this.exceptionList.Add(authenticationMechanism, value);
				}
				catch (PSRemotingDataStructureException value2)
				{
					this.exceptionList.Add(authenticationMechanism, value2);
				}
			}
			this.supportedVersionList = null;
			return null;
		}

		private int MaxRedirectionCount
		{
			get
			{
				if (this.orgType != OrganizationType.Cloud)
				{
					return 0;
				}
				return 3;
			}
		}

		private string GetErrorMessage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<AuthenticationMechanism, Exception> keyValuePair in this.exceptionList)
			{
				stringBuilder.AppendLine(Strings.TryAuthenticationMechanismFailedMessage(this.uri.ToString(), LocalizedDescriptionAttribute.FromEnum(typeof(AuthenticationMechanism), keyValuePair.Key), keyValuePair.Value.Message));
			}
			return stringBuilder.ToString();
		}

		private const int maxRedirectionCountForCloud = 3;

		private const int maxRedirectionCountForOnPremise = 0;

		private OrganizationType orgType;

		private string displayName;

		private Uri uri;

		private bool logonWithDefaultCredential;

		private string credentialKey;

		private PSCredential proposedCredential;

		private UIInteractionHandler uiInteractionHandler;

		private bool rememberCredentialChecked;

		private SupportedVersionList supportedVersionList;

		private string slotVersion;

		private static bool skipCertificateCheckSetting;

		private Dictionary<AuthenticationMechanism, Exception> exceptionList = new Dictionary<AuthenticationMechanism, Exception>();
	}
}
