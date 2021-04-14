using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.ClientAccess
{
	[Cmdlet("Install", "ShareIISLogFileDirectory")]
	[LocDescription(Strings.IDs.InstallShareIISLogFileDirectoryTask)]
	public sealed class InstallShareIISLogFileDirectory : Task
	{
		[Parameter(Mandatory = true)]
		public string AppcmdPath
		{
			get
			{
				return this.appcmdPath;
			}
			set
			{
				this.appcmdPath = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string DefaultAdminUserDomain
		{
			get
			{
				return this.defaultAdminUserDomain;
			}
			set
			{
				this.defaultAdminUserDomain = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string text = "list config \"Default Web Site\" -section:System.ApplicationHost/log";
			base.WriteVerbose(Strings.ProcessStart(this.AppcmdPath, text));
			string text2;
			int num;
			if (!this.RunCommand(this.AppcmdPath, text, out text2, out num))
			{
				TaskLogger.LogExit();
				return;
			}
			if (text2 == null)
			{
				base.WriteError(new TaskException(Strings.ErrorRecordReport("outputString is empty", 1)), ErrorCategory.InvalidOperation, null);
				TaskLogger.LogExit();
				return;
			}
			int num2 = text2.IndexOf("centralW3CLogFile", StringComparison.OrdinalIgnoreCase);
			if (num2 < 0)
			{
				base.WriteError(new TaskException(Strings.ErrorRecordReport(string.Format("\"centralW3CLogFile\" is not found in the outputString: \"{0}\"", text2), 2)), ErrorCategory.InvalidOperation, null);
				TaskLogger.LogExit();
				return;
			}
			string text3 = "directory=\"";
			num2 = text2.IndexOf(text3, num2, StringComparison.OrdinalIgnoreCase);
			if (num2 < 0)
			{
				base.WriteError(new TaskException(Strings.ErrorRecordReport(string.Format("\"directory=\"\" is not found in the outputString: \"{0}\"", text2), 3)), ErrorCategory.InvalidOperation, null);
				TaskLogger.LogExit();
				return;
			}
			num2 += text3.Length;
			int num3 = text2.IndexOf('"', num2);
			if (num3 < 0 || num3 == num2)
			{
				base.WriteError(new TaskException(Strings.ErrorRecordReport(string.Format("Closing quote is not found in the outputString: \"{0}\"", text2), 4)), ErrorCategory.InvalidOperation, null);
				TaskLogger.LogExit();
				return;
			}
			string text4 = text2.Substring(num2, num3 - num2);
			char[] separator = new char[]
			{
				'%'
			};
			string[] array = text4.Split(separator, StringSplitOptions.None);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(array[0]);
			int num4 = 1;
			while (num4 + 1 < array.Length)
			{
				stringBuilder.Append(Environment.GetEnvironmentVariable(array[num4]));
				stringBuilder.Append(array[num4 + 1]);
				num4 += 2;
			}
			string text5 = stringBuilder.ToString();
			if (!Directory.Exists(text5))
			{
				Directory.CreateDirectory(text5);
			}
			stringBuilder = new StringBuilder();
			stringBuilder.Append("\"");
			stringBuilder.Append(text5);
			stringBuilder.Append("\"");
			text5 = stringBuilder.ToString();
			string text6 = this.DefaultAdminUserDomain + "\\View-Only Organization Management";
			string text7 = "Cacls";
			text = text5 + " /T /E /G \"" + text6 + ":R\"";
			base.WriteVerbose(Strings.ProcessStart(text7, text));
			if (!this.RunCommand(text7, text, out text2, out num))
			{
				TaskLogger.LogExit();
				return;
			}
			text7 = "net";
			text = string.Concat(new string[]
			{
				"share IISLogs=",
				text5,
				" /GRANT:\"",
				text6,
				",READ\""
			});
			base.WriteVerbose(Strings.ProcessStart(text7, text));
			if (!this.RunCommand(text7, text, 2, out text2, out num))
			{
				TaskLogger.LogExit();
				return;
			}
			TaskLogger.LogExit();
		}

		private bool RunCommand(string command, string args, out string outputString, out int exitCode)
		{
			return this.RunCommand(command, args, 0, out outputString, out exitCode);
		}

		private bool RunCommand(string command, string args, int ignoreExitCode, out string outputString, out int exitCode)
		{
			outputString = null;
			exitCode = 0;
			string output;
			try
			{
				exitCode = ProcessRunner.Run(command, args, 180000, null, out outputString, out output);
			}
			catch (Win32Exception exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				return false;
			}
			catch (TimeoutException exception2)
			{
				base.WriteError(exception2, ErrorCategory.OperationTimeout, null);
				return false;
			}
			base.WriteVerbose(Strings.ProcessStandardOutput(outputString));
			base.WriteVerbose(Strings.ProcessStandardError(output));
			if (exitCode != 0 && exitCode != ignoreExitCode)
			{
				base.WriteError(new TaskException(Strings.ExceptionRunProcessFailed(exitCode)), ErrorCategory.InvalidOperation, null);
				return false;
			}
			return true;
		}

		private string appcmdPath;

		private string defaultAdminUserDomain;
	}
}
