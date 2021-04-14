using System;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants;

namespace Microsoft.Exchange.Assistants.Logging
{
	internal class MailboxAssistantsSlaReportLogFileSettings : ActivityContextLogFileSettings
	{
		protected override string LogSubFolderName
		{
			get
			{
				return "MailboxAssistantsSlaReportLog";
			}
		}

		protected override string LogTypeName
		{
			get
			{
				return "MailboxAssistantsSlaReportLog";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AssistantBaseTracer;
			}
		}

		internal static IDisposable SetAssistantsLogFileSettingsEnabledTestHook(bool assistantsLogFileSettingsEnabled)
		{
			return MailboxAssistantsSlaReportLogFileSettings.logTestHook.SetTestHook(assistantsLogFileSettingsEnabled);
		}

		protected override void LoadSettings()
		{
			if (MailboxAssistantsSlaReportLogFileSettings.logTestHook.Value)
			{
				base.LoadSettings();
				base.DirectoryPath = Path.GetFullPath(Path.Combine(base.DirectoryPath, "..\\" + this.LogSubFolderName));
				return;
			}
			base.Enabled = false;
		}

		internal const string MailboxSlaReportLogName = "MailboxAssistantsSlaReportLog";

		private static readonly Hookable<bool> logTestHook = Hookable<bool>.Create(true, true);
	}
}
