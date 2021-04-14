using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageMailSubmissionService : ManageService
	{
		public ManageMailSubmissionService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Disabled;
			base.DisplayName = Strings.MailSubmissionServiceDisplayName;
			base.Description = Strings.MailSubmissionServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeMailSubmission.exe");
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.MailSubmission.EventLog.dll");
			base.CategoryCount = 1;
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
				return "MSExchangeMailSubmission";
			}
		}

		private const string ServiceShortName = "MSExchangeMailSubmission";

		private const string ServiceBinaryName = "MSExchangeMailSubmission.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.MailSubmission.EventLog.dll";
	}
}
