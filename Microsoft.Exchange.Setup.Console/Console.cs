using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;

namespace Microsoft.Exchange.Setup.Console
{
	internal class Console : LauncherBase
	{
		public Console()
		{
		}

		public Console(ISetupContext context, MonadConnection connection) : base(context, connection)
		{
		}

		public int StartMain(string[] args, Dictionary<string, object> parsedArguments, SetupBase theApp)
		{
			Console.CancelKeyPress += Console.ExsetupConcoleExitControlCEventHandler;
			int num = base.MainCore(args, parsedArguments, theApp);
			SetupLogger.Log(new LocalizedString("CurrentResult console.startmain:52: " + num));
			return num;
		}

		protected override int ProcessRunInternal(string[] args, SetupBase theApp)
		{
			base.RootDataHandler.UpdateWorkUnits();
			ModeDataHandler modeDatahandler = base.RootDataHandler.ModeDatahandler;
			foreach (string installableUnitName in base.RootDataHandler.RequestedInstallableUnits.ToArray())
			{
				InstallableUnitConfigurationInfo installableUnitConfigurationInfoByName = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName);
				theApp.ReportMessage(string.Format("     {0}", installableUnitConfigurationInfoByName.DisplayName));
			}
			List<ValidationError> list = new List<ValidationError>();
			list.AddRange(base.RootDataHandler.Validate());
			if (list.Count > 0)
			{
				foreach (ValidationError validationError in list)
				{
					theApp.ReportError(validationError.Description);
				}
				SetupLogger.Log(new LocalizedString("CurrentResult console.ProcessRunInternal:90: " + 1));
				return 1;
			}
			theApp.ReportMessage();
			theApp.ReportMessage(Strings.PrereqCheckBanner);
			theApp.ReportMessage();
			if (base.RootDataHandler.TreatPreReqErrorsAsWarningsInDC)
			{
				theApp.ReportMessage(Strings.TreatPreReqErrorsAsWarnings);
			}
			DataHandler preCheckDataHandler = base.RootDataHandler.ModeDatahandler.PreCheckDataHandler;
			preCheckDataHandler.Read(theApp.InteractionHandler, null);
			preCheckDataHandler.UpdateWorkUnits();
			preCheckDataHandler.Save(theApp.InteractionHandler);
			int num = 0;
			foreach (object obj in preCheckDataHandler.SavedResults)
			{
				SetupLogger.Log(Strings.ExecutionResult(num++, obj.ToString()));
			}
			if (preCheckDataHandler.WorkUnits.Count > 0 && !preCheckDataHandler.IsSucceeded && !base.RootDataHandler.TreatPreReqErrorsAsWarningsInDC)
			{
				SetupLogger.Log(new LocalizedString("CurrentResult console.ProcessRunInternal:136: " + 1));
				RestoreServer restoreServer = new RestoreServer();
				restoreServer.RestoreServerOnPrereqFailure();
				return 1;
			}
			List<ValidationError> list2 = new List<ValidationError>();
			list2.AddRange(preCheckDataHandler.Validate());
			if (list2.Count > 0)
			{
				try
				{
					TaskLogger.IsPrereqLogging = true;
					foreach (ValidationError validationError2 in list2)
					{
						theApp.ReportError(validationError2.Description);
					}
				}
				finally
				{
					TaskLogger.IsPrereqLogging = false;
				}
				SetupLogger.Log(new LocalizedString("CurrentResult console.ProcessRunInternal:167: " + 1));
				return 1;
			}
			Console.CancelKeyPress += Console.ExsetupConsoleCancelControlCEventHandler;
			Console.CancelKeyPress -= Console.ExsetupConcoleExitControlCEventHandler;
			theApp.ReportMessage();
			theApp.ReportMessage(Strings.ConfiguringExchangeServer);
			theApp.ReportMessage();
			base.RootDataHandler.Save(theApp.InteractionHandler);
			int num2 = 0;
			foreach (object obj2 in base.RootDataHandler.SavedResults)
			{
				SetupLogger.Log(Strings.ExecutionResult(num2++, obj2.ToString()));
			}
			if (!base.RootDataHandler.IsSucceeded)
			{
				SetupLogger.Log(new LocalizedString("CurrentResult console.ProcessRunInternal:198: " + 1));
				return 1;
			}
			theApp.ReportMessage();
			theApp.ReportMessage(Strings.SuccessSummary);
			SetupLogger.Log(new LocalizedString("CurrentResult console.ProcessRunInternal:205: " + 0));
			return 0;
		}

		private static void ExsetupConsoleCancelControlCEventHandler(object sender, ConsoleCancelEventArgs args)
		{
			try
			{
				args.Cancel = true;
			}
			catch (InvalidOperationException ex)
			{
				SetupLogger.LogWarning(Strings.ExsetupTerminatedWithControlBreak(ex.Message));
			}
		}

		private static void ExsetupConcoleExitControlCEventHandler(object sender, ConsoleCancelEventArgs args)
		{
			args.Cancel = false;
			Environment.Exit(0);
		}
	}
}
