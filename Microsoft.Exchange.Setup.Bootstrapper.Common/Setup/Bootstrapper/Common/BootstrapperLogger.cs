using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Setup.CommonBase;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	internal class BootstrapperLogger : IBootstrapperLogger
	{
		public void StartLogging()
		{
			if (!Directory.Exists(BootstrapperLogger.setupLogDirectory))
			{
				Directory.CreateDirectory(BootstrapperLogger.setupLogDirectory);
			}
			try
			{
				BootstrapperLogger.indentationLevel = 0;
				FileStream stream = new FileStream(BootstrapperLogger.setupLogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				BootstrapperLogger.sw = new StreamWriter(stream);
				BootstrapperLogger.sw.AutoFlush = true;
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
			this.Log(Strings.AsterixLine);
			this.Log(Strings.SetupLogStarted);
			this.Log(Strings.AsterixLine);
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
		}

		public void StopLogging()
		{
			if (this.isLoggingStarted)
			{
				this.Log(Strings.SetupLogEnd);
				this.Log(Strings.AsterixLine);
				this.isLoggingStarted = false;
				BootstrapperLogger.sw.Close();
				BootstrapperLogger.sw = null;
			}
		}

		public void Log(LocalizedString localizedString)
		{
			if (this.isLoggingStarted)
			{
				BootstrapperLogger.LogMessageString(localizedString.ToString());
			}
		}

		public void LogWarning(LocalizedString localizedString)
		{
			if (this.isLoggingStarted)
			{
				BootstrapperLogger.LogWarningString(localizedString.ToString());
			}
		}

		public void LogError(Exception e)
		{
			while (e != null)
			{
				LocalizedException ex = e as LocalizedException;
				string message;
				if (ex != null)
				{
					ex.FormatProvider = new CultureInfo("en-US");
					message = ex.Message;
				}
				else
				{
					message = e.Message;
				}
				if (this.isLoggingStarted)
				{
					BootstrapperLogger.LogErrorString(message);
				}
				e = e.InnerException;
			}
		}

		public void IncreaseIndentation(LocalizedString tag)
		{
			if (this.isLoggingStarted)
			{
				BootstrapperLogger.indentationLevel++;
				if (!string.IsNullOrEmpty(tag))
				{
					this.Log(tag);
				}
			}
		}

		public void DecreaseIndentation()
		{
			if (this.isLoggingStarted)
			{
				BootstrapperLogger.indentationLevel--;
			}
		}

		private static void LogErrorString(string message)
		{
			BootstrapperLogger.LogMessageString("[ERROR] " + message);
		}

		private static void LogMessageString(string message)
		{
			try
			{
				DateTime utcNow = DateTime.UtcNow;
				BootstrapperLogger.sw.WriteLine(string.Format("[{0}.{1:0000}] [{2}] {3}", new object[]
				{
					utcNow.ToString("MM/dd/yyyy HH:mm:ss"),
					utcNow.Millisecond,
					BootstrapperLogger.indentationLevel,
					message
				}));
			}
			catch (IOException)
			{
			}
		}

		private static void LogWarningString(string message)
		{
			BootstrapperLogger.LogMessageString("[WARNING] " + message);
		}

		private static string GetExecutingVersion()
		{
			string result = string.Empty;
			string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Setup\\ServerRoles\\Common");
			if (!Directory.Exists(text))
			{
				text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
			string text2 = Path.Combine(text, "ExSetup.exe");
			if (File.Exists(text2))
			{
				string versionOfApplication = SetupHelper.GetVersionOfApplication(text2);
				if (!string.IsNullOrEmpty(versionOfApplication))
				{
					result = new Version(versionOfApplication).ToString();
				}
			}
			return result;
		}

		private void LogAssemblyVersion()
		{
			this.Log(Strings.AssemblyVersion(BootstrapperLogger.GetExecutingVersion()));
		}

		private void LogUserName()
		{
			try
			{
				WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				string name = windowsPrincipal.Identity.Name;
				this.Log(Strings.UserName(name));
			}
			catch (SecurityException ex)
			{
				this.LogWarning(Strings.UserNameError(ex.Message));
			}
		}

		private const int MinimumIndentationLevel = 0;

		private const int MaximumIndentationLevel = 2;

		private const string ErrorTag = "[ERROR] ";

		private const string WarningTag = "[WARNING] ";

		private static readonly string setupLogDirectory = Environment.ExpandEnvironmentVariables("%systemdrive%\\ExchangeSetupLogs");

		private static readonly string setupLogFileName = "ExchangeSetupBootStrapper.log";

		private static readonly string setupLogFilePath = Path.Combine(BootstrapperLogger.setupLogDirectory, BootstrapperLogger.setupLogFileName);

		private static StreamWriter sw;

		private static int indentationLevel;

		private bool isLoggingStarted;
	}
}
