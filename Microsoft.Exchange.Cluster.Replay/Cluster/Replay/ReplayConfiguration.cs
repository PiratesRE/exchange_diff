using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class ReplayConfiguration : IReplayConfiguration, ITruncationConfiguration
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ReplayConfigurationTracer;
			}
		}

		private static QueryFilter BuildServerFilterForSiteWithServerRole(ADObjectId siteId, ServerRole serverRole)
		{
			if (siteId == null)
			{
				return new BitMaskAndFilter(ServerSchema.CurrentServerRole, (ulong)((long)serverRole));
			}
			return new AndFilter(new QueryFilter[]
			{
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, (ulong)((long)serverRole)),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, siteId)
			});
		}

		private static bool StringArraysEqual(string[] array1, string[] array2)
		{
			if (array1.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static List<string> GetFrontendTransportServersInLocalSite()
		{
			return ReplayConfiguration.GetServersWithServerRoleInSite(null, ServerRole.FrontendTransport);
		}

		public static List<string> GetBHServersInSite(string serverName)
		{
			return ReplayConfiguration.GetServersWithServerRoleInSite(serverName, ServerRole.HubTransport);
		}

		public static List<string> GetServersWithServerRoleInSite(string serverName, ServerRole serverRole)
		{
			IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
			IADServer iadserver = null;
			if (!string.IsNullOrEmpty(serverName))
			{
				if (!SharedHelper.StringIEquals(serverName, "localhost"))
				{
					goto IL_2F;
				}
			}
			try
			{
				iadserver = iadtoplogyConfigurationSession.FindServerByName(Environment.MachineName);
				goto IL_5A;
			}
			catch (LocalServerNotFoundException)
			{
				goto IL_5A;
			}
			IL_2F:
			string nodeNameFromFqdn = MachineName.GetNodeNameFromFqdn(serverName);
			iadserver = iadtoplogyConfigurationSession.FindServerByName(nodeNameFromFqdn);
			string arg = "FindServerByName";
			if (iadserver == null)
			{
				ExTraceGlobals.ReplayConfigurationTracer.TraceDebug<string, string>(0L, "GetServersWithServerRoleInSite: {0} didn't find any server for {1}", arg, serverName);
			}
			IL_5A:
			return ReplayConfiguration.GetServersWithServerRoleInSiteByServer(iadserver, serverRole);
		}

		public static List<string> GetServersWithServerRoleInSiteByServer(IADServer mailboxServer, ServerRole serverRole)
		{
			List<string> list = new List<string>();
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 328, "GetServersWithServerRoleInSiteByServer", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\Core\\ReplayConfiguration.cs");
			ADObjectId siteId = null;
			if (mailboxServer != null && mailboxServer.ServerSite != null)
			{
				siteId = mailboxServer.ServerSite;
			}
			try
			{
				QueryFilter filter = ReplayConfiguration.BuildServerFilterForSiteWithServerRole(siteId, serverRole);
				ADPagedReader<Server> adpagedReader = topologyConfigurationSession.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
				foreach (Server server in adpagedReader)
				{
					if (mailboxServer != null && server.MajorVersion != mailboxServer.MajorVersion)
					{
						ExTraceGlobals.ReplayConfigurationTracer.TraceDebug<string, int, int>(0L, "GetServersWithServerRoleInSiteByServer: Filtering Server {0}, major version {1} doesn't match {2}", server.Name, server.MajorVersion, mailboxServer.MajorVersion);
					}
					else
					{
						ExTraceGlobals.ReplayConfigurationTracer.TraceDebug<string>(0L, "GetServersWithServerRoleInSiteByServer: Found Server {0}", server.Name);
						list.Add(server.Name);
					}
				}
				return list;
			}
			catch (ADTransientException arg)
			{
				ExTraceGlobals.ReplayConfigurationTracer.TraceDebug<ADTransientException>(0L, "GetServersWithServerRoleInSiteByServer: Exception {0}", arg);
			}
			catch (ADOperationException arg2)
			{
				ExTraceGlobals.ReplayConfigurationTracer.TraceDebug<ADOperationException>(0L, "GetServersWithServerRoleInSiteByServer: Exception {0}", arg2);
			}
			catch (DataValidationException arg3)
			{
				ExTraceGlobals.ReplayConfigurationTracer.TraceDebug<DataValidationException>(0L, "GetServersWithServerRoleInSiteByServer: Exception {0}", arg3);
			}
			return list;
		}

		public virtual bool ConfigEquals(IReplayConfiguration other, out ReplayConfigChangedFlags changedFlags)
		{
			changedFlags = ReplayConfigChangedFlags.None;
			string b = null;
			string b2 = null;
			string b3 = null;
			string b4 = null;
			string b5 = null;
			string b6 = null;
			AutoDatabaseMountDial autoDatabaseMountDial = AutoDatabaseMountDial.Lossless;
			AutoDatabaseMountDial autoDatabaseMountDial2 = AutoDatabaseMountDial.Lossless;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			StringBuilder stringBuilder = new StringBuilder();
			ReplayConfigType type;
			string name;
			Guid identityGuid;
			string sourceMachine;
			string logFilePrefix;
			string destinationLogPath;
			string destinationSystemPath;
			string destinationEdbPath;
			bool circularLoggingEnabled;
			EnhancedTimeSpan replayLagTime;
			EnhancedTimeSpan truncationLagTime;
			bool databaseCreated;
			int num;
			try
			{
				type = this.Type;
				name = this.Name;
				identityGuid = this.IdentityGuid;
				sourceMachine = this.SourceMachine;
				logFilePrefix = this.LogFilePrefix;
				destinationLogPath = this.DestinationLogPath;
				destinationSystemPath = this.DestinationSystemPath;
				destinationEdbPath = this.DestinationEdbPath;
				circularLoggingEnabled = this.CircularLoggingEnabled;
				replayLagTime = this.ReplayLagTime;
				truncationLagTime = this.TruncationLagTime;
				databaseCreated = this.DatabaseCreated;
				if (type == ReplayConfigType.RemoteCopySource || type == ReplayConfigType.RemoteCopyTarget)
				{
					flag = true;
					autoDatabaseMountDial = ((RemoteReplayConfiguration)this).AutoDatabaseMountDial;
					num = ((RemoteReplayConfiguration)this).Database.DatabaseCopies.Length;
				}
				else
				{
					num = 1;
				}
			}
			catch (TransientException)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ReplayConfiguration.ConfigEquals() returning false because a TransientException was encountered.");
				return false;
			}
			ReplayConfigType type2;
			Guid identityGuid2;
			bool circularLoggingEnabled2;
			EnhancedTimeSpan replayLagTime2;
			EnhancedTimeSpan truncationLagTime2;
			bool databaseCreated2;
			int num2;
			try
			{
				type2 = other.Type;
				b = other.Name;
				identityGuid2 = other.IdentityGuid;
				b6 = other.SourceMachine;
				b2 = other.LogFilePrefix;
				b3 = other.DestinationLogPath;
				b4 = other.DestinationSystemPath;
				b5 = other.DestinationEdbPath;
				circularLoggingEnabled2 = other.CircularLoggingEnabled;
				replayLagTime2 = other.ReplayLagTime;
				truncationLagTime2 = other.TruncationLagTime;
				databaseCreated2 = other.DatabaseCreated;
				if (type2 == ReplayConfigType.RemoteCopySource || type2 == ReplayConfigType.RemoteCopyTarget)
				{
					flag2 = true;
					autoDatabaseMountDial2 = ((RemoteReplayConfiguration)other).AutoDatabaseMountDial;
					num2 = ((RemoteReplayConfiguration)other).Database.DatabaseCopies.Length;
				}
				else
				{
					num2 = 1;
				}
			}
			catch (TransientException)
			{
				return true;
			}
			if (num != num2)
			{
				flag3 = false;
				stringBuilder.AppendLine("origCopyCount != newCopyCount");
			}
			if (type != type2)
			{
				flag3 = false;
				changedFlags |= ReplayConfigChangedFlags.ActiveServer;
				stringBuilder.AppendLine("origType != newType");
			}
			if (name != b)
			{
				flag3 = false;
				stringBuilder.AppendLine("origName != newName");
			}
			if (identityGuid != identityGuid2)
			{
				flag3 = false;
				stringBuilder.AppendLine("origIdentityGuid != newIdentityGuid");
			}
			if (sourceMachine != b6)
			{
				flag3 = false;
				changedFlags |= ReplayConfigChangedFlags.ActiveServer;
				stringBuilder.AppendLine("origSourceMachine != newSourceMachine");
			}
			if (logFilePrefix != b2)
			{
				flag3 = false;
				stringBuilder.AppendLine("origLogFilePrefix != newLogFilePrefix");
			}
			if (destinationLogPath != b3)
			{
				flag3 = false;
				stringBuilder.AppendLine("origDestinationLogPath != newDestinationLogPath");
			}
			if (destinationSystemPath != b4)
			{
				flag3 = false;
				stringBuilder.AppendLine("origDestinationSystemPath != newDestinationSystemPath");
			}
			if (destinationEdbPath != b5)
			{
				flag3 = false;
				stringBuilder.AppendLine("origDestinationEdbPath != newDestinationEdbPath");
			}
			if (circularLoggingEnabled != circularLoggingEnabled2)
			{
				flag3 = false;
				stringBuilder.AppendLine("origCircularLoggingEnabled != newCircularLoggingEnabled");
			}
			if (replayLagTime != replayLagTime2)
			{
				flag3 = false;
				stringBuilder.AppendLine("origReplayLagTime != newReplayLagTime");
			}
			if (truncationLagTime != truncationLagTime2)
			{
				flag3 = false;
				stringBuilder.AppendLine("origTruncationLagTime != newTruncationLagTime");
			}
			if (flag && flag2 && autoDatabaseMountDial != autoDatabaseMountDial2)
			{
				flag3 = false;
				stringBuilder.AppendLine("origMountDial != newMountDial");
			}
			if (databaseCreated != databaseCreated2)
			{
				flag3 = false;
				stringBuilder.AppendLine("origDatabaseCreated != newDatabaseCreated");
			}
			if (!flag3)
			{
				changedFlags |= ReplayConfigChangedFlags.Other;
				ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "{0}: ReplayConfiguration.ConfigEquals() returning false because of the following: {1}ChangedFlags:{2}; Reason:{3}", new object[]
				{
					this.m_displayName,
					Environment.NewLine,
					changedFlags,
					stringBuilder.ToString()
				});
			}
			return flag3;
		}

		public bool IsSourceMachineEqual(AmServerName sourceServer)
		{
			AmServerName src = new AmServerName(this.SourceMachine);
			return AmServerName.IsEqual(src, sourceServer);
		}

		public virtual IADDatabase Database
		{
			get
			{
				if (this.m_database == null)
				{
					throw new ReplayConfigPropException(this.m_identity, "Database");
				}
				return this.m_database;
			}
		}

		public IADServer GetAdServerObject()
		{
			return this.m_server;
		}

		public virtual bool AllowFileRestore
		{
			get
			{
				return this.Database.AllowFileRestore;
			}
		}

		public virtual ReplayConfigType Type
		{
			get
			{
				return this.m_type;
			}
		}

		public bool IsPassiveCopy
		{
			get
			{
				return this.Type == ReplayConfigType.RemoteCopyTarget;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.Database.Name;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_displayName))
				{
					this.m_displayName = string.Format("{0}\\{1}", this.Name, this.ServerName);
				}
				return this.m_displayName;
			}
		}

		public virtual string Identity
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_identity))
				{
					this.m_identity = ReplayConfiguration.GetIdentityFromGuid(this.IdentityGuid);
				}
				return this.m_identity;
			}
		}

		public static string GetIdentityFromGuid(Guid guid)
		{
			return guid.ToString().ToLowerInvariant();
		}

		public virtual ObjectId IdentityObject
		{
			get
			{
				return this.Database.Identity;
			}
		}

		public virtual string DatabaseDn
		{
			get
			{
				return this.Database.DistinguishedName;
			}
		}

		public virtual Guid IdentityGuid
		{
			get
			{
				return this.Database.Guid;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.IdentityGuid;
			}
		}

		public virtual int ActivationPreference
		{
			get
			{
				return this.m_activationPreference;
			}
		}

		public virtual string LogFilePrefix
		{
			get
			{
				return this.Database.LogFilePrefix;
			}
		}

		public virtual string LogExtension
		{
			get
			{
				return "log";
			}
		}

		public virtual string LogFileSuffix
		{
			get
			{
				return ".log";
			}
		}

		public virtual string LogInspectorPath
		{
			get
			{
				return Path.Combine(this.DestinationLogPath, this.m_LogInspectorDirectory);
			}
		}

		public virtual string E00LogBackupPath
		{
			get
			{
				return Path.Combine(this.DestinationLogPath, "IgnoredLogs");
			}
		}

		public virtual string DatabaseName
		{
			get
			{
				return this.Database.Name;
			}
		}

		public virtual bool IsPublicFolderDatabase
		{
			get
			{
				return this.Database.IsPublicFolderDatabase;
			}
		}

		public virtual bool DatabaseIsPrivate
		{
			get
			{
				return this.Database.IsMailboxDatabase;
			}
		}

		public virtual bool CircularLoggingEnabled
		{
			get
			{
				return this.Database.CircularLoggingEnabled;
			}
		}

		public virtual string ServerName
		{
			get
			{
				return this.m_server.Name;
			}
		}

		public virtual int ServerVersion
		{
			get
			{
				return this.m_server.VersionNumber;
			}
		}

		public virtual ReplayState ReplayState
		{
			get
			{
				return this.m_replayState;
			}
		}

		public virtual string SourceMachine
		{
			get
			{
				return this.m_sourceNodeFqdn;
			}
		}

		public virtual string TargetMachine
		{
			get
			{
				return this.m_targetNodeFqdn;
			}
		}

		public virtual EnhancedTimeSpan ReplayLagTime
		{
			get
			{
				return EnhancedTimeSpan.FromTicks(0L);
			}
		}

		public virtual EnhancedTimeSpan TruncationLagTime
		{
			get
			{
				return EnhancedTimeSpan.FromTicks(0L);
			}
		}

		public virtual bool DatabaseCreated
		{
			get
			{
				return this.Database.DatabaseCreated;
			}
		}

		public virtual string DestinationEdbPath
		{
			get
			{
				return this.Database.EdbFilePath.ToString();
			}
		}

		public virtual string DestinationSystemPath
		{
			get
			{
				if (this.Database.SystemFolderPath == null)
				{
					throw new ReplayConfigPropException(this.m_identity, "Database.SystemFolderPath");
				}
				return this.Database.SystemFolderPath.ToString();
			}
		}

		public virtual string DestinationLogPath
		{
			get
			{
				if (this.Database.LogFolderPath == null)
				{
					throw new ReplayConfigPropException(this.m_identity, "Database.LogFolderPath");
				}
				return this.Database.LogFolderPath.ToString();
			}
		}

		public virtual string SourceEdbPath
		{
			get
			{
				return this.DestinationEdbPath;
			}
		}

		public virtual string SourceSystemPath
		{
			get
			{
				return this.DestinationSystemPath;
			}
		}

		public virtual string SourceLogPath
		{
			get
			{
				return this.DestinationLogPath;
			}
		}

		public virtual string AutoDagVolumesRootFolderPath
		{
			get
			{
				return this.m_autoDagVolumesRootFolderPath;
			}
		}

		public virtual string AutoDagDatabasesRootFolderPath
		{
			get
			{
				return this.m_autoDagDatabasesRootFolderPath;
			}
		}

		public virtual int AutoDagDatabaseCopiesPerVolume
		{
			get
			{
				return this.m_autoDagDatabaseCopiesPerVolume;
			}
		}

		public override string ToString()
		{
			return this.m_debugString;
		}

		public string GetXmlDescription(JET_SIGNATURE logfileSignature)
		{
			string text = this.IdentityGuid.ToString();
			string name = this.Name;
			string destinationSystemPath = this.DestinationSystemPath;
			string logFilePrefix = this.LogFilePrefix;
			string.Format("{0}*.{1}", this.LogFilePrefix, this.LogExtension);
			string destinationLogPath = this.DestinationLogPath;
			string destinationEdbPath = this.DestinationEdbPath;
			string databaseName = this.DatabaseName;
			bool databaseIsPrivate = this.DatabaseIsPrivate;
			bool circularLoggingEnabled = this.CircularLoggingEnabled;
			StringWriter stringWriter = new StringWriter();
			XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
			xmlWriter.WriteStartDocument(true);
			xmlWriter.WriteStartElement("EXWRITER_METADATA");
			xmlWriter.WriteStartElement("VERSION_STAMP");
			xmlWriter.WriteString("15.00.1497.012");
			xmlWriter.WriteEndElement();
			string fileName = Path.GetFileName(destinationEdbPath);
			string directoryName = Path.GetDirectoryName(destinationEdbPath);
			xmlWriter.WriteStartElement("DATABASE_NAME");
			xmlWriter.WriteString(databaseName);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("DATABASE_GUID");
			xmlWriter.WriteString(text);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("DATABASE_GUID_ORIGINAL");
			xmlWriter.WriteString(text);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("EDB_LOCATION_ORIGINAL");
			xmlWriter.WriteString(directoryName);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("EDB_FILENAME_ORIGINAL");
			xmlWriter.WriteString(fileName);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("PRIVATE_MDB");
			xmlWriter.WriteString(databaseIsPrivate ? "yes" : "no");
			xmlWriter.WriteEndElement();
			string text2 = logfileSignature.ulRandom.ToString();
			xmlWriter.WriteStartElement("LOG_SIGNATURE_ID");
			xmlWriter.WriteString(text2);
			xmlWriter.WriteEndElement();
			string text3 = InteropShim.Uint64FromLogTime(new JET_LOGTIME?(logfileSignature.logtimeCreate)).ToString();
			xmlWriter.WriteStartElement("LOG_SIGNATURE_TIMESTAMP");
			xmlWriter.WriteString(text3);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("LOG_BASE_NAME");
			xmlWriter.WriteString(logFilePrefix);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("LOG_PATH_ORIGINAL");
			xmlWriter.WriteString(destinationLogPath);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("SYSTEM_PATH_ORIGINAL");
			xmlWriter.WriteString(destinationSystemPath);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("CIRCULAR_LOGGING");
			xmlWriter.WriteString(circularLoggingEnabled ? "yes" : "no");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("REPLICA");
			xmlWriter.WriteString("yes");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();
			string result = stringWriter.ToString();
			stringWriter.Dispose();
			return result;
		}

		protected virtual void BuildDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				stringBuilder.Append("ReplayConfig: Id='" + this.Identity + "', ");
			}
			catch (TransientException)
			{
				stringBuilder.Append("ReplayConfig: Id='ERROR', ");
			}
			try
			{
				stringBuilder.Append("Type='" + this.Type + "', ");
			}
			catch (TransientException)
			{
				stringBuilder.Append("Type='ERROR', ");
			}
			try
			{
				stringBuilder.Append("Prefix='" + this.LogFilePrefix + "', ");
			}
			catch (TransientException)
			{
				stringBuilder.Append("Prefix='ERROR', ");
			}
			try
			{
				stringBuilder.Append("SourceMachine='" + this.SourceMachine + "', ");
			}
			catch (TransientException)
			{
				stringBuilder.Append("SourceMachine='ERROR', ");
			}
			try
			{
				stringBuilder.Append("DestinationLogPath='" + this.DestinationLogPath + "', ");
			}
			catch (TransientException)
			{
				stringBuilder.Append("DestinationLogPath='ERROR', ");
			}
			try
			{
				stringBuilder.Append("DestinationSystemPath='" + this.DestinationSystemPath + "', ");
			}
			catch (TransientException)
			{
				stringBuilder.Append("DestinationSystemPath='ERROR', ");
			}
			try
			{
				stringBuilder.Append("DestinationEdbPath='" + this.DestinationEdbPath + "'");
			}
			catch (TransientException)
			{
				stringBuilder.Append("DestinationEdbPath='ERROR'");
			}
			this.m_debugString = stringBuilder.ToString();
		}

		public string BuildShortLogfileName(long generation)
		{
			return EseHelper.MakeLogfileName(this.LogFilePrefix, this.LogFileSuffix, generation);
		}

		public string BuildFullLogfileName(long generation)
		{
			return Path.Combine(this.DestinationLogPath, this.BuildShortLogfileName(generation));
		}

		public static void ConstructAllLocalConfigurations(IADConfig adConfig, ActiveManager activeManager, out List<ReplayConfiguration> currentSourceConfigurations, out List<ReplayConfiguration> currentRemoteTargetConfigurations, out List<KeyValuePair<IADDatabase, Exception>> failedConfigurations)
		{
			currentSourceConfigurations = new List<ReplayConfiguration>(20);
			currentRemoteTargetConfigurations = new List<ReplayConfiguration>(48);
			failedConfigurations = new List<KeyValuePair<IADDatabase, Exception>>();
			if (activeManager == null)
			{
				activeManager = ActiveManager.GetNoncachingActiveManagerInstance();
			}
			IADServer localServer = adConfig.GetLocalServer();
			if (localServer == null)
			{
				ReplayConfiguration.Tracer.TraceError(0L, "ConstructAllLocalConfigurations: didn't find local server");
				return;
			}
			IEnumerable<IADDatabase> databasesOnLocalServer = adConfig.GetDatabasesOnLocalServer();
			if (databasesOnLocalServer == null)
			{
				ReplayConfiguration.Tracer.TraceError(0L, "There are no mailbox dbs on this server");
				return;
			}
			IADDatabaseAvailabilityGroup localDag = adConfig.GetLocalDag();
			foreach (IADDatabase iaddatabase in databasesOnLocalServer)
			{
				Exception ex = null;
				bool flag;
				ReplayConfiguration replayConfiguration = ReplayConfiguration.GetReplayConfiguration(localDag, iaddatabase, localServer, activeManager, out flag, out ex);
				if (replayConfiguration != null)
				{
					if (flag)
					{
						currentSourceConfigurations.Add(replayConfiguration);
					}
					else
					{
						currentRemoteTargetConfigurations.Add(replayConfiguration);
					}
				}
				else if (ex != null)
				{
					ReplayConfiguration.Tracer.TraceError<Guid, string>(0L, "ReplayConfiguration for database '{0}' was not created due to an Exception. The configuration will not be added to the list of possible instances to run. Exception: {1}", iaddatabase.Guid, ex.ToString());
					failedConfigurations.Add(new KeyValuePair<IADDatabase, Exception>(iaddatabase, ex));
				}
				else
				{
					ReplayConfiguration.Tracer.TraceError<string>(0L, "ConstructAllLocalConfigurations() did not find a ReplayConfiguration for database '{0}' and no error occurred.", iaddatabase.Name);
				}
			}
		}

		public static ReplayConfiguration GetReplayConfiguration(IADDatabaseAvailabilityGroup dag, IADDatabase mdb, IADServer hostServer, ActiveManager activeManager, out bool fSource, out Exception ex)
		{
			ReplayConfiguration result = null;
			fSource = false;
			ex = null;
			try
			{
				DatabaseLocationInfo serverForDatabase = activeManager.GetServerForDatabase(mdb.Guid, GetServerForDatabaseFlags.BasicQuery);
				if (string.IsNullOrEmpty(serverForDatabase.ServerFqdn))
				{
					ReplayConfiguration.Tracer.TraceDebug<string>(0L, "GetReplayConfiguration({0}) Performing a BasicQuery was insufficient. Calling again with AD access.", mdb.Name);
					serverForDatabase = activeManager.GetServerForDatabase(mdb.Guid);
				}
				else
				{
					ReplayConfiguration.Tracer.TraceDebug<string>(0L, "GetReplayConfiguration({0}) Performing a BasicQuery was sufficient. AD calls were avoided!", mdb.Name);
				}
				AmServerName amServerName = new AmServerName(serverForDatabase.ServerFqdn);
				AmServerName amServerName2 = new AmServerName(hostServer.Fqdn);
				ReplayConfiguration.Tracer.TraceDebug<string, string, string>(0L, "GetReplayConfiguration({0}): GSFD returned active server of: {1}, hostServerFqdn is: {2}", mdb.Name, amServerName.Fqdn, amServerName2.Fqdn);
				if (AmServerName.IsEqual(amServerName, amServerName2))
				{
					fSource = true;
				}
				if (fSource)
				{
					if (mdb.ReplicationType == ReplicationType.None)
					{
						result = new SingleCopyReplayConfiguration(dag, mdb, hostServer, LockType.ReplayService);
					}
					else
					{
						result = RemoteReplayConfiguration.ServiceGetReplayConfig(dag, mdb, hostServer, serverForDatabase.ServerFqdn, ReplayConfigType.RemoteCopySource);
					}
				}
				else
				{
					if (mdb.ReplicationType != ReplicationType.Remote)
					{
						throw new ReplayConfigNotFoundException(mdb.Name, hostServer.Name);
					}
					result = RemoteReplayConfiguration.ServiceGetReplayConfig(dag, mdb, hostServer, serverForDatabase.ServerFqdn, ReplayConfigType.RemoteCopyTarget);
				}
			}
			catch (DatabaseNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ServerForDatabaseNotFoundException ex3)
			{
				ex = ex3;
			}
			catch (ObjectNotFoundException ex4)
			{
				ex = ex4;
			}
			catch (TransientException ex5)
			{
				ex = ex5;
			}
			return result;
		}

		public void UpdateLastLogGeneratedAndEndOfLogInfo(long highestLogGen)
		{
			ActiveManagerCore.SetLastLogGenerationNumber(this.DatabaseGuid, highestLogGen);
			MonitoredDatabase monitoredDatabase = MonitoredDatabase.FindMonitoredDatabase(this.ServerName, this.DatabaseGuid);
			if (monitoredDatabase != null)
			{
				monitoredDatabase.UpdateCurrentEndOfLog(highestLogGen, true);
			}
		}

		internal static void TryUpdateLastLogGenerationNumberOnMount(ReplayConfiguration config, LogStreamResetOnMount logReset, MountDirectPerformanceTracker mountPerf, int mountFlags, long highestLogGen)
		{
			bool flag = (mountFlags & 2) == 2;
			mountPerf.IsLossyMountEnabled = flag;
			mountPerf.HighestLogGenBefore = highestLogGen;
			mountPerf.HighestLogGenAfter = highestLogGen;
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, Guid, bool>((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): fLossyMountEnabled = '{2}'", config.Name, config.IdentityGuid, flag);
			string logDirectory = config.DestinationLogPath;
			if (!flag)
			{
				long lowestLogGen = 0L;
				mountPerf.RunTimedOperation(MountDatabaseDirectOperation.LowestGenerationInDirectory, delegate
				{
					lowestLogGen = ShipControl.LowestGenerationInDirectory(new DirectoryInfo(logDirectory), config.LogFilePrefix, "." + config.LogExtension, true);
				});
				if (lowestLogGen == 0L)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)config.GetHashCode(), "Looks like a log stream reset");
					logReset.ResetLogStream();
					return;
				}
			}
			if (config.Type == ReplayConfigType.SingleCopySource)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, Guid, ReplayConfigType>((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): Skipping updating last log generation since the config is of type '{2}'.", config.DisplayName, config.IdentityGuid, config.Type);
				return;
			}
			try
			{
				if (highestLogGen > 0L)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, Guid, long>((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): Known highest log generation is {2}.", config.Name, config.IdentityGuid, highestLogGen);
					mountPerf.RunTimedOperation(MountDatabaseDirectOperation.GenerationAvailableInDirectory, delegate
					{
						if (!ShipControl.GenerationAvailableInDirectory(new DirectoryInfo(logDirectory), config.LogFilePrefix, "." + config.LogExtension, highestLogGen))
						{
							ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, Guid>((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): Log file for known highest generation does not exist, will scan directory to determine the max generation.", config.Name, config.IdentityGuid);
							highestLogGen = 0L;
						}
					});
				}
				if (highestLogGen <= 0L)
				{
					mountPerf.RunTimedOperation(MountDatabaseDirectOperation.HighestGenerationInDirectory, delegate
					{
						highestLogGen = ShipControl.HighestGenerationInDirectory(new DirectoryInfo(logDirectory), config.LogFilePrefix, "." + config.LogExtension);
					});
					ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): Highest log in directory '{2}' is {3}.", new object[]
					{
						config.Name,
						config.IdentityGuid,
						logDirectory,
						highestLogGen
					});
				}
				mountPerf.HighestLogGenAfter = highestLogGen;
				if (highestLogGen <= 0L)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, Guid>((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): Skipping updating last log generation since highest log gen is <= 0.", config.Name, config.IdentityGuid);
				}
				else
				{
					mountPerf.RunTimedOperation(MountDatabaseDirectOperation.UpdateLastLogGeneratedInClusDB, delegate
					{
						config.UpdateLastLogGeneratedAndEndOfLogInfo(highestLogGen);
					});
				}
			}
			catch (IOException ex)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): Caught IO exception for path '{2}'. {3}", new object[]
				{
					config.Name,
					config.IdentityGuid,
					logDirectory,
					ex
				});
			}
			catch (SecurityException ex2)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): Caught SecurityException for path '{2}'. {3}", new object[]
				{
					config.Name,
					config.IdentityGuid,
					logDirectory,
					ex2
				});
			}
			catch (ClusterException arg)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceError<string, Guid, ClusterException>((long)config.GetHashCode(), "TryUpdateLastLogGenerationNumber {0} ({1}): Caught exception at SetLastLogGenerationNumber. {2}", config.Name, config.IdentityGuid, arg);
			}
		}

		protected void PopulatePropertiesFromDag(IADDatabaseAvailabilityGroup dag)
		{
			if (dag != null)
			{
				this.m_autoDagVolumesRootFolderPath = dag.AutoDagVolumesRootFolderPath.PathName;
				this.m_autoDagDatabasesRootFolderPath = dag.AutoDagDatabasesRootFolderPath.PathName;
				this.m_autoDagDatabaseCopiesPerVolume = dag.AutoDagDatabaseCopiesPerVolume;
				return;
			}
			this.m_autoDagVolumesRootFolderPath = string.Empty;
			this.m_autoDagDatabasesRootFolderPath = string.Empty;
			this.m_autoDagDatabaseCopiesPerVolume = 1;
		}

		public static ReplayConfiguration ConfigConverter<TInput>(TInput input) where TInput : ReplayConfiguration
		{
			return input;
		}

		protected const string DefaultReplayLagTimeStr = "1.00:00:00";

		protected const string DefaultTruncationLagTimeStr = "00:00:00";

		private const string DisplayNameFormat = "{0}\\{1}";

		protected string m_identity;

		protected IConfigurationSession m_adSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1912, "m_adSession", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\Core\\ReplayConfiguration.cs");

		protected readonly string m_LogInspectorDirectory = "inspector";

		protected string m_sourceNodeFqdn;

		protected string m_targetNodeFqdn;

		protected ReplayConfigType m_type;

		protected ReplayState m_replayState;

		protected IADDatabase m_database;

		protected IADServer m_server;

		protected string m_displayName;

		protected EnhancedTimeSpan m_replayLagTime;

		protected EnhancedTimeSpan m_truncationLagTime;

		protected int m_activationPreference;

		protected string m_autoDagVolumesRootFolderPath;

		protected string m_autoDagDatabasesRootFolderPath;

		protected int m_autoDagDatabaseCopiesPerVolume;

		protected string m_debugString;
	}
}
