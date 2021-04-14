using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.PhysicalAccessJet;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal sealed class EsebackCallbacks
	{
		public static JET_err PrepareInstanceForBackup(ESEBACK_CONTEXT context, JET_INSTANCE instance)
		{
			JET_err returnCode;
			using (Context contextForEsebackCallback = EsebackCallbacks.GetContextForEsebackCallback())
			{
				ExTraceGlobals.EsebackTracer.TraceFunction<JET_INSTANCE>(0L, "PrepareInstanceForBackup({0})", instance);
				EsebackCallbacks.StoreDatabaseCallback storeDatabaseCallback = new EsebackCallbacks.StoreDatabaseCallback(instance, delegate(StoreDatabase database)
				{
					if (database.IsBackupInProgress)
					{
						throw new StoreException((LID)41088U, ErrorCodeValue.BackupInProgress);
					}
					database.SetBackupInProgress();
				});
				Storage.ForEachDatabase(contextForEsebackCallback, new Storage.DatabaseEnumerationCallback(storeDatabaseCallback.Delegate));
				returnCode = (JET_err)storeDatabaseCallback.ReturnCode;
			}
			return returnCode;
		}

		public static JET_err DoneWithInstanceForBackup(ESEBACK_CONTEXT context, JET_INSTANCE instance, BackupDone complete)
		{
			JET_err returnCode;
			using (Context contextForEsebackCallback = EsebackCallbacks.GetContextForEsebackCallback())
			{
				ExTraceGlobals.EsebackTracer.TraceFunction<JET_INSTANCE>(0L, "DoneWithInstanceForBackup({0})", instance);
				EsebackCallbacks.StoreDatabaseCallback storeDatabaseCallback = new EsebackCallbacks.StoreDatabaseCallback(instance, delegate(StoreDatabase database)
				{
					database.ResetBackupInProgress();
				});
				Storage.ForEachDatabase(contextForEsebackCallback, new Storage.DatabaseEnumerationCallback(storeDatabaseCallback.Delegate));
				returnCode = (JET_err)storeDatabaseCallback.ReturnCode;
			}
			return returnCode;
		}

		public static JET_err GetDatabasesInfo(ESEBACK_CONTEXT context, out INSTANCE_BACKUP_INFO[] instances)
		{
			List<INSTANCE_BACKUP_INFO> instanceList = new List<INSTANCE_BACKUP_INFO>();
			Storage.DatabaseEnumerationCallback enumCallback = delegate(Context ignoredContext, StoreDatabase database, Func<bool> ignored)
			{
				JetDatabase jetDatabase = (JetDatabase)database.PhysicalDatabase;
				INSTANCE_BACKUP_INFO instance_BACKUP_INFO = new INSTANCE_BACKUP_INFO();
				INSTANCE_BACKUP_INFO instance_BACKUP_INFO2 = instance_BACKUP_INFO;
				IntPtr value = jetDatabase.JetInstance.Value;
				instance_BACKUP_INFO2.hInstanceId = value.ToInt64();
				instance_BACKUP_INFO.wszInstanceName = database.MdbName;
				instance_BACKUP_INFO.rgDatabase = new DATABASE_BACKUP_INFO[]
				{
					new DATABASE_BACKUP_INFO
					{
						wszDatabaseDisplayName = database.MdbName,
						wszDatabaseStreams = new string[]
						{
							Path.Combine(database.FilePath, database.FileName)
						},
						guidDatabase = database.MdbGuid,
						fDatabaseFlags = DatabaseBackupInfoFlags.Mounted
					}
				};
				INSTANCE_BACKUP_INFO item = instance_BACKUP_INFO;
				instanceList.Add(item);
			};
			JET_err result;
			using (Context contextForEsebackCallback = EsebackCallbacks.GetContextForEsebackCallback())
			{
				Storage.ForEachDatabase(contextForEsebackCallback, enumCallback);
				instances = instanceList.ToArray();
				ExTraceGlobals.EsebackTracer.TraceFunction<int>(0L, "GetDatabasesInfo returns {0} instances", instances.Length);
				result = JET_err.Success;
			}
			return result;
		}

		public static JET_err IsSGReplicated(ESEBACK_CONTEXT jetContext, JET_INSTANCE instance, out bool isReplicated, out Guid guid, out LOGSHIP_INFO[] info)
		{
			EsebackCallbacks.<>c__DisplayClassa CS$<>8__locals1 = new EsebackCallbacks.<>c__DisplayClassa();
			CS$<>8__locals1.databaseGuid = Guid.Empty;
			CS$<>8__locals1.infoList = null;
			JET_err returnCode;
			using (Context context = EsebackCallbacks.GetContextForEsebackCallback())
			{
				EsebackCallbacks.StoreDatabaseCallback storeDatabaseCallback = new EsebackCallbacks.StoreDatabaseCallback(instance, delegate(StoreDatabase database)
				{
					CS$<>8__locals1.databaseGuid = database.MdbGuid;
					CS$<>8__locals1.infoList = EsebackCallbacks.GetInfoListFromDatabase(context, database);
				});
				try
				{
					Storage.ForEachDatabase(context, new Storage.DatabaseEnumerationCallback(storeDatabaseCallback.Delegate));
					if (CS$<>8__locals1.infoList != null)
					{
						isReplicated = true;
						guid = CS$<>8__locals1.databaseGuid;
						info = CS$<>8__locals1.infoList;
						ExTraceGlobals.EsebackTracer.TraceDebug<JET_INSTANCE, Guid, int>(0L, "IsSGReplicated: Database {0} ({1}) is replicated ({2} copies)", instance, guid, info.Length);
					}
					else
					{
						isReplicated = false;
						guid = Guid.Empty;
						info = null;
						ExTraceGlobals.EsebackTracer.TraceDebug<JET_INSTANCE>(0L, "IsSGReplicated: Database {0} is not replicated", instance);
					}
				}
				catch (DirectoryTransientErrorException ex)
				{
					context.OnExceptionCatch(ex);
					ExTraceGlobals.EsebackTracer.TraceError<DirectoryTransientErrorException>(0L, "IsSGReplicated: Failed due to AD exception: {0}", ex);
					isReplicated = false;
					guid = Guid.Empty;
					info = null;
				}
				returnCode = (JET_err)storeDatabaseCallback.ReturnCode;
			}
			return returnCode;
		}

		public static int ServerAccessCheck()
		{
			SecurityDescriptor ntsecurityDescriptor;
			using (Context contextForEsebackCallback = EsebackCallbacks.GetContextForEsebackCallback())
			{
				ntsecurityDescriptor = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(contextForEsebackCallback).NTSecurityDescriptor;
			}
			int result;
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(current))
				{
					int num = SecurityHelper.CheckAdministrativeRights(clientSecurityContext, ntsecurityDescriptor) ? 0 : -2147024891;
					ExTraceGlobals.EsebackTracer.TraceFunction<ClientSecurityContext, int>(0L, "ServerAccessCheck({0}) returns {1}", clientSecurityContext, num);
					result = num;
				}
			}
			return result;
		}

		public static JET_err Trace(string data)
		{
			ExTraceGlobals.EsebackTracer.TraceDebug(0L, data);
			return JET_err.Success;
		}

		private static Context GetContextForEsebackCallback()
		{
			return Context.CreateForSystem();
		}

		private static LOGSHIP_INFO[] GetInfoListFromDatabase(Context context, StoreDatabase database)
		{
			Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.RefreshDatabaseInfo(context, database.MdbGuid);
			DatabaseInfo databaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(context, database.MdbGuid);
			if (databaseInfo.HostServerNames == null)
			{
				ExTraceGlobals.EsebackTracer.TraceDebug(0L, "GetInfoListFromDatabase: <null>");
			}
			else
			{
				ExTraceGlobals.EsebackTracer.TraceDebug<int>(0L, "GetInfoListFromDatabase: {0} servers", databaseInfo.HostServerNames.Length);
				foreach (string arg in databaseInfo.HostServerNames)
				{
					ExTraceGlobals.EsebackTracer.TraceDebug<string>(0L, "GetInfoListFromDatabase: {0}", arg);
				}
			}
			string localMachineName = Environment.MachineName;
			if (databaseInfo.HostServerNames != null && databaseInfo.HostServerNames.Length > 1)
			{
				return (from server in databaseInfo.HostServerNames
				where !string.Equals(server, localMachineName, StringComparison.OrdinalIgnoreCase)
				select new LOGSHIP_INFO
				{
					wszName = server,
					ulType = LogshipType.Standby
				}).ToArray<LOGSHIP_INFO>();
			}
			return null;
		}

		private sealed class StoreDatabaseCallback
		{
			public StoreDatabaseCallback(JET_INSTANCE instance, Action<StoreDatabase> action)
			{
				this.instance = instance;
				this.action = action;
			}

			public int ReturnCode
			{
				get
				{
					return (int)this.errorCode;
				}
			}

			public void Delegate(Context context, StoreDatabase database, Func<bool> shouldCallbackContinue)
			{
				if (this.IsCorrectDatabase(database))
				{
					try
					{
						this.action(database);
						this.errorCode = ErrorCode.NoError;
						return;
					}
					catch (StoreException ex)
					{
						context.OnExceptionCatch(ex);
						this.errorCode = ex.Error;
						return;
					}
				}
				this.errorCode = ErrorCodeValue.NotFound;
			}

			private bool IsCorrectDatabase(StoreDatabase database)
			{
				JetDatabase jetDatabase = (JetDatabase)database.PhysicalDatabase;
				return jetDatabase.JetInstance.Equals(this.instance);
			}

			private readonly JET_INSTANCE instance;

			private readonly Action<StoreDatabase> action;

			private ErrorCodeValue errorCode = ErrorCode.NoError;
		}
	}
}
