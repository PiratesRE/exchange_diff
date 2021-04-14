using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class AdminRpcMountDatabase : AdminRpc
	{
		public AdminRpcMountDatabase(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint flags, byte[] auxiliaryIn) : base(AdminMethod.EcMountDatabase50, callerSecurityContext, auxiliaryIn)
		{
			base.MdbGuid = new Guid?(mdbGuid);
			this.flags = (MountFlags)flags;
		}

		protected override ErrorCode EcCheckPermissions(MapiContext context)
		{
			return AdminRpcPermissionChecks.EcCheckConstrainedDelegationRights(context, base.DatabaseInfo);
		}

		protected override ErrorCode EcExecuteRpc(MapiContext context)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			try
			{
				if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, "AdminRpcMountDatabase: Retrieve AD information.");
				}
				ServerInfo serverInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(context);
				Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.RefreshDatabaseInfo(context, base.MdbGuid.Value);
				DatabaseInfo databaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(context, base.MdbGuid.Value);
				if (!databaseInfo.IsPublicFolderDatabase)
				{
					bool flag = databaseInfo.HostServerNames != null && databaseInfo.HostServerNames.Length > 1;
					bool circularLoggingEnabled = databaseInfo.CircularLoggingEnabled && !flag;
					StoreDatabase database = new StoreDatabase(databaseInfo.MdbName, databaseInfo.MdbGuid, databaseInfo.DagOrServerGuid, databaseInfo.ServerName, databaseInfo.LegacyDN, databaseInfo.Description, databaseInfo.LogPath, databaseInfo.FilePath, databaseInfo.FileName, serverInfo.IsDAGMember, circularLoggingEnabled, databaseInfo.Options, serverInfo.IsMultiRole, databaseInfo.EventHistoryRetentionPeriod, databaseInfo.IsRecoveryDatabase, databaseInfo.AllowFileRestore, serverInfo.Edition, databaseInfo.ForestName);
					if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, "AdminRpcMountDatabase: Mount the database.");
					}
					errorCode = Storage.MountDatabase(context, database, this.flags);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)62392U);
					}
				}
				else
				{
					errorCode = ErrorCode.CreateNotFound((LID)17336U);
				}
			}
			catch (CouldNotCreateDatabaseException exception)
			{
				context.OnExceptionCatch(exception);
				errorCode = ErrorCode.CreateDiskError((LID)35480U);
			}
			return errorCode;
		}

		private MountFlags flags;
	}
}
