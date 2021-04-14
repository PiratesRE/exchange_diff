using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Setup;
using Microsoft.Exchange.Setup.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class SetupLoggerImpl : ISetupLogger
	{
		public bool IsPrereqLogging
		{
			get
			{
				return TaskLogger.IsPrereqLogging;
			}
			set
			{
				TaskLogger.IsPrereqLogging = value;
			}
		}

		public void StartLogging()
		{
			if (!Directory.Exists(SetupLoggerImpl.setupLogDirectory))
			{
				Directory.CreateDirectory(SetupLoggerImpl.setupLogDirectory);
			}
			string filename = Path.Combine(SetupLoggerImpl.setupLogDirectory, SetupLoggerImpl.setupLogFileNameForWatson);
			string filename2 = Path.Combine(SetupLoggerImpl.setupLogDirectory, "ExchangeSetup.msilog");
			string dataMiningPath = null;
			if (DatacenterRegistry.IsMicrosoftHostedOnly())
			{
				string text = "d:\\ExchangeSetupLogs";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				dataMiningPath = Path.Combine(text, SetupLoggerImpl.setupLogFileName);
			}
			ExWatson.TryAddExtraFile(filename);
			ExWatson.TryAddExtraFile(filename2);
			try
			{
				TaskLogger.IsSetupLogging = true;
				TaskLogger.StartFileLogging(SetupLoggerImpl.setupLogFilePath, dataMiningPath);
				this.isLoggingStarted = true;
			}
			catch (IOException ex)
			{
				throw new SetupLogInitializeException(ex.Message, ex);
			}
			catch (UnauthorizedAccessException ex2)
			{
				throw new SetupLogInitializeException(ex2.Message, ex2);
			}
			this.Log(SetupLoggerImpl.AsterixLine);
			this.Log(Strings.SetupLogStarted);
			this.Log(SetupLoggerImpl.AsterixLine);
			this.Log(Strings.LocalTimeZone(TimeZoneInfo.Local.DisplayName));
			this.Log(Strings.OSVersion(Environment.OSVersion.ToString()));
			try
			{
				this.LogAssemblyVersion();
			}
			catch (FileVersionNotFoundException ex3)
			{
				throw new SetupLogInitializeException(ex3.Message, ex3);
			}
			this.LogUserName();
			this.TaskStartTime = DateTime.UtcNow;
		}

		public void StopLogging()
		{
			if (this.isLoggingStarted)
			{
				this.Log(Strings.SetupLogEnd);
				this.Log(SetupLoggerImpl.AsterixLine);
				this.LogForDataMining(Strings.SetupLogStarted, this.TaskStartTime);
				TaskLogger.StopFileLogging();
				this.isLoggingStarted = false;
			}
		}

		public void Log(LocalizedString localizedString)
		{
			TaskLogger.Log(localizedString);
		}

		public void LogWarning(LocalizedString localizedString)
		{
			TaskLogger.LogWarning(localizedString);
		}

		public void LogError(Exception e)
		{
			TaskLogger.LogError(e);
		}

		public void TraceEnter(params object[] arguments)
		{
			if (!ExTraceGlobals.TraceTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				return;
			}
			StackTrace stackTrace = new StackTrace();
			MethodBase method = stackTrace.GetFrame(1).GetMethod();
			StringBuilder stringBuilder = new StringBuilder();
			string argumentList = string.Empty;
			if (arguments != null)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					stringBuilder.Append((arguments[i] != null) ? arguments[i].ToString() : "null");
					if (i + 1 < arguments.Length)
					{
						stringBuilder.Append(", ");
					}
				}
				argumentList = stringBuilder.ToString();
			}
			ExTraceGlobals.TraceTracer.Information(0L, Strings.TraceFunctionEnter(method.ReflectedType, method.Name, argumentList));
		}

		public void TraceExit()
		{
			if (!ExTraceGlobals.TraceTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				return;
			}
			StackTrace stackTrace = new StackTrace();
			MethodBase method = stackTrace.GetFrame(1).GetMethod();
			ExTraceGlobals.TraceTracer.Information(0L, Strings.TraceFunctionExit(method.ReflectedType, method.Name));
		}

		public void IncreaseIndentation(LocalizedString tag)
		{
			TaskLogger.IncreaseIndentation(tag);
		}

		public void DecreaseIndentation()
		{
			TaskLogger.DecreaseIndentation();
		}

		public void LogForDataMining(string task, DateTime startTime)
		{
			TaskLogger.LogDataMiningMessage(SetupLoggerImpl.BuildVersion, task, startTime);
		}

		private void LogAssemblyVersion()
		{
			SetupLoggerImpl.BuildVersion = ConfigurationContext.Setup.GetExecutingVersion().ToString();
			SetupLogger.Log(Strings.AssemblyVersion(SetupLoggerImpl.BuildVersion));
		}

		private void LogUserName()
		{
			try
			{
				WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				string name = windowsPrincipal.Identity.Name;
				SetupLogger.Log(Strings.UserName(name));
			}
			catch (SecurityException ex)
			{
				this.LogWarning(Strings.UserNameError(ex.Message));
			}
		}

		private const string MsiLogFileName = "ExchangeSetup.msilog";

		private static readonly LocalizedString AsterixLine = new LocalizedString("**********************************************");

		private static readonly string setupLogDirectory = ConfigurationContext.Setup.SetupLoggingPath;

		private static readonly string setupLogFileName = ConfigurationContext.Setup.SetupLogFileName;

		private static readonly string setupLogFileNameForWatson = ConfigurationContext.Setup.SetupLogFileNameForWatson;

		private static readonly string setupLogFilePath = Path.Combine(SetupLoggerImpl.setupLogDirectory, SetupLoggerImpl.setupLogFileName);

		private static string BuildVersion;

		private bool isLoggingStarted;

		private DateTime TaskStartTime;
	}
}
