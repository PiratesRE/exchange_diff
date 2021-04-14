using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleCentricTriage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleCentricTriageConfiguration
	{
		public PeopleCentricTriageConfiguration(string exchangeInstallPath, TimeSpan skipMailboxInactivityThreshold)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("exchangeInstallPath", exchangeInstallPath);
			this.exchangeInstallPath = exchangeInstallPath;
			this.skipMailboxInactivityThreshold = skipMailboxInactivityThreshold;
			this.assistantLoggingPath = this.ComputeAssistantLoggingPath();
		}

		public PeopleCentricTriageConfiguration(string exchangeInstallPath) : this(exchangeInstallPath, PeopleCentricTriageConfiguration.DefaultSkipMailboxInactivityThreshold)
		{
		}

		public TimeSpan SkipMailboxInactivityThreshold
		{
			get
			{
				return this.skipMailboxInactivityThreshold;
			}
		}

		public string AssistantLoggingPath
		{
			get
			{
				return this.assistantLoggingPath;
			}
		}

		public TimeSpan LogFileMaxAge
		{
			get
			{
				return PeopleCentricTriageConfiguration.DefaultLogFileMaxAge;
			}
		}

		public long LogDirectoryMaxSize
		{
			get
			{
				return 104857600L;
			}
		}

		public long LogFileMaxSize
		{
			get
			{
				return 10485760L;
			}
		}

		private string ComputeAssistantLoggingPath()
		{
			return Path.Combine(this.exchangeInstallPath, "Logging\\PeopleCentricTriageAssistant");
		}

		internal const string AssistantLoggingComponentName = "PeopleCentricTriageAssistant";

		internal const string AssistantLogFilePrefix = "PeopleCentricTriageAssistant";

		private const long DefaultLogDirectoryMaxSize = 104857600L;

		private const long DefaultLogFileMaxSize = 10485760L;

		private static readonly TimeSpan DefaultLogFileMaxAge = TimeSpan.FromDays(90.0);

		private static readonly TimeSpan DefaultSkipMailboxInactivityThreshold = TimeSpan.FromDays(60.0);

		private readonly string exchangeInstallPath;

		private readonly string assistantLoggingPath;

		private readonly TimeSpan skipMailboxInactivityThreshold;
	}
}
