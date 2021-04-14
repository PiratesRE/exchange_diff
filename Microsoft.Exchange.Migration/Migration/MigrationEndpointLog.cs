using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal static class MigrationEndpointLog
	{
		public static void LogStatusEvent(MigrationEndpoint migrationObject, MigrationEndpointLog.EndpointState state)
		{
			MigrationEndpointLog.MigrationEndpointLogEntry migrationObject2 = new MigrationEndpointLog.MigrationEndpointLogEntry(migrationObject, state);
			MigrationEndpointLog.MigrationEndpointLogger.LogEvent(migrationObject2);
		}

		internal enum EndpointState
		{
			Created,
			Updated,
			Deleted
		}

		internal class MigrationEndpointLogEntry
		{
			public MigrationEndpointLogEntry(MigrationEndpoint endpoint, MigrationEndpointLog.EndpointState state)
			{
				this.Endpoint = endpoint;
				this.State = state;
			}

			public readonly MigrationEndpoint Endpoint;

			public readonly MigrationEndpointLog.EndpointState State;
		}

		private class MigrationEndpointLogger : MigrationObjectLog<MigrationEndpointLog.MigrationEndpointLogEntry, MigrationEndpointLog.MigrationEndpointLogSchema, MigrationEndpointLog.MigrationEndpointLogConfiguration>
		{
			public static void LogEvent(MigrationEndpointLog.MigrationEndpointLogEntry migrationObject)
			{
				MigrationObjectLog<MigrationEndpointLog.MigrationEndpointLogEntry, MigrationEndpointLog.MigrationEndpointLogSchema, MigrationEndpointLog.MigrationEndpointLogConfiguration>.Write(migrationObject);
			}
		}

		private class MigrationEndpointLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Migration";
				}
			}

			public override string LogType
			{
				get
				{
					return "Migration Endpoint";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointGuid = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointGuid", (MigrationEndpointLog.MigrationEndpointLogEntry d) => d.Endpoint.Guid);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointName = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointName", (MigrationEndpointLog.MigrationEndpointLogEntry d) => d.Endpoint.Identity.Id);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointType = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointType", (MigrationEndpointLog.MigrationEndpointLogEntry d) => d.Endpoint.EndpointType);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointRemoteServer = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointRemoteServer", delegate(MigrationEndpointLog.MigrationEndpointLogEntry d)
			{
				if (d.Endpoint.EndpointType != MigrationType.ExchangeOutlookAnywhere && d.Endpoint.EndpointType != MigrationType.PublicFolder)
				{
					return d.Endpoint.RemoteServer;
				}
				return d.Endpoint.RpcProxyServer;
			});

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointMailboxPermission = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointMailboxPermission", (MigrationEndpointLog.MigrationEndpointLogEntry d) => d.Endpoint.MailboxPermission);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointMaxConcurrentMigrations = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointMaxConcurrentMigrations", (MigrationEndpointLog.MigrationEndpointLogEntry d) => d.Endpoint.MaxConcurrentMigrations);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointMaxConcurrentIncrementalSyncs = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointMaxConcurrentIncrementalSyncs", (MigrationEndpointLog.MigrationEndpointLogEntry d) => d.Endpoint.MaxConcurrentIncrementalSyncs);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointLastModifiedTime = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointLastModifiedTime", (MigrationEndpointLog.MigrationEndpointLogEntry d) => d.Endpoint.LastModifiedTime);

			public static readonly ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry> EndpointState = new ObjectLogSimplePropertyDefinition<MigrationEndpointLog.MigrationEndpointLogEntry>("EndpointState", (MigrationEndpointLog.MigrationEndpointLogEntry d) => d.State);
		}

		private class MigrationEndpointLogConfiguration : MigrationObjectLogConfiguration
		{
			public override string LoggingFolder
			{
				get
				{
					return base.LoggingFolder + "\\MigrationEndpoint";
				}
			}

			public override long MaxLogDirSize
			{
				get
				{
					return ConfigBase<MigrationServiceConfigSchema>.GetConfig<long>("MigrationReportingEndpointMaxDirSizeKey");
				}
			}

			public override long MaxLogFileSize
			{
				get
				{
					return ConfigBase<MigrationServiceConfigSchema>.GetConfig<long>("MigrationReportingEndpointMaxFileSize");
				}
			}

			public override string LogComponentName
			{
				get
				{
					return "MigrationEndpointLog";
				}
			}

			public override string FilenamePrefix
			{
				get
				{
					return "MigrationEndpoint_Log_";
				}
			}
		}
	}
}
