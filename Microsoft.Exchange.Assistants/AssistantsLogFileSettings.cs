using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Assistants
{
	internal class AssistantsLogFileSettings : ActivityContextLogFileSettings
	{
		private AssistantsLogFileSettings()
		{
		}

		internal string[] LogDisabledAssistants { get; private set; }

		protected override string LogTypeName
		{
			get
			{
				return "Mailbox Assistants Log";
			}
		}

		protected override string LogSubFolderName
		{
			get
			{
				return "MailboxAssistantsLog";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AssistantBaseTracer;
			}
		}

		internal static AssistantsLogFileSettings Load()
		{
			return new AssistantsLogFileSettings();
		}

		internal static IDisposable SetAssistantsLogFileSettingsEnabledTestHook(bool assistantsLogFileSettingsEnabled)
		{
			return AssistantsLogFileSettings.hookableAssistantsLogFileSettingsEnabled.SetTestHook(assistantsLogFileSettingsEnabled);
		}

		protected override void LoadSettings()
		{
			if (AssistantsLogFileSettings.hookableAssistantsLogFileSettingsEnabled.Value)
			{
				base.LoadSettings();
				return;
			}
			base.Enabled = false;
		}

		protected override void LoadCustomSettings()
		{
			base.LoadCustomSettings();
			StringArrayAppSettingsEntry stringArrayAppSettingsEntry = new StringArrayAppSettingsEntry("LogDisabledAssistants", new string[0], this.Tracer);
			this.LogDisabledAssistants = stringArrayAppSettingsEntry.Value;
		}

		internal const string AssistantsLogSubFolderName = "MailboxAssistantsLog";

		private static readonly Hookable<bool> hookableAssistantsLogFileSettingsEnabled = Hookable<bool>.Create(true, true);
	}
}
