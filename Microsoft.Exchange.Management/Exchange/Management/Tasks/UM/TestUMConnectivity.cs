using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Test", "UMConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "LocalLoop")]
	public sealed class TestUMConnectivity : Task
	{
		private TestUMConnectivityHelper.LocalUMConnectivityOptionsResults LocalCallRouterResult
		{
			get
			{
				return (TestUMConnectivityHelper.LocalUMConnectivityOptionsResults)this.result;
			}
		}

		private TestUMConnectivityHelper.LocalUMConnectivityResults LocalBackendResult
		{
			get
			{
				return (TestUMConnectivityHelper.LocalUMConnectivityResults)this.result;
			}
		}

		[Parameter]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonSpecific")]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonGeneral")]
		[Parameter(Mandatory = false, ParameterSetName = "PinReset")]
		[Parameter(Mandatory = false, ParameterSetName = "LocalLoop")]
		public bool MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonSpecific")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "LocalLoop")]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonGeneral")]
		public int ListenPort
		{
			get
			{
				return (int)((base.Fields["ListenPort"] == null) ? 0 : base.Fields["ListenPort"]);
			}
			set
			{
				base.Fields["ListenPort"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonGeneral")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonSpecific")]
		[Parameter(Mandatory = false, ParameterSetName = "LocalLoop")]
		public int RemotePort
		{
			get
			{
				return (int)((base.Fields["RemotePort"] == null) ? 0 : base.Fields["RemotePort"]);
			}
			set
			{
				base.Fields["RemotePort"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonSpecific")]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonGeneral")]
		[Parameter(Mandatory = false, ParameterSetName = "LocalLoop")]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		public bool Secured
		{
			get
			{
				return (bool)((base.Fields["Secured"] == null) ? false : base.Fields["Secured"]);
			}
			set
			{
				base.Fields["Secured"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonSpecific")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "LocalLoop")]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonGeneral")]
		public string CertificateThumbprint
		{
			get
			{
				return (string)base.Fields["CertificateThumbprint"];
			}
			set
			{
				base.Fields["CertificateThumbprint"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonSpecific")]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		[Parameter(Mandatory = false, ParameterSetName = "LocalLoop")]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonGeneral")]
		[ValidateNotNullOrEmpty]
		public bool MediaSecured
		{
			get
			{
				return (bool)((base.Fields["MediaSecured"] == null) ? false : base.Fields["MediaSecured"]);
			}
			set
			{
				base.Fields["MediaSecured"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		[ValidateNotNullOrEmpty]
		public int DiagInitialSilenceInMilisecs
		{
			get
			{
				return (int)((base.Fields["DiagInitialSilenceInMilisecs"] == null) ? 0 : base.Fields["DiagInitialSilenceInMilisecs"]);
			}
			set
			{
				base.Fields["DiagInitialSilenceInMilisecs"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		public string DiagDtmfSequence
		{
			get
			{
				return (string)((base.Fields["DiagDtmfSequence"] == null) ? "ABCD*#0123456789" : base.Fields["DiagDtmfSequence"]);
			}
			set
			{
				base.Fields["DiagDtmfSequence"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		[ValidateNotNullOrEmpty]
		public int DiagInterDtmfGapInMilisecs
		{
			get
			{
				return (int)((base.Fields["DiagInterDtmfGapInMilisecs"] == null) ? 0 : base.Fields["DiagInterDtmfGapInMilisecs"]);
			}
			set
			{
				base.Fields["DiagInterDtmfGapInMilisecs"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		public int DiagDtmfDurationInMilisecs
		{
			get
			{
				return (int)((base.Fields["DiagDtmfDurationInMilisecs"] == null) ? 0 : base.Fields["DiagDtmfDurationInMilisecs"]);
			}
			set
			{
				base.Fields["DiagDtmfDurationInMilisecs"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		public string DiagInterDtmfDiffGapInMilisecs
		{
			get
			{
				return (string)((base.Fields["DiagInterDtmfDiffGapInMilisecs"] == null) ? string.Empty : base.Fields["DiagInterDtmfDiffGapInMilisecs"]);
			}
			set
			{
				base.Fields["DiagInterDtmfDiffGapInMilisecs"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonSpecific")]
		[Parameter(Mandatory = false, ParameterSetName = "LocalLoop")]
		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		[Parameter(Mandatory = false, ParameterSetName = "TuiLogonGeneral")]
		public int Timeout
		{
			get
			{
				return (int)base.Fields["Timeout"];
			}
			set
			{
				base.Fields["Timeout"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EndToEnd")]
		[ValidateNotNullOrEmpty]
		public UMIPGatewayIdParameter UMIPGateway
		{
			get
			{
				return (UMIPGatewayIdParameter)base.Fields["IPGateway"];
			}
			set
			{
				base.Fields["IPGateway"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "TuiLogonSpecific")]
		[Parameter(Mandatory = true, ParameterSetName = "EndToEnd")]
		[ValidateNotNullOrEmpty]
		public string Phone
		{
			get
			{
				return (string)base.Fields["Phone"];
			}
			set
			{
				base.Fields["Phone"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EndToEnd")]
		[ValidateNotNullOrEmpty]
		public string From
		{
			get
			{
				return (string)base.Fields["From"];
			}
			set
			{
				base.Fields["From"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "TuiLogonSpecific")]
		[ValidateNotNullOrEmpty]
		public string PIN
		{
			get
			{
				return (string)base.Fields["PIN"];
			}
			set
			{
				base.Fields["PIN"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "TuiLogonSpecific")]
		public UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["UMDialPlan"];
			}
			set
			{
				base.Fields["UMDialPlan"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "TuiLogonSpecific")]
		public bool TUILogon
		{
			get
			{
				return (bool)((base.Fields["TUILogon"] == null) ? false : base.Fields["TUILogon"]);
			}
			set
			{
				base.Fields["TUILogon"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "TuiLogonGeneral")]
		public bool TUILogonAll
		{
			get
			{
				return (bool)((base.Fields["TUILogonAll"] == null) ? false : base.Fields["TUILogonAll"]);
			}
			set
			{
				base.Fields["TUILogonAll"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PinReset")]
		[ValidateNotNullOrEmpty]
		public bool ResetPIN
		{
			get
			{
				return (bool)((base.Fields["ResetPIN"] == null) ? false : base.Fields["ResetPIN"]);
			}
			set
			{
				base.Fields["ResetPIN"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "LocalLoop")]
		public SwitchParameter CallRouter
		{
			get
			{
				return (SwitchParameter)(base.Fields["CallRouter"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CallRouter"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("EndToEnd" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageTestUMConnectivityEndToEnd(this.UMIPGateway.ToString(), this.Phone.ToString());
				}
				if ("PinReset" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageTestUMConnectivityPinReset;
				}
				if ("TuiLogonGeneral" == base.ParameterSetName || "TuiLogonSpecific" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageTestUMConnectivityTUILocalLoop;
				}
				return Strings.ConfirmationMessageTestUMConnectivityLocalLoop;
			}
		}

		protected override void InternalStopProcessing()
		{
			TaskLogger.LogEnter();
			if (this.umconnection != null)
			{
				this.umconnection.EndCall();
				this.umconnection.Shutdown();
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.WriteErrorIfMissingUCMA();
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 784, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\TestUMConnectivity.cs");
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			this.globalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, sessionSettings, 791, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\TestUMConnectivity.cs");
			if (!this.globalCatalogSession.IsReadConnectionAvailable())
			{
				this.globalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 800, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\TestUMConnectivity.cs");
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				this.ipGateway = this.UMIPGateway;
				this.dialPlan = this.UMDialPlan;
				this.localIPAddress = Utils.GetLocalIPv4Address().ToString();
				this.localLoopTarget = LocalLoopTargetStrategy.Create(this.configurationSession, this.CallRouter);
				base.InternalValidate();
				this.DoOwnValidate();
			}
			catch (LocalizedException ex)
			{
				TestUMConnectivity.EventId id = TestUMConnectivity.EventId.TestExecuteError;
				if (ex is DataSourceTransientException || ex is DataSourceOperationException || ex is DataValidationException)
				{
					id = TestUMConnectivity.EventId.ADError;
				}
				else if (ex is LocalServerNotFoundException || ex is ExchangeServerNotFoundException || ex is SIPFEServerConfigurationNotFoundException || ex is ServiceNotStarted || ex is UMServiceDisabled)
				{
					id = TestUMConnectivity.EventId.ServiceNotInstalledOrRunning;
				}
				this.HandleValidationFailed(ex, id);
			}
			finally
			{
				if (base.HasErrors && this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.startTime = ExDateTime.UtcNow;
			try
			{
				if (this.taskMode == TestUMConnectivity.Mode.ResetPin)
				{
					this.ExecutePinResetTest();
				}
				else if (this.taskMode == TestUMConnectivity.Mode.LocalLoopTUILogonAll)
				{
					this.ExecuteAutoEnumerateTest();
				}
				else if (this.taskMode == TestUMConnectivity.Mode.EndToEndTest)
				{
					this.ExecuteRemoteTest();
				}
				else if (this.CallRouter)
				{
					this.ExecuteCallRouterTest();
				}
				else
				{
					this.ExecuteTest();
				}
			}
			finally
			{
				if (this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this.umconnection != null)
				{
					this.umconnection.Shutdown();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private static bool UserUMEnabled(ADUser user)
		{
			return user != null && UMSubscriber.IsValidSubscriber(user);
		}

		private bool IsMailboxLocal(ExchangePrincipal principal)
		{
			return string.Equals(this.localLoopTarget.Server.Fqdn, principal.MailboxInfo.Location.ServerFqdn, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsParameterSpecified(string parameterName)
		{
			return base.Fields.IsModified(parameterName);
		}

		private void ExecutePinResetTest()
		{
			foreach (TestUMConnectivityHelper.UsersForResetPin usersForResetPin in this.pinResetUsers)
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (!UmConnectivityCredentialsHelper.ResetMailboxPassword(usersForResetPin.ExPrincipal, usersForResetPin.NetCreds))
				{
					this.HandlePinResetScenarioOutCome(usersForResetPin, TestUMConnectivity.PinResetOutcome.PasswordResetFailed, utcNow);
				}
				else if (!UmConnectivityCredentialsHelper.ResetUMPin(usersForResetPin.Aduser, usersForResetPin.NetCreds.Password))
				{
					this.HandlePinResetScenarioOutCome(usersForResetPin, TestUMConnectivity.PinResetOutcome.PinResetFailed, utcNow);
				}
				else
				{
					this.HandlePinResetScenarioOutCome(usersForResetPin, TestUMConnectivity.PinResetOutcome.Success, utcNow);
				}
			}
			this.WriteResetPinTestResults();
		}

		private void HandlePinResetScenarioOutCome(TestUMConnectivityHelper.UsersForResetPin user, TestUMConnectivity.PinResetOutcome outcome, ExDateTime time)
		{
			TimeSpan timeSpan = ExDateTime.UtcNow.Subtract(time);
			TestUMConnectivityHelper.ResetPinResults resetPinResults = new TestUMConnectivityHelper.ResetPinResults();
			resetPinResults.Latency = timeSpan.TotalMilliseconds;
			resetPinResults.MailboxServerBeingTested = user.MailboxServer;
			string mailboxServer = user.MailboxServer;
			double performanceValue = resetPinResults.Latency;
			switch (outcome)
			{
			case TestUMConnectivity.PinResetOutcome.PasswordResetFailed:
				resetPinResults.SuccessfullyResetPasswordForValidUMUser = false;
				resetPinResults.SuccessfullyResetPINForValidUMUser = false;
				performanceValue = -1.0;
				this.pinResetTestsListOfServersFailedToResetPasswd = this.pinResetTestsListOfServersFailedToResetPasswd + user.MailboxServer + ", ";
				break;
			case TestUMConnectivity.PinResetOutcome.PinResetFailed:
				resetPinResults.SuccessfullyResetPasswordForValidUMUser = true;
				resetPinResults.SuccessfullyResetPINForValidUMUser = false;
				performanceValue = -1.0;
				this.pinResetTestsListOfServersFailedToResetPin = this.pinResetTestsListOfServersFailedToResetPin + user.MailboxServer + ", ";
				break;
			case TestUMConnectivity.PinResetOutcome.Success:
				resetPinResults.SuccessfullyResetPasswordForValidUMUser = true;
				resetPinResults.SuccessfullyResetPINForValidUMUser = true;
				this.pinResetTestsListOfServersPassed = this.pinResetTestsListOfServersPassed + user.MailboxServer + ", ";
				break;
			}
			this.resetPinResults.Add(resetPinResults);
			this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter(this.momSourceString, "Call Latency", mailboxServer, performanceValue));
		}

		private void ExecuteAutoEnumerateTest()
		{
			foreach (TestUMConnectivityHelper.TestMailboxUserDetails testMailboxUserDetails in this.tuiEnumerateUsers)
			{
				Thread.Sleep(500);
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (!this.umconnection.MakeCall(this.addressToCall, this.addressLocalCaller))
				{
					this.HandleTUILogonScenarioOutCome(testMailboxUserDetails, TestUMConnectivity.TUILogonAllOutcome.MakeCallFailed, utcNow);
				}
				else
				{
					this.testParams.DpName = testMailboxUserDetails.DialPlan;
					this.testParams.Phone = testMailboxUserDetails.Phone;
					this.testParams.PIN = testMailboxUserDetails.Pin;
					if (!this.umconnection.ExecuteTest(this.testParams))
					{
						this.HandleTUILogonScenarioOutCome(testMailboxUserDetails, TestUMConnectivity.TUILogonAllOutcome.ExecuteScenarioFailed, utcNow);
					}
					else
					{
						this.HandleTUILogonScenarioOutCome(testMailboxUserDetails, TestUMConnectivity.TUILogonAllOutcome.Success, utcNow);
					}
				}
			}
			Thread.Sleep(500);
			this.umconnection.Shutdown();
			this.WriteTUITestResults();
		}

		private void WriteResetPinTestResults()
		{
			this.WriteResults(this.resetPinResults);
			if (!string.IsNullOrEmpty(this.pinResetTestsListOfServersPassed))
			{
				this.monitoringData.Events.Add(new MonitoringEvent(this.momSourceString, 1000, EventTypeEnumeration.Success, Strings.PINResetSuccessful(this.pinResetTestsListOfServersPassed)));
			}
			string text = string.Empty;
			if (!string.IsNullOrEmpty(this.pinResetTestsListOfServersFailedToResetPasswd))
			{
				text += Strings.PINResetfailedToResetPasswd(this.pinResetTestsListOfServersFailedToResetPasswd).ToString();
				text += "   ";
			}
			if (!string.IsNullOrEmpty(this.pinResetTestsListOfServersFailedToResetPin))
			{
				text += Strings.PINResetfailedToResetPin(this.pinResetTestsListOfServersFailedToResetPin).ToString();
				text += "   ";
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.monitoringData.Events.Add(new MonitoringEvent(this.momSourceString, 1006, EventTypeEnumeration.Error, text));
			}
		}

		private void WriteTUITestResults()
		{
			this.WriteResults(this.tuiResults);
			if (!string.IsNullOrEmpty(this.tuiAllTestsListOfServersPassed))
			{
				this.monitoringData.Events.Add(new MonitoringEvent(this.momSourceString, 1000, EventTypeEnumeration.Success, Strings.TUILogonSuccessful(this.tuiAllTestsListOfServersPassed)));
			}
			string text = string.Empty;
			if (!string.IsNullOrEmpty(this.tuiAllTestsListOfServersFailedToGetPin))
			{
				text += Strings.TUILogonfailedToGetPin(this.tuiAllTestsListOfServersFailedToGetPin).ToString();
				text += "   ";
			}
			if (!string.IsNullOrEmpty(this.tuiAllTestsListOfServersFailedToMakeCall))
			{
				text += Strings.TUILogonfailedToMakeCall(this.tuiAllTestsListOfServersFailedToMakeCall).ToString();
				text += "   ";
			}
			if (!string.IsNullOrEmpty(this.tuiAllTestsListOfServersFailedToExcuteScenario))
			{
				text += Strings.TUILogonfailedToLogon(this.tuiAllTestsListOfServersFailedToExcuteScenario).ToString();
				text += "   ";
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.monitoringData.Events.Add(new MonitoringEvent(this.momSourceString, 1006, EventTypeEnumeration.Error, text));
			}
		}

		private void HandleTUILogonScenarioOutCome(TestUMConnectivityHelper.TestMailboxUserDetails user, TestUMConnectivity.TUILogonAllOutcome reason, ExDateTime time)
		{
			Thread.Sleep(500);
			this.umconnection.EndCall();
			TimeSpan timeSpan = ExDateTime.UtcNow.Subtract(time);
			TestUMConnectivityHelper.TUILogonEnumerateResults tuilogonEnumerateResults = new TestUMConnectivityHelper.TUILogonEnumerateResults();
			tuilogonEnumerateResults.TotalQueuedMessages = "0";
			tuilogonEnumerateResults.SuccessfullyRetrievedPINForValidUMUser = true;
			tuilogonEnumerateResults.CurrCalls = "0";
			tuilogonEnumerateResults.Latency = timeSpan.TotalMilliseconds;
			tuilogonEnumerateResults.MailboxServerBeingTested = user.MailboxServer;
			tuilogonEnumerateResults.UmserverAcceptingCallAnsweringMessages = true;
			tuilogonEnumerateResults.UmIPAddress = this.localIPAddress;
			string performanceInstance = string.Concat(new string[]
			{
				user.MailboxServer,
				"|",
				user.Phone,
				"|",
				this.localIPAddress
			});
			double performanceValue = tuilogonEnumerateResults.Latency;
			switch (reason)
			{
			case TestUMConnectivity.TUILogonAllOutcome.MakeCallFailed:
				tuilogonEnumerateResults.OutBoundSIPCallSuccess = false;
				tuilogonEnumerateResults.EntireOperationSuccess = false;
				performanceValue = -1.0;
				this.tuiAllTestsListOfServersFailedToMakeCall = this.tuiAllTestsListOfServersFailedToMakeCall + user.MailboxServer + ", ";
				break;
			case TestUMConnectivity.TUILogonAllOutcome.ExecuteScenarioFailed:
				tuilogonEnumerateResults.OutBoundSIPCallSuccess = true;
				tuilogonEnumerateResults.EntireOperationSuccess = false;
				performanceValue = -1.0;
				this.tuiAllTestsListOfServersFailedToExcuteScenario = this.tuiAllTestsListOfServersFailedToExcuteScenario + user.MailboxServer + ", ";
				break;
			case TestUMConnectivity.TUILogonAllOutcome.Success:
				tuilogonEnumerateResults.OutBoundSIPCallSuccess = true;
				tuilogonEnumerateResults.EntireOperationSuccess = true;
				this.tuiAllTestsListOfServersPassed = this.tuiAllTestsListOfServersPassed + user.MailboxServer + ", ";
				tuilogonEnumerateResults.CurrCalls = this.umconnection.CurrCalls;
				tuilogonEnumerateResults.TotalQueuedMessages = ((LocalTUILogonConnectivityTester)this.umconnection).TotalQueueLength;
				tuilogonEnumerateResults.UmserverAcceptingCallAnsweringMessages = ((LocalTUILogonConnectivityTester)this.umconnection).AcceptingCalls;
				break;
			}
			this.tuiResults.Add(tuilogonEnumerateResults);
			this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter(this.momSourceString, "Call Latency", performanceInstance, performanceValue));
		}

		private void ExecuteTest()
		{
			this.PrepareSessionAndMakeCall();
			if (!this.umconnection.ExecuteTest(this.testParams))
			{
				this.result.EntireOperationSuccess = false;
				this.result.ReasonForFailure = this.umconnection.Error.LocalizedString;
				this.Cleanup(true);
				return;
			}
			this.result.EntireOperationSuccess = true;
			this.PopulateResults();
			this.Cleanup();
		}

		private void ExecuteCallRouterTest()
		{
			this.LocalCallRouterResult.EntireOperationSuccess = this.umconnection.SendOptions(this.addressToCall);
			this.LocalCallRouterResult.Diagnostics = this.umconnection.MsDiagnosticsHeaderValue;
			LocalizedException error = this.umconnection.Error;
			this.LocalCallRouterResult.ReasonForFailure = ((error != null) ? error.LocalizedString : LocalizedString.Empty);
			this.Cleanup(true);
		}

		private void ExecuteRemoteTest()
		{
			if (!this.TryRemoteTest())
			{
				LocalizedException localizedException;
				if (this.umconnection.Error == null)
				{
					localizedException = new TUC_NoDTMFSwereReceived();
				}
				else
				{
					localizedException = this.umconnection.Error;
				}
				this.Cleanup();
				this.HandleError(localizedException, TestUMConnectivity.EventId.TestExecuteError, this.momSourceString);
			}
		}

		private bool TryRemoteTest()
		{
			this.startTime = ExDateTime.UtcNow;
			this.testParams.DiagDtmfSequence = this.DiagDtmfSequence;
			this.testParams.DiagInitialSilenceInMilisecs = this.DiagInitialSilenceInMilisecs;
			this.testParams.DiagInterDtmfGapInMilisecs = this.DiagInterDtmfGapInMilisecs;
			this.testParams.DiagDtmfDurationInMilisecs = this.DiagDtmfDurationInMilisecs;
			this.testParams.DiagInterDtmfGapDiffInMilisecs = this.DiagInterDtmfDiffGapInMilisecs;
			this.PrepareSessionAndMakeCall();
			if (!this.umconnection.ExecuteTest(this.testParams))
			{
				this.result.EntireOperationSuccess = false;
				if (this.TotalDTMFLoss())
				{
					this.umconnection.EndCall();
					return false;
				}
				this.result = new TestUMConnectivityHelper.RemoteUMConnectivityResults(this.result);
				this.result.ReasonForFailure = this.umconnection.Error.LocalizedString;
				((TestUMConnectivityHelper.RemoteUMConnectivityResults)this.result).ReceivedDigits = ((RemoteConnectivityTester)this.umconnection).AllDTMFsReceived;
				((TestUMConnectivityHelper.RemoteUMConnectivityResults)this.result).ExpectedDigits = Strings.DiagnosticSequence("ABCD*#0123456789");
				this.Cleanup(true);
				return true;
			}
			else
			{
				if (this.MonitoringContext || (!this.MonitoringContext && this.IsUMIPValid()))
				{
					this.result.EntireOperationSuccess = true;
					this.PopulateResults();
					this.Cleanup();
					return true;
				}
				this.result = new TestUMConnectivityHelper.RemoteUMConnectivityResults(this.result);
				this.result.EntireOperationSuccess = false;
				LocalizedException ex = new TUC_InvalidIPAddressReceived();
				this.result.ReasonForFailure = ex.LocalizedString;
				((TestUMConnectivityHelper.RemoteUMConnectivityResults)this.result).ReceivedDigits = ((RemoteConnectivityTester)this.umconnection).AllDTMFsReceived;
				((TestUMConnectivityHelper.RemoteUMConnectivityResults)this.result).ExpectedDigits = Strings.DiagnosticSequence("ABCD*#0123456789");
				this.Cleanup(true);
				return true;
			}
		}

		private bool TotalDTMFLoss()
		{
			bool flag = false;
			if (this.taskMode == TestUMConnectivity.Mode.EndToEndTest && this.umconnection.IsCallEstablished && !((RemoteConnectivityTester)this.umconnection).AnyDTMFsReceived)
			{
				flag = true;
			}
			return flag;
		}

		private bool IsUMIPValid()
		{
			IPAddress ipaddress;
			return this.taskMode != TestUMConnectivity.Mode.EndToEndTest || IPAddress.TryParse(this.umconnection.UmIP, out ipaddress);
		}

		private void PrepareSessionAndMakeCall()
		{
			if (!this.umconnection.MakeCall(this.addressToCall, this.addressLocalCaller))
			{
				this.Cleanup();
				this.HandleError(this.umconnection.Error, TestUMConnectivity.EventId.TestExecuteError, this.momSourceString);
				return;
			}
			TestUMConnectivityHelper.UMConnectivityCallResults umconnectivityCallResults = (TestUMConnectivityHelper.UMConnectivityCallResults)this.result;
			umconnectivityCallResults.OutBoundSIPCallSuccess = true;
		}

		private void Cleanup()
		{
			this.Cleanup(false);
		}

		private void Cleanup(bool cleanUpAnyways)
		{
			Thread.Sleep(1000);
			this.umconnection.EndCall();
			this.umconnection.Shutdown();
			TimeSpan timeSpan = ExDateTime.UtcNow.Subtract(this.startTime);
			TestUMConnectivityHelper.UMConnectivityCallResults umconnectivityCallResults = this.result as TestUMConnectivityHelper.UMConnectivityCallResults;
			if (umconnectivityCallResults != null)
			{
				umconnectivityCallResults.Latency = timeSpan.TotalMilliseconds;
			}
			if (this.result.EntireOperationSuccess || cleanUpAnyways)
			{
				this.WriteResult(this.result);
				string performanceInstance;
				switch (this.taskMode)
				{
				case TestUMConnectivity.Mode.LocalLoopTUILogonSpecific:
					performanceInstance = string.Concat(new string[]
					{
						this.dialPlan.ToString(),
						"|",
						this.Phone,
						"|",
						this.result.UmIPAddress
					});
					break;
				case TestUMConnectivity.Mode.EndToEndTest:
					performanceInstance = this.ipGateway.ToString() + "|" + this.Phone;
					break;
				default:
					performanceInstance = this.result.UmIPAddress;
					break;
				}
				this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter(this.momSourceString, "Call Latency", performanceInstance, timeSpan.TotalMilliseconds));
				this.monitoringData.Events.Add(new MonitoringEvent(this.momSourceString, 1000, EventTypeEnumeration.Success, Strings.OperationSuccessful));
			}
		}

		private void HandleError(LocalizedException localizedException, TestUMConnectivity.EventId id, string eventSource)
		{
			this.WriteErrorAndMonitoringEvent(localizedException, ErrorCategory.NotSpecified, null, (int)id, eventSource);
		}

		private void WriteResult(IConfigurable result)
		{
			base.WriteObject(result);
		}

		private void WriteResults(IEnumerable dataObjects)
		{
			if (dataObjects != null)
			{
				foreach (object obj in dataObjects)
				{
					IConfigurable sendToPipeline = (IConfigurable)obj;
					base.WriteObject(sendToPipeline);
				}
			}
		}

		private void WriteErrorAndMonitoringEvent(LocalizedException localizedException, ErrorCategory errorCategory, object target, int eventId, string eventSource)
		{
			this.monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, EventTypeEnumeration.Error, localizedException.LocalizedString));
			string performanceInstance;
			if (this.ipGateway != null)
			{
				performanceInstance = this.ipGateway.ToString() + "|" + this.Phone;
			}
			else if (this.TUILogon)
			{
				performanceInstance = string.Concat(new string[]
				{
					this.dialPlan.ToString(),
					"|",
					this.Phone,
					"|",
					this.localIPAddress
				});
			}
			else if (this.TUILogonAll)
			{
				performanceInstance = this.localIPAddress;
			}
			else
			{
				performanceInstance = this.localIPAddress;
			}
			this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter(this.momSourceString, "Call Latency", performanceInstance, -1.0));
			base.WriteError(localizedException, errorCategory, target);
		}

		private void DoOwnValidate()
		{
			if (this.ipGateway != null)
			{
				this.momSourceString = "MSExchange Monitoring UMConnectivity Remote Voice";
				IEnumerable<UMIPGateway> objects = this.ipGateway.GetObjects<UMIPGateway>(null, this.configurationSession);
				using (IEnumerator<UMIPGateway> enumerator = objects.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						this.HandleValidationFailed(new LocalizedException(Strings.NonExistantIPGateway(this.ipGateway.ToString())), TestUMConnectivity.EventId.NoIPGatewaysFound);
					}
					this.adresults = enumerator.Current;
					if (enumerator.MoveNext())
					{
						this.HandleValidationFailed(new LocalizedException(Strings.MultipleIPGatewaysWithSameId(this.ipGateway.ToString())), TestUMConnectivity.EventId.MultipleIPGatewaysFound);
					}
				}
				UMIPGateway umipgateway = (UMIPGateway)this.adresults;
				this.ValidateRemoteProxy(umipgateway.Address.ToString(), umipgateway.Port);
				this.ValidateLocalSipUri();
				this.taskMode = TestUMConnectivity.Mode.EndToEndTest;
				this.umconnection = new RemoteConnectivityTester(this.hostToCall, this.portToCall);
			}
			else if (this.TUILogonAll)
			{
				this.momSourceString = "MSExchange Monitoring UMConnectivity Local TUI";
				this.taskMode = TestUMConnectivity.Mode.LocalLoopTUILogonAll;
				this.umconnection = new LocalTUILogonConnectivityTester();
			}
			else
			{
				if (this.TUILogon)
				{
					this.momSourceString = "MSExchange Monitoring UMConnectivity Local TUI";
					IEnumerable<UMDialPlan> objects2 = this.dialPlan.GetObjects<UMDialPlan>(null, this.configurationSession);
					using (IEnumerator<UMDialPlan> enumerator2 = objects2.GetEnumerator())
					{
						if (!enumerator2.MoveNext())
						{
							this.HandleValidationFailed(new LocalizedException(Strings.NonExistantDialPlan(this.dialPlan.ToString())), TestUMConnectivity.EventId.NoDialPlansFound);
						}
						this.adresults = enumerator2.Current;
						if (enumerator2.MoveNext())
						{
							this.HandleValidationFailed(new LocalizedException(Strings.MultipleDialplansWithSameId(this.dialPlan.ToString())), TestUMConnectivity.EventId.MultipleDialPlansFound);
						}
					}
					this.umDP = (UMDialPlan)this.adresults;
					this.taskMode = TestUMConnectivity.Mode.LocalLoopTUILogonSpecific;
					this.umconnection = new LocalTUILogonConnectivityTester();
					using (UMSubscriber umsubscriber = UMRecipient.Factory.FromExtension<UMSubscriber>(this.Phone, this.umDP, null))
					{
						if (umsubscriber == null)
						{
							this.HandleValidationFailed(new LocalizedException(Strings.InvalidUMUser(this.Phone, this.dialPlan.ToString())), TestUMConnectivity.EventId.InvalidUMUser);
						}
						else if (!this.IsMailboxLocal(umsubscriber.ExchangePrincipal))
						{
							LocalizedException localizedException = new LocalizedException(Strings.MailboxNotLocal(umsubscriber.ExchangeLegacyDN, umsubscriber.ExchangePrincipal.MailboxInfo.Location.ServerFqdn));
							this.HandleValidationFailed(localizedException, TestUMConnectivity.EventId.InvalidUMUser);
						}
						goto IL_29B;
					}
				}
				if (this.ResetPIN)
				{
					this.momSourceString = "MSExchange Monitoring UMConnectivity Reset Pin";
					this.taskMode = TestUMConnectivity.Mode.ResetPin;
				}
				else
				{
					this.taskMode = TestUMConnectivity.Mode.LocalLoop;
					this.momSourceString = "MSExchange Monitoring UMConnectivity Local Voice";
					this.umconnection = new LocalConnectivityTester();
				}
			}
			IL_29B:
			if (!this.Secured && this.MediaSecured)
			{
				this.HandleValidationFailed(new LocalizedException(Strings.SrtpWithoutTls), TestUMConnectivity.EventId.TestExecuteError);
			}
			if (!this.Secured && !string.IsNullOrEmpty(this.CertificateThumbprint))
			{
				this.HandleValidationFailed(new LocalizedException(Strings.CertWithoutTls), TestUMConnectivity.EventId.TestExecuteError);
			}
			if (this.Secured && string.IsNullOrEmpty(this.CertificateThumbprint) && string.IsNullOrEmpty(this.localLoopTarget.CertificateThumbprint))
			{
				this.HandleValidationFailed(new LocalizedException(Strings.MustSpecifyThumbprint), TestUMConnectivity.EventId.TestExecuteError);
			}
			if (this.taskMode == TestUMConnectivity.Mode.ResetPin)
			{
				this.GetListOfUMTestMailboxUsers();
			}
			else if (this.taskMode == TestUMConnectivity.Mode.EndToEndTest)
			{
				this.result = new TestUMConnectivityHelper.UMConnectivityCallResults();
				this.FormRemoteSipUri();
				this.FormLocalSipUri();
				this.testParams = new TestParameters(this.addressToCall, null, null, null, this.MonitoringContext);
			}
			else
			{
				this.localLoopTarget.CheckServiceRunning();
				if (!this.IsParameterSpecified("Secured"))
				{
					if (this.localLoopTarget.StartupMode == UMStartupMode.TCP)
					{
						this.Secured = false;
					}
					else
					{
						this.Secured = true;
					}
				}
				this.FormSIPUriToCall();
				switch (this.taskMode)
				{
				case TestUMConnectivity.Mode.LocalLoopTUILogonAll:
					this.GetListofUMTestMailboxUsersWithPin();
					break;
				case TestUMConnectivity.Mode.LocalLoopTUILogonSpecific:
					this.PerformFurtherChecks();
					this.result = new TestUMConnectivityHelper.LocalUMConnectivityResults();
					this.result.UmIPAddress = this.localIPAddress;
					break;
				default:
					this.result = this.localLoopTarget.CreateTestResult();
					this.result.UmIPAddress = this.localIPAddress;
					break;
				}
				if (this.taskMode != TestUMConnectivity.Mode.LocalLoop)
				{
					this.testParams = new TestParameters(this.addressToCall, this.PIN, this.Phone, (this.umDP == null) ? null : this.umDP.Name, this.MonitoringContext);
				}
				else
				{
					this.testParams = new TestParameters(this.addressToCall, null, null, null, this.MonitoringContext);
				}
			}
			if (this.taskMode != TestUMConnectivity.Mode.ResetPin && !this.umconnection.Initialize(this.ListenPort, this.Secured, this.MediaSecured, this.CertificateThumbprint))
			{
				this.HandleValidationFailed(this.umconnection.Error, TestUMConnectivity.EventId.TestExecuteError);
			}
		}

		private void PerformFurtherChecks()
		{
			if (!this.UserFoundAndEnabled(this.Phone, this.umDP))
			{
				this.HandleValidationFailed(new UmUserProblem(), TestUMConnectivity.EventId.ADError);
			}
		}

		private void HandleValidationFailed(LocalizedException localizedException, TestUMConnectivity.EventId id)
		{
			if (this.umconnection != null)
			{
				this.umconnection.Shutdown();
			}
			this.HandleError(localizedException, id, this.momSourceString);
		}

		private void GetListOfUMTestMailboxUsers()
		{
			this.GetListOfCompatibleMailboxServers();
			this.resetPinResults = new List<TestUMConnectivityHelper.ResetPinResults>();
			this.pinResetUsers = new List<TestUMConnectivityHelper.UsersForResetPin>();
			foreach (Server server in this.allMailboxServers)
			{
				ADSite localSite = this.configurationSession.GetLocalSite();
				UmConnectivityCredentialsHelper umConnectivityCredentialsHelper = new UmConnectivityCredentialsHelper(localSite, server);
				umConnectivityCredentialsHelper.InitializeUser(true);
				if (umConnectivityCredentialsHelper.IsUserFound && umConnectivityCredentialsHelper.IsUserUMEnabled && umConnectivityCredentialsHelper.IsExchangePrincipalFound && this.IsMailboxLocal(umConnectivityCredentialsHelper.ExPrincipal))
				{
					this.pinResetUsers.Add(new TestUMConnectivityHelper.UsersForResetPin(umConnectivityCredentialsHelper.ExPrincipal, new NetworkCredential(umConnectivityCredentialsHelper.UserName, string.Empty, umConnectivityCredentialsHelper.UserDomain), umConnectivityCredentialsHelper.User, server.Fqdn));
				}
			}
		}

		private void GetListofUMTestMailboxUsersWithPin()
		{
			this.GetListOfCompatibleMailboxServers();
			this.tuiEnumerateUsers = new List<TestUMConnectivityHelper.TestMailboxUserDetails>();
			this.tuiResults = new List<TestUMConnectivityHelper.TUILogonEnumerateResults>();
			foreach (Server server in this.allMailboxServers)
			{
				ADSite localSite = this.configurationSession.GetLocalSite();
				UmConnectivityCredentialsHelper umConnectivityCredentialsHelper = new UmConnectivityCredentialsHelper(localSite, server);
				umConnectivityCredentialsHelper.InitializeUser(false);
				if (umConnectivityCredentialsHelper.IsUserFound && umConnectivityCredentialsHelper.IsUserUMEnabled && umConnectivityCredentialsHelper.IsExchangePrincipalFound && this.IsMailboxLocal(umConnectivityCredentialsHelper.ExPrincipal))
				{
					if (!umConnectivityCredentialsHelper.SuccessfullyGotPin)
					{
						this.InitializePINRetrievalFaulures(server.Fqdn, umConnectivityCredentialsHelper.User.UMExtension);
					}
					else
					{
						this.tuiEnumerateUsers.Add(new TestUMConnectivityHelper.TestMailboxUserDetails(umConnectivityCredentialsHelper.User.UMExtension, umConnectivityCredentialsHelper.UMPin, umConnectivityCredentialsHelper.UserDP.Name, server.Fqdn));
					}
				}
			}
		}

		private void GetListOfCompatibleMailboxServers()
		{
			LocalizedException ex = null;
			Server server = null;
			bool flag = false;
			try
			{
				server = this.configurationSession.FindLocalServer();
			}
			catch (LocalServerNotFoundException ex2)
			{
				ex = ex2;
			}
			if (ex != null)
			{
				TopologyDiscoveryProblem localizedException = new TopologyDiscoveryProblem(ex.Message);
				this.HandleError(localizedException, TestUMConnectivity.EventId.ADError, this.momSourceString);
			}
			if (server != null && server.ServerSite != null)
			{
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, server.ServerSite)
				});
				ADPagedReader<Server> adpagedReader = this.configurationSession.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
				this.allMailboxServers = new List<Server>();
				foreach (Server server2 in adpagedReader)
				{
					if (!CommonUtil.IsServerCompatible(server2))
					{
						flag = true;
					}
					else
					{
						this.allMailboxServers.Add(server2);
					}
				}
			}
			if (flag)
			{
				this.WriteWarning(Strings.InvalidMailboxServerVersionForTUMCTask);
			}
			if (this.allMailboxServers == null || this.allMailboxServers.Count == 0)
			{
				NoMailboxServersFound localizedException2 = new NoMailboxServersFound();
				this.HandleError(localizedException2, TestUMConnectivity.EventId.ADError, this.momSourceString);
			}
		}

		private void InitializePINRetrievalFaulures(string server, string phone)
		{
			TestUMConnectivityHelper.TUILogonEnumerateResults tuilogonEnumerateResults = new TestUMConnectivityHelper.TUILogonEnumerateResults();
			tuilogonEnumerateResults.TotalQueuedMessages = "0";
			tuilogonEnumerateResults.SuccessfullyRetrievedPINForValidUMUser = false;
			tuilogonEnumerateResults.CurrCalls = "0";
			tuilogonEnumerateResults.EntireOperationSuccess = false;
			tuilogonEnumerateResults.Latency = 0.0;
			tuilogonEnumerateResults.MailboxServerBeingTested = server;
			tuilogonEnumerateResults.OutBoundSIPCallSuccess = false;
			tuilogonEnumerateResults.UmserverAcceptingCallAnsweringMessages = true;
			tuilogonEnumerateResults.UmIPAddress = this.localIPAddress;
			this.tuiResults.Add(tuilogonEnumerateResults);
			this.tuiAllTestsListOfServersFailedToGetPin = this.tuiAllTestsListOfServersFailedToGetPin + server + ", ";
			string performanceInstance = string.Concat(new string[]
			{
				server,
				"|",
				phone,
				"|",
				this.localIPAddress
			});
			this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter(this.momSourceString, "Call Latency", performanceInstance, -1.0));
		}

		private bool UserFoundAndEnabled(string phone, UMDialPlan dp)
		{
			return UMSubscriber.IsValidSubscriber(phone, dp, null);
		}

		private void PopulateResults()
		{
			((TestUMConnectivityHelper.UMConnectivityCallResults)this.result).CurrCalls = this.umconnection.CurrCalls;
			TestUMConnectivity.Mode mode = this.taskMode;
			if (mode == TestUMConnectivity.Mode.EndToEndTest)
			{
				this.result.UmIPAddress = this.umconnection.UmIP;
				return;
			}
			this.LocalBackendResult.UmserverAcceptingCallAnsweringMessages = ((LocalConnectivityTester)this.umconnection).AcceptingCalls;
			this.LocalBackendResult.TotalQueuedMessages = ((LocalConnectivityTester)this.umconnection).TotalQueueLength;
		}

		private void FormSIPUriToCall()
		{
			int num = (this.RemotePort == 0) ? this.localLoopTarget.GetPort(this.Secured) : this.RemotePort;
			string targetFqdn = this.localLoopTarget.TargetFqdn;
			this.addressToCall = "sip:" + targetFqdn + ":" + num.ToString(CultureInfo.InvariantCulture);
		}

		private void ValidateRemoteProxy(string address, int port)
		{
			if (!this.Secured)
			{
				this.hostToCall = address;
				this.portToCall = ((port != 0) ? port : 5060);
				return;
			}
			IPHostEntry iphostEntry;
			if (Utils.HasValidDNSRecord(address, out iphostEntry))
			{
				this.hostToCall = iphostEntry.HostName;
				this.portToCall = ((port != 0) ? port : 5061);
				return;
			}
			DNSEntryNotFound localizedException = new DNSEntryNotFound();
			this.HandleError(localizedException, TestUMConnectivity.EventId.DNSEntryNotFound, this.momSourceString);
		}

		private void FormRemoteSipUri()
		{
			this.addressToCall = (this.Phone.StartsWith("sip:", StringComparison.InvariantCulture) ? this.Phone : string.Concat(new string[]
			{
				"sip:",
				this.Phone,
				"@",
				this.hostToCall,
				":",
				this.portToCall.ToString(CultureInfo.InvariantCulture)
			}));
		}

		private void FormLocalSipUri()
		{
			this.addressLocalCaller = (string.IsNullOrEmpty(this.From) ? string.Empty : this.From);
		}

		private void ValidateLocalSipUri()
		{
			if (!string.IsNullOrEmpty(this.From) && !this.From.StartsWith("sip:", StringComparison.InvariantCulture))
			{
				LocalizedException localizedException = new TUC_SipUriError(" \"From\" ");
				this.HandleError(localizedException, TestUMConnectivity.EventId.TestExecuteError, this.momSourceString);
			}
		}

		private void WriteErrorIfMissingUCMA()
		{
			bool flag = false;
			Exception innerException = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\UCMA\\{902F4F35-D5DC-4363-8671-D5EF0D26C21D}"))
				{
					if (registryKey != null)
					{
						string text = registryKey.GetValue("Version") as string;
						if (!string.IsNullOrEmpty(text))
						{
							Version version = new Version(text);
							if (version.Major >= 5 && version.Minor >= 0)
							{
								flag = true;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (!(ex is ArgumentException) && !(ex is FormatException) && !(ex is OverflowException) && !(ex is SecurityException))
				{
					throw;
				}
				innerException = ex;
			}
			if (!flag)
			{
				base.WriteError(new UCMAPreReqException(innerException), ErrorCategory.NotSpecified, this);
			}
		}

		private const string SIP = "sip:";

		private const string AT = "@";

		private const string COLON = ":";

		private const string LocalVoiceSource = " Local Voice";

		private const string LocalVoiceTUISource = " Local TUI";

		private const string RemoteVoiceSource = " Remote Voice";

		private const string ResetPinSource = " Reset Pin";

		private const string TaskNoun = "UMConnectivity";

		private const string MonitoringPerformanceObject = "Exchange Monitoring";

		private const double LatencyPerformanceInCaseOfError = -1.0;

		private const string TaskMonitoringEventSource = "MSExchange Monitoring UMConnectivity";

		private ITopologyConfigurationSession configurationSession;

		private IRecipientSession globalCatalogSession;

		private string momSourceString;

		private MonitoringData monitoringData = new MonitoringData();

		private BaseUMconnectivityTester umconnection;

		private ExDateTime startTime;

		private TestUMConnectivityHelper.UMConnectivityResults result;

		private List<TestUMConnectivityHelper.TUILogonEnumerateResults> tuiResults;

		private List<TestUMConnectivityHelper.ResetPinResults> resetPinResults;

		private List<Server> allMailboxServers;

		private string tuiAllTestsListOfServersPassed = string.Empty;

		private string tuiAllTestsListOfServersFailedToGetPin = string.Empty;

		private string tuiAllTestsListOfServersFailedToMakeCall = string.Empty;

		private string tuiAllTestsListOfServersFailedToExcuteScenario = string.Empty;

		private List<TestUMConnectivityHelper.TestMailboxUserDetails> tuiEnumerateUsers;

		private List<TestUMConnectivityHelper.UsersForResetPin> pinResetUsers;

		private string pinResetTestsListOfServersPassed = string.Empty;

		private string pinResetTestsListOfServersFailedToResetPin = string.Empty;

		private string pinResetTestsListOfServersFailedToResetPasswd = string.Empty;

		private string addressToCall;

		private string addressLocalCaller;

		private string hostToCall;

		private int portToCall;

		private string localIPAddress;

		private UMDialPlan umDP;

		private TestParameters testParams;

		private IConfigurable adresults;

		private UMIPGatewayIdParameter ipGateway;

		private UMDialPlanIdParameter dialPlan;

		private TestUMConnectivity.Mode taskMode;

		private LocalLoopTargetStrategy localLoopTarget;

		private enum Mode
		{
			LocalLoop,
			LocalLoopTUILogonAll,
			LocalLoopTUILogonSpecific,
			EndToEndTest,
			ResetPin
		}

		private enum EventId
		{
			OperationSuccessFul = 1000,
			MultipleIPGatewaysFound,
			NoIPGatewaysFound,
			NoDialPlansFound,
			MultipleDialPlansFound,
			DNSEntryNotFound,
			TestExecuteError,
			ADError = 1008,
			ServiceNotInstalledOrRunning,
			InvalidUMUser
		}

		private enum TUILogonAllOutcome
		{
			MakeCallFailed,
			ExecuteScenarioFailed,
			Success
		}

		private enum PinResetOutcome
		{
			PasswordResetFailed,
			PinResetFailed,
			Success
		}
	}
}
