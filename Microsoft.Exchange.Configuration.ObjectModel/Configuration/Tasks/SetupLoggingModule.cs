using System;
using System.Collections;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class SetupLoggingModule : ITaskModule, ICriticalFeature
	{
		public SetupLoggingModule(TaskContext context)
		{
			this.context = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			if (this.context.InvocationInfo.IsVerboseOn && !string.Equals(this.context.InvocationInfo.CommandName, "Write-ExchangeSetupLog", StringComparison.OrdinalIgnoreCase))
			{
				task.PreInit += this.WriteCommandParams;
				task.PreRelease += this.ResetTaskLoggerSetting;
			}
		}

		public void Dispose()
		{
		}

		private void WriteCommandParams(object sender, EventArgs e)
		{
			TaskInvocationInfo invocationInfo = this.context.InvocationInfo;
			this.savedLogAllAsInfo = TaskLogger.LogAllAsInfo;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in invocationInfo.UserSpecifiedParameters.Keys)
			{
				string text = (string)obj;
				object obj2 = invocationInfo.UserSpecifiedParameters[text];
				if (obj2 != null)
				{
					try
					{
						stringBuilder.AppendFormat(" -{0}:{1}", text, SetupLoggingModule.GetPSValue(obj2));
						goto IL_8C;
					}
					catch (Exception e2)
					{
						this.context.CommandShell.WriteVerbose(Strings.VerboseTaskParameterLoggingFailed(text, e2));
						goto IL_8C;
					}
					goto IL_7F;
				}
				goto IL_7F;
				IL_8C:
				if (string.Equals(text, "ErrorAction", StringComparison.OrdinalIgnoreCase) && (ActionPreference)obj2 == ActionPreference.SilentlyContinue)
				{
					TaskLogger.LogAllAsInfo = true;
					continue;
				}
				continue;
				IL_7F:
				stringBuilder.AppendFormat(" -{0}:$null", text);
				goto IL_8C;
			}
			this.context.CommandShell.WriteVerbose(Strings.VerboseTaskSpecifiedParameters(stringBuilder.ToString()));
		}

		private void ResetTaskLoggerSetting(object sender, EventArgs e)
		{
			TaskLogger.LogAllAsInfo = this.savedLogAllAsInfo;
		}

		private static string GetPSValue(object value)
		{
			return SetupLoggingModule.GetPSValue(value, false);
		}

		private static string GetPSValue(object value, bool decorateArray)
		{
			if (value == null)
			{
				return "$null";
			}
			if (value is IEnumerable && !(value is string))
			{
				bool flag = true;
				StringBuilder stringBuilder = new StringBuilder();
				if (decorateArray)
				{
					stringBuilder.Append("@(");
				}
				foreach (object value2 in ((IEnumerable)value))
				{
					if (!flag)
					{
						stringBuilder.Append(',');
					}
					flag = false;
					stringBuilder.Append(SetupLoggingModule.GetPSValue(value2, true));
				}
				if (decorateArray)
				{
					stringBuilder.Append(")");
				}
				return stringBuilder.ToString();
			}
			return string.Format("'{0}'", value.ToString().Replace("'", "''"));
		}

		private readonly TaskContext context;

		private bool savedLogAllAsInfo;
	}
}
