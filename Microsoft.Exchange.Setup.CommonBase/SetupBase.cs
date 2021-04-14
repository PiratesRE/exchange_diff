using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Setup.Bootstrapper.Common;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.Parser;

namespace Microsoft.Exchange.Setup.CommonBase
{
	internal abstract class SetupBase : DisposeTrackableBase, ISetupBase
	{
		protected SetupBase()
		{
			this.SourceDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			this.TargetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp\\ExchangeSetup");
			this.IsExchangeInstalled = SetupLauncherHelper.IsExchangeInstalled();
		}

		public static string[] SetupArgs { get; set; }

		public static SetupBase TheApp { get; internal set; }

		public Dictionary<string, object> ParsedArguments { get; private set; }

		public abstract CommandLineParser Parser { get; }

		public string SourceDir { get; private set; }

		public string TargetDir { get; private set; }

		public ExitCode HasValidArgs { get; private set; }

		public CommandInteractionHandler InteractionHandler { get; set; }

		public bool IsExchangeInstalled { get; private set; }

		public ISetupLogger Logger { get; internal set; }

		public virtual void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs eventArgs)
		{
			SetupLauncherHelper.LogInstalledExchangeDirAcl();
			if (eventArgs.ExceptionObject is Exception)
			{
				Exception e = (Exception)eventArgs.ExceptionObject;
				this.Logger.LogError(e);
				return;
			}
			string errMsg = eventArgs.ExceptionObject.ToString();
			SetupLogger.Log(Strings.UnhandledErrorMessage(errMsg));
		}

		public virtual void ReportException(Exception e)
		{
			this.Logger.LogError(e);
		}

		public virtual void ReportError(string error)
		{
			this.Logger.LogError(new Exception(error));
		}

		public abstract void ReportMessage(string message);

		public abstract void ReportMessage();

		public abstract void ReportWarning(string warning);

		public abstract void WriteError(string error);

		public virtual ExitCode SetupChecks()
		{
			this.HasValidArgs = this.ProcessArguments();
			if (this.HasValidArgs == ExitCode.Success && this.ParsedArguments.ContainsKey("sourcedir"))
			{
				this.SourceDir = this.ParsedArguments["sourcedir"].ToString();
			}
			return this.HasValidArgs;
		}

		public virtual int Run()
		{
			return (int)this.SetupChecks();
		}

		public virtual ExitCode ProcessArguments()
		{
			this.ParsedArguments = null;
			ExitCode result = ExitCode.Success;
			try
			{
				this.ParsedArguments = this.Parser.ParseCommandLine(SetupBase.SetupArgs);
				this.Logger.Log(Strings.CommandLine(Assembly.GetExecutingAssembly().GetType().Name, string.Join(" ", SetupBase.SetupArgs)));
			}
			catch (ParseException e)
			{
				this.Logger.Log(Strings.CommandLine(Assembly.GetExecutingAssembly().GetType().Name, string.Join(" ", SetupBase.SetupArgs)));
				this.ReportException(e);
				result = ExitCode.Error;
			}
			return result;
		}

		protected static int MainCore<T>(string[] args, ISetupLogger logger) where T : SetupBase, new()
		{
			SetupLogger.Logger = logger;
			SetupBase.SetupArgs = args;
			int num = Privileges.RemoveAllExcept(new string[]
			{
				"SeAuditPrivilege",
				"SeBackupPrivilege",
				"SeChangeNotifyPrivilege",
				"SeCreateGlobalPrivilege",
				"SeRestorePrivilege",
				"SeSecurityPrivilege",
				"SeShutdownPrivilege",
				"SeTakeOwnershipPrivilege",
				"SeDebugPrivilege",
				"SeCreateSymbolicLinkPrivilege"
			});
			SecurityException ex = null;
			try
			{
				ExWatson.Register();
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (TypeInitializationException ex3)
			{
				if (!(ex3.InnerException is SecurityException))
				{
					throw;
				}
				ex = (SecurityException)ex3.InnerException;
			}
			bool flag;
			int result;
			using (new Mutex(true, "Microsoft.Exchange.Setup", ref flag))
			{
				using (SetupBase.TheApp = Activator.CreateInstance<T>())
				{
					SetupBase.TheApp.Logger = logger;
					if (num != 0)
					{
						SetupBase.TheApp.WriteError(Strings.RemovePrivileges(num).ToString());
					}
					if (ex != null)
					{
						SetupBase.TheApp.WriteError(Strings.SecurityIssueFoundWhenInit(ex.Message));
					}
					if (!flag)
					{
						SetupBase.TheApp.WriteError(Strings.CannotRunMultipleInstances);
					}
					try
					{
						logger.StartLogging();
					}
					catch (SetupLogInitializeException ex4)
					{
						SetupBase.TheApp.WriteError(ex4.Message);
						logger.Log(new LocalizedString("CurrentResult setupbase.maincore:353: " + ExitCode.Error));
						return 1;
					}
					int num2 = 0;
					if (num != 0)
					{
						logger.Log(Strings.RemovePrivileges(num));
						num2 = 1;
					}
					if (ex != null)
					{
						logger.Log(Strings.SecurityIssueFoundWhenInit(ex.Message));
						num2 = 1;
					}
					if (!flag)
					{
						logger.Log(Strings.CannotRunMultipleInstances);
						num2 = 1;
					}
					if (num2 != 0)
					{
						logger.Log(new LocalizedString("CurrentResult setupbase.maincore:380: " + num2));
						result = num2;
					}
					else
					{
						SetupLauncherHelper.LogInstalledExchangeDirAcl();
						AppDomain.CurrentDomain.UnhandledException += SetupBase.TheApp.UnhandledExceptionHandler;
						num2 = SetupBase.TheApp.Run();
						SetupLauncherHelper.LogInstalledExchangeDirAcl();
						logger.Log(new LocalizedString("CurrentResult setupbase.maincore:396: " + num2));
						result = num2;
					}
				}
			}
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SetupBase>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			try
			{
				this.Logger.StopLogging();
			}
			catch (IOException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public const string Indent1 = " ";

		private const string SetupMutexName = "Microsoft.Exchange.Setup";
	}
}
