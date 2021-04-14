using System;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface IMessagingDatabaseConfig
	{
		string DatabasePath { get; }

		string LogFilePath { get; }

		uint LogFileSize { get; }

		uint LogBufferSize { get; }

		uint ExtensionSize { get; }

		uint MaxBackgroundCleanupTasks { get; }

		int MaxConnections { get; }

		DatabaseRecoveryAction DatabaseRecoveryAction { get; }

		TimeSpan MessagingGenerationCleanupAge { get; }

		TimeSpan MessagingGenerationExpirationAge { get; }

		TimeSpan MessagingGenerationLength { get; }

		TimeSpan DefaultAsyncCommitTimeout { get; }

		byte MaxMessageLoadTimePercentage { get; }

		int RecentGenerationDepth { get; }

		TimeSpan StatisticsUpdateInterval { get; }

		bool CloneInOriginalGeneration { get; }
	}
}
