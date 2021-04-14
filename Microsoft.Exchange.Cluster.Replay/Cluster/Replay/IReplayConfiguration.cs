using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IReplayConfiguration
	{
		bool ConfigEquals(IReplayConfiguration other, out ReplayConfigChangedFlags changedFlags);

		bool IsSourceMachineEqual(AmServerName sourceServer);

		bool AllowFileRestore { get; }

		ReplayConfigType Type { get; }

		bool IsPassiveCopy { get; }

		IADDatabase Database { get; }

		string Name { get; }

		string DisplayName { get; }

		string Identity { get; }

		string DatabaseDn { get; }

		Guid IdentityGuid { get; }

		Guid DatabaseGuid { get; }

		string LogFilePrefix { get; }

		string LogFileSuffix { get; }

		string LogExtension { get; }

		string LogInspectorPath { get; }

		string E00LogBackupPath { get; }

		string DatabaseName { get; }

		bool IsPublicFolderDatabase { get; }

		bool DatabaseIsPrivate { get; }

		bool CircularLoggingEnabled { get; }

		string ServerName { get; }

		int ServerVersion { get; }

		ReplayState ReplayState { get; }

		string SourceMachine { get; }

		string TargetMachine { get; }

		EnhancedTimeSpan ReplayLagTime { get; }

		EnhancedTimeSpan TruncationLagTime { get; }

		bool DatabaseCreated { get; }

		string DestinationEdbPath { get; }

		string DestinationSystemPath { get; }

		string DestinationLogPath { get; }

		string SourceEdbPath { get; }

		string SourceSystemPath { get; }

		string SourceLogPath { get; }

		string AutoDagVolumesRootFolderPath { get; }

		string AutoDagDatabasesRootFolderPath { get; }

		int AutoDagDatabaseCopiesPerVolume { get; }

		string GetXmlDescription(JET_SIGNATURE logfileSignature);

		IADServer GetAdServerObject();

		string BuildShortLogfileName(long generation);

		string BuildFullLogfileName(long generation);

		void UpdateLastLogGeneratedAndEndOfLogInfo(long highestLogGen);
	}
}
