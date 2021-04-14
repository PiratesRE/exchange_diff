using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MrsAndProxyLoggerSettings : ActivityContextLogFileSettings
	{
		protected override string LogTypeName
		{
			get
			{
				return "Mailbox Replication Log";
			}
		}

		protected override string LogSubFolderName
		{
			get
			{
				return "MailboxReplicationService";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MailboxReplicationServiceTracer;
			}
		}

		internal static MrsAndProxyLoggerSettings Load()
		{
			return new MrsAndProxyLoggerSettings();
		}

		private MrsAndProxyLoggerSettings()
		{
		}

		internal const string LoggerSubFolderName = "MailboxReplicationService";
	}
}
