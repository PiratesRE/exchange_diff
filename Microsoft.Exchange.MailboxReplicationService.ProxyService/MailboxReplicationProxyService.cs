using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.ServiceModel;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any, ConcurrencyMode = ConcurrencyMode.Multiple)]
	internal class MailboxReplicationProxyService : DisposeTrackableBase, IMailboxReplicationProxyService
	{
		static MailboxReplicationProxyService()
		{
			MailboxReplicationProxyService.activeConnectionsUpdateLock = new object();
			ServerThrottlingResource.InitializeServerThrottlingObjects(false);
		}

		public MailboxReplicationProxyService()
		{
			ADSession.DisableAdminTopologyMode();
			this.handleCache = new HandleCache();
			MrsTracer.ProxyService.Debug("MailboxReplicationProxyService instance created.", new object[0]);
			this.clientVersion = null;
			this.proxyControlFlags = ProxyControlFlags.None;
		}

		public VersionInformation ClientVersion
		{
			get
			{
				return this.clientVersion;
			}
		}

		public bool UseCompression
		{
			get
			{
				return !this.proxyControlFlags.HasFlag(ProxyControlFlags.DoNotCompress);
			}
		}

		public bool UseBufferring
		{
			get
			{
				return !this.proxyControlFlags.HasFlag(ProxyControlFlags.DoNotBuffer);
			}
		}

		public int ExportBufferSizeFromMrsKB { get; private set; }

		public bool SkipWLMThrottling
		{
			get
			{
				return this.proxyControlFlags.HasFlag(ProxyControlFlags.SkipWLMThrottling) || this.proxyControlFlags.HasFlag(ProxyControlFlags.DoNotApplyProxyThrottling);
			}
		}

		public Guid ExchangeGuid { get; private set; }

		public bool IsE15OrHigher { get; private set; }

		public bool IsHighPriority { get; private set; }

		public bool IsInFinalization { get; private set; }

		private bool IsThrottled
		{
			get
			{
				return !this.proxyControlFlags.HasFlag(ProxyControlFlags.DoNotApplyProxyThrottling);
			}
		}

		public string DecompressString(byte[] data)
		{
			return CommonUtils.UnpackString(data, this.UseCompression);
		}

		public byte[] CompressString(string str)
		{
			return CommonUtils.PackString(str, this.UseCompression);
		}

		void IMailboxReplicationProxyService.ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ExchangeVersionInformation", new object[0]);
			serverVersion = VersionInformation.MRSProxy;
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.ThrottlingNotRequired | ExecutionFlags.NoLock, DelayScopeKind.NoDelay, delegate(object o)
			{
				this.clientVersion = clientVersion;
				if (!clientVersion[24])
				{
					MrsTracer.ProxyService.Error("Client does not support TenantHint.", new object[0]);
					throw new UnsupportedClientVersionPermanentException(clientVersion.ComputerName, clientVersion.ToString(), "TenantHint");
				}
			});
		}

		ProxyServerInformation IMailboxReplicationProxyService.FindServerByDatabaseOrMailbox(string databaseId, Guid? physicalMailboxGuid, Guid? primaryMailboxGuid, byte[] tenantPartitionHintBytes)
		{
			MrsTracer.ProxyService.Function("MRSProxy.FindServerByDatabaseOrMailbox", new object[0]);
			ProxyServerInformation result = null;
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.ThrottlingNotRequired | ExecutionFlags.NoLock, DelayScopeKind.NoDelay, delegate(object o)
			{
				ProxyServerSettings proxyServerSettings;
				if (databaseId != null)
				{
					Guid guid;
					if (!Guid.TryParse(databaseId, out guid))
					{
						throw new UnexpectedErrorPermanentException(-2147024809);
					}
					proxyServerSettings = CommonUtils.MapDatabaseToProxyServer(new ADObjectId(guid, PartitionId.LocalForest.ForestFQDN));
				}
				else
				{
					proxyServerSettings = CommonUtils.MapMailboxToProxyServer(physicalMailboxGuid, primaryMailboxGuid, tenantPartitionHintBytes);
				}
				result = new ProxyServerInformation(proxyServerSettings.Fqdn, proxyServerSettings.IsProxyLocal);
			});
			return result;
		}

		IEnumerable<ContainerMailboxInformation> IMailboxReplicationProxyService.GetMailboxContainerMailboxes(Guid mdbGuid, Guid primaryMailboxGuid)
		{
			MrsTracer.ProxyService.Function("MRSProxy.GetMailboxContainerMailboxes({0}, {1})", new object[]
			{
				mdbGuid,
				primaryMailboxGuid
			});
			List<ContainerMailboxInformation> containerMailboxes = null;
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.ThrottlingNotRequired | ExecutionFlags.NoLock, DelayScopeKind.NoDelay, delegate(object o)
			{
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=MSExchangeMigration", null, null, null, null))
				{
					Guid[] array = null;
					PropValue[][] mailboxTableInfo = exRpcAdmin.GetMailboxTableInfo(mdbGuid, primaryMailboxGuid, new PropTag[]
					{
						PropTag.MailboxPartitionMailboxGuids
					});
					if (mailboxTableInfo != null)
					{
						foreach (PropValue[] array3 in mailboxTableInfo)
						{
							if (array3 != null)
							{
								array = array3[0].GetGuidArray();
							}
						}
						if (array != null)
						{
							containerMailboxes = new List<ContainerMailboxInformation>();
							foreach (Guid guid in array)
							{
								byte[] persistableTenantPartitionHint = null;
								mailboxTableInfo = exRpcAdmin.GetMailboxTableInfo(mdbGuid, guid, new PropTag[]
								{
									PropTag.PersistableTenantPartitionHint
								});
								if (mailboxTableInfo != null)
								{
									foreach (PropValue[] array6 in mailboxTableInfo)
									{
										if (array6 != null)
										{
											persistableTenantPartitionHint = array6[0].GetBytes();
										}
									}
								}
								containerMailboxes.Add(new ContainerMailboxInformation
								{
									MailboxGuid = guid,
									PersistableTenantPartitionHint = persistableTenantPartitionHint
								});
							}
						}
					}
				}
			});
			return containerMailboxes;
		}

		bool IMailboxReplicationProxyService.ArePublicFoldersReadyForMigrationCompletion()
		{
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.PublicFolderMailboxesMigration.Enabled)
			{
				throw new NotImplementedException();
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId), 469, "ArePublicFoldersReadyForMigrationCompletion", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\ProxyService\\MRSProxyService.cs");
			Organization orgContainer = tenantOrTopologyConfigurationSession.GetOrgContainer();
			if (!orgContainer.Heuristics.HasFlag(HeuristicsFlags.PublicFolderMailboxesLockedForNewConnections))
			{
				return false;
			}
			List<MoveRequest> moveRequests = CommonUtils.GetMoveRequests();
			if (moveRequests != null && moveRequests.Any<MoveRequest>())
			{
				foreach (MoveRequest moveRequest in moveRequests)
				{
					if (moveRequest.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
					{
						return false;
					}
				}
			}
			List<PublicFolderMoveRequest> publicFolderMoveRequests = CommonUtils.GetPublicFolderMoveRequests();
			return publicFolderMoveRequests == null || !publicFolderMoveRequests.Any<PublicFolderMoveRequest>();
		}

		List<Guid> IMailboxReplicationProxyService.GetPublicFolderMailboxesExchangeGuids()
		{
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.PublicFolderMailboxesMigration.Enabled)
			{
				throw new NotImplementedException();
			}
			List<Mailbox> publicFolderMailboxes = CommonUtils.GetPublicFolderMailboxes();
			List<Guid> list = new List<Guid>();
			if (publicFolderMailboxes != null && publicFolderMailboxes.Any<Mailbox>())
			{
				foreach (Mailbox mailbox in publicFolderMailboxes)
				{
					list.Add(mailbox.ExchangeGuid);
				}
			}
			return list;
		}

		Guid IMailboxReplicationProxyService.IReservationManager_ReserveResources(Guid mailboxGuid, byte[] partitionHintBytes, Guid mdbGuid, int flags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IReservationManager_ReserveResources({0}, {1}, {2})", new object[]
			{
				mailboxGuid,
				mdbGuid,
				flags
			});
			ReservationBase reservation = null;
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(object o)
			{
				TenantPartitionHint partitionHint = (partitionHintBytes != null) ? TenantPartitionHint.FromPersistablePartitionHint(partitionHintBytes) : null;
				reservation = ReservationManager.CreateReservation(mailboxGuid, partitionHint, mdbGuid, (ReservationFlags)flags, this.ClientVersion.ComputerName);
			});
			return reservation.ReservationId;
		}

		void IMailboxReplicationProxyService.IReservationManager_ReleaseResources(Guid reservationId)
		{
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(object o)
			{
				try
				{
					ReservationBase reservationBase = ReservationManager.FindReservation(reservationId);
					if (reservationBase != null)
					{
						reservationBase.Dispose();
					}
				}
				catch (ExpiredReservationException)
				{
				}
			});
		}

		long IMailboxReplicationProxyService.IMailbox_Config3(Guid primaryMailboxGuid, Guid physicalMailboxGuid, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_Config3({0})", new object[]
			{
				primaryMailboxGuid
			});
			LocalMailboxFlags localMailboxFlags = LocalMailboxFlags.None;
			if (mbxType == MailboxType.SourceMailbox && (proxyControlFlags & 8) != 0)
			{
				localMailboxFlags |= LocalMailboxFlags.StripLargeRulesForDownlevelTargets;
				proxyControlFlags &= -9;
			}
			return ((IMailboxReplicationProxyService)this).IMailbox_Config4(primaryMailboxGuid, physicalMailboxGuid, null, mdbGuid, mdbName, mbxType, proxyControlFlags, (int)localMailboxFlags);
		}

		long IMailboxReplicationProxyService.IMailbox_Config4(Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHintBytes, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMbxFlags)
		{
			return ((IMailboxReplicationProxyService)this).IMailbox_Config5(Guid.Empty, primaryMailboxGuid, physicalMailboxGuid, partitionHintBytes, mdbGuid, mdbName, mbxType, proxyControlFlags, localMbxFlags);
		}

		long IMailboxReplicationProxyService.IMailbox_Config5(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHintBytes, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMbxFlags)
		{
			return ((IMailboxReplicationProxyService)this).IMailbox_Config6(reservationId, primaryMailboxGuid, physicalMailboxGuid, null, partitionHintBytes, mdbGuid, mdbName, mbxType, proxyControlFlags, localMbxFlags);
		}

		long IMailboxReplicationProxyService.IMailbox_Config6(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, string filePath, byte[] partitionHintBytes, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMbxFlags)
		{
			return this.Config(reservationId, primaryMailboxGuid, physicalMailboxGuid, filePath, partitionHintBytes, mdbGuid, mdbName, mbxType, proxyControlFlags, localMbxFlags, null);
		}

		long IMailboxReplicationProxyService.IMailbox_Config7(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHintBytes, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMbxFlags, Guid? mailboxContainerGuid)
		{
			return this.Config(reservationId, primaryMailboxGuid, physicalMailboxGuid, null, partitionHintBytes, mdbGuid, mdbName, mbxType, proxyControlFlags, localMbxFlags, mailboxContainerGuid);
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigureProxyService(ProxyConfiguration configuration)
		{
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(object o)
			{
				this.ExportBufferSizeFromMrsKB = configuration.ExportBufferSizeKB;
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigADConnection(long mailboxHandle, string domainControllerName, string userName, string userDomain, string userPassword)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ConfigADConnection(domainControllerName={0}, userName={1}, userDomain={2}, pwd))", new object[]
			{
				domainControllerName,
				userName,
				userDomain
			});
			NetworkCredential cred = null;
			if (userName != null || userDomain != null || userPassword != null)
			{
				cred = new NetworkCredential(userName, userDomain, userPassword);
			}
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				mbx.ConfigADConnection(domainControllerName, domainControllerName, cred);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigPst(long mailboxHandle, string filePath, int? contentCodePage)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ConfigPst({0}, {1}, {2})", new object[]
			{
				mailboxHandle,
				filePath,
				contentCodePage
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				mbx.ConfigPst(filePath, contentCodePage);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigEas(long mailboxHandle, string password, string address)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ConfigEas({0}, {1})", new object[]
			{
				mailboxHandle,
				address
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				NetworkCredential userCredential = new NetworkCredential(address, password);
				SmtpAddress smtpAddress = new SmtpAddress(address);
				mbx.ConfigEas(userCredential, smtpAddress, Guid.Empty, null);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigEas2(long mailboxHandle, string password, string address, Guid mailboxGuid, string remoteHostName)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ConfigEas2({0}, {1}, {2}, {3})", new object[]
			{
				mailboxHandle,
				address,
				mailboxGuid,
				remoteHostName
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				NetworkCredential userCredential = new NetworkCredential(address, password);
				SmtpAddress smtpAddress = new SmtpAddress(address);
				mbx.ConfigEas(userCredential, smtpAddress, mailboxGuid, remoteHostName);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigRestore(long mailboxHandle, int restoreFlags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ConfigRestore({0}, 0x{1:x})", new object[]
			{
				mailboxHandle,
				restoreFlags
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				mbx.ConfigRestore((MailboxRestoreType)restoreFlags);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigOlc(long mailboxHandle, OlcMailboxConfiguration config)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ConfigOlc({0}, {1})", new object[]
			{
				mailboxHandle,
				config.ToString()
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				mbx.ConfigOlc(config);
			});
		}

		int IMailboxReplicationProxyService.IMailbox_ReserveResources(Guid reservationId, Guid resourceId, int reservationType)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ReserveResources", new object[0]);
			throw new UnexpectedErrorPermanentException(-2147024809);
		}

		void IMailboxReplicationProxyService.CloseHandle(long handle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.CloseHandle({0})", new object[]
			{
				handle
			});
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.NoDelay, delegate(object o)
			{
				this.handleCache.ReleaseObject(handle);
			});
		}

		void IMailboxReplicationProxyService.DataExport_CancelExport(long dataExportHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.DataExport_CancelExport({0})", new object[]
			{
				dataExportHandle
			});
			this.ExecuteServiceCall<IDataExport>(dataExportHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IDataExport dataExport)
			{
				dataExport.CancelExport();
				((IMailboxReplicationProxyService)this).CloseHandle(dataExportHandle);
			});
		}

		DataExportBatch IMailboxReplicationProxyService.DataExport_ExportData2(long dataExportHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.DataExport_ExportData({0})", new object[]
			{
				dataExportHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<IDataExport>(dataExportHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IDataExport dataExport)
			{
				result = dataExport.ExportData();
				if (result.IsLastBatch)
				{
					((IMailboxReplicationProxyService)this).CloseHandle(dataExportHandle);
				}
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDataImport_Flush(long dataImportHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDataImport_Flush({0})", new object[]
			{
				dataImportHandle
			});
			this.ExecuteServiceCall<IDataImport>(dataImportHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDataImport dataImport)
			{
				dataImport.SendMessageAndWaitForReply(FlushMessage.Instance);
			});
		}

		void IMailboxReplicationProxyService.IDataImport_ImportBuffer(long dataImportHandle, int opcode, byte[] data)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDataImport_ImportBuffer({0}, opcode={1}, data={2})", new object[]
			{
				dataImportHandle,
				opcode,
				TraceUtils.DumpArray(data)
			});
			this.ExecuteServiceCall<IDataImport>(dataImportHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDataImport dataImport)
			{
				IDataMessage message = DataMessageSerializer.Deserialize(opcode, data, this.UseCompression);
				dataImport.SendMessage(message);
			});
		}

		long IMailboxReplicationProxyService.IDestinationFolder_GetFxProxy(long folderHandle, out byte[] objectData)
		{
			return ((IMailboxReplicationProxyService)this).IDestinationFolder_GetFxProxy2(folderHandle, 0, out objectData);
		}

		long IMailboxReplicationProxyService.IDestinationFolder_GetFxProxy2(long folderHandle, int flags, out byte[] objectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_GetFxProxy({0})", new object[]
			{
				folderHandle
			});
			long result = -1L;
			byte[] objData = null;
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IDestinationFolder folder)
			{
				IFxProxy fxProxy = folder.GetFxProxy((FastTransferFlags)flags);
				IDataImport destination = new FxProxyReceiver(fxProxy, true);
				objData = fxProxy.GetObjectData();
				BufferedReceiver obj = new BufferedReceiver(destination, true, this.UseBufferring, this.UseCompression);
				result = this.handleCache.AddObject(obj, folderHandle);
			});
			objectData = objData;
			return result;
		}

		PropProblemData[] IMailboxReplicationProxyService.IDestinationFolder_SetProps(long folderHandle, PropValueData[] pva)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetProps({0})", new object[]
			{
				folderHandle
			});
			return ((IMailboxReplicationProxyService)this).IFolder_SetProps(folderHandle, pva);
		}

		bool IMailboxReplicationProxyService.IDestinationFolder_SetSearchCriteria(long folderHandle, RestrictionData restriction, byte[][] entryIDs, int searchFlags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetSearchCriteria({0})", new object[]
			{
				folderHandle
			});
			bool result = false;
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationFolder folder)
			{
				result = folder.SetSearchCriteria(restriction, entryIDs, (SearchCriteriaFlags)searchFlags);
			});
			return result;
		}

		PropProblemData[] IMailboxReplicationProxyService.IDestinationFolder_SetSecurityDescriptor(long folderHandle, int secProp, byte[] sdData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetSecurityDescriptor({0})", new object[]
			{
				folderHandle
			});
			PropProblemData[] result = null;
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationFolder folder)
			{
				RawSecurityDescriptor sd;
				if (sdData != null)
				{
					sd = new RawSecurityDescriptor(sdData, 0);
				}
				else
				{
					sd = null;
				}
				result = folder.SetSecurityDescriptor((SecurityProp)secProp, sd);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationFolder_DeleteMessages(long folderHandle, byte[][] entryIds)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_DeleteMessages({0}, {1})", new object[]
			{
				folderHandle,
				(entryIds == null) ? 0 : entryIds.Length
			});
			((IMailboxReplicationProxyService)this).IFolder_DeleteMessages(folderHandle, entryIds);
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetReadFlagsOnMessages(long folderHandle, int flags, byte[][] entryIds)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetReadFlagsOnMessages({0})", new object[]
			{
				folderHandle
			});
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationFolder folder)
			{
				folder.SetReadFlagsOnMessages((SetReadFlags)flags, entryIds);
			});
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetMessageProps(long folderHandle, byte[] entryId, PropValueData[] propValues)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetMessageProps({0})", new object[]
			{
				folderHandle
			});
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationFolder folder)
			{
				folder.SetMessageProps(entryId, propValues);
			});
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_CreateFolder(long mailboxHandle, FolderRec sourceFolder, bool failIfExists)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_CreateFolder({0})", new object[]
			{
				mailboxHandle
			});
			byte[] array;
			((IMailboxReplicationProxyService)this).IDestinationMailbox_CreateFolder2(mailboxHandle, sourceFolder, failIfExists, out array);
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_CreateFolder2(long mailboxHandle, FolderRec sourceFolder, bool failIfExists, out byte[] newFolderId)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_CreateFolder2({0})", new object[]
			{
				mailboxHandle
			});
			CreateFolderFlags createFolderFlags = failIfExists ? CreateFolderFlags.FailIfExists : CreateFolderFlags.None;
			((IMailboxReplicationProxyService)this).IDestinationMailbox_CreateFolder3(mailboxHandle, sourceFolder, (int)createFolderFlags, out newFolderId);
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_CreateFolder3(long mailboxHandle, FolderRec sourceFolder, int createFolderFlags, out byte[] newFolderId)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_CreateFolder2({0})", new object[]
			{
				mailboxHandle
			});
			byte[] newFolderIdInt = null;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				mbx.CreateFolder(sourceFolder, (CreateFolderFlags)createFolderFlags, out newFolderIdInt);
			});
			newFolderId = newFolderIdInt;
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetRules(long folderHandle, RuleData[] rules)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetRules({0})", new object[]
			{
				folderHandle
			});
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationFolder folder)
			{
				folder.SetRules(rules);
			});
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetACL(long folderHandle, int secProp, PropValueData[][] aclData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetACL({0})", new object[]
			{
				folderHandle
			});
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationFolder folder)
			{
				folder.SetACL((SecurityProp)secProp, aclData);
			});
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetExtendedAcl(long folderHandle, int aclFlags, PropValueData[][] aclData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetExtendedAcl({0})", new object[]
			{
				folderHandle
			});
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationFolder folder)
			{
				folder.SetExtendedAcl((AclFlags)aclFlags, aclData);
			});
		}

		Guid IMailboxReplicationProxyService.IDestinationFolder_LinkMailPublicFolder(long folderHandle, LinkMailPublicFolderFlags flags, byte[] objectId)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationFolder_SetReadFlagsOnMessages({0})", new object[]
			{
				folderHandle
			});
			Guid objectGuid = Guid.Empty;
			this.ExecuteServiceCall<IDestinationFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IDestinationFolder folder)
			{
				objectGuid = folder.LinkMailPublicFolder(flags, objectId);
			});
			return objectGuid;
		}

		CreateMailboxResult IMailboxReplicationProxyService.IDestinationMailbox_CreateMailbox(long mailboxHandle, byte[] mailboxData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_CreateMailbox({0})", new object[]
			{
				mailboxHandle
			});
			CreateMailboxResult result = CreateMailboxResult.Success;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				result = mbx.CreateMailbox(mailboxData, MailboxSignatureFlags.GetLegacy);
			});
			return result;
		}

		CreateMailboxResult IMailboxReplicationProxyService.IDestinationMailbox_CreateMailbox2(long mailboxHandle, byte[] mailboxData, int sourceSignatureFlags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_CreateMailbox2({0})", new object[]
			{
				mailboxHandle
			});
			CreateMailboxResult result = CreateMailboxResult.Success;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				result = mbx.CreateMailbox(mailboxData, (MailboxSignatureFlags)sourceSignatureFlags);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_ProcessMailboxSignature(long mailboxHandle, byte[] mailboxData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_ProcessMailboxSignature({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				mbx.ProcessMailboxSignature(mailboxData);
			});
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_DeleteFolder(long mailboxHandle, FolderRec folderRec)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_DeleteFolder({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				mbx.DeleteFolder(folderRec);
			});
		}

		long IMailboxReplicationProxyService.IDestinationMailbox_GetFolder(long mailboxHandle, byte[] entryId)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_GetFolder({0})", new object[]
			{
				mailboxHandle
			});
			long result = -1L;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IDestinationMailbox mbx)
			{
				IDestinationFolder folder = mbx.GetFolder(entryId);
				if (folder != null)
				{
					result = this.handleCache.AddObject(folder, mailboxHandle);
					return;
				}
				result = 0L;
			});
			return result;
		}

		long IMailboxReplicationProxyService.IDestinationMailbox_GetFxProxy(long mailboxHandle, out byte[] objectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_GetFxProxy({0})", new object[]
			{
				mailboxHandle
			});
			long result = -1L;
			byte[] objData = null;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				IFxProxy fxProxy = mbx.GetFxProxy();
				IDataImport destination = new FxProxyReceiver(fxProxy, true);
				objData = fxProxy.GetObjectData();
				BufferedReceiver obj = new BufferedReceiver(destination, true, this.UseBufferring, this.UseCompression);
				result = this.handleCache.AddObject(obj, mailboxHandle);
			});
			objectData = objData;
			return result;
		}

		long IMailboxReplicationProxyService.IDestinationMailbox_GetFxProxyPool(long mailboxHandle, byte[][] folderIds, out byte[] objectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_GetFxProxyPool({0})", new object[]
			{
				mailboxHandle
			});
			long result = -1L;
			byte[] objData = null;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				IFxProxyPool fxProxyPool = mbx.GetFxProxyPool(folderIds);
				IDataImport dataImport = new FxProxyPoolReceiver(fxProxyPool, true);
				IDataMessage dataMessage = dataImport.SendMessageAndWaitForReply(FxProxyPoolGetFolderDataRequestMessage.Instance);
				DataMessageOpcode dataMessageOpcode;
				dataMessage.Serialize(this.UseCompression, out dataMessageOpcode, out objData);
				BufferedReceiver obj = new BufferedReceiver(dataImport, true, this.UseBufferring, this.UseCompression);
				result = this.handleCache.AddObject(obj, mailboxHandle);
			});
			objectData = objData;
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.IDestinationMailbox_LoadSyncState2(long mailboxHandle, byte[] key)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_LoadSyncState2({0})", new object[]
			{
				mailboxHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.NoDelay, delegate(IDestinationMailbox mbx)
			{
				string data = mbx.LoadSyncState(key);
				IDataExport dataExport = new PagedTransmitter(data, this.UseCompression);
				result = dataExport.ExportData();
				if (!result.IsLastBatch)
				{
					result.DataExportHandle = this.handleCache.AddObject(dataExport, mailboxHandle);
				}
			});
			return result;
		}

		long IMailboxReplicationProxyService.IDestinationMailbox_SaveSyncState2(long mailboxHandle, byte[] key, DataExportBatch firstBatch)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_GetMailboxSyncState({0})", new object[]
			{
				mailboxHandle
			});
			long result = -1L;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.NoDelay, delegate(IDestinationMailbox mbx)
			{
				IDataImport dataImport = new PagedReceiver(delegate(string str)
				{
					mbx.SaveSyncState(key, str);
				}, this.UseCompression);
				try
				{
					IDataMessage message = DataMessageSerializer.Deserialize(firstBatch.Opcode, firstBatch.Data, this.UseCompression);
					dataImport.SendMessage(message);
					if (!firstBatch.IsLastBatch)
					{
						result = this.handleCache.AddObject(dataImport, mailboxHandle);
						dataImport = null;
					}
					else
					{
						result = 0L;
					}
				}
				finally
				{
					if (dataImport != null)
					{
						dataImport.Dispose();
					}
				}
			});
			return result;
		}

		bool IMailboxReplicationProxyService.IDestinationMailbox_MailboxExists(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_MailboxExists({0})", new object[]
			{
				mailboxHandle
			});
			bool result = false;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IDestinationMailbox mbx)
			{
				result = mbx.MailboxExists();
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_MoveFolder(long mailboxHandle, byte[] folderId, byte[] oldParentId, byte[] newParentId)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_MoveFolder({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				mbx.MoveFolder(folderId, oldParentId, newParentId);
			});
		}

		PropProblemData[] IMailboxReplicationProxyService.IDestinationMailbox_SetProps(long mailboxHandle, PropValueData[] pva)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_SetProps({0})", new object[]
			{
				mailboxHandle
			});
			PropProblemData[] result = null;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				result = mbx.SetProps(pva);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_SetMailboxSecurityDescriptor(long mailboxHandle, byte[] sdData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_SetMailboxSecurityDescriptor({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				if (sdData != null)
				{
					RawSecurityDescriptor mailboxSecurityDescriptor = new RawSecurityDescriptor(sdData, 0);
					mbx.SetMailboxSecurityDescriptor(mailboxSecurityDescriptor);
				}
			});
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_SetUserSecurityDescriptor(long mailboxHandle, byte[] sdData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_SetUserSecurityDescriptor({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IDestinationMailbox mbx)
			{
				if (sdData != null)
				{
					RawSecurityDescriptor userSecurityDescriptor = new RawSecurityDescriptor(sdData, 0);
					mbx.SetUserSecurityDescriptor(userSecurityDescriptor);
				}
			});
		}

		List<MessageRec> IMailboxReplicationProxyService.IFolder_EnumerateMessagesPaged2(long folderHandle, EnumerateMessagesFlags emFlags, int[] additionalPtagsToLoad, out bool moreData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_EnumerateMessagesPaged2({0})", new object[]
			{
				folderHandle
			});
			this.enumerateMessagesRemainingData = null;
			this.enumerateMessagesFolder = -1L;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				this.enumerateMessagesRemainingData = folder.EnumerateMessages(emFlags, DataConverter<PropTagConverter, PropTag, int>.GetNative(additionalPtagsToLoad));
				this.enumerateMessagesFolder = folderHandle;
			});
			if (this.enumerateMessagesRemainingData != null)
			{
				foreach (MessageRec messageRec in this.enumerateMessagesRemainingData)
				{
					messageRec.FolderId = null;
				}
			}
			return ((IMailboxReplicationProxyService)this).IFolder_EnumerateMessagesNextBatch(folderHandle, out moreData);
		}

		List<MessageRec> IMailboxReplicationProxyService.IFolder_EnumerateMessagesNextBatch(long folderHandle, out bool moreData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_EnumerateMessagesNextBatch({0})", new object[]
			{
				folderHandle
			});
			List<MessageRec> result = null;
			bool moreDataTemp = false;
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(object o)
			{
				if (folderHandle != this.enumerateMessagesFolder)
				{
					MrsTracer.ProxyService.Warning("EnumerateMessagesNextBatch is called on the wrong folder handle", new object[0]);
					moreDataTemp = false;
					result = null;
					return;
				}
				int num = 1000;
				result = CommonUtils.ExtractBatch<MessageRec>(ref this.enumerateMessagesRemainingData, ref num, out moreDataTemp);
				if (!moreDataTemp)
				{
					this.enumerateMessagesFolder = -1L;
				}
			});
			moreData = moreDataTemp;
			return result;
		}

		FolderRec IMailboxReplicationProxyService.IFolder_GetFolderRec2(long folderHandle, int[] additionalPtagsToLoad)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_GetFolderRec2({0})", new object[]
			{
				folderHandle
			});
			return ((IMailboxReplicationProxyService)this).IFolder_GetFolderRec3(folderHandle, additionalPtagsToLoad, 0);
		}

		FolderRec IMailboxReplicationProxyService.IFolder_GetFolderRec3(long folderHandle, int[] additionalPtagsToLoad, int flags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_GetFolderRec({0})", new object[]
			{
				folderHandle
			});
			FolderRec result = null;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				result = folder.GetFolderRec(DataConverter<PropTagConverter, PropTag, int>.GetNative(additionalPtagsToLoad), (GetFolderRecFlags)flags);
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IFolder_GetSecurityDescriptor(long folderHandle, int secProp)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_GetSecurityDescriptor({0})", new object[]
			{
				folderHandle
			});
			byte[] result = null;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				RawSecurityDescriptor securityDescriptor = folder.GetSecurityDescriptor((SecurityProp)secProp);
				byte[] array;
				if (securityDescriptor != null)
				{
					array = new byte[securityDescriptor.BinaryLength];
					securityDescriptor.GetBinaryForm(array, 0);
				}
				else
				{
					array = null;
				}
				result = array;
			});
			return result;
		}

		void IMailboxReplicationProxyService.IFolder_SetContentsRestriction(long folderHandle, RestrictionData restriction)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_SetContentsRestriction({0})", new object[]
			{
				folderHandle
			});
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				folder.SetContentsRestriction(restriction);
			});
		}

		PropValueData[] IMailboxReplicationProxyService.IFolder_GetProps(long folderHandle, int[] pta)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_GetProps({0})", new object[]
			{
				folderHandle
			});
			PropValueData[] result = null;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				result = folder.GetProps(DataConverter<PropTagConverter, PropTag, int>.GetNative(pta));
				DiagnosticContext currentContext = DiagnosticContext.GetCurrentContext();
				if (currentContext != null)
				{
					this.mapiDiagnosticGetProp = currentContext.ToString();
				}
			});
			return result;
		}

		PropProblemData[] IMailboxReplicationProxyService.IFolder_SetProps(long folderHandle, PropValueData[] pva)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_SetProps({0})", new object[]
			{
				folderHandle
			});
			PropProblemData[] result = null;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IFolder folder)
			{
				result = folder.SetProps(pva);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IFolder_DeleteMessages(long folderHandle, byte[][] entryIds)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_DeleteMessages({0})", new object[]
			{
				folderHandle
			});
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IFolder folder)
			{
				folder.DeleteMessages(entryIds);
			});
		}

		RuleData[] IMailboxReplicationProxyService.IFolder_GetRules(long folderHandle, int[] extraProps)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_GetRules({0})", new object[]
			{
				folderHandle
			});
			RuleData[] result = null;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				result = folder.GetRules(DataConverter<PropTagConverter, PropTag, int>.GetNative(extraProps));
				if (!this.ClientVersion[32])
				{
					RuleData.ConvertRulesToDownlevel(result);
				}
			});
			return result;
		}

		PropValueData[][] IMailboxReplicationProxyService.IFolder_GetACL(long folderHandle, int secProp)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_GetACL({0})", new object[]
			{
				folderHandle
			});
			PropValueData[][] result = null;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				result = folder.GetACL((SecurityProp)secProp);
			});
			return result;
		}

		PropValueData[][] IMailboxReplicationProxyService.IFolder_GetExtendedAcl(long folderHandle, int aclFlags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_GetExtendedAcl({0})", new object[]
			{
				folderHandle
			});
			PropValueData[][] result = null;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				result = folder.GetExtendedAcl((AclFlags)aclFlags);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IFolder_GetSearchCriteria(long folderHandle, out RestrictionData restriction, out byte[][] entryIDs, out int searchState)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_GetSearchCriteria({0})", new object[]
			{
				folderHandle
			});
			RestrictionData rd = null;
			byte[][] eids = null;
			SearchState ss = SearchState.None;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				folder.GetSearchCriteria(out rd, out eids, out ss);
			});
			restriction = rd;
			entryIDs = eids;
			searchState = (int)ss;
		}

		List<MessageRec> IMailboxReplicationProxyService.IFolder_LookupMessages(long folderHandle, int ptagToLookup, byte[][] keysToLookup, int[] additionalPtagsToLoad)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IFolder_LookupMessages({0})", new object[]
			{
				folderHandle
			});
			List<MessageRec> result = null;
			this.ExecuteServiceCall<IFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IFolder folder)
			{
				result = folder.LookupMessages(DataConverter<PropTagConverter, PropTag, int>.GetNative(ptagToLookup), new List<byte[]>(keysToLookup), DataConverter<PropTagConverter, PropTag, int>.GetNative(additionalPtagsToLoad));
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_Connect(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_Connect({0})", new object[]
			{
				mailboxHandle
			});
			((IMailboxReplicationProxyService)this).IMailbox_Connect2(mailboxHandle, 0);
		}

		void IMailboxReplicationProxyService.IMailbox_Connect2(long mailboxHandle, int connectFlags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_Connect({0})", new object[]
			{
				mailboxHandle
			});
			this.IncrementConnectionCount();
			this.IsHighPriority = ((connectFlags & 16) != 0);
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				mbx.Connect((MailboxConnectFlags)connectFlags);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_DeleteMailbox(long mailboxHandle, int flags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_DeleteMailbox({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IMailbox mbx)
			{
				mbx.DeleteMailbox(flags);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_Disconnect(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_Disconnect({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.NoDelay, delegate(IMailbox mbx)
			{
				this.DisconnectMailboxSession(mailboxHandle);
			});
			this.DecrementConnectionCount();
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigMailboxOptions(long mailboxHandle, int options)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ConfigMailboxOptions({0}, {1})", new object[]
			{
				mailboxHandle,
				options
			});
			this.IsInFinalization = ((MailboxOptions)options).HasFlag(MailboxOptions.Finalize);
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				mbx.ConfigMailboxOptions((MailboxOptions)options);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigPreferredADConnection(long mailboxHandle, string preferredDomainControllerName)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ConfigPreferredADConnection(preferredDomainControllerName={0})", new object[]
			{
				preferredDomainControllerName
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				mbx.ConfigPreferredADConnection(preferredDomainControllerName);
			});
		}

		MailboxServerInformation IMailboxReplicationProxyService.IMailbox_GetMailboxServerInformation(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetMailboxServerInformation({0})", new object[]
			{
				mailboxHandle
			});
			MailboxServerInformation result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				result = mbx.GetMailboxServerInformation();
				if (result == null)
				{
					result = new MailboxServerInformation();
					Server server = LocalServer.GetServer();
					result.MailboxServerName = server.Name;
					result.MailboxServerVersion = server.VersionNumber;
					result.MailboxServerGuid = server.Guid;
				}
				result.ProxyServerName = CommonUtils.LocalComputerName;
				result.ProxyServerVersion = VersionInformation.MRSProxy;
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_SetOtherSideVersion(long mailboxHandle, VersionInformation otherSideInfo)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_SetOtherSideMailboxServerInformation({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				mbx.SetOtherSideVersion(otherSideInfo);
			});
		}

		List<FolderRec> IMailboxReplicationProxyService.IMailbox_EnumerateFolderHierarchyPaged2(long mailboxHandle, EnumerateFolderHierarchyFlags flags, int[] additionalPtagsToLoad, out bool moreData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_EnumerateFolderHierarchyPaged2({0})", new object[]
			{
				mailboxHandle
			});
			this.enumerateFoldersRemainingData = null;
			this.enumerateFoldersMailbox = -1L;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				this.enumerateFoldersRemainingData = mbx.EnumerateFolderHierarchy(flags, DataConverter<PropTagConverter, PropTag, int>.GetNative(additionalPtagsToLoad));
				this.enumerateFoldersMailbox = mailboxHandle;
			});
			return ((IMailboxReplicationProxyService)this).IMailbox_EnumerateFolderHierarchyNextBatch(mailboxHandle, out moreData);
		}

		List<FolderRec> IMailboxReplicationProxyService.IMailbox_EnumerateFolderHierarchyNextBatch(long mailboxHandle, out bool moreData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_EnumerateFolderHierarchyNextBatch({0})", new object[]
			{
				mailboxHandle
			});
			List<FolderRec> result = null;
			bool moreDataInt = false;
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(object o)
			{
				if (mailboxHandle != this.enumerateFoldersMailbox)
				{
					MrsTracer.ProxyService.Warning("EnumerateFolderHierarchyNextBatch is called on the wrong mailbox handle", new object[0]);
					return;
				}
				int num = 100;
				result = CommonUtils.ExtractBatch<FolderRec>(ref this.enumerateFoldersRemainingData, ref num, out moreDataInt);
				if (!moreDataInt)
				{
					this.enumerateFoldersMailbox = -1L;
				}
			});
			moreData = moreDataInt;
			return result;
		}

		List<WellKnownFolder> IMailboxReplicationProxyService.IMailbox_DiscoverWellKnownFolders(long mailboxHandle, int flags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_DiscoverWellKnownFolders({0})", new object[]
			{
				mailboxHandle
			});
			List<WellKnownFolder> result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				result = mbx.DiscoverWellKnownFolders(flags);
			});
			return result;
		}

		bool IMailboxReplicationProxyService.IMailbox_IsMailboxCapabilitySupported(long mailboxHandle, MailboxCapabilities capability)
		{
			return ((IMailboxReplicationProxyService)this).IMailbox_IsMailboxCapabilitySupported2(mailboxHandle, (int)capability);
		}

		bool IMailboxReplicationProxyService.IMailbox_IsMailboxCapabilitySupported2(long mailboxHandle, int capability)
		{
			bool result = false;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				result = mbx.IsMailboxCapabilitySupported((MailboxCapabilities)capability);
			});
			return result;
		}

		int[] IMailboxReplicationProxyService.IMailbox_GetIDsFromNames(long mailboxHandle, bool createIfNotExists, NamedPropData[] npa)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetIDsFromNames({0})", new object[]
			{
				mailboxHandle
			});
			PropTag[] propTags = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				propTags = mbx.GetIDsFromNames(createIfNotExists, npa);
			});
			if (propTags == null)
			{
				return null;
			}
			int[] array = new int[propTags.Length];
			for (int i = 0; i < propTags.Length; i++)
			{
				array[i] = (int)propTags[i];
			}
			return array;
		}

		NamedPropData[] IMailboxReplicationProxyService.IMailbox_GetNamesFromIDs(long mailboxHandle, int[] pta)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetNamesFromIDs({0})", new object[]
			{
				mailboxHandle
			});
			NamedPropData[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				result = mbx.GetNamesFromIDs(DataConverter<PropTagConverter, PropTag, int>.GetNative(pta));
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IMailbox_GetSessionSpecificEntryId(long mailboxHandle, byte[] entryId)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetSessionSpecificEntryId({0})", new object[]
			{
				mailboxHandle
			});
			byte[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				result = mbx.GetSessionSpecificEntryId(entryId);
			});
			return result;
		}

		MappedPrincipal[] IMailboxReplicationProxyService.IMailbox_GetPrincipalsFromMailboxGuids(long mailboxHandle, Guid[] mailboxGuids)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetPrincipalsFromMailboxGuids({0})", new object[]
			{
				mailboxHandle
			});
			MappedPrincipal[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				MappedPrincipal[] array = new MappedPrincipal[mailboxGuids.Length];
				for (int i = 0; i < mailboxGuids.Length; i++)
				{
					array[i] = new MappedPrincipal(mailboxGuids[i]);
				}
				result = mbx.ResolvePrincipals(array);
			});
			return result;
		}

		Guid[] IMailboxReplicationProxyService.IMailbox_GetMailboxGuidsFromPrincipals(long mailboxHandle, MappedPrincipal[] principals)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetMailboxGuidsFromPrincipals({0})", new object[]
			{
				mailboxHandle
			});
			Guid[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				MappedPrincipal[] array = mbx.ResolvePrincipals(principals);
				result = new Guid[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					result[i] = ((array[i] != null) ? array[i].MailboxGuid : Guid.Empty);
				}
			});
			return result;
		}

		MappedPrincipal[] IMailboxReplicationProxyService.IMailbox_ResolvePrincipals(long mailboxHandle, MappedPrincipal[] principals)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_ResolvePrincipals({0})", new object[]
			{
				mailboxHandle
			});
			MappedPrincipal[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				result = mbx.ResolvePrincipals(principals);
			});
			return result;
		}

		MailboxInformation IMailboxReplicationProxyService.IMailbox_GetMailboxInformation(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetMailboxInformation({0})", new object[]
			{
				mailboxHandle
			});
			MailboxInformation result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				result = mbx.GetMailboxInformation();
				if (result != null)
				{
					this.IsE15OrHigher = (result.ServerVersion >= Server.E15MinVersion);
				}
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IMailbox_GetMailboxSecurityDescriptor(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetMailboxSecurityDescriptor({0})", new object[]
			{
				mailboxHandle
			});
			byte[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				RawSecurityDescriptor mailboxSecurityDescriptor = mbx.GetMailboxSecurityDescriptor();
				byte[] array;
				if (mailboxSecurityDescriptor != null)
				{
					array = new byte[mailboxSecurityDescriptor.BinaryLength];
					mailboxSecurityDescriptor.GetBinaryForm(array, 0);
				}
				else
				{
					array = null;
				}
				result = array;
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IMailbox_GetUserSecurityDescriptor(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetUserSecurityDescriptor({0})", new object[]
			{
				mailboxHandle
			});
			byte[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				RawSecurityDescriptor userSecurityDescriptor = mbx.GetUserSecurityDescriptor();
				byte[] array;
				if (userSecurityDescriptor != null)
				{
					array = new byte[userSecurityDescriptor.BinaryLength];
					userSecurityDescriptor.GetBinaryForm(array, 0);
				}
				else
				{
					array = null;
				}
				result = array;
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_SetInTransitStatus(long mailboxHandle, int status, out bool onlineMoveSupported)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_SetInTransitStatus({0})", new object[]
			{
				mailboxHandle
			});
			bool oms = false;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				mbx.SetInTransitStatus((InTransitStatus)status, out oms);
				if (!this.ClientVersion[11] && status == 0)
				{
					mbx.SeedMBICache();
				}
			});
			onlineMoveSupported = oms;
		}

		void IMailboxReplicationProxyService.IMailbox_SeedMBICache(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_SeedMBICache({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				mbx.SeedMBICache();
			});
		}

		bool IMailboxReplicationProxyService.IMailbox_UpdateRemoteHostName(long mailboxHandle, string value)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_UpdateRemoteHostName({0}, {1})", new object[]
			{
				mailboxHandle,
				value
			});
			bool result = false;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				result = mbx.UpdateRemoteHostName(value);
			});
			return result;
		}

		string IMailboxReplicationProxyService.IMailbox_GetADUser(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetADUser({0})", new object[]
			{
				mailboxHandle
			});
			string result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				ADUser aduser = mbx.GetADUser();
				if (!this.ClientVersion[9] && aduser.HasSeparatedArchive)
				{
					throw new UnsupportedClientVersionPermanentException(this.ClientVersion.ComputerName, this.ClientVersion.ToString(), "ArchiveSeparation");
				}
				result = ConfigurableObjectXML.Serialize<ADUser>(aduser);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_UpdateMovedMailbox(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries)
		{
			((IMailboxReplicationProxyService)this).IMailbox_UpdateMovedMailbox3(mailboxHandle, op, remoteRecipientData, domainController, out entries, Guid.Empty, Guid.Empty, null, 0, 0);
		}

		void IMailboxReplicationProxyService.IMailbox_UpdateMovedMailbox2(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus)
		{
			((IMailboxReplicationProxyService)this).IMailbox_UpdateMovedMailbox3(mailboxHandle, op, remoteRecipientData, domainController, out entries, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, archiveStatus, 0);
		}

		void IMailboxReplicationProxyService.IMailbox_UpdateMovedMailbox3(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus, int updateMovedMailboxFlags)
		{
			((IMailboxReplicationProxyService)this).IMailbox_UpdateMovedMailbox4(mailboxHandle, op, remoteRecipientData, domainController, out entries, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, archiveStatus, updateMovedMailboxFlags, null, null);
		}

		void IMailboxReplicationProxyService.IMailbox_UpdateMovedMailbox4(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus, int updateMovedMailboxFlags, Guid? newMailboxContainerGuid, byte[] newUnifiedMailboxIdData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_UpdateMovedMailbox4({0}, {1}, {2}, {3}, {4}, {5}, {6})", new object[]
			{
				mailboxHandle,
				op,
				remoteRecipientData,
				domainController,
				updateMovedMailboxFlags,
				newMailboxContainerGuid,
				TraceUtils.DumpBytes(newUnifiedMailboxIdData)
			});
			entries = null;
			ReportEntry[] entryArray = null;
			try
			{
				this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
				{
					ADUser remoteRecipientData2 = ConfigurableObjectXML.Deserialize<ADUser>(remoteRecipientData);
					CrossTenantObjectId newUnifiedMailboxId = (newUnifiedMailboxIdData == null) ? null : CrossTenantObjectId.Parse(newUnifiedMailboxIdData, true);
					mbx.UpdateMovedMailbox(op, remoteRecipientData2, domainController, out entryArray, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, (ArchiveStatusFlags)archiveStatus, (UpdateMovedMailboxFlags)updateMovedMailboxFlags, newMailboxContainerGuid, newUnifiedMailboxId);
				});
			}
			finally
			{
				if (entryArray != null)
				{
					entries = XMLSerializableBase.Serialize(new List<ReportEntry>(entryArray), false);
				}
			}
		}

		void IMailboxReplicationProxyService.IMailbox_AddMoveHistoryEntry(long mailboxHandle, string mheData, int maxMoveHistoryLength)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_AddMoveHistoryEntry({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IMailbox mbx)
			{
				MoveHistoryEntryInternal mhei = XMLSerializableBase.Deserialize<MoveHistoryEntryInternal>(mheData, true);
				mbx.AddMoveHistoryEntry(mhei, maxMoveHistoryLength);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_CheckServerHealth(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_CheckServerHealth({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				ServerHealthStatus serverHealthStatus = mbx.CheckServerHealth();
				if (serverHealthStatus.HealthState == ServerHealthState.NotHealthy)
				{
					throw new MailboxReplicationTransientException(serverHealthStatus.FailureReason);
				}
			});
		}

		ServerHealthStatus IMailboxReplicationProxyService.IMailbox_CheckServerHealth2(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_CheckServerHealth2({0})", new object[]
			{
				mailboxHandle
			});
			ServerHealthStatus serverHealthStatus = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
			{
				serverHealthStatus = mbx.CheckServerHealth();
			});
			return serverHealthStatus;
		}

		PropValueData[] IMailboxReplicationProxyService.IMailbox_GetProps(long mailboxHandle, int[] ptags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetProps({0})", new object[]
			{
				mailboxHandle
			});
			PropValueData[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				result = mbx.GetProps(DataConverter<PropTagConverter, PropTag, int>.GetNative(ptags));
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IMailbox_GetReceiveFolderEntryId(long mailboxHandle, string msgClass)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetReceiveFolderEntryId({0})", new object[]
			{
				mailboxHandle
			});
			byte[] result = null;
			this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IMailbox mbx)
			{
				result = mbx.GetReceiveFolderEntryId(msgClass);
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceFolder_CopyTo(long folderHandle, int flags, int[] excludeTags, byte[] targetObjectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_CopyTo({0})", new object[]
			{
				folderHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<ISourceFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceFolder folder)
			{
				IDataMessage getDataResponseMsg = FxProxyGetObjectDataResponseMessage.Deserialize(DataMessageOpcode.FxProxyGetObjectDataResponse, targetObjectData, this.UseCompression);
				DataExport dataExport = new DataExport(getDataResponseMsg, this);
				try
				{
					dataExport.FolderExport(folder, (CopyPropertiesFlags)flags, DataConverter<PropTagConverter, PropTag, int>.GetNative(excludeTags));
					result = ((IDataExport)dataExport).ExportData();
					if (!result.IsLastBatch)
					{
						result.DataExportHandle = this.handleCache.AddObject(dataExport, folderHandle);
						dataExport = null;
					}
				}
				finally
				{
					if (dataExport != null)
					{
						dataExport.Dispose();
					}
				}
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceFolder_Export2(long folderHandle, int[] excludeTags, byte[] targetObjectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_Export2({0})", new object[]
			{
				folderHandle
			});
			return ((IMailboxReplicationProxyService)this).ISourceFolder_CopyTo(folderHandle, 0, excludeTags, targetObjectData);
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceFolder_ExportMessages(long folderHandle, int flags, byte[][] entryIds, byte[] targetObjectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_ExportMessages({0})", new object[]
			{
				folderHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<ISourceFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceFolder folder)
			{
				IDataMessage getDataResponseMsg = FxProxyGetObjectDataResponseMessage.Deserialize(DataMessageOpcode.FxProxyGetObjectDataResponse, targetObjectData, this.UseCompression);
				DataExport dataExport = new DataExport(getDataResponseMsg, this);
				try
				{
					dataExport.FolderExportMessages(folder, (CopyMessagesFlags)flags, entryIds);
					result = ((IDataExport)dataExport).ExportData();
					if (!result.IsLastBatch)
					{
						result.DataExportHandle = this.handleCache.AddObject(dataExport, folderHandle);
						dataExport = null;
					}
				}
				finally
				{
					if (dataExport != null)
					{
						dataExport.Dispose();
					}
				}
			});
			return result;
		}

		FolderChangesManifest IMailboxReplicationProxyService.ISourceFolder_EnumerateChanges(long folderHandle, bool catchup)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_EnumerateChanges({0})", new object[]
			{
				folderHandle
			});
			EnumerateContentChangesFlags flags = catchup ? EnumerateContentChangesFlags.Catchup : EnumerateContentChangesFlags.None;
			return ((IMailboxReplicationProxyService)this).ISourceFolder_EnumerateChanges2(folderHandle, (int)flags, 0);
		}

		FolderChangesManifest IMailboxReplicationProxyService.ISourceFolder_EnumerateChanges2(long folderHandle, int flags, int maxChanges)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_EnumerateChanges2({0})", new object[]
			{
				folderHandle
			});
			FolderChangesManifest result = null;
			this.ExecuteServiceCall<ISourceFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceFolder folder)
			{
				result = folder.EnumerateChanges((EnumerateContentChangesFlags)flags, maxChanges);
			});
			return result;
		}

		List<MessageRec> IMailboxReplicationProxyService.ISourceFolder_EnumerateMessagesPaged(long folderHandle, int maxPageSize)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_EnumerateMessagesPaged({0}, {1})", new object[]
			{
				folderHandle,
				maxPageSize
			});
			List<MessageRec> result = null;
			this.ExecuteServiceCall<ISourceFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceFolder folder)
			{
				result = folder.EnumerateMessagesPaged(maxPageSize);
			});
			return result;
		}

		int IMailboxReplicationProxyService.ISourceFolder_GetEstimatedItemCount(long folderHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_GetEstimatedItemCount({0})", new object[]
			{
				folderHandle
			});
			int result = 0;
			this.ExecuteServiceCall<ISourceFolder>(folderHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceFolder folder)
			{
				result = folder.GetEstimatedItemCount();
			});
			return result;
		}

		PropValueData[] IMailboxReplicationProxyService.ISourceFolder_GetProps(long folderHandle, int[] pta)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_GetProps({0})", new object[]
			{
				folderHandle
			});
			return ((IMailboxReplicationProxyService)this).IFolder_GetProps(folderHandle, pta);
		}

		void IMailboxReplicationProxyService.ISourceFolder_GetSearchCriteria(long folderHandle, out RestrictionData restriction, out byte[][] entryIDs, out int searchState)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceFolder_GetSearchCriteria({0})", new object[]
			{
				folderHandle
			});
			((IMailboxReplicationProxyService)this).IFolder_GetSearchCriteria(folderHandle, out restriction, out entryIDs, out searchState);
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_Export2(long mailboxHandle, int[] excludeProps, byte[] targetObjectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_Export2({0})", new object[]
			{
				mailboxHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				IDataMessage getDataResponseMsg = FxProxyGetObjectDataResponseMessage.Deserialize(DataMessageOpcode.FxProxyGetObjectDataResponse, targetObjectData, this.UseCompression);
				DataExport dataExport = new DataExport(getDataResponseMsg, this);
				try
				{
					dataExport.MailboxExport(mbx, DataConverter<PropTagConverter, PropTag, int>.GetNative(excludeProps));
					result = ((IDataExport)dataExport).ExportData();
					if (!result.IsLastBatch)
					{
						result.DataExportHandle = this.handleCache.AddObject(dataExport, mailboxHandle);
						dataExport = null;
					}
				}
				finally
				{
					if (dataExport != null)
					{
						dataExport.Dispose();
					}
				}
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_ExportMessageBatch2(long mailboxHandle, List<MessageRec> messages, byte[] targetObjectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_ExportMessageBatch2({0})", new object[]
			{
				mailboxHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				IDataMessage getDataResponseMsg = FxProxyPoolGetFolderDataResponseMessage.Deserialize(DataMessageOpcode.FxProxyPoolGetFolderDataResponse, targetObjectData, this.UseCompression);
				DataExport dataExport = new DataExport(getDataResponseMsg, this);
				try
				{
					dataExport.MessageExportWithBadMessageDetection(mbx, messages, ExportMessagesFlags.None, null, true);
					result = ((IDataExport)dataExport).ExportData();
					if (!result.IsLastBatch)
					{
						result.DataExportHandle = this.handleCache.AddObject(dataExport, mailboxHandle);
						dataExport = null;
					}
				}
				finally
				{
					if (dataExport != null)
					{
						dataExport.Dispose();
					}
				}
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_ExportMessages(long mailboxHandle, List<MessageRec> messages, int flags, int[] excludeProps, byte[] targetObjectData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_ExportMessages({0})", new object[]
			{
				mailboxHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				IDataMessage getDataResponseMsg = FxProxyPoolGetFolderDataResponseMessage.Deserialize(DataMessageOpcode.FxProxyPoolGetFolderDataResponse, targetObjectData, this.UseCompression);
				DataExport dataExport = new DataExport(getDataResponseMsg, this);
				try
				{
					PropTag[] native = DataConverter<PropTagConverter, PropTag, int>.GetNative(excludeProps);
					if (!this.ClientVersion[16])
					{
						dataExport.MessageExportWithBadMessageDetection(mbx, messages, (ExportMessagesFlags)flags, native, false);
					}
					else
					{
						dataExport.MessageExport(mbx, messages, (ExportMessagesFlags)flags, native);
					}
					result = ((IDataExport)dataExport).ExportData();
					if (!result.IsLastBatch)
					{
						result.DataExportHandle = this.handleCache.AddObject(dataExport, mailboxHandle);
						dataExport = null;
					}
				}
				finally
				{
					if (dataExport != null)
					{
						dataExport.Dispose();
					}
				}
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_ExportFolders(long mailboxHandle, List<byte[]> folderIds, int exportFoldersDataToCopyFlags, int folderRecFlags, int[] additionalFolderRecProps, int copyPropertiesFlags, int[] excludeProps, int extendedAclFlags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_ExportFolders({0})", new object[]
			{
				mailboxHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				EntryIdMap<byte[]> entryIdMap = new EntryIdMap<byte[]>();
				foreach (byte[] key in folderIds)
				{
					entryIdMap.Add(key, MapiUtils.MapiFolderObjectData);
				}
				IDataMessage getDataResponseMsg = new FxProxyPoolGetFolderDataResponseMessage(entryIdMap);
				DataExport dataExport = new DataExport(getDataResponseMsg, this);
				try
				{
					PropTag[] native = DataConverter<PropTagConverter, PropTag, int>.GetNative(additionalFolderRecProps);
					PropTag[] native2 = DataConverter<PropTagConverter, PropTag, int>.GetNative(excludeProps);
					dataExport.FoldersExport(mbx, folderIds, (ExportFoldersDataToCopyFlags)exportFoldersDataToCopyFlags, (GetFolderRecFlags)folderRecFlags, native, (CopyPropertiesFlags)copyPropertiesFlags, native2, (AclFlags)extendedAclFlags);
					result = ((IDataExport)dataExport).ExportData();
					if (!result.IsLastBatch)
					{
						result.DataExportHandle = this.handleCache.AddObject(dataExport, mailboxHandle);
						dataExport = null;
					}
				}
				finally
				{
					if (dataExport != null)
					{
						dataExport.Dispose();
					}
				}
			});
			return result;
		}

		MailboxChangesManifest IMailboxReplicationProxyService.ISourceMailbox_EnumerateHierarchyChanges(long mailboxHandle, bool catchup)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_EnumerateHierarchyChanges({0})", new object[]
			{
				mailboxHandle
			});
			EnumerateHierarchyChangesFlags flags = catchup ? EnumerateHierarchyChangesFlags.Catchup : EnumerateHierarchyChangesFlags.None;
			return ((IMailboxReplicationProxyService)this).ISourceMailbox_EnumerateHierarchyChanges2(mailboxHandle, (int)flags, 0);
		}

		MailboxChangesManifest IMailboxReplicationProxyService.ISourceMailbox_EnumerateHierarchyChanges2(long mailboxHandle, int flags, int maxChanges)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_EnumerateHierarchyChanges({0})", new object[]
			{
				mailboxHandle
			});
			MailboxChangesManifest result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				result = mbx.EnumerateHierarchyChanges((EnumerateHierarchyChangesFlags)flags, maxChanges);
			});
			return result;
		}

		long IMailboxReplicationProxyService.ISourceMailbox_GetFolder(long mailboxHandle, byte[] entryId)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_GetFolder({0})", new object[]
			{
				mailboxHandle
			});
			long result = -1L;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				ISourceFolder folder = mbx.GetFolder(entryId);
				if (folder != null)
				{
					result = this.handleCache.AddObject(folder, mailboxHandle);
					return;
				}
				result = 0L;
			});
			return result;
		}

		List<ReplayActionResult> IMailboxReplicationProxyService.ISourceMailbox_ReplayActions(long mailboxHandle, List<ReplayAction> actions)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_ReplayActions({0})", new object[]
			{
				mailboxHandle
			});
			List<ReplayActionResult> result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(ISourceMailbox mbx)
			{
				result = mbx.ReplayActions(actions);
			});
			return result;
		}

		List<ItemPropertiesBase> IMailboxReplicationProxyService.ISourceMailbox_GetMailboxSettings(long mailboxHandle, int flags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_GetMailboxSettings({0})", new object[]
			{
				mailboxHandle
			});
			List<ItemPropertiesBase> result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(ISourceMailbox mbx)
			{
				result = mbx.GetMailboxSettings((GetMailboxSettingsFlags)flags);
			});
			return result;
		}

		Guid IMailboxReplicationProxyService.IMailbox_StartIsInteg(long mailboxHandle, List<uint> mailboxCorruptionTypes)
		{
			Guid result = Guid.Empty;
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_StartIsInteg({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				result = mbx.StartIsInteg(mailboxCorruptionTypes);
			});
			return result;
		}

		List<StoreIntegrityCheckJob> IMailboxReplicationProxyService.IMailbox_QueryIsInteg(long mailboxHandle, Guid isIntegRequestGuid)
		{
			List<StoreIntegrityCheckJob> jobs = null;
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_QueryIsInteg({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				jobs = mbx.QueryIsInteg(isIntegRequestGuid);
			});
			return jobs;
		}

		SessionStatistics IMailboxReplicationProxyService.IMailbox_GetSessionStatistics(long mailboxHandle, int statisticsTypes)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IMailbox_GetSessionStatistics)", new object[0]);
			SessionStatistics result = null;
			SessionStatisticsFlags flags = (SessionStatisticsFlags)statisticsTypes;
			if (flags.HasFlag(SessionStatisticsFlags.MapiDiagnosticGetProp))
			{
				result = new SessionStatistics();
				result.MapiDiagnosticGetProp = this.mapiDiagnosticGetProp;
			}
			else
			{
				this.ExecuteServiceCall<IMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(IMailbox mbx)
				{
					result = mbx.GetSessionStatistics(flags);
				});
			}
			return result;
		}

		byte[] IMailboxReplicationProxyService.ISourceMailbox_GetMailboxBasicInfo(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_GetMailboxBasicInfo({0})", new object[]
			{
				mailboxHandle
			});
			byte[] result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				result = mbx.GetMailboxBasicInfo(MailboxSignatureFlags.GetLegacy);
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.ISourceMailbox_GetMailboxBasicInfo2(long mailboxHandle, int signatureFlags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_GetMailboxBasicInfo2({0}, {1})", new object[]
			{
				mailboxHandle,
				signatureFlags
			});
			byte[] result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				result = mbx.GetMailboxBasicInfo((MailboxSignatureFlags)signatureFlags);
			});
			return result;
		}

		PropValueData[] IMailboxReplicationProxyService.ISourceMailbox_GetProps(long mailboxHandle, int[] ptags)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_GetProps({0})", new object[]
			{
				mailboxHandle
			});
			PropValueData[] result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				result = mbx.GetProps(DataConverter<PropTagConverter, PropTag, int>.GetNative(ptags));
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_GetMailboxSyncState(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_GetMailboxSyncState({0})", new object[]
			{
				mailboxHandle
			});
			DataExportBatch result = null;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(ISourceMailbox mbx)
			{
				string mailboxSyncState = mbx.GetMailboxSyncState();
				IDataExport dataExport = new PagedTransmitter(mailboxSyncState, this.UseCompression);
				result = dataExport.ExportData();
				if (!result.IsLastBatch)
				{
					result.DataExportHandle = this.handleCache.AddObject(dataExport, mailboxHandle);
				}
			});
			return result;
		}

		long IMailboxReplicationProxyService.ISourceMailbox_SetMailboxSyncState(long mailboxHandle, DataExportBatch firstBatch)
		{
			MrsTracer.ProxyService.Function("MRSProxy.ISourceMailbox_GetMailboxSyncState({0})", new object[]
			{
				mailboxHandle
			});
			long result = -1L;
			this.ExecuteServiceCall<ISourceMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.CPUOnly, delegate(ISourceMailbox mbx)
			{
				IDataImport dataImport = new PagedReceiver(delegate(string str)
				{
					mbx.SetMailboxSyncState(str);
				}, this.UseCompression);
				try
				{
					IDataMessage message = DataMessageSerializer.Deserialize(firstBatch.Opcode, firstBatch.Data, this.UseCompression);
					dataImport.SendMessage(message);
					if (!firstBatch.IsLastBatch)
					{
						result = this.handleCache.AddObject(dataImport, mailboxHandle);
						dataImport = null;
					}
					else
					{
						result = 0L;
					}
				}
				finally
				{
					if (dataImport != null)
					{
						dataImport.Dispose();
					}
				}
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_PreFinalSyncDataProcessing(long mailboxHandle, int? sourceMailboxVersion)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_PreFinalSyncDataProcessing({0}, {1})", new object[]
			{
				mailboxHandle,
				sourceMailboxVersion
			});
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				mbx.PreFinalSyncDataProcessing(sourceMailboxVersion);
			});
		}

		int IMailboxReplicationProxyService.IDestinationMailbox_CheckDataGuarantee(long mailboxHandle, DateTime commitTimestamp, out byte[] failureReasonData)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_CheckDataGuarantee({0})", new object[]
			{
				mailboxHandle
			});
			ConstraintCheckResultType result = ConstraintCheckResultType.Satisfied;
			LocalizedString failureReason = LocalizedString.Empty;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				result = mbx.CheckDataGuarantee(commitTimestamp, out failureReason);
			});
			failureReasonData = CommonUtils.ByteSerialize(failureReason);
			return (int)result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_ForceLogRoll(long mailboxHandle)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_ForceLogRoll({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				mbx.ForceLogRoll();
			});
		}

		List<ReplayAction> IMailboxReplicationProxyService.IDestinationMailbox_GetActions(long mailboxHandle, string replaySyncState, int maxNumberOfActions)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_GetActions({0})", new object[]
			{
				mailboxHandle
			});
			List<ReplayAction> result = null;
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbRead, delegate(IDestinationMailbox mbx)
			{
				result = mbx.GetActions(replaySyncState, maxNumberOfActions);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_SetMailboxSettings(long mailboxHandle, ItemPropertiesBase item)
		{
			MrsTracer.ProxyService.Function("MRSProxy.IDestinationMailbox_GetActions({0})", new object[]
			{
				mailboxHandle
			});
			this.ExecuteServiceCall<IDestinationMailbox>(mailboxHandle, ExecutionFlags.Default, DelayScopeKind.DbWrite, delegate(IDestinationMailbox mbx)
			{
				mbx.SetMailboxSettings(item);
			});
		}

		MigrationAccount[] IMailboxReplicationProxyService.SelectAccountsToMigrate(long maximumAccounts, long? maximumTotalSize, int? constraintId)
		{
			throw new NotImplementedException();
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			lock (MailboxReplicationProxyService.activeConnectionsUpdateLock)
			{
				if (this.connections > 0)
				{
					MailboxReplicationProxyService.activeConnections -= this.connections;
					this.connections = 0;
				}
			}
			if (calledFromDispose)
			{
				lock (this.locker)
				{
					this.handleCache.Dispose();
				}
			}
			MrsTracer.ProxyService.Debug("MailboxReplicationProxyService instance disposed.", new object[0]);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxReplicationProxyService>(this);
		}

		private void IncrementConnectionCount()
		{
			if (!this.IsThrottled)
			{
				return;
			}
			lock (MailboxReplicationProxyService.activeConnectionsUpdateLock)
			{
				int maxMRSConnections = MRSProxyConfiguration.Instance.MaxMRSConnections;
				if (MailboxReplicationProxyService.activeConnections >= maxMRSConnections)
				{
					MrsTracer.ProxyService.Debug("MRSProxy Service cannot currently accept more connections.", new object[0]);
					MailboxReplicationServiceFault.Throw(new MRSProxyTooManyConnectionsTransientException(MailboxReplicationProxyService.activeConnections, maxMRSConnections));
				}
				MailboxReplicationProxyService.activeConnections++;
				this.connections++;
			}
		}

		private void DecrementConnectionCount()
		{
			if (!this.IsThrottled)
			{
				return;
			}
			lock (MailboxReplicationProxyService.activeConnectionsUpdateLock)
			{
				if (this.connections > 0)
				{
					MailboxReplicationProxyService.activeConnections--;
					this.connections--;
				}
			}
		}

		private void DisconnectOrphanedSession(Guid physicalMailboxGuid)
		{
			lock (this.locker)
			{
				long mailboxHandle;
				if (this.activeMailboxes.TryGetValue(physicalMailboxGuid, out mailboxHandle))
				{
					this.DisconnectMailboxSession(mailboxHandle);
				}
			}
		}

		private void DisconnectMailboxSession(long mailboxHandle)
		{
			this.handleCache.ReleaseObject(mailboxHandle);
			Guid? guid = null;
			foreach (KeyValuePair<Guid, long> keyValuePair in this.activeMailboxes)
			{
				if (keyValuePair.Value == mailboxHandle)
				{
					guid = new Guid?(keyValuePair.Key);
				}
			}
			if (guid != null)
			{
				this.activeMailboxes.Remove(guid.Value);
			}
		}

		private void ExecuteServiceCall<T>(long handle, ExecutionFlags flags, DelayScopeKind delayScopeKind, Action<T> operation) where T : class
		{
			if (!flags.HasFlag(ExecutionFlags.NoLock))
			{
				Monitor.Enter(this.locker);
			}
			try
			{
				Exception failure = null;
				try
				{
					CommonUtils.CatchKnownExceptions(delegate
					{
						if (!flags.HasFlag(ExecutionFlags.ThrottlingNotRequired) && !this.IsThrottled)
						{
							MrsTracer.ProxyService.Warning("MRSProxy Service cannot process this call since this connection is not throttled.", new object[0]);
							throw new MRSProxyCannotProcessRequestOnUnthrottledConnectionPermanentException();
						}
						T t = default(T);
						if (handle != -1L)
						{
							t = this.handleCache.GetObject<T>(handle);
						}
						using (this.ActivateSettingsContext(t, handle))
						{
							using (MicroDelayScope.Create(this, delayScopeKind))
							{
								operation(t);
							}
						}
					}, delegate(Exception f)
					{
						failure = f;
					});
				}
				catch (Exception ex)
				{
					MrsTracer.ProxyService.Error("Unhandled exception in MRSProxy:\n{0}\n{1}", new object[]
					{
						CommonUtils.FullExceptionMessage(ex),
						ex.StackTrace
					});
					ExWatson.SendReport(ex);
					throw;
				}
				if (failure != null)
				{
					MailboxReplicationServiceFault.Throw(failure);
				}
			}
			finally
			{
				if (!flags.HasFlag(ExecutionFlags.NoLock))
				{
					Monitor.Exit(this.locker);
				}
			}
		}

		private IDisposable ActivateSettingsContext(object curObject, long handle)
		{
			if (curObject == null)
			{
				return null;
			}
			ISettingsContextProvider settingsContextProvider;
			for (;;)
			{
				settingsContextProvider = (curObject as ISettingsContextProvider);
				if (settingsContextProvider != null)
				{
					break;
				}
				handle = this.handleCache.GetParentHandle(handle);
				if (handle == -1L)
				{
					goto IL_39;
				}
				curObject = this.handleCache.GetObject<object>(handle);
			}
			return SettingsContextBase.ActivateContext(settingsContextProvider);
			IL_39:
			return null;
		}

		private long Config(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, string filePath, byte[] partitionHintBytes, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMbxFlags, Guid? mailboxContainerGuid)
		{
			MrsTracer.ProxyService.Function("MRSProxy.Config(reservationId={0}, primaryMailboxGuid={1}, physicalMailboxGuid={2}, filePath={3}, mdbGuid={4}, mdbName='{5}', mbxType={6}, proxyControlFlags={7}, localMbxFlags={8}, mailboxContainerGuid={9})", new object[]
			{
				reservationId,
				primaryMailboxGuid,
				physicalMailboxGuid,
				filePath,
				mdbGuid,
				mdbName,
				mbxType,
				(ProxyControlFlags)proxyControlFlags,
				(LocalMailboxFlags)localMbxFlags,
				mailboxContainerGuid
			});
			this.ExchangeGuid = physicalMailboxGuid;
			this.proxyControlFlags = (ProxyControlFlags)proxyControlFlags;
			LocalMailboxFlags localMailboxFlags = (LocalMailboxFlags)localMbxFlags;
			long result = -1L;
			this.ExecuteServiceCall<object>(-1L, ExecutionFlags.ThrottlingNotRequired, DelayScopeKind.NoDelay, delegate(object o)
			{
				Guid guid = Guid.Empty;
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Items.Contains("MRSProxy.MailboxGuid"))
					{
						object obj = HttpContext.Current.Items["MRSProxy.MailboxGuid"];
						if (obj is Guid)
						{
							guid = (Guid)obj;
						}
					}
					if (primaryMailboxGuid != guid)
					{
						MrsTracer.ProxyService.Debug("The mailbox guid passed in by the caller {0} does not match the one in message headers {1}, bailing.", new object[]
						{
							primaryMailboxGuid,
							guid
						});
						throw new UnexpectedErrorPermanentException(-2147024809);
					}
				}
				TenantPartitionHint partitionHint = null;
				if (partitionHintBytes != null)
				{
					partitionHint = TenantPartitionHint.FromPersistablePartitionHint(partitionHintBytes);
				}
				if (localMailboxFlags.HasFlag(LocalMailboxFlags.ParallelPublicFolderMigration) && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.PublicFolderMailboxesMigration.Enabled && primaryMailboxGuid == Guid.Empty)
				{
					ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId);
					PublicFolderInformation hierarchyMailboxInformation = TenantPublicFolderConfigurationCache.Instance.GetValue(adsessionSettings.CurrentOrganizationId).GetHierarchyMailboxInformation();
					if (hierarchyMailboxInformation.Type != PublicFolderInformation.HierarchyType.MailboxGuid || !(hierarchyMailboxInformation.HierarchyMailboxGuid != Guid.Empty))
					{
						throw new NoPublicFolderMailboxFoundInSourceException();
					}
					this.ExchangeGuid = (physicalMailboxGuid = (primaryMailboxGuid = hierarchyMailboxInformation.HierarchyMailboxGuid));
				}
				if (primaryMailboxGuid != Guid.Empty && (localMailboxFlags.HasFlag(LocalMailboxFlags.UseHomeMDB) || (mdbGuid == Guid.Empty && mbxType == MailboxType.SourceMailbox)))
				{
					MiniRecipient miniRecipient = CommonUtils.FindUserByMailboxGuid(primaryMailboxGuid, partitionHint, null, null, MailboxReplicationProxyService.UserPropertiesToLoad);
					if (miniRecipient != null)
					{
						ADObjectId adobjectId;
						if (primaryMailboxGuid == physicalMailboxGuid)
						{
							adobjectId = miniRecipient.Database;
						}
						else
						{
							adobjectId = miniRecipient.ArchiveDatabase;
						}
						if (adobjectId != null)
						{
							mdbGuid = adobjectId.ObjectGuid;
							MrsTracer.ProxyService.Debug("MRSProxy.IMailbox_Config4: Using homeMdb {0} for mailbox {1}", new object[]
							{
								mdbGuid,
								physicalMailboxGuid
							});
						}
					}
				}
				if (!string.IsNullOrEmpty(mdbName))
				{
					MailboxDatabase mailboxDatabase = CommonUtils.FindMdbByName(mdbName, null, null);
					mdbGuid = mailboxDatabase.Guid;
				}
				bool flag = mdbGuid != Guid.Empty && MapiUtils.FindServerForMdb(mdbGuid, null, null, FindServerFlags.None).IsOnThisServer;
				IMailbox mailbox;
				switch (mbxType)
				{
				case MailboxType.SourceMailbox:
					if (localMailboxFlags.HasFlag(LocalMailboxFlags.EasSync))
					{
						mailbox = new EasSourceMailbox();
					}
					else if (localMailboxFlags.HasFlag(LocalMailboxFlags.PstImport))
					{
						mailbox = new PstSourceMailbox();
						mailbox.ConfigPst(filePath, null);
					}
					else if (!flag || localMailboxFlags.HasFlag(LocalMailboxFlags.UseMapiProvider))
					{
						mailbox = new MapiSourceMailbox(localMailboxFlags);
					}
					else
					{
						mailbox = new StorageSourceMailbox(localMailboxFlags);
					}
					break;
				case MailboxType.DestMailboxIntraOrg:
				case MailboxType.DestMailboxCrossOrg:
					if (localMailboxFlags.HasFlag(LocalMailboxFlags.PstExport))
					{
						mailbox = new PstDestinationMailbox();
						mailbox.ConfigPst(filePath, null);
					}
					else if (!flag || localMailboxFlags.HasFlag(LocalMailboxFlags.UseMapiProvider))
					{
						mailbox = new MapiDestinationMailbox(localMailboxFlags);
					}
					else
					{
						mailbox = new StorageDestinationMailbox(localMailboxFlags);
					}
					break;
				default:
					throw new UnexpectedErrorPermanentException(-2147024809);
				}
				((MailboxProviderBase)mailbox).MRSVersion = this.clientVersion;
				ReservationBase reservationBase = ReservationManager.FindReservation(reservationId);
				if (reservationBase != null)
				{
					MailboxReservation mailboxReservation = reservationBase as MailboxReservation;
					mailboxReservation.DisconnectOrphanedSession(physicalMailboxGuid);
					mailboxReservation.RegisterDisconnectOrphanedSessionAction(physicalMailboxGuid, new Action<Guid>(this.DisconnectOrphanedSession));
				}
				mailbox.Config(reservationBase, primaryMailboxGuid, physicalMailboxGuid, partitionHint, mdbGuid, mbxType, mailboxContainerGuid);
				if (!string.IsNullOrEmpty(mdbName))
				{
					mailbox.ConfigMDBByName(mdbName);
				}
				result = this.handleCache.AddObject(mailbox, -1L);
				this.activeMailboxes[physicalMailboxGuid] = result;
			});
			return result;
		}

		private const int FolderBatchSize = 100;

		private const int MessageBatchSize = 1000;

		private const long NoHandle = -1L;

		private static readonly ADPropertyDefinition[] UserPropertiesToLoad = new ADPropertyDefinition[]
		{
			ADMailboxRecipientSchema.Database,
			ADUserSchema.ArchiveDatabase
		};

		private static readonly object activeConnectionsUpdateLock;

		private static int activeConnections = 0;

		private readonly object locker = new object();

		private readonly Dictionary<Guid, long> activeMailboxes = new Dictionary<Guid, long>();

		private HandleCache handleCache;

		private VersionInformation clientVersion;

		private int connections;

		private ProxyControlFlags proxyControlFlags;

		private long enumerateMessagesFolder;

		private List<MessageRec> enumerateMessagesRemainingData;

		private long enumerateFoldersMailbox;

		private List<FolderRec> enumerateFoldersRemainingData;

		private string mapiDiagnosticGetProp = string.Empty;
	}
}
