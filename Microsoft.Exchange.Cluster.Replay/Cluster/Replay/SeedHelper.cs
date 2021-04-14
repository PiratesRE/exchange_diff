using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Search.Fast;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SeedHelper
	{
		public static string TranslateEsebackErrorCode(long ec, long dwExtError)
		{
			uint num = (uint)ec;
			int extcode = (int)dwExtError;
			string result = null;
			uint num2 = num;
			if (num2 <= 3355379671U)
			{
				if (num2 <= 3355378867U)
				{
					if (num2 == 1293U)
					{
						return ReplayStrings.SeederEcBackupInProgress;
					}
					if (num2 != 3355378867U)
					{
						goto IL_10C;
					}
				}
				else
				{
					if (num2 == 3355379665U)
					{
						return ReplayStrings.FailedToOpenShipLogContextInvalidParameter;
					}
					if (num2 != 3355379671U)
					{
						goto IL_10C;
					}
					return ReplayStrings.FailedToOpenShipLogContextStoreStopped;
				}
			}
			else if (num2 <= 3355381669U)
			{
				if (num2 == 3355379675U)
				{
					return ReplayStrings.FailedToOpenShipLogContextEseCircularLoggingEnabled;
				}
				switch (num2)
				{
				case 3355381668U:
					return ReplayStrings.SeederEchrInvalidCallSequence;
				case 3355381669U:
					return ReplayStrings.SeederEchrRestoreAtFileLevel;
				default:
					goto IL_10C;
				}
			}
			else
			{
				if (num2 == 3355444321U)
				{
					return ReplayStrings.EseBackFileSystemCorruption;
				}
				if (num2 != 3355444403U)
				{
					if (num2 != 3355444505U)
					{
						goto IL_10C;
					}
					return ReplayStrings.JetErrorDatabaseNotFound;
				}
			}
			return ReplayStrings.FailedToOpenShipLogContextDatabaseNotMounted;
			IL_10C:
			if (!SeedHelper.EseMessageIdToString(num, extcode, out result))
			{
				result = null;
			}
			return result;
		}

		public static string StrEmbedEsebackExtendedErrorCode(string strTarget, int dwExtError)
		{
			string result = strTarget;
			if (strTarget == null)
			{
				return string.Empty;
			}
			string[] array = strTarget.Split(new char[]
			{
				'%'
			});
			if (array.Length == 2)
			{
				if (array[1] != null && array[1] != string.Empty && array[1][0] == 'd')
				{
					string arg = array[1].Substring(1);
					result = array[0] + dwExtError + arg;
				}
				else if (array[1] != null && array[1] != string.Empty && array[1][0] == 'X')
				{
					string arg2 = array[1].Substring(1);
					result = string.Format("{0}{1:X}{2}", array[0], dwExtError, arg2);
				}
			}
			return result;
		}

		internal static bool IsDbPendingLcrRcrTarget(Guid guid, out ReplayConfiguration replayConfig, out bool fSource)
		{
			replayConfig = null;
			fSource = false;
			IADDatabaseAvailabilityGroup dag;
			IADDatabase db;
			IADServer server;
			bool flag = SeedHelper.IsDbPendingLcrRcrTarget(guid, out dag, out db, out server);
			if (flag)
			{
				replayConfig = RemoteReplayConfiguration.TaskGetReplayConfig(dag, db, server);
				fSource = (replayConfig.Type == ReplayConfigType.RemoteCopySource);
			}
			return flag;
		}

		internal static bool IsDbPendingLcrRcrTarget(Guid guid, out IADDatabaseAvailabilityGroup dag, out IADDatabase db, out IADServer server)
		{
			IADConfig adconfig = Dependencies.ADConfig;
			dag = adconfig.GetLocalDag();
			db = adconfig.GetDatabase(guid);
			server = adconfig.GetLocalServer();
			if (db != null && server != null && SeedHelper.IsDatabasePendingRcrTarget(db, server))
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>(0L, "SeedHelper.IsDbPendingLcrRcrTarget(): The Replica Instance for Database '{0}' ({1})has not yet been started, but the DB is configured as an RCR target on the local machine.", db.Name, guid);
				return true;
			}
			ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>(0L, "SeedHelper.IsDbPendingLcrRcrTarget(): Database '{0}' ({1}) is not a valid LCR/RCR replica target.", (db != null) ? db.Name : "<null>", guid);
			return false;
		}

		internal static void GetDatabaseNameAndPath(Guid databaseGuid, out string databaseName, out string databaseFilePath)
		{
			IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreateFullyConsistentRootOrgSession(true);
			IADDatabase iaddatabase = iadtoplogyConfigurationSession.FindDatabaseByGuid(databaseGuid);
			databaseName = iaddatabase.Name;
			if (iaddatabase.EdbFilePath != null)
			{
				databaseFilePath = iaddatabase.EdbFilePath.ToString();
				return;
			}
			databaseFilePath = null;
		}

		internal static string TranslateSeederErrorCode(long seederEc, string sourceServer)
		{
			switch ((int)seederEc)
			{
			case 0:
				return ReplayStrings.SeederEcSuccess;
			case 1:
				return ReplayStrings.SeederEcError;
			case 2:
				return ReplayStrings.SeederEcInvalidInput;
			case 3:
				return ReplayStrings.SeederEcOOMem;
			case 4:
				return ReplayStrings.SeederEcNotEnoughDisk;
			case 5:
				return ReplayStrings.SeederEcFailAcqRight;
			case 9:
				return ReplayStrings.SeederEcDBNotFound;
			case 10:
				return ReplayStrings.SeederEcStoreNotOnline(sourceServer);
			case 11:
				return ReplayStrings.SeederEcNoOnlineEdb;
			case 12:
				return ReplayStrings.SeederEcSeedingCancelled;
			case 13:
				return ReplayStrings.SeederEcOverlappedWriteErr;
			case 15:
				return "Seeding aborted by test hook.";
			case 17:
				return ReplayStrings.SeederEcDeviceNotReady;
			case 18:
				return ReplayStrings.SeederEcCommunicationsError;
			}
			string text = SeedHelper.TranslateEsebackErrorCode(seederEc, 0L);
			if (text == null)
			{
				text = ReplayStrings.SeederEcUndefined((int)seederEc);
			}
			return text;
		}

		internal static void SuspendDatabaseCopy(Guid dbGuid, string server, string comment)
		{
			try
			{
				DatabaseCopyActionFlags flags = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation;
				Dependencies.ReplayRpcClientWrapper.RequestSuspend3(server, dbGuid, comment, (uint)flags, 1U);
			}
			catch (ReplayServiceSuspendCommentException arg)
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<ReplayServiceSuspendCommentException>(0L, "SeedHelper.SuspendDatabaseCopy(): Catching and ignoring exception: {0}.", arg);
			}
			catch (ReplayServiceSuspendRpcPartialSuccessCatalogFailedException arg2)
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<ReplayServiceSuspendRpcPartialSuccessCatalogFailedException>(0L, "SeedHelper.SuspendDatabaseCopy(): Catching and ignoring exception: {0}.", arg2);
			}
			catch (TaskServerException ex)
			{
				throw new SeederSuspendFailedException(ex.Message, ex);
			}
		}

		internal static void TryResumeDatabaseCopy(Guid dbGuid, string server, bool skipSettingResumeAutoReseedState = false)
		{
			try
			{
				DatabaseCopyActionFlags databaseCopyActionFlags = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation;
				if (skipSettingResumeAutoReseedState)
				{
					databaseCopyActionFlags |= DatabaseCopyActionFlags.SkipSettingResumeAutoReseedState;
				}
				Dependencies.ReplayRpcClientWrapper.RequestResume2(server, dbGuid, (uint)databaseCopyActionFlags);
			}
			catch (TaskServerException arg)
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<TaskServerException>(0L, "SeedHelper.ResumeDatabaseCopy(): Catching and ignoring exception: {0}.", arg);
			}
			catch (TaskServerTransientException arg2)
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<TaskServerTransientException>(0L, "SeedHelper.ResumeDatabaseCopy(): Catching and ignoring exception: {0}.", arg2);
			}
		}

		internal static bool IsPerformingFastOperationException(SeederServerException exception, out string fastErrorMessage)
		{
			fastErrorMessage = null;
			Exception ex;
			if (exception != null && exception.TryGetExceptionOrInnerOfType(out ex))
			{
				fastErrorMessage = ((ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
				return true;
			}
			return false;
		}

		private static bool IsMachineNameLocal(string machineName, bool usableRemotely)
		{
			if (string.IsNullOrEmpty(machineName))
			{
				return false;
			}
			string machineName2 = Environment.MachineName;
			if (Cluster.StringIEquals(machineName, machineName2))
			{
				return true;
			}
			string fqdnNameFromNode = SharedHelper.GetFqdnNameFromNode(machineName2);
			if (Cluster.StringIEquals(machineName, fqdnNameFromNode))
			{
				return true;
			}
			if (Cluster.StringIEquals(machineName, "localhost"))
			{
				return !usableRemotely;
			}
			IPHostEntry hostEntry;
			try
			{
				hostEntry = Dns.GetHostEntry(machineName);
			}
			catch (SocketException arg)
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<string, bool, SocketException>(0L, "SeedHelper: IsMachineNameLocal( {0}, {1} ) threw a SocketException: {2}.", machineName, usableRemotely, arg);
				return false;
			}
			IPHostEntry hostEntry2 = Dns.GetHostEntry("localhost");
			foreach (IPAddress ipaddress in hostEntry2.AddressList)
			{
				if (Cluster.StringIEquals(ipaddress.ToString(), machineName))
				{
					return !usableRemotely;
				}
				foreach (IPAddress obj in hostEntry.AddressList)
				{
					if (ipaddress.Equals(obj))
					{
						return !usableRemotely;
					}
				}
			}
			hostEntry2 = Dns.GetHostEntry("..localmachine");
			foreach (IPAddress ipaddress2 in hostEntry2.AddressList)
			{
				if (Cluster.StringIEquals(ipaddress2.ToString(), machineName))
				{
					return true;
				}
				foreach (IPAddress obj2 in hostEntry.AddressList)
				{
					if (ipaddress2.Equals(obj2))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool IsDatabasePendingRcrTarget(IADDatabase db, IADServer server)
		{
			if (db == null)
			{
				throw new ArgumentNullException("db");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (!db.Recovery && server.IsMailboxServer && db.ReplicationType == ReplicationType.Remote)
			{
				foreach (IADDatabaseCopy iaddatabaseCopy in db.DatabaseCopies)
				{
					if (iaddatabaseCopy != null && iaddatabaseCopy.HostServer.ObjectGuid == server.Id.ObjectGuid)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool EseMessageIdToString(uint ec, int extcode, out string msg)
		{
			bool result = DiagCore.FormatMessageFromModule("eseback2.dll", ec, out msg, null);
			if (3355381765U == ec || 3355381764U == ec)
			{
				msg = SeedHelper.StrEmbedEsebackExtendedErrorCode(msg, extcode);
			}
			return result;
		}

		internal const uint HrDatabaseNotFound = 3355378867U;

		internal const uint HrDatabaseNotFoundStrange = 3355444403U;

		internal const uint HrInvalidParam = 3355379665U;

		internal const uint HrCouldNotConnect = 3355379671U;

		internal const uint HrCircularLogging = 3355379675U;

		internal const uint HrInvalidCallSequence = 3355381668U;

		internal const uint HrRestoreAtFileLevel = 3355381669U;

		internal const uint EcBackupInProgress = 1293U;

		internal const uint HrBackupInProgress = 3355443705U;

		internal const uint HrDiskIO = 3355444222U;

		private const string Eseback2Dll = "eseback2.dll";

		private const uint HrErrorFromESECall = 3355381764U;

		private const uint HrErrorFromCallbackCall = 3355381765U;

		private const uint HrObjectNotFound = 3355444505U;

		public const uint HrFileSystemCorruption = 3355444321U;
	}
}
