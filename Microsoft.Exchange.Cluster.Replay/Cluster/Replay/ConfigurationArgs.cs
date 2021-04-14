using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConfigurationArgs
	{
		public bool IsTestEnvironment { get; private set; }

		public ConfigurationArgs(IReplayConfiguration config, IReplicaInstanceManager replicaInstanceManager) : this(config, replicaInstanceManager, false)
		{
		}

		internal ConfigurationArgs(IReplayConfiguration config, IReplicaInstanceManager replicaInstanceManager, bool isTestCode)
		{
			this.IsTestEnvironment = isTestCode;
			this.m_replicaInstanceManager = replicaInstanceManager;
			this.m_identityGuid = config.IdentityGuid;
			this.m_name = config.Name;
			this.m_logFilePrefix = config.LogFilePrefix;
			this.m_destinationSystemPath = config.DestinationSystemPath;
			this.m_destinationEdbPath = config.DestinationEdbPath;
			this.m_destinationLogPath = config.DestinationLogPath;
			this.m_inspectorLogPath = config.LogInspectorPath;
			this.m_ignoredLogsPath = config.E00LogBackupPath;
			this.m_sourceMachine = config.SourceMachine;
			this.m_LogFileSuffix = config.LogFileSuffix;
			this.m_autoDagVolumesRootFolderPath = config.AutoDagVolumesRootFolderPath;
			this.m_autoDagDatabasesRootFolderPath = config.AutoDagDatabasesRootFolderPath;
			this.m_autoDagDatabaseCopiesPerVolume = config.AutoDagDatabaseCopiesPerVolume;
			this.CircularLoggingEnabled = config.CircularLoggingEnabled;
			this.BuildDebugString();
		}

		public Guid IdentityGuid
		{
			get
			{
				return this.m_identityGuid;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public string SourceEdbPath
		{
			get
			{
				return this.m_destinationEdbPath;
			}
		}

		public string SourceMachine
		{
			get
			{
				return this.m_sourceMachine;
			}
		}

		public string LogFilePrefix
		{
			get
			{
				return this.m_logFilePrefix;
			}
		}

		public string LogFileSuffix
		{
			get
			{
				return this.m_LogFileSuffix;
			}
		}

		public string DestinationSystemPath
		{
			get
			{
				return this.m_destinationSystemPath;
			}
		}

		public string DestinationEdbPath
		{
			get
			{
				return this.m_destinationEdbPath;
			}
		}

		public string DestinationLogPath
		{
			get
			{
				return this.m_destinationLogPath;
			}
		}

		public string InspectorLogPath
		{
			get
			{
				return this.m_inspectorLogPath;
			}
		}

		public string IgnoredLogsPath
		{
			get
			{
				return this.m_ignoredLogsPath;
			}
		}

		public string AutoDagVolumesRootFolderPath
		{
			get
			{
				return this.m_autoDagVolumesRootFolderPath;
			}
		}

		public string AutoDagDatabasesRootFolderPath
		{
			get
			{
				return this.m_autoDagDatabasesRootFolderPath;
			}
		}

		public int AutoDagDatabaseCopiesPerVolume
		{
			get
			{
				return this.m_autoDagDatabaseCopiesPerVolume;
			}
		}

		public IReplicaInstanceManager ReplicaInstanceManager
		{
			get
			{
				return this.m_replicaInstanceManager;
			}
		}

		internal bool CircularLoggingEnabled { get; private set; }

		public override string ToString()
		{
			return this.m_debugString;
		}

		private void BuildDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("ConfigurationArgs: IdentityGuid='{0}',", this.m_identityGuid.ToString());
			stringBuilder.AppendFormat("Name='{0}',", this.m_name);
			stringBuilder.AppendFormat("CircularLoggingEnabled='{0}',", this.CircularLoggingEnabled);
			stringBuilder.AppendFormat("SourceMachine='{0}',", this.m_sourceMachine);
			stringBuilder.AppendFormat("LogFilePrefix='{0}',", this.m_logFilePrefix);
			stringBuilder.AppendFormat("LogFileSuffix='{0}',", this.m_LogFileSuffix);
			stringBuilder.AppendFormat("DestinationSystemPath='{0}',", this.m_destinationSystemPath);
			stringBuilder.AppendFormat("DestinationEdbPath='{0}',", this.m_destinationEdbPath);
			stringBuilder.AppendFormat("DestinationLogPath='{0}',", this.m_destinationLogPath);
			stringBuilder.AppendFormat("InspectorLogPath='{0}',", this.m_inspectorLogPath);
			stringBuilder.AppendFormat("IgnoredLogsPath='{0}',", this.m_ignoredLogsPath);
			stringBuilder.AppendFormat("AutoDagVolumesRootFolderPath='{0}',", this.m_autoDagVolumesRootFolderPath);
			stringBuilder.AppendFormat("AutoDagDatabasesRootFolderPath='{0}',", this.m_autoDagDatabasesRootFolderPath);
			stringBuilder.AppendFormat("AutoDagDatabaseCopiesPerVolume='{0}',", this.m_autoDagDatabaseCopiesPerVolume);
			this.m_debugString = stringBuilder.ToString();
		}

		private readonly Guid m_identityGuid;

		private readonly string m_name;

		private readonly string m_logFilePrefix;

		private readonly string m_destinationSystemPath;

		private readonly string m_destinationEdbPath;

		private readonly string m_destinationLogPath;

		private readonly string m_ignoredLogsPath;

		private readonly string m_inspectorLogPath;

		private readonly string m_sourceMachine;

		private readonly string m_LogFileSuffix;

		private readonly string m_autoDagVolumesRootFolderPath;

		private readonly string m_autoDagDatabasesRootFolderPath;

		private readonly int m_autoDagDatabaseCopiesPerVolume;

		private readonly IReplicaInstanceManager m_replicaInstanceManager;

		private string m_debugString;
	}
}
