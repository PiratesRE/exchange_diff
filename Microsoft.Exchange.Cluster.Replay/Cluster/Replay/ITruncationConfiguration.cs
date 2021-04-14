using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface ITruncationConfiguration
	{
		string SourceMachine { get; }

		string ServerName { get; }

		Guid IdentityGuid { get; }

		string LogFilePrefix { get; }

		string DestinationLogPath { get; }

		bool CircularLoggingEnabled { get; }
	}
}
