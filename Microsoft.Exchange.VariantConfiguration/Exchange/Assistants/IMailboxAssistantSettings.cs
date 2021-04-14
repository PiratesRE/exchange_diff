using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IMailboxAssistantSettings : ISettings
	{
		bool Enabled { get; }

		TimeSpan MailboxNotInterestingLogInterval { get; }

		bool SpreadLoad { get; }

		bool SlaMonitoringEnabled { get; }

		bool CompletionMonitoringEnabled { get; }

		bool ActiveDatabaseProcessingMonitoringEnabled { get; }

		float SlaUrgentThreshold { get; }

		float SlaNonUrgentThreshold { get; }
	}
}
