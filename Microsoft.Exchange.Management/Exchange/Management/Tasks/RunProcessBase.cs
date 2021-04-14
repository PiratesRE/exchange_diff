using System;
using System.ComponentModel;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class RunProcessBase : Task
	{
		[Parameter(Mandatory = false)]
		public string Args
		{
			get
			{
				return (string)base.Fields["Args"];
			}
			set
			{
				base.Fields["Args"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Timeout
		{
			get
			{
				if (base.Fields["Timeout"] != null)
				{
					return (int)base.Fields["Timeout"];
				}
				return -1;
			}
			set
			{
				base.Fields["Timeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int[] IgnoreExitCode
		{
			get
			{
				if (base.Fields["IgnoreExitCode"] != null)
				{
					return (int[])base.Fields["IgnoreExitCode"];
				}
				return new int[0];
			}
			set
			{
				base.Fields["IgnoreExitCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint RetryCount
		{
			get
			{
				if (base.Fields["RetryCount"] != null)
				{
					return (uint)base.Fields["RetryCount"];
				}
				return 0U;
			}
			set
			{
				base.Fields["RetryCount"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint RetryDelay
		{
			get
			{
				if (base.Fields["RetryDelay"] != null)
				{
					return (uint)base.Fields["RetryDelay"];
				}
				return 200U;
			}
			set
			{
				base.Fields["RetryDelay"] = value;
			}
		}

		protected string ExeName
		{
			get
			{
				return this.exeName;
			}
			set
			{
				this.exeName = value;
			}
		}

		protected bool IsExitCodeIgnorable(int ExitCode)
		{
			foreach (int num in this.IgnoreExitCode)
			{
				if (ExitCode == num)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void HandleProcessOutput(string outputString, string errorString)
		{
			base.WriteVerbose(Strings.ProcessStandardOutput(outputString));
			base.WriteVerbose(Strings.ProcessStandardError(errorString));
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (string.IsNullOrEmpty(this.ExeName))
			{
				base.WriteError(new ArgumentException(Strings.ErrorFileNameCannotBeEmptyOrNull, "ExeName"), ErrorCategory.InvalidArgument, "ExeName");
			}
			base.WriteVerbose(Strings.ProcessStart(this.ExeName, this.Args));
			int num = 0;
			uint num2 = 1U + this.RetryCount;
			while (num2-- > 0U)
			{
				string outputString;
				string errorString;
				try
				{
					num = ProcessRunner.Run(this.ExeName, this.Args, this.Timeout, null, out outputString, out errorString);
				}
				catch (Win32Exception exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidOperation, null);
					TaskLogger.LogExit();
					return;
				}
				catch (TimeoutException exception2)
				{
					base.WriteError(exception2, ErrorCategory.OperationTimeout, null);
					TaskLogger.LogExit();
					return;
				}
				this.HandleProcessOutput(outputString, errorString);
				if (num == 0)
				{
					continue;
				}
				if (this.IsExitCodeIgnorable(num))
				{
					base.WriteVerbose(Strings.ExceptionRunProcessExitIgnored(num));
					break;
				}
				if (num2 > 0U)
				{
					base.WriteVerbose(Strings.ExceptionRunProcessFailedRetry(num, num2));
					Thread.Sleep((int)this.RetryDelay);
					continue;
				}
				base.WriteError(new TaskException(Strings.ExceptionRunProcessFailed(num)), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		private const string ArgsParameter = "Args";

		private const string TimeoutParameter = "Timeout";

		private const string IgnoreExitCodeParameter = "IgnoreExitCode";

		private const string RetryCountParameter = "RetryCount";

		private const string RetryDelayParameter = "RetryDelay";

		private const int DefaultRetryDelayMs = 200;

		private string exeName;
	}
}
