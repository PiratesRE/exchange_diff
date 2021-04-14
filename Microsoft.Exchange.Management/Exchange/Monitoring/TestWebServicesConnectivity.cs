using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "WebServicesConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "ClientAccessServerParameterSet")]
	public sealed class TestWebServicesConnectivity : TestWebServicesTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestWebServicesConnectivity;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter LightMode
		{
			get
			{
				return (bool)(base.Fields["LightMode"] ?? false);
			}
			set
			{
				base.Fields["LightMode"] = value.IsPresent;
			}
		}

		protected override string CmdletName
		{
			get
			{
				return "Test-WebServicesConnectivity";
			}
		}

		protected override bool IsOutlookProvider
		{
			get
			{
				return false;
			}
		}

		protected override void InternalProcessRecord()
		{
			string text;
			string text2;
			bool flag = base.ValidateAutoDiscover(out text, out text2);
			string text3 = null;
			WebServicesTestOutcome.TestScenario scenario = this.LightMode.IsPresent ? WebServicesTestOutcome.TestScenario.EwsConvertId : WebServicesTestOutcome.TestScenario.EwsGetFolder;
			if (base.IsFromAutoDiscover)
			{
				if (string.IsNullOrEmpty(text))
				{
					base.OutputSkippedOutcome(scenario, Strings.InformationSkippedEws);
				}
				else
				{
					text3 = text;
					base.WriteVerbose(Strings.VerboseTestEwsFromAutoDiscover(text3));
				}
			}
			else
			{
				text3 = base.GetSpecifiedEwsUrl();
				base.WriteVerbose(Strings.VerboseTestEwsFromParameter(text3));
			}
			if (!string.IsNullOrEmpty(text3))
			{
				EwsValidator ewsValidator = new EwsValidator(text3, base.TestAccount.Credential, base.TestAccount.User.PrimarySmtpAddress.ToString())
				{
					VerboseDelegate = new Task.TaskVerboseLoggingDelegate(base.WriteVerbose),
					UserAgent = base.UserAgentString,
					IgnoreSslCertError = (base.TrustAnySSLCertificate || base.MonitoringContext.IsPresent),
					Operation = (this.LightMode.IsPresent ? EwsValidator.RequestOperation.ConvertId : EwsValidator.RequestOperation.GetFolder),
					PublicFolderMailboxGuid = ((base.TestAccount.User.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox) ? base.TestAccount.User.ExchangeGuid : Guid.Empty)
				};
				flag = ewsValidator.Invoke();
				WebServicesTestOutcome outcome = new WebServicesTestOutcome
				{
					Scenario = scenario,
					Source = base.LocalServer.Fqdn,
					Result = (flag ? CasTransactionResultEnum.Success : CasTransactionResultEnum.Failure),
					Error = string.Format("{0}", ewsValidator.Error),
					ServiceEndpoint = TestWebServicesTaskBase.FqdnFromUrl(text3),
					Latency = ewsValidator.Latency,
					ScenarioDescription = TestWebServicesTaskBase.GetScenarioDescription(scenario),
					Verbose = ewsValidator.Verbose
				};
				base.Output(outcome);
			}
			base.InternalProcessRecord();
		}
	}
}
