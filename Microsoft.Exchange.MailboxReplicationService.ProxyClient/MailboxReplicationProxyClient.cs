using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxReplicationProxyClient : WcfClientWithFaultHandling<IMailboxReplicationProxyService, FaultException<MailboxReplicationServiceFault>>, IMailboxReplicationProxyService
	{
		private MailboxReplicationProxyClient(Binding binding, EndpointAddress address, Guid physicalMbxGuid, Guid primaryMbxGuid, string filePath, string targetDatabase, TenantPartitionHint partitionHint, ProxyControlFlags proxyControlFlags, TimeSpan longOperationTimeout) : base(binding, address)
		{
			this.Init(physicalMbxGuid, primaryMbxGuid, filePath, targetDatabase, partitionHint, proxyControlFlags, longOperationTimeout);
		}

		private MailboxReplicationProxyClient(string endpointConfigurationName, Guid physicalMbxGuid, Guid primaryMbxGuid, string filePath, string targetDatabase, TenantPartitionHint partitionHint, ProxyControlFlags proxyControlFlags, TimeSpan longOperationTimeout) : base(endpointConfigurationName)
		{
			this.Init(physicalMbxGuid, primaryMbxGuid, filePath, targetDatabase, partitionHint, proxyControlFlags, longOperationTimeout);
		}

		public LatencyInfo LatencyInfo
		{
			get
			{
				return this.latencyInfo;
			}
		}

		public bool UseCompression
		{
			get
			{
				return !this.proxyControlFlags.HasFlag(ProxyControlFlags.DoNotCompress);
			}
		}

		public bool UseBuffering
		{
			get
			{
				return !this.proxyControlFlags.HasFlag(ProxyControlFlags.DoNotBuffer);
			}
		}

		public bool UseCertificateToAuthenticate
		{
			get
			{
				return this.proxyControlFlags.HasFlag(ProxyControlFlags.UseCertificateToAuthenticate);
			}
		}

		public string ServerName
		{
			get
			{
				if (base.ServerVersion == null)
				{
					return base.Endpoint.Address.ToString();
				}
				return base.ServerVersion.ComputerName;
			}
		}

		public TimeSpan LongOperationTimeout { get; private set; }

		public MRSProxyRequestContext RequestContext { get; private set; }

		public static MailboxReplicationProxyClient CreateForOlcConnection(string serverName, ProxyControlFlags flags)
		{
			flags |= (ProxyControlFlags.UseCertificateToAuthenticate | ProxyControlFlags.Olc);
			return MailboxReplicationProxyClient.Create(serverName, null, null, Guid.Empty, Guid.Empty, null, null, null, true, flags, ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("MRSProxyLongOperationTimeout"));
		}

		public static MailboxReplicationProxyClient Create(string serverName, string remoteOrgName, NetworkCredential remoteCred, Guid physicalMbxGuid, Guid primaryMbxGuid, string filePath, string database, TenantPartitionHint partitionHint, bool useHttps, ProxyControlFlags flags, TimeSpan longOperationTimeout)
		{
			string endpointConfigurationName;
			if (flags.HasFlag(ProxyControlFlags.UseTcp) || TestIntegration.Instance.UseTcpForRemoteMoves)
			{
				endpointConfigurationName = "MrsProxyClientTcpEndpoint";
			}
			else if (TestIntegration.Instance.UseHttpsForLocalMoves && !useHttps)
			{
				endpointConfigurationName = "MrsProxyClientMrsHttpsEndpoint";
			}
			else if (flags.HasFlag(ProxyControlFlags.UseCertificateToAuthenticate))
			{
				endpointConfigurationName = "MrsProxyClientCertEndpoint";
			}
			else if (useHttps)
			{
				endpointConfigurationName = "MrsProxyClientHttpsEndpoint";
			}
			else
			{
				endpointConfigurationName = "MrsProxyClientTcpEndpoint";
			}
			MailboxReplicationProxyClient result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MailboxReplicationProxyClient mailboxReplicationProxyClient;
				if (TestIntegration.Instance.ProtocolTest)
				{
					NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.Transport);
					netTcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
					netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
					netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
					netTcpBinding.TransactionFlow = false;
					netTcpBinding.TransferMode = TransferMode.Buffered;
					netTcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(20.0);
					netTcpBinding.SendTimeout = TimeSpan.FromMinutes(20.0);
					netTcpBinding.MaxReceivedMessageSize = 100000000L;
					netTcpBinding.ReaderQuotas.MaxDepth = 256;
					netTcpBinding.ReaderQuotas.MaxStringContentLength = 35000000;
					netTcpBinding.ReaderQuotas.MaxArrayLength = 35000000;
					netTcpBinding.ReaderQuotas.MaxBytesPerRead = 4096;
					netTcpBinding.ReaderQuotas.MaxNameTableCharCount = 16384;
					EndpointAddress address = new EndpointAddress("net.tcp://localhost/Microsoft.Exchange.MailboxReplicationService.ProxyService");
					mailboxReplicationProxyClient = new MailboxReplicationProxyClient(netTcpBinding, address, physicalMbxGuid, primaryMbxGuid, filePath, database, partitionHint, flags, longOperationTimeout);
				}
				else
				{
					mailboxReplicationProxyClient = new MailboxReplicationProxyClient(endpointConfigurationName, physicalMbxGuid, primaryMbxGuid, filePath, database, partitionHint, flags, longOperationTimeout);
				}
				disposeGuard.Add<MailboxReplicationProxyClient>(mailboxReplicationProxyClient);
				mailboxReplicationProxyClient.Initialize(serverName, remoteCred, useHttps);
				if (flags.HasFlag(ProxyControlFlags.ResolveServerName) && mailboxReplicationProxyClient.ServerVersion[45])
				{
					ProxyServerInformation proxyServerInformation = ((IMailboxReplicationProxyService)mailboxReplicationProxyClient).FindServerByDatabaseOrMailbox(database, new Guid?(physicalMbxGuid), new Guid?(primaryMbxGuid), (partitionHint != null) ? partitionHint.GetPersistablePartitionHint() : null);
					if (!proxyServerInformation.IsProxyLocal)
					{
						flags &= ~ProxyControlFlags.ResolveServerName;
						return MailboxReplicationProxyClient.Create(proxyServerInformation.ServerFqdn, remoteOrgName, remoteCred, physicalMbxGuid, primaryMbxGuid, filePath, database, partitionHint, useHttps, flags, longOperationTimeout);
					}
				}
				disposeGuard.Success();
				result = mailboxReplicationProxyClient;
			}
			return result;
		}

		public static MailboxReplicationProxyClient CreateWithoutThrottling(string serverFQDN, NetworkCredential credentials, Guid mailboxGuid, Guid mdbGuid)
		{
			return MailboxReplicationProxyClient.Create(serverFQDN, null, credentials, mailboxGuid, mailboxGuid, null, mdbGuid.ToString(), null, false, ProxyControlFlags.DoNotApplyProxyThrottling | ProxyControlFlags.DoNotCompress, MailboxReplicationProxyClient.DefaultOperationTimeout);
		}

		public static MailboxReplicationProxyClient CreateConnectivityTestClient(string serverFQDN, NetworkCredential credentials, bool useHttps)
		{
			return MailboxReplicationProxyClient.Create(serverFQDN, null, credentials, MailboxReplicationProxyClient.ConnectionTestGuid, MailboxReplicationProxyClient.ConnectionTestGuid, null, null, null, useHttps, ProxyControlFlags.DoNotApplyProxyThrottling | ProxyControlFlags.DoNotAddIdentifyingCafeHeaders, MailboxReplicationProxyClient.DefaultOperationTimeout);
		}

		public void ExchangeVersionInformation()
		{
			VersionInformation serverVersion = null;
			((IMailboxReplicationProxyService)this).ExchangeVersionInformation(VersionInformation.MRSProxy, out serverVersion);
			base.ServerVersion = serverVersion;
			this.VerifyRequiredCapability(MRSProxyCapabilities.CachedMailboxSyncState);
		}

		public string UnpackString(byte[] data)
		{
			return CommonUtils.UnpackString(data, this.UseCompression);
		}

		public byte[] PackString(string str)
		{
			return CommonUtils.PackString(str, this.UseCompression);
		}

		void IMailboxReplicationProxyService.ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			VersionInformation sv = null;
			this.CallService(delegate()
			{
				this.Channel.ExchangeVersionInformation(clientVersion, out sv);
			});
			serverVersion = sv;
		}

		ProxyServerInformation IMailboxReplicationProxyService.FindServerByDatabaseOrMailbox(string databaseId, Guid? physicalMailboxGuid, Guid? primaryMailboxGuid, byte[] tenantPartitionHintBytes)
		{
			ProxyServerInformation result = null;
			this.CallService(delegate()
			{
				result = this.Channel.FindServerByDatabaseOrMailbox(databaseId, physicalMailboxGuid, primaryMailboxGuid, tenantPartitionHintBytes);
			});
			return result;
		}

		IEnumerable<ContainerMailboxInformation> IMailboxReplicationProxyService.GetMailboxContainerMailboxes(Guid mdbGuid, Guid primaryMailboxGuid)
		{
			this.VerifyRequiredCapability(MRSProxyCapabilities.GetMailboxContainerMailboxes);
			IEnumerable<ContainerMailboxInformation> result = null;
			this.CallService(delegate()
			{
				result = this.Channel.GetMailboxContainerMailboxes(mdbGuid, primaryMailboxGuid);
			});
			return result;
		}

		bool IMailboxReplicationProxyService.ArePublicFoldersReadyForMigrationCompletion()
		{
			this.VerifyRequiredCapability(MRSProxyCapabilities.PublicFolderMailboxesMigrationSupport);
			bool result = false;
			this.CallService(delegate()
			{
				result = this.Channel.ArePublicFoldersReadyForMigrationCompletion();
			});
			return result;
		}

		List<Guid> IMailboxReplicationProxyService.GetPublicFolderMailboxesExchangeGuids()
		{
			this.VerifyRequiredCapability(MRSProxyCapabilities.PublicFolderMailboxesMigrationSupport);
			List<Guid> result = null;
			this.CallService(delegate()
			{
				result = this.Channel.GetPublicFolderMailboxesExchangeGuids();
			});
			return result;
		}

		long IMailboxReplicationProxyService.IDestinationMailbox_GetFxProxy(long mailboxHandle, out byte[] objectData)
		{
			long result = -1L;
			byte[] objData = null;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationMailbox_GetFxProxy(mailboxHandle, out objData);
			});
			objectData = objData;
			return result;
		}

		PropProblemData[] IMailboxReplicationProxyService.IDestinationMailbox_SetProps(long mailboxHandle, PropValueData[] pva)
		{
			PropProblemData[] result = null;
			this.CallService(delegate()
			{
				this.Channel.IDestinationMailbox_SetProps(mailboxHandle, pva);
			});
			return result;
		}

		long IMailboxReplicationProxyService.IDestinationMailbox_GetFxProxyPool(long mailboxHandle, byte[][] folderIds, out byte[] objectData)
		{
			long result = -1L;
			byte[] objData = null;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationMailbox_GetFxProxyPool(mailboxHandle, folderIds, out objData);
			});
			objectData = objData;
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_CreateFolder(long mailboxHandle, FolderRec sourceFolder, bool failIfExists)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationMailbox_CreateFolder(mailboxHandle, sourceFolder, failIfExists);
			});
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_CreateFolder2(long mailboxHandle, FolderRec sourceFolder, bool failIfExists, out byte[] newFolderId)
		{
			byte[] newFolderIdInt = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationMailbox_CreateFolder2(mailboxHandle, sourceFolder, failIfExists, out newFolderIdInt);
			});
			newFolderId = newFolderIdInt;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_CreateFolder3(long mailboxHandle, FolderRec sourceFolder, int createFolderFlags, out byte[] newFolderId)
		{
			byte[] newFolderIdInt = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationMailbox_CreateFolder3(mailboxHandle, sourceFolder, createFolderFlags, out newFolderIdInt);
			});
			newFolderId = newFolderIdInt;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_MoveFolder(long mailboxHandle, byte[] folderId, byte[] oldParentId, byte[] newParentId)
		{
			this.CallService(delegate()
			{
				this.Channel.IDestinationMailbox_MoveFolder(mailboxHandle, folderId, oldParentId, newParentId);
			});
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_DeleteFolder(long mailboxHandle, FolderRec folderRec)
		{
			this.CallService(delegate()
			{
				this.Channel.IDestinationMailbox_DeleteFolder(mailboxHandle, folderRec);
			});
		}

		DataExportBatch IMailboxReplicationProxyService.IDestinationMailbox_LoadSyncState2(long mailboxHandle, byte[] key)
		{
			DataExportBatch result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.IDestinationMailbox_LoadSyncState2(mailboxHandle, key);
			});
			return result;
		}

		long IMailboxReplicationProxyService.IDestinationMailbox_SaveSyncState2(long mailboxHandle, byte[] key, DataExportBatch firstBatch)
		{
			long result = -1L;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.IDestinationMailbox_SaveSyncState2(mailboxHandle, key, firstBatch);
			});
			return result;
		}

		void IMailboxReplicationProxyService.CloseHandle(long handle)
		{
			this.CallService(delegate()
			{
				this.Channel.CloseHandle(handle);
			});
		}

		DataExportBatch IMailboxReplicationProxyService.DataExport_ExportData2(long dataExportHandle)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.DataExport_ExportData2(dataExportHandle);
			});
			return result;
		}

		void IMailboxReplicationProxyService.DataExport_CancelExport(long dataExportHandle)
		{
			this.CallService(delegate()
			{
				this.Channel.DataExport_CancelExport(dataExportHandle);
			});
		}

		void IMailboxReplicationProxyService.IDataImport_ImportBuffer(long dataImportHandle, int opcode, byte[] data)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				try
				{
					this.Channel.IDataImport_ImportBuffer(dataImportHandle, opcode, data);
				}
				catch (TimeoutException ex)
				{
					MrsTracer.ProxyClient.Warning("Import buffer timed out. {0}", new object[]
					{
						ex
					});
					throw;
				}
			});
		}

		void IMailboxReplicationProxyService.IDataImport_Flush(long dataImportHandle)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDataImport_Flush(dataImportHandle);
			});
		}

		FolderRec IMailboxReplicationProxyService.IFolder_GetFolderRec2(long folderHandle, int[] additionalPtagsToLoad)
		{
			FolderRec result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IFolder_GetFolderRec2(folderHandle, additionalPtagsToLoad);
			});
			return result;
		}

		FolderRec IMailboxReplicationProxyService.IFolder_GetFolderRec3(long folderHandle, int[] additionalPtagsToLoad, int flags)
		{
			FolderRec result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IFolder_GetFolderRec3(folderHandle, additionalPtagsToLoad, flags);
			});
			return result;
		}

		List<MessageRec> IMailboxReplicationProxyService.IFolder_EnumerateMessagesPaged2(long folderHandle, EnumerateMessagesFlags emFlags, int[] additionalPtagsToLoad, out bool moreData)
		{
			List<MessageRec> result = null;
			bool moreDataInt = false;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.IFolder_EnumerateMessagesPaged2(folderHandle, emFlags, additionalPtagsToLoad, out moreDataInt);
			});
			moreData = moreDataInt;
			return result;
		}

		List<MessageRec> IMailboxReplicationProxyService.IFolder_EnumerateMessagesNextBatch(long folderHandle, out bool moreData)
		{
			List<MessageRec> result = null;
			bool moreDataInt = false;
			this.CallService(delegate()
			{
				result = this.Channel.IFolder_EnumerateMessagesNextBatch(folderHandle, out moreDataInt);
			});
			moreData = moreDataInt;
			return result;
		}

		byte[] IMailboxReplicationProxyService.IFolder_GetSecurityDescriptor(long folderHandle, int secProp)
		{
			byte[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IFolder_GetSecurityDescriptor(folderHandle, secProp);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IFolder_SetContentsRestriction(long folderHandle, RestrictionData restriction)
		{
			this.CallService(delegate()
			{
				this.Channel.IFolder_SetContentsRestriction(folderHandle, restriction);
			});
		}

		PropValueData[] IMailboxReplicationProxyService.IFolder_GetProps(long folderHandle, int[] pta)
		{
			PropValueData[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IFolder_GetProps(folderHandle, pta);
			});
			return result;
		}

		PropProblemData[] IMailboxReplicationProxyService.IFolder_SetProps(long folderHandle, PropValueData[] pva)
		{
			PropProblemData[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IFolder_SetProps(folderHandle, pva);
			});
			return result;
		}

		RuleData[] IMailboxReplicationProxyService.IFolder_GetRules(long folderHandle, int[] extraProps)
		{
			RuleData[] result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.IFolder_GetRules(folderHandle, extraProps);
			});
			return result;
		}

		PropValueData[][] IMailboxReplicationProxyService.IFolder_GetACL(long folderHandle, int secProp)
		{
			PropValueData[][] result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.IFolder_GetACL(folderHandle, secProp);
			});
			return result;
		}

		PropValueData[][] IMailboxReplicationProxyService.IFolder_GetExtendedAcl(long folderHandle, int aclFlags)
		{
			PropValueData[][] result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.IFolder_GetExtendedAcl(folderHandle, aclFlags);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IFolder_DeleteMessages(long folderHandle, byte[][] entryIds)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IFolder_DeleteMessages(folderHandle, entryIds);
			});
		}

		PropValueData[] IMailboxReplicationProxyService.ISourceFolder_GetProps(long folderHandle, int[] pta)
		{
			PropValueData[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceFolder_GetProps(folderHandle, pta);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IFolder_GetSearchCriteria(long folderHandle, out RestrictionData restrictionData, out byte[][] entryIDs, out int searchState)
		{
			RestrictionData rd = null;
			byte[][] eids = null;
			int ss = -1;
			this.CallService(delegate()
			{
				this.Channel.IFolder_GetSearchCriteria(folderHandle, out rd, out eids, out ss);
			});
			restrictionData = rd;
			entryIDs = eids;
			searchState = ss;
		}

		List<MessageRec> IMailboxReplicationProxyService.IFolder_LookupMessages(long folderHandle, int ptagToLookup, byte[][] keysToLookup, int[] additionalPtagsToLoad)
		{
			List<MessageRec> result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.IFolder_LookupMessages(folderHandle, ptagToLookup, keysToLookup, additionalPtagsToLoad);
			});
			return result;
		}

		void IMailboxReplicationProxyService.ISourceFolder_GetSearchCriteria(long folderHandle, out RestrictionData restrictionData, out byte[][] entryIDs, out int searchState)
		{
			RestrictionData rd = null;
			byte[][] eids = null;
			int ss = -1;
			this.CallService(delegate()
			{
				this.Channel.ISourceFolder_GetSearchCriteria(folderHandle, out rd, out eids, out ss);
			});
			restrictionData = rd;
			entryIDs = eids;
			searchState = ss;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceFolder_CopyTo(long folderHandle, int flags, int[] excludeTags, byte[] targetObjectData)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceFolder_CopyTo(folderHandle, flags, excludeTags, targetObjectData);
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceFolder_Export2(long folderHandle, int[] excludeTags, byte[] targetObjectData)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceFolder_Export2(folderHandle, excludeTags, targetObjectData);
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceFolder_ExportMessages(long folderHandle, int flags, byte[][] entryIds, byte[] targetObjectData)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceFolder_ExportMessages(folderHandle, flags, entryIds, targetObjectData);
			});
			return result;
		}

		FolderChangesManifest IMailboxReplicationProxyService.ISourceFolder_EnumerateChanges(long folderHandle, bool catchup)
		{
			FolderChangesManifest result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.ISourceFolder_EnumerateChanges(folderHandle, catchup);
			});
			return result;
		}

		FolderChangesManifest IMailboxReplicationProxyService.ISourceFolder_EnumerateChanges2(long folderHandle, int flags, int maxChanges)
		{
			FolderChangesManifest result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.ISourceFolder_EnumerateChanges2(folderHandle, flags, maxChanges);
			});
			return result;
		}

		List<MessageRec> IMailboxReplicationProxyService.ISourceFolder_EnumerateMessagesPaged(long folderHandle, int maxPageSize)
		{
			List<MessageRec> result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.ISourceFolder_EnumerateMessagesPaged(folderHandle, maxPageSize);
			});
			return result;
		}

		int IMailboxReplicationProxyService.ISourceFolder_GetEstimatedItemCount(long folderHandle)
		{
			int result = 0;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceFolder_GetEstimatedItemCount(folderHandle);
			});
			return result;
		}

		PropProblemData[] IMailboxReplicationProxyService.IDestinationFolder_SetProps(long folderHandle, PropValueData[] pva)
		{
			PropProblemData[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationFolder_SetProps(folderHandle, pva);
			});
			return result;
		}

		PropProblemData[] IMailboxReplicationProxyService.IDestinationFolder_SetSecurityDescriptor(long folderHandle, int secProp, byte[] sdData)
		{
			PropProblemData[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationFolder_SetSecurityDescriptor(folderHandle, secProp, sdData);
			});
			return result;
		}

		bool IMailboxReplicationProxyService.IDestinationFolder_SetSearchCriteria(long folderHandle, RestrictionData restriction, byte[][] entryIDs, int searchFlags)
		{
			bool result = false;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationFolder_SetSearchCriteria(folderHandle, restriction, entryIDs, searchFlags);
			});
			return result;
		}

		long IMailboxReplicationProxyService.IDestinationFolder_GetFxProxy2(long folderHandle, int flags, out byte[] objectData)
		{
			long result = -1L;
			byte[] objData = null;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationFolder_GetFxProxy2(folderHandle, flags, out objData);
			});
			objectData = objData;
			return result;
		}

		long IMailboxReplicationProxyService.IDestinationFolder_GetFxProxy(long folderHandle, out byte[] objectData)
		{
			long result = -1L;
			byte[] objData = null;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationFolder_GetFxProxy(folderHandle, out objData);
			});
			objectData = objData;
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationFolder_DeleteMessages(long folderHandle, byte[][] entryIds)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationFolder_DeleteMessages(folderHandle, entryIds);
			});
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetReadFlagsOnMessages(long folderHandle, int flags, byte[][] entryIds)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationFolder_SetReadFlagsOnMessages(folderHandle, flags, entryIds);
			});
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetMessageProps(long folderHandle, byte[] entryId, PropValueData[] propValues)
		{
			this.CallService(delegate()
			{
				this.Channel.IDestinationFolder_SetMessageProps(folderHandle, entryId, propValues);
			});
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetRules(long folderHandle, RuleData[] rules)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationFolder_SetRules(folderHandle, rules);
			});
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetACL(long folderHandle, int secProp, PropValueData[][] aclData)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationFolder_SetACL(folderHandle, secProp, aclData);
			});
		}

		void IMailboxReplicationProxyService.IDestinationFolder_SetExtendedAcl(long folderHandle, int aclFlags, PropValueData[][] aclData)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationFolder_SetExtendedAcl(folderHandle, aclFlags, aclData);
			});
		}

		Guid IMailboxReplicationProxyService.IDestinationFolder_LinkMailPublicFolder(long folderHandle, LinkMailPublicFolderFlags flags, byte[] objectId)
		{
			Guid result = Guid.Empty;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationFolder_LinkMailPublicFolder(folderHandle, flags, objectId);
			});
			return result;
		}

		long IMailboxReplicationProxyService.IMailbox_Config3(Guid primaryMailboxGuid, Guid physicalMailboxGuid, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags)
		{
			long result = -1L;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_Config3(primaryMailboxGuid, physicalMailboxGuid, mdbGuid, mdbName, mbxType, proxyControlFlags);
			});
			return result;
		}

		long IMailboxReplicationProxyService.IMailbox_Config4(Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMailboxFlags)
		{
			long result = -1L;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_Config4(primaryMailboxGuid, physicalMailboxGuid, partitionHint, mdbGuid, mdbName, mbxType, proxyControlFlags, localMailboxFlags);
			});
			return result;
		}

		long IMailboxReplicationProxyService.IMailbox_Config5(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMailboxFlags)
		{
			long result = -1L;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_Config5(reservationId, primaryMailboxGuid, physicalMailboxGuid, partitionHint, mdbGuid, mdbName, mbxType, proxyControlFlags, localMailboxFlags);
			});
			return result;
		}

		long IMailboxReplicationProxyService.IMailbox_Config6(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, string filePath, byte[] partitionHint, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMailboxFlags)
		{
			long result = -1L;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_Config6(reservationId, primaryMailboxGuid, physicalMailboxGuid, filePath, partitionHint, mdbGuid, mdbName, mbxType, proxyControlFlags, localMailboxFlags);
			});
			return result;
		}

		long IMailboxReplicationProxyService.IMailbox_Config7(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMailboxFlags, Guid? mailboxContainerGuid)
		{
			long result = -1L;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_Config7(reservationId, primaryMailboxGuid, physicalMailboxGuid, partitionHint, mdbGuid, mdbName, mbxType, proxyControlFlags, localMailboxFlags, mailboxContainerGuid);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigureProxyService(ProxyConfiguration configuration)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigureProxyService(configuration);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigADConnection(long mailboxHandle, string domainControllerName, string userName, string userDomain, string userPassword)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigADConnection(mailboxHandle, domainControllerName, userName, userDomain, userPassword);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigEas(long mailboxHandle, string password, string address)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigEas(mailboxHandle, password, address);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigEas2(long mailboxHandle, string password, string address, Guid mailboxGuid, string remoteHostName)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigEas2(mailboxHandle, password, address, mailboxGuid, remoteHostName);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigOlc(long mailboxHandle, OlcMailboxConfiguration config)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigOlc(mailboxHandle, config);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigPreferredADConnection(long mailboxHandle, string preferredDomainControllerName)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigPreferredADConnection(mailboxHandle, preferredDomainControllerName);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigPst(long mailboxHandle, string filePath, int? contentCodePage)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigPst(mailboxHandle, filePath, contentCodePage);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigRestore(long mailboxHandle, int restoreFlags)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigRestore(mailboxHandle, restoreFlags);
			});
		}

		MailboxInformation IMailboxReplicationProxyService.IMailbox_GetMailboxInformation(long mailboxHandle)
		{
			MailboxInformation result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetMailboxInformation(mailboxHandle);
			});
			return result;
		}

		Guid IMailboxReplicationProxyService.IReservationManager_ReserveResources(Guid mailboxGuid, byte[] partitionHintBytes, Guid mdbGuid, int flags)
		{
			Guid result = Guid.Empty;
			this.CallService(delegate()
			{
				result = this.Channel.IReservationManager_ReserveResources(mailboxGuid, partitionHintBytes, mdbGuid, flags);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IReservationManager_ReleaseResources(Guid reservationId)
		{
			this.CallService(delegate()
			{
				this.Channel.IReservationManager_ReleaseResources(reservationId);
			});
		}

		int IMailboxReplicationProxyService.IMailbox_ReserveResources(Guid reservationId, Guid resourceId, int reservationType)
		{
			int status = 0;
			this.CallService(delegate()
			{
				status = this.Channel.IMailbox_ReserveResources(reservationId, resourceId, reservationType);
			});
			return status;
		}

		void IMailboxReplicationProxyService.IMailbox_Connect(long mailboxHandle)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_Connect(mailboxHandle);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_Connect2(long mailboxHandle, int connectFlags)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_Connect2(mailboxHandle, connectFlags);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_Disconnect(long mailboxHandle)
		{
			MailboxReplicationProxyClient.KeepAlivePinger.Remove(this.RequestContext.Id);
			this.CallService(delegate()
			{
				this.Channel.IMailbox_Disconnect(mailboxHandle);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_ConfigMailboxOptions(long mailboxHandle, int options)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_ConfigMailboxOptions(mailboxHandle, options);
			});
		}

		MailboxServerInformation IMailboxReplicationProxyService.IMailbox_GetMailboxServerInformation(long mailboxHandle)
		{
			MailboxServerInformation result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetMailboxServerInformation(mailboxHandle);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_SetOtherSideVersion(long mailboxHandle, VersionInformation otherSideInfo)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_SetOtherSideVersion(mailboxHandle, otherSideInfo);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_SetInTransitStatus(long mailboxHandle, int status, out bool onlineMoveSupported)
		{
			bool oms = false;
			this.CallService(delegate()
			{
				this.Channel.IMailbox_SetInTransitStatus(mailboxHandle, status, out oms);
			});
			onlineMoveSupported = oms;
		}

		void IMailboxReplicationProxyService.IMailbox_SeedMBICache(long mailboxHandle)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_SeedMBICache(mailboxHandle);
			});
		}

		List<FolderRec> IMailboxReplicationProxyService.IMailbox_EnumerateFolderHierarchyPaged2(long mailboxHandle, EnumerateFolderHierarchyFlags flags, int[] additionalPtagsToLoad, out bool moreData)
		{
			List<FolderRec> result = null;
			bool moreDataInt = false;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.IMailbox_EnumerateFolderHierarchyPaged2(mailboxHandle, flags, additionalPtagsToLoad, out moreDataInt);
			});
			moreData = moreDataInt;
			return result;
		}

		List<FolderRec> IMailboxReplicationProxyService.IMailbox_EnumerateFolderHierarchyNextBatch(long mailboxHandle, out bool moreData)
		{
			List<FolderRec> result = null;
			bool moreDataInt = false;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_EnumerateFolderHierarchyNextBatch(mailboxHandle, out moreDataInt);
			});
			moreData = moreDataInt;
			return result;
		}

		List<WellKnownFolder> IMailboxReplicationProxyService.IMailbox_DiscoverWellKnownFolders(long mailboxHandle, int flags)
		{
			List<WellKnownFolder> result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_DiscoverWellKnownFolders(mailboxHandle, flags);
			});
			return result;
		}

		bool IMailboxReplicationProxyService.IMailbox_IsMailboxCapabilitySupported(long mailboxHandle, MailboxCapabilities capability)
		{
			bool result = false;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_IsMailboxCapabilitySupported(mailboxHandle, capability);
			});
			return result;
		}

		bool IMailboxReplicationProxyService.IMailbox_IsMailboxCapabilitySupported2(long mailboxHandle, int capability)
		{
			bool result = false;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_IsMailboxCapabilitySupported2(mailboxHandle, capability);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_DeleteMailbox(long mailboxHandle, int flags)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_DeleteMailbox(mailboxHandle, flags);
			});
		}

		NamedPropData[] IMailboxReplicationProxyService.IMailbox_GetNamesFromIDs(long mailboxHandle, int[] pta)
		{
			NamedPropData[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetNamesFromIDs(mailboxHandle, pta);
			});
			return result;
		}

		int[] IMailboxReplicationProxyService.IMailbox_GetIDsFromNames(long mailboxHandle, bool createIfNotExists, NamedPropData[] npa)
		{
			int[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetIDsFromNames(mailboxHandle, createIfNotExists, npa);
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IMailbox_GetSessionSpecificEntryId(long mailboxHandle, byte[] entryId)
		{
			byte[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetSessionSpecificEntryId(mailboxHandle, entryId);
			});
			return result;
		}

		bool IMailboxReplicationProxyService.IMailbox_UpdateRemoteHostName(long mailboxHandle, string value)
		{
			bool result = false;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_UpdateRemoteHostName(mailboxHandle, value);
			});
			return result;
		}

		string IMailboxReplicationProxyService.IMailbox_GetADUser(long mailboxHandle)
		{
			string result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetADUser(mailboxHandle);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_UpdateMovedMailbox(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries)
		{
			entries = null;
			string tempEntries = null;
			try
			{
				this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
				{
					this.Channel.IMailbox_UpdateMovedMailbox(mailboxHandle, op, remoteRecipientData, domainController, out tempEntries);
				});
			}
			finally
			{
				entries = tempEntries;
			}
		}

		void IMailboxReplicationProxyService.IMailbox_UpdateMovedMailbox2(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus)
		{
			entries = null;
			string tempEntries = null;
			try
			{
				this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
				{
					this.Channel.IMailbox_UpdateMovedMailbox2(mailboxHandle, op, remoteRecipientData, domainController, out tempEntries, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, archiveStatus);
				});
			}
			finally
			{
				entries = tempEntries;
			}
		}

		void IMailboxReplicationProxyService.IMailbox_UpdateMovedMailbox3(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus, int updateMovedMailboxFlags)
		{
			entries = null;
			string tempEntries = null;
			try
			{
				this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
				{
					this.Channel.IMailbox_UpdateMovedMailbox3(mailboxHandle, op, remoteRecipientData, domainController, out tempEntries, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, archiveStatus, updateMovedMailboxFlags);
				});
			}
			finally
			{
				entries = tempEntries;
			}
		}

		void IMailboxReplicationProxyService.IMailbox_UpdateMovedMailbox4(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus, int updateMovedMailboxFlags, Guid? newMailboxContainerGuid, byte[] newUnifiedMailboxIdData)
		{
			entries = null;
			string tempEntries = null;
			try
			{
				this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
				{
					this.Channel.IMailbox_UpdateMovedMailbox4(mailboxHandle, op, remoteRecipientData, domainController, out tempEntries, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, archiveStatus, updateMovedMailboxFlags, newMailboxContainerGuid, newUnifiedMailboxIdData);
				});
			}
			finally
			{
				entries = tempEntries;
			}
		}

		MappedPrincipal[] IMailboxReplicationProxyService.IMailbox_GetPrincipalsFromMailboxGuids(long mailboxHandle, Guid[] mailboxGuids)
		{
			MappedPrincipal[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetPrincipalsFromMailboxGuids(mailboxHandle, mailboxGuids);
			});
			return result;
		}

		Guid[] IMailboxReplicationProxyService.IMailbox_GetMailboxGuidsFromPrincipals(long mailboxHandle, MappedPrincipal[] principals)
		{
			Guid[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetMailboxGuidsFromPrincipals(mailboxHandle, principals);
			});
			return result;
		}

		MappedPrincipal[] IMailboxReplicationProxyService.IMailbox_ResolvePrincipals(long mailboxHandle, MappedPrincipal[] principals)
		{
			MappedPrincipal[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_ResolvePrincipals(mailboxHandle, principals);
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IMailbox_GetMailboxSecurityDescriptor(long mailboxHandle)
		{
			byte[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetMailboxSecurityDescriptor(mailboxHandle);
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IMailbox_GetUserSecurityDescriptor(long mailboxHandle)
		{
			byte[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetUserSecurityDescriptor(mailboxHandle);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IMailbox_AddMoveHistoryEntry(long mailboxHandle, string mheData, int maxMoveHistoryLength)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_AddMoveHistoryEntry(mailboxHandle, mheData, maxMoveHistoryLength);
			});
		}

		void IMailboxReplicationProxyService.IMailbox_CheckServerHealth(long mailboxHandle)
		{
			this.CallService(delegate()
			{
				this.Channel.IMailbox_CheckServerHealth(mailboxHandle);
			});
		}

		ServerHealthStatus IMailboxReplicationProxyService.IMailbox_CheckServerHealth2(long mailboxHandle)
		{
			ServerHealthStatus serverHealthStatus = new ServerHealthStatus(ServerHealthState.Healthy);
			this.CallService(delegate()
			{
				serverHealthStatus = this.Channel.IMailbox_CheckServerHealth2(mailboxHandle);
			});
			return serverHealthStatus;
		}

		PropValueData[] IMailboxReplicationProxyService.IMailbox_GetProps(long mailboxHandle, int[] ptags)
		{
			PropValueData[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetProps(mailboxHandle, ptags);
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.IMailbox_GetReceiveFolderEntryId(long mailboxHandle, string msgClass)
		{
			byte[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetReceiveFolderEntryId(mailboxHandle, msgClass);
			});
			return result;
		}

		SessionStatistics IMailboxReplicationProxyService.IMailbox_GetSessionStatistics(long mailboxHandle, int statisticsTypes)
		{
			SessionStatistics result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_GetSessionStatistics(mailboxHandle, statisticsTypes);
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.ISourceMailbox_GetMailboxBasicInfo(long mailboxHandle)
		{
			byte[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_GetMailboxBasicInfo(mailboxHandle);
			});
			return result;
		}

		byte[] IMailboxReplicationProxyService.ISourceMailbox_GetMailboxBasicInfo2(long mailboxHandle, int signatureFlags)
		{
			byte[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_GetMailboxBasicInfo2(mailboxHandle, signatureFlags);
			});
			return result;
		}

		long IMailboxReplicationProxyService.ISourceMailbox_GetFolder(long mailboxHandle, byte[] entryId)
		{
			long result = -1L;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_GetFolder(mailboxHandle, entryId);
			});
			return result;
		}

		PropValueData[] IMailboxReplicationProxyService.ISourceMailbox_GetProps(long mailboxHandle, int[] ptags)
		{
			PropValueData[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_GetProps(mailboxHandle, ptags);
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_Export2(long mailboxHandle, int[] excludeProps, byte[] targetObjectData)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_Export2(mailboxHandle, excludeProps, targetObjectData);
			});
			return result;
		}

		MailboxChangesManifest IMailboxReplicationProxyService.ISourceMailbox_EnumerateHierarchyChanges(long mailboxHandle, bool catchup)
		{
			MailboxChangesManifest result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.ISourceMailbox_EnumerateHierarchyChanges(mailboxHandle, catchup);
			});
			return result;
		}

		MailboxChangesManifest IMailboxReplicationProxyService.ISourceMailbox_EnumerateHierarchyChanges2(long mailboxHandle, int flags, int maxChanges)
		{
			MailboxChangesManifest result = null;
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				result = this.Channel.ISourceMailbox_EnumerateHierarchyChanges2(mailboxHandle, flags, maxChanges);
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_GetMailboxSyncState(long mailboxHandle)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_GetMailboxSyncState(mailboxHandle);
			});
			return result;
		}

		long IMailboxReplicationProxyService.ISourceMailbox_SetMailboxSyncState(long mailboxHandle, DataExportBatch firstBatch)
		{
			long result = -1L;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_SetMailboxSyncState(mailboxHandle, firstBatch);
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_ExportMessageBatch2(long mailboxHandle, List<MessageRec> messages, byte[] targetObjectData)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_ExportMessageBatch2(mailboxHandle, messages, targetObjectData);
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_ExportMessages(long mailboxHandle, List<MessageRec> messages, int flags, int[] excludeProps, byte[] targetObjectData)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_ExportMessages(mailboxHandle, messages, flags, excludeProps, targetObjectData);
			});
			return result;
		}

		DataExportBatch IMailboxReplicationProxyService.ISourceMailbox_ExportFolders(long mailboxHandle, List<byte[]> folderIds, int exportFoldersDataToCopyFlags, int folderRecFlags, int[] additionalFolderRecProps, int copyPropertiesFlags, int[] excludeProps, int extendedAclFlags)
		{
			DataExportBatch result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_ExportFolders(mailboxHandle, folderIds, exportFoldersDataToCopyFlags, folderRecFlags, additionalFolderRecProps, copyPropertiesFlags, excludeProps, extendedAclFlags);
			});
			return result;
		}

		List<ReplayActionResult> IMailboxReplicationProxyService.ISourceMailbox_ReplayActions(long mailboxHandle, List<ReplayAction> actions)
		{
			List<ReplayActionResult> result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_ReplayActions(mailboxHandle, actions);
			});
			return result;
		}

		public List<ItemPropertiesBase> ISourceMailbox_GetMailboxSettings(long mailboxHandle, int flags)
		{
			List<ItemPropertiesBase> result = null;
			this.CallService(delegate()
			{
				result = this.Channel.ISourceMailbox_GetMailboxSettings(mailboxHandle, flags);
			});
			return result;
		}

		Guid IMailboxReplicationProxyService.IMailbox_StartIsInteg(long mailboxHandle, List<uint> mailboxCorruptionTypes)
		{
			Guid result = Guid.Empty;
			this.CallService(delegate()
			{
				result = this.Channel.IMailbox_StartIsInteg(mailboxHandle, mailboxCorruptionTypes);
			});
			return result;
		}

		List<StoreIntegrityCheckJob> IMailboxReplicationProxyService.IMailbox_QueryIsInteg(long mailboxHandle, Guid isIntegRequestGuid)
		{
			List<StoreIntegrityCheckJob> jobs = null;
			this.CallService(delegate()
			{
				jobs = this.Channel.IMailbox_QueryIsInteg(mailboxHandle, isIntegRequestGuid);
			});
			return jobs;
		}

		bool IMailboxReplicationProxyService.IDestinationMailbox_MailboxExists(long mailboxHandle)
		{
			bool result = false;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationMailbox_MailboxExists(mailboxHandle);
			});
			return result;
		}

		CreateMailboxResult IMailboxReplicationProxyService.IDestinationMailbox_CreateMailbox(long mailboxHandle, byte[] mailboxData)
		{
			CreateMailboxResult result = CreateMailboxResult.Success;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationMailbox_CreateMailbox(mailboxHandle, mailboxData);
			});
			return result;
		}

		CreateMailboxResult IMailboxReplicationProxyService.IDestinationMailbox_CreateMailbox2(long mailboxHandle, byte[] mailboxData, int sourceSignatureFlags)
		{
			CreateMailboxResult result = CreateMailboxResult.Success;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationMailbox_CreateMailbox2(mailboxHandle, mailboxData, sourceSignatureFlags);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_ProcessMailboxSignature(long mailboxHandle, byte[] mailboxData)
		{
			this.CallService(delegate()
			{
				this.Channel.IDestinationMailbox_ProcessMailboxSignature(mailboxHandle, mailboxData);
			});
		}

		long IMailboxReplicationProxyService.IDestinationMailbox_GetFolder(long mailboxHandle, byte[] entryId)
		{
			long result = -1L;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationMailbox_GetFolder(mailboxHandle, entryId);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_SetMailboxSecurityDescriptor(long mailboxHandle, byte[] sdData)
		{
			this.CallService(delegate()
			{
				this.Channel.IDestinationMailbox_SetMailboxSecurityDescriptor(mailboxHandle, sdData);
			});
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_SetUserSecurityDescriptor(long mailboxHandle, byte[] sdData)
		{
			this.CallService(delegate()
			{
				this.Channel.IDestinationMailbox_SetUserSecurityDescriptor(mailboxHandle, sdData);
			});
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_PreFinalSyncDataProcessing(long mailboxHandle, int? sourceMailboxVersion)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationMailbox_PreFinalSyncDataProcessing(mailboxHandle, sourceMailboxVersion);
			});
		}

		int IMailboxReplicationProxyService.IDestinationMailbox_CheckDataGuarantee(long mailboxHandle, DateTime commitTimestamp, out byte[] failureReasonData)
		{
			int result = -1;
			byte[] frd = null;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationMailbox_CheckDataGuarantee(mailboxHandle, commitTimestamp, out frd);
			});
			failureReasonData = frd;
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_ForceLogRoll(long mailboxHandle)
		{
			this.CallService(delegate()
			{
				this.Channel.IDestinationMailbox_ForceLogRoll(mailboxHandle);
			});
		}

		List<ReplayAction> IMailboxReplicationProxyService.IDestinationMailbox_GetActions(long mailboxHandle, string replaySyncState, int maxNumberOfActions)
		{
			List<ReplayAction> result = null;
			this.CallService(delegate()
			{
				result = this.Channel.IDestinationMailbox_GetActions(mailboxHandle, replaySyncState, maxNumberOfActions);
			});
			return result;
		}

		void IMailboxReplicationProxyService.IDestinationMailbox_SetMailboxSettings(long mailboxHandle, ItemPropertiesBase item)
		{
			this.CallServiceWithTimeout(this.LongOperationTimeout, delegate
			{
				this.Channel.IDestinationMailbox_SetMailboxSettings(mailboxHandle, item);
			});
		}

		MigrationAccount[] IMailboxReplicationProxyService.SelectAccountsToMigrate(long maximumAccounts, long? maximumTotalSize, int? constraintId)
		{
			MigrationAccount[] result = null;
			this.CallService(delegate()
			{
				result = this.Channel.SelectAccountsToMigrate(maximumAccounts, maximumTotalSize, constraintId);
			});
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
			if (disposing)
			{
				this.RequestContext.Unregister();
			}
		}

		protected override IMailboxReplicationProxyService CreateChannel()
		{
			return ExchangeSessionAwareClientsHelper.CreateChannel(this);
		}

		protected override void HandleFaultException(FaultException<MailboxReplicationServiceFault> fault, string context)
		{
			fault.Detail.ReconstructAndThrow(context, base.ServerVersion);
		}

		private static bool KeepAlivePingerShouldRemove(Guid key, MailboxReplicationProxyClient client)
		{
			bool flag = false;
			try
			{
				Monitor.TryEnter(client.serviceCallLock, ref flag);
				if (flag)
				{
					if (client.State != CommunicationState.Opened)
					{
						return true;
					}
					client.Ping();
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(client.serviceCallLock);
				}
				else
				{
					client.pingPostponed = true;
				}
			}
			return false;
		}

		private void Init(Guid physicalMbxGuid, Guid primaryMbxGuid, string filePath, string targetDatabase, TenantPartitionHint partitionHint, ProxyControlFlags proxyControlFlags, TimeSpan longOperationTimeout)
		{
			this.primaryMbxGuid = primaryMbxGuid;
			this.partitionHint = partitionHint;
			this.proxyControlFlags = proxyControlFlags;
			this.LongOperationTimeout = longOperationTimeout;
			this.lastFailure = null;
			CertificateValidationManager.RegisterCallback(base.GetType().FullName, new RemoteCertificateValidationCallback(ExchangeCertificateValidator.CertificateValidatorCallback));
			this.RequestContext = new MRSProxyRequestContext();
			this.RequestContext.HttpHeaders[WellKnownHeader.ClientVersion] = Microsoft.Exchange.Data.ServerVersion.InstalledVersion.ToString();
			this.RequestContext.HttpHeaders[CertificateValidationManager.ComponentIdHeaderName] = CertificateValidationManager.GenerateComponentIdQueryString(base.GetType().FullName);
			if (!this.proxyControlFlags.HasFlag(ProxyControlFlags.DoNotAddIdentifyingCafeHeaders))
			{
				if (physicalMbxGuid != Guid.Empty)
				{
					this.RequestContext.HttpHeaders[WellKnownHeader.AnchorMailbox] = physicalMbxGuid.ToString();
				}
				if (!string.IsNullOrEmpty(filePath))
				{
					this.RequestContext.HttpHeaders[WellKnownHeader.GenericAnchorHint] = Convert.ToBase64String(Encoding.Unicode.GetBytes(filePath));
				}
				if (targetDatabase != null)
				{
					this.RequestContext.HttpHeaders[WellKnownHeader.TargetDatabase] = targetDatabase;
				}
			}
		}

		private void Ping()
		{
			this.pingPostponed = false;
			TimeSpan proxyClientPingInterval = TestIntegration.Instance.ProxyClientPingInterval;
			MrsTracer.ProxyClient.Debug("KeepAlive pinger making dummy wcf call, previous call time {0}, timeout interval {1}", new object[]
			{
				this.lastCallTimeStamp,
				proxyClientPingInterval
			});
			Stopwatch watch = Stopwatch.StartNew();
			CommonUtils.CatchKnownExceptions(delegate
			{
				this.ExchangeVersionInformation();
				this.latencyInfo.AddSample((int)watch.ElapsedMilliseconds);
				watch.Stop();
			}, null);
		}

		private void Initialize(string serverName, NetworkCredential remoteCred, bool useHttps)
		{
			ExchangeCertificateValidator.Initialize();
			Uri uri = base.Endpoint.Address.Uri;
			string uri2 = string.Format("{0}://{1}{2}", uri.Scheme, serverName, uri.PathAndQuery);
			try
			{
				base.Endpoint.Address = new EndpointAddress(uri2);
			}
			catch (UriFormatException innerException)
			{
				MrsTracer.ProxyClient.Error("Invalid ServerName in MRSProxyClient.Create", new object[0]);
				throw new InvalidServerNamePermanentException(serverName, innerException);
			}
			this.RequestContext.EndpointUri = base.Endpoint.Address.Uri;
			if (useHttps)
			{
				Server localServer = LocalServerCache.LocalServer;
				if (localServer != null && localServer.InternetWebProxy != null)
				{
					MrsTracer.ProxyClient.Debug("Using custom InternetWebProxy {0}", new object[]
					{
						localServer.InternetWebProxy
					});
					CustomBinding customBinding = base.Endpoint.Binding as CustomBinding;
					if (customBinding != null)
					{
						HttpsTransportBindingElement httpsTransportBindingElement = customBinding.Elements.Find<HttpsTransportBindingElement>();
						if (httpsTransportBindingElement != null)
						{
							httpsTransportBindingElement.UseDefaultWebProxy = false;
							httpsTransportBindingElement.ProxyAddress = localServer.InternetWebProxy;
							httpsTransportBindingElement.BypassProxyOnLocal = true;
						}
					}
				}
			}
			base.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
			base.ClientCredentials.Windows.ClientCredential = remoteCred;
			base.Endpoint.Behaviors.Add(new MaxFaultSizeBehavior(10485760));
			this.ExchangeVersionInformation();
			MailboxReplicationProxyClient.KeepAlivePinger.TryInsertAbsolute(this.RequestContext.Id, this, TestIntegration.Instance.ProxyClientPingInterval);
		}

		private void CallService(Action serviceCall)
		{
			this.CallServiceWithTimeout(MailboxReplicationProxyClient.DefaultOperationTimeout, serviceCall);
		}

		private void CallServiceWithTimeout(TimeSpan timeout, Action serviceCall)
		{
			lock (this.serviceCallLock)
			{
				if (this.lastCallTimeStamp != DateTime.MinValue)
				{
					TimeSpan timeSpan = DateTime.UtcNow - this.lastCallTimeStamp;
					TimeSpan timeSpan2 = TestIntegration.Instance.ProxyClientPingInterval;
					timeSpan2 += timeSpan2;
					if (timeSpan >= timeSpan2)
					{
						MrsTracer.ProxyClient.Error("The socket connection has been stale for {0}, more than expected {1}", new object[]
						{
							timeSpan,
							timeSpan2
						});
					}
				}
				this.lastCallTimeStamp = DateTime.UtcNow;
				if (base.State == CommunicationState.Faulted)
				{
					string text = base.Endpoint.Address.ToString();
					if (base.ServerVersion != null)
					{
						text += string.Format(" {0} ({1})", base.ServerVersion.ComputerName, base.ServerVersion.ToString());
					}
					throw new CommunicationWithRemoteServiceFailedTransientException(text, this.lastFailure);
				}
				using (new OperationContextScope(base.InnerChannel))
				{
					((IContextChannel)base.Channel).OperationTimeout = timeout;
					OperationContext.Current.OutgoingMessageHeaders.Clear();
					MessageHeader header = MessageHeader.CreateHeader("MailboxGuid", "http://schemas.microsoft.com/exchange/services/2006/types", this.primaryMbxGuid);
					OperationContext.Current.OutgoingMessageHeaders.Add(header);
					MessageHeader header2 = MessageHeader.CreateHeader("PartitionHint", "http://schemas.microsoft.com/exchange/services/2006/types", this.partitionHint);
					OperationContext.Current.OutgoingMessageHeaders.Add(header2);
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null)
					{
						currentActivityScope.SerializeTo(OperationContext.Current);
					}
					string context = base.Endpoint.Address.ToString();
					Stopwatch stopwatch = Stopwatch.StartNew();
					try
					{
						this.CallService(serviceCall, context);
						this.lastFailure = null;
					}
					catch (Exception ex)
					{
						this.lastFailure = ex;
						throw;
					}
					finally
					{
						this.latencyInfo.TotalRemoteCallDuration += stopwatch.Elapsed;
						this.latencyInfo.TotalNumberOfRemoteCalls++;
						this.lastCallTimeStamp = DateTime.UtcNow;
						if (!this.isBackendCookieFetched)
						{
							this.SetBackendCookie();
						}
					}
				}
				if (this.pingPostponed)
				{
					this.Ping();
				}
			}
		}

		private void SetBackendCookie()
		{
			if (base.Endpoint.Address.Uri.Scheme != Uri.UriSchemeHttps && base.Endpoint.Address.Uri.Scheme != Uri.UriSchemeHttp)
			{
				this.isBackendCookieFetched = true;
				return;
			}
			string backendCookieValue = this.GetBackendCookieValue();
			if (string.IsNullOrWhiteSpace(backendCookieValue))
			{
				return;
			}
			this.RequestContext.BackendCookie = new Cookie("X-BackEndCookie", backendCookieValue);
			this.isBackendCookieFetched = true;
		}

		private string GetBackendCookieValue()
		{
			if (OperationContext.Current == null || OperationContext.Current.IncomingMessageProperties == null || !OperationContext.Current.IncomingMessageProperties.ContainsKey(HttpResponseMessageProperty.Name))
			{
				MrsTracer.ProxyClient.Warning("Http response is missing or not initialized.", new object[0]);
				return null;
			}
			HttpResponseMessageProperty httpResponseMessageProperty = OperationContext.Current.IncomingMessageProperties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
			if (httpResponseMessageProperty == null || httpResponseMessageProperty.Headers == null)
			{
				MrsTracer.ProxyClient.Warning("Http response header is missing.", new object[0]);
				return null;
			}
			string text = httpResponseMessageProperty.Headers[HttpResponseHeader.SetCookie];
			if (string.IsNullOrWhiteSpace(text))
			{
				MrsTracer.ProxyClient.Debug("No cookies from server. This is expected when MRS proxy is e14.", new object[0]);
				return null;
			}
			MrsTracer.ProxyClient.Debug("Found cookies: {0}", new object[]
			{
				text
			});
			int num = text.IndexOf("X-BackEndCookie=");
			if (num < 0)
			{
				MrsTracer.ProxyClient.Debug("No backend cookie from server. This is expected when MRS proxy is e14.", new object[0]);
				return null;
			}
			int num2 = num + "X-BackEndCookie=".Length;
			num = text.IndexOf(';', num2);
			if (num < 0)
			{
				MrsTracer.ProxyClient.Warning("Malformed cookie. Cannot locate terminating semicolon.", new object[0]);
				return null;
			}
			return text.Substring(num2, num - num2);
		}

		private void VerifyRequiredCapability(MRSProxyCapabilities requiredCapability)
		{
			if (!base.ServerVersion[(int)requiredCapability])
			{
				MrsTracer.ProxyClient.Error("Talking to downlevel server: no {0} support", new object[]
				{
					requiredCapability.ToString()
				});
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.Endpoint.Address.ToString(), base.ServerVersion.ToString(), requiredCapability.ToString());
			}
		}

		public const string EwsTypeNamespace = "http://schemas.microsoft.com/exchange/services/2006/types";

		public const string MailboxGuidHeaderName = "MailboxGuid";

		private const string PartitionHintHeaderName = "PartitionHint";

		private const string BackendCookieName = "X-BackEndCookie";

		private const string BackendCookiePrefix = "X-BackEndCookie=";

		private static readonly Guid ConnectionTestGuid = new Guid("2985c88d-e426-474b-b7c1-bae258c73db3");

		private static readonly TimeSpan DefaultOperationTimeout = TimeSpan.FromSeconds(50.0);

		private static readonly ExactTimeoutCache<Guid, MailboxReplicationProxyClient> KeepAlivePinger = new ExactTimeoutCache<Guid, MailboxReplicationProxyClient>(null, new ShouldRemoveDelegate<Guid, MailboxReplicationProxyClient>(MailboxReplicationProxyClient.KeepAlivePingerShouldRemove), null, 2000, false);

		private readonly object serviceCallLock = new object();

		private Guid primaryMbxGuid;

		private ProxyControlFlags proxyControlFlags;

		private TenantPartitionHint partitionHint;

		private Exception lastFailure;

		private DateTime lastCallTimeStamp = DateTime.MinValue;

		private LatencyInfo latencyInfo = new LatencyInfo();

		private bool isBackendCookieFetched;

		private bool pingPostponed;
	}
}
