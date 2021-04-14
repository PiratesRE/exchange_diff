using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RemoteSourceMailbox : RemoteMailbox, ISourceMailbox, IMailbox, IDisposable
	{
		public RemoteSourceMailbox(string serverName, string remoteOrgName, NetworkCredential remoteCred, ProxyControlFlags proxyControlFlags, IEnumerable<MRSProxyCapabilities> requiredCapabilities, bool useHttps, LocalMailboxFlags localMailboxFlags) : base(serverName, remoteOrgName, remoteCred, proxyControlFlags, requiredCapabilities, useHttps, localMailboxFlags)
		{
		}

		public static IEnumerable<ContainerMailboxInformation> GetMailboxContainerMailboxes(string serverFQDN, Guid mdbGuid, Guid primaryMailboxGuid)
		{
			IEnumerable<ContainerMailboxInformation> mailboxContainerMailboxes;
			using (MailboxReplicationProxyClient mailboxReplicationProxyClient = MailboxReplicationProxyClient.CreateWithoutThrottling(serverFQDN, null, primaryMailboxGuid, mdbGuid))
			{
				mailboxContainerMailboxes = ((IMailboxReplicationProxyService)mailboxReplicationProxyClient).GetMailboxContainerMailboxes(mdbGuid, primaryMailboxGuid);
			}
			return mailboxContainerMailboxes;
		}

		List<ItemPropertiesBase> ISourceMailbox.GetMailboxSettings(GetMailboxSettingsFlags flags)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.GetMailboxSettings()", new object[0]);
			base.VerifyMailboxConnection();
			return base.MrsProxy.ISourceMailbox_GetMailboxSettings(base.Handle, (int)flags);
		}

		MailboxChangesManifest ISourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.EnumerateHierarchyChanges({0}, {1})", new object[]
			{
				flags,
				maxChanges
			});
			base.VerifyMailboxConnection();
			if (((IMailbox)this).IsMailboxCapabilitySupported(MailboxCapabilities.PagedEnumerateHierarchyChanges))
			{
				return base.MrsProxy.ISourceMailbox_EnumerateHierarchyChanges2(base.Handle, (int)flags, maxChanges);
			}
			if (maxChanges != 0)
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "ISourceMailbox_EnumerateHierarchyChanges2");
			}
			return base.MrsProxy.ISourceMailbox_EnumerateHierarchyChanges(base.Handle, flags.HasFlag(EnumerateHierarchyChangesFlags.Catchup));
		}

		ISourceFolder ISourceMailbox.GetFolder(byte[] entryId)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			base.VerifyMailboxConnection();
			long num = base.MrsProxy.ISourceMailbox_GetFolder(base.Handle, entryId);
			if (num == 0L)
			{
				return null;
			}
			return new RemoteSourceFolder(base.MrsProxy, num, entryId, this);
		}

		byte[] ISourceMailbox.GetMailboxBasicInfo(MailboxSignatureFlags flags)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.GetMailboxBasicInfo({0})", new object[]
			{
				flags
			});
			base.VerifyMailboxConnection();
			if (!base.ServerVersion[24])
			{
				return base.MrsProxy.ISourceMailbox_GetMailboxBasicInfo(base.Handle);
			}
			return base.MrsProxy.ISourceMailbox_GetMailboxBasicInfo2(base.Handle, (int)flags);
		}

		void ISourceMailbox.ExportMessages(List<MessageRec> messages, IFxProxyPool proxyPool, ExportMessagesFlags flags, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.ExportMessages({0} messages)", new object[]
			{
				messages.Count
			});
			if (propsToCopyExplicitly != null)
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "ISourceMailbox_ExportMessages");
			}
			base.VerifyMailboxConnection();
			using (IDataImport dataImport = new FxProxyPoolReceiver(proxyPool, false))
			{
				IDataMessage dataMessage = new FxProxyPoolGetFolderDataResponseMessage(proxyPool.GetFolderData());
				DataMessageOpcode dataMessageOpcode;
				byte[] targetObjectData;
				dataMessage.Serialize(base.MrsProxyClient.UseCompression, out dataMessageOpcode, out targetObjectData);
				DataExportBatch dataExportBatch;
				if (!base.ServerVersion[8])
				{
					if ((flags & ExportMessagesFlags.OneByOne) != ExportMessagesFlags.None || excludeProps != null)
					{
						throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "ISourceMailbox_ExportMessages");
					}
					dataExportBatch = base.MrsProxy.ISourceMailbox_ExportMessageBatch2(base.Handle, messages, targetObjectData);
				}
				else
				{
					int[] data = DataConverter<PropTagConverter, PropTag, int>.GetData(excludeProps);
					dataExportBatch = base.MrsProxy.ISourceMailbox_ExportMessages(base.Handle, messages, (int)flags, data, targetObjectData);
				}
				MessageExportResultReceiver messageExportResultReceiver = null;
				IDataImport destination = dataImport;
				if (!base.ServerVersion[16])
				{
					messageExportResultReceiver = new MessageExportResultReceiver(destination, false);
					destination = messageExportResultReceiver;
				}
				using (messageExportResultReceiver)
				{
					using (BufferedReceiver bufferedReceiver = new BufferedReceiver(destination, false, base.MrsProxyClient.UseBuffering, base.MrsProxyClient.UseCompression))
					{
						RemoteDataExport.ExportRoutine(base.MrsProxy, dataExportBatch.DataExportHandle, bufferedReceiver, dataExportBatch, base.MrsProxyClient.UseCompression);
						if (messageExportResultReceiver != null)
						{
							List<BadMessageRec> badMessages = messageExportResultReceiver.BadMessages;
							if (messageExportResultReceiver.MissingMessages != null)
							{
								foreach (MessageRec msg in messageExportResultReceiver.MissingMessages)
								{
									badMessages.Add(BadMessageRec.MissingItem(msg));
								}
							}
							if (badMessages != null && badMessages.Count > 0)
							{
								throw new DownlevelBadItemsPermanentException(badMessages);
							}
						}
					}
				}
			}
		}

		void ISourceMailbox.ExportFolders(List<byte[]> folderIds, IFxProxyPool proxyPool, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.ExportFolders", new object[0]);
			base.VerifyMailboxConnection();
			if (!base.ServerVersion[54])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "ISourceMailbox_ExportFolders");
			}
			DataExportBatch dataExportBatch = base.MrsProxy.ISourceMailbox_ExportFolders(base.Handle, folderIds, (int)exportFoldersDataToCopyFlags, (int)folderRecFlags, DataConverter<PropTagConverter, PropTag, int>.GetData(additionalFolderRecProps), (int)copyPropertiesFlags, DataConverter<PropTagConverter, PropTag, int>.GetData(excludeProps), (int)extendedAclFlags);
			using (IDataImport dataImport = new FxProxyPoolReceiver(proxyPool, false))
			{
				using (BufferedReceiver bufferedReceiver = new BufferedReceiver(dataImport, false, base.MrsProxyClient.UseBuffering, base.MrsProxyClient.UseCompression))
				{
					RemoteDataExport.ExportRoutine(base.MrsProxy, dataExportBatch.DataExportHandle, bufferedReceiver, dataExportBatch, base.MrsProxyClient.UseCompression);
				}
			}
		}

		void ISourceMailbox.CopyTo(IFxProxy destMailbox, PropTag[] excludeProps)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.CopyTo", new object[0]);
			base.VerifyMailboxConnection();
			byte[] objectData = destMailbox.GetObjectData();
			DataExportBatch dataExportBatch = base.MrsProxy.ISourceMailbox_Export2(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(excludeProps), objectData);
			using (FxProxyReceiver fxProxyReceiver = new FxProxyReceiver(destMailbox, false))
			{
				using (BufferedReceiver bufferedReceiver = new BufferedReceiver(fxProxyReceiver, false, base.MrsProxyClient.UseBuffering, base.MrsProxyClient.UseCompression))
				{
					RemoteDataExport.ExportRoutine(base.MrsProxy, dataExportBatch.DataExportHandle, bufferedReceiver, dataExportBatch, base.MrsProxyClient.UseCompression);
				}
			}
		}

		void ISourceMailbox.SetMailboxSyncState(string syncState)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.SetMailboxSyncState", new object[0]);
			base.VerifyMailboxConnection();
			IDataExport dataExport = new PagedTransmitter(syncState, base.MrsProxyClient.UseCompression);
			DataExportBatch dataExportBatch = dataExport.ExportData();
			long handle = base.MrsProxy.ISourceMailbox_SetMailboxSyncState(base.Handle, dataExportBatch);
			if (!dataExportBatch.IsLastBatch)
			{
				using (IDataImport dataImport = new RemoteDataImport(base.MrsProxy, handle, null))
				{
					do
					{
						dataExportBatch = dataExport.ExportData();
						IDataMessage message = DataMessageSerializer.Deserialize(dataExportBatch.Opcode, dataExportBatch.Data, base.MrsProxyClient.UseCompression);
						dataImport.SendMessage(message);
					}
					while (!dataExportBatch.IsLastBatch);
				}
			}
		}

		string ISourceMailbox.GetMailboxSyncState()
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.GetMailboxSyncState", new object[0]);
			base.VerifyMailboxConnection();
			DataExportBatch dataExportBatch = base.MrsProxy.ISourceMailbox_GetMailboxSyncState(base.Handle);
			string syncState = null;
			using (PagedReceiver pagedReceiver = new PagedReceiver(delegate(string data)
			{
				syncState = data;
			}, base.MrsProxyClient.UseCompression))
			{
				RemoteDataExport.ExportRoutine(base.MrsProxy, dataExportBatch.DataExportHandle, pagedReceiver, dataExportBatch, base.MrsProxyClient.UseCompression);
			}
			return syncState;
		}

		List<ReplayActionResult> ISourceMailbox.ReplayActions(List<ReplayAction> actions)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.ReplayActions", new object[0]);
			if (!((IMailbox)this).IsMailboxCapabilitySupported(MailboxCapabilities.ReplayActions))
			{
				MrsTracer.ProxyClient.Debug("Downlevel server does not support ReplayActions call.", new object[0]);
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "ISourceMailbox_ReplayActions");
			}
			base.VerifyMailboxConnection();
			return base.MrsProxy.ISourceMailbox_ReplayActions(base.Handle, actions);
		}
	}
}
