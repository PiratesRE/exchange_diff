using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips
{
	internal class Constants
	{
		public const string Monitor = "Monitor";

		public const string CollectFIPSLogsResponder = "CollectFIPSLogsResponder";

		public const string EscalateResponder = "EscalateResponder";

		public const string PerfcounterFormatWithInstance = "{0}\\{1}\\{2}";

		public const string CategoryMSExchangeHygieneAntimalware = "MSExchange Hygiene Antimalware";

		public const string CategoryMSExchangeHygieneClassification = "MSExchange Hygiene Classification";

		public const string CounterEngineErrors = "Engine Errors";

		public const string WorkItemPrefixEngineErrors = "EngineErrors";

		public const string AntimalwareType = "Antivirus";

		public const string ClassificationType = "Classification";

		public const string workitemNameFormat = "{0}{1}{2}";

		public const string CmdletGetFailedUpdatesCounter = "Get-Counter";

		public const string CmdletGetEngineUpdateInformation = "Get-EngineUpdateInformation";

		public const string ConsecutiveFailedUpdatesCounterName = "\\msexchange hygiene updates engine(*)\\consecutive failed updates";

		public const string PseudoLocUpdatesEngineCounterCategory = "[MSExchange Hygiene Updates Engine xxx xxx xxx xx]";

		public const string PseudoLocConsecutiveFailedUpdatesCounterName = "\\[msexchange hygiene updates engine xxx xxx xxx xx](*)\\[consecutive failed updates xxx xxx xxx]";

		public const int DefaultConsecutiveFailures = 8;

		public const int MaximumConsecutiveFailures = 24;

		public const int MinimumConsecutiveFailures = 2;

		public const int DefaultFailedEngine = 1;

		public const int MinimumFailedEngine = 1;

		public const int MaximumFailedEngine = 3;
	}
}
