using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class TestWebServicesTaskBase : Task
	{
		internal ITopologyConfigurationSession ConfigSession { get; private set; }

		internal IRecipientSession RecipientSession { get; private set; }

		private protected Server LocalServer { protected get; private set; }

		private protected string ClientAccessServerFqdn { protected get; private set; }

		private protected string AutoDiscoverServerFqdn { protected get; private set; }

		internal UserWithCredential TestAccount { get; private set; }

		private protected string AutoDiscoverUrl { protected get; private set; }

		protected abstract string CmdletName { get; }

		protected abstract bool IsOutlookProvider { get; }

		protected bool IsFromAutoDiscover
		{
			get
			{
				return !this.MonitoringContext.IsPresent && this.ClientAccessServer == null;
			}
		}

		protected string UserAgentString
		{
			get
			{
				return string.Format("{0}/{1}/{2}", this.LocalServer.Name, this.CmdletName, this.TestAccount.User.PrimarySmtpAddress);
			}
		}

		[Parameter(ParameterSetName = "MonitoringContextParameterSet", Mandatory = true)]
		public SwitchParameter MonitoringContext
		{
			get
			{
				return (SwitchParameter)(base.Fields["MonitoringContext"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "ClientAccessServerParameterSet", Mandatory = false, ValueFromPipeline = true)]
		public ClientAccessServerIdParameter ClientAccessServer
		{
			get
			{
				return (ClientAccessServerIdParameter)base.Fields["ClientAccessServer"];
			}
			set
			{
				base.Fields["ClientAccessServer"] = value;
			}
		}

		[Parameter(ParameterSetName = "AutoDiscoverServerParameterSet", Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public ClientAccessServerIdParameter AutoDiscoverServer
		{
			get
			{
				return (ClientAccessServerIdParameter)base.Fields["AutoDiscoverServer"];
			}
			set
			{
				base.Fields["AutoDiscoverServer"] = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
		public MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential MailboxCredential
		{
			get
			{
				return (PSCredential)base.Fields["MailboxCredential"];
			}
			set
			{
				base.Fields["MailboxCredential"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter TrustAnySSLCertificate
		{
			get
			{
				return (SwitchParameter)(base.Fields["TrustAnySSLCertificate"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["TrustAnySSLCertificate"] = value;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e) || e is CasHealthInstructResetCredentialsException;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.ConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 259, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AutoDiscover\\TestWebServicesTaskBase.cs");
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false);
			this.RecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, ConfigScopes.TenantSubTree, 270, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AutoDiscover\\TestWebServicesTaskBase.cs");
			this.LocalServer = this.ConfigSession.FindLocalServer();
			this.ResolveAutoDiscoverServerFqdn();
			base.WriteVerbose(Strings.VerboseTestSourceServer(this.LocalServer.Fqdn));
			base.WriteVerbose(Strings.VerboseTestSourceSite(this.LocalServer.ServerSite.ToString()));
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Identity != null && this.MailboxCredential == null)
			{
				base.WriteError(new TestWebServicesTaskException(Strings.ErrorCredentialRequiredForIdentity(this.Identity.RawIdentity)), ErrorCategory.InvalidArgument, null);
			}
			this.ResolveTestAccount();
			base.WriteVerbose(Strings.VerboseTestUserIdentity(this.TestAccount.User.Identity.ToString()));
			base.WriteVerbose(Strings.VerboseTestUserAddress(this.TestAccount.User.PrimarySmtpAddress.ToString()));
			base.WriteVerbose(Strings.VerboseTestUserOrganization(this.TestAccount.User.OrganizationId.ToString()));
			this.ResolveClientAccessServerFqdn();
			this.ResolveAutoDiscoverUri();
		}

		protected override void InternalProcessRecord()
		{
			this.OutputMonitoringData();
		}

		protected bool ValidateAutoDiscover(out string ewsUrl, out string oabUrl)
		{
			AutoDiscoverValidator autoDiscoverValidator = new AutoDiscoverValidator(this.AutoDiscoverUrl, this.TestAccount.Credential, this.TestAccount.User.PrimarySmtpAddress.ToString())
			{
				VerboseDelegate = new Task.TaskVerboseLoggingDelegate(base.WriteVerbose),
				UserAgent = this.UserAgentString,
				IgnoreSslCertError = (this.TrustAnySSLCertificate || this.MonitoringContext.IsPresent),
				Provider = (this.IsOutlookProvider ? AutoDiscoverValidator.ProviderSchema.Outlook : AutoDiscoverValidator.ProviderSchema.Soap)
			};
			WebServicesTestOutcome.TestScenario scenario = this.IsOutlookProvider ? WebServicesTestOutcome.TestScenario.AutoDiscoverOutlookProvider : WebServicesTestOutcome.TestScenario.AutoDiscoverSoapProvider;
			bool flag = autoDiscoverValidator.Invoke();
			WebServicesTestOutcome outcome = new WebServicesTestOutcome
			{
				Scenario = scenario,
				Source = this.LocalServer.Fqdn,
				Result = (flag ? CasTransactionResultEnum.Success : CasTransactionResultEnum.Failure),
				Error = string.Format("{0}", autoDiscoverValidator.Error),
				ServiceEndpoint = TestWebServicesTaskBase.FqdnFromUrl(this.AutoDiscoverUrl),
				Latency = autoDiscoverValidator.Latency,
				ScenarioDescription = TestWebServicesTaskBase.GetScenarioDescription(scenario),
				Verbose = autoDiscoverValidator.Verbose
			};
			this.Output(outcome);
			ewsUrl = autoDiscoverValidator.EwsUrl;
			oabUrl = autoDiscoverValidator.OabUrl;
			return flag;
		}

		private void ResolveTestAccount()
		{
			UserWithCredential testAccount;
			if (this.Identity != null)
			{
				testAccount = default(UserWithCredential);
				testAccount.User = this.GetUniqueADObject<ADUser>(this.Identity, this.RecipientSession, true);
				testAccount.Credential = this.MailboxCredential.GetNetworkCredential();
			}
			else
			{
				testAccount = this.GetMonitoringAccount();
			}
			if (SmtpAddress.IsValidSmtpAddress(testAccount.Credential.UserName))
			{
				testAccount.Credential.Domain = null;
			}
			else if (Datacenter.IsMultiTenancyEnabled())
			{
				testAccount.Credential.UserName = string.Format("{0}@{1}", testAccount.Credential.UserName, testAccount.Credential.Domain);
				testAccount.Credential.Domain = null;
			}
			this.TestAccount = testAccount;
		}

		private UserWithCredential GetMonitoringAccount()
		{
			UserWithCredential result = default(UserWithCredential);
			string datacenter = Datacenter.IsMicrosoftHostedOnly(true) ? ".Datacenter" : string.Empty;
			ADSite localSite = this.ConfigSession.GetLocalSite();
			try
			{
				result = CommonTestTasks.GetDefaultTestAccount(new CommonTestTasks.ClientAccessContext
				{
					Instance = this,
					MonitoringContext = true,
					ConfigurationSession = this.ConfigSession,
					RecipientSession = this.RecipientSession,
					Site = localSite,
					WindowsDomain = localSite.Id.DomainId.ToCanonicalName()
				});
			}
			catch (Exception ex)
			{
				base.WriteError(new TestWebServicesTaskException(Strings.ErrorTestMailboxNotFound(ExchangeSetupContext.ScriptPath, datacenter, ex.ToString())), ErrorCategory.InvalidData, null);
			}
			if (string.IsNullOrEmpty(result.Credential.Password))
			{
				base.WriteError(new TestWebServicesTaskException(Strings.ErrorTestMailboxPasswordNotFound(result.User.Identity.ToString(), ExchangeSetupContext.ScriptPath, datacenter)), ErrorCategory.InvalidData, null);
			}
			return result;
		}

		private void ResolveAutoDiscoverServerFqdn()
		{
			this.AutoDiscoverServerFqdn = null;
			if (this.AutoDiscoverServer == null)
			{
				return;
			}
			this.AutoDiscoverServerFqdn = this.AutoDiscoverServer.ToString();
			Server uniqueADObject = this.GetUniqueADObject<Server>(this.AutoDiscoverServer, this.ConfigSession, false);
			if (uniqueADObject != null)
			{
				if (!uniqueADObject.IsClientAccessServer)
				{
					base.WriteError(new TestWebServicesTaskException(Strings.ErrorServerNotCAS(uniqueADObject.Id.ToString())), ErrorCategory.InvalidArgument, null);
				}
				this.AutoDiscoverServerFqdn = uniqueADObject.Fqdn;
			}
		}

		private void ResolveClientAccessServerFqdn()
		{
			this.ClientAccessServerFqdn = null;
			if (this.ClientAccessServer == null)
			{
				return;
			}
			this.ClientAccessServerFqdn = this.ClientAccessServer.ToString();
			Server uniqueADObject = this.GetUniqueADObject<Server>(this.ClientAccessServer, this.ConfigSession, false);
			if (uniqueADObject != null)
			{
				if (!uniqueADObject.IsClientAccessServer)
				{
					base.WriteError(new TestWebServicesTaskException(Strings.ErrorServerNotCAS(uniqueADObject.Id.ToString())), ErrorCategory.InvalidArgument, null);
				}
				this.ClientAccessServerFqdn = uniqueADObject.Fqdn;
			}
		}

		private void ResolveAutoDiscoverUri()
		{
			if (this.MonitoringContext.IsPresent)
			{
				this.AutoDiscoverUrl = this.FormatAutoDiscoverUrl(this.LocalServer.Fqdn);
				base.WriteVerbose(Strings.VerboseBuildAutoDiscoverUrl(this.AutoDiscoverUrl));
				return;
			}
			if (this.ClientAccessServerFqdn != null)
			{
				this.AutoDiscoverUrl = this.FormatAutoDiscoverUrl(this.ClientAccessServerFqdn);
				base.WriteVerbose(Strings.VerboseBuildAutoDiscoverUrl(this.AutoDiscoverUrl));
				return;
			}
			if (this.AutoDiscoverServerFqdn != null)
			{
				this.AutoDiscoverUrl = this.FormatAutoDiscoverUrl(this.AutoDiscoverServerFqdn);
				base.WriteVerbose(Strings.VerboseBuildAutoDiscoverUrl(this.AutoDiscoverUrl));
				return;
			}
			SmtpAddress primarySmtpAddress = this.TestAccount.User.PrimarySmtpAddress;
			string text = AutoDiscoverHelper.GetAutoDiscoverEndpoint(primarySmtpAddress.ToString(), this.ConfigSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			if (string.IsNullOrEmpty(text))
			{
				base.WriteError(new AutoDiscoverEndpointException(Strings.ErrorAutoDiscoverEndpointNotFound(primarySmtpAddress.Domain)), ErrorCategory.InvalidData, null);
			}
			text = this.NormalizeAutoDiscoverUrl(text);
			base.WriteVerbose(Strings.VerboseFindAutoDiscoverUrl(text, primarySmtpAddress.ToString()));
			this.AutoDiscoverUrl = text;
		}

		protected string GetSpecifiedEwsUrl()
		{
			if (this.MonitoringContext.IsPresent)
			{
				return TestWebServicesTaskBase.FormatEwsUrl(this.LocalServer.Fqdn);
			}
			if (this.ClientAccessServerFqdn != null)
			{
				return TestWebServicesTaskBase.FormatEwsUrl(this.ClientAccessServerFqdn);
			}
			return null;
		}

		protected T GetUniqueADObject<T>(ADIdParameter idParameter, IConfigDataProvider provider, bool throwWhenMissing) where T : ADObject, new()
		{
			T t = default(T);
			foreach (T t2 in idParameter.GetObjects<T>(null, provider))
			{
				if (t != null)
				{
					base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorManagementObjectAmbiguous(idParameter.ToString())), ErrorCategory.InvalidArgument, null);
				}
				t = t2;
			}
			if (t == null && throwWhenMissing)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorManagementObjectNotFound(idParameter.ToString())), ErrorCategory.InvalidArgument, null);
			}
			return t;
		}

		protected void Output(WebServicesTestOutcome outcome)
		{
			base.WriteObject(outcome);
			if (this.MonitoringContext.IsPresent)
			{
				this.outcomes.Add(outcome);
			}
		}

		protected void OutputMonitoringData()
		{
			if (this.MonitoringContext.IsPresent)
			{
				MonitoringData monitoringData = new MonitoringData();
				foreach (WebServicesTestOutcome webServicesTestOutcome in this.outcomes)
				{
					MonitoringEvent item = new MonitoringEvent("MSExchange Monitoring " + this.CmdletName, webServicesTestOutcome.MonitoringEventId, TestWebServicesTaskBase.EventTypeFromCasResultEnum(webServicesTestOutcome.Result), webServicesTestOutcome.ToString());
					monitoringData.Events.Add(item);
					if (Datacenter.IsMultiTenancyEnabled())
					{
						EventLogEntryType entryType = EventLogEntryType.Information;
						string text;
						if (webServicesTestOutcome.Result == CasTransactionResultEnum.Failure)
						{
							entryType = EventLogEntryType.Error;
							text = Strings.WebServicesTestErrorEventDetail(this.CmdletName, webServicesTestOutcome.Scenario.ToString(), webServicesTestOutcome.Result.ToString(), webServicesTestOutcome.Latency.ToString(), webServicesTestOutcome.Error, webServicesTestOutcome.Verbose);
						}
						else if (webServicesTestOutcome.Result == CasTransactionResultEnum.Skipped)
						{
							entryType = EventLogEntryType.Warning;
							text = Strings.WebServicesTestEventDetail(this.CmdletName, webServicesTestOutcome.Scenario.ToString(), webServicesTestOutcome.Result.ToString(), string.Empty);
						}
						else
						{
							text = Strings.WebServicesTestEventDetail(this.CmdletName, webServicesTestOutcome.Scenario.ToString(), webServicesTestOutcome.Result.ToString(), webServicesTestOutcome.Latency.ToString());
						}
						ExEventLog.EventTuple tuple = new ExEventLog.EventTuple((uint)webServicesTestOutcome.MonitoringEventId, 0, entryType, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);
						TestWebServicesTaskBase.EventLog.LogEvent(tuple, string.Empty, new object[]
						{
							text
						});
					}
				}
				base.WriteObject(monitoringData);
			}
		}

		protected void OutputSkippedOutcome(WebServicesTestOutcome.TestScenario scenario, LocalizedString message)
		{
			WebServicesTestOutcome outcome = new WebServicesTestOutcome
			{
				Scenario = scenario,
				Source = this.LocalServer.Fqdn,
				Result = CasTransactionResultEnum.Skipped,
				Error = message.ToString(),
				ServiceEndpoint = string.Empty,
				Latency = 0L,
				ScenarioDescription = TestWebServicesTaskBase.GetScenarioDescription(scenario)
			};
			this.Output(outcome);
		}

		protected string FormatAutoDiscoverUrl(string fqdn)
		{
			return string.Format("https://{0}/AutoDiscover/AutoDiscover.{1}", fqdn, this.IsOutlookProvider ? "xml" : "svc");
		}

		protected string NormalizeAutoDiscoverUrl(string url)
		{
			int length = url.Length;
			string text = url.Substring(length - 3, 3);
			if (!text.Equals("xml", StringComparison.OrdinalIgnoreCase) && !text.Equals("svc", StringComparison.OrdinalIgnoreCase))
			{
				return url;
			}
			return url.Substring(0, length - 3) + (this.IsOutlookProvider ? "xml" : "svc");
		}

		protected static string FormatEwsUrl(string fqdn)
		{
			return string.Format("https://{0}/ews/Exchange.asmx", fqdn);
		}

		protected static string FqdnFromUrl(string url)
		{
			Uri uri = new Uri(url);
			return uri.Host;
		}

		protected static string GetScenarioDescription(WebServicesTestOutcome.TestScenario scenario)
		{
			return LocalizedDescriptionAttribute.FromEnum(typeof(WebServicesTestOutcome.TestScenario), scenario);
		}

		private static EventTypeEnumeration EventTypeFromCasResultEnum(CasTransactionResultEnum casResult)
		{
			switch (casResult)
			{
			case CasTransactionResultEnum.Success:
				return EventTypeEnumeration.Success;
			case CasTransactionResultEnum.Failure:
				return EventTypeEnumeration.Error;
			case CasTransactionResultEnum.Skipped:
				return EventTypeEnumeration.Warning;
			default:
				return EventTypeEnumeration.Warning;
			}
		}

		protected const string MonitoringContextParameterSet = "MonitoringContextParameterSet";

		protected const string ClientAccessServerParameterSet = "ClientAccessServerParameterSet";

		protected const string AutoDiscoverServerParameterSet = "AutoDiscoverServerParameterSet";

		private static readonly ExEventLog EventLog = new ExEventLog(new Guid("400A88BD-6F94-480C-8B77-28B373BD8574"), "EWS Monitoring", "EWS Monitoring Events");

		private List<WebServicesTestOutcome> outcomes = new List<WebServicesTestOutcome>();
	}
}
