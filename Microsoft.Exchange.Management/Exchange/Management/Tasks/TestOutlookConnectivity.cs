using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Test", "OutlookConnectivity")]
	public sealed class TestOutlookConnectivity : DataAccessTask<Server>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, HelpMessage = "The serverId parameter on which the probe should be executed; default is the local machine.")]
		public ServerIdParameter RunFromServerId { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Position = 0, Mandatory = true, HelpMessage = "The probe identity to invoke, followed by the optional [\\target resource] for certain probes. Example: 'OutlookMailboxDeepTest\\Mailbox Database XXX'.")]
		public string ProbeIdentity
		{
			get
			{
				return (string)base.Fields["ProbeIdentity"];
			}
			set
			{
				base.Fields["ProbeIdentity"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ValueFromPipeline = true, HelpMessage = "The Mailbox Id Parameter for the mailbox you want to logon.")]
		public MailboxIdParameter MailboxId
		{
			get
			{
				return (MailboxIdParameter)base.Fields["MailboxId"];
			}
			set
			{
				base.Fields["MailboxId"] = value;
			}
		}

		[Parameter(Mandatory = false, HelpMessage = "The endpoint you wish to hit.  Default value is the local to the probe's server.")]
		[ValidateNotNullOrEmpty]
		public string Hostname
		{
			get
			{
				return (string)base.Fields["Hostname"];
			}
			set
			{
				base.Fields["Hostname"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string TimeOutSeconds
		{
			get
			{
				return (string)base.Fields["TimeOutSeconds"];
			}
			set
			{
				base.Fields["TimeOutSeconds"] = value;
			}
		}

		[Parameter(Mandatory = false, HelpMessage = "The credential used to authenticate on connect.  Default value is the Monitoring Test Account credentials for that database.")]
		[ValidateNotNullOrEmpty]
		public PSCredential Credential
		{
			get
			{
				return (PSCredential)base.Fields["Credential"];
			}
			set
			{
				base.Fields["Credential"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 215, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Tasks\\TestOutlookConnectivity.cs");
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.RunFromServerId == null)
				{
					this.RunFromServerId = new ServerIdParameter(new Fqdn(ComputerInformation.DnsPhysicalFullyQualifiedDomainName));
				}
				this.server = (Server)base.GetDataObject<Server>(this.RunFromServerId, base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorServerNotFound(this.RunFromServerId.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.RunFromServerId.ToString())));
				this.WriteInfo(Strings.RunFromServer(this.RunFromServerId.ToString()));
				if (this.MailboxId != null)
				{
					this.mailboxAdUser = (ADUser)base.GetDataObject<ADUser>(this.MailboxId, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.MailboxId.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.MailboxId.ToString())));
				}
				this.WriteInfo((this.mailboxAdUser == null) ? Strings.UsingTargetMonitoringMailbox : Strings.UsingTargetMailbox(this.mailboxAdUser.PrimarySmtpAddress.ToString()));
				if (!TestOutlookConnectivity.IsMailboxCredentialEmpty(this.Credential))
				{
					if (this.MailboxId == null)
					{
						base.WriteError(new ArgumentException(Strings.MissingMailboxId(this.Credential.UserName)), (ErrorCategory)1000, null);
					}
					this.authenticateAsUser = (ADUser)base.GetDataObject<ADUser>(new RecipientIdParameter(this.Credential.UserName), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.OutlookConnectivityErrorUserNotFound(this.Credential.UserName.ToString())), new LocalizedString?(Strings.OutlookConnectivityErrorUserNotUnique(this.Credential.UserName.ToString())));
				}
				this.WriteInfo((this.Credential != null) ? Strings.UsingAuthenticationCredentials(this.Credential.UserName) : Strings.UsingMonitoringMailboxAuthenticationCredentials);
				string probeIdentity = this.ProbeIdentity;
				ProbeIdentity probeIdentity2 = null;
				try
				{
					probeIdentity2 = OutlookConnectivity.ResolveIdentity(probeIdentity, TestOutlookConnectivity.IsDcOrDedicated);
				}
				catch (ArgumentException exception)
				{
					base.WriteError(exception, (ErrorCategory)1000, null);
				}
				this.ProbeIdentity = probeIdentity2.GetIdentity(true);
				this.WriteInfo(Strings.UsingProbeIdentity(this.ProbeIdentity));
				if (!TestOutlookConnectivity.IsMailboxCredentialEmpty(this.Credential) && !this.IsProbePasswordAuthenticated)
				{
					this.WriteWarning(Strings.IgnoredAuthenticationWarning);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				Dictionary<string, string> dictionary = this.CreatePropertyBag();
				string text = null;
				if (dictionary.TryGetValue("ItemTargetExtension", out text))
				{
					dictionary.Remove("ItemTargetExtension");
				}
				string text2 = string.Empty;
				if (dictionary.Count != 0)
				{
					text2 = CrimsonHelper.ConvertDictionaryToXml(dictionary);
				}
				text = (text ?? string.Empty);
				text2 = (text2 ?? string.Empty);
				RpcInvokeMonitoringProbe.Reply reply = RpcInvokeMonitoringProbe.Invoke(this.server.Fqdn, this.ProbeIdentity, text2, text, 300000);
				if (reply != null)
				{
					if (!string.IsNullOrEmpty(reply.ErrorMessage))
					{
						throw new InvalidOperationException(reply.ErrorMessage);
					}
					MonitoringProbeResult monitoringProbeResult = new MonitoringProbeResult(this.server.Fqdn, reply.ProbeResult);
					this.WriteInfo(Strings.ProbeResult(monitoringProbeResult.ResultType.ToString()));
					if (monitoringProbeResult.ResultType != ResultType.Succeeded)
					{
						string failedProbeResultDetailsString = string.Format("Error: {0}\r\nException: {1}", monitoringProbeResult.Error, monitoringProbeResult.Exception);
						this.WriteWarning(Strings.FailedProbeResultDetails(failedProbeResultDetailsString));
					}
					base.WriteObject(monitoringProbeResult);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is InvalidVersionException || exception is InvalidIdentityException || exception is InvalidDurationException || exception is ActiveMonitoringServerException || exception is ActiveMonitoringServerTransientException || base.IsKnownException(exception);
		}

		private void WriteInfo(LocalizedString text)
		{
			base.WriteVerbose(text);
		}

		private bool IsCtpTest
		{
			get
			{
				return MonitoringItemIdentity.MonitorIdentityId.GetMonitor(this.ProbeIdentity).IndexOf("ctp", StringComparison.InvariantCultureIgnoreCase) >= 0;
			}
		}

		private bool IsProbePasswordAuthenticated
		{
			get
			{
				return this.IsCtpTest || TestOutlookConnectivity.IsDcOrDedicated;
			}
		}

		private Dictionary<string, string> CreatePropertyBag()
		{
			Dictionary<string, string> dictionary = null;
			try
			{
				dictionary = TestOutlookConnectivity.CreateTestOutlookConnectivityPropertyBag(this.mailboxAdUser, this.IsProbePasswordAuthenticated, this.IsCtpTest, this.Credential, this.authenticateAsUser, this.Hostname);
				TestOutlookConnectivity.AddToPropertyBag(dictionary, "TimeoutSeconds", this.TimeOutSeconds);
				TestOutlookConnectivity.AddToPropertyBag(dictionary, "ServiceName", MonitoringItemIdentity.MonitorIdentityId.GetHealthSet(this.ProbeIdentity));
				TestOutlookConnectivity.AddToPropertyBag(dictionary, "Name", MonitoringItemIdentity.MonitorIdentityId.GetMonitor(this.ProbeIdentity));
				TestOutlookConnectivity.AddToPropertyBag(dictionary, "TargetResource", MonitoringItemIdentity.MonitorIdentityId.GetTargetResource(this.ProbeIdentity));
			}
			catch (ArgumentException exception)
			{
				base.WriteError(exception, (ErrorCategory)1000, null);
			}
			return dictionary;
		}

		private static void AddToPropertyBag(Dictionary<string, string> propertyBag, string key, string value)
		{
			if (value != null && key != null)
			{
				propertyBag.Add(key, value);
			}
		}

		internal static Dictionary<string, string> CreateTestOutlookConnectivityPropertyBag(ADUser mailboxAdUser, bool isPasswordAuthenticated, bool isCtpTest, PSCredential mailboxCredential, ADUser adUserCorrespondingToMailboxCredential, string endpoint)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>(0);
			AutodiscoverCommonUserSettings autodiscoverCommonUserSettings = null;
			if (mailboxAdUser != null)
			{
				TestOutlookConnectivity.AddToPropertyBag(dictionary2, "MailboxLegacyDN", mailboxAdUser.LegacyExchangeDN);
				autodiscoverCommonUserSettings = AutodiscoverCommonUserSettings.GetSettingsFromRecipient(mailboxAdUser, mailboxAdUser.PrimarySmtpAddress.ToString());
				TestOutlookConnectivity.AddToPropertyBag(dictionary2, "PersonalizedServerName", string.Format("{0}@{1}", autodiscoverCommonUserSettings.MailboxGuid, autodiscoverCommonUserSettings.PrimarySmtpAddress.Domain));
				if (!TestOutlookConnectivity.IsMailboxCredentialEmpty(mailboxCredential))
				{
					if (adUserCorrespondingToMailboxCredential == null)
					{
						throw new ArgumentException("This should never happen.  If mailboxCredential is passed in, then adUserCorrespondingToMailboxCredential should match.");
					}
					TestOutlookConnectivity.AddToPropertyBag(dictionary, "Account", TestOutlookConnectivity.GetAccountLoginName(adUserCorrespondingToMailboxCredential));
					TestOutlookConnectivity.AddToPropertyBag(dictionary2, "AccountLegacyDN", adUserCorrespondingToMailboxCredential.LegacyExchangeDN);
					if (isPasswordAuthenticated)
					{
						string value = TestOutlookConnectivity.ConvertSecureStringToPlainString(mailboxCredential.Password);
						TestOutlookConnectivity.AddToPropertyBag(dictionary, "Password", value);
					}
				}
			}
			TestOutlookConnectivity.AddToPropertyBag(dictionary, "Endpoint", endpoint);
			if (!isCtpTest)
			{
				TestOutlookConnectivity.AddToPropertyBag(dictionary, "SecondaryEndpoint", endpoint);
			}
			if (autodiscoverCommonUserSettings != null && !string.IsNullOrEmpty(autodiscoverCommonUserSettings.RpcServer) && isCtpTest)
			{
				TestOutlookConnectivity.AddToPropertyBag(dictionary, "SecondaryEndpoint", autodiscoverCommonUserSettings.RpcServer);
			}
			TestOutlookConnectivity.AddToPropertyBag(dictionary, "ItemTargetExtension", WorkDefinition.SerializeExtensionAttributes(dictionary2));
			return dictionary;
		}

		private static string GetAccountLoginName(ADUser accountAdUser)
		{
			if (Datacenter.IsLiveIDForExchangeLogin(true))
			{
				return accountAdUser.WindowsLiveID.ToString();
			}
			return accountAdUser.UserPrincipalName;
		}

		private static bool IsMailboxCredentialEmpty(PSCredential credential)
		{
			return credential == null || string.IsNullOrEmpty(credential.UserName) || credential == PSCredential.Empty;
		}

		private static string ConvertSecureStringToPlainString(SecureString secureString)
		{
			if (secureString == null)
			{
				return null;
			}
			IntPtr intPtr = Marshal.SecureStringToBSTR(secureString);
			string result;
			try
			{
				result = Marshal.PtrToStringUni(intPtr);
			}
			finally
			{
				Marshal.ZeroFreeBSTR(intPtr);
			}
			return result;
		}

		public const string ItemTargetExtensionPropBagEntry = "ItemTargetExtension";

		public const string TimeoutSecondsPropBagEntry = "TimeoutSeconds";

		public const string HealthSetServiceNamePropBagEntry = "ServiceName";

		public const string ProbeNamePropBagEntry = "Name";

		public const string TargetResourcePropBagEntry = "TargetResource";

		public const string PasswordPropBagEntry = "Password";

		public const string AccountPropBagEntry = "Account";

		public const string AccountLegacyDNAttributePropBagEntry = "AccountLegacyDN";

		public const string PersonalizedServernameAttributePropBagEntry = "PersonalizedServerName";

		public const string MailboxLegacyDNAttributePropBagEntry = "MailboxLegacyDN";

		public const string EndpointPropBagEntry = "Endpoint";

		public const string SecondaryEndpointPropBagEntry = "SecondaryEndpoint";

		private Server server;

		private ADUser mailboxAdUser;

		private ADUser authenticateAsUser;

		private static bool IsDcOrDedicated = VariantConfiguration.InvariantNoFlightingSnapshot.Global.DistributedKeyManagement.Enabled;
	}
}
