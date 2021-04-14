using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.MailboxSignature;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Rpc.AdminRpc;
using Microsoft.Exchange.Rpc.MultiMailboxSearch;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.HA;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.MapiDisp;
using Microsoft.Exchange.Server.Storage.MultiMailboxSearch;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;
using Microsoft.Exchange.Server.Storage.StoreIntegrityCheck;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	public class AdminRpcServer : IAdminRpcServer
	{
		public void AdminGetIFVersion(ClientSecurityContext callerSecurityContext, out ushort majorVersion, out ushort minorVersion)
		{
			majorVersion = 7;
			minorVersion = 17;
		}

		public int EcAdminGetCnctTable50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, int lparam, out byte[] result, uint[] propTags, uint cpid, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			result = null;
			rowCount = 0U;
			auxiliaryOut = Array<byte>.Empty;
			return -2147221246;
		}

		public int EcAdminGetLogonTable50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, int lparam, out byte[] result, uint[] propTags, uint cpid, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			result = null;
			rowCount = 0U;
			auxiliaryOut = Array<byte>.Empty;
			return -2147221246;
		}

		public int EcAdminGetFeatureVersion50(ClientSecurityContext callerSecurityContext, uint feature, out uint version, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			switch (feature)
			{
			case 1U:
				version = 104U;
				break;
			case 2U:
				version = 1U;
				break;
			case 3U:
				version = 1U;
				break;
			case 4U:
				version = 4U;
				break;
			default:
				version = 0U;
				errorCode = ErrorCode.CreateInvalidParameter((LID)37752U);
				break;
			}
			auxiliaryOut = Array<byte>.Empty;
			return (int)errorCode;
		}

		public int EcListAllMdbStatus50(ClientSecurityContext callerSecurityContext, bool basicInformation, out uint countMdbs, out byte[] mdbStatus, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			countMdbs = 0U;
			mdbStatus = null;
			AdminRpcServer.AdminRpcListAllMdbStatus adminRpcListAllMdbStatus = new AdminRpcServer.AdminRpcListAllMdbStatus(callerSecurityContext, basicInformation, auxiliaryIn);
			ErrorCode errorCode = adminRpcListAllMdbStatus.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				countMdbs = adminRpcListAllMdbStatus.MdbCount;
				mdbStatus = adminRpcListAllMdbStatus.MdbStatusRaw;
			}
			auxiliaryOut = adminRpcListAllMdbStatus.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcListMdbStatus50(ClientSecurityContext callerSecurityContext, Guid[] mdbGuids, out uint[] mdbStatusFlags, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			mdbStatusFlags = null;
			AdminRpcListMdbStatus adminRpcListMdbStatus = new AdminRpcListMdbStatus(callerSecurityContext, mdbGuids, auxiliaryIn);
			ErrorCode errorCode = adminRpcListMdbStatus.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				mdbStatusFlags = adminRpcListMdbStatus.MdbStatus;
			}
			auxiliaryOut = adminRpcListMdbStatus.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcGetDatabaseSizeEx50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, out uint totalPages, out uint availablePages, out uint pageSize, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcGetDatabaseSize adminRpcGetDatabaseSize = new AdminRpcServer.AdminRpcGetDatabaseSize(callerSecurityContext, mdbGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetDatabaseSize.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				totalPages = adminRpcGetDatabaseSize.TotalPages;
				availablePages = adminRpcGetDatabaseSize.AvailablePages;
				pageSize = adminRpcGetDatabaseSize.PageSize;
			}
			else
			{
				totalPages = 0U;
				availablePages = 0U;
				pageSize = 0U;
			}
			auxiliaryOut = adminRpcGetDatabaseSize.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcMountDatabase50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint flags, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcMountDatabase adminRpcMountDatabase = new AdminRpcMountDatabase(callerSecurityContext, mdbGuid, flags, auxiliaryIn);
			ErrorCode errorCode = adminRpcMountDatabase.EcExecute();
			auxiliaryOut = adminRpcMountDatabase.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcUnmountDatabase50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint flags, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcUnmountDatabase adminRpcUnmountDatabase = new AdminRpcUnmountDatabase(callerSecurityContext, mdbGuid, flags, auxiliaryIn);
			ErrorCode errorCode = adminRpcUnmountDatabase.EcExecute();
			auxiliaryOut = adminRpcUnmountDatabase.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcStartBlockModeReplicationToPassive50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, string passiveName, uint firstGenToSend, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcStartBlockModeReplicationToPassive adminRpcStartBlockModeReplicationToPassive = new AdminRpcServer.AdminRpcStartBlockModeReplicationToPassive(callerSecurityContext, mdbGuid, passiveName, firstGenToSend, auxiliaryIn);
			ErrorCode errorCode = adminRpcStartBlockModeReplicationToPassive.EcExecute();
			auxiliaryOut = adminRpcStartBlockModeReplicationToPassive.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcPurgeCachedMdbObject50(ClientSecurityContext callerSecurityContext, Guid databaseGuid, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcPurgeCachedMdbObject adminRpcPurgeCachedMdbObject = new AdminRpcServer.AdminRpcPurgeCachedMdbObject(callerSecurityContext, databaseGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcPurgeCachedMdbObject.EcExecute();
			auxiliaryOut = adminRpcPurgeCachedMdbObject.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcDoMaintenanceTask50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint task, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminDoMaintenanceTask adminDoMaintenanceTask = new AdminRpcServer.AdminDoMaintenanceTask(callerSecurityContext, mdbGuid, task, auxiliaryIn);
			ErrorCode errorCode = adminDoMaintenanceTask.EcExecute();
			auxiliaryOut = adminDoMaintenanceTask.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcGetLastBackupInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, out long lastFullBackupTime, out long lastIncrementalBackupTime, out long lastDifferentialBackupTime, out long lastCopyBackupTime, out int snapFull, out int snapIncremental, out int snapDifferential, out int snapCopy, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			lastFullBackupTime = 0L;
			lastIncrementalBackupTime = 0L;
			lastDifferentialBackupTime = 0L;
			lastCopyBackupTime = 0L;
			snapFull = 0;
			snapIncremental = 0;
			snapDifferential = 0;
			snapCopy = 0;
			AdminRpcServer.AdminRpcGetLastBackupInfo50 adminRpcGetLastBackupInfo = new AdminRpcServer.AdminRpcGetLastBackupInfo50(callerSecurityContext, mdbGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetLastBackupInfo.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				if (adminRpcGetLastBackupInfo.LastFullBackupTime > ParseSerialize.MinFileTimeDateTime)
				{
					lastFullBackupTime = adminRpcGetLastBackupInfo.LastFullBackupTime.ToFileTimeUtc();
				}
				if (adminRpcGetLastBackupInfo.LastIncrementalBackupTime > ParseSerialize.MinFileTimeDateTime)
				{
					lastIncrementalBackupTime = adminRpcGetLastBackupInfo.LastIncrementalBackupTime.ToFileTimeUtc();
				}
				if (adminRpcGetLastBackupInfo.LastDifferentialBackupTime > ParseSerialize.MinFileTimeDateTime)
				{
					lastDifferentialBackupTime = adminRpcGetLastBackupInfo.LastDifferentialBackupTime.ToFileTimeUtc();
				}
				if (adminRpcGetLastBackupInfo.LastCopyBackupTime > ParseSerialize.MinFileTimeDateTime)
				{
					lastCopyBackupTime = adminRpcGetLastBackupInfo.LastCopyBackupTime.ToFileTimeUtc();
				}
				snapFull = adminRpcGetLastBackupInfo.SnapshotFullBackup;
				snapIncremental = adminRpcGetLastBackupInfo.SnapshotIncrementalBackup;
				snapDifferential = adminRpcGetLastBackupInfo.SnapshotDifferentialBackup;
				snapCopy = adminRpcGetLastBackupInfo.SnapshotCopyBackup;
			}
			auxiliaryOut = adminRpcGetLastBackupInfo.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcGetLastBackupTimes50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, out long lastFullBackupTime, out long lastIncrementalBackupTime, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			lastFullBackupTime = 0L;
			lastIncrementalBackupTime = 0L;
			AdminRpcServer.AdminRpcGetLastBackupInfo50 adminRpcGetLastBackupInfo = new AdminRpcServer.AdminRpcGetLastBackupInfo50(callerSecurityContext, mdbGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetLastBackupInfo.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				if (adminRpcGetLastBackupInfo.LastFullBackupTime > ParseSerialize.MinFileTimeDateTime)
				{
					lastFullBackupTime = adminRpcGetLastBackupInfo.LastFullBackupTime.ToFileTimeUtc();
				}
				if (adminRpcGetLastBackupInfo.LastIncrementalBackupTime > ParseSerialize.MinFileTimeDateTime)
				{
					lastIncrementalBackupTime = adminRpcGetLastBackupInfo.LastIncrementalBackupTime.ToFileTimeUtc();
				}
			}
			auxiliaryOut = adminRpcGetLastBackupInfo.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcLogReplayRequest2(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint logReplayMax, uint logReplayFlags, out uint logReplayNext, out byte[] databaseInfo, out uint patchPageNumber, out byte[] patchToken, out byte[] patchData, out uint[] corruptPages, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcLogReplayRequest2 adminRpcLogReplayRequest = new AdminRpcServer.AdminRpcLogReplayRequest2(callerSecurityContext, mdbGuid, logReplayMax, logReplayFlags, auxiliaryIn);
			ErrorCode errorCode = adminRpcLogReplayRequest.EcExecute();
			logReplayNext = adminRpcLogReplayRequest.LogReplayNext;
			databaseInfo = adminRpcLogReplayRequest.DatabaseHeaderInfo;
			patchPageNumber = adminRpcLogReplayRequest.PatchPageNumber;
			patchToken = adminRpcLogReplayRequest.PatchToken;
			patchData = adminRpcLogReplayRequest.PatchData;
			corruptPages = adminRpcLogReplayRequest.CorruptPages;
			auxiliaryOut = adminRpcLogReplayRequest.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetResourceMonitorDigest50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint[] propertyTags, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcGetResourceMonitorDigest adminRpcGetResourceMonitorDigest = new AdminRpcServer.AdminRpcGetResourceMonitorDigest(callerSecurityContext, mdbGuid, propertyTags, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetResourceMonitorDigest.EcExecute();
			result = adminRpcGetResourceMonitorDigest.Result;
			rowCount = adminRpcGetResourceMonitorDigest.RowCount;
			auxiliaryOut = adminRpcGetResourceMonitorDigest.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetDatabaseProcessInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint[] propTags, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcGetDatabaseProcessInfo adminRpcGetDatabaseProcessInfo = new AdminRpcServer.AdminRpcGetDatabaseProcessInfo(callerSecurityContext, mdbGuid, propTags, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetDatabaseProcessInfo.EcExecute();
			result = adminRpcGetDatabaseProcessInfo.Result;
			rowCount = adminRpcGetDatabaseProcessInfo.RowCount;
			auxiliaryOut = adminRpcGetDatabaseProcessInfo.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminProcessSnapshotOperation50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint opCode, uint flags, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.EcAdminProcessSnapshotOperation ecAdminProcessSnapshotOperation = new AdminRpcServer.EcAdminProcessSnapshotOperation(callerSecurityContext, mdbGuid, opCode, flags, auxiliaryIn);
			ErrorCode errorCode = ecAdminProcessSnapshotOperation.EcExecute();
			auxiliaryOut = ecAdminProcessSnapshotOperation.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetPhysicalDatabaseInformation50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, out byte[] databaseInfo, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.EcAdminGetPhysicalDatabaseInformation ecAdminGetPhysicalDatabaseInformation = new AdminRpcServer.EcAdminGetPhysicalDatabaseInformation(callerSecurityContext, mdbGuid, auxiliaryIn);
			ErrorCode errorCode = ecAdminGetPhysicalDatabaseInformation.EcExecute();
			databaseInfo = ecAdminGetPhysicalDatabaseInformation.DatabaseHeaderInfo;
			auxiliaryOut = ecAdminGetPhysicalDatabaseInformation.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcForceNewLog50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.EcAdminForceNewLog ecAdminForceNewLog = new AdminRpcServer.EcAdminForceNewLog(callerSecurityContext, mdbGuid, auxiliaryIn);
			ErrorCode errorCode = ecAdminForceNewLog.EcExecute();
			auxiliaryOut = ecAdminForceNewLog.AuxiliaryOut;
			errorCode != ErrorCode.NoError;
			return (int)errorCode;
		}

		public int EcReadMdbEvents50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, ref Guid mdbVersionGuid, byte[] request, out byte[] response, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			response = null;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcReadMdbEvents adminRpcReadMdbEvents = new AdminRpcServer.AdminRpcReadMdbEvents(callerSecurityContext, mdbGuid, mdbVersionGuid, request, auxiliaryIn);
			ErrorCode errorCode = adminRpcReadMdbEvents.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				response = adminRpcReadMdbEvents.Response;
				mdbVersionGuid = adminRpcReadMdbEvents.MdbVersionGuid;
			}
			auxiliaryOut = adminRpcReadMdbEvents.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcWriteMdbEvents50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, ref Guid mdbVersionGuid, byte[] request, out byte[] response, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			response = null;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcWriteMdbEvents adminRpcWriteMdbEvents = new AdminRpcServer.AdminRpcWriteMdbEvents(callerSecurityContext, mdbGuid, mdbVersionGuid, request, auxiliaryIn);
			ErrorCode errorCode = adminRpcWriteMdbEvents.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				response = adminRpcWriteMdbEvents.Response;
				mdbVersionGuid = adminRpcWriteMdbEvents.MdbVersionGuid;
			}
			auxiliaryOut = adminRpcWriteMdbEvents.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcDeleteMdbWatermarksForConsumer50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, ref Guid mdbVersionGuid, Guid? mailboxDsGuid, Guid consumerGuid, out uint delCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			delCount = 0U;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcDeleteMdbWatermarksForConsumer adminRpcDeleteMdbWatermarksForConsumer = new AdminRpcServer.AdminRpcDeleteMdbWatermarksForConsumer(callerSecurityContext, mdbGuid, mdbVersionGuid, mailboxDsGuid, consumerGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcDeleteMdbWatermarksForConsumer.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				delCount = adminRpcDeleteMdbWatermarksForConsumer.DeletedCount;
				mdbVersionGuid = adminRpcDeleteMdbWatermarksForConsumer.MdbVersionGuid;
			}
			auxiliaryOut = adminRpcDeleteMdbWatermarksForConsumer.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcDeleteMdbWatermarksForMailbox50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, ref Guid mdbVersionGuid, Guid mailboxDsGuid, out uint delCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			delCount = 0U;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcDeleteMdbWatermarksForMailbox adminRpcDeleteMdbWatermarksForMailbox = new AdminRpcServer.AdminRpcDeleteMdbWatermarksForMailbox(callerSecurityContext, mdbGuid, mdbVersionGuid, mailboxDsGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcDeleteMdbWatermarksForMailbox.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				delCount = adminRpcDeleteMdbWatermarksForMailbox.DeletedCount;
				mdbVersionGuid = adminRpcDeleteMdbWatermarksForMailbox.MdbVersionGuid;
			}
			auxiliaryOut = adminRpcDeleteMdbWatermarksForMailbox.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcSaveMdbWatermarks50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, ref Guid mdbVersionGuid, MDBEVENTWM[] wms, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcSaveMdbWatermarks adminRpcSaveMdbWatermarks = new AdminRpcServer.AdminRpcSaveMdbWatermarks(callerSecurityContext, mdbGuid, mdbVersionGuid, wms, auxiliaryIn);
			ErrorCode errorCode = adminRpcSaveMdbWatermarks.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				mdbVersionGuid = adminRpcSaveMdbWatermarks.MdbVersionGuid;
			}
			auxiliaryOut = adminRpcSaveMdbWatermarks.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcGetMdbWatermarksForConsumer50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, ref Guid mdbVersionGuid, Guid? mailboxDsGuid, Guid consumerGuid, out MDBEVENTWM[] wms, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			wms = null;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcGetMdbWatermarksForConsumer adminRpcGetMdbWatermarksForConsumer = new AdminRpcServer.AdminRpcGetMdbWatermarksForConsumer(callerSecurityContext, mdbGuid, mdbVersionGuid, mailboxDsGuid, consumerGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetMdbWatermarksForConsumer.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				wms = adminRpcGetMdbWatermarksForConsumer.Wms;
				mdbVersionGuid = adminRpcGetMdbWatermarksForConsumer.MdbVersionGuid;
			}
			auxiliaryOut = adminRpcGetMdbWatermarksForConsumer.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcGetMdbWatermarksForMailbox50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, ref Guid mdbVersionGuid, Guid mailboxDsGuid, out MDBEVENTWM[] wms, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			wms = null;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcGetMdbWatermarksForMailbox adminRpcGetMdbWatermarksForMailbox = new AdminRpcServer.AdminRpcGetMdbWatermarksForMailbox(callerSecurityContext, mdbGuid, mdbVersionGuid, mailboxDsGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetMdbWatermarksForMailbox.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				wms = adminRpcGetMdbWatermarksForMailbox.Wms;
				mdbVersionGuid = adminRpcGetMdbWatermarksForMailbox.MdbVersionGuid;
			}
			auxiliaryOut = adminRpcGetMdbWatermarksForMailbox.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcClearAbsentInDsFlagOnMailbox50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcClearAbsentInDsOnMailbox adminRpcClearAbsentInDsOnMailbox = new AdminRpcServer.AdminRpcClearAbsentInDsOnMailbox(callerSecurityContext, mdbGuid, mailboxGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcClearAbsentInDsOnMailbox.EcExecute();
			auxiliaryOut = adminRpcClearAbsentInDsOnMailbox.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcPurgeCachedMailboxObject50(ClientSecurityContext callerSecurityContext, Guid mailboxGuid, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcPurgeCachedMailboxObject adminRpcPurgeCachedMailboxObject = new AdminRpcServer.AdminRpcPurgeCachedMailboxObject(callerSecurityContext, mailboxGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcPurgeCachedMailboxObject.EcExecute();
			auxiliaryOut = adminRpcPurgeCachedMailboxObject.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcSyncMailboxesWithDS50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			auxiliaryOut = Array<byte>.Empty;
			return (int)ErrorCode.CreateNotSupported((LID)45031U);
		}

		public int EcAdminDeletePrivateMailbox50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint flags, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcDeletePrivateMailbox adminRpcDeletePrivateMailbox = new AdminRpcServer.AdminRpcDeletePrivateMailbox(callerSecurityContext, mdbGuid, mailboxGuid, (DeleteMailboxFlags)flags, auxiliaryIn);
			ErrorCode errorCode = adminRpcDeletePrivateMailbox.EcExecute();
			auxiliaryOut = adminRpcDeletePrivateMailbox.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcSetMailboxSecurityDescriptor50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] ntsd, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			auxiliaryOut = Array<byte>.Empty;
			return (int)ErrorCode.CreateNotSupported((LID)45376U);
		}

		public int EcGetMailboxSecurityDescriptor50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, out byte[] ntsd, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcGetMailboxSecurityDescriptor adminRpcGetMailboxSecurityDescriptor = new AdminRpcServer.AdminRpcGetMailboxSecurityDescriptor(callerSecurityContext, mdbGuid, mailboxGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetMailboxSecurityDescriptor.EcExecute();
			ntsd = adminRpcGetMailboxSecurityDescriptor.NTSecurityDescriptor;
			auxiliaryOut = adminRpcGetMailboxSecurityDescriptor.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetMailboxSignature50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint flags, out byte[] mailboxSignature, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminGetMailboxSignature adminGetMailboxSignature = new AdminRpcServer.AdminGetMailboxSignature(callerSecurityContext, mdbGuid, mailboxGuid, (MailboxSignatureFlags)flags, auxiliaryIn);
			ErrorCode errorCode = adminGetMailboxSignature.EcExecute();
			mailboxSignature = adminGetMailboxSignature.Result;
			auxiliaryOut = adminGetMailboxSignature.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminSetMailboxBasicInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] mailboxBasicInfo, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcSetMailboxBasicInfo adminRpcSetMailboxBasicInfo = new AdminRpcServer.AdminRpcSetMailboxBasicInfo(callerSecurityContext, mdbGuid, mailboxGuid, mailboxBasicInfo, auxiliaryIn);
			ErrorCode errorCode = adminRpcSetMailboxBasicInfo.EcExecute();
			auxiliaryOut = adminRpcSetMailboxBasicInfo.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetMailboxTable50(ClientSecurityContext callerSecurityContext, Guid? mdbGuid, int lparam, out byte[] result, uint[] propTags, uint cpid, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			result = null;
			rowCount = 0U;
			AdminRpcServer.AdminRpcGetMailboxTable adminRpcGetMailboxTable = (mdbGuid != null) ? new AdminRpcServer.AdminRpcGetMailboxTable(callerSecurityContext, mdbGuid, lparam, propTags, cpid, auxiliaryIn) : new AdminRpcServer.AdminRpcGetMailboxTable(callerSecurityContext, lparam, propTags, cpid, auxiliaryIn);
			ErrorCode errorCode = adminRpcGetMailboxTable.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				result = adminRpcGetMailboxTable.Result;
				rowCount = adminRpcGetMailboxTable.RowCount;
			}
			auxiliaryOut = adminRpcGetMailboxTable.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminNotifyOnDSChange50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint obj, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			auxiliaryOut = Array<byte>.Empty;
			return (int)ErrorCode.NoError;
		}

		public int EcSyncMailboxWithDS50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminRpcSyncMailboxWithDS adminRpcSyncMailboxWithDS = new AdminRpcServer.AdminRpcSyncMailboxWithDS(callerSecurityContext, mdbGuid, mailboxGuid, auxiliaryIn);
			ErrorCode errorCode = adminRpcSyncMailboxWithDS.EcExecute();
			auxiliaryOut = adminRpcSyncMailboxWithDS.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetMailboxTableEntry50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint[] propTags, uint cpid, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminGetMailboxTableEntry50 adminGetMailboxTableEntry = new AdminRpcServer.AdminGetMailboxTableEntry50(callerSecurityContext, mdbGuid, mailboxGuid, GetMailboxInfoTableFlags.None, propTags, auxiliaryIn);
			ErrorCode errorCode = adminGetMailboxTableEntry.EcExecute();
			result = adminGetMailboxTableEntry.Result;
			rowCount = ((errorCode == ErrorCode.NoError) ? 1U : 0U);
			auxiliaryOut = adminGetMailboxTableEntry.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetMailboxTableEntryFlags50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint flags, uint[] propTags, uint cpid, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminGetMailboxTableEntry50 adminGetMailboxTableEntry = new AdminRpcServer.AdminGetMailboxTableEntry50(callerSecurityContext, mdbGuid, mailboxGuid, (GetMailboxInfoTableFlags)flags, propTags, auxiliaryIn);
			ErrorCode errorCode = adminGetMailboxTableEntry.EcExecute();
			result = adminGetMailboxTableEntry.Result;
			rowCount = ((errorCode == ErrorCode.NoError) ? 1U : 0U);
			auxiliaryOut = adminGetMailboxTableEntry.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetViewsTableEx50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, LTID folderLTID, uint[] propTags, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminGetViewsTableEx50 adminGetViewsTableEx = new AdminRpcServer.AdminGetViewsTableEx50(callerSecurityContext, mdbGuid, mailboxGuid, folderLTID, propTags, auxiliaryIn);
			ErrorCode errorCode = adminGetViewsTableEx.EcExecute();
			result = adminGetViewsTableEx.Result;
			rowCount = adminGetViewsTableEx.ResultRowCount;
			auxiliaryOut = adminGetViewsTableEx.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminGetRestrictionTableEx50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, LTID folderLTID, uint[] propTags, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminGetRestrictionTableEx50 adminGetRestrictionTableEx = new AdminRpcServer.AdminGetRestrictionTableEx50(callerSecurityContext, mdbGuid, mailboxGuid, folderLTID, propTags, auxiliaryIn);
			ErrorCode errorCode = adminGetRestrictionTableEx.EcExecute();
			result = adminGetRestrictionTableEx.Result;
			rowCount = adminGetRestrictionTableEx.ResultRowCount;
			auxiliaryOut = adminGetRestrictionTableEx.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminExecuteTask50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid taskClass, int taskId, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminExecuteTask50 adminExecuteTask = new AdminRpcServer.AdminExecuteTask50(callerSecurityContext, mdbGuid, taskClass, taskId, auxiliaryIn);
			ErrorCode errorCode = adminExecuteTask.EcExecute();
			auxiliaryOut = adminExecuteTask.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminPrePopulateCacheEx50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] partitionHint, string domainController, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminPrePopulateCacheEx50 adminPrePopulateCacheEx = new AdminRpcServer.AdminPrePopulateCacheEx50(callerSecurityContext, mdbGuid, mailboxGuid, partitionHint, domainController, auxiliaryIn);
			ErrorCode errorCode = adminPrePopulateCacheEx.EcExecute();
			auxiliaryOut = adminPrePopulateCacheEx.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcMultiMailboxSearch(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] searchRequest, out byte[] searchResultsOut, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.TraceFunction("Entering AdminRpcServer.EcMultiMailboxSearch");
			int result = this.ExecuteEcMultiMailboxSearch(null, callerSecurityContext, mdbGuid, searchRequest, out searchResultsOut, auxiliaryIn, out auxiliaryOut);
			AdminRpcServer.TraceFunction("Exiting AdminRpcServer.EcMultiMailboxSearch");
			return result;
		}

		public int EcGetMultiMailboxSearchKeywordStats(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] keywordStatRequest, out byte[] keywordStatsResultsOut, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.TraceFunction("Entering AdminRpcServer.EcGetMultiMailboxSearchKeywordStats");
			int result = this.ExecuteEcGetMultiMailboxSearchKeywordStats(null, callerSecurityContext, mdbGuid, keywordStatRequest, out keywordStatsResultsOut, auxiliaryIn, out auxiliaryOut);
			AdminRpcServer.TraceFunction("Exiting AdminRpcServer.EcGetMultiMailboxSearchKeywordStats");
			return result;
		}

		internal int ExecuteEcMultiMailboxSearch(IMultiMailboxSearch searcher, ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] searchRequestByteArray, out byte[] searchResponseByteArray, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.TraceFunction("Entering AdminRpcServer.ExecuteEcMultiMailboxSearch");
			searchResponseByteArray = null;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcMultiMailboxSearch adminRpcMultiMailboxSearch = new AdminRpcServer.AdminRpcMultiMailboxSearch(callerSecurityContext, mdbGuid, searchRequestByteArray, auxiliaryIn);
			IDisposable disposable = null;
			if (searcher != null)
			{
				disposable = adminRpcMultiMailboxSearch.SetMultiMailboxSearchTestHook(searcher);
			}
			ErrorCode errorCode = ErrorCode.NoError;
			using (disposable)
			{
				errorCode = adminRpcMultiMailboxSearch.EcExecute();
				if (errorCode == ErrorCode.NoError)
				{
					searchResponseByteArray = adminRpcMultiMailboxSearch.ResponseAsByteArray;
				}
			}
			auxiliaryOut = adminRpcMultiMailboxSearch.AuxiliaryOut;
			AdminRpcServer.TraceFunction("Exiting AdminRpcServer.ExecuteEcMultiMailboxSearch");
			return (int)errorCode;
		}

		internal int ExecuteEcGetMultiMailboxSearchKeywordStats(IMultiMailboxSearch searcher, ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] searchRequestByteArray, out byte[] searchResponseByteArray, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.TraceFunction("Entering AdminRpcServer.ExecuteEcGetMultiMailboxSearchKeywordStats");
			searchResponseByteArray = null;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcMultiMailboxSearchKeywordStats adminRpcMultiMailboxSearchKeywordStats = new AdminRpcServer.AdminRpcMultiMailboxSearchKeywordStats(callerSecurityContext, mdbGuid, searchRequestByteArray, auxiliaryIn);
			IDisposable disposable = null;
			if (searcher != null)
			{
				disposable = adminRpcMultiMailboxSearchKeywordStats.SetMultiMailboxSearchTestHook(searcher);
			}
			ErrorCode errorCode = ErrorCode.NoError;
			using (disposable)
			{
				errorCode = adminRpcMultiMailboxSearchKeywordStats.EcExecute();
				if (errorCode == ErrorCode.NoError)
				{
					searchResponseByteArray = adminRpcMultiMailboxSearchKeywordStats.ResponseAsByteArray;
				}
			}
			auxiliaryOut = adminRpcMultiMailboxSearchKeywordStats.AuxiliaryOut;
			AdminRpcServer.TraceFunction("Exiting AdminRpcServer.ExecuteEcGetMultiMailboxSearchKeywordStats");
			return (int)errorCode;
		}

		private static void TraceFunction(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}
			if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ExTraceGlobals.MultiMailboxSearchTracer.TraceFunction(36368, 0L, message);
			}
		}

		public int EcCreateUserInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] properties, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminCreateUserInfo50 adminCreateUserInfo = new AdminRpcServer.AdminCreateUserInfo50(callerSecurityContext, mdbGuid, userInfoGuid, flags, properties, auxiliaryIn);
			ErrorCode errorCode = adminCreateUserInfo.EcExecute();
			auxiliaryOut = adminCreateUserInfo.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcReadUserInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid userInfoGuid, uint flags, uint[] propertyTags, out ArraySegment<byte> result, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminReadUserInfo50 adminReadUserInfo = new AdminRpcServer.AdminReadUserInfo50(callerSecurityContext, mdbGuid, userInfoGuid, flags, propertyTags, auxiliaryIn);
			ErrorCode errorCode = adminReadUserInfo.EcExecute();
			result = adminReadUserInfo.Result;
			auxiliaryOut = adminReadUserInfo.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcUpdateUserInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] properties, uint[] deletePropertyTags, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			AdminRpcServer.AdminUpdateUserInfo50 adminUpdateUserInfo = new AdminRpcServer.AdminUpdateUserInfo50(callerSecurityContext, mdbGuid, userInfoGuid, flags, properties, deletePropertyTags, auxiliaryIn);
			ErrorCode errorCode = adminUpdateUserInfo.EcExecute();
			auxiliaryOut = adminUpdateUserInfo.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcDeleteUserInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			auxiliaryOut = null;
			return -2147221246;
		}

		private static Properties ParseProperties(byte[] propertiesBuffer, uint[] deletePropertyTags)
		{
			Properties result = new Properties(20);
			if (propertiesBuffer != null && propertiesBuffer.Length != 0)
			{
				using (Reader reader = new BufferReader(new ArraySegment<byte>(propertiesBuffer)))
				{
					PropertyValue[] array = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
					foreach (PropertyValue propertyValue in array)
					{
						object value = propertyValue.Value;
						RcaTypeHelpers.MassageIncomingPropertyValue(propertyValue.PropertyTag, ref value);
						result.Add(LegacyHelper.ConvertFromLegacyPropTag(propertyValue.PropertyTag, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.UserInfo, null, false), value);
					}
				}
			}
			if (deletePropertyTags != null && deletePropertyTags.Length != 0)
			{
				StorePropTag[] array3 = LegacyHelper.ConvertFromLegacyPropTags(deletePropertyTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.UserInfo, null, false);
				foreach (StorePropTag storePropTag in array3)
				{
					if (result.Contains(storePropTag))
					{
						throw new StoreException((LID)54716U, ErrorCodeValue.InvalidParameter);
					}
					result.Add(storePropTag, null);
				}
			}
			return result;
		}

		public int EcAdminISIntegCheck50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint flags, uint[] taskIds, out string requestId, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			requestId = null;
			AdminRpcServer.AdminRpcStoreIntegrityCheck adminRpcStoreIntegrityCheck = new AdminRpcServer.AdminRpcStoreIntegrityCheck(callerSecurityContext, mdbGuid, mailboxGuid, flags, taskIds, auxiliaryIn);
			ErrorCode errorCode = adminRpcStoreIntegrityCheck.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				requestId = adminRpcStoreIntegrityCheck.RequestGuid.ToString();
			}
			auxiliaryOut = adminRpcStoreIntegrityCheck.AuxiliaryOut;
			return (int)errorCode;
		}

		public int EcAdminIntegrityCheckEx50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint operation, byte[] request, out byte[] response, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			response = null;
			auxiliaryOut = null;
			AdminRpcServer.AdminRpcStoreIntegrityCheckEx adminRpcStoreIntegrityCheckEx = new AdminRpcServer.AdminRpcStoreIntegrityCheckEx(callerSecurityContext, mdbGuid, mailboxGuid, operation, request, auxiliaryIn);
			ErrorCode errorCode = adminRpcStoreIntegrityCheckEx.EcExecute();
			if (errorCode == ErrorCode.NoError)
			{
				response = adminRpcStoreIntegrityCheckEx.Response;
			}
			auxiliaryOut = adminRpcStoreIntegrityCheckEx.AuxiliaryOut;
			return (int)errorCode;
		}

		public const uint MailboxSignatureVersion = 104U;

		public const uint DeleteMailboxVersion = 1U;

		public const uint InTransitStatusVersion = 1U;

		public const uint MailboxShapeVersion = 4U;

		public const ushort MajorInterfaceVersion = 7;

		public const ushort MinorInterfaceVersion = 17;

		public enum VersionedFeature
		{
			None,
			MailboxSignatureServerVersion,
			DeleteMailboxServerVersion,
			IntransitStatusServerVersion,
			MailboxShapeServerVersion
		}

		internal class AdminRpcListAllMdbStatus : AdminRpc
		{
			public AdminRpcListAllMdbStatus(ClientSecurityContext callerSecurityContext, bool basicInformation, byte[] auxiliaryIn) : base(AdminMethod.EcListAllMdbStatus50, callerSecurityContext, auxiliaryIn)
			{
				this.basicInformation = basicInformation;
			}

			public uint MdbCount
			{
				get
				{
					return this.mdbCount;
				}
			}

			public byte[] MdbStatusRaw
			{
				get
				{
					return this.mdbStatusRaw;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ICollection<StoreDatabase> databaseListSnapshot = Storage.GetDatabaseListSnapshot();
				this.mdbCount = (uint)databaseListSnapshot.Count;
				List<AdminRpcServer.AdminRpcListAllMdbStatus.MdbStatus> list = new List<AdminRpcServer.AdminRpcListAllMdbStatus.MdbStatus>((int)this.MdbCount);
				foreach (StoreDatabase storeDatabase in databaseListSnapshot)
				{
					storeDatabase.GetSharedLock(context.Diagnostics);
					try
					{
						list.Add(new AdminRpcServer.AdminRpcListAllMdbStatus.MdbStatus(storeDatabase.MdbGuid, (ulong)storeDatabase.ExternalMdbStatus, storeDatabase.MdbName, storeDatabase.VServerName, storeDatabase.LegacyDN));
					}
					finally
					{
						storeDatabase.ReleaseSharedLock();
					}
				}
				AdminRpcParseFormat.SerializeMdbStatus(list, this.basicInformation, out this.mdbStatusRaw);
				return ErrorCode.NoError;
			}

			private bool basicInformation;

			private uint mdbCount;

			private byte[] mdbStatusRaw;

			public struct MdbStatus
			{
				public MdbStatus(Guid guidMdb, ulong ulStatus, string mdbName, string vServerName, string legacyDN)
				{
					this.guidMdb = guidMdb;
					this.ulStatus = ulStatus;
					this.mdbName = mdbName;
					this.vServerName = vServerName;
					this.legacyDN = legacyDN;
				}

				public Guid GuidMdb
				{
					get
					{
						return this.guidMdb;
					}
				}

				public ulong UlStatus
				{
					get
					{
						return this.ulStatus;
					}
				}

				public string MdbName
				{
					get
					{
						return this.mdbName;
					}
				}

				public string VServerName
				{
					get
					{
						return this.vServerName;
					}
				}

				public string LegacyDN
				{
					get
					{
						return this.legacyDN;
					}
				}

				private Guid guidMdb;

				private ulong ulStatus;

				private string mdbName;

				private string vServerName;

				private string legacyDN;
			}
		}

		internal class AdminRpcGetDatabaseSize : AdminRpc
		{
			public AdminRpcGetDatabaseSize(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] auxiliaryIn) : base(AdminMethod.EcGetDatabaseSizeEx50, callerSecurityContext, new Guid?(mdbGuid), auxiliaryIn)
			{
			}

			public uint TotalPages
			{
				get
				{
					return this.totalPages;
				}
			}

			public uint AvailablePages
			{
				get
				{
					return this.availablePages;
				}
			}

			public uint PageSize
			{
				get
				{
					return this.pageSize;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				base.Database.PhysicalDatabase.GetDatabaseSize(context, out this.totalPages, out this.availablePages, out this.pageSize);
				return ErrorCode.NoError;
			}

			private uint totalPages;

			private uint availablePages;

			private uint pageSize;
		}

		internal class AdminRpcStartBlockModeReplicationToPassive : AdminRpc
		{
			public AdminRpcStartBlockModeReplicationToPassive(ClientSecurityContext callerSecurityContext, Guid mdbGuid, string passiveName, uint firstGenToSend, byte[] auxiliaryIn) : base(AdminMethod.EcAdminStartBlockModeReplicationToPassive50, callerSecurityContext, auxiliaryIn)
			{
				base.MdbGuid = new Guid?(mdbGuid);
				this.passiveName = passiveName;
				this.firstGenToSend = firstGenToSend;
			}

			protected override ErrorCode EcCheckPermissions(MapiContext context)
			{
				return AdminRpcPermissionChecks.EcCheckConstrainedDelegationRights(context, base.DatabaseInfo);
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				StoreDatabase storeDatabase = Storage.FindDatabase(base.MdbGuid.Value);
				if (storeDatabase == null)
				{
					errorCode = ErrorCode.CreateNotFound((LID)49352U);
				}
				else
				{
					storeDatabase.GetSharedLock(context.Diagnostics);
					try
					{
						JetHADatabase jetHADatabase = storeDatabase.PhysicalDatabase as JetHADatabase;
						if (jetHADatabase == null)
						{
							errorCode = ErrorCode.CreateBlockModeInitFailed((LID)41928U);
						}
						else
						{
							errorCode = jetHADatabase.StartBlockModeReplicationToPassive(context.Diagnostics, this.passiveName, this.firstGenToSend);
						}
					}
					finally
					{
						storeDatabase.ReleaseSharedLock();
					}
				}
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)48840U);
				}
				return errorCode;
			}

			private string passiveName;

			private uint firstGenToSend;
		}

		internal class AdminRpcPurgeCachedMdbObject : AdminRpc
		{
			internal AdminRpcPurgeCachedMdbObject(ClientSecurityContext callerSecurityContext, Guid databaseGuid, byte[] auxiliaryIn) : base(AdminMethod.EcPurgeCachedMdbObject50, callerSecurityContext, auxiliaryIn)
			{
				this.databaseGuid = databaseGuid;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.RefreshDatabaseInfo(context, this.databaseGuid);
				ServerInfo serverInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(context);
				if (this.databaseGuid.Equals(serverInfo.Guid))
				{
					Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.RefreshServerInfo(context);
				}
				return ErrorCode.NoError;
			}

			private readonly Guid databaseGuid;
		}

		internal class AdminDoMaintenanceTask : AdminRpc
		{
			public AdminDoMaintenanceTask(ClientSecurityContext callerSecurityContext, Guid databaseGuid, uint task, byte[] auxiliaryIn) : base(AdminMethod.EcDoMaintenanceTask50, callerSecurityContext, auxiliaryIn)
			{
				this.task = (AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask)task;
				base.MdbGuid = new Guid?(databaseGuid);
			}

			internal override int OperationDetail
			{
				get
				{
					return (int)(this.task + 2000U);
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode result = ErrorCode.NoError;
				AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask maintenanceTask = this.task;
				if (maintenanceTask != AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask.ReReadMDBQuotas)
				{
					switch (maintenanceTask)
					{
					case AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask.PurgeDatabaseCache:
						result = Storage.PurgeDatabaseCache(context, base.MdbGuid.Value).Propagate((LID)38152U);
						break;
					case AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask.ExtendDatabase:
						result = Storage.ExtendDatabase(context, base.MdbGuid.Value).Propagate((LID)44648U);
						break;
					case AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask.ShrinkDatabase:
						result = Storage.ShrinkDatabase(context, base.MdbGuid.Value).Propagate((LID)64264U);
						break;
					case AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask.CleanupVersionStore:
						if (!DefaultSettings.Get.VersionStoreCleanupMaintenanceTaskSupported)
						{
							result = ErrorCode.CreateWithLid((LID)44028U, ErrorCodeValue.NotSupported);
						}
						else
						{
							result = Storage.VersionStoreCleanup(context, base.MdbGuid.Value).Propagate((LID)60412U);
						}
						break;
					default:
						result = ErrorCode.CreateWithLid((LID)57016U, ErrorCodeValue.NotSupported);
						break;
					}
				}
				else
				{
					Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.RefreshDatabaseInfo(context, base.MdbGuid.Value);
				}
				return result;
			}

			private readonly AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask task;

			internal enum MaintenanceTask : uint
			{
				FolderTombstones = 1U,
				FolderConflictAging,
				SiteFolderCheck,
				EventHistoryCleanup,
				TombstonesMaintenance,
				PurgeIndices,
				PFExpiry,
				UpdateServerVersions,
				HardDeletes,
				DeletedMailboxCleanup,
				ReReadMDBQuotas,
				ReReadAuditInfo,
				InvCachedDsInfo,
				DbSizeCheck,
				DeliveredToCleanup,
				FolderCleanup,
				AgeOutAllFolders,
				AgeOutAllViews,
				AgeOutAllDVUEntries,
				MdbHealthCheck,
				QuarantinedMailboxCleanup,
				RequestTimeoutTest,
				DeletedCiFailedItemCleanup,
				ISINTEGProvisionedFolders,
				PurgeDatabaseCache = 100U,
				ExtendDatabase,
				ShrinkDatabase,
				CleanupVersionStore
			}
		}

		internal class AdminRpcGetLastBackupInfo50 : AdminRpc
		{
			public AdminRpcGetLastBackupInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] auxiliaryIn) : base(AdminMethod.EcGetLastBackupTimes50, callerSecurityContext, new Guid?(mdbGuid), auxiliaryIn)
			{
			}

			public DateTime LastFullBackupTime
			{
				get
				{
					return this.lastFullBackupTime;
				}
			}

			public DateTime LastIncrementalBackupTime
			{
				get
				{
					return this.lastIncrementalBackupTime;
				}
			}

			public DateTime LastDifferentialBackupTime
			{
				get
				{
					return this.lastDifferentialBackupTime;
				}
			}

			public DateTime LastCopyBackupTime
			{
				get
				{
					return this.lastCopyBackupTime;
				}
			}

			public int SnapshotFullBackup
			{
				get
				{
					return this.snapFull;
				}
			}

			public int SnapshotIncrementalBackup
			{
				get
				{
					return this.snapIncremental;
				}
			}

			public int SnapshotDifferentialBackup
			{
				get
				{
					return this.snapDifferential;
				}
			}

			public int SnapshotCopyBackup
			{
				get
				{
					return this.snapCopy;
				}
			}

			protected override ErrorCode EcCheckPermissions(MapiContext context)
			{
				return AdminRpcPermissionChecks.EcDefaultCheck(context, base.DatabaseInfo);
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				base.Database.PhysicalDatabase.GetLastBackupInformation(context, out this.lastFullBackupTime, out this.lastIncrementalBackupTime, out this.lastDifferentialBackupTime, out this.lastCopyBackupTime, out this.snapFull, out this.snapIncremental, out this.snapDifferential, out this.snapCopy);
				return ErrorCode.NoError;
			}

			private DateTime lastFullBackupTime;

			private DateTime lastIncrementalBackupTime;

			private DateTime lastDifferentialBackupTime;

			private DateTime lastCopyBackupTime;

			private int snapFull;

			private int snapIncremental;

			private int snapDifferential;

			private int snapCopy;
		}

		internal sealed class AdminRpcLogReplayRequest2 : AdminRpc
		{
			public AdminRpcLogReplayRequest2(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint logReplayMax, uint logReplayFlags, byte[] auxiliaryIn) : base(AdminMethod.EcLogReplayRequest2, callerSecurityContext, new Guid?(mdbGuid), AdminRpc.ExpectedDatabaseState.AnyOnlineState, auxiliaryIn)
			{
				this.logReplayMax = logReplayMax;
				this.logReplayFlags = logReplayFlags;
			}

			public uint LogReplayNext { get; private set; }

			public byte[] DatabaseHeaderInfo { get; private set; }

			public uint PatchPageNumber { get; private set; }

			public byte[] PatchToken { get; private set; }

			public byte[] PatchData { get; private set; }

			public uint[] CorruptPages { get; private set; }

			protected override ErrorCode EcCheckPermissions(MapiContext context)
			{
				return AdminRpcPermissionChecks.EcCheckConstrainedDelegationRights(context, base.DatabaseInfo);
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				if (!base.Database.IsOnlinePassive)
				{
					base.Database.TraceState((LID)54268U);
					return ErrorCode.CreateNotFound((LID)43672U);
				}
				base.Database.PhysicalDatabase.LogReplayStatus.SetMaxLogGenerationToReplay(this.logReplayMax, this.logReplayFlags);
				uint logReplayNext;
				byte[] databaseHeaderInfo;
				uint patchPageNumber;
				byte[] patchToken;
				byte[] patchData;
				uint[] corruptPages;
				base.Database.PhysicalDatabase.LogReplayStatus.GetReplayStatus(out logReplayNext, out databaseHeaderInfo, out patchPageNumber, out patchToken, out patchData, out corruptPages);
				this.LogReplayNext = logReplayNext;
				this.DatabaseHeaderInfo = databaseHeaderInfo;
				this.PatchPageNumber = patchPageNumber;
				this.PatchToken = patchToken;
				this.PatchData = patchData;
				this.CorruptPages = corruptPages;
				return ErrorCode.NoError;
			}

			private readonly uint logReplayMax;

			private readonly uint logReplayFlags;
		}

		internal sealed class AdminRpcGetResourceMonitorDigest : AdminRpc
		{
			public AdminRpcGetResourceMonitorDigest(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint[] propertyTags, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetResourceMonitorDigest50, callerSecurityContext, new Guid?(mdbGuid), AdminRpc.ExpectedDatabaseState.AnyAttachedState, auxiliaryIn)
			{
				this.propertyTags = propertyTags;
				this.RowCount = 0U;
				this.Result = null;
			}

			public byte[] Result { get; private set; }

			public uint RowCount { get; private set; }

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode errorCode = base.EcValidateArguments(context);
				if (!(ErrorCode.NoError != errorCode) && (this.propertyTags == null || this.propertyTags.Length == 0))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)50984U);
				}
				return errorCode;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				StorePropTag[] storePropTags = LegacyHelper.ConvertFromLegacyPropTags(this.propertyTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Mailbox, null, true);
				List<Properties> digestRows = this.GetDigestRows(context, context.Database, storePropTags);
				if (digestRows != null && digestRows.Count > 0)
				{
					int num = 0;
					int num2 = 0;
					for (int i = 0; i < digestRows.Count; i++)
					{
						LegacyHelper.MassageOutgoingProperties(digestRows[i]);
						num2 += AdminRpcParseFormat.SetValues(null, ref num, 0, digestRows[i]);
					}
					byte[] array = new byte[num2];
					num = 0;
					for (int j = 0; j < digestRows.Count; j++)
					{
						AdminRpcParseFormat.SetValues(array, ref num, array.Length, digestRows[j]);
					}
					this.RowCount = (uint)digestRows.Count;
					this.Result = array;
				}
				return ErrorCode.NoError;
			}

			private List<Properties> GetDigestRows(MapiContext context, StoreDatabase database, StorePropTag[] storePropTags)
			{
				ResourceMonitorDigestSnapshot digestHistory = database.ResourceDigest.GetDigestHistory();
				int num = ((digestHistory.TimeInServerDigest != null) ? digestHistory.TimeInServerDigest.Length : 0) + ((digestHistory.LogRecordBytesDigest != null) ? digestHistory.LogRecordBytesDigest.Length : 0);
				if (num == 0)
				{
					return null;
				}
				List<Properties> list = new List<Properties>(num * 25 * 2);
				if (digestHistory.TimeInServerDigest != null && digestHistory.TimeInServerDigest.Length > 0)
				{
					for (int i = 0; i < digestHistory.TimeInServerDigest.Length; i++)
					{
						ResourceDigestStats[] array = digestHistory.TimeInServerDigest[i];
						if (array != null && array.Length > 0)
						{
							for (int j = 0; j < array.Length; j++)
							{
								list.Add(this.GetRowFromSample(context, i + 1, "TimeInServer", array[j], storePropTags));
							}
						}
					}
				}
				if (digestHistory.LogRecordBytesDigest != null && digestHistory.LogRecordBytesDigest.Length > 0)
				{
					for (int k = 0; k < digestHistory.LogRecordBytesDigest.Length; k++)
					{
						ResourceDigestStats[] array2 = digestHistory.LogRecordBytesDigest[k];
						if (array2 != null && array2.Length > 0)
						{
							for (int l = 0; l < array2.Length; l++)
							{
								list.Add(this.GetRowFromSample(context, k + 1, "LogBytes", array2[l], storePropTags));
							}
						}
					}
				}
				return list;
			}

			private Properties GetRowFromSample(MapiContext context, int sampleId, string digestCategory, ResourceDigestStats sample, StorePropTag[] storePropTags)
			{
				string text;
				bool flag;
				this.GetMailboxDetails(context, sample.MailboxNumber, out text, out flag);
				Properties result = new Properties(storePropTags.Length);
				for (int i = 0; i < storePropTags.Length; i++)
				{
					object obj = null;
					ushort propId = storePropTags[i].PropId;
					if (propId <= 26375)
					{
						if (propId != 12289)
						{
							if (propId == 26375)
							{
								obj = sample.MailboxGuid.ToByteArray();
							}
						}
						else
						{
							obj = text;
						}
					}
					else
					{
						switch (propId)
						{
						case 26413:
							obj = (int)sample.TimeInServer.TotalMilliseconds;
							break;
						case 26414:
							obj = (int)sample.TimeInCPU.TotalMilliseconds;
							break;
						case 26415:
							obj = sample.ROPCount;
							break;
						case 26416:
							obj = sample.PageRead;
							break;
						case 26417:
							obj = sample.PagePreread;
							break;
						case 26418:
							obj = sample.LogRecordCount;
							break;
						case 26419:
							obj = sample.LogRecordBytes;
							break;
						case 26420:
							obj = sample.LdapReads;
							break;
						case 26421:
							obj = sample.LdapSearches;
							break;
						case 26422:
							obj = digestCategory;
							break;
						case 26423:
							obj = sampleId;
							break;
						case 26424:
							obj = sample.SampleTime;
							break;
						default:
							if (propId == 26650)
							{
								obj = flag;
							}
							break;
						}
					}
					if (obj != null)
					{
						result.Add(storePropTags[i], obj);
					}
					else
					{
						result.Add(storePropTags[i].ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
					}
				}
				return result;
			}

			private void GetMailboxDetails(MapiContext context, int mailboxNumber, out string displayName, out bool quarantined)
			{
				displayName = null;
				quarantined = false;
				if (this.mailboxInfo == null)
				{
					this.mailboxInfo = new Dictionary<int, AdminRpcServer.AdminRpcGetResourceMonitorDigest.MailboxDigestInfo>();
				}
				AdminRpcServer.AdminRpcGetResourceMonitorDigest.MailboxDigestInfo value;
				if (this.mailboxInfo.TryGetValue(mailboxNumber, out value))
				{
					displayName = value.DisplayName;
					quarantined = value.Quarantined;
					return;
				}
				TimeSpan lockTimeout = TimeSpan.FromSeconds(5.0);
				context.InitializeMailboxExclusiveOperation(mailboxNumber, ExecutionDiagnostics.OperationSource.AdminRpc, lockTimeout);
				try
				{
					ErrorCode first = context.StartMailboxOperation(MailboxCreation.DontAllow, false, true);
					if (first != ErrorCode.NoError)
					{
						return;
					}
					quarantined = context.LockedMailboxState.Quarantined;
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
					{
						if (mailbox != null)
						{
							displayName = mailbox.GetDisplayName(context);
						}
					}
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(false);
					}
				}
				value.DisplayName = displayName;
				value.Quarantined = quarantined;
				this.mailboxInfo[mailboxNumber] = value;
			}

			private readonly uint[] propertyTags;

			private Dictionary<int, AdminRpcServer.AdminRpcGetResourceMonitorDigest.MailboxDigestInfo> mailboxInfo;

			private struct MailboxDigestInfo
			{
				public string DisplayName;

				public bool Quarantined;
			}
		}

		internal sealed class AdminRpcGetDatabaseProcessInfo : AdminRpc
		{
			public AdminRpcGetDatabaseProcessInfo(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint[] propTags, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetDatabaseProcessInfo50, callerSecurityContext, new Guid?(mdbGuid), AdminRpc.ExpectedDatabaseState.AnyOnlineState, auxiliaryIn)
			{
				this.propTags = propTags;
			}

			public byte[] Result
			{
				get
				{
					return this.result;
				}
			}

			public uint RowCount
			{
				get
				{
					return this.rowCount;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				StorePropTag[] array = LegacyHelper.ConvertFromLegacyPropTags(this.propTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.ProcessInfo, null, true);
				Properties properties = new Properties(array.Length);
				if (context.Database != null)
				{
					int i = 0;
					while (i < array.Length)
					{
						object obj = null;
						switch (array[i].PropId)
						{
						case 26263:
							using (Process currentProcess = Process.GetCurrentProcess())
							{
								obj = currentProcess.Id;
								break;
							}
							goto IL_87;
						case 26264:
							goto IL_87;
						case 26265:
							obj = StoreDatabase.GetMaximumSchemaVersion().Value;
							break;
						case 26266:
							if (context.Database.IsOnlineActive)
							{
								obj = context.Database.GetCurrentSchemaVersion(context).Value;
							}
							break;
						case 26267:
							if (context.Database.IsOnlineActive)
							{
								obj = context.Database.GetRequestedSchemaVersion(context, context.Database.GetCurrentSchemaVersion(context), StoreDatabase.GetMaximumSchemaVersion()).Value;
							}
							break;
						}
						IL_116:
						if (obj != null)
						{
							properties.Add(array[i], obj);
						}
						else
						{
							properties.Add(array[i].ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
						}
						i++;
						continue;
						IL_87:
						obj = StoreDatabase.GetMinimumSchemaVersion().Value;
						goto IL_116;
					}
					if (properties.Count > 0)
					{
						LegacyHelper.MassageOutgoingProperties(properties);
						int num = 0;
						int num2 = 0;
						int num3 = AdminRpcParseFormat.SetValues(null, ref num, 0, properties);
						byte[] array2 = new byte[num3];
						AdminRpcParseFormat.SetValues(array2, ref num2, array2.Length, properties);
						this.result = array2;
					}
					this.rowCount = 1U;
					return ErrorCode.NoError;
				}
				return ErrorCode.CreateDatabaseError((LID)44712U);
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				if (this.propTags == null || this.propTags.Length == 0)
				{
					return ErrorCode.CreateInvalidParameter((LID)61096U);
				}
				return ErrorCode.NoError;
			}

			private uint[] propTags;

			private byte[] result;

			private uint rowCount;
		}

		internal sealed class EcAdminProcessSnapshotOperation : AdminRpc
		{
			public EcAdminProcessSnapshotOperation(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint opCode, uint flags, byte[] auxiliaryIn) : base(AdminMethod.EcAdminProcessSnapshotOperation50, callerSecurityContext, new Guid?(mdbGuid), auxiliaryIn)
			{
				this.operationCode = (SnapshotOperationCode)opCode;
				this.flags = flags;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				if (base.Database != null && base.Database.IsOnlineActive)
				{
					switch (this.operationCode)
					{
					case SnapshotOperationCode.Prepare:
						if (base.Database.IsBackupInProgress)
						{
							return ErrorCode.CreateBackupInProgress((LID)33832U);
						}
						base.Database.SetBackupInProgress();
						base.Database.PhysicalDatabase.SnapshotPrepare(this.flags);
						break;
					case SnapshotOperationCode.Freeze:
						if (!base.Database.IsBackupInProgress)
						{
							return ErrorCode.CreateInvalidBackupSequence((LID)50216U);
						}
						base.Database.PhysicalDatabase.SnapshotFreeze(this.flags);
						break;
					case SnapshotOperationCode.Thaw:
						if (!base.Database.IsBackupInProgress)
						{
							return ErrorCode.CreateInvalidBackupSequence((LID)47144U);
						}
						base.Database.PhysicalDatabase.SnapshotThaw(this.flags);
						break;
					case SnapshotOperationCode.Truncate:
						if (!base.Database.IsBackupInProgress)
						{
							return ErrorCode.CreateInvalidBackupSequence((LID)63528U);
						}
						base.Database.PhysicalDatabase.SnapshotTruncateLogInstance(this.flags);
						break;
					case SnapshotOperationCode.Stop:
						if (!base.Database.IsBackupInProgress)
						{
							return ErrorCode.CreateInvalidBackupSequence((LID)38952U);
						}
						base.Database.PhysicalDatabase.SnapshotStop(this.flags);
						base.Database.ResetBackupInProgress();
						break;
					default:
						return ErrorCode.CreateInvalidParameter((LID)46928U);
					}
					return ErrorCode.NoError;
				}
				return ErrorCode.CreateDatabaseError((LID)38736U);
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				if (this.operationCode <= SnapshotOperationCode.None || this.operationCode >= SnapshotOperationCode.Last)
				{
					return ErrorCode.CreateInvalidParameter((LID)55120U);
				}
				return ErrorCode.NoError;
			}

			private readonly SnapshotOperationCode operationCode;

			private readonly uint flags;
		}

		internal class EcAdminGetPhysicalDatabaseInformation : AdminRpc
		{
			public EcAdminGetPhysicalDatabaseInformation(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetPhysicalDatabaseInformation50, callerSecurityContext, new Guid?(mdbGuid), AdminRpc.ExpectedDatabaseState.AnyOnlineState, auxiliaryIn)
			{
			}

			public byte[] DatabaseHeaderInfo { get; private set; }

			protected override ErrorCode EcCheckPermissions(MapiContext context)
			{
				return AdminRpcPermissionChecks.EcCheckConstrainedDelegationRights(context, base.DatabaseInfo);
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				byte[] databaseHeaderInfo = null;
				if (base.Database.IsOnlineActive)
				{
					base.Database.PhysicalDatabase.GetSerializedDatabaseInformation(context, out databaseHeaderInfo);
				}
				else if (base.Database.IsOnlinePassive)
				{
					base.Database.PhysicalDatabase.LogReplayStatus.GetDatabaseInformation(out databaseHeaderInfo);
				}
				this.DatabaseHeaderInfo = databaseHeaderInfo;
				return ErrorCode.NoError;
			}
		}

		internal class EcAdminForceNewLog : AdminRpc
		{
			public EcAdminForceNewLog(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] auxiliaryIn) : base(AdminMethod.EcForceNewLog50, callerSecurityContext, new Guid?(mdbGuid), AdminRpc.ExpectedDatabaseState.OnlineActive, auxiliaryIn)
			{
			}

			protected override ErrorCode EcCheckPermissions(MapiContext context)
			{
				return AdminRpcPermissionChecks.EcDefaultCheck(context, base.DatabaseInfo);
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				base.Database.PhysicalDatabase.ForceNewLog(context);
				return ErrorCode.NoError;
			}
		}

		internal abstract class AdminRpcEventsBase : AdminRpc
		{
			public AdminRpcEventsBase(AdminMethod methodId, ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mdbVersionGuid, byte[] auxiliaryIn) : base(methodId, callerSecurityContext, new Guid?(mdbGuid), auxiliaryIn)
			{
				this.mdbVersionGuid = mdbVersionGuid;
			}

			public AdminRpcEventsBase(AdminMethod methodId, ClientSecurityContext callerSecurityContext, Guid mdbGuid, AdminRpc.ExpectedDatabaseState expectedDatabaseState, Guid mdbVersionGuid, byte[] auxiliaryIn) : base(methodId, callerSecurityContext, new Guid?(mdbGuid), expectedDatabaseState, auxiliaryIn)
			{
				this.mdbVersionGuid = mdbVersionGuid;
			}

			public Guid MdbVersionGuid
			{
				get
				{
					return this.mdbVersionGuid;
				}
			}

			protected ErrorCode CheckMdbVersion()
			{
				EventHistory eventHistory = EventHistory.GetEventHistory(base.Database);
				if (this.mdbVersionGuid == Guid.Empty)
				{
					this.mdbVersionGuid = eventHistory.MdbVersionGuid;
				}
				else if (this.mdbVersionGuid != eventHistory.MdbVersionGuid)
				{
					return ErrorCode.CreateVersionMismatch((LID)42044U);
				}
				return ErrorCode.NoError;
			}

			protected Guid mdbVersionGuid;
		}

		internal class AdminRpcReadMdbEvents : AdminRpcServer.AdminRpcEventsBase
		{
			public AdminRpcReadMdbEvents(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mdbVersionGuid, byte[] request, byte[] auxiliaryIn) : base(AdminMethod.EcReadMdbEvents50, callerSecurityContext, mdbGuid, AdminRpc.ExpectedDatabaseState.AnyAttachedState, mdbVersionGuid, auxiliaryIn)
			{
				this.request = request;
			}

			public byte[] Response
			{
				get
				{
					return this.response;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = base.CheckMdbVersion();
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)54781U);
				}
				else
				{
					EventHistory.ReadEventsFlags readEventsFlags;
					long num;
					uint num2;
					uint num3;
					Restriction restriction;
					errorCode = AdminRpcParseFormat.ParseReadEventsRequest(context, this.request, out readEventsFlags, out num, out num2, out num3, out restriction);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)42493U);
					}
					else
					{
						if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder(100);
							stringBuilder.Append("INPUT  ReadMdbEvents:");
							stringBuilder.Append(" callerSecurityContext:[");
							stringBuilder.Append((base.ClientSecurityContext == null) ? "null" : base.ClientSecurityContext.ToString());
							stringBuilder.Append("] mdbGuid:[");
							stringBuilder.Append(base.MdbGuid.Value);
							stringBuilder.Append("] mdbVersionGuid:[");
							stringBuilder.Append(base.MdbVersionGuid);
							stringBuilder.Append("] readFlags:[");
							stringBuilder.Append(readEventsFlags);
							stringBuilder.Append("] startCounter:[");
							stringBuilder.Append(num);
							stringBuilder.Append("] eventsWant:[");
							stringBuilder.Append(num2);
							stringBuilder.Append("] eventsToCheck:[");
							stringBuilder.Append(num3);
							if (restriction != null)
							{
								stringBuilder.Append("] restriction:[");
								stringBuilder.Append(restriction);
							}
							stringBuilder.Append("]");
							ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder.ToString());
						}
						EventHistory eventHistory = EventHistory.GetEventHistory(base.Database);
						List<EventEntry> list = null;
						long num4 = 0L;
						if (-1L == num)
						{
							EventEntry item;
							errorCode = eventHistory.ReadLastEvent(context, out item);
							if (errorCode == ErrorCode.NoError)
							{
								list = new List<EventEntry>(1);
								list.Add(item);
							}
						}
						else
						{
							if (num < 0L)
							{
								return ErrorCode.CreateRpcFormat((LID)64888U);
							}
							errorCode = eventHistory.ReadEvents(context, num, num2, num3, restriction, readEventsFlags, out list, out num4);
						}
						if (errorCode != ErrorCode.NoError)
						{
							errorCode = errorCode.Propagate((LID)58877U);
						}
						else
						{
							if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder2 = new StringBuilder(100);
								stringBuilder2.Append("OUTPUT  ReadMdbEvents:");
								stringBuilder2.Append(" mdbVersionGuid:[");
								stringBuilder2.Append(base.MdbVersionGuid);
								stringBuilder2.Append("] events.Count:[");
								stringBuilder2.Append((list == null) ? 0 : list.Count);
								stringBuilder2.Append("] endCounter:[");
								stringBuilder2.Append(num4);
								stringBuilder2.Append("]");
								ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder2.ToString());
							}
							errorCode = AdminRpcParseFormat.FormatReadEventsResponse(0, num4, list, out this.response);
							if (errorCode != ErrorCode.NoError)
							{
								errorCode = errorCode.Propagate((LID)34301U);
							}
						}
					}
				}
				return errorCode;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode errorCode = base.EcValidateArguments(context);
				if (!(errorCode != ErrorCode.NoError) && (this.request == null || this.request.Length < 4))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)38397U);
				}
				return errorCode;
			}

			private byte[] request;

			private byte[] response;
		}

		internal class AdminRpcWriteMdbEvents : AdminRpcServer.AdminRpcEventsBase
		{
			public AdminRpcWriteMdbEvents(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mdbVersionGuid, byte[] request, byte[] auxiliaryIn) : base(AdminMethod.EcWriteMdbEvents50, callerSecurityContext, mdbGuid, mdbVersionGuid, auxiliaryIn)
			{
				this.request = request;
			}

			public byte[] Response
			{
				get
				{
					return this.response;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = base.CheckMdbVersion();
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)50685U);
				}
				else
				{
					int value;
					List<EventEntry> list;
					errorCode = AdminRpcParseFormat.ParseWriteEventsRequest(this.request, out value, out list);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)45397U);
					}
					else
					{
						if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder(100);
							stringBuilder.Append("INPUT  WriteMdbEvents:");
							stringBuilder.Append(" callerSecurityContext:[");
							stringBuilder.Append((base.ClientSecurityContext == null) ? "null" : base.ClientSecurityContext.ToString());
							stringBuilder.Append("] mdbGuid:[");
							stringBuilder.Append(base.MdbGuid.Value);
							stringBuilder.Append("] mdbVersionGuid:[");
							stringBuilder.Append(base.MdbVersionGuid);
							stringBuilder.Append("] writeFlags:[");
							stringBuilder.Append(value);
							stringBuilder.Append("] events.Count:[");
							stringBuilder.Append((list == null) ? 0 : list.Count);
							stringBuilder.Append("]");
							ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder.ToString());
						}
						EventHistory eventHistory = EventHistory.GetEventHistory(base.Database);
						List<long> list2;
						errorCode = eventHistory.WriteEvents(context, list, out list2);
						if (errorCode != ErrorCode.NoError)
						{
							errorCode = errorCode.Propagate((LID)61781U);
						}
						else
						{
							if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								StringBuilder stringBuilder2 = new StringBuilder(100);
								stringBuilder2.Append("OUTPUT  WriteMdbEvents:");
								stringBuilder2.Append(" mdbVersionGuid:[");
								stringBuilder2.Append(base.MdbVersionGuid);
								stringBuilder2.Append("] eventCounters.Count:[");
								stringBuilder2.Append((list2 == null) ? 0 : list2.Count);
								stringBuilder2.Append("]");
								ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder2.ToString());
							}
							errorCode = AdminRpcParseFormat.FormatWriteEventsResponse(list2, out this.response);
							if (errorCode != ErrorCode.NoError)
							{
								errorCode = errorCode.Propagate((LID)37205U);
							}
						}
					}
				}
				return errorCode;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode errorCode = base.EcValidateArguments(context);
				if (!(errorCode != ErrorCode.NoError) && (this.request == null || this.request.Length < 4))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)41301U);
				}
				return errorCode;
			}

			private byte[] request;

			private byte[] response;
		}

		internal class AdminRpcDeleteMdbWatermarksForConsumer : AdminRpcServer.AdminRpcEventsBase
		{
			public AdminRpcDeleteMdbWatermarksForConsumer(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mdbVersionGuid, Guid? mailboxDsGuid, Guid consumerGuid, byte[] auxiliaryIn) : base(AdminMethod.EcDeleteMdbWatermarksForConsumer50, callerSecurityContext, mdbGuid, mdbVersionGuid, auxiliaryIn)
			{
				this.mailboxDsGuid = mailboxDsGuid;
				this.consumerGuid = consumerGuid;
			}

			public uint DeletedCount
			{
				get
				{
					return this.deletedCount;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = base.CheckMdbVersion();
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)57685U);
				}
				else
				{
					if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("INPUT  DeleteMdbWatermarksForConsumer:");
						stringBuilder.Append(" callerSecurityContext:[");
						stringBuilder.Append((base.ClientSecurityContext == null) ? "null" : base.ClientSecurityContext.ToString());
						stringBuilder.Append("] mdbGuid:[");
						stringBuilder.Append(base.MdbGuid.Value);
						stringBuilder.Append("] mdbVersionGuid:[");
						stringBuilder.Append(base.MdbVersionGuid);
						stringBuilder.Append("] consumerGuid:[");
						stringBuilder.Append(this.consumerGuid);
						if (this.mailboxDsGuid != null)
						{
							stringBuilder.Append("] mailboxGuid:[");
							stringBuilder.Append(this.mailboxDsGuid);
						}
						stringBuilder.Append("]");
						ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					EventHistory eventHistory = EventHistory.GetEventHistory(base.Database);
					errorCode = eventHistory.DeleteWatermarksForConsumer(context, this.consumerGuid, this.mailboxDsGuid, out this.deletedCount);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)33109U);
					}
					else if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder2 = new StringBuilder(100);
						stringBuilder2.Append("OUTPUT  DeleteMdbWatermarksForConsumer:");
						stringBuilder2.Append(" mdbVersionGuid:[");
						stringBuilder2.Append(base.MdbVersionGuid);
						stringBuilder2.Append("] deletedCount:[");
						stringBuilder2.Append(this.deletedCount);
						stringBuilder2.Append("]");
						ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder2.ToString());
					}
				}
				return errorCode;
			}

			private Guid? mailboxDsGuid;

			private Guid consumerGuid;

			private uint deletedCount;
		}

		internal class AdminRpcDeleteMdbWatermarksForMailbox : AdminRpcServer.AdminRpcEventsBase
		{
			public AdminRpcDeleteMdbWatermarksForMailbox(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mdbVersionGuid, Guid mailboxDsGuid, byte[] auxiliaryIn) : base(AdminMethod.EcDeleteMdbWatermarksForMailbox50, callerSecurityContext, mdbGuid, mdbVersionGuid, auxiliaryIn)
			{
				this.mailboxDsGuid = mailboxDsGuid;
			}

			public uint DeletedCount
			{
				get
				{
					return this.deletedCount;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = base.CheckMdbVersion();
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)48981U);
				}
				else
				{
					if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("INPUT  DeleteMdbWatermarksForMailbox:");
						stringBuilder.Append(" callerSecurityContext:[");
						stringBuilder.Append((base.ClientSecurityContext == null) ? "null" : base.ClientSecurityContext.ToString());
						stringBuilder.Append("] mdbGuid:[");
						stringBuilder.Append(base.MdbGuid.Value);
						stringBuilder.Append("] mdbVersionGuid:[");
						stringBuilder.Append(base.MdbVersionGuid);
						stringBuilder.Append("] mailboxGuid:[");
						stringBuilder.Append(this.mailboxDsGuid);
						stringBuilder.Append("]");
						ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					EventHistory eventHistory = EventHistory.GetEventHistory(base.Database);
					errorCode = eventHistory.DeleteWatermarksForMailbox(context, this.mailboxDsGuid, out this.deletedCount);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)65365U);
					}
					else if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder2 = new StringBuilder(100);
						stringBuilder2.Append("OUTPUT  DeleteMdbWatermarksForMailbox:");
						stringBuilder2.Append(" mdbVersionGuid:[");
						stringBuilder2.Append(base.MdbVersionGuid);
						stringBuilder2.Append("] deletedCount:[");
						stringBuilder2.Append(this.deletedCount);
						stringBuilder2.Append("]");
						ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder2.ToString());
					}
				}
				return errorCode;
			}

			private Guid mailboxDsGuid;

			private uint deletedCount;
		}

		internal class AdminRpcSaveMdbWatermarks : AdminRpcServer.AdminRpcEventsBase
		{
			public AdminRpcSaveMdbWatermarks(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mdbVersionGuid, MDBEVENTWM[] wms, byte[] auxiliaryIn) : base(AdminMethod.EcSaveMdbWatermarks50, callerSecurityContext, mdbGuid, mdbVersionGuid, auxiliaryIn)
			{
				this.wms = wms;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = base.CheckMdbVersion();
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)57173U);
				}
				else
				{
					List<EventWatermark> list;
					errorCode = AdminRpcParseFormat.ParseMDBEVENTWMs(this.wms, out list);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)44885U);
					}
					else
					{
						if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder(100);
							stringBuilder.Append("INPUT  SaveMdbWatermarks:");
							stringBuilder.Append(" callerSecurityContext:[");
							stringBuilder.Append((base.ClientSecurityContext == null) ? "null" : base.ClientSecurityContext.ToString());
							stringBuilder.Append("] mdbGuid:[");
							stringBuilder.Append(base.MdbGuid.Value);
							stringBuilder.Append("] mdbVersionGuid:[");
							stringBuilder.Append(base.MdbVersionGuid);
							stringBuilder.Append("] watermarks.Count:[");
							stringBuilder.Append((list == null) ? 0 : list.Count);
							stringBuilder.Append("]");
							ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder.ToString());
						}
						EventHistory eventHistory = EventHistory.GetEventHistory(base.Database);
						errorCode = eventHistory.SaveWatermarks(context, list);
						if (errorCode != ErrorCode.NoError)
						{
							errorCode = errorCode.Propagate((LID)61269U);
						}
					}
				}
				return errorCode;
			}

			private MDBEVENTWM[] wms;
		}

		internal class AdminRpcGetMdbWatermarksForConsumer : AdminRpcServer.AdminRpcEventsBase
		{
			public AdminRpcGetMdbWatermarksForConsumer(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mdbVersionGuid, Guid? mailboxDsGuid, Guid consumerGuid, byte[] auxiliaryIn) : base(AdminMethod.EcGetMdbWatermarksForConsumer50, callerSecurityContext, mdbGuid, mdbVersionGuid, auxiliaryIn)
			{
				this.mailboxDsGuid = mailboxDsGuid;
				this.consumerGuid = consumerGuid;
			}

			public MDBEVENTWM[] Wms
			{
				get
				{
					return this.wms;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = base.CheckMdbVersion();
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)36693U);
				}
				else
				{
					if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("INPUT  GetMdbWatermarksForConsumer:");
						stringBuilder.Append(" callerSecurityContext:[");
						stringBuilder.Append((base.ClientSecurityContext == null) ? "null" : base.ClientSecurityContext.ToString());
						stringBuilder.Append("] mdbGuid:[");
						stringBuilder.Append(base.MdbGuid.Value);
						stringBuilder.Append("] mdbVersionGuid:[");
						stringBuilder.Append(base.MdbVersionGuid);
						stringBuilder.Append("] consumerGuid:[");
						stringBuilder.Append(this.consumerGuid);
						if (this.mailboxDsGuid != null)
						{
							stringBuilder.Append("] mailboxGuid:[");
							stringBuilder.Append(this.mailboxDsGuid);
						}
						stringBuilder.Append("]");
						ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					EventHistory eventHistory = EventHistory.GetEventHistory(base.Database);
					List<EventWatermark> list;
					errorCode = eventHistory.ReadWatermarksForConsumer(context, this.consumerGuid, this.mailboxDsGuid, out list);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)53077U);
					}
					else
					{
						if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder2 = new StringBuilder(100);
							stringBuilder2.Append("OUTPUT  GetMdbWatermarksForConsumer:");
							stringBuilder2.Append(" mdbVersionGuid:[");
							stringBuilder2.Append(base.MdbVersionGuid);
							stringBuilder2.Append("] watermarks.Count:[");
							stringBuilder2.Append((list == null) ? 0 : list.Count);
							stringBuilder2.Append("]");
							ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder2.ToString());
						}
						errorCode = AdminRpcParseFormat.FormatMDBEVENTWMs(list, out this.wms);
						if (errorCode != ErrorCode.NoError)
						{
							errorCode = errorCode.Propagate((LID)46933U);
						}
					}
				}
				return errorCode;
			}

			private MDBEVENTWM[] wms;

			private Guid? mailboxDsGuid;

			private Guid consumerGuid;
		}

		internal class AdminRpcGetMdbWatermarksForMailbox : AdminRpcServer.AdminRpcEventsBase
		{
			public AdminRpcGetMdbWatermarksForMailbox(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mdbVersionGuid, Guid mailboxDsGuid, byte[] auxiliaryIn) : base(AdminMethod.EcGetMdbWatermarksForMailbox50, callerSecurityContext, mdbGuid, mdbVersionGuid, auxiliaryIn)
			{
				this.mailboxDsGuid = mailboxDsGuid;
			}

			public MDBEVENTWM[] Wms
			{
				get
				{
					return this.wms;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = base.CheckMdbVersion();
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)63317U);
				}
				else
				{
					if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("INPUT  GetMdbWatermarksForMailbox:");
						stringBuilder.Append(" callerSecurityContext:[");
						stringBuilder.Append((base.ClientSecurityContext == null) ? "null" : base.ClientSecurityContext.ToString());
						stringBuilder.Append("] mdbGuid:[");
						stringBuilder.Append(base.MdbGuid.Value);
						stringBuilder.Append("] mdbVersionGuid:[");
						stringBuilder.Append(base.MdbVersionGuid);
						stringBuilder.Append("] mailboxGuid:[");
						stringBuilder.Append(this.mailboxDsGuid);
						stringBuilder.Append("]");
						ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					EventHistory eventHistory = EventHistory.GetEventHistory(base.Database);
					List<EventWatermark> list;
					errorCode = eventHistory.ReadWatermarksForMailbox(context, this.mailboxDsGuid, out list);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)38741U);
					}
					else
					{
						if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder2 = new StringBuilder(100);
							stringBuilder2.Append("OUTPUT  GetMdbWatermarksForMailbox:");
							stringBuilder2.Append(" mdbVersionGuid:[");
							stringBuilder2.Append(base.MdbVersionGuid);
							stringBuilder2.Append("] watermarks.Count:[");
							stringBuilder2.Append((list == null) ? 0 : list.Count);
							stringBuilder2.Append("]");
							ExTraceGlobals.AdminRpcTracer.TraceDebug(0L, stringBuilder2.ToString());
						}
						errorCode = AdminRpcParseFormat.FormatMDBEVENTWMs(list, out this.wms);
						if (errorCode != ErrorCode.NoError)
						{
							errorCode = errorCode.Propagate((LID)55125U);
						}
					}
				}
				return errorCode;
			}

			private MDBEVENTWM[] wms;

			private Guid mailboxDsGuid;
		}

		internal abstract class MailboxAdminRpc : AdminRpc
		{
			internal Guid MailboxGuid
			{
				get
				{
					return this.mailboxGuid;
				}
			}

			internal MailboxAdminRpc(AdminMethod methodId, ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, bool includeSoftDeleted, byte[] auxiliaryIn) : base(methodId, callerSecurityContext, new Guid?(mdbGuid), auxiliaryIn)
			{
				this.mailboxGuid = mailboxGuid;
				this.includeSoftDeleted = includeSoftDeleted;
			}

			protected override ErrorCode EcInitializeResources(MapiContext context)
			{
				ErrorCode first = base.EcInitializeResources(context);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)17512U);
				}
				((AdminExecutionDiagnostics)context.Diagnostics).AdminExMonLogger.SetMailboxGuid(this.MailboxGuid);
				return ErrorCode.NoError;
			}

			protected override void CleanupResources(MapiContext context)
			{
				base.CleanupResources(context);
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				bool commit = false;
				try
				{
					context.InitializeMailboxExclusiveOperation(this.MailboxGuid, ExecutionDiagnostics.OperationSource.AdminRpc, MapiContext.MailboxLockTimeout);
					ErrorCode errorCode = context.StartMailboxOperation(MailboxCreation.DontAllow, false, true);
					if (errorCode != ErrorCode.NoError)
					{
						if (errorCode == ErrorCodeValue.NotFound)
						{
							return ErrorCode.CreateUnknownMailbox((LID)63960U);
						}
						return errorCode.Propagate((LID)35288U);
					}
					else
					{
						if (!this.includeSoftDeleted && context.LockedMailboxState.IsSoftDeleted)
						{
							return ErrorCode.CreateUnknownMailbox((LID)17520U);
						}
						errorCode = this.EcExecuteMailboxRpcOperation(context);
						if (errorCode != ErrorCode.NoError)
						{
							return errorCode.Propagate((LID)39164U);
						}
						commit = true;
					}
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(commit);
					}
				}
				return ErrorCode.NoError;
			}

			protected abstract ErrorCode EcExecuteMailboxRpcOperation(MapiContext context);

			private readonly Guid mailboxGuid;

			private readonly bool includeSoftDeleted;
		}

		internal class AdminRpcClearAbsentInDsOnMailbox : AdminRpcServer.MailboxAdminRpc
		{
			internal AdminRpcClearAbsentInDsOnMailbox(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] auxiliaryIn) : base(AdminMethod.EcClearAbsentInDsFlagOnMailbox50, callerSecurityContext, mdbGuid, mailboxGuid, true, auxiliaryIn)
			{
			}

			protected override ErrorCode EcExecuteMailboxRpcOperation(MapiContext context)
			{
				if (base.Database.IsRecovery)
				{
					return ErrorCode.CreateInvalidParameter((LID)39624U);
				}
				if (context.LockedMailboxState.IsSoftDeleted)
				{
					return ErrorCode.CreateInvalidParameter((LID)54136U);
				}
				MailboxInfo directoryMailboxInfo = null;
				TenantHint tenantHint = context.LockedMailboxState.TenantHint;
				ErrorCode first = context.PulseMailboxOperation(delegate()
				{
					directoryMailboxInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetMailboxInfo(context, tenantHint, this.MailboxGuid, GetMailboxInfoFlags.None);
				});
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)55548U);
				}
				if (directoryMailboxInfo == null)
				{
					return ErrorCode.CreateNotFound((LID)43260U);
				}
				using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
				{
					Microsoft.Exchange.Server.Storage.MapiDisp.MailboxCleanup.ReconnectMailboxToADObject(context, mailbox, directoryMailboxInfo);
				}
				return ErrorCode.NoError;
			}
		}

		internal class AdminRpcPurgeCachedMailboxObject : AdminRpc
		{
			internal AdminRpcPurgeCachedMailboxObject(ClientSecurityContext callerSecurityContext, Guid mailboxGuid, byte[] auxiliaryIn) : base(AdminMethod.EcPurgeCachedMailboxObject50, callerSecurityContext, auxiliaryIn)
			{
				this.mailboxGuid = mailboxGuid;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.RefreshMailboxInfo(context, this.mailboxGuid);
				return ErrorCode.NoError;
			}

			private readonly Guid mailboxGuid;
		}

		internal class AdminRpcDeletePrivateMailbox : AdminRpcServer.MailboxAdminRpc
		{
			private bool IsMailboxMoved
			{
				get
				{
					return (this.flags & DeleteMailboxFlags.MailboxMoved) == DeleteMailboxFlags.MailboxMoved;
				}
			}

			private bool IsHardDelete
			{
				get
				{
					return (this.flags & DeleteMailboxFlags.HardDelete) == DeleteMailboxFlags.HardDelete;
				}
			}

			private bool IsMoveFailed
			{
				get
				{
					return (this.flags & DeleteMailboxFlags.MoveFailed) == DeleteMailboxFlags.MoveFailed;
				}
			}

			private bool IsSoftDelete
			{
				get
				{
					return (this.flags & DeleteMailboxFlags.SoftDelete) == DeleteMailboxFlags.SoftDelete;
				}
			}

			private bool IsRemoveSoftDeleted
			{
				get
				{
					return (this.flags & DeleteMailboxFlags.RemoveSoftDeleted) == DeleteMailboxFlags.RemoveSoftDeleted;
				}
			}

			internal AdminRpcDeletePrivateMailbox(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, DeleteMailboxFlags flags, byte[] auxiliaryIn) : base(AdminMethod.EcAdminDeletePrivateMailbox50, callerSecurityContext, mdbGuid, mailboxGuid, true, auxiliaryIn)
			{
				this.flags = flags;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				if ((this.flags & ~(DeleteMailboxFlags.MailboxMoved | DeleteMailboxFlags.HardDelete | DeleteMailboxFlags.MoveFailed | DeleteMailboxFlags.SoftDelete | DeleteMailboxFlags.RemoveSoftDeleted)) != DeleteMailboxFlags.None)
				{
					return ErrorCode.CreateInvalidParameter((LID)41848U);
				}
				if ((this.IsHardDelete && this.IsSoftDelete) || (!this.IsHardDelete && !this.IsSoftDelete))
				{
					return ErrorCode.CreateInvalidParameter((LID)60056U);
				}
				if (this.IsSoftDelete && this.IsRemoveSoftDeleted)
				{
					return ErrorCode.CreateInvalidParameter((LID)17584U);
				}
				if (!this.IsMailboxMoved && this.IsMoveFailed)
				{
					return ErrorCode.CreateInvalidParameter((LID)33656U);
				}
				if (this.IsSoftDelete && this.IsMoveFailed)
				{
					return ErrorCode.CreateInvalidParameter((LID)58232U);
				}
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcInitializeResources(MapiContext context)
			{
				if (base.MdbGuid == null)
				{
					return ErrorCode.CreateInvalidParameter((LID)49744U);
				}
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.DeregisterAllSessionssOfMailbox(context, base.MdbGuid.Value, base.MailboxGuid);
				ErrorCode errorCode = base.EcInitializeResources(context);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)17600U);
				}
				return errorCode;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				if (DefaultSettings.Get.UserInformationIsEnabled && UserInfoUpgrader.IsReady(context, context.Database) && context.Database.PhysicalDatabase.DatabaseType != DatabaseType.Sql && this.IsSoftDelete)
				{
					UserInformation.TryMarkAsSoftDeleted(context, base.MailboxGuid);
				}
				return base.EcExecuteRpc(context);
			}

			protected override ErrorCode EcExecuteMailboxRpcOperation(MapiContext context)
			{
				ErrorCode result = ErrorCode.NoError;
				MailboxNotificationEvent mailboxNotificationEvent = null;
				if (!context.LockedMailboxState.IsAccessible)
				{
					result = ErrorCode.CreateUnexpectedMailboxState((LID)50040U);
				}
				else if (!context.LockedMailboxState.IsSoftDeleted && this.IsRemoveSoftDeleted)
				{
					result = ErrorCode.CreateUnexpectedMailboxState((LID)48248U);
				}
				else if (context.LockedMailboxState.IsSoftDeleted && !this.IsRemoveSoftDeleted)
				{
					result = ErrorCode.CreateUnexpectedMailboxState((LID)64632U);
				}
				else if (context.LockedMailboxState.IsSoftDeleted && this.IsSoftDelete)
				{
					result = ErrorCode.CreateUnexpectedMailboxState((LID)40056U);
				}
				else
				{
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
					{
						if (this.IsMailboxMoved)
						{
							if (this.IsMoveFailed)
							{
								mailboxNotificationEvent = NotificationEvents.CreateMailboxMoveFailedNotificationEvent(context, mailbox, false);
							}
							else
							{
								MailboxQuarantineProvider.Instance.UnquarantineMailbox(base.MdbGuid.Value, base.MailboxGuid);
								mailboxNotificationEvent = NotificationEvents.CreateMailboxMoveSucceededNotificationEvent(context, mailbox, true);
							}
						}
						else if (this.IsHardDelete)
						{
							mailboxNotificationEvent = NotificationEvents.CreateMailboxDeletedNotificationEvent(context, mailbox);
						}
						if (this.IsSoftDelete)
						{
							mailbox.SoftDelete(context);
						}
						else if (this.IsHardDelete)
						{
							mailbox.HardDelete(context);
						}
						if (mailboxNotificationEvent != null)
						{
							context.RiseNotificationEvent(mailboxNotificationEvent);
						}
					}
				}
				return result;
			}

			private DeleteMailboxFlags flags;
		}

		internal class AdminRpcGetMailboxSecurityDescriptor : AdminRpcServer.MailboxAdminRpc
		{
			internal AdminRpcGetMailboxSecurityDescriptor(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] auxiliaryIn) : base(AdminMethod.EcGetMailboxSecurityDescriptor50, callerSecurityContext, mdbGuid, mailboxGuid, false, auxiliaryIn)
			{
			}

			public byte[] NTSecurityDescriptor
			{
				get
				{
					return this.ntSecurityDescriptor;
				}
			}

			protected override ErrorCode EcExecuteMailboxRpcOperation(MapiContext context)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				MailboxInfo mailboxInfo = null;
				SecurityDescriptor databaseOrServerADSecurityDescriptor = null;
				TenantHint tenantHint = context.LockedMailboxState.TenantHint;
				errorCode = context.PulseMailboxOperation(delegate()
				{
					Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.RefreshMailboxInfo(context, this.MailboxGuid);
					mailboxInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetMailboxInfo(context, tenantHint, this.MailboxGuid, GetMailboxInfoFlags.None);
					if (mailboxInfo == null)
					{
						if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							ExTraceGlobals.AdminRpcTracer.TraceError<Guid>(0L, "mailbox info for mailbox {0} was not found", this.MailboxGuid);
						}
						throw new StoreException((LID)41624U, ErrorCodeValue.NotFound);
					}
					if (mailboxInfo.IsSystemAttendantRecipient)
					{
						databaseOrServerADSecurityDescriptor = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(context).NTSecurityDescriptor;
						return;
					}
					DatabaseInfo databaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(context, mailboxInfo.MdbGuid);
					if (databaseInfo == null)
					{
						if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							ExTraceGlobals.AdminRpcTracer.TraceError<Guid>(0L, "database info for database {0} was not found", mailboxInfo.MdbGuid);
						}
						throw new StoreException((LID)58008U, ErrorCodeValue.NotFound);
					}
					databaseOrServerADSecurityDescriptor = databaseInfo.NTSecurityDescriptor;
				});
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.AdminRpcTracer.TraceError<Guid, ErrorCode>(0L, "PulseMailboxOperation for mailbox {0} failed with {1}", base.MailboxGuid, errorCode);
					}
					return errorCode.Propagate((LID)59644U);
				}
				SecurityDescriptor exchangeSecurityDescriptor = mailboxInfo.ExchangeSecurityDescriptor;
				this.ntSecurityDescriptor = Mailbox.CreateMailboxSecurityDescriptorBlob(databaseOrServerADSecurityDescriptor, exchangeSecurityDescriptor);
				if (this.ntSecurityDescriptor == null)
				{
					if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.AdminRpcTracer.TraceError(0L, "computing mailbox SD has failed");
					}
					return ErrorCode.CreateCallFailed((LID)33432U);
				}
				return ErrorCode.NoError;
			}

			private byte[] ntSecurityDescriptor;
		}

		internal class AdminGetMailboxSignature : AdminRpcServer.MailboxAdminRpc
		{
			internal AdminGetMailboxSignature(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, MailboxSignatureFlags flags, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetMailboxSignature50, callerSecurityContext, mdbGuid, mailboxGuid, false, auxiliaryIn)
			{
				this.flags = flags;
			}

			public byte[] Result
			{
				get
				{
					return this.mailboxSignature;
				}
			}

			protected override ErrorCode EcExecuteMailboxRpcOperation(MapiContext context)
			{
				MailboxSignatureSectionType mailboxSignatureSectionType = MailboxSignatureSectionType.None;
				MailboxSignatureFlags mailboxSignatureFlags = this.flags;
				if ((mailboxSignatureFlags & MailboxSignatureFlags.GetMailboxShape) == MailboxSignatureFlags.GetMailboxShape)
				{
					mailboxSignatureSectionType |= MailboxSignatureSectionType.MailboxShape;
					mailboxSignatureFlags &= ~MailboxSignatureFlags.GetMailboxShape;
				}
				bool flag = false;
				if (MailboxSignatureFlags.GetLegacy == mailboxSignatureFlags)
				{
					mailboxSignatureSectionType |= (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.TenantHint);
					flag = true;
				}
				else if (MailboxSignatureFlags.GetMailboxSignature == mailboxSignatureFlags)
				{
					mailboxSignatureSectionType |= (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata | MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping | MailboxSignatureSectionType.TenantHint);
					flag = true;
				}
				else if ((MailboxSignatureFlags.GetNamedPropertyMapping | MailboxSignatureFlags.GetReplidGuidMapping) == mailboxSignatureFlags)
				{
					mailboxSignatureSectionType |= (MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping);
					flag = true;
				}
				else if (MailboxSignatureFlags.GetMappingMetadata == mailboxSignatureFlags)
				{
					mailboxSignatureSectionType |= MailboxSignatureSectionType.MappingMetadata;
				}
				uint num;
				bool assertCondition;
				if (flag && Mailbox.GetMailboxTypeVersion(context, context.LockedMailboxState.MailboxType, context.LockedMailboxState.MailboxTypeDetail, out num, out assertCondition))
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "Mailbox exists and isAllowed==false?");
					mailboxSignatureSectionType |= MailboxSignatureSectionType.MailboxTypeVersion;
				}
				if (UserInfoUpgrader.IsReady(context, context.Database) && DefaultSettings.Get.UserInformationIsEnabled && context.Database.PhysicalDatabase.DatabaseType != DatabaseType.Sql)
				{
					mailboxSignatureSectionType |= MailboxSignatureSectionType.UserInformation;
				}
				using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
				{
					MailboxSignature.Serialize(context, mailbox, mailboxSignatureSectionType, out this.mailboxSignature);
				}
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode first = base.EcValidateArguments(context);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)36444U);
				}
				MailboxSignatureFlags mailboxSignatureFlags = this.flags;
				if ((mailboxSignatureFlags & MailboxSignatureFlags.GetMailboxShape) == MailboxSignatureFlags.GetMailboxShape)
				{
					mailboxSignatureFlags &= ~MailboxSignatureFlags.GetMailboxShape;
					if (mailboxSignatureFlags != MailboxSignatureFlags.GetLegacy && mailboxSignatureFlags != MailboxSignatureFlags.GetMailboxSignature)
					{
						return ErrorCode.CreateInvalidParameter((LID)57436U);
					}
				}
				if (mailboxSignatureFlags != MailboxSignatureFlags.GetLegacy && mailboxSignatureFlags != MailboxSignatureFlags.GetMailboxSignature && mailboxSignatureFlags != MailboxSignatureFlags.GetMappingMetadata && mailboxSignatureFlags != (MailboxSignatureFlags.GetNamedPropertyMapping | MailboxSignatureFlags.GetReplidGuidMapping))
				{
					return ErrorCode.CreateInvalidParameter((LID)64584U);
				}
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcInitializeResources(MapiContext context)
			{
				ErrorCode errorCode = base.EcInitializeResources(context);
				if (errorCode == ErrorCodeValue.UnknownMailbox)
				{
					errorCode = ErrorCode.CreateNotFound((LID)48200U);
				}
				return errorCode;
			}

			private MailboxSignatureFlags flags;

			private byte[] mailboxSignature;
		}

		internal class AdminRpcSetMailboxBasicInfo : AdminRpcServer.MailboxAdminRpc
		{
			internal AdminRpcSetMailboxBasicInfo(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] mailboxSignature, byte[] auxiliaryIn) : base(AdminMethod.EcAdminSetMailboxBasicInfo50, callerSecurityContext, mdbGuid, mailboxGuid, true, auxiliaryIn)
			{
				this.mailboxSignature = mailboxSignature;
			}

			protected override ErrorCode EcExecuteMailboxRpcOperation(MapiContext context)
			{
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				Guid mailboxInstanceGuid;
				Guid defaultFoldersReplGuid;
				ExchangeId[] fidList;
				Guid mappingSignatureGuid;
				Guid localIdGuid;
				ulong nextIdCounter;
				uint? reservedIdCounterRange;
				ulong nextCnCounter;
				uint? reservedCnCounterRange;
				TenantHint tenantHint;
				Dictionary<ushort, StoreNamedPropInfo> numberToNameMap;
				Dictionary<ushort, Guid> replidGuidMap;
				PropertyBlob.BlobReader mailboxShapePropertyBlobReader;
				Mailbox.MailboxTypeVersion mailboxTypeVersion;
				PartitionInformation partitionInformation;
				PropertyBlob.BlobReader userInformationPropertyBlobReader;
				MailboxSignatureSectionType mailboxSignatureSectionType;
				MailboxSignature.Parse(this.mailboxSignature, base.MdbGuid.Value, base.MailboxGuid, out mailboxInstanceGuid, out defaultFoldersReplGuid, out fidList, out mappingSignatureGuid, out localIdGuid, out nextIdCounter, out reservedIdCounterRange, out nextCnCounter, out reservedCnCounterRange, out tenantHint, out numberToNameMap, out replidGuidMap, out mailboxShapePropertyBlobReader, out mailboxTypeVersion, out partitionInformation, out userInformationPropertyBlobReader, out mailboxSignatureSectionType);
				if ((short)(mailboxSignatureSectionType & MailboxSignatureSectionType.UserInformation) == 256)
				{
					errorCode = this.ProcessUserInformation(context, userInformationPropertyBlobReader);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode.Propagate((LID)49996U);
					}
				}
				mailboxSignatureSectionType &= ~(MailboxSignatureSectionType.MailboxTypeVersion | MailboxSignatureSectionType.PartitionInformation | MailboxSignatureSectionType.UserInformation);
				context.InitializeMailboxExclusiveOperation(base.MailboxGuid, ExecutionDiagnostics.OperationSource.AdminRpc, MapiContext.MailboxLockTimeout);
				errorCode = AdminRpcPermissionChecks.EcDefaultCheck(context, base.DatabaseInfo);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)50023U);
				}
				if ((short)(mailboxSignatureSectionType & (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.TenantHint)) == 17 || (short)(mailboxSignatureSectionType & (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata | MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping | MailboxSignatureSectionType.TenantHint)) == 31)
				{
					errorCode = this.CreateDestinationMailbox(context, fidList, tenantHint, mailboxInstanceGuid, localIdGuid, mappingSignatureGuid, nextIdCounter, reservedIdCounterRange, nextCnCounter, reservedCnCounterRange, numberToNameMap, replidGuidMap, defaultFoldersReplGuid, (short)(mailboxSignatureSectionType & (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata | MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping | MailboxSignatureSectionType.TenantHint)) == 31, mailboxTypeVersion, partitionInformation);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode.Propagate((LID)32860U);
					}
					errorCode = this.CheckMailboxVersionAndUpgrade(context);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode.Propagate((LID)54876U);
					}
					if ((short)(mailboxSignatureSectionType & MailboxSignatureSectionType.MailboxShape) == 32 && mailboxShapePropertyBlobReader.PropertyCount > 0)
					{
						errorCode = this.ProcessMailboxShape(context, mailboxShapePropertyBlobReader);
						if (errorCode != ErrorCode.NoError)
						{
							return errorCode.Propagate((LID)49244U);
						}
					}
				}
				else if ((short)(mailboxSignatureSectionType & (MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping)) == 12)
				{
					errorCode = this.ProcessMailboxSignatureMappings(context, numberToNameMap, replidGuidMap, mailboxTypeVersion);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode.Propagate((LID)48732U);
					}
				}
				else if ((short)(mailboxSignatureSectionType & MailboxSignatureSectionType.MappingMetadata) == 2)
				{
					errorCode = this.ProcessMailboxSignatureMappingMetadata(context, localIdGuid, mappingSignatureGuid, nextIdCounter, reservedIdCounterRange.Value, nextCnCounter, reservedCnCounterRange.Value);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode.Propagate((LID)65116U);
					}
				}
				return errorCode;
			}

			private ErrorCode CreateDestinationMailbox(MapiContext context, ExchangeId[] fidList, TenantHint tenantHint, Guid mailboxInstanceGuid, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint? reservedIdCounterRange, ulong nextCnCounter, uint? reservedCnCounterRange, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Guid defaultFoldersReplGuid, bool preserveMailboxSignature, Mailbox.MailboxTypeVersion mailboxTypeVersion, PartitionInformation partitionInformation)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				MailboxInfo mailboxInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetMailboxInfo(context, tenantHint, base.MailboxGuid, GetMailboxInfoFlags.IgnoreHomeMdb);
				AddressInfo addressInfoByMailboxGuid = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByMailboxGuid(context, tenantHint, base.MailboxGuid, GetAddressInfoFlags.None);
				DatabaseInfo databaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(context, base.MdbGuid.Value);
				if (!preserveMailboxSignature)
				{
					localIdGuid = Guid.NewGuid();
					mappingSignatureGuid = Guid.NewGuid();
					nextIdCounter = Mailbox.GetFirstAvailableIdGlobcount(mailboxInfo);
					nextCnCounter = 1UL;
					reservedIdCounterRange = null;
					reservedCnCounterRange = null;
					numberToNameMap = null;
					replidGuidMap = null;
				}
				bool flag = false;
				MailboxCreation mailboxCreation;
				if (partitionInformation != null)
				{
					flag = ((partitionInformation.Flags & PartitionInformation.ControlFlags.CreateNewPartition) == PartitionInformation.ControlFlags.CreateNewPartition);
					mailboxCreation = MailboxCreation.Allow(new Guid?(partitionInformation.PartitionGuid), flag);
				}
				else
				{
					mailboxCreation = MailboxCreation.Allow(null);
				}
				errorCode = context.StartMailboxOperation(mailboxCreation, true, false);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)58428U);
				}
				bool commit = false;
				try
				{
					Mailbox.ValidateMailboxTypeVersion(context, mailboxInfo.Type, mailboxInfo.TypeDetail, mailboxTypeVersion);
					if (flag)
					{
						while (!context.LockedMailboxState.IsNewMailboxPartition)
						{
							Guid newUnifiedMailboxGuid = Guid.NewGuid();
							try
							{
								context.LockedMailboxState.AddReference();
								Mailbox.MakeRoomForNewPartition(context, context.LockedMailboxState.UnifiedState.UnifiedMailboxGuid, newUnifiedMailboxGuid);
							}
							finally
							{
								context.LockedMailboxState.ReleaseReference();
							}
							context.Commit();
							MailboxState mailboxState = MailboxStateCache.Get(context, context.LockedMailboxState.MailboxNumber);
							MailboxStateCache.MakeRoomForNewPartition(context, mailboxState, newUnifiedMailboxGuid);
							errorCode = context.PulseMailboxOperation(mailboxCreation, true, null);
							if (errorCode != ErrorCode.NoError)
							{
								return errorCode.Propagate((LID)54524U);
							}
						}
					}
					while (!context.LockedMailboxState.IsNew)
					{
						using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
						{
							mailbox.MakeRoomForNewMailbox(context);
							context.Commit();
							MailboxState mailboxState2 = MailboxStateCache.Get(context, context.LockedMailboxState.MailboxNumber);
							MailboxStateCache.MakeRoomForNewMailbox(mailboxState2);
						}
						errorCode = context.PulseMailboxOperation(mailboxCreation, true, null);
						if (errorCode != ErrorCode.NoError)
						{
							return errorCode.Propagate((LID)59864U);
						}
					}
					context.LockedMailboxState.MailboxInstanceGuid = mailboxInstanceGuid;
					context.LockedMailboxState.TenantHint = tenantHint;
					if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MailboxSignatureTracer.TraceDebug(58440L, "Database {0} : Mailbox {1} : {2} : Create the destination mailbox preserving mailbox signature: {3}.", new object[]
						{
							context.Database.MdbName,
							context.LockedMailboxState.MailboxNumber,
							mailboxInfo.OwnerDisplayName,
							preserveMailboxSignature
						});
					}
					CrucialFolderId fidc = MapiMailbox.GetFIDC(context, fidList, mailboxInfo.Type != MailboxInfo.MailboxType.Private);
					using (context.GrantMailboxFullRights())
					{
						MapiMailbox.CreateMailbox(context, context.LockedMailboxState, addressInfoByMailboxGuid, mailboxInfo, databaseInfo, tenantHint, mailboxInstanceGuid, localIdGuid, mappingSignatureGuid, nextIdCounter, reservedIdCounterRange, nextCnCounter, reservedCnCounterRange, fidc, numberToNameMap, replidGuidMap, defaultFoldersReplGuid, true);
					}
					context.LockedMailboxState.AddReference();
					try
					{
						InTransitInfo.SetInTransitState(context.LockedMailboxState, InTransitStatus.DestinationOfMove | InTransitStatus.OnlineMove, null);
						LogicalIndexCache.DiscardCacheForMailbox(context, context.LockedMailboxState);
					}
					finally
					{
						context.LockedMailboxState.ReleaseReference();
					}
					commit = true;
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						MailboxState capturedMailboxState = context.LockedMailboxState;
						context.RegisterStateAction(null, delegate(Context ctx)
						{
							MailboxStateCache.DeleteMailboxState(ctx, capturedMailboxState);
						});
						context.EndMailboxOperation(commit);
					}
				}
				return errorCode;
			}

			private ErrorCode CheckMailboxVersionAndUpgrade(MapiContext context)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				errorCode = context.StartMailboxOperation();
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)42588U);
				}
				bool commit = false;
				try
				{
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
					{
						mailbox.CheckMailboxVersionAndUpgrade(context);
						mailbox.Save(context);
					}
					commit = true;
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(commit);
					}
				}
				return errorCode;
			}

			private ErrorCode ProcessMailboxShape(MapiContext context, PropertyBlob.BlobReader mailboxShapePropertyBlobReader)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				errorCode = context.StartMailboxOperation();
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)35900U);
				}
				bool commit = false;
				try
				{
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
					{
						errorCode = mailbox.SetMailboxShape(context, mailboxShapePropertyBlobReader);
						if (errorCode != ErrorCode.NoError)
						{
							return errorCode.Propagate((LID)40540U);
						}
						mailbox.Save(context);
					}
					commit = true;
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(commit);
					}
				}
				return errorCode;
			}

			private ErrorCode ProcessMailboxSignatureMappings(MapiContext context, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Mailbox.MailboxTypeVersion mailboxTypeVersion)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				errorCode = context.StartMailboxOperation();
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)35528U);
				}
				bool commit = false;
				try
				{
					Mailbox.ValidateMailboxTypeVersion(context, context.LockedMailboxState.MailboxType, context.LockedMailboxState.MailboxTypeDetail, mailboxTypeVersion);
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
					{
						if (!mailbox.GetMRSPreservingMailboxSignature(context))
						{
							return ErrorCode.CreateCorruptData((LID)46240U);
						}
						if (!mailbox.GetPreservingMailboxSignature(context))
						{
							return ErrorCode.CreateCorruptData((LID)54332U);
						}
						mailbox.NamedPropertyMap.Process(context, numberToNameMap);
						mailbox.ReplidGuidMap.Process(context, replidGuidMap);
					}
					commit = true;
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(commit);
					}
				}
				return errorCode;
			}

			private ErrorCode ProcessMailboxSignatureMappingMetadata(MapiContext context, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint reservedIdCounterRange, ulong nextCnCounter, uint reservedCnCounterRange)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				errorCode = context.StartMailboxOperation();
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)59616U);
				}
				bool commit = false;
				checked
				{
					try
					{
						using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
						{
							if (!mailbox.GetMRSPreservingMailboxSignature(context))
							{
								if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.ErrorTrace))
								{
									ExTraceGlobals.MailboxSignatureTracer.TraceError<string, Guid, string>(10L, "The mailbox {0} with Guid {1}, on database {2} is not in MRS mailbox signature preserving mode", mailbox.GetDisplayName(context), mailbox.MailboxGuid, context.Database.MdbName);
								}
								return ErrorCode.CreateCorruptData((LID)52284U);
							}
							if (mailbox.GetPreservingMailboxSignature(context))
							{
								if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.ErrorTrace))
								{
									ExTraceGlobals.MailboxSignatureTracer.TraceError<string, Guid, string>(35040L, "The mailbox {0} with Guid {1}, on database {2} is still in mailbox signature preserving mode", mailbox.GetDisplayName(context), mailbox.MailboxGuid, context.Database.MdbName);
								}
								return ErrorCode.CreateCorruptData((LID)51424U);
							}
							if (!mappingSignatureGuid.Equals(mailbox.GetMappingSignatureGuid(context)))
							{
								if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.ErrorTrace))
								{
									ExTraceGlobals.MailboxSignatureTracer.TraceError(45280L, "The mailbox {0} with Guid {1}, on database {2} has mapping signature Guid {3} != {4}.", new object[]
									{
										mailbox.GetDisplayName(context),
										mailbox.MailboxGuid,
										context.Database.MdbName,
										mailbox.GetMappingSignatureGuid(context),
										mappingSignatureGuid
									});
								}
								return ErrorCode.CreateCorruptData((LID)61664U);
							}
							if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ulong num;
								ulong num2;
								mailbox.GetGlobalCounters(context, out num, out num2);
								ExTraceGlobals.MailboxSignatureTracer.TraceDebug(53472L, "The mailbox {0} with Guid {1}, has current Id: {2}, Cn: {3}, next Id: {4}, Cn: {5}.", new object[]
								{
									mailbox.GetDisplayName(context),
									mailbox.MailboxGuid,
									num,
									num2,
									nextIdCounter,
									nextCnCounter
								});
							}
							mailbox.VerifyAndUpdateGlobalCounters(context, localIdGuid, nextIdCounter, nextCnCounter);
							ulong num3 = nextIdCounter + unchecked((ulong)reservedIdCounterRange);
							mailbox.SetProperty(context, PropTag.Mailbox.ReservedIdCounterRangeUpperLimit, (long)num3);
							ulong num4 = nextCnCounter + unchecked((ulong)reservedCnCounterRange);
							mailbox.SetProperty(context, PropTag.Mailbox.ReservedCnCounterRangeUpperLimit, (long)num4);
							if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.MailboxSignatureTracer.TraceDebug(41184L, "The mailbox {0} with Guid {1}, reserver counter upper limit Id: {2}, Cn: {3}", new object[]
								{
									mailbox.GetDisplayName(context),
									mailbox.MailboxGuid,
									num3,
									num4
								});
							}
							mailbox.SetPreservingMailboxSignature(context, true);
							mailbox.Save(context);
						}
						commit = true;
					}
					finally
					{
						if (context.IsMailboxOperationStarted)
						{
							context.EndMailboxOperation(commit);
						}
					}
					return errorCode;
				}
			}

			private ErrorCode ProcessUserInformation(MapiContext context, PropertyBlob.BlobReader userInformationPropertyBlobReader)
			{
				if (!DefaultSettings.Get.UserInformationIsEnabled)
				{
					return ErrorCode.CreateNotSupported((LID)51788U);
				}
				if (!UserInfoUpgrader.IsReady(context, context.Database))
				{
					return ErrorCode.CreateNotSupported((LID)62540U);
				}
				if (userInformationPropertyBlobReader.PropertyCount == 0)
				{
					return ErrorCode.NoError;
				}
				List<uint> list = new List<uint>(userInformationPropertyBlobReader.PropertyCount);
				List<object> list2 = new List<object>(userInformationPropertyBlobReader.PropertyCount);
				for (int i = 0; i < userInformationPropertyBlobReader.PropertyCount; i++)
				{
					list.Add(userInformationPropertyBlobReader.GetPropertyTag(i));
					list2.Add(userInformationPropertyBlobReader.GetPropertyValue(i));
				}
				Guid? guid = null;
				Properties initialProperties = new Properties(list.Count);
				for (int j = 0; j < list.Count; j++)
				{
					StorePropTag storePropTag = LegacyHelper.ConvertFromLegacyPropTag(list[j], Microsoft.Exchange.Server.Storage.PropTags.ObjectType.UserInfo, null, false);
					if (storePropTag == PropTag.UserInfo.UserInformationGuid)
					{
						guid = (Guid?)list2[j];
					}
					else if (storePropTag.PropInfo == null || !storePropTag.PropInfo.IsCategory(3) || storePropTag.PropInfo.IsCategory(4))
					{
						initialProperties.Add(storePropTag, list2[j]);
					}
				}
				if (guid == null)
				{
					return ErrorCode.CreateCorruptData((LID)43084U);
				}
				UserInformation.Create(context, guid.Value, initialProperties, true);
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode first = base.EcValidateArguments(context);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)52828U);
				}
				MailboxSignatureSectionType mailboxSignatureSectionType;
				MailboxSignature.Parse(this.mailboxSignature, out mailboxSignatureSectionType);
				mailboxSignatureSectionType &= ~(MailboxSignatureSectionType.MailboxTypeVersion | MailboxSignatureSectionType.PartitionInformation | MailboxSignatureSectionType.UserInformation);
				if ((short)(mailboxSignatureSectionType & MailboxSignatureSectionType.MailboxShape) == 32)
				{
					mailboxSignatureSectionType &= ~MailboxSignatureSectionType.MailboxShape;
					if (mailboxSignatureSectionType != (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata | MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping | MailboxSignatureSectionType.TenantHint) && mailboxSignatureSectionType != (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.TenantHint))
					{
						return ErrorCode.CreateCorruptData((LID)56924U);
					}
				}
				if (mailboxSignatureSectionType != (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata | MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping | MailboxSignatureSectionType.TenantHint) && mailboxSignatureSectionType != (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.TenantHint) && mailboxSignatureSectionType != (MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping) && mailboxSignatureSectionType != MailboxSignatureSectionType.MappingMetadata)
				{
					return ErrorCode.CreateCorruptData((LID)17428U);
				}
				return ErrorCode.NoError;
			}

			private readonly byte[] mailboxSignature;
		}

		internal class AdminRpcGetMailboxTable : AdminRpc
		{
			internal AdminRpcGetMailboxTable(ClientSecurityContext callerSecurityContext, Guid? mdbGuid, int lparam, uint[] propTags, uint cpid, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetMailboxTable50, callerSecurityContext, mdbGuid, AdminRpc.ExpectedDatabaseState.AnyAttachedState, auxiliaryIn)
			{
				this.lparam = lparam;
				this.propTags = propTags;
			}

			internal AdminRpcGetMailboxTable(ClientSecurityContext callerSecurityContext, int lparam, uint[] propTags, uint cpid, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetMailboxTable50, callerSecurityContext, auxiliaryIn)
			{
				this.lparam = lparam;
				this.propTags = propTags;
			}

			public byte[] Result
			{
				get
				{
					return this.result;
				}
			}

			public uint RowCount
			{
				get
				{
					return this.rowCount;
				}
			}

			private static RequiredMaintenanceResourceType RequiredMaintenanceResourceTypeFromMailboxInfoTableFlags(GetMailboxInfoTableFlags mailboxInfoTableFlags)
			{
				switch (mailboxInfoTableFlags)
				{
				case GetMailboxInfoTableFlags.MaintenanceItems:
					return RequiredMaintenanceResourceType.Store;
				case GetMailboxInfoTableFlags.MaintenanceItemsWithDS:
					return RequiredMaintenanceResourceType.DirectoryServiceAndStore;
				case GetMailboxInfoTableFlags.UrgentMaintenanceItems:
					return RequiredMaintenanceResourceType.StoreUrgent;
				default:
					return RequiredMaintenanceResourceType.Store;
				}
			}

			private void GetMailboxTableInfoForDatabase(MapiContext context, StoreDatabase database)
			{
				int num = 0;
				List<Properties> list;
				try
				{
					IL_02:
					GetMailboxInfoTableFlags getMailboxInfoTableFlags = (GetMailboxInfoTableFlags)this.lparam;
					if (GetMailboxInfoTableFlags.MaintenanceItems == getMailboxInfoTableFlags || GetMailboxInfoTableFlags.MaintenanceItemsWithDS == getMailboxInfoTableFlags || GetMailboxInfoTableFlags.UrgentMaintenanceItems == getMailboxInfoTableFlags)
					{
						list = new List<Properties>();
						List<MaintenanceHandler.MaintenanceToSchedule> scheduledMaintenances = MaintenanceHandler.GetScheduledMaintenances(context, AdminRpcServer.AdminRpcGetMailboxTable.RequiredMaintenanceResourceTypeFromMailboxInfoTableFlags(getMailboxInfoTableFlags));
						using (List<MaintenanceHandler.MaintenanceToSchedule>.Enumerator enumerator = scheduledMaintenances.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								MaintenanceHandler.MaintenanceToSchedule maintenanceToSchedule = enumerator.Current;
								Properties item = new Properties(2);
								foreach (StorePropTag tag in this.storePropTags)
								{
									object value = Property.NotFoundError(tag);
									uint propTag = tag.PropTag;
									if (propTag != 1035665480U)
									{
										if (propTag != 1728512072U)
										{
											if (propTag == 1746862083U)
											{
												value = maintenanceToSchedule.MailboxNumber;
											}
										}
										else
										{
											value = maintenanceToSchedule.MailboxGuid;
										}
									}
									else
									{
										value = maintenanceToSchedule.MaintenanceId;
									}
									item.Add(tag, value);
								}
								list.Add(item);
							}
							goto IL_118;
						}
					}
					list = MapiMailbox.GetMailboxInfoTable(context, database, getMailboxInfoTableFlags, this.storePropTags);
					IL_118:;
				}
				catch (SqlException ex)
				{
					context.OnExceptionCatch(ex);
					if (ex.Number != 601)
					{
						throw;
					}
					if (++num >= 10)
					{
						throw new StoreException((LID)39352U, ErrorCodeValue.Busy, "Server is busy", ex);
					}
					if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.AdminRpcTracer.TraceError<SqlException, int>(47957, 0L, "Encountered exception {0}, retry: {1}", ex, num);
					}
					goto IL_02;
				}
				int num2 = 0;
				for (int j = 0; j < list.Count; j++)
				{
					LegacyHelper.MassageOutgoingProperties(list[j]);
					this.outputBufferSize += AdminRpcParseFormat.SetValues(null, ref num2, 0, list[j]);
				}
				if (this.tableRows == null)
				{
					this.tableRows = list;
					return;
				}
				this.tableRows.AddRange(list);
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode noError = ErrorCode.NoError;
				this.storePropTags = LegacyHelper.ConvertFromLegacyPropTags(this.propTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Mailbox, null, true);
				if (base.MdbGuid == null)
				{
					Storage.ForEachDatabase(context, delegate(Context executionContext, StoreDatabase database, Func<bool> shouldCallbackContinue)
					{
						this.GetMailboxTableInfoForDatabase((MapiContext)executionContext, database);
					});
				}
				else
				{
					if (base.Database == null)
					{
						return ErrorCode.CreateNotInitialized((LID)49789U);
					}
					this.GetMailboxTableInfoForDatabase(context, base.Database);
				}
				byte[] array = new byte[this.outputBufferSize];
				if (this.tableRows != null)
				{
					int num = 0;
					for (int i = 0; i < this.tableRows.Count; i++)
					{
						AdminRpcParseFormat.SetValues(array, ref num, array.Length, this.tableRows[i]);
					}
					this.rowCount = (uint)this.tableRows.Count;
				}
				else
				{
					this.rowCount = 0U;
				}
				this.result = array;
				return noError;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				if (this.propTags == null || this.propTags.Length == 0)
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)60925U);
				}
				else if (!EnumValidator.IsValidValue<GetMailboxInfoTableFlags>((GetMailboxInfoTableFlags)this.lparam))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)63416U);
				}
				return errorCode;
			}

			private const int AverageNumberOfMailboxes = 100;

			private byte[] result;

			private uint[] propTags;

			private uint rowCount;

			private int lparam;

			private StorePropTag[] storePropTags;

			private int outputBufferSize;

			private List<Properties> tableRows;
		}

		internal class AdminRpcSyncMailboxWithDS : AdminRpc
		{
			internal AdminRpcSyncMailboxWithDS(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] auxiliaryIn) : base(AdminMethod.EcSyncMailboxWithDS50, callerSecurityContext, new Guid?(mdbGuid), auxiliaryIn)
			{
				this.mailboxGuid = mailboxGuid;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				if (this.mailboxGuid == Guid.Empty)
				{
					return ErrorCode.CreateInvalidParameter((LID)57319U);
				}
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode result = ErrorCode.NoError;
				if (base.Database.IsRecovery)
				{
					result = ErrorCode.CreateInvalidParameter((LID)56008U);
				}
				else
				{
					Microsoft.Exchange.Server.Storage.MapiDisp.MailboxCleanup.SynchronizeDirectoryAndMailboxTable(context, this.mailboxGuid, true);
				}
				return result;
			}

			private Guid mailboxGuid;
		}

		internal class AdminGetMailboxTableEntry50 : AdminRpc
		{
			private Guid MailboxGuid
			{
				get
				{
					return this.mailboxGuid;
				}
				set
				{
					this.mailboxGuid = value;
				}
			}

			private GetMailboxInfoTableFlags Flags
			{
				get
				{
					return this.flags;
				}
				set
				{
					this.flags = value;
				}
			}

			internal AdminGetMailboxTableEntry50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, GetMailboxInfoTableFlags flags, uint[] propTags, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetMailboxTableEntry50, callerSecurityContext, new Guid?(mdbGuid), AdminRpc.ExpectedDatabaseState.AnyAttachedState, auxiliaryIn)
			{
				this.propTags = propTags;
				this.MailboxGuid = mailboxGuid;
				this.Flags = flags;
			}

			public byte[] Result
			{
				get
				{
					return this.result;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				StorePropTag[] requestedPropTags = LegacyHelper.ConvertFromLegacyPropTags(this.propTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Mailbox, null, true);
				this.TryUpdateTableSizeStatistics(context, requestedPropTags);
				Properties mailboxInfoTableEntry = MapiMailbox.GetMailboxInfoTableEntry(context, base.Database, this.MailboxGuid, this.Flags, requestedPropTags);
				int num = 0;
				int num2 = AdminRpcParseFormat.SetValues(null, ref num, 0, mailboxInfoTableEntry);
				num = 0;
				byte[] array = new byte[num2];
				AdminRpcParseFormat.SetValues(array, ref num, array.Length, mailboxInfoTableEntry);
				this.result = array;
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode first = base.EcValidateArguments(context);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)58240U);
				}
				if (this.propTags == null || this.propTags.Length == 0)
				{
					return ErrorCode.CreateInvalidParameter((LID)57519U);
				}
				return first;
			}

			protected override ErrorCode EcInitializeResources(MapiContext context)
			{
				ErrorCode errorCode = base.EcInitializeResources(context);
				if (errorCode == ErrorCodeValue.UnknownMailbox)
				{
					return ErrorCode.CreateNotFound((LID)48504U);
				}
				return errorCode;
			}

			private void TryUpdateTableSizeStatistics(MapiContext context, StorePropTag[] requestedPropTags)
			{
				bool flag = false;
				IList<Mailbox.TableSizeStatistics> tableSizeStatisticsDefinitions = Mailbox.TableSizeStatisticsDefinitions;
				for (int i = 0; i < requestedPropTags.Length; i++)
				{
					StorePropTag requestedPropTag = requestedPropTags[i];
					if (tableSizeStatisticsDefinitions.Any((Mailbox.TableSizeStatistics s) => s.TotalPagesProperty == requestedPropTag || s.AvailablePagesProperty == requestedPropTag))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return;
				}
				bool value = ConfigurationSchema.AggresiveUpdateMailboxTableSizeStatistics.Value;
				TimeSpan lockTimeout = value ? TimeSpan.FromMinutes(10.0) : TimeSpan.FromSeconds(5.0);
				context.InitializeMailboxExclusiveOperation(this.MailboxGuid, ExecutionDiagnostics.OperationSource.AdminRpc, lockTimeout);
				bool commit = false;
				try
				{
					ErrorCode errorCode = context.StartMailboxOperation(MailboxCreation.DontAllow, false, true);
					if (errorCode != ErrorCode.NoError)
					{
						if (value)
						{
							throw new StoreException((LID)49100U, errorCode);
						}
					}
					else if (context.LockedMailboxState.IsAccessible)
					{
						using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
						{
							mailbox.UpdateTableSizeStatistics(context);
							mailbox.Save(context);
						}
						commit = true;
					}
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(commit);
					}
				}
			}

			private Guid mailboxGuid;

			private GetMailboxInfoTableFlags flags;

			private byte[] result;

			private uint[] propTags;
		}

		internal class AdminGetViewsTableEx50 : AdminRpcServer.MailboxAdminRpc
		{
			internal AdminGetViewsTableEx50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, LTID folderLTID, uint[] propTags, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetViewsTableEx50, callerSecurityContext, mdbGuid, mailboxGuid, false, auxiliaryIn)
			{
				this.propTags = propTags;
				this.folderLTID = folderLTID;
			}

			public byte[] Result
			{
				get
				{
					return this.result;
				}
			}

			public uint ResultRowCount
			{
				get
				{
					return this.resultRowCount;
				}
			}

			protected override ErrorCode EcExecuteMailboxRpcOperation(MapiContext context)
			{
				IList<Properties> viewsProperties;
				using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
				{
					StorePropTag[] storePropTags = LegacyHelper.ConvertFromLegacyPropTags(this.propTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.ViewDefinition, null, true);
					ExchangeId folderId = ExchangeId.Create(context, mailbox.ReplidGuidMap, this.folderLTID.guid, this.folderLTID.globCount);
					viewsProperties = Folder.GetViewsProperties(context, mailbox, folderId, storePropTags);
				}
				int num = 0;
				int num2 = 0;
				foreach (Properties properties in viewsProperties)
				{
					LegacyHelper.MassageOutgoingProperties(properties);
					num2 += AdminRpcParseFormat.SetValues(null, ref num, 0, properties);
				}
				byte[] buff = new byte[num2];
				num = 0;
				foreach (Properties properties2 in viewsProperties)
				{
					num2 += AdminRpcParseFormat.SetValues(buff, ref num, num2, properties2);
				}
				this.result = buff;
				this.resultRowCount = (uint)viewsProperties.Count;
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode first = base.EcValidateArguments(context);
				if (!(first != ErrorCode.NoError))
				{
					if (this.propTags == null || this.propTags.Length == 0)
					{
						first = ErrorCode.CreateInvalidParameter((LID)42407U);
					}
					else if (this.folderLTID.guid == Guid.Empty || this.folderLTID.globCount == 0UL)
					{
						first = ErrorCode.CreateInvalidParameter((LID)58791U);
					}
				}
				return first;
			}

			private byte[] result;

			private uint[] propTags;

			private LTID folderLTID;

			private uint resultRowCount;
		}

		internal class AdminGetRestrictionTableEx50 : AdminRpcServer.MailboxAdminRpc
		{
			internal AdminGetRestrictionTableEx50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, LTID folderLTID, uint[] propTags, byte[] auxiliaryIn) : base(AdminMethod.EcAdminGetRestrictionTableEx50, callerSecurityContext, mdbGuid, mailboxGuid, false, auxiliaryIn)
			{
				this.propTags = propTags;
				this.folderLTID = folderLTID;
			}

			public byte[] Result
			{
				get
				{
					return this.result;
				}
			}

			public uint ResultRowCount
			{
				get
				{
					return this.resultRowCount;
				}
			}

			protected override ErrorCode EcExecuteMailboxRpcOperation(MapiContext context)
			{
				IList<Properties> restrictionsProperties;
				using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
				{
					StorePropTag[] storePropTags = LegacyHelper.ConvertFromLegacyPropTags(this.propTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.RestrictionView, null, true);
					ExchangeId exchangeId = ExchangeId.Create(context, mailbox.ReplidGuidMap, this.folderLTID.guid, this.folderLTID.globCount);
					ExchangeId materializedRestrictionRootForFolder = ((LogicalMailbox)mailbox).GetMaterializedRestrictionRootForFolder(context, exchangeId);
					restrictionsProperties = Folder.GetRestrictionsProperties(context, mailbox, exchangeId, materializedRestrictionRootForFolder, storePropTags);
				}
				int num = 0;
				int num2 = 0;
				foreach (Properties properties in restrictionsProperties)
				{
					LegacyHelper.MassageOutgoingProperties(properties);
					num2 += AdminRpcParseFormat.SetValues(null, ref num, 0, properties);
				}
				byte[] buff = new byte[num2];
				num = 0;
				foreach (Properties properties2 in restrictionsProperties)
				{
					num2 += AdminRpcParseFormat.SetValues(buff, ref num, num2, properties2);
				}
				this.result = buff;
				this.resultRowCount = (uint)restrictionsProperties.Count;
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode first = base.EcValidateArguments(context);
				if (!(first != ErrorCode.NoError))
				{
					if (this.propTags == null || this.propTags.Length == 0)
					{
						first = ErrorCode.CreateInvalidParameter((LID)65228U);
					}
					else if (this.folderLTID.guid == Guid.Empty || this.folderLTID.globCount == 0UL)
					{
						first = ErrorCode.CreateInvalidParameter((LID)63180U);
					}
				}
				return first;
			}

			private byte[] result;

			private uint[] propTags;

			private LTID folderLTID;

			private uint resultRowCount;
		}

		internal class AdminExecuteTask50 : AdminRpc
		{
			internal AdminExecuteTask50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid taskClass, int taskId, byte[] auxiliaryIn) : base(AdminMethod.EcAdminExecuteTask50, callerSecurityContext, new Guid?(mdbGuid), auxiliaryIn)
			{
				this.taskClass = taskClass;
				this.taskId = taskId;
			}

			internal override int OperationDetail
			{
				get
				{
					if (this.taskMapping.ContainsKey(this.taskClass))
					{
						return (int)(this.taskMapping[this.taskClass] + 4000U);
					}
					return base.OperationDetail;
				}
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode result = ErrorCode.NoError;
				if (base.Database.IsRecovery)
				{
					result = ErrorCode.CreateInvalidParameter((LID)43720U);
				}
				else if (this.taskId == 0)
				{
					MaintenanceHandler.DoDatabaseMaintenance(context, base.DatabaseInfo, this.taskClass);
				}
				else
				{
					MaintenanceHandler.DoMailboxMaintenance(context, this.taskClass, this.taskId);
				}
				return result;
			}

			private int taskId;

			private Guid taskClass;

			public static readonly Guid TestPurposesTaskId = Guid.Empty;

			private Dictionary<Guid, AdminRpcServer.AdminExecuteTask50.MaintenanceTask> taskMapping = new Dictionary<Guid, AdminRpcServer.AdminExecuteTask50.MaintenanceTask>
			{
				{
					new Guid("{8dda68d9-e1c4-4b97-a884-bf0ab208cf5c}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.MarkLogicalIndicesForCleanup
				},
				{
					new Guid("{f6f50b68-76c8-4b41-865f-e984022602ac}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.DeliveredToCleanupMaintenance
				},
				{
					new Guid("{9a0932ca-268a-4a60-b90e-fa9335a2f139}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.EventHistoryCleanupMaintenance
				},
				{
					new Guid("{128e9fa8-7013-42d8-a957-9bda9f288649}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.MarkHardDeletedMailboxesForCleanupMaintenance
				},
				{
					new Guid("{81650b69-0c92-488f-9de7-d2e41fca7efa}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.CleanupAndRemoveTombstoneMailboxes
				},
				{
					new Guid("{db82d4f5-00a5-4d65-96bc-45b81285f12f}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.MarkMailboxesForSearchFolderAgeOut
				},
				{
					new Guid("{94196d5c-e792-466d-8f8d-e72ae0dd780f}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.MaintenanceCleanupTombstoneTable
				},
				{
					new Guid("{ecb20c7e-2942-40bc-92b2-acdf8948ab1a}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.UrgentTombstoneTableCleanup
				},
				{
					new Guid("{285abee5-b82c-4849-ac49-beaf265f3b46}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.MarkExpiredDisabledMailboxesForSynchronizationWithDS
				},
				{
					new Guid("{82d947ed-87da-4389-b4fa-af51d947f16e}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.MarkIdleUserAccessibleMailboxesForSynchronizationWithDS
				},
				{
					new Guid("{818429a5-c7c8-4546-8cad-c71efaf3c219}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.CleanupLogicalIndexes
				},
				{
					new Guid("{f4946920-3356-4f2d-bfb0-e72f14af6f56}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.ApplyMaintenanceTable
				},
				{
					new Guid("{c9490642-e68b-4157-954e-540d81e0ed87}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.AgeOutMailboxSearchFolders
				},
				{
					new Guid("{05ad2280-b95c-4e3f-bc1c-baaa8fb97e55}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.CleanupAndRemoveHardDeletedMailbox
				},
				{
					new Guid("{8b5c9cf4-109d-46b2-a050-d509f4c58e03}"),
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.SynchronizeDirectoryAndMailboxTable
				},
				{
					AdminRpcServer.AdminExecuteTask50.TestPurposesTaskId,
					AdminRpcServer.AdminExecuteTask50.MaintenanceTask.ForTestPurposes
				}
			};

			internal enum MaintenanceTask : uint
			{
				ForTestPurposes,
				MarkLogicalIndicesForCleanup,
				DeliveredToCleanupMaintenance,
				EventHistoryCleanupMaintenance,
				MarkHardDeletedMailboxesForCleanupMaintenance,
				CleanupAndRemoveTombstoneMailboxes,
				MarkMailboxesForSearchFolderAgeOut,
				MaintenanceCleanupTombstoneTable,
				UrgentTombstoneTableCleanup,
				MarkExpiredDisabledMailboxesForSynchronizationWithDS,
				MarkIdleUserAccessibleMailboxesForSynchronizationWithDS,
				CleanupLogicalIndexes,
				ApplyMaintenanceTable,
				AgeOutMailboxSearchFolders,
				CleanupAndRemoveHardDeletedMailbox,
				SynchronizeDirectoryAndMailboxTable
			}
		}

		internal class AdminPrePopulateCacheEx50 : AdminRpcServer.MailboxAdminRpc
		{
			internal AdminPrePopulateCacheEx50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, byte[] partitionHint, string domainController, byte[] auxiliaryIn) : base(AdminMethod.EcAdminPrePopulateCacheEx50, callerSecurityContext, mdbGuid, mailboxGuid, false, auxiliaryIn)
			{
				this.domainController = domainController;
				this.partitionHint = partitionHint;
			}

			protected override ErrorCode EcExecuteMailboxRpcOperation(MapiContext context)
			{
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				TenantHint tenantHint = TenantHint.RootOrg;
				if (this.partitionHint != null)
				{
					tenantHint = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.ResolveTenantHint(context, this.partitionHint);
				}
				Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.PrePopulateCachesForMailbox(context, tenantHint, base.MailboxGuid, this.domainController);
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode errorCode = base.EcValidateArguments(context);
				if (!(errorCode != ErrorCode.NoError) && (this.domainController == null || this.domainController.Length == 0))
				{
					errorCode = ErrorCode.CreateInvalidParameter((LID)42800U);
				}
				return errorCode;
			}

			protected override ErrorCode EcInitializeResources(MapiContext context)
			{
				((AdminExecutionDiagnostics)context.Diagnostics).AdminExMonLogger.SetMdbGuid(base.MdbGuid.Value);
				((AdminExecutionDiagnostics)context.Diagnostics).AdminExMonLogger.SetMailboxGuid(base.MailboxGuid);
				return ErrorCode.NoError;
			}

			protected override void CleanupResources(MapiContext context)
			{
			}

			private readonly string domainController;

			private readonly byte[] partitionHint;
		}

		internal abstract class AdminRpcMultiMailboxSearchBase : AdminRpc
		{
			protected AdminRpcMultiMailboxSearchBase(AdminMethod methodId, ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] auxiliaryIn) : base(methodId, callerSecurityContext, new Guid?(mdbGuid), auxiliaryIn)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.ctor");
				this.multiMailboxSearcher = Hookable<IMultiMailboxSearch>.Create(true, new MultiMailboxSearch(mdbGuid, AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.SearchTimeOut));
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.ctor");
			}

			protected static ErrorCode InspectAndFixSearchCriteria(uint errorLID, Guid mdbGuid, ref SearchCriteria searchCriteria, CompareInfo compareInfo)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.InspectAndFixSearchCriteria");
				ErrorCode errorCode = ErrorCode.NoError;
				StorePropTag nonFullTextPropTag = StorePropTag.Invalid;
				if (searchCriteria != null)
				{
					searchCriteria = searchCriteria.InspectAndFix(delegate(SearchCriteria criteriaToInspect, CompareInfo localCompareInfo)
					{
						ErrorCode errorCode = ErrorCode.NoError;
						SearchCriteriaText searchCriteriaText = criteriaToInspect as SearchCriteriaText;
						SearchCriteriaCompare searchCriteriaCompare = (searchCriteriaText == null) ? (criteriaToInspect as SearchCriteriaCompare) : null;
						ExtendedPropertyColumn extendedColumn = null;
						if (searchCriteriaText != null)
						{
							extendedColumn = (searchCriteriaText.Lhs as ExtendedPropertyColumn);
						}
						else if (searchCriteriaCompare != null)
						{
							extendedColumn = (searchCriteriaCompare.Lhs as ExtendedPropertyColumn);
						}
						errorCode = AdminRpcServer.AdminRpcMultiMailboxSearchBase.IsPropertyInFullTextIndex(errorLID, mdbGuid, extendedColumn, ref nonFullTextPropTag);
						if (errorCode != ErrorCode.NoError)
						{
							errorCode = errorCode;
							return Factory.CreateSearchCriteriaFalse();
						}
						return criteriaToInspect;
					}, compareInfo, true);
				}
				if (errorCode != ErrorCode.NoError && nonFullTextPropTag != StorePropTag.Invalid && ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(39056, 0L, "eDiscovery search on request validation failed due to query restriction/searchCriteria:{0} containing non full text property:{1}.", searchCriteria.ToString(), string.IsNullOrEmpty(nonFullTextPropTag.DescriptiveName) ? string.Empty : nonFullTextPropTag.DescriptiveName);
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.InspectAndFixSearchCriteria");
				return errorCode;
			}

			protected static void TraceFunction(string message)
			{
				if (string.IsNullOrEmpty(message))
				{
					return;
				}
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceFunction(40080, 0L, message);
				}
			}

			protected IMultiMailboxSearch MultiMailboxSearcher
			{
				get
				{
					return this.multiMailboxSearcher.Value;
				}
			}

			protected static string GetFastPropertyNameOfColumn(StorePropTag storePropTag, Guid mdbGuid)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.GetFastPropertyNameOfColumn");
				string text = string.Empty;
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(58768, 0L, "Fast Property name requested for {0} Store Property.", storePropTag.DescriptiveName);
				}
				FullTextIndexSchema.FullTextIndexInfo fullTextIndexInfo = null;
				if (FullTextIndexSchema.Current.IsPropertyInFullTextIndex(storePropTag.PropInfo.PropName, mdbGuid, out fullTextIndexInfo))
				{
					text = fullTextIndexInfo.FastPropertyName;
					ExAssert.RetailAssert(!string.IsNullOrEmpty(fullTextIndexInfo.FastPropertyName), "Fast Property Name " + storePropTag.DescriptiveName + " cannot be null or empty.");
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(34192, 0L, "Fast Property name requested for {0} Store Property and the Fast Property name is {1}", storePropTag.DescriptiveName, text);
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.GetFastPropertyNameOfColumn");
				return text;
			}

			protected abstract ErrorCode ExecuteRpc(MapiContext context);

			internal IDisposable SetMultiMailboxSearchTestHook(IMultiMailboxSearch search)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.SetMultiMailboxSearchTestHook");
				IDisposable result = this.multiMailboxSearcher.SetTestHook(search);
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.SetMultiMailboxSearchTestHook");
				return result;
			}

			protected abstract void UpdatePerfCounters(StorePerDatabasePerformanceCountersInstance perfInstance, long timeTaken, bool isFailed);

			protected ErrorCode ValidateSearchCriteria(MapiContext context, SearchCriteria searchCriteria, uint errorLID)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.ValidateSearchCriteria");
				ErrorCode errorCode = AdminRpcServer.AdminRpcMultiMailboxSearchBase.InspectAndFixSearchCriteria(errorLID, context.DatabaseGuid, ref searchCriteria, (context.Culture == null) ? null : context.Culture.CompareInfo);
				if (errorCode != ErrorCode.NoError)
				{
					errorCode = errorCode.Propagate((LID)50752U);
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(63216, 0L, "eDiscovery search on database {0} request validation failed due to query:{1} containing non full text property.", (base.Database != null) ? base.Database.MdbName : string.Empty, searchCriteria.ToString());
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.ValidateSearchCriteria");
				return errorCode;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.EcExcuteRpc");
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(50576, 0L, "eDiscovery search initiated on database {0}", base.Database.MdbName);
				}
				if (AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.IsMaxSearchCountReached())
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(47504, 0L, "eDiscovery Max allowed searches limit hit for database {0}", base.Database.MdbName);
					}
					ErrorCode result = ErrorCode.CreateMaxMultiMailboxSearchExceeded((LID)55560U);
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.EcExcuteRpc");
					return result;
				}
				ErrorCode errorCode = ErrorCode.NoError;
				StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(base.Database);
				StopwatchStamp stamp = StopwatchStamp.GetStamp();
				try
				{
					AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.IncrementSearchCount();
					errorCode = this.ExecuteRpc(context);
				}
				finally
				{
					AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.DecrementSearchCount();
					long num = (long)stamp.ElapsedTime.TotalMilliseconds;
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, long>(63888, 0L, "eDiscovery search initiated on database {0} took {1} ms", base.Database.MdbName, num);
					}
					if (databaseInstance != null)
					{
						this.UpdatePerfCounters(databaseInstance, num, errorCode != ErrorCode.NoError);
						databaseInstance.AverageMultiMailboxSearchTimeSpentInStore.IncrementBy(num);
						databaseInstance.AverageMultiMailboxSearchTimeSpentInStoreBase.Increment();
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.EcExcuteRpc");
				return errorCode;
			}

			protected ErrorCode RequestToSearchCriteria(MapiContext context, MultiMailboxRequestBase request, byte[] serializedQuery, out SearchCriteria searchCriteria)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcServerMultiMailboxSearchBase.RequestToSearchCriteria");
				ErrorCode result = ErrorCode.NoError;
				searchCriteria = null;
				try
				{
					Restriction restriction = Restriction.Deserialize(context, serializedQuery, null, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);
					searchCriteria = restriction.ToSearchCriteria(base.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);
				}
				catch (StoreException ex)
				{
					context.OnExceptionCatch(ex);
					result = ErrorCode.CreateMultiMailboxSearchInvalidRestriction((LID)43104U);
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceError<int, string>(59488, 0L, "Invalid query specified in the search request. The de-serialize of the query restriction failed with error code {0}, reason: {1}.", (int)ex.Error, string.IsNullOrEmpty(ex.Message) ? string.Empty : ex.Message);
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcServerMultiMailboxSearchBase.RequestToSearchCriteria");
				return result;
			}

			protected SearchCriteria CreateFolderRestrictionCriteria(MapiContext context, IReplidGuidMap replidGuidMap, MultiMailboxSearchMailboxInfo searchMailboxInfo)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Enterting AdminRpcServerMultiMailboxSearchBase.CreateMailboxSearchCriteria");
				SearchCriteria result = null;
				MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(base.Database);
				if (searchMailboxInfo.FolderRestriction == null || searchMailboxInfo.FolderRestriction.Length == 0)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<Guid, string>(39312, 0L, "No Folder Restriction specified for the mailbox {0} on database {1}", searchMailboxInfo.MailboxGuid, base.Database.MdbName);
					}
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcServerMultiMailboxSearchBase.CreateMailboxSearchCriteria");
					return result;
				}
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<Guid, string>(55696, 0L, "Folder Restriction specified for the mailbox {0} on database {1}, creating the folder restriction.", searchMailboxInfo.MailboxGuid, base.Database.MdbName);
				}
				if (searchMailboxInfo.FolderRestriction != null && searchMailboxInfo.FolderRestriction.Length > 0)
				{
					List<SearchCriteria> list = new List<SearchCriteria>(6);
					Restriction restriction = Restriction.Deserialize(context, searchMailboxInfo.FolderRestriction, null, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);
					SearchCriteria searchCriteria = restriction.ToSearchCriteria(base.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);
					SearchCriteriaAnd searchCriteriaAnd = searchCriteria as SearchCriteriaAnd;
					if (searchCriteriaAnd != null && searchCriteriaAnd.NestedCriteria != null && searchCriteriaAnd.NestedCriteria.Length > 0)
					{
						foreach (SearchCriteria searchCriteria2 in searchCriteriaAnd.NestedCriteria)
						{
							SearchCriteriaCompare searchCriteriaCompare = searchCriteria2 as SearchCriteriaCompare;
							if (searchCriteriaCompare == null)
							{
								if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.WarningTrace))
								{
									ExTraceGlobals.MultiMailboxSearchTracer.TraceWarning<string, Guid, string>(43584, 0L, "Invalid SearchCriteria type found for request:{0} on mailbox:{1} in database:{2}, expected SearchCriteriaCompare, ignoring the current Criteria", searchMailboxInfo.CorrelationId.ToString(), searchMailboxInfo.MailboxGuid, base.Database.MdbName);
								}
							}
							else
							{
								ConstantColumn constantColumn = searchCriteriaCompare.Rhs as ConstantColumn;
								if (constantColumn == null)
								{
									if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.WarningTrace))
									{
										ExTraceGlobals.MultiMailboxSearchTracer.TraceWarning<string, Guid, string>(59968, 0L, "FolderRestriction SearchCriteria has incorrect RHS, RHS must be a ConstantColumn on request:{0} on mailbox:{1} in database:{2}, ignoring the current Criteria", searchMailboxInfo.CorrelationId.ToString(), searchMailboxInfo.MailboxGuid, base.Database.MdbName);
									}
								}
								else
								{
									byte[] array = constantColumn.Value as byte[];
									if (array == null || array.Length != 22)
									{
										if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.WarningTrace))
										{
											ExTraceGlobals.MultiMailboxSearchTracer.TraceWarning<string, Guid, string>(35392, 0L, "FolderRestriction SearchCriteria has invalid folderId value for request:{0} on mailbox:{1} in database:{2}.Expected a FID represented as ByteArray[22] ignoring the current Criteria", searchMailboxInfo.CorrelationId.ToString(), searchMailboxInfo.MailboxGuid, base.Database.MdbName);
										}
									}
									else
									{
										ExchangeId exchangeId = ExchangeId.CreateFrom22ByteArray(context, replidGuidMap, array);
										list.Add(Factory.CreateSearchCriteriaCompare(messageTable.FolderId, searchCriteriaCompare.RelOp, Factory.CreateConstantColumn(exchangeId.To26ByteArray())));
									}
								}
							}
						}
					}
					if (list.Count == 1)
					{
						result = list[0];
					}
					if (list.Count > 1)
					{
						result = Factory.CreateSearchCriteriaAnd(list.ToArray());
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcServerMultiMailboxSearchBase.CreateMailboxSearchCriteria");
				return result;
			}

			protected override void CleanupResources(MapiContext context)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.CleanupResources");
				if (this.MultiMailboxSearcher != null && this.MultiMailboxSearcher is IDisposable)
				{
					((IDisposable)this.MultiMailboxSearcher).Dispose();
					this.multiMailboxSearcher = null;
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.CleanupResources");
			}

			protected void SetResponseByteArray(byte[] responseAsBytes)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.SetResponseByteArray");
				this.responseByteArray = responseAsBytes;
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.SetResponseByteArray");
			}

			private static ErrorCode IsPropertyInFullTextIndex(uint errorLID, Guid mdbGuid, ExtendedPropertyColumn extendedColumn, ref StorePropTag nonFullTextPropTag)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchBase.IsPropertyInFullTextIndex");
				ErrorCode result = ErrorCode.NoError;
				FullTextIndexSchema.FullTextIndexInfo fullTextIndexInfo = null;
				if (extendedColumn != null && !FullTextIndexSchema.Current.IsPropertyInFullTextIndex(extendedColumn.StorePropTag.PropInfo.PropName, mdbGuid, out fullTextIndexInfo))
				{
					nonFullTextPropTag = extendedColumn.StorePropTag;
					result = ErrorCode.CreateMultiMailboxSearchNonFullTextSearch((LID)errorLID, nonFullTextPropTag.PropTag);
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchBase.IsPropertyInFullTextIndex");
				return result;
			}

			internal byte[] ResponseAsByteArray
			{
				get
				{
					return this.responseByteArray;
				}
			}

			protected MultiMailboxResponseBase Response
			{
				get
				{
					return this.searchResponse;
				}
				set
				{
					this.searchResponse = value;
				}
			}

			private const int ExpectedFolderIdByteArrayLength = 22;

			private Hookable<IMultiMailboxSearch> multiMailboxSearcher;

			private MultiMailboxResponseBase searchResponse;

			private byte[] responseByteArray;
		}

		internal class AdminRpcMultiMailboxSearch : AdminRpcServer.AdminRpcMultiMailboxSearchBase
		{
			internal AdminRpcMultiMailboxSearch(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] searchRequestByteArray, byte[] auxiliaryIn) : base(AdminMethod.EcMultiMailboxSearch, callerSecurityContext, mdbGuid, auxiliaryIn)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.ctor");
				this.searchRequest = MultiMailboxSearchRequest.DeSerialize(searchRequestByteArray);
				this.resultSet = new List<MultiMailboxSearchResult>(1);
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.ctor");
			}

			internal static List<string> RequiredRefinersList
			{
				get
				{
					return AdminRpcServer.AdminRpcMultiMailboxSearch.requiredRefinerList;
				}
			}

			private static List<string> InitializeRefinerList()
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminMultiMailboxSearch.InitializeRefinerList");
				List<string> list = new List<string>(1);
				string fastPropertyNameOfColumn = AdminRpcServer.AdminRpcMultiMailboxSearchBase.GetFastPropertyNameOfColumn(PropTag.Message.MessageSize, Guid.Empty);
				ExAssert.RetailAssert(!string.IsNullOrEmpty(fastPropertyNameOfColumn), "MessageSize FastProperty Name cannot be empty.");
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(43408, 0L, "Selecting the following refiners {0}(FastPropertyName:{1})", PropTag.Message.MessageSize.DescriptiveName, fastPropertyNameOfColumn);
				}
				list.Add(fastPropertyNameOfColumn);
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminMultiMailboxSearch.InitializeRefinerList");
				return list;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.EcValidateArguments");
				ErrorCode errorCode = base.EcValidateArguments(context);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(59792, 0L, "eDiscovery search on database {0} base request validation failed.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.EcValidateArguments");
					return errorCode.Propagate((LID)49248U);
				}
				if (this.SearchRequest == null)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(36080, 0L, "eDiscovery search on database {0} request validation failed.Invalid or empty search request.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)46140U);
				}
				if (errorCode == ErrorCode.NoError && (this.SearchRequest.Restriction == null || this.SearchRequest.Restriction.Length == 0))
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(60656, 0L, "eDiscovery search on database {0} request validation failed.Invalid or empty restriction.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)37872U);
				}
				if (errorCode == ErrorCode.NoError && (this.SearchRequest.MailboxInfos == null || this.SearchRequest.MailboxInfos.Length == 0))
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(35216, 0L, "eDiscovery search on database {0} request validation failed.Invalid or empty Mailbox.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)54256U);
				}
				if (errorCode == ErrorCode.NoError && this.SearchRequest.MailboxInfos.Length > 1)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(44272, 0L, "eDiscovery search on database {0} request validation failed. Request has multiple mailboxes to search for.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)41968U);
				}
				if (errorCode == ErrorCode.NoError && this.SearchRequest.Paging == null)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(56560, 0L, "eDiscovery search on database {0} request validation failed. Invalid paging info.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)58352U);
				}
				if (errorCode == ErrorCode.NoError && this.SearchRequest.Paging.PageSize <= 0)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(40176, 0L, "eDiscovery search on database {0} request validation failed. Invalid Page size specified.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)33776U);
				}
				if (errorCode == ErrorCode.NoError && string.IsNullOrEmpty(this.SearchRequest.Query))
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(64752, 0L, "eDiscovery search on database {0} request validation failed.Invalid or empty query in the request.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)62524U);
				}
				if (errorCode == ErrorCode.NoError && this.SearchRequest.RefinersEnabled && this.SearchRequest.RefinerResultsTrimCount <= 0)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(48368, 0L, "eDiscovery search on database {0} request validation failed.Refiner validation failed.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)50160U);
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.EcValidateArguments");
				return errorCode;
			}

			protected override void UpdatePerfCounters(StorePerDatabasePerformanceCountersInstance perfInstance, long timeTaken, bool isFailed)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.UpdatePerfCounters");
				if (perfInstance != null)
				{
					perfInstance.TotalMultiMailboxPreviewSearches.Increment();
					perfInstance.AverageMultiMailboxPreviewSearchLatency.IncrementBy(timeTaken);
					perfInstance.AverageMultiMailboxPreviewSearchLatencyBase.Increment();
					if (isFailed)
					{
						perfInstance.MultiMailboxPreviewSearchesFailed.Increment();
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.UpdatePerfCounters");
			}

			protected override ErrorCode ExecuteRpc(MapiContext context)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.ExecuteRpc");
				ErrorCode errorCode = this.InitializeAndValidateSearchRequest(context);
				if (errorCode != ErrorCode.NoError)
				{
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.ExecuteRpc");
					return errorCode.Propagate((LID)38464U);
				}
				string text = this.GenerateClauseforPagination(context);
				char c = (this.searchRequest.SortingOrder == Sorting.Ascending) ? '+' : '-';
				string text2 = string.Format("{0}{1} {0}[docid]", c, this.sortByPropertyName);
				SearchCriteria searchCriteria = null;
				context.InitializeMailboxExclusiveOperation(this.SearchRequest.MailboxInfos[0].MailboxGuid, ExecutionDiagnostics.OperationSource.AdminRpc, MapiContext.MailboxLockTimeout);
				bool commit = false;
				int mailboxNumber;
				try
				{
					errorCode = context.StartMailboxOperation(MailboxCreation.DontAllow, false, true);
					if (errorCode != ErrorCode.NoError)
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceError<Guid, string>(55872L, "eDiscovery preview search on mailbox:{0} in database {1} failed. Mailbox state for this mailbox is invalid.", this.SearchRequest.MailboxInfos[0].MailboxGuid, base.Database.MdbName);
						}
						return ErrorCode.CreateMultiMailboxSearchMailboxNotFound((LID)47680U);
					}
					mailboxNumber = context.LockedMailboxState.MailboxNumber;
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
					{
						searchCriteria = base.CreateFolderRestrictionCriteria(context, mailbox.ReplidGuidMap, this.SearchRequest.MailboxInfos[0]);
					}
					commit = true;
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(commit);
					}
				}
				if (searchCriteria != null)
				{
					this.searchCriteria = Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
					{
						this.searchCriteria,
						searchCriteria
					});
				}
				MultiMailboxSearchCriteria multiMailboxSearchCriteria = new MultiMailboxSearchCriteria(this.SearchRequest.CorrelationId, this.searchCriteria, this.SearchRequest.MailboxInfos[0].MailboxGuid, mailboxNumber, this.SearchRequest.Query, (this.SearchRequest.Paging != null) ? this.SearchRequest.Paging.PageSize : this.DefaultPageSize, text2, text);
				string text3 = multiMailboxSearchCriteria.SearchCriteria.ToString();
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation(51600, 0L, "eDiscovery search on database {0} for {1} mailboxes using the following paginationClause {2} and sortSpecification {3}", new object[]
					{
						base.Database.MdbName,
						this.SearchRequest.MailboxInfos.Length,
						text,
						text2
					});
				}
				base.MultiMailboxSearcher.RefinersList = AdminRpcServer.AdminRpcMultiMailboxSearch.RequiredRefinersList;
				IList<FullTextIndexRow> list = null;
				Dictionary<string, List<RefinersResultRow>> dictionary = null;
				KeywordStatsResultRow keywordStatsResultRow = null;
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(45456, 0L, "Executing eDiscovery search on database {0} for the following query {1}", base.Database.MdbName, text3);
				}
				errorCode = base.MultiMailboxSearcher.Search(context, multiMailboxSearchCriteria, out list, out keywordStatsResultRow, out dictionary);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string, string>(61840, 0L, "Executing eDiscovery search on database {0} for the following query {1} failed with the error code {2}.", base.Database.MdbName, text3, errorCode.ToString());
					}
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.ExecuteRpc");
					return errorCode.Propagate((LID)63040U);
				}
				if (list != null && list.Count > 0 && this.SearchRequest.RefinersEnabled && dictionary == null && ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.WarningTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceWarning<int, string>(64656, 0L, "MultiMailbox Search Query returned results(count={0}), but refiners result were not returned for query = {1}", list.Count, this.SearchRequest.Query);
				}
				long num = 0L;
				long num2 = 0L;
				if (keywordStatsResultRow != null)
				{
					num = keywordStatsResultRow.Count;
					num2 = (long)keywordStatsResultRow.Size;
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation(37264, 0L, "Executing eDiscovery search on database {0} for the following query {1} returned {2} hits accounting to {3} bytes result size.", new object[]
						{
							base.Database.MdbName,
							text3,
							num,
							num2
						});
					}
				}
				if (this.SearchRequest.RefinersEnabled && dictionary != null && dictionary.Count > 0)
				{
					Dictionary<string, List<MultiMailboxSearchRefinersResult>> refinersOutput = this.CreateMultiMailboxSearchRefinerResults(dictionary);
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(53648, 0L, "Creating Response with refiners data for eDiscovery search on database {0} for the following query {1}", base.Database.MdbName, text3);
					}
					base.Response = new MultiMailboxSearchResponse(refinersOutput, num, num2);
				}
				else
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(41360, 0L, "Creating Response with no refiners data for eDiscovery search on database {0} for the following query {1}", base.Database.MdbName, text3);
					}
					base.Response = new MultiMailboxSearchResponse(null, num, num2);
				}
				foreach (FullTextIndexRow fullTextIndexRow in list)
				{
					this.resultSet.Add(new MultiMailboxSearchResult(fullTextIndexRow.MailboxGuid, fullTextIndexRow.DocumentId, fullTextIndexRow.FastDocumentId));
				}
				base.Response.Results = this.resultSet.ToArray();
				if (this.resultSet.Count > 0)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(57744, 0L, "Setting the response with paging reference for eDiscovery search on database {0} for the following query {1}", base.Database.MdbName, text3);
					}
					((MultiMailboxSearchResponse)base.Response).PagingReference = new PageReference
					{
						PreviousPageReference = this.resultSet[0],
						NextPageReference = this.resultSet[this.resultSet.Count - 1]
					};
				}
				base.SetResponseByteArray(MultiMailboxSearchResponse.Serialize((MultiMailboxSearchResponse)base.Response));
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<int, string, string>(33168, 0L, "Serializing the response(size:{0}) for the eDiscovery search query on database {1} for the following query {2}", base.ResponseAsByteArray.Length, base.Database.MdbName, text3);
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.ExecuteRpc");
				return errorCode;
			}

			private ErrorCode InitializeAndValidateSearchRequest(MapiContext context)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.InitializeAndValidateSearchRequest");
				ErrorCode errorCode = ErrorCode.NoError;
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(38640, 0L, "eDiscovery search on database {0} request validation, Generating the SearchCriteria from the SearchRequest.", (base.Database != null) ? base.Database.MdbName : string.Empty);
				}
				errorCode = base.RequestToSearchCriteria(context, this.SearchRequest, this.SearchRequest.Restriction, out this.searchCriteria);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(51296, 0L, "eDiscovery search on database {0} request validation failed.Invalid/Incorrect query:\"{1}\".", (base.Database != null) ? base.Database.MdbName : string.Empty, this.searchRequest.Query);
					}
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.InitializeAndValidateSearchRequest");
					return errorCode.Propagate((LID)34912U);
				}
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(55024, 0L, "eDiscovery search on database {0} request validation, validating the SearchCriteria for non full text index property.", (base.Database != null) ? base.Database.MdbName : string.Empty);
				}
				errorCode = base.ValidateSearchCriteria(context, this.searchCriteria, 43152U);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(48368, 0L, "eDiscovery search on database {0} request validation failed.Query has non full text index property.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.InitializeAndValidateSearchRequest");
					return errorCode.Propagate((LID)51952U);
				}
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(42736, 0L, "eDiscovery search on database {0} request validation, validating the sortBy Criteria.", (base.Database != null) ? base.Database.MdbName : string.Empty);
				}
				if (errorCode == ErrorCode.NoError && (this.SearchRequest.SortCriteria == null || this.SearchRequest.SortCriteria.Length == 0))
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(58608, 0L, "eDiscovery search on database {0} request validation failed.No SortCriteria specified.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateMultiMailboxSearchInvalidSortBy((LID)34032U);
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.InitializeAndValidateSearchRequest");
					return errorCode;
				}
				if (errorCode == ErrorCode.NoError)
				{
					errorCode = this.GetSortByPropertyFromRequest(context, out this.sortByPropertyName);
					if (errorCode != ErrorCode.NoError)
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(48368, 0L, "eDiscovery search on database {0} request validation failed.Invalid SortBy property:{1} specified in the request", (base.Database != null) ? base.Database.MdbName : string.Empty, this.sortByPropertyName);
						}
						AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.InitializeAndValidateSearchRequest");
						return errorCode.Propagate((LID)62192U);
					}
				}
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(59120, 0L, "eDiscovery search on database {0} request validation, validating the pagination Criteria.", (base.Database != null) ? base.Database.MdbName : string.Empty);
				}
				if (errorCode == ErrorCode.NoError && this.SearchRequest.Paging.Direction != PagingDirection.None)
				{
					if (this.SearchRequest.Paging.ReferenceItem == null)
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string, string>(35568, 0L, "eDiscovery search query on database {0} for the following query {1} with paging direction:{2}, but reference item is null", (base.Database != null) ? base.Database.MdbName : string.Empty, this.SearchRequest.Query, (this.SearchRequest.Paging.Direction == PagingDirection.None) ? "none" : ((this.SearchRequest.Paging.Direction == PagingDirection.Next) ? "next" : "previous"));
						}
						errorCode = ErrorCode.CreateMultiMailboxSearchInvalidPagination((LID)56048U);
					}
					if (errorCode == ErrorCode.NoError && (this.SearchRequest.Paging.ReferenceItem.EqualsRestriction == null || this.SearchRequest.Paging.ReferenceItem.EqualsRestriction.Length == 0))
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(60144, 0L, "eDiscovery search on database {0} request validation failed.Invalid or empty PaginationRestriction", (base.Database != null) ? base.Database.MdbName : string.Empty);
						}
						errorCode = ErrorCode.CreateMultiMailboxSearchInvalidPagination((LID)43760U);
					}
					if (errorCode == ErrorCode.NoError && (this.SearchRequest.Paging.ReferenceItem.ComparisionRestriction == null || this.SearchRequest.Paging.ReferenceItem.ComparisionRestriction.Length == 0))
					{
						errorCode = ErrorCode.CreateMultiMailboxSearchInvalidPagination((LID)63728U);
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(61168, 0L, "eDiscovery search on database {0} request validation failed.Invalid or empty PaginationRestriction", (base.Database != null) ? base.Database.MdbName : string.Empty);
						}
					}
				}
				if (errorCode == ErrorCode.NoError && this.SearchRequest.Paging.Direction != PagingDirection.None && this.SearchRequest.Paging.ReferenceItem != null)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(47344, 0L, "eDiscovery search on database {0} request validation, validating the pagination filter for non fulltext property", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = this.CreatePagingCriteriaFromRequest(context, out this.paginationEqualsSearchCriteria, out this.paginationCriteria);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)41712U);
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.InitializeAndValidateSearchRequest");
				return errorCode;
			}

			private ErrorCode GetSortByPropertyFromRequest(MapiContext context, out string sortByPropertyName)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.GetSortByPropertyFromRequest");
				sortByPropertyName = string.Empty;
				RestrictionExists restrictionExists = Restriction.Deserialize(context, this.SearchRequest.SortCriteria, null, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message) as RestrictionExists;
				ErrorCode result;
				if (restrictionExists != null)
				{
					sortByPropertyName = AdminRpcServer.AdminRpcMultiMailboxSearchBase.GetFastPropertyNameOfColumn(restrictionExists.PropertyTag, context.DatabaseGuid);
					if (!string.IsNullOrEmpty(sortByPropertyName))
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation(52976, 0L, "eDiscovery search query on database {0} for the following query {1} using {2} sort on sortBy property:{3}", new object[]
							{
								base.Database.MdbName,
								this.SearchRequest.Query,
								(this.SearchRequest.SortingOrder == Sorting.Ascending) ? "ascending" : "descending",
								sortByPropertyName
							});
						}
						result = ErrorCode.NoError;
					}
					else
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation(46832, 0L, "eDiscovery search query on database {0} for the following query {1} using {2} sort on non full text index property:{3}", new object[]
							{
								base.Database.MdbName,
								this.SearchRequest.Query,
								(this.SearchRequest.SortingOrder == Sorting.Ascending) ? "ascending" : "descending",
								restrictionExists.PropertyTag.DescriptiveName
							});
						}
						result = ErrorCode.CreateMultiMailboxSearchNonFullTextSortBy((LID)36592U, restrictionExists.PropertyTag.PropTag);
					}
				}
				else
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string, string>(34544, 0L, "eDiscovery search query on database {0} for the following query {1} using {2} sort, has invalid sortBy restriction", base.Database.MdbName, this.SearchRequest.Query, (this.SearchRequest.SortingOrder == Sorting.Ascending) ? "ascending" : "descending");
					}
					result = ErrorCode.CreateMultiMailboxSearchInvalidSortBy((LID)50416U);
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.GetSortByPropertyFromRequest");
				return result;
			}

			private ErrorCode CreatePagingCriteriaFromRequest(MapiContext context, out SearchCriteria paginationReferenceItemEqualsSearchCritieria, out SearchCriteria paginationSearchCriteria)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.CreatePagingCriteriaFromRequest");
				ErrorCode errorCode = ErrorCode.NoError;
				paginationReferenceItemEqualsSearchCritieria = null;
				paginationSearchCriteria = null;
				if (this.SearchRequest.Paging.Direction != PagingDirection.None && this.SearchRequest.Paging.ReferenceDocumentId > 0L)
				{
					errorCode = this.CreatePaginationSearchCriteriaFromReferenceItemRestriction(context, this.SearchRequest.Paging.ReferenceItem.EqualsRestriction, out paginationReferenceItemEqualsSearchCritieria);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)32832U);
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string, ErrorCode>(47856, 0L, "eDiscovery search on database {0} for query:{1} coverting pagination reference item's equals filter restriction to search criteria failed with errorcode: {2}", (base.Database != null) ? base.Database.MdbName : string.Empty, this.SearchRequest.Query, errorCode);
						}
					}
					if (errorCode == ErrorCode.NoError)
					{
						errorCode = this.CreatePaginationSearchCriteriaFromReferenceItemRestriction(context, this.SearchRequest.Paging.ReferenceItem.ComparisionRestriction, out paginationSearchCriteria);
						if (errorCode != ErrorCode.NoError)
						{
							errorCode = errorCode.Propagate((LID)49216U);
							if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
							{
								ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string, ErrorCode>(64240, 0L, "eDiscovery search on database {0} for query:{1} coverting pagination reference item's comparision filter restriction to search criteria failed with errorcode: {2}", (base.Database != null) ? base.Database.MdbName : string.Empty, this.SearchRequest.Query, errorCode);
							}
						}
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.CreatePagingCriteriaFromRequest");
				return errorCode;
			}

			private ErrorCode CreatePaginationSearchCriteriaFromReferenceItemRestriction(MapiContext context, byte[] serializedRestriction, out SearchCriteria paginationSearchCriteria)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.CreatePaginationSearchCriteriaFromReferenceItemRestriction");
				ErrorCode errorCode = ErrorCode.NoError;
				paginationSearchCriteria = null;
				if (serializedRestriction == null || serializedRestriction.Length == 0)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(50928, 0L, "eDiscovery search on database {0} for query:{1} pagination reference item's filter restriction is null or empty length.", (base.Database != null) ? base.Database.MdbName : string.Empty, this.SearchRequest.Query);
					}
					errorCode = ErrorCode.CreateMultiMailboxSearchInvalidPagination((LID)39664U);
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.CreatePaginationSearchCriteriaFromReferenceItemRestriction");
					return errorCode;
				}
				Restriction restriction = Restriction.Deserialize(context, serializedRestriction, null, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);
				if (restriction != null)
				{
					SearchCriteria searchCriteria = restriction.ToSearchCriteria(base.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);
					errorCode = AdminRpcServer.AdminRpcMultiMailboxSearchBase.InspectAndFixSearchCriteria(42224U, context.DatabaseGuid, ref searchCriteria, (context.Culture == null) ? null : context.Culture.CompareInfo);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode = errorCode.Propagate((LID)48704U);
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string, string>(54512, 0L, "eDiscovery search on database {0} for query:{1} has non FullText Index property in the pagination reference item equals filter:{2}.", (base.Database != null) ? base.Database.MdbName : string.Empty, this.SearchRequest.Query, (searchCriteria != null) ? searchCriteria.ToString() : string.Empty);
						}
						errorCode = ErrorCode.CreateMultiMailboxSearchNonFullTextPropertyInPagination((LID)38128U);
						AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.CreatePaginationSearchCriteriaFromReferenceItemRestriction");
						return errorCode;
					}
					paginationSearchCriteria = searchCriteria;
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string, string>(59536, 0L, "eDiscovery search query on database {0} for the following query {1} using pagination criteria:{2}", base.Database.MdbName, this.SearchRequest.Query, paginationSearchCriteria.ToFqlString(FqlQueryGenerator.Options.LooseCheck, context.Culture));
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.CreatePaginationSearchCriteriaFromReferenceItemRestriction");
				return errorCode;
			}

			private Dictionary<string, List<MultiMailboxSearchRefinersResult>> CreateMultiMailboxSearchRefinerResults(Dictionary<string, List<RefinersResultRow>> refinersResults)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.CreateMultiMailboxSearchRefinerResults");
				Dictionary<string, List<MultiMailboxSearchRefinersResult>> dictionary = new Dictionary<string, List<MultiMailboxSearchRefinersResult>>(refinersResults.Count);
				foreach (string key in refinersResults.Keys)
				{
					List<RefinersResultRow> list = refinersResults[key];
					if (list != null && list.Count > 0)
					{
						if (list.Count > this.SearchRequest.RefinerResultsTrimCount)
						{
							list = list.Take(this.SearchRequest.RefinerResultsTrimCount).ToList<RefinersResultRow>();
						}
						List<MultiMailboxSearchRefinersResult> list2 = new List<MultiMailboxSearchRefinersResult>(list.Count);
						foreach (RefinersResultRow refinersResultRow in list)
						{
							list2.Add(new MultiMailboxSearchRefinersResult(refinersResultRow.EntryName, refinersResultRow.EntryCount));
						}
						dictionary.Add(key, list2);
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.CreateMultiMailboxSearchRefinerResults");
				return dictionary;
			}

			private string GenerateClauseforPagination(MapiContext context)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearch.GenerateClauseforPagination");
				string text = string.Empty;
				if (this.SearchRequest.Paging != null && this.SearchRequest.Paging.Direction != PagingDirection.None && this.SearchRequest.Paging.ReferenceDocumentId > 0L && this.SearchRequest.Paging.ReferenceItem != null && this.paginationCriteria != null && this.paginationEqualsSearchCriteria != null)
				{
					string text2 = string.Empty;
					string text3 = string.Empty;
					if (this.SearchRequest.SortingOrder == Sorting.Ascending)
					{
						text2 = this.SearchRequest.Paging.ReferenceDocumentId.ToString();
						text3 = "max";
					}
					else
					{
						text2 = "min";
						text3 = this.SearchRequest.Paging.ReferenceDocumentId.ToString();
					}
					text = string.Format("and({0},or(and(documentid:range({1},{2},from=gt,to=lt),{3}),{4}))", new string[]
					{
						"{0}",
						text2,
						text3,
						this.paginationEqualsSearchCriteria.ToFqlString(FqlQueryGenerator.Options.LooseCheck, context.Culture),
						this.paginationCriteria.ToFqlString(FqlQueryGenerator.Options.LooseCheck, context.Culture)
					});
				}
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string, string>(49552, 0L, "eDiscovery search query on database {0} for the following query {1} following pagination clause specified {2}", base.Database.MdbName, this.SearchRequest.Query, text);
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearch.GenerateClauseforPagination");
				return text;
			}

			protected MultiMailboxSearchRequest SearchRequest
			{
				get
				{
					return this.searchRequest;
				}
			}

			private const string PaginationClauseFormat = "and({0},or(and(documentid:range({1},{2},from=gt,to=lt),{3}),{4}))";

			private const string SortSpecificationFormat = "{0}{1} {0}[docid]";

			private const char FastQueryAscendingSortOrder = '+';

			private const char FastQueryDescendingSortOrder = '-';

			private readonly int DefaultPageSize = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "MultiMailboxSearch.DefaultPageSize", 25);

			private readonly MultiMailboxSearchRequest searchRequest;

			private static List<string> requiredRefinerList = AdminRpcServer.AdminRpcMultiMailboxSearch.InitializeRefinerList();

			private List<MultiMailboxSearchResult> resultSet;

			private SearchCriteria searchCriteria;

			private string sortByPropertyName = string.Empty;

			private SearchCriteria paginationEqualsSearchCriteria;

			private SearchCriteria paginationCriteria;
		}

		internal class AdminRpcMultiMailboxSearchKeywordStats : AdminRpcServer.AdminRpcMultiMailboxSearchBase
		{
			internal static List<string> RequiredRefinersList
			{
				get
				{
					return AdminRpcServer.AdminRpcMultiMailboxSearchKeywordStats.requiredRefinerList;
				}
			}

			internal AdminRpcMultiMailboxSearchKeywordStats(ClientSecurityContext callerSecurityContext, Guid mdbGuid, byte[] searchRequestByteArray, byte[] auxiliaryIn) : base(AdminMethod.EcGetMultiMailboxSearchKeywordStats, callerSecurityContext, mdbGuid, auxiliaryIn)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchKeywordStats.ctor");
				this.searchRequest = MultiMailboxKeywordStatsRequest.DeSerialize(searchRequestByteArray);
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.ctor");
			}

			private static List<string> InitializeRefinerList()
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchKeywordStats.InitializeRefinerList");
				List<string> list = new List<string>(1);
				string fastPropertyNameOfColumn = AdminRpcServer.AdminRpcMultiMailboxSearchBase.GetFastPropertyNameOfColumn(PropTag.Message.MessageSize, Guid.Empty);
				ExAssert.RetailAssert(!string.IsNullOrEmpty(fastPropertyNameOfColumn), "MessageSize FastProperty Name cannot be empty.");
				list.Add(fastPropertyNameOfColumn);
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.InitializeRefinerList");
				return list;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchKeywordStats.EcValidateArguments");
				ErrorCode errorCode = base.EcValidateArguments(context);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(49040, 0L, "eDiscovery keyword stats query on database {0} base request validation failed.", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.EcValidateArguments");
					return errorCode.Propagate((LID)53344U);
				}
				if (this.SearchRequest == null)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(65424, 0L, "eDiscovery keyword stats query on database {0} request validation failed.Search request is null", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxKeywordStatsRequest((LID)65084U);
				}
				if (errorCode == ErrorCode.NoError && (this.SearchRequest.MailboxInfos == null || this.SearchRequest.MailboxInfos.Length == 0))
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(52464, 0L, "eDiscovery keyword stats query on database {0} request validation failed.Search request is null", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxKeywordStatsRequest((LID)34960U);
				}
				if (errorCode == ErrorCode.NoError && this.SearchRequest.MailboxInfos.Length > 1)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(46320, 0L, "eDiscovery keyword stats query on database {0} request validation failed.Search request is null", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxKeywordStatsRequest((LID)51344U);
				}
				if (errorCode == ErrorCode.NoError && (this.SearchRequest.Keywords == null || this.SearchRequest.Keywords.Count == 0))
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string>(62704, 0L, "eDiscovery keyword stats query on database {0} request validation failed for Keywords property.Invalid or empty Keywords", (base.Database != null) ? base.Database.MdbName : string.Empty);
					}
					errorCode = ErrorCode.CreateInvalidMultiMailboxKeywordStatsRequest((LID)62448U);
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchKeywordStats.EcValidateArguments");
				return errorCode;
			}

			protected override void UpdatePerfCounters(StorePerDatabasePerformanceCountersInstance perfInstance, long timeTaken, bool isFailed)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchKeywordStats.UpdatePerfCounters");
				if (perfInstance != null)
				{
					perfInstance.TotalMultiMailboxKeywordStatsSearches.Increment();
					perfInstance.AverageMultiMailboxKeywordStatsSearchLatency.IncrementBy(timeTaken);
					perfInstance.AverageMultiMailboxKeywordStatsSearchLatencyBase.Increment();
					if (isFailed)
					{
						perfInstance.MultiMailboxKeywordStatsSearchesFailed.Increment();
					}
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.UpdatePerfCounters");
			}

			protected override ErrorCode ExecuteRpc(MapiContext context)
			{
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Entering AdminRpcMultiMailboxSearchKeywordStats.ExecuteRpc");
				ErrorCode errorCode = ErrorCode.NoError;
				int num = Math.Min(this.SearchRequest.Keywords.Count, AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.MaxAllowedKeywordStatsQueryCount);
				List<MultiMailboxKeywordStatsResult> list = new List<MultiMailboxKeywordStatsResult>(num);
				IList<KeywordStatsResultRow> list2 = new List<KeywordStatsResultRow>(num);
				base.MultiMailboxSearcher.RefinersList = AdminRpcServer.AdminRpcMultiMailboxSearchKeywordStats.RequiredRefinersList;
				List<MultiMailboxSearchCriteria> list3 = new List<MultiMailboxSearchCriteria>(num);
				SearchCriteria searchCriteria = null;
				context.InitializeMailboxExclusiveOperation(this.SearchRequest.MailboxInfos[0].MailboxGuid, ExecutionDiagnostics.OperationSource.AdminRpc, MapiContext.MailboxLockTimeout);
				bool commit = false;
				int mailboxNumber;
				try
				{
					errorCode = context.StartMailboxOperation(MailboxCreation.DontAllow, false, true);
					if (errorCode != ErrorCode.NoError)
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceError<Guid, string>(64064L, "eDiscovery preview search on mailbox:{0} in database {1} failed. Mailbox state for this mailbox is invalid.", this.SearchRequest.MailboxInfos[0].MailboxGuid, base.Database.MdbName);
						}
						return ErrorCode.CreateMultiMailboxSearchMailboxNotFound((LID)39488U);
					}
					mailboxNumber = context.LockedMailboxState.MailboxNumber;
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
					{
						searchCriteria = base.CreateFolderRestrictionCriteria(context, mailbox.ReplidGuidMap, this.SearchRequest.MailboxInfos[0]);
					}
					commit = true;
				}
				finally
				{
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(commit);
					}
				}
				for (int i = 0; i < num; i++)
				{
					KeyValuePair<string, byte[]> keyValuePair = this.SearchRequest.Keywords[i];
					if (string.IsNullOrEmpty(keyValuePair.Key) || keyValuePair.Value == null || keyValuePair.Value.Length == 0)
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, int, int>(40848, 0L, "eDiscovery Keyword stats search on database {0} for {1} mailboxes, found invalid request at row ", base.Database.MdbName, this.SearchRequest.MailboxInfos.Length, num + 1);
						}
						AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.ExecuteRpc");
						errorCode = ErrorCode.CreateInvalidMultiMailboxKeywordStatsRequest((LID)65040U);
						return errorCode;
					}
					SearchCriteria searchCriteria2 = null;
					errorCode = base.RequestToSearchCriteria(context, this.SearchRequest, keyValuePair.Value, out searchCriteria2);
					if (errorCode != ErrorCode.NoError)
					{
						if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
						{
							ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, string>(61536, 0L, "eDiscovery search on database {0} request validation failed.Invalid/Incorrect query:\"{1}\".", (base.Database != null) ? base.Database.MdbName : string.Empty, keyValuePair.Key);
						}
						AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.ExecuteRpc");
						return errorCode.Propagate((LID)45152U);
					}
					errorCode = base.ValidateSearchCriteria(context, searchCriteria2, 55440U);
					if (errorCode != ErrorCode.NoError)
					{
						AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.ExecuteRpc");
						return errorCode.Propagate((LID)57440U);
					}
					if (searchCriteria != null)
					{
						searchCriteria2 = Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
						{
							searchCriteria2,
							searchCriteria
						});
					}
					MultiMailboxSearchCriteria item = new MultiMailboxSearchCriteria(this.SearchRequest.CorrelationId, searchCriteria2, this.SearchRequest.MailboxInfos[0].MailboxGuid, mailboxNumber, keyValuePair.Key);
					list3.Add(item);
				}
				errorCode = base.MultiMailboxSearcher.GetKeywordStatistics(context, list3.ToArray(), out list2);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<string, int, string>(57232, 0L, "eDiscovery Keyword stats search on database {0} for {1} mailboxes failed with the error code {2}", base.Database.MdbName, this.SearchRequest.MailboxInfos.Length, errorCode.ToString());
					}
					AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.ExecuteRpc");
					return errorCode.Propagate((LID)32864U);
				}
				base.Response = new MultiMailboxKeywordStatsResponse();
				foreach (KeywordStatsResultRow keywordStatsResultRow in list2)
				{
					list.Add(new MultiMailboxKeywordStatsResult(keywordStatsResultRow.Keyword, keywordStatsResultRow.Count, (long)keywordStatsResultRow.Size));
				}
				base.Response.Results = list.ToArray();
				base.SetResponseByteArray(MultiMailboxKeywordStatsResponse.Serialize((MultiMailboxKeywordStatsResponse)base.Response));
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<int, string, int>(44944, 0L, "Serializing the response(size:{0}) for the eDiscovery keyword stats query on database {1} for the keyword count {2}", base.ResponseAsByteArray.Length, base.Database.MdbName, this.SearchRequest.Keywords.Count);
				}
				AdminRpcServer.AdminRpcMultiMailboxSearchBase.TraceFunction("Exiting AdminRpcMultiMailboxSearchKeywordStats.ExecuteRpc");
				return errorCode;
			}

			protected MultiMailboxKeywordStatsRequest SearchRequest
			{
				get
				{
					return this.searchRequest;
				}
			}

			private readonly MultiMailboxKeywordStatsRequest searchRequest;

			private static List<string> requiredRefinerList = AdminRpcServer.AdminRpcMultiMailboxSearchKeywordStats.InitializeRefinerList();
		}

		internal class MultiMailboxSearchFactory
		{
			protected MultiMailboxSearchFactory()
			{
				AdminRpcServer.TraceFunction("Entering MultiMailboxSearchFactory.ctor");
				this.maxSearchAllowed = (long)RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "eDiscoveryMaxSearches", 100);
				this.searchTimeOutInSeconds = (long)RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "eDiscoverySearchTimeout", 120);
				this.maxAllowedKeywordStatsQueryPerCall = (long)RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "eDiscoverySearchMaxQueryCount", 25);
				if (ExTraceGlobals.MultiMailboxSearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.MultiMailboxSearchTracer.TraceInformation<long, long, long>(61328, 0L, "eDiscovery MultiMailboxSearch Factory Intialized with values MaxSearchAllowed: {0}, SearchTimeOutInSecond: {1}, KeywordStatsMaBatchSize: {2}", this.maxSearchAllowed, this.searchTimeOutInSeconds, this.maxAllowedKeywordStatsQueryPerCall);
				}
				AdminRpcServer.TraceFunction("Exiting MultiMailboxSearchFactory.ctor");
			}

			internal static Hookable<AdminRpcServer.MultiMailboxSearchFactory> Instance
			{
				get
				{
					if (AdminRpcServer.MultiMailboxSearchFactory.instance == null)
					{
						AdminRpcServer.MultiMailboxSearchFactory.syncRoot = new object();
						using (LockManager.Lock(AdminRpcServer.MultiMailboxSearchFactory.syncRoot))
						{
							if (AdminRpcServer.MultiMailboxSearchFactory.instance == null)
							{
								AdminRpcServer.MultiMailboxSearchFactory.instance = Hookable<AdminRpcServer.MultiMailboxSearchFactory>.Create(true, new AdminRpcServer.MultiMailboxSearchFactory());
							}
						}
					}
					return AdminRpcServer.MultiMailboxSearchFactory.instance;
				}
			}

			internal IDisposable SetMultiMailboxSearchFactoryTestHook(AdminRpcServer.MultiMailboxSearchFactory factory)
			{
				AdminRpcServer.TraceFunction("Entering MultiMailboxSearchFactory.SetMultiMailboxSearchFactoryTestHook");
				IDisposable result = AdminRpcServer.MultiMailboxSearchFactory.Instance.SetTestHook(factory);
				AdminRpcServer.TraceFunction("Exiting MultiMailboxSearchFactory.SetMultiMailboxSearchFactoryTestHook");
				return result;
			}

			internal TimeSpan SearchTimeOut
			{
				get
				{
					return TimeSpan.FromSeconds((double)AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.searchTimeOutInSeconds);
				}
			}

			internal long SearchTimeOutInSeconds
			{
				get
				{
					return AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.searchTimeOutInSeconds;
				}
				set
				{
					Interlocked.Exchange(ref AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.searchTimeOutInSeconds, value);
				}
			}

			internal long CurrentSearchCount
			{
				get
				{
					return AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.currentSearchCount;
				}
			}

			internal void IncrementSearchCount()
			{
				Interlocked.Increment(ref AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.currentSearchCount);
			}

			internal void DecrementSearchCount()
			{
				Interlocked.Decrement(ref AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.currentSearchCount);
			}

			internal int MaxSearchesAllowed
			{
				get
				{
					return (int)AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.maxSearchAllowed;
				}
				set
				{
					Interlocked.Exchange(ref AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.maxSearchAllowed, (long)value);
				}
			}

			internal int MaxAllowedKeywordStatsQueryCount
			{
				get
				{
					return (int)AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.maxAllowedKeywordStatsQueryPerCall;
				}
				set
				{
					Interlocked.Exchange(ref AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.maxAllowedKeywordStatsQueryPerCall, (long)value);
				}
			}

			internal bool IsMaxSearchCountReached()
			{
				long num = AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.CurrentSearchCount;
				Interlocked.Increment(ref num);
				return num >= (long)AdminRpcServer.MultiMailboxSearchFactory.Instance.Value.MaxSearchesAllowed;
			}

			private static object syncRoot = new object();

			private static Hookable<AdminRpcServer.MultiMailboxSearchFactory> instance;

			private long currentSearchCount;

			private long maxSearchAllowed;

			private long searchTimeOutInSeconds;

			private long maxAllowedKeywordStatsQueryPerCall;
		}

		internal abstract class AdminUserInfoBase : AdminRpc
		{
			protected AdminUserInfoBase(AdminMethod methodId, ClientSecurityContext callerSecurityContext, Guid mdbGuid, AdminRpc.ExpectedDatabaseState expectedDatabaseState, byte[] auxiliaryIn) : base(methodId, callerSecurityContext, new Guid?(mdbGuid), expectedDatabaseState, auxiliaryIn)
			{
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				if (!DefaultSettings.Get.UserInformationIsEnabled)
				{
					return ErrorCode.CreateNotSupported((LID)57932U);
				}
				ErrorCode errorCode = base.EcValidateArguments(context);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)48572U);
				}
				if (context.ClientType != ClientType.ADDriver)
				{
					return ErrorCode.CreateNoAccess((LID)37964U);
				}
				return errorCode;
			}
		}

		internal class AdminCreateUserInfo50 : AdminRpcServer.AdminUserInfoBase
		{
			internal AdminCreateUserInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] properties, byte[] auxiliaryIn) : base(AdminMethod.EcCreateUserInfo50, callerSecurityContext, mdbGuid, AdminRpc.ExpectedDatabaseState.OnlineActive, auxiliaryIn)
			{
				this.userInfoGuid = userInfoGuid;
				this.properties = properties;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				Properties initialProperties = AdminRpcServer.ParseProperties(this.properties, null);
				UserInformation.Create(context, this.userInfoGuid, initialProperties);
				return ErrorCode.NoError;
			}

			private readonly Guid userInfoGuid;

			private readonly byte[] properties;
		}

		internal class AdminReadUserInfo50 : AdminRpcServer.AdminUserInfoBase
		{
			internal AdminReadUserInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid userInfoGuid, uint flags, uint[] propertyTags, byte[] auxiliaryIn) : base(AdminMethod.EcReadUserInfo50, callerSecurityContext, mdbGuid, AdminRpc.ExpectedDatabaseState.AnyAttachedState, auxiliaryIn)
			{
				this.userInfoGuid = userInfoGuid;
				this.propertyTags = propertyTags;
			}

			public ArraySegment<byte> Result
			{
				get
				{
					return this.result;
				}
			}

			protected override ErrorCode EcCheckPermissions(MapiContext context)
			{
				if (context.SecurityContext.IsAuthenticated && context.SecurityContext.UserSid.IsWellKnown(WellKnownSidType.NetworkServiceSid))
				{
					return ErrorCode.NoError;
				}
				return base.EcCheckPermissions(context);
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				Properties properties = UserInformation.Read(context, this.userInfoGuid, LegacyHelper.ConvertFromLegacyPropTags(this.propertyTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.UserInfo, null, false));
				int num = 0;
				int num2 = AdminRpcParseFormat.SetValues(null, ref num, 0, properties);
				BufferPoolCollection.BufferSize bufferSize;
				byte[] array;
				if (BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize(num2, out bufferSize))
				{
					BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize);
					array = bufferPool.Acquire();
				}
				else
				{
					array = new byte[num2];
				}
				num = 0;
				AdminRpcParseFormat.SetValues(array, ref num, num2, properties);
				this.result = new ArraySegment<byte>(array, 0, num2);
				return ErrorCode.NoError;
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode first = base.EcValidateArguments(context);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)48572U);
				}
				if (this.propertyTags == null || this.propertyTags.Length == 0)
				{
					return ErrorCode.CreateInvalidParameter((LID)42428U);
				}
				return first;
			}

			private readonly Guid userInfoGuid;

			private readonly uint[] propertyTags;

			private ArraySegment<byte> result;
		}

		internal class AdminUpdateUserInfo50 : AdminRpcServer.AdminUserInfoBase
		{
			internal AdminUpdateUserInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] properties, uint[] deletePropertyTags, byte[] auxiliaryIn) : base(AdminMethod.EcUpdateUserInfo50, callerSecurityContext, mdbGuid, AdminRpc.ExpectedDatabaseState.OnlineActive, auxiliaryIn)
			{
				this.userInfoGuid = userInfoGuid;
				this.properties = properties;
				this.deletePropertyTags = deletePropertyTags;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				Properties properties = AdminRpcServer.ParseProperties(this.properties, this.deletePropertyTags);
				UserInformation.Update(context, this.userInfoGuid, properties);
				return ErrorCode.NoError;
			}

			private readonly Guid userInfoGuid;

			private readonly byte[] properties;

			private readonly uint[] deletePropertyTags;
		}

		internal class AdminDeleteUserInfo50 : AdminRpcServer.AdminUserInfoBase
		{
			internal AdminDeleteUserInfo50(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] auxiliaryIn) : base(AdminMethod.EcDeleteUserInfo50, callerSecurityContext, mdbGuid, AdminRpc.ExpectedDatabaseState.OnlineActive, auxiliaryIn)
			{
				this.userInfoGuid = userInfoGuid;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				UserInformation.Delete(context, this.userInfoGuid);
				return ErrorCode.NoError;
			}

			private readonly Guid userInfoGuid;
		}

		internal class AdminRpcStoreIntegrityCheck : AdminRpc
		{
			internal AdminRpcStoreIntegrityCheck(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint flags, uint[] taskIds, byte[] auxiliaryIn) : base(AdminMethod.EcAdminISIntegCheck50, callerSecurityContext, new Guid?(mdbGuid), AdminRpc.ExpectedDatabaseState.OnlineActive, auxiliaryIn)
			{
				this.mailboxGuid = mailboxGuid;
				this.flags = (IntegrityCheckRequestFlags)flags;
				this.taskIds = (from x in taskIds
				select (TaskId)x).ToArray<TaskId>();
			}

			public Guid RequestGuid
			{
				get
				{
					return this.requestGuid;
				}
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				errorCode = base.EcValidateArguments(context);
				if (errorCode != ErrorCode.NoError)
				{
					errorCode.Propagate((LID)41576U);
				}
				return errorCode;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				Properties[] array = null;
				Guid a = JobBuilder.BuildAndSchedule(context, this.mailboxGuid, this.flags, this.taskIds, null, ref array);
				if (a == Guid.Empty)
				{
					return ErrorCode.CreateInvalidParameter((LID)62556U);
				}
				this.requestGuid = a;
				return ErrorCode.NoError;
			}

			private readonly Guid mailboxGuid;

			private readonly IntegrityCheckRequestFlags flags;

			private readonly TaskId[] taskIds;

			private Guid requestGuid;
		}

		internal class AdminRpcStoreIntegrityCheckEx : AdminRpc
		{
			internal AdminRpcStoreIntegrityCheckEx(ClientSecurityContext callerSecurityContext, Guid mdbGuid, Guid mailboxGuid, uint operation, byte[] request, byte[] auxiliaryIn) : base(AdminMethod.EcAdminIntegrityCheckEx50, callerSecurityContext, new Guid?(mdbGuid), AdminRpc.ExpectedDatabaseState.OnlineActive, auxiliaryIn)
			{
				this.mailboxGuid = mailboxGuid;
				this.operation = (Operation)operation;
				this.request = request;
			}

			public byte[] Response
			{
				get
				{
					return this.response;
				}
			}

			internal override int OperationDetail
			{
				get
				{
					return (int)(this.operation + 3000U);
				}
			}

			protected override ErrorCode EcValidateArguments(MapiContext context)
			{
				ErrorCode errorCode = ErrorCode.NoError;
				errorCode = base.EcValidateArguments(context);
				if (errorCode != ErrorCode.NoError)
				{
					errorCode.Propagate((LID)40284U);
				}
				else
				{
					errorCode = AdminRpcParseFormat.ParseIntegrityCheckRequest(this.request, out this.flags, out this.requestGuid, out this.taskIds, out this.propTags);
					if (errorCode != ErrorCode.NoError)
					{
						errorCode.Propagate((LID)56668U);
					}
					else if (!Enum.IsDefined(typeof(Operation), this.operation))
					{
						errorCode = ErrorCode.CreateInvalidParameter((LID)60764U);
					}
					else if (this.operation == Operation.CreateJob && this.requestGuid != Guid.Empty)
					{
						errorCode = ErrorCode.CreateInvalidParameter((LID)36188U);
					}
				}
				return errorCode;
			}

			protected override ErrorCode EcExecuteRpc(MapiContext context)
			{
				ErrorCode noError = ErrorCode.NoError;
				this.response = null;
				switch (this.operation)
				{
				case Operation.CreateJob:
					return this.CreateIntegrityCheckJob(context);
				case Operation.QueryJob:
					return this.QueryIntegrityCheckJob(context);
				case Operation.RemoveJob:
					return this.RemoveIntegrityCheckJob(context);
				case Operation.ExecuteJob:
					return this.ExecuteIntegrityCheckJob(context);
				}
				return ErrorCode.CreateInvalidParameter((LID)46428U);
			}

			private ErrorCode CreateIntegrityCheckJob(MapiContext context)
			{
				Properties[] rows = null;
				Guid a = JobBuilder.BuildAndSchedule(context, this.mailboxGuid, (IntegrityCheckRequestFlags)this.flags, this.taskIds, this.propTags, ref rows);
				if (a == Guid.Empty)
				{
					return ErrorCode.CreateInvalidParameter((LID)37980U);
				}
				int num = 0;
				int num2 = AdminRpcParseFormat.SerializeIntegrityCheckResponse(null, ref num, 0, rows);
				this.response = new byte[num2];
				num = 0;
				AdminRpcParseFormat.SerializeIntegrityCheckResponse(this.response, ref num, num2, rows);
				return ErrorCode.NoError;
			}

			private ErrorCode QueryIntegrityCheckJob(MapiContext context)
			{
				List<Properties> list = new List<Properties>();
				IEnumerable<IntegrityCheckJob> enumerable = null;
				if (this.flags == 0)
				{
					if (this.requestGuid != Guid.Empty)
					{
						enumerable = InMemoryJobStorage.Instance(context.Database).GetJobsByRequestGuid(this.requestGuid);
					}
					else if (this.mailboxGuid != Guid.Empty)
					{
						enumerable = InMemoryJobStorage.Instance(context.Database).GetJobsByMailboxGuid(this.mailboxGuid);
					}
					else
					{
						enumerable = InMemoryJobStorage.Instance(context.Database).GetAllJobs();
					}
				}
				else if (this.flags == 1 || this.flags == 2)
				{
					JobPriority priority = JobPriority.Normal;
					RequiredMaintenanceResourceType requiredMaintenanceResourceType = RequiredMaintenanceResourceType.StoreOnlineIntegrityCheck;
					if (this.flags == 2)
					{
						priority = JobPriority.Low;
						requiredMaintenanceResourceType = RequiredMaintenanceResourceType.StoreScheduledIntegrityCheck;
					}
					AssistantActivityMonitor.Instance(context.Database).UpdateAssistantActivityState(requiredMaintenanceResourceType, true);
					enumerable = JobScheduler.Instance(context.Database).GetReadyJobs(priority);
				}
				else if (this.flags == 4)
				{
					IntegrityCheckJob job = InMemoryJobStorage.Instance(context.Database).GetJob(this.requestGuid);
					if (job != null)
					{
						enumerable = new IntegrityCheckJob[]
						{
							job
						};
					}
				}
				if (enumerable != null)
				{
					foreach (IntegrityCheckJob integrityCheckJob in enumerable)
					{
						list.Add(integrityCheckJob.GetProperties(this.propTags));
					}
				}
				if (list.Count > 0)
				{
					int num = 0;
					int num2 = AdminRpcParseFormat.SerializeIntegrityCheckResponse(null, ref num, 0, list);
					this.response = new byte[num2];
					num = 0;
					AdminRpcParseFormat.SerializeIntegrityCheckResponse(this.response, ref num, num2, list);
				}
				return ErrorCode.NoError;
			}

			private ErrorCode RemoveIntegrityCheckJob(MapiContext context)
			{
				IntegrityCheckJob job = InMemoryJobStorage.Instance(context.Database).GetJob(this.requestGuid);
				if (job != null)
				{
					JobScheduler.Instance(context.Database).RemoveJob(job);
					InMemoryJobStorage.Instance(context.Database).RemoveJob(this.requestGuid);
				}
				return ErrorCode.NoError;
			}

			private ErrorCode ExecuteIntegrityCheckJob(MapiContext context)
			{
				List<Properties> list = new List<Properties>();
				if ((this.flags & 1) != 0 || (this.flags & 2) != 0)
				{
					RequiredMaintenanceResourceType requiredMaintenanceResourceType = RequiredMaintenanceResourceType.StoreOnlineIntegrityCheck;
					if ((this.flags & 2) != 0)
					{
						requiredMaintenanceResourceType = RequiredMaintenanceResourceType.StoreScheduledIntegrityCheck;
					}
					AssistantActivityMonitor.Instance(context.Database).UpdateAssistantActivityState(requiredMaintenanceResourceType, false);
				}
				IntegrityCheckJob job = InMemoryJobStorage.Instance(context.Database).GetJob(this.requestGuid);
				if (job != null)
				{
					JobScheduler.Instance(context.Database).ExecuteJob(context, job);
					list.Add(job.GetProperties(this.propTags));
				}
				if (list.Count > 0)
				{
					int num = 0;
					int num2 = AdminRpcParseFormat.SerializeIntegrityCheckResponse(null, ref num, 0, list);
					this.response = new byte[num2];
					num = 0;
					AdminRpcParseFormat.SerializeIntegrityCheckResponse(this.response, ref num, num2, list);
				}
				return ErrorCode.NoError;
			}

			private readonly Guid mailboxGuid;

			private readonly Operation operation;

			private byte[] request;

			private byte[] response;

			private int flags;

			private Guid requestGuid;

			private TaskId[] taskIds;

			private StorePropTag[] propTags;
		}
	}
}
