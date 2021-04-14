using System;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Security;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallCentralAdminServiceTask)]
	[Cmdlet("execute", "sqlcommand")]
	public class ExecuteSqlCommand : ManageSqlDatabase
	{
		[Parameter(Mandatory = true)]
		public string Command
		{
			get
			{
				return (string)base.Fields["Command"];
			}
			set
			{
				base.Fields["Command"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ExecuteScalar
		{
			get
			{
				return (SwitchParameter)(base.Fields["ExecuteScalar"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ExecuteScalar"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ExecuteScript
		{
			get
			{
				return (SwitchParameter)(base.Fields["ExecuteScript"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ExecuteScript"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Hashtable Arguments
		{
			get
			{
				return (Hashtable)(base.Fields["Arguments"] ?? new Hashtable());
			}
			set
			{
				base.Fields["Arguments"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(1, 2147483647)]
		public int Timeout
		{
			get
			{
				return (int)(base.Fields["Timeout"] ?? 0);
			}
			set
			{
				base.Fields["Timeout"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.ExecuteScript)
			{
				this.ExecuteSqlScript();
			}
			else
			{
				base.ExecuteCommand(this.ApplyArguments(this.Command, this.Arguments), this.ExecuteScalar, this.Timeout);
			}
			TaskLogger.LogExit();
		}

		private void ExecuteSqlScript()
		{
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				FileInfo fileInfo = new FileInfo(this.Command);
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						text = text.Trim();
						if (string.Compare(text, "GO", true) == 0)
						{
							base.ExecuteCommand(this.ApplyArguments(stringBuilder.ToString(), this.Arguments), false, this.Timeout);
							stringBuilder.Remove(0, stringBuilder.Length);
						}
						else
						{
							stringBuilder.Append(text);
							stringBuilder.Append(Environment.NewLine);
						}
					}
					base.ExecuteCommand(this.ApplyArguments(stringBuilder.ToString(), this.Arguments), false, this.Timeout);
				}
			}
			catch (SecurityException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (FileNotFoundException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
			}
			catch (UnauthorizedAccessException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
			catch (DirectoryNotFoundException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidOperation, null);
			}
			catch (IOException exception5)
			{
				base.WriteError(exception5, ErrorCategory.InvalidOperation, null);
			}
		}

		private string ApplyArguments(string command, Hashtable arguments)
		{
			if (command != null && arguments != null && arguments.Count != 0)
			{
				foreach (object obj in arguments.Keys)
				{
					string text = (string)obj;
					if (arguments[text] != null)
					{
						string pattern = string.Format("$({0})", text);
						command = ExecuteSqlCommand.Replace(command, pattern, arguments[text].ToString());
					}
				}
			}
			return command;
		}

		private static string Replace(string command, string pattern, string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int num2 = command.IndexOf(pattern, StringComparison.OrdinalIgnoreCase); num2 != -1; num2 = command.IndexOf(pattern, num, StringComparison.OrdinalIgnoreCase))
			{
				stringBuilder.Append(command.Substring(num, num2 - num));
				stringBuilder.Append(value);
				num = num2 + pattern.Length;
			}
			stringBuilder.Append(command.Substring(num));
			return stringBuilder.ToString();
		}
	}
}
