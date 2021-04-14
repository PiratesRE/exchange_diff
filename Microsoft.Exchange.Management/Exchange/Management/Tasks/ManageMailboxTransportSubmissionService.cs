using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageMailboxTransportSubmissionService : ManageService
	{
		public ManageMailboxTransportSubmissionService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.MailboxTransportSubmissionServiceDisplayName;
			base.Description = Strings.MailboxTransportSubmissionServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeSubmission.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeSubmission";
			}
		}

		private const string ServiceShortName = "MSExchangeSubmission";

		private const string ServiceBinaryName = "MSExchangeSubmission.exe";

		private const int SubmissionServiceFirstFailureActionDelay = 5000;

		private const int SubmissionServiceSecondFailureActionDelay = 5000;

		private const int SubmissionServiceAllOtherFailuresActionDelay = 5000;

		private const int SubmissionServiceFailureResetPeriod = 0;
	}
}
